using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.Models.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public decimal AssetValue { get; set; }
        public DateTime Date { get; set; }
        public CurrencyCode AssetType { get; set; }

    }
}
