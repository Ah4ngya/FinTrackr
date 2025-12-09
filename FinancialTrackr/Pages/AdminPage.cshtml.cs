using FinancialTrackr.Data;
using FinancialTrackr.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace FinancialTrackr.Pages
{
    public class AdminPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminPageModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public User User { get; set; }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string NewUsername { get; set; }
        [BindProperty]
        public string NewEmail { get; set; }
        [BindProperty]
        public string ChartDataJson { get; set; } = "{}";


        public int isUserAnAdmin { get; set; }
        public List<User> Users { get; set; } = new();
        public ICollection<Expense> Expenses { get; set; }

        public IActionResult OnGet()

        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null || currentUser.IsAdmin == 0)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }

            FullName = currentUser.FullName;

            Users = _context.Users
                .Include(u => u.Expenses)
                .ToList();
            var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var userSpendingByMonth = _context.Users
                .Select(u => new {
                    username = u.Username,
                    total = _context.Expenses
                        .Where(e => e.UserId == u.Id && e.date >= currentMonth)
                        .Sum(e => (double?)e.ExpenseValue) ?? 0
                })
                .ToList();

            var userSpendingByCategory = _context.Expenses
                .Where(e => e.date >= currentMonth)
                .GroupBy(e => e.ExpenseType)
                .Select(g => new { category = g.Key, total = g.Sum(e => e.ExpenseValue) })
                .ToList();

            var userSpendingByPayment = _context.Expenses
                .Where(e => e.date >= currentMonth)
                .GroupBy(e => e.PaymentMethod)
                .Select(g => new { method = g.Key, total = g.Sum(e => e.ExpenseValue) })
                .ToList();

            var chartData = new
            {
                userSpendingByMonth,
                userSpendingByCategory,
                userSpendingByPayment
            };

            ChartDataJson = JsonSerializer.Serialize(chartData);


            return Page();

            

        }
        public IActionResult OnPostSelectUser()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null || currentUser.IsAdmin == 0)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }
            if (string.IsNullOrWhiteSpace(Username))
            {
                TempData["Success"] = "Az összes felhasználó listázva.";
                Users = _context.Users
                    .Include(u => u.Expenses)
                    .OrderBy(u => u.Username)
                    .ToList();
                return Page();
            }

            Users = _context.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{Username}%"))
                .Include(u => u.Expenses)
                .OrderBy(u => u.Username)
                .ToList();


            if (!Users.Any())
                TempData["Error"] = "Nem található ilyen felhasználó.";
            else
                TempData["Success"] = $"{Users.Count} találat a(z) '{Username}' kifejezésre.";

            return Page();
        }
        public IActionResult OnPostEditUserCredentials()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null || currentUser.IsAdmin == 0)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }

            var editedUser = _context.Users.FirstOrDefault(u => u.Username == Username);
            if (editedUser != null)
            {
                editedUser.Username = NewUsername;
                editedUser.Email = NewEmail;
                editedUser.IsAdmin = isUserAnAdmin;
                _context.SaveChanges();
                TempData["Success"] = "Felhasználó adatai frissítve!";

            }

            return RedirectToPage();
        }
        public IActionResult OnPostDeleteUser()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null || currentUser.IsAdmin == 0)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Login");
            }
            var DeletedUser = _context.Users.FirstOrDefault(u => u.Username == Username);
            if (DeletedUser != null)
            {
                _context.Users.Remove(DeletedUser);
                _context.SaveChanges();
                TempData["Success"] = "Felhasználó sikeresn törölve.";

            }

            return RedirectToPage();
        }
    }
}
