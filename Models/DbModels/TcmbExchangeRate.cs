using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//veri tabanımı bu sınıfla ilişkilendirdim
namespace Entities.DbModels
{
    [Table("tcmb_exchange_rate")]
    public class TcmbExchangeRate
    {
        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("CurrencyCode")]
        public string CurrencyCode { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("Value")]
        public decimal Value { get; set; }

    }
}
