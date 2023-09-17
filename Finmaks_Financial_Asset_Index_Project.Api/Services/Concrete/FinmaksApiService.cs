using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response;
using System.Text.Json;

namespace Finmaks_Financial_Asset_Index_Project.Api.Services.Concrete
{
    public class FinmaksApiService: IFinmaksApiService
    {

        public async Task<FinmaksExchangeRatesResponse> GetFinmaksExchangeRates(DateTime? startDate) { 
        
            string apiUrl = "https://testapi.finmaks.com/ExchangeRates";
         
            var requestParams = new FinmaksExchangeRatesRequest
            {
                Key = "Finmaks123",
                StartDate =(DateTime)(startDate == null ? new DateTime(2023, 09, 01) : startDate),
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
    }
}
