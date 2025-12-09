using FinancialTrackr.Data.Models;
using FinancialTrackr.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace FinancialTrackr.Pages
{
    public class AdminDVModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public AdminDVModel(ApplicationDbContext context)
        {
            _context = context;
        }
        #region properties
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }
        [BindProperty(SupportsGet = true)]

        public DateTime selectedMonth { get; set; }

        [BindProperty]
        public User User { get; set; }

        public List<Category> Categories { get; set; }

        public List<User> Users { get; set; }

        public List<Expense> Expenses { get; set; }

        //[BindProperty(SupportsGet = true)]
        public string ExpenseJson { get; set; } = "[]";

        #endregion

        #region handlers
        #region on get, handles the same as in datavisualization.cshtml.cs but it also looks for users to select
        public void OnGet(string? userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return;

            UserName = userName;
            int? userId = ValidateUser();
            if (userId == 0)
            {
                Response.Redirect("/Login");
                return;
            }

            var currentUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null || currentUser.IsAdmin == 0)
            {
                HttpContext.Session.Clear();
                Response.Redirect("/Login");
                return;
            }
            DateTime targetMonth;
            if (string.IsNullOrEmpty(selectedMonth.ToString()))
                targetMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            else
                targetMonth = DateTime.Parse(selectedMonth + "-01");

            DateTime nextMonth = targetMonth.AddMonths(1);



            Users = _context.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{UserName}%"))
                .Include(u => u.Expenses)
                .OrderBy(u => u.Username)
                .ToList();

            User = Users.FirstOrDefault();
            Expenses = User?.Expenses?.OrderBy(e => e.date).ToList() ?? new List<Expense>();

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
        }
        #endregion
        #endregion

        #region functions
        public int? ValidateUser() {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return 0;
            return userId;

        }
        #endregion
    }
}
