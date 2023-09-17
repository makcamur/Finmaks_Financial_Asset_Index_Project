using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Finmaks_Financial_Asset_Index_Project.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}
