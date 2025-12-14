using FinancialTrackr.Data.Models;
using FinancialTrackr.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata;
using SQLitePCL;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FinancialTrackr.Pages
{


    public class UserpageModel : PageModel
    {
        #region private properties
        private readonly ApplicationDbContext _context;
        private readonly CurrencyServices _currencyServices;
        #endregion
        public string? FullName { get; set; }
        public double? BudgetForTheMonth { get; set; }
        public UserpageModel(ApplicationDbContext context, CurrencyServices currencyService)
        {
            _context = context;
            _currencyServices = currencyService;

        }
        #region properties
        [BindProperty]
        public double ExpenseValue { get; set; }
        [BindProperty]
        public string ExpenseType { get; set; }
        [BindProperty]
        public string PaymentMethod { get; set; }
        [BindProperty]
        public DateTime date { get; set; }
        [BindProperty]
        public double Budget { get; set; }
        [BindProperty]
        public DateTime Month { get; set; }
        [BindProperty]
        public string newCategory { get; set; }
        [BindProperty]
        public string newPaymentMethod { get; set; }
        [BindProperty]
        public int SelectedCategoryId { get; set; }
        [BindProperty]
        public double BudgetLimit { get; set; }


        public List<Expense> expenses { get; set; } = new();
        public List<Category> UserCategories { get; set; } = new();
        public List<PaymentMethods> paymentMethods { get; set; } = new();
        public Dictionary<string, decimal> ExchangeRates { get; set; } = new();
        public Dictionary<string, decimal> PreviousRates { get; set; } = new();
        public DateTime CurrentMonth { get; set; }
        #endregion

        #region handlers and calls

        #region user validation
        public int? ValidateUser()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return 0;
            return userId;
        }
        #endregion

        #region on get, contains most of loading logic
        public async Task<IActionResult> OnGet()
        {
            
            var prevRates = HttpContext.Session.GetString("PreviousRates");
            if (!string.IsNullOrEmpty(prevRates))
            {
                PreviousRates = JsonSerializer.Deserialize<Dictionary<string,decimal>>(prevRates);
            }
            ExchangeRates = await _currencyServices.GetExchangeRatesAsync("USD");
            var newRatesJson = JsonSerializer.Serialize(ExchangeRates);
            HttpContext.Session.SetString("PreviousRates", newRatesJson);

            //ExchangeRates = GetDemoRates(ExchangeRates);


            int? userId = ValidateUser();
            if (userId == 0)
            {
                return Redirect("/Login");
            }


            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }

            var defaultCategories = new[] { "Étel", "Lakhatás", "Utazás", "Szórakozás", "Egyéb" };

            foreach (var catName in defaultCategories)
            {
                bool exists = _context.Categories.Any(c => c.UserId == userId && c.Name == catName);
                if (!exists)
                {
                    _context.Categories.Add(new Category
                    {
                        UserId = userId.Value,
                        Name = catName,
                        BudgetLimit = 0
                    });
                }
            }
            _context.SaveChanges();

            CurrentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            FullName = user.FullName;

            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var userBudget = _context.Budgets
                .FirstOrDefault(b => b.UserId == userId && b.Month == currentMonth);

            if (userBudget != null)
                BudgetForTheMonth = userBudget.MonthlyBudgetAmount;
            else
                BudgetForTheMonth = null;

            expenses = _context.Expenses
               .Where(e => e.UserId == userId)
               .OrderBy(e => e.date)
               .ToList();

            
            UserCategories = _context.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToList();

            paymentMethods = _context.Methods
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToList();
            expenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.date)
                .ToList();

            return Page();


        }

        #endregion

        #region on post, contains the new expense adding logic and most of the warning logic
        public IActionResult OnPost()
        {
            int? userId = ValidateUser();
            if (userId == 0)
            {
                return Redirect("/Login");
            }
            var expense = new Expense
            {
                UserId = userId.Value,
                ExpenseType = ExpenseType,
                ExpenseValue = ExpenseValue,
                PaymentMethod = PaymentMethod,
                date = date
            };
            if (expense.ExpenseValue > 0)
            {
                _context.Expenses.Add(expense);
                _context.SaveChanges();
            }
            else
            {
                TempData["Warning"] = "Az új kiadás értéke nem lehet nulla!";
                return RedirectToPage();
            }
            

            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            double totalSpentThisMonth = _context.Expenses
                .Where(u => u.UserId == userId && u.date >= CurrentMonth)
                .Sum(u => u.ExpenseValue);

            var monthlyBudget = _context.Budgets
                .FirstOrDefault(u => u.UserId == userId && u.Month == currentMonth);

            if (monthlyBudget != null)
            {
                double limit = monthlyBudget.MonthlyBudgetAmount;
                double percentage = (totalSpentThisMonth / limit) * 100;

                if (percentage > 80 && percentage < 100)
                {
                    TempData["Warning"] = $" Figyelem! Már elköltötted a havi költségvetésed {percentage:F1}%-át.";
                }
                else if (percentage >= 100)
                {
                    {
                        TempData["Warning"] = $" Figyelem! Túllépted a havi költségvetésed! {percentage:F1}%-át.";

                    }
                }
            }
            var category = _context.Categories
                .FirstOrDefault(c => c.UserId == userId && c.Name == ExpenseType);

            if (category != null && category.BudgetLimit > 0)
            {
                double totalInCategory = _context.Expenses
                    .Where(e => e.UserId == userId && e.ExpenseType == category.Name)
                    .Sum(e => e.ExpenseValue);

                double percentUsed = (totalInCategory / category.BudgetLimit) * 100;

                if (percentUsed >= 100)
                    TempData["Warning"] = $"Túllépted a '{category.Name}' kategória keretét ({percentUsed:F1}%)!";
                else if (percentUsed >= 80)
                    TempData["Warning"] = $"Figyelem! A '{category.Name}' kategória {percentUsed:F1}% keretnél jár.";
            }

            expenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.date)
                .ToList();

            ModelState.Clear();

            ExpenseValue = 0;
            ExpenseType = string.Empty;
            PaymentMethod = string.Empty;
            date = DateTime.Today;

            return RedirectToPage();

        }
        #endregion

        #region export xml
        public IActionResult OnPostExportXml()
        {
            int? userId = ValidateUser();
            if (userId == 0)
            {
                return Redirect("/Login");
            }

            var UserExpenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.date).
                ToList();
            var serializer = new XmlSerializer(typeof(List<Expense>));
            var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, UserExpenses);
            memoryStream.Position = 0;

            return File(memoryStream, "application/xml", "MyExpenses.xml");


        }
        #endregion

        #region set monthly budget
        public IActionResult OnPostSetMonthlyBudget()
        {

            int? userId = ValidateUser();
            if (userId == 0)
            {
                return Redirect("/Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToPage("/Login");

            CurrentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var existingBudget = _context.Budgets
                .FirstOrDefault(b => b.UserId == userId && b.Month == CurrentMonth);

            if (existingBudget != null && Budget > 0)
            {

                existingBudget.MonthlyBudgetAmount = Budget;
                _context.SaveChanges();

                TempData["Success"] = "Havi Költségvetés sikeresen frissítve!";
            } else if (Budget <= 0) {
                TempData["Error"] = "A Havi Költségvetés összege nem lehet kisebb mint 0 vagy 0.";
            }
            else
            {

                var newBudget = new MonthlyBudget
                {
                    UserId = userId.Value,
                    Month = CurrentMonth,
                    MonthlyBudgetAmount = Budget
                };

                _context.Budgets.Add(newBudget);
                _context.SaveChanges();
                TempData["Success"] = "Havi Költségvetés sikeresen beállítva!";
            }

            return RedirectToPage();

        }
        #endregion

        #region add new category
        public IActionResult OnPostAddNewCategory()
        {

            int? userId = ValidateUser();
            if (userId == 0)
            {
                return Redirect("/Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToPage("/Login");

            var staticNames = new[] { "Food", "Rent", "Transport", "Entertainment", "Other" };

            if (staticNames.Contains(newCategory, StringComparer.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Ez a kategória már létezik az alap listában.";
                return RedirectToPage();
            }

            var category = _context.Categories.FirstOrDefault(c => c.UserId == userId && c.Name == newCategory);
            var existingCategories = _context.Categories
              .Where(c => c.UserId == userId)
              .Select(c => c.Name)
              .ToList();

            if (newCategory != null)
            {
                foreach (var cat in existingCategories)
                {
                    int distance = LevenshteinDistance(cat.ToLower(), newCategory.ToLower());
                    if (distance <= 2)
                    {
                        TempData["Warning"] = $"Figyelem! A(z) '{newCategory}' hasonlít a meglévõ '{cat}' kategóriához.";
                        return RedirectToPage();
                    }
                }

                if (category == null)
                {
                    var newUserCategory = new Category
                    {
                        UserId = userId.Value,
                        Name = newCategory,
                        BudgetLimit = Budget,

                    };
                    if (newCategory != null)
                    {
                        _context.Categories.Add(newUserCategory);
                        _context.SaveChanges();
                        TempData["Success"] = "Új kategória Sikeresen hozzáadva!";
                    }


                }
            }
            else
            {
                TempData["Warning"] = "A kategória nem lehet üres";
            }

                return RedirectToPage();

        }
        #endregion

        #region add new paymentmethod
        public IActionResult OnPostAddNewPaymentMethod()
        {

            int? userId = ValidateUser();
            if (userId == 0)
            {
                return Redirect("/Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToPage("/Login");

            var staticNames = new[] { "Készpénz", "Kártya","Crypto Valuta" };

            if (staticNames.Contains(newPaymentMethod, StringComparer.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Ez a kategória már létezik az alap listában.";
                return RedirectToPage();
            }

            var paymentmethod = _context.Methods.FirstOrDefault(u => u.UserId == userId && u.Name == newPaymentMethod);

            if (newPaymentMethod != null)
            {
                if (paymentmethod == null)
                {
                    var newUserPayment = new PaymentMethods
                    {

                        UserId = userId.Value,
                        Name = newPaymentMethod,

                    };

                    _context.Methods.Add(newUserPayment);
                    _context.SaveChanges();

                    TempData["Success"] = "Új Fizetõeszköz Sikeresen hozzáadva!";

                }
            }
            else
            {
                TempData["Error"] = "Az új fizetõeszköz felvételéhez kötelezõ megadni egy nevet!";
            }
            return RedirectToPage();
        }
        #endregion

        #region set category budget
        public IActionResult OnPostSetCategoryBudget()
        {
            int? userId = ValidateUser();
            if (userId == 0)
            {
                return Redirect("/Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToPage("/Login");

            var category = _context.Categories
            .FirstOrDefault(c => c.UserId == userId && c.CategoryId == SelectedCategoryId);

            if (category == null)
            {
                TempData["Error"] = "A kiválasztott kategória nem található!";
                return RedirectToPage();
            }

            if (BudgetLimit <= 0)
            {
                TempData["Error"] = "A kategória limit nem lehet nulla vagy negatív!";
                return RedirectToPage();
            }

            category.BudgetLimit = BudgetLimit;
            _context.SaveChanges();

            TempData["Success"] = $"A(z) '{category.Name}' kategória limitje sikeresen beállítva {category.BudgetLimit} Ft értékre!";
            return RedirectToPage();

           
        }
        #endregion

        #region currency service
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnGetRatesAsync()
        {

            var prevRates = HttpContext.Session.GetString("PreviousRates");
            if (!string.IsNullOrEmpty(prevRates))
            {
                PreviousRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(prevRates);
            }
            ExchangeRates = await _currencyServices.GetExchangeRatesAsync("USD");
            var newRatesJson = JsonSerializer.Serialize(ExchangeRates);
            HttpContext.Session.SetString("PreviousRates", newRatesJson);
            return new JsonResult(ExchangeRates);
        }
        #endregion
        #endregion

        #region levenshteindistance
        private int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            return d[s.Length, t.Length];
        }
        #endregion

        #region demo rates randomizer
        //public Dictionary<string, decimal> GetDemoRates(Dictionary<string, decimal> originalRates)
        //{
        //    var random = new Random();
        //    var demoRates = new Dictionary<string, decimal>();

        //    foreach (var kvp in originalRates)
        //    {
        //        var changePercent = (decimal)(random.NextDouble() * 0.06 - 0.03);
        //        var newValue = kvp.Value + kvp.Value * changePercent;
        //        demoRates[kvp.Key] = Math.Round(newValue, 4);
        //    }

        //    return demoRates;
        //}
        #endregion
    }
}
