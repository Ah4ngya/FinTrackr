using FinancialTrackr.Data;
using FinancialTrackr.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace FinancialTrackr.Pages
{
    public class DataVisualizationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public List<Expense> Expenses { get; set; } = new();
        public string ExpenseJson { get; set; } = "[]";
        [BindProperty(SupportsGet = true)]

        public string selectedMonth { get; set; }

        public DataVisualizationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            // Default to current month if not provided
            DateTime targetMonth;
            if (string.IsNullOrEmpty(selectedMonth))
                targetMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            else
                targetMonth = DateTime.Parse(selectedMonth + "-01");

            DateTime nextMonth = targetMonth.AddMonths(1);

            Expenses = _context.Expenses
                .Where(e => e.UserId == userId && e.date >= targetMonth && e.date < nextMonth)
                .OrderBy(e => e.date)
                .ToList();

            var expenseData = Expenses.Select(e => new
            {
                e.ExpenseType,
                e.ExpenseValue,
                e.date
            }).ToList();

            ExpenseJson = JsonSerializer.Serialize(expenseData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return Page();
        }
        public IActionResult OnPostExportXml()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            var userExpenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.date)
                .ToList();

            if (userExpenses.Count == 0)
            {
                TempData["Error"] = "Nincsenek exportálható költségek.";
                return RedirectToPage();
            }

            var serializer = new XmlSerializer(typeof(List<Expense>));
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            {
                serializer.Serialize(writer, userExpenses);
            }
            memoryStream.Position = 0;

            string fileName = $"Expenses_{DateTime.Now:yyyyMMdd_HHmm}.xml";
            return File(memoryStream, "application/xml", fileName);
        }

    }
}
