using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response
{
    public class FinmaksExchangeRateResponse
    {
        public CurrencyCode BaseCurrencyCode { get; set; }
        public CurrencyCode ForeignCurrencyCode { get; set; }
        public decimal CashChangeRate { get; set; }
        public decimal CashExchangeRate { get; set; }         
        public decimal CentralBankChangeRate { get; set; }
        public decimal CentralBankExchangeRate { get; set; }
        public decimal CrossRate { get; set; }
        public DateTime CurrentDate { get; set; }
     

    }
}
