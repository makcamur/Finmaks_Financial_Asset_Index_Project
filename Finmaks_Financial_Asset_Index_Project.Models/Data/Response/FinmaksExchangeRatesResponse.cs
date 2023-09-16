using Finmaks_Financial_Asset_Index_Project.Models.Models.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.Models.Models.Response
{
    public class FinmaksExchangeRatesResponse
    {
        public List<FinmaksExchangeRateResponse> ExchangeRates { get; set; }
        public Header Header { get; set; }

    }
}
