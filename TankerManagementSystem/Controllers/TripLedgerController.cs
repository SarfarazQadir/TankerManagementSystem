using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerManagementSystem.Models;

public class TripLedgerController : Controller
{
    private readonly TankerDbContext _db;

    public TripLedgerController(TankerDbContext db)
    {
        _db = db;
    }

    // LIST
    public IActionResult Index()
    {
        var data = _db.TripLedgers
            .Include(x => x.Product)
            .Include(x => x.TripEntryFk)
            .OrderByDescending(x => x.Id)
            .ToList();

        return View(data);
    }

    // ADD GET
    /*  public IActionResult AddLedger()
      {
          ViewBag.Products = _db.Products.ToList();
          ViewBag.Trips = _db.TripEntries.ToList();
          return View();
      }*/
    public IActionResult AddLedger(int tripId)
    {
        var trip = _db.TripEntries
            .Include(x => x.TankerFk)
            .FirstOrDefault(x => x.Id == tripId);

        if (trip == null) return NotFound();

        ViewBag.Trip = trip;
        ViewBag.Products = _db.Products.ToList();
        ViewBag.TankerPreviousBalance = trip.TankerFk?.PreviousBalance ?? 0;

        return View();
    }

    // ADD POST
    /*   [HttpPost]
       public IActionResult AddLedger(TripLedger model, List<TripExpense> expenses)
       {
           var tz = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");

           model.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
           var sessionValue = HttpContext.Session.GetString("admin_session");

           // 🔴 UNIQUE TOKEN CHECK
           if (_db.TripLedgers.Any(x => x.TokenNo == model.TokenNo))
           {
               ModelState.AddModelError("TokenNo", "Token No already exists.");
           }
               if (!string.IsNullOrEmpty(sessionValue))
           {
               model.CreatedBy = int.Parse(sessionValue);
           }
           if (model.Remarks == null)
           {
               model.Remarks = "N/A";
           }
           // GRAND TOTAL CALCULATION
           decimal totalExpense = expenses.Sum(x => x.Amount);

           model.GrandTotal =
               model.Freight
               - model.AdvanceCash
               - model.Shortage
               - model.Commission
               - model.Munshiana
               - totalExpense
               + model.PreviousBalance;

           _db.TripLedgers.Add(model);
           _db.SaveChanges();

           // SAVE EXPENSES
           foreach (var item in expenses)
           {
               item.TripLedgerId = model.Id;
               _db.TripExpenses.Add(item);
           }

           _db.SaveChanges();

           TempData["add_ledger"] = "Ledger Added Successfully";
           return RedirectToAction("Index");
       }
   */
    // working
    /*    [HttpPost]
        public IActionResult AddLedger(TripLedger model, List<TripExpense> expenses, decimal PayingAmount)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            model.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            // 🔴 UNIQUE TOKEN CHECK
            if (_db.TripLedgers.Any(x => x.TokenNo == model.TokenNo))
            {
                ModelState.AddModelError("TokenNo", "Token No already exists.");
            }

            // 👉 Trip + Tanker fetch
            var trip = _db.TripEntries
                .Include(x => x.TankerFk)
                .FirstOrDefault(x => x.Id == model.TripEntryId);

            if (trip == null) return NotFound();

            var tanker = trip.TankerFk;

            // 👉 Previous Balance tanker se lo
            model.PreviousBalance = tanker.PreviousBalance;

            // ✅ 👉 VALIDATION (IMPORTANT)
            if (PayingAmount > tanker.PreviousBalance)
            {
                ModelState.AddModelError("", "Amount exceeds tanker balance");

                ViewBag.Trip = trip;
                ViewBag.Products = _db.Products.ToList();

                return View(model);
            }
            if (model.Remarks == null)
            {
                model.Remarks = "N/A";
            }
            // 👉 Expenses total
            decimal totalExpense = expenses?.Sum(x => x.Amount) ?? 0;

            // 👉 Grand Total (ledger ke liye only)
            model.GrandTotal =
                model.Freight
                - model.AdvanceCash
                - model.Shortage
                - model.Commission
                - model.Munshiana
                - PayingAmount
                - totalExpense;

            _db.TripLedgers.Add(model);
            // 🔥 👉 Sirf Paying Amount minus hoga tanker se
            tanker.PreviousBalance -= PayingAmount;
            _db.SaveChanges();

            // 👉 Save expenses
            if (expenses != null)
            {
                foreach (var item in expenses)
                {
                    item.TripLedgerId = model.Id;
                    _db.TripExpenses.Add(item);
                }
                _db.SaveChanges();
            }

            TempData["add_ledger"] = "Ledger Added & Tanker Balance Updated";
            return RedirectToAction("Index");
        }
    */

    [HttpPost]
    public IActionResult AddLedger(TripLedger model, List<TripExpense> expenses, decimal PayingAmount)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        model.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

        // 🔴 UNIQUE TOKEN CHECK
        if (_db.TripLedgers.Any(x => x.TokenNo == model.TokenNo))
        {
            ModelState.AddModelError("TokenNo", "Token No already exists.");
            return View(model);
        }

        // 👉 Trip + Tanker fetch
        var trip = _db.TripEntries
            .Include(x => x.TankerFk)
            .FirstOrDefault(x => x.Id == model.TripEntryId);

        if (trip == null) return NotFound();

        var tanker = trip.TankerFk;

        // 👉 View wapas bhejne ke liye (agar error aaye)
        ViewBag.Trip = trip;
        ViewBag.Products = _db.Products.ToList();
        ViewBag.TankerPreviousBalance = tanker?.PreviousBalance ?? 0;

        // 👉 Validation: PayingAmount > balance
        if (PayingAmount > tanker.PreviousBalance)
        {
            ModelState.AddModelError("", "Amount exceeds tanker balance");
            return View(model);
        }

        // 👉 Default remarks
        if (string.IsNullOrEmpty(model.Remarks))
        {
            model.Remarks = "N/A";
        }

        // 👉 Commission % fetch
        var commissionSetup = _db.CommissionSetups
            .FirstOrDefault(x => x.IsActive);

        decimal commissionPercent = commissionSetup?.Percentage ?? 0;

        // 👉 Commission calculate
        model.Commission = (model.Freight * commissionPercent) / 100;

        // 👉 Expenses total
        decimal totalExpense = expenses?.Sum(x => x.Amount) ?? 0;

        // 👉 Grand Total
        model.GrandTotal =
            model.Freight
            - model.AdvanceCash
            - model.Shortage
            - model.Commission
            - model.Munshiana
            - PayingAmount
            - totalExpense;

        // 👉 Ledger me store karo (kitna pay hua)
        model.PreviousBalance = PayingAmount;

        // 👉 Tanker balance update (DB me)
        tanker.PreviousBalance -= PayingAmount;

        // 👉 Save Ledger
        _db.TripLedgers.Add(model);
        _db.SaveChanges();

        // 👉 Save Expenses
        if (expenses != null && expenses.Count > 0)
        {
            foreach (var item in expenses)
            {
                item.TripLedgerId = model.Id;
                _db.TripExpenses.Add(item);
            }
            _db.SaveChanges();
        }

        TempData["add_ledger"] = "Ledger Added & Tanker Balance Updated";
        return RedirectToAction("Index");
    }
    // EDIT GET
    public IActionResult EditLedger(int id)
    {
        var data = _db.TripLedgers
            .Include(x => x.Expenses)
            .FirstOrDefault(x => x.Id == id);

        ViewBag.Products = _db.Products.ToList();
        ViewBag.Trips = _db.TripEntries.ToList();

        return View(data);
    }

    // EDIT POST
    [HttpPost]
    public IActionResult EditLedger(TripLedger model, List<TripExpense> expenses)
    {
        var ledger = _db.TripLedgers.FirstOrDefault(x => x.Id == model.Id);

        ledger.Freight = model.Freight;
        ledger.AdvanceCash = model.AdvanceCash;
        ledger.Shortage = model.Shortage;
        ledger.Commission = model.Commission;
        ledger.Munshiana = model.Munshiana;
        ledger.PreviousBalance = model.PreviousBalance;

        // REMOVE OLD EXPENSES
        var old = _db.TripExpenses.Where(x => x.TripLedgerId == ledger.Id);
        _db.TripExpenses.RemoveRange(old);

        // ADD NEW
        decimal totalExpense = 0;

        foreach (var item in expenses)
        {
            item.TripLedgerId = ledger.Id;
            totalExpense += item.Amount;
            _db.TripExpenses.Add(item);
        }

        ledger.GrandTotal =
            ledger.Freight
            - ledger.AdvanceCash
            - ledger.Shortage
            - ledger.Commission
            - ledger.Munshiana
            - totalExpense
            + ledger.PreviousBalance.Value;

        _db.SaveChanges();

        TempData["edit_ledger"] = "Ledger Updated";
        return RedirectToAction("Index");
    }

   /* public IActionResult PrintBill(int id)
    {
        var data = _db.TripLedgers
            .Include(x => x.TripEntryFk)
                .ThenInclude(t => t.TankerFk)
            .Include(x => x.Product)
            .Include(x => x.Expenses)
            .FirstOrDefault(x => x.Id == id);

        return View(data);
    }*/
    public IActionResult PrintBill(int id)
    {
        var data = _db.TripLedgers
            .Include(x => x.TripEntryFk)
                .ThenInclude(t => t.TankerFk)
            .Include(x => x.Product)
            .Include(x => x.Expenses)
            .FirstOrDefault(x => x.Id == id);

        if (data == null) return NotFound();

        return View(data);
    }

}