using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OfficeOpenXml;
using System.Data;
using System.Text.Json;

namespace Finmaks_Financial_Asset_Index_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeController : Controller
    {
        private readonly IFinmaksApiService _finmaksApiService;

        public ExchangeController(IFinmaksApiService finmaksApiService)
        {
            _finmaksApiService = finmaksApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            DateTime startDate = new DateTime(2023, 08, 01);
            var result = _finmaksApiService.GetFinmaksExchangeRates(startDate);
            var jsonResult = JsonSerializer.Serialize(result);
            return Ok(jsonResult);

        }
      

        [HttpPost("[action]")]
        public IActionResult ProcessForm([FromForm]DataDTO data)
        {
            try
            {             
                var lastdate=data.EndDate;

                _finmaksApiService.MakeExchangesUpToDate(lastdate);

                ////Burada form verilerini işleyin.
                // fileAsset ve fileIndex dosyalarını kaydedebilir veya işleyebilirsiniz.
                // startDate ve endDate gibi tarih verilerini kullanabilirsiniz.
                // İşleme sonucunu oluşturun
                DataTable table = new DataTable();
                try
                {
                    if (data.FileAsset != null)
                    {
                        //if you want to read data from a excel file use this
                        //using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
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

                                    ViewBag.SuccessMessage = "Excel Successfully Converted to Data Table";
                                }
                            }
                            else
                                ViewBag.ErrorMessage = "No Work Sheet available in Excel File";

                        }
                    }


                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
                var aa = table;
                var be = aa.Rows;             
                Dictionary<string, string> dict = new Dictionary<string, string>();

                for(int i = 7; i < be.Count; i++)
                {
                    be[i].ItemArray[0].ToString();
                    dict.Add(be[i].ItemArray[0].ToString(), be[i].ItemArray[1].ToString());
                }
                var dict2 = dict;

                //ara



                var result = new
                {
                    message = "Form verileri başarıyla işlendi.",
                    // Ek sonuç verilerini burada ekleyebilirsiniz.
                };

               
                // İşlem başarılı ise 200 OK yanıtı gönderin
                return Ok(result);
            }
            catch (Exception ex)
            {
                // İşlem sırasında bir hata oluştuysa hata yanıtı gönderin
                return BadRequest(new { error = ex.Message });
            }
        }
  
    }
}
