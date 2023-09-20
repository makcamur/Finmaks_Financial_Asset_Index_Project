using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Entities;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Repository.Irepository;
using Hangfire.Annotations;
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
            var test = result.Data;
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

                                int rowIndex = 6;

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
            return result;
        }
        /// <summary>
        /// Önce verilen varlık (AssetResultDTO) üzerinde işlem yapılabilmesi için varlık tarihlerini hesaplar (CalculateAssetDate metodu kullanılır).
        /// Ardından, her bir tarih için (item) belirli bir döviz kuru almak için bir döngü kullanır.
        /// Bu döviz kurlarını almak için _unitOfWorksRepository.ExchangeRepository.Get metodu kullanılır. Bu metod, belirli bir tarih (item),
        /// temel para birimi kodu (BaseCurrencyCode) 1 (dolar) ve yabancı para birimi kodu (ForeignCurrencyCode) 56 (Türk Lirası) olan döviz
        /// kurlarını getirir ve bunları exchangeResultDTO.Data koleksiyonuna ekler.
        /// Eğer varlık tarihleri (assetDates) null ise veya bir hata oluşursa, metot hata mesajını ayarlar (exchangeResultDTO.Success = false ve
        /// exchangeResultDTO.ErrorMessage) ve işlemi sonlandırır.
        /// Eğer her şey başarılı bir şekilde tamamlanırsa, metot başarılı olduğunu belirtir (exchangeResultDTO.Success = true) ve bir başarı
        /// mesajı ayarlar (exchangeResultDTO.Message).
        /// Eğer herhangi bir hata oluşursa (örneğin, bir istisna fırlatılırsa), hata mesajını yakalar ve exchangeResultDTO içinde hata mesajını ayarlar.
        /// Sonuç olarak, bu metod bir varlık için belirli tarihlerde döviz kurlarını almayı amaçlar ve sonucu bir ExchangeResultDTO nesnesi içinde döner.
        /// Başlangıç para birimi kodu 1 (dolar) ve hedef para birimi kodu 56 (Türk Lirası) olarak belirlenmiştir.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public ExchangeResultDTO GetExchange(AssetResultDTO asset)
        {
            var assetDates = CalculateAssetDate(asset);
            ExchangeResultDTO exchangeResultDTO = new ExchangeResultDTO();

            try
            {
                if (assetDates != null)
                {
                    for (int i = 0; i < assetDates.Count; i++)
                    {
                        var exchangeExcel = _unitOfWorksRepository.ExchangeRepository.Get(x => x.CurrentDate.Month == assetDates[i].Month && x.CurrentDate.Day == assetDates[i].Day && x.CurrentDate.Year == assetDates[i].Year && x.BaseCurrencyCode == 1 && x.ForeignCurrencyCode == 56);
                        if (exchangeExcel != null)
                        {
                            if (exchangeResultDTO.Data == null)
                            {
                                exchangeResultDTO.Data = new List<Exchange>();
                            }

                            exchangeResultDTO.Data.Add(exchangeExcel);
                        }
                    }
                }
                else
                {
                    exchangeResultDTO.Success = false;
                    exchangeResultDTO.ErrorMessage = "Exchange matching with excel file might have some problem";
                    return exchangeResultDTO;
                }
                exchangeResultDTO.Success = true;
                exchangeResultDTO.Message = "Exchange Successfully Converted to Data Table";
            }
            catch (Exception ex)
            {
                exchangeResultDTO.Success = false;
                exchangeResultDTO.ErrorMessage = ex.Message;
            }
            return exchangeResultDTO;

        }
        public AssetIndexExchangeFinalTableDTO CalculateFinalTable(AssetResultDTO asset, IndexResultDTO ındex, ExchangeResultDTO exchange)
        {
            List<List<object>> assetExcelFileColumn = new List<List<object>>();

            for (int columnIndex = 0; columnIndex < asset.Data.Columns.Count; columnIndex++)
            {
                // Her sütunun verilerini saklamak için bir liste oluşturun
                List<object> columnData = new List<object>();

                for (int rowIndex = 0; rowIndex < asset.Data.Rows.Count; rowIndex++)
                {
                    // Sütundaki veriyi alın ve listeye ekleyin
                    object cellValue = asset.Data.Rows[rowIndex][columnIndex];
                    columnData.Add(cellValue);
                }

                // Sütunun verilerini genel listeye ekleyin
                assetExcelFileColumn.Add(columnData);
            }

            AssetIndexExchangeFinalTableDTO dto = new AssetIndexExchangeFinalTableDTO();


            // Final Table için tarih sütununun alınması
            dto.dateTimes = assetExcelFileColumn[0].Select(item =>
            {
                if (DateTime.TryParse((string?)item, out DateTime dateTime))
                {
                    return dateTime;
                }
                // Dönüşüm başarısız olursa varsayılan bir değer kullanabilirsiniz.
                // Örneğin, DateTime.MinValue veya null.
                return DateTime.MinValue;
            }).ToList();


            // Final Table için varlık tarih varlık  tutarları sütununun alınması
            dto.Assets = assetExcelFileColumn[1].Select(item =>
            {
                if (decimal.TryParse((string?)item, out decimal decimalValue))
                {
                    return decimalValue;
                }
                else
                {
                    // Dönüşüm başarısız olduğunda nasıl bir işlem yapılacağını burada belirleyebilirsiniz.
                    // Örneğin, hata işleme veya varsayılan bir değer atama.
                    return 0; // Varsayılan değer
                }
            }).ToList();

            //Final Table için Önceki Aya Göre Varlık Artış Sütünunun alınması 
            dto.IncreaseInAssetsComparedToThePreviousMonth = new List<decimal>();

            for (int i = 0; i < dto.Assets.Count; i++)
            {
                if (i == 0)
                {
                    dto.IncreaseInAssetsComparedToThePreviousMonth.Add(0);
                }
                if (i != 0)
                {
                    var value = (dto.Assets[i] - dto.Assets[i - 1]) / dto.Assets[i - 1];
                    dto.IncreaseInAssetsComparedToThePreviousMonth.Add(value);
                }

            }
            // Final Table için Varlık Değişim Oranı Sütünunun alınması
            dto.AssetTurnoverRatio = new List<decimal>();
            for (int i = 0; i < dto.Assets.Count; i++)
            {
                var value = (dto.Assets[dto.Assets.Count - 1] - dto.Assets[i]) / dto.Assets[i];
                dto.AssetTurnoverRatio.Add(value);

            }
           

            // Final Table için Varlık Tarih Dolar Kuru Sütünunun alınması
            dto.AssetHistoricalExchangeRate = new List<decimal>();

            exchange.Data.ForEach(item =>
            {
                dto.AssetHistoricalExchangeRate.Add(item.CashExchangeRate);
            });

            //Final Table için Dolarizasyon Varlık Tutarı Sütünunun alınması
            dto.DollarizationAssetAmount = new List<decimal>();
            for (int i = 0; i < dto.Assets.Count; i++)
            {
                var value = dto.AssetHistoricalExchangeRate[dto.Assets.Count-1]/dto.AssetHistoricalExchangeRate[i]*dto.Assets[i];
                dto.DollarizationAssetAmount.Add(value);
            }
             

            //Final Table için Dolarizasyon Önceki Aya Göre Varlık Artış Sütünunun alınması
            dto.DollarizationIncreaseComparedToThePreviousMonth = new List<decimal>();
            for (int i = 0; i < dto.DollarizationAssetAmount.Count; i++)
            {
                if (i == 0)
                {
                    dto.DollarizationIncreaseComparedToThePreviousMonth.Add(0);
                }
                if (i != 0)
                {
                    var value = (dto.DollarizationAssetAmount[i] - dto.DollarizationAssetAmount[i - 1]) / dto.DollarizationAssetAmount[i - 1];
                    dto.DollarizationIncreaseComparedToThePreviousMonth.Add(value);
                }

            }

            //Final Table için Dolarizasyon Varlık Değişim Oranı Sütünunun alınması
            dto.DollarizationAssetTurnoverRate = new List<decimal>();
            for (int i = 0; i < dto.DollarizationAssetAmount.Count; i++)
            {
                var value = (dto.DollarizationAssetAmount[dto.DollarizationAssetAmount.Count - 1] - dto.DollarizationAssetAmount[i]) / dto.DollarizationAssetAmount[i];
                dto.DollarizationAssetTurnoverRate.Add(value);
            }
            //Final Table için Dolarizasyon Etkisi Yüzde Sütünunun alınması
            dto.DollarizationImpactPercentage = new List<decimal>();
            for (int i = 0; i < dto.DollarizationAssetAmount.Count; i++)
            {
                var value = (dto.Assets[i] - dto.DollarizationAssetAmount[i]) / dto.DollarizationAssetAmount[i];
                dto.DollarizationImpactPercentage.Add(value);
            }
            var test = dto;
            return dto;

        }
        public List<DateTime> CalculateAssetDate(AssetResultDTO asset)
        {
            List<List<object>> assetExcelFileColumn = new List<List<object>>();

            for (int columnIndex = 0; columnIndex < asset.Data.Columns.Count; columnIndex++)
            {
                // Her sütunun verilerini saklamak için bir liste oluşturun
                List<object> columnData = new List<object>();

                for (int rowIndex = 0; rowIndex < asset.Data.Rows.Count; rowIndex++)
                {
                    // Sütundaki veriyi alın ve listeye ekleyin
                    object cellValue = asset.Data.Rows[rowIndex][columnIndex];
                    columnData.Add(cellValue);
                }

                // Sütunun verilerini genel listeye ekleyin
                assetExcelFileColumn.Add(columnData);
            }

            AssetIndexExchangeFinalTableDTO dto = new AssetIndexExchangeFinalTableDTO();


            // assetExcelFileColumn[0]'ı dto.dateTimes'e eşitleme
            dto.dateTimes = assetExcelFileColumn[0].Select(item =>
            {
                if (DateTime.TryParse((string?)item, out DateTime dateTime))
                {
                    return dateTime;
                }
                // Dönüşüm başarısız olursa varsayılan bir değer kullanabilirsiniz.
                // Örneğin, DateTime.MinValue veya null.
                return DateTime.MinValue;
            }).ToList();

            return dto.dateTimes;
        }


    }
}
