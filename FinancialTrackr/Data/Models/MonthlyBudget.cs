using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialTrackr.Data.Models
{
    public class MonthlyBudget
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public double MonthlyBudgetAmount { get; set; } 

        [Required]
        public int UserId { get; set; } 

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public DateTime Month { get; set; }
    }
}
