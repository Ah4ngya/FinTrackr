using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace FinancialTrackr.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public double BudgetLimit { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [XmlIgnore]
        public User User { get; set; }
    }
}
