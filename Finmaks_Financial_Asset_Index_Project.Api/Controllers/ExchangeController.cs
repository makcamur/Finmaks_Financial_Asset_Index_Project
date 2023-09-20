using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs;
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
                var asset=_finmaksApiService.GetAsset(data);
                var index=_finmaksApiService.GetIndex(data);

                //for (int i = 0; i < asset.Data.Rows.Count; i++)
                //{
                //    asset.Data.Rows[i].ItemArray.ToList<>
                //}
            

                


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
