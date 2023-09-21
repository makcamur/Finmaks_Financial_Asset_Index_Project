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
        /// <summary>
        /// Bu fonksiyon, gelen bir DataDTO nesnesi içindeki Excel dosyasını okuyarak bu veriyi bir AssetResultDTO nesnesi içinde döndürmeyi amaçlar.
        /// Excel dosyasının içeriği bir veri tablosuna aktarılır ve işlev sonucunda, bu veri tablosu result.Data özelliği altında döndürülür.
        /// Fonksiyon, istisnai durumlar oluştuğunda hata mesajları ile birlikte AssetResultDTO nesnesini döndürür.
        /// </summary>
        /// <param name="data">Excel dosyasını içeren bir DataDTO nesnesi.</param>
        /// <returns>Excel dosyasının okunması ve işlenmesi sonucu elde edilen sonuçları içeren AssetResultDTO nesnesi.</returns>
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
                            return result; 
                        }
                    }
                }
                result.Success = true;
                result.Message = "Excel Successfully Converted to Data Table";
                result.Data = table; 
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            var test = result.Data;
            return result;
        }
        /// <summary>
        /// Spesifik Şekilde hazırlanmış olan Excel dosyasını okuyarak içeriğini bir DataTable'e dönüştüren metot.
        /// </summary>
        /// <param name="data">Excel dosyasının verilerini içeren DataDTO nesnesi.</param>
        /// <returns>IndexResultDTO nesnesi içinde işlem sonucu ve veri tablosu.</returns>
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
        /// Verilen varlık bilgilerine göre döviz değişim verilerini alır.
        /// </summary>
        /// <param name="asset">Döviz değişim verileri alınacak varlık bilgileri.</param>
        /// <returns>Döviz değişim verilerini içeren ExchangeResultDTO nesnesi.</returns>
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
                        DateTime today = DateTime.Today; // Şu anki tarih
                        var excelExchangeAll = _unitOfWorksRepository.ExchangeRepository.GetAll();
                        Exchange exchangeExcel = null;

                        if (assetDates[i].Year == today.Year && assetDates[i].Month == today.Month)
                        {
                            // İçinde bulunduğumuz aydaysak, bugünkü tarihle ilgili kur bilgisi
                            exchangeExcel = excelExchangeAll.FirstOrDefault(x => x.CurrentDate.Year == today.Year && x.CurrentDate.Month == today.Month && x.CurrentDate.Day == today.Day && x.BaseCurrencyCode == 1 && x.ForeignCurrencyCode == 56);
                        }
                        else
                        {
                            // Ayın son gününün kur bilgisi
                            DateTime sonGun = new DateTime(assetDates[i].Year, assetDates[i].Month, DateTime.DaysInMonth(assetDates[i].Year, assetDates[i].Month));
                            exchangeExcel = excelExchangeAll.FirstOrDefault(x => x.CurrentDate.Year == assetDates[i].Year && x.CurrentDate.Month == assetDates[i].Month && x.CurrentDate.Day == sonGun.Day && x.BaseCurrencyCode == 1 && x.ForeignCurrencyCode == 56);
                        }

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
        /// <summary>
        /// Verilen varlık, endeks ve döviz verileri kullanarak final bir tabloyu hesaplar.
        /// </summary>
        /// <param name="asset">Varlık verilerini içeren AssetResultDTO nesnesi.</param>
        /// <param name="ındexValue">Endeks verilerini içeren IndexResultDTO nesnesi.</param>
        /// <param name="exchange">Döviz verilerini içeren ExchangeResultDTO nesnesi.</param>
        /// <returns>AssetIndexExchangeFinalTableDTO türünde hesaplanan final tablo.</returns>
        public AssetIndexExchangeFinalTableDTO CalculateFinalTable(AssetResultDTO asset, IndexResultDTO ındexValue, ExchangeResultDTO exchange)
        {
           

            AssetIndexExchangeFinalTableDTO dto = new AssetIndexExchangeFinalTableDTO();


            try
            {
                List<List<object>> assetExcelFileColumn = new List<List<object>>();

                for (int columnIndex = 0; columnIndex < asset.Data.Columns.Count; columnIndex++)
                {
                    List<object> columnData = new List<object>();

                    for (int rowIndex = 0; rowIndex < asset.Data.Rows.Count; rowIndex++)
                    {
                        object cellValue = asset.Data.Rows[rowIndex][columnIndex];
                        columnData.Add(cellValue);
                    }
                    assetExcelFileColumn.Add(columnData);
                }
                // Final Table için tarih sütununun alınması
                dto.dateTimes = assetExcelFileColumn[0].Select(item =>
                {
                    if (DateTime.TryParse((string?)item, out DateTime dateTime))
                    {
                        return dateTime;
                    }
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
                    var value = dto.AssetHistoricalExchangeRate[dto.Assets.Count - 1] / dto.AssetHistoricalExchangeRate[i] * dto.Assets[i];
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
                //Final Tablo için Üfe Endeks Tutarı Sütünunun alınması  
                var allIndex = GetIndicesFromDataTable(ındexValue.Data);
                dto.ProducerPriceIndex = new List<decimal>();
                for (int i = 0; i < dto.Assets.Count; i++)
                {
                    var existIndex = allIndex.Indices.Where(x => x.Date.Month == dto.dateTimes[i].Month && x.Date.Year == dto.dateTimes[i].Year).FirstOrDefault();
                    if (existIndex != null)
                    {
                        dto.ProducerPriceIndex.Add(Convert.ToDecimal(existIndex.IndexValue));
                    }



                }
                //Enflasyon Varlık Tutarı Sütünunun alınması
                dto.InflationAssetValue = new List<decimal>();
                for (int i = 0; i < dto.Assets.Count; i++)
                {
                    var value = dto.ProducerPriceIndex[dto.ProducerPriceIndex.Count - 1] / dto.ProducerPriceIndex[i] * dto.Assets[i];
                    dto.InflationAssetValue.Add(value);
                }

                //Enflasyon Önceki Aya Göre Varlık Artış Sütünunun alınması
                dto.InflationAssetIncreaseComparedToThePreviousMonth = new List<decimal>();
                for (int i = 0; i < dto.InflationAssetValue.Count; i++)
                {
                    if (i == 0)
                    {
                        dto.InflationAssetIncreaseComparedToThePreviousMonth.Add(0);
                    }
                    if (i != 0)
                    {
                        var value = (dto.InflationAssetValue[i] - dto.InflationAssetValue[i - 1]) / dto.InflationAssetValue[i - 1];
                        dto.InflationAssetIncreaseComparedToThePreviousMonth.Add(value);
                    }

                }
                //Enflasyon Varlık Değişim Oranı Sütünunun alınması
                dto.InflationAssetTurnoverRate = new List<decimal>();
                for (int i = 0; i < dto.InflationAssetValue.Count; i++)
                {
                    var value = (dto.InflationAssetValue[dto.InflationAssetValue.Count - 1] - dto.InflationAssetValue[i]) / dto.InflationAssetValue[i];
                    dto.InflationAssetTurnoverRate.Add(value);
                }
                //Enflasyon Etkisi Yüzde Sütünunun alınması
                dto.InflationImpactPercentage = new List<decimal>();
                for (int i = 0; i < dto.InflationAssetValue.Count; i++)
                {
                    var value = (dto.Assets[i] - dto.InflationAssetValue[i]) / dto.InflationAssetValue[i];
                    dto.InflationImpactPercentage.Add(value);
                }


                dto.Success = true;
                dto.Message = "Final Table Successfully Converted to Data Table";

                
            }
            catch (Exception ex)
            {

                dto.Success = false;
                dto.ErrorMessage = ex.Message;
                dto.Message = "Final Table Successfully Converted to Data Table";
            }
            return dto;
        }
        /// <summary>
        /// Tablodan üfe endeks verilerini alır ve bir dto da birleştirir.
        /// </summary>
        /// <param name="dataTable">Endeks verilerini içeren DataTable nesnesi.</param>
        /// <returns>İşlem sonucunu temsil eden IndexResultDTO nesnesi.</returns>
        public IndexResultDTO GetIndicesFromDataTable(DataTable dataTable)
        {
            IndexResultDTO result = new IndexResultDTO();

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                result.Success = false;
                result.Message = "No data available.";
                return result;
            }

            Dictionary<DateTime, decimal?> birlesmisVeri = Birlestir(dataTable);

            if (birlesmisVeri.Count == 0)
            {
                result.Success = false;
                result.Message = "No valid index data available.";
                return result;
            }

            result.Indices = birlesmisVeri.Select(kv => new IndexEntity
            {
                Date = kv.Key,
                IndexValue = kv.Value ?? 0 // Null değerleri 0 olarak kabul ediyoruz
            }).ToList();

            result.Success = true;
            result.Message = "Index data retrieved successfully.";
            result.Data = dataTable;

            return result;
        }
        /// <summary>
        /// Verilen bir DataTable'ı kullanarak, her satırın yıl ve aylarına göre bir Dictionary oluşturan bir yöntem.
        /// </summary>
        /// <param name="dataTable">Veri çıkartılacak DataTable.</param>
        /// <returns>DateTime anahtarları ve decimal? değerleri içeren bir Dictionary.</returns>
        /// <remarks>
        /// Bu yöntem, her satırın belirtilen sütunlarını bir DateTime anahtarıyla eşleştirir ve bu anahtara karşılık gelen
        /// hücresel değerleri decimal? türünde bir değere çevirir. Eğer hücresel değerler geçerli bir decimal değilse, 
        /// null olarak saklanır. Eğer bir hücresel değer boşsa veya DBNull.Value ise, o hücre null olarak kabul edilir.
        /// </remarks>
        public Dictionary<DateTime, decimal?> Birlestir(DataTable dataTable)
        {
            Dictionary<DateTime, decimal?> birlesmisVeri = new Dictionary<DateTime, decimal?>();

            foreach (DataRow row in dataTable.Rows)
            {
                int yil = Convert.ToInt32(row[0]);
                int ay = 1;
                int sonSutun = row.Table.Columns.Count - 1;
                while (ay <= 12)
                {
                    object hucreselDeger = row[ay];

                    if (ay > sonSutun)
                    {
                        break; 
                    }
                    decimal? deger = null;

                    if (hucreselDeger != DBNull.Value)
                    {
                        string valueStr = hucreselDeger.ToString();

                        if (!string.IsNullOrWhiteSpace(valueStr))
                        {
                            if (decimal.TryParse(valueStr, out decimal parsedValue))
                            {
                                deger = parsedValue;
                            }
                            else
                            {
                            }
                        }
                    }

                    DateTime tarih = new DateTime(yil, ay, 1);
                    birlesmisVeri.Add(tarih, deger);

                    ay++;
                }
            }

            return birlesmisVeri;
        }
        /// <summary>
        /// Verilen bir AssetResultDTO içindeki veri sütunlarından ilki olarak kabul edilen sütundaki DateTime değerlerini çıkarır ve bir liste olarak döndürür.
        /// Dönüşüm başarısız olursa, varsayılan olarak DateTime.MinValue kullanılır.
        /// </summary>
        /// <param name="asset">Tarih verileri içeren AssetResultDTO nesnesi</param>
        /// <returns>DateTime değerlerini içeren bir liste</returns>
        public List<DateTime> CalculateAssetDate(AssetResultDTO asset)
        {
            List<List<object>> assetExcelFileColumn = new List<List<object>>();

            for (int columnIndex = 0; columnIndex < asset.Data.Columns.Count; columnIndex++)
            {
                List<object> columnData = new List<object>();

                for (int rowIndex = 0; rowIndex < asset.Data.Rows.Count; rowIndex++)
                {
                    
                    object cellValue = asset.Data.Rows[rowIndex][columnIndex];
                    columnData.Add(cellValue);
                }
                assetExcelFileColumn.Add(columnData);
            }
            AssetIndexExchangeFinalTableDTO dto = new AssetIndexExchangeFinalTableDTO();
            dto.dateTimes = assetExcelFileColumn[0].Select(item =>
            {
                if (DateTime.TryParse((string?)item, out DateTime dateTime))
                {
                    return dateTime;
                }
                return DateTime.MinValue;
            }).ToList();

            return dto.dateTimes;
        }




    }
}

