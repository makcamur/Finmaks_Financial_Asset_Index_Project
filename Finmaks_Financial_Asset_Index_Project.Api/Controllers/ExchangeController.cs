using Microsoft.AspNetCore.Mvc;

namespace Finmaks_Financial_Asset_Index_Project.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : Controller
    {
       
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string apiUrl = "https://testapi.finmaks.com/ExchangeRates?key=Finmaks123&startDate=2023-09-01&endDate=2023-09-05";
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(apiUrl))
                {
                    using (HttpContent content = res.Content)
                    {
                        string data = await content.ReadAsStringAsync();
                        if (data != null)
                        {
                            return Ok(data);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
            }
            
        }
    }
}
