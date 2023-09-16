using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.Models.Models.Request
{
    public class FinmaksExchangeRatesRequest
    {
        public string Key { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ToQueryString()
        {
            return $"?key={Key}&startDate={StartDate.ToString("yyyy-MM-dd")}&endDate={EndDate.ToString("yyyy-MM-dd")}";
        }
    }
}
