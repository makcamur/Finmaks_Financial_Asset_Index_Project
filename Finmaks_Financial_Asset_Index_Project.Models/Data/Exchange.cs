using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.Models.Models
{
    public class Exchange
    {
        public int ID { get; set; }
        public CurrencyCode BaseCurrencyCode { get; set; }
        public CurrencyCode ForeignCurrencyCode { get; set; }
        public decimal CashChangeRate { get; set; }
        public decimal CashExchangeRate { get; set; }
        public decimal CentralBankChangeRate { get; set; }
        public decimal CentralBankExchangeRate { get; set; }
        public decimal CrossRate { get; set; }
        public DateTime CurrentDate { get; set; }

        //navigation property
        public int AssetId { get; set; }
        public Asset? Asset { get; set; }

    }
}
