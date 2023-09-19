using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Repository.Irepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Finmaks_Financial_Asset_Index_Project.Api.Services.Concrete
{
    public class FinmaksApiService : IFinmaksApiService
    {
        private readonly IUnitOfWorksRepository _unitOfWorksRepository;

        public FinmaksApiService(IUnitOfWorksRepository unitOfWorksRepository)
        {
            _unitOfWorksRepository = unitOfWorksRepository;
        }
        /// <summary>
        /// Database'de kayıtlı son tarihin bulunması ve son tarihten itibaren FinmaksApiden güncel verilerin çekilmesi ve database'e kaydedilmesi
        /// </summary>
        /// <param name="lastDate"></param>
        public void MakeExchangesUpToDate(DateTime? lastDate)
        {

            var allExchanges = _unitOfWorksRepository.ExchangeRepository.GetAll();
            var lastExchange = allExchanges.FirstOrDefault(x =>
                                                   x.CurrentDate.Year == lastDate.Value.Year &&
                                                   x.CurrentDate.Month == lastDate.Value.Month &&
                                                   x.CurrentDate.Day == lastDate.Value.Day);
            if (lastExchange == null)
            {
                //databasede kayıtlı son tarihin bulunması ve son tarihten itibaren güncel verilerin çekilmesi
                var startdate = FindLastDate();
                var updateExchanges = GetFinmaksExchangeRates(startdate);
                //db kayıt


                foreach (var item in updateExchanges.Result.ExchangeRates)
                {
                    var exchange = new DataAccess.Data.Entities.Exchange();
                    exchange.BaseCurrencyCode = (int)item.BaseCurrencyCode;
                    exchange.ForeignCurrencyCode = (int)item.ForeignCurrencyCode;
                    exchange.CashChangeRate = item.CashChangeRate;
                    exchange.CashExchangeRate = item.CashExchangeRate;
                    exchange.CentralBankChangeRate = item.CentralBankChangeRate;
                    exchange.CentralBankExchangeRate = item.CentralBankExchangeRate;
                    exchange.CrossRate = item.CrossRate;
                    exchange.CurrentDate = item.CurrentDate;
                    _unitOfWorksRepository.ExchangeRepository.Add(exchange);
                    _unitOfWorksRepository.Save();
                }



            }
            else if (lastExchange.CurrentDate.Date == DateTime.Now.Date)
            {
                var RemoveTodayExchanges = _unitOfWorksRepository.ExchangeRepository.GetAll().Where(x => x.CurrentDate.Date == DateTime.Now.Date);
                foreach (var item in RemoveTodayExchanges)
                {
                    _unitOfWorksRepository.ExchangeRepository.Remove(item);
                    _unitOfWorksRepository.Save();
                }
                MakeExchangesUpToDateLive(lastDate);
            }

        }
        /// <summary>
        /// Database' in son gün icin kur bilgilerinin anlık güncellenmesini sağlar .
        /// </summary>
        /// <param name="lastDate"></param>
        public void MakeExchangesUpToDateLive(DateTime? lastDate)
        {

            var allExchanges = _unitOfWorksRepository.ExchangeRepository.GetAll();
            var lastExchange = allExchanges.FirstOrDefault(x =>
                                                   x.CurrentDate.Year == lastDate.Value.Year &&
                                                   x.CurrentDate.Month == lastDate.Value.Month &&
                                                   x.CurrentDate.Day == lastDate.Value.Day);
            if (lastExchange == null)
            {
                //databasede kayıtlı son tarihin bulunması ve son tarihten itibaren güncel verilerin çekilmesi
                var startdate = FindLastDate();
                var updateExchanges = GetFinmaksExchangeRates(startdate);
                //db kayıt


                foreach (var item in updateExchanges.Result.ExchangeRates)
                {
                    var exchange = new DataAccess.Data.Entities.Exchange();
                    exchange.BaseCurrencyCode = (int)item.BaseCurrencyCode;
                    exchange.ForeignCurrencyCode = (int)item.ForeignCurrencyCode;
                    exchange.CashChangeRate = item.CashChangeRate;
                    exchange.CashExchangeRate = item.CashExchangeRate;
                    exchange.CentralBankChangeRate = item.CentralBankChangeRate;
                    exchange.CentralBankExchangeRate = item.CentralBankExchangeRate;
                    exchange.CrossRate = item.CrossRate;
                    exchange.CurrentDate = item.CurrentDate;
                    _unitOfWorksRepository.ExchangeRepository.Add(exchange);
                    _unitOfWorksRepository.Save();
                }
            }
        }
        /// <summary>
        /// Database'de kayıtlı son tarihin bulunması ve yoksa eğer 2021-12-01 tarihinden itibaren güncel verilerin çekilmesi
        /// </summary>
        /// <returns>DateTime</returns>
        public DateTime FindLastDate()
        {
            var allExchanges = _unitOfWorksRepository.ExchangeRepository.GetAll();
            var findLastDate = allExchanges.OrderByDescending(x => x.CurrentDate).FirstOrDefault();
            if (findLastDate == null)
            {
                var startdate = new DateTime(2021, 12, 01);
                return startdate.Date;
            }
            else
            {
                var startdate = findLastDate.CurrentDate.Date;
                return startdate;
            }

        }
        /// <summary>
        /// Finmaks API'den verilerin çekilmesi
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FinmaksExchangeRatesResponse> GetFinmaksExchangeRates(DateTime? startDate)
        {

            string apiUrl = "https://testapi.finmaks.com/ExchangeRates";

            var requestParams = new FinmaksExchangeRatesRequest
            {
                Key = "Finmaks123",
                StartDate = (DateTime)(startDate == null ? new DateTime(2023, 09, 01) : startDate),
                EndDate = DateTime.Now
            };
            apiUrl += requestParams.ToQueryString();
            var client = new HttpClient();
            var response = await client.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<FinmaksExchangeRatesResponse>(responseContent);
                if (result == null || result.Header.Status != 0)
                {
                    throw new Exception(
                                           $"Error occured while getting data from Finmaks API. Status Code: {response.StatusCode} - {response.ReasonPhrase} - {responseContent}");
                }

                return result;
            }
            else
            {
                throw new Exception(
                    $"Error occured while getting data from Finmaks API. Status Code: {response.StatusCode} - {response.ReasonPhrase} - {responseContent}");
            }
        }
        //public void GetAsset2(DataDTO data)
        //{
            
        //    var aa = table;
        //    var be = aa.Rows;
        //    Dictionary<string, string> dict = new Dictionary<string, string>();

        //    for (int i = 7; i < be.Count; i++)
        //    {
        //        be[i].ItemArray[0].ToString();
        //        dict.Add(be[i].ItemArray[0].ToString(), be[i].ItemArray[1].ToString());
        //    }
        //    var dict2 = dict;
        //}
        public AssetResultDTO GetAsset(DataDTO data)
        {
            AssetResultDTO result = new AssetResultDTO();
            try
            {
                DataTable table = new DataTable();
                if (data.FileAsset != null)
                {
                    using (var stream = data.FileAsset.OpenReadStream())
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();
                        package.Load(stream);
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            using (ExcelWorksheet workSheet = package.Workbook.Worksheets.First())
                            {
                                int noOfCol = workSheet.Dimension.End.Column;
                                int noOfRow = workSheet.Dimension.End.Row;
                                int rowIndex = 1;

                                for (int c = 1; c <= noOfCol; c++)
                                {
                                    table.Columns.Add(workSheet.Cells[rowIndex, c].Text);
                                }
                                rowIndex = 2;
                                for (int r = rowIndex; r <= noOfRow; r++)
                                {
                                    DataRow dr = table.NewRow();
                                    for (int c = 1; c <= noOfCol; c++)
                                    {
                                        dr[c - 1] = workSheet.Cells[r, c].Value;
                                    }
                                    table.Rows.Add(dr);
                                }
                            }
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "No Work Sheet available in Excel File";
                            return result; // Return early with an error result
                        }
                    }
                }

                // Your existing code for processing the asset data

                result.Success = true;
                result.Message = "Excel Successfully Converted to Data Table";
                result.Data = table; // Assign your data here
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            var test=result.Data;
            return result;
        }
        public IndexResultDTO GetIndex(DataDTO data)
        {
            IndexResultDTO result = new IndexResultDTO();
            try
            {
                DataTable table = new DataTable();
                if (data.FileIndex != null)
                {
                    using (var stream = data.FileIndex.OpenReadStream())
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();
                        package.Load(stream);
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            using (ExcelWorksheet workSheet = package.Workbook.Worksheets.First())
                            {
                                int noOfCol = workSheet.Dimension.End.Column;
                                int noOfRow = workSheet.Dimension.End.Row;
                           
                                int rowIndex = 7;

                                for (int c = 1; c <= noOfCol; c++)
                                {
                                    table.Columns.Add(workSheet.Cells[rowIndex, c].Text);
                                }
                                rowIndex++;

                                for (int r = rowIndex; r <= noOfRow; r++)
                                {
                                    DataRow dr = table.NewRow();
                                    for (int c = 1; c <= noOfCol; c++)
                                    {
                                        dr[c - 1] = workSheet.Cells[r, c].Value;
                                    }
                                    table.Rows.Add(dr);
                                }
                            }
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "No Work Sheet available in Excel File";
                            return result; 
                        }
                    }
                } 
                result.Success = true;
                result.Message = "Excel Successfully Converted to Data Table";
                result.Data = table; // Verileri burada atayın
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            var test = result.Data;
            return result;
        }
         



    }
}
