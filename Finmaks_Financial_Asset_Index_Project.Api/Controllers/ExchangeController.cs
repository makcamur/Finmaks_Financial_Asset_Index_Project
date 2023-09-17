using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
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
                // Burada form verilerini işleyin.
                // fileAsset ve fileIndex dosyalarını kaydedebilir veya işleyebilirsiniz.
                // startDate ve endDate gibi tarih verilerini kullanabilirsiniz.

                // İşleme sonucunu oluşturun
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
