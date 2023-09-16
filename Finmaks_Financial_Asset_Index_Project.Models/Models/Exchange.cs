using Finmaks_Financial_Asset_Index_Project.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.Models.Models
{
    public class Exchange
    {
        public int Id { get; set; }
        public decimal ExchangeValue { get; set; }
        public DateTime Date { get; set; }
        public AssetType ExchangeType { get; set; }

        //navigation property
        public int AssetId { get; set; }
        public Asset? Asset { get; set; }

    }
}
