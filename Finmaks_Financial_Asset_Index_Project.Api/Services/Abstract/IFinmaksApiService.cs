using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;

namespace Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract
{
    public interface IFinmaksApiService
    {
        public Task<FinmaksExchangeRatesResponse> GetFinmaksExchangeRates(DateTime? startDate);
        public void MakeExchangesUpToDate(DateTime? lastdate);
        public void MakeExchangesUpToDateLive(DateTime? lastdate);
        public DateTime FindLastDate();
        public AssetResultDTO GetAsset(DataDTO data);
        public IndexResultDTO GetIndex(DataDTO data);
        public ExchangeResultDTO GetExchange(AssetResultDTO asset);
        public AssetIndexExchangeFinalTableDTO CalculateFinalTable(AssetResultDTO asset, IndexResultDTO ındex,ExchangeResultDTO exchange);

        public Dictionary<DateTime, decimal?> Birlestir(DataTable dataTable);
        public IndexResultDTO GetIndicesFromDataTable(DataTable dataTable);



     }
}
