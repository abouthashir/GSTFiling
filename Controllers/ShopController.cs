using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GSTFiling.Data;
using GSTFiling.Models;
using GSTFiling.Services;

namespace GSTFiling.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly OpenAiService _openAiService;

        public ShopController(AppDbContext context, OpenAiService openAiService)
        {
            _context = context;
            _openAiService = openAiService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(int? id, int? year, int? monthId, int? typeId)
        {
            Customer? customer;

            if (id.HasValue && id.Value > 0)
            {
                customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id.Value);
            }
            else
            {
                customer = await _context.Customers.OrderBy(c => c.Id).FirstOrDefaultAsync();
            }

            if (customer == null)
            {
                return NotFound("No customers found in the database. Please register a customer first.");
            }

            var invoices = await _context.Invoice
                .Where(i => i.cstmrid == customer.Id)
                .ToListAsync();

            var filings = await _context.GstFilings.OrderByDescending(x => x.UpdatedAt).ToListAsync();

            var gstDetailsMap = new Dictionary<int, string>();

            for (int i = 0; i < filings.Count; i++)
            {
                var filing = filings[i];
                var filingType = filing.GetType();

                // Safely read properties at runtime without compile-time dependency
                string filingMonth = filingType.GetProperty("Month")?.GetValue(filing)?.ToString()
                                  ?? filingType.GetProperty("month")?.GetValue(filing)?.ToString() ?? "";
                string filingYear = filingType.GetProperty("Year")?.GetValue(filing)?.ToString()
                                 ?? filingType.GetProperty("year")?.GetValue(filing)?.ToString() ?? "";

                var matched = invoices.Where(inv =>
                    (string.IsNullOrEmpty(filingYear) || inv.invyear.ToString() == filingYear) &&
                    (string.IsNullOrEmpty(filingMonth) || (inv.invmonth != null && inv.invmonth.Equals(filingMonth, StringComparison.OrdinalIgnoreCase)))
                ).ToList();

                // Fallback to match customer invoices if exact property mapping needs flexibility
                if (!matched.Any() && invoices.Any())
                {
                    matched = invoices;
                }

                if (matched.Any())
                {
                    decimal taxSum = matched.Sum(inv => inv.taxable_amt);
                    decimal gstSum = matched.Sum(inv => inv.gst_amt);
                    gstDetailsMap[i] = $"Taxable: ₹{taxSum:N2} | GST: ₹{gstSum:N2}";
                }
                else
                {
                    gstDetailsMap[i] = "-";
                }
            }

            ViewBag.GstDetailsMap = gstDetailsMap;
            ViewBag.GstFilings = filings;
            ViewBag.SelYear = year ?? DateTime.Now.Year;
            ViewBag.SelMonth = monthId ?? 0;
            ViewBag.SelType = typeId ?? 0;

            return View("Dashboard", customer);
        }

        [HttpGet]
        public async Task<IActionResult> Invoices(int cmid, int typeId = 0)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == cmid);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var invoices = await _context.Invoice
                .Where(i => i.cstmrid == cmid)
                .OrderByDescending(i => i.invid)
                .ToListAsync();

            ViewBag.Invoices = invoices;
            ViewBag.Cmid = cmid;
            ViewBag.TypeId = typeId;

            return View("Invoices", customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInvoice(
            int cmid, int typeId, string? invno, string? invdate,
            string? customerName, string? gstNumber,
            decimal taxrate, decimal taxable, decimal cgst, decimal sgst, decimal cess)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == cmid);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            decimal gstAmount = cgst + sgst + cess;
            decimal totalAmount = taxable + gstAmount;

            var invoice = new Invoice
            {
                cstmrid = cmid,
                invno = invno,
                invdate = invdate,
                taxable_amt = taxable,
                gst_amt = gstAmount,
                type = typeId.ToString(),
                invyear = DateTime.TryParse(invdate, out DateTime parsedDate) ? parsedDate.Year : DateTime.Now.Year,
                invmonth = DateTime.TryParse(invdate, out parsedDate) ? parsedDate.ToString("MMMM") : DateTime.Now.ToString("MMMM"),
                taxrate = taxrate,
                cgst = cgst,
                sgst = sgst,
                cess = cess,
                billamtstr = totalAmount.ToString("0.00")
            };

            _context.Invoice.Add(invoice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Invoices), new { cmid, typeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice(int id, int cmid, int typeId)
        {
            var invoice = await _context.Invoice.FirstOrDefaultAsync(i => i.invid == id && i.cstmrid == cmid);
            if (invoice == null)
            {
                return NotFound("Invoice not found.");
            }

            _context.Invoice.Remove(invoice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Invoices), new { cmid, typeId });
        }

        [HttpPost]
        public async Task<IActionResult> UploadAndReadInvoiceAjax(IFormFile? file, int cmid, int typeId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Please select an invoice file." });
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == cmid);
            if (customer == null)
            {
                return BadRequest(new { message = "Customer not found." });
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size must be less than 10 MB." });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".pdf")
            {
                return BadRequest(new { message = "Only JPG, JPEG, PNG and PDF files are supported." });
            }

            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");

            try
            {
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string? aiResponseJson = await _openAiService.AnalyzeDocumentAsync(tempFilePath, extension);

                if (string.IsNullOrWhiteSpace(aiResponseJson))
                {
                    return BadRequest(new { message = "AI returned an empty response." });
                }

                if (aiResponseJson.TrimStart().StartsWith("{\"error\""))
                {
                    using var errDoc = JsonDocument.Parse(aiResponseJson);
                    string errMsg = errDoc.RootElement.TryGetProperty("error", out var errProp) ? errProp.GetString() ?? "AI Error" : "AI Error";
                    return BadRequest(new { message = errMsg });
                }

                using var doc = JsonDocument.Parse(aiResponseJson);
                var root = doc.RootElement;

                string? invno = GetString(root, "invoice_number");
                string? invdate = GetString(root, "invoice_date");
                string? customerName = GetString(root, "buyer_name");
                string? gstNumber = GetString(root, "buyer_gstin");

                decimal taxable = GetDecimal(root, "total_taxable_amount");
                decimal cgst = GetDecimal(root, "total_cgst");
                decimal sgst = GetDecimal(root, "total_sgst");
                decimal cess = GetDecimal(root, "total_cess");
                decimal grandTotal = GetDecimal(root, "grand_total");

                if (taxable == 0 && root.TryGetProperty("items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in itemsElement.EnumerateArray())
                    {
                        taxable += GetDecimal(item, "taxable_amount");
                        cgst += GetDecimal(item, "cgst");
                        sgst += GetDecimal(item, "sgst");
                        cess += GetDecimal(item, "cess");
                    }
                }

                decimal taxrate = 0;
                if (root.TryGetProperty("items", out var itemArr) && itemArr.GetArrayLength() > 0)
                {
                    taxrate = GetDecimal(itemArr[0], "rate");
                }

                // Fallback tax rate bounding if extracted rate is invalid or out of standard range
                if (taxrate <= 0 || taxrate > 28)
                {
                    taxrate = 18;
                }


                decimal totalGst = cgst + sgst + cess;
                if (cess == 0 && taxrate == 28)
                {
                    cess = taxable * 0.12m;
                }
                // Automatically calculate CGST and SGST splits if they weren't explicitly separated by AI
                if ((cgst == 0 && sgst == 0) && taxable > 0 && taxrate > 0)
                {
                    decimal totalTaxAmount = taxable * (taxrate / 100);
                    cgst = totalTaxAmount / 2;
                    sgst = totalTaxAmount / 2;
                    totalGst = cgst + sgst + cess;
                }

                if (grandTotal == 0)
                {
                    grandTotal = taxable + totalGst;
                }

                var invoice = new Invoice
                {
                    cstmrid = cmid,
                    invno = invno ?? "N/A",
                    invdate = invdate ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    taxable_amt = taxable,
                    gst_amt = totalGst,
                    type = typeId.ToString(),
                    invyear = DateTime.TryParse(invdate, out DateTime parsedDate) ? parsedDate.Year : DateTime.Now.Year,
                    invmonth = DateTime.TryParse(invdate, out parsedDate) ? parsedDate.ToString("MMMM") : DateTime.Now.ToString("MMMM"),
                    taxrate = taxrate,
                    cgst = cgst,
                    sgst = sgst,
                    cess = cess,
                    billamtstr = grandTotal.ToString("0.00")
                };

                _context.Invoice.Add(invoice);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    invid = invoice.invid,
                    invno = invoice.invno,
                    invdate = invoice.invdate,
                    customerName,
                    gstNumber,
                    taxrate = invoice.taxrate,
                    taxable = invoice.taxable_amt,
                    cgst = invoice.cgst,
                    sgst = invoice.sgst,
                    cess = invoice.cess,
                    totalAmount = grandTotal,
                    message = "Invoice extracted and saved successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error while processing invoice: " + ex.Message });
            }
            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    try { System.IO.File.Delete(tempFilePath); } catch { }
                }
            }
        }

        private static string? GetString(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var element) && element.ValueKind == JsonValueKind.String)
            {
                return element.GetString();
            }
            return null;
        }

        private static decimal GetDecimal(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var element)) return 0;
            if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out decimal num)) return num;
            if (element.ValueKind == JsonValueKind.String && decimal.TryParse(element.GetString(), out decimal parsed)) return parsed;
            return 0;
        }
    }
}