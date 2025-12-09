using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace FinancialTrackr.Data.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string ExpenseType { get; set; }
        public double ExpenseValue { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime date { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }

        [ForeignKey("UserId")]
        [XmlIgnore]
        public User User { get; set; }
        [ForeignKey("CategoryId")]
        [XmlIgnore]
        public Category category { get; set; }
    }
}
