using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract
{
    public interface IFinmaksApiService
    {
        public Task<FinmaksExchangeRatesResponse> GetFinmaksExchangeRates(DateTime? startDate);
        public void MakeExchangesUpToDate(DateTime? lastdate);
        public DateTime FindLastDate();


     }
}
