namespace FinancialTrackr.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int IsAdmin { get; set; }
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Income> Incomes { get; set; } = new List<Income>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<MonthlyBudget> Budgets { get; set; } = new List<MonthlyBudget>();
        public ICollection<PaymentMethods> PaymentMethod { get; set; } = new List<PaymentMethods>();
    }
}
