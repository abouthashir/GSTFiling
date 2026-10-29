using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GSTFiling.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private const string SYSTEM_PROMPT = @"You are an expert document reader that extracts structured details from Indian GST tax invoices and bills.
You MUST carefully read the entire document and return a single JSON object with these fields:

{
  ""invoice_number"": ""the invoice number or bill number"",
  ""invoice_date"": ""the date of the invoice"",
  ""order_number"": ""the order number if present, or null"",
  ""order_date"": ""the order date if present, or null"",
  ""place_of_supply"": ""the place of supply (state)"",
  ""nature_of_supply"": ""Goods or Services"",
  ""seller_name"": ""name of the seller / bill from company"",
  ""seller_address"": ""full address of the seller"",
  ""seller_gstin"": ""GSTIN of the seller (format: 29AAGCC4236P1Z5)"",
  ""buyer_name"": ""name of the buyer / bill to person or company"",
  ""buyer_address"": ""full address of the buyer"",
  ""buyer_gstin"": ""GSTIN of the buyer if available, or null"",
  ""items"": [
    {
      ""description"": ""product or service description"",
      ""hsn_sac"": ""HSN or SAC code if available"",
      ""qty"": ""quantity"",
      ""rate"": ""unit price or rate"",
      ""taxable_amount"": ""taxable amount before tax"",
      ""cgst"": ""CGST amount or 0"",
      ""sgst"": ""SGST/UGST amount or 0"",
      ""igst"": ""IGST amount or 0"",
      ""cess"": ""Cess amount or 0"",
      ""total"": ""total amount for this line item""
    }
  ],
  ""total_taxable_amount"": ""sum of all taxable amounts"",
  ""total_cgst"": ""total CGST"",
  ""total_sgst"": ""total SGST/UGST"",
  ""total_igst"": ""total IGST"",
  ""total_cess"": ""total Cess or 0"",
  ""grand_total"": ""the final grand total / invoice total amount"",
  ""amount_in_words"": ""total amount in words if present, or null"",
  ""reverse_charge"": ""Yes or No if mentioned, or null""
}

IMPORTANT RULES:
- Read every part of the document carefully including headers, tables, and footers.
- All monetary values must be numbers (not strings), e.g. 1234.56 not ""1234.56"".
- If a field is not found in the document, set it to null.
- The items array should contain every line item found in the invoice.
- For GSTIN, preserve the exact format found in the document.
- Return ONLY the JSON object, no extra text.";

        public OpenAiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            _apiKey = configuration["OpenAI:ApiKey"] ?? "";
        }

        public async Task<string?> AnalyzeDocumentAsync(string filePath, string fileExtension)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_OPENAI_API_KEY_HERE")
            {
                return "{\"error\": \"OpenAI API Key is not configured. Please add it to User Secrets.\"}";
            }

            fileExtension = fileExtension.ToLowerInvariant();

            try
            {
                if (fileExtension == ".pdf")
                {
                    string pdfText = ExtractTextFromPdf(filePath);
                    if (string.IsNullOrWhiteSpace(pdfText) || pdfText.Length < 30)
                    {
                        return "{\"error\": \"Could not extract text from the PDF. Please upload as JPG or PNG instead.\"}";
                    }
                    return await AnalyzeTextAsync(pdfText);
                }
                else if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                {
                    byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
                    string base64Image = Convert.ToBase64String(imageBytes);
                    string mimeType = fileExtension == ".png" ? "image/png" : "image/jpeg";
                    return await AnalyzeImageAsync(base64Image, mimeType);
                }
                else
                {
                    return "{\"error\": \"Unsupported file format. Please upload PDF, JPG, or PNG files.\"}";
                }
            }
            catch (Exception ex)
            {
                return $"{{\"error\": \"Exception during document processing: {EscapeJsonString(ex.Message)}\"}}";
            }
        }

        private static string ExtractTextFromPdf(string filePath)
        {
            try
            {
                using var document = UglyToad.PdfPig.PdfDocument.Open(filePath);
                var sb = new StringBuilder();
                foreach (var page in document.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        private async Task<string?> AnalyzeTextAsync(string text)
        {
            if (text.Length > 15000)
                text = text[..15000];

            var requestBody = new
            {
                model = "gpt-4o-mini",
                response_format = new { type = "json_object" },
                messages = new[]
                {
                    new { role = "system", content = SYSTEM_PROMPT },
                    new { role = "user", content = $"Extract all invoice details from the following text:\n\n{text}" }
                }
            };

            return await CallOpenAiApiAsync(requestBody);
        }

        private async Task<string?> AnalyzeImageAsync(string base64Image, string mimeType)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                response_format = new { type = "json_object" },
                messages = new object[]
                {
                    new { role = "system", content = SYSTEM_PROMPT },
                    new { role = "user", content = new object[] {
                        new { type = "text", text = "Extract all invoice details from the attached GST invoice/bill image." },
                        new { type = "image_url", image_url = new { url = $"data:{mimeType};base64,{base64Image}", detail = "high" } }
                    } }
                }
            };

            return await CallOpenAiApiAsync(requestBody);
        }

        private async Task<string?> CallOpenAiApiAsync(object requestBody)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            string jsonPayload = JsonSerializer.Serialize(requestBody);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return $"{{\"error\": \"OpenAI API returned status {response.StatusCode}: {EscapeJsonString(errorContent)}\"}}";
            }

            string responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }

        private static string EscapeJsonString(string s) =>
            s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }
}