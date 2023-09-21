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

        [HttpPost("[action]")]
        public IActionResult ProcessForm([FromForm] DataDTO data)
        {
            try
            {
                var startDate = data.StartDate;
                var lastdate = data.EndDate;
                _finmaksApiService.MakeExchangesUpToDate(lastdate);
                var asset = _finmaksApiService.GetAsset(data);
                var index = _finmaksApiService.GetIndex(data);
                var exchange = _finmaksApiService.GetExchange(asset);
                var finalTable = _finmaksApiService.CalculateFinalTable(asset, index, exchange);       
                return Ok(finalTable);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
