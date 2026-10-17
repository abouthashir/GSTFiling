# Single Login (Design B) — Team Guide

Read this after the "Design B" pull request is merged into `main`.

## What changed

| Before | Now |
|---|---|
| Admin/Centre logins in `Users`, consultant logins in `Consultants`, shop logins in `Customers` | **Every** login is in `Users` |
| `Consultants.Password`, `Customers.Password` | **Removed** — passwords are only in `Users.Password` |
| — | `Users.ConsultantId` → links a login to `Consultants.Id` |
| — | `Users.ShopId` → links a login to `Customers.Id` |
| Duplicate emails possible | `Users.Email` is **unique** (the database refuses duplicates) |
| Role names typed by hand | `Role.Admin`, `Role.Centre`, `Role.Consultant`, `Role.Shop` (in `Models/Role.cs`) |

After login, these values are on the login cookie (claims):

| Role | Goes to | Claims you can read |
|---|---|---|
| Admin | `/Admin/Dashboard` | — |
| Centre | `/Firm/Dashboard` | `CentreId` |
| Consultant | `/Consultant/Dashboard` | `ConsultantId`, `CentreId` |
| Shop | `/Shop/Dashboard` | `ShopId`, `CentreId` |

## Test logins

| Email | Password | Role |
|---|---|---|
| `admin@gmail.com` | `1234` | Admin |
| `kochi@test.com` | `centre123` | Centre (Kochi) |
| `anu@test.com` | `test` | Consultant |
| `mart@test.com` | `test` | Shop |

---

## How to run a SQL query

Use **either** SQL Server Management Studio (SSMS) **or** Visual Studio.

**SSMS**
1. Open SSMS → **Connect to Server**: Server name `(localdb)\MSSQLLocalDB`, Authentication **Windows Authentication** → **Connect**.
2. Object Explorer → **Databases** → right-click → **Refresh** → right-click **GSTFilingDb** → **New Query**.
3. To run a file instead: **File → Open → File…** → choose `docs/seed-test-data.sql`.
4. Check the **database dropdown** in the toolbar says **GSTFilingDb** (not `master`).
5. Click **Execute** (or press **F5**). Read the **Messages** tab for errors.

**Visual Studio**
1. **View → SQL Server Object Explorer** → `(localdb)\MSSQLLocalDB` → **Databases** → right-click → **Refresh**.
2. Right-click **GSTFilingDb** → **New Query** → paste the SQL.
3. To run a file instead: open `docs/seed-test-data.sql` in Solution Explorer → click **Connect** (plug icon) in the file's toolbar → choose `(localdb)\MSSQLLocalDB` and database **GSTFilingDb**.
4. Check the **database dropdown** says **GSTFilingDb**.
5. Click **Execute** (▶) or press **Ctrl+Shift+E**.

⚠ Run `docs/seed-test-data.sql` **only on a new, empty database**. On a database that already has data you will get duplicate errors.

---

## Step 1 — Update your branch and database (everyone)

Do these **in this order**:

1. **Commit or Stash** your work (Git Changes → ▼ next to Commit → Stash All). Changes must be empty.
2. Switch to **main** → **Pull**.
3. Switch to **your branch** → Git → Manage Branches → right-click **main** → **Merge 'main' into '<your branch>'**.
4. If you stashed: Git Changes → Stashes → right-click → **Pop**.
5. **Build** (`Ctrl+Shift+B`). Fix any errors in YOUR code first (see Step 2). `Update-Database` will not run while the build fails.
6. Package Manager Console:
   ```
   Update-Database
   ```
   Expected: `Done.` Your old test consultants and shops are copied into `Users` automatically, with their old passwords.
7. Check in SQL Server Object Explorer → `GSTFilingDb` → New Query:
   ```sql
   SELECT Email, RoleId, CentreId, ConsultantId, ShopId FROM Users;
   ```
8. **Push** your branch.

**No database yet** (fresh clone)? Run `Update-Database`, then open `docs/seed-test-data.sql` and execute it once. It creates all the test data and the test logins above.

**Have consultants/shops but no Admin or Centre login?** Run:

```sql
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@gmail.com')
    INSERT INTO Users (UserName, Email, Password, RoleId, IsActive, CentreId, UpdatedAt)
    VALUES ('Admin', 'admin@gmail.com', '1234',
            (SELECT Id FROM Roles WHERE Name = 'Admin'), 1, NULL,
            CONVERT(nvarchar(50), GETDATE(), 120));

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'kochi@test.com')
    INSERT INTO Users (UserName, Email, Password, RoleId, IsActive, CentreId, UpdatedAt)
    VALUES ('Kochi Centre', 'kochi@test.com', 'centre123',
            (SELECT Id FROM Roles WHERE Name = 'Centre'), 1,
            (SELECT Id FROM Centres WHERE Name = 'Kochi Centre'),
            CONVERT(nvarchar(50), GETDATE(), 120));
```

---

## Step 2 — Fix your code if it used the old passwords

Search your code: **Ctrl+Shift+F** → `.Password`.

Any `consultant.Password` or `shop.Password` (on `Consultant` / `Customer`) no longer exists. Passwords are now read and written on the matching `Users` row:

```csharp
// The login row of a consultant
var login = await _db.Users.FirstOrDefaultAsync(u => u.ConsultantId == consultantId);

// The login row of a shop
var login = await _db.Users.FirstOrDefaultAsync(u => u.ShopId == shopId);
```

(Use your own DbContext variable name, e.g. `_context` instead of `_db`.)

---

## Step 3 — Protect your controller (each module owner)

At the top of your controller:

```csharp
using System.Security.Claims;
using GSTFiling.Models;
using Microsoft.AspNetCore.Authorization;
```

Above your class, and how to read "who is logged in":

| Controller | Attribute above the class | Read the logged-in id |
|---|---|---|
| `AdminController` | `[Authorize(Roles = Role.Admin)]` | — |
| `ConsultantController` | `[Authorize(Roles = Role.Consultant)]` | `int.Parse(User.FindFirstValue("ConsultantId")!)` |
| `ShopController` | `[Authorize(Roles = Role.Shop)]` | `int.Parse(User.FindFirstValue("ShopId")!)` |

Example (Consultant):

```csharp
    [Authorize(Roles = Role.Consultant)]
    public class ConsultantController : Controller
    {
        // ...

        private int GetCurrentConsultantId()
        {
            return int.Parse(User.FindFirstValue("ConsultantId")!);
        }

        public async Task<IActionResult> Dashboard()
        {
            int consultantId = GetCurrentConsultantId();

            // Only the shops assigned to THIS consultant
            var myShops = await _db.Customers
                .Where(s => s.ConsultantId == consultantId)
                .ToListAsync();

            return View(myShops);
        }
    }
```

**Rules:**
- Your controller **must** have a `Dashboard` action — the login sends your users there.
- Always filter data by the logged-in id (a consultant sees only their shops, a shop sees only its own invoices).
- Never type role names as text (`"Admin"`); use `Role.Admin`.

Test: log in with your role → your Dashboard opens. Log in with another role and open your URL → "Access denied". Logged out → sent to the login page.

---

## Step 4 — Admin only: "Register Centre" must also create the login

A new centre needs a `Centres` row **and** a `Users` row (role Centre). Save both together in one transaction, so you never get a centre without a login (or the other way round):

```csharp
        private async Task CreateCentreWithLoginAsync(Centre centre, string loginEmail, string password)
        {
            int centreRoleId = await _db.Roles
                .Where(r => r.Name == Role.Centre)
                .Select(r => r.Id)
                .FirstAsync();

            await using var transaction = await _db.Database.BeginTransactionAsync();

            _db.Centres.Add(centre);
            await _db.SaveChangesAsync();          // gives centre.Id

            _db.Users.Add(new User
            {
                UserName = centre.Name,
                Email = loginEmail.Trim().ToLower(),
                Password = password,
                RoleId = centreRoleId,
                IsActive = true,
                CentreId = centre.Id,
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();       // both rows saved, or neither
        }
```

Before calling it, check the email is free:

```csharp
bool emailTaken = await _db.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == loginEmail.Trim().ToLower());
```

Passwords can be at most **50 characters** (`Users.Password` is `MaxLength(50)`).

---

## Problems and fixes

| You see | Why | Fix |
|---|---|---|
| `Build failed.` when running `Update-Database` | Your code still uses something that changed (e.g. `.Password` on a consultant) | Fix the build errors (Step 2), then run it again |
| `No migrations were applied` | You did not merge `main` yet | Do Step 1, points 2–3 first |
| Error mentioning `IX_Users_Email` / `duplicate key` | Two accounts in your data have the same email | Change one email, run `Update-Database` again. Nothing is broken — the migration cancels itself |
| `Invalid object name` / `already exists` | Your database was made from different migrations | `Drop-Database` → `Update-Database` → run `docs/seed-test-data.sql` (deletes test data only) |
| Login says "Email or password is incorrect" for a consultant/shop | No `Users` row for them (e.g. your `Roles` table names are not exactly `Admin`, `Centre`, `Consultant`, `Shop`) | Fix the role names, then add their `Users` row (see `docs/seed-test-data.sql`, last part) |
| Red `Invalid column name 'ConsultantId'` in the SQL editor, but the query works | The SQL editor remembers the old table layout | Edit → IntelliSense → Refresh Local Cache (`Ctrl+Shift+R`) |

## Do not

- Do not run `Add-Migration` without telling the group first (only one migration at a time).
- Do not edit `Models/User.cs`, `Models/Role.cs`, `Program.cs` or `Migrations/*` without telling the group.
- Do not add a `Password` back to `Consultant` or `Customer`.
