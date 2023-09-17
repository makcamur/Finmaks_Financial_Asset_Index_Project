﻿using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response;


namespace Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract
{
    public interface IFinmaksApiService
    {
        public Task<FinmaksExchangeRatesResponse> GetFinmaksExchangeRates(DateTime? startDate);
    }
}