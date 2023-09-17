using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Commons;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Entities
{
    public class Exchange:Base
    {
        public int BaseCurrencyCode { get; set; }
        public int ForeignCurrencyCode { get; set; }
        public decimal CashChangeRate { get; set; }
        public decimal CashExchangeRate { get; set; }
        public decimal CentralBankChangeRate { get; set; }
        public decimal CentralBankExchangeRate { get; set; }
        public decimal CrossRate { get; set; }
        public DateTime CurrentDate { get; set; }

        ////navigation property
        //public int AssetId { get; set; }
        //public Asset? Asset { get; set; }

    }
}
