using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialTrackr.Data.Models
{
    public class Income
    {
        public int Id { get; set; }
        public string IncomeType { get; set; }
        public double IncomeValue { get; set; }
        public string PaidIn { get; set; }
        public int UserId { get; set; }
        public DateTime date { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
