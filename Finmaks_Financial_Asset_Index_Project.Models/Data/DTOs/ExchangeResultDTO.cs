using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs
{
    public class ExchangeResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Exchange>? Data { get; set; } = new List<Exchange>();
        public string ErrorMessage { get; set; }
    }
}
