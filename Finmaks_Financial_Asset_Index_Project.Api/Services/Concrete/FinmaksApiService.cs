﻿using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Response;
using Finmaks_Financial_Asset_Index_Project.DataAccess.Repository.Irepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Finmaks_Financial_Asset_Index_Project.Api.Services.Concrete
{
    public class FinmaksApiService: IFinmaksApiService
    {
        private readonly IUnitOfWorksRepository _unitOfWorksRepository;

        public FinmaksApiService(IUnitOfWorksRepository unitOfWorksRepository)
        {
            _unitOfWorksRepository = unitOfWorksRepository;
        }

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

       
        /// <summary>
        /// Database'de kayıtlı son tarihin bulunması ve son tarihten itibaren güncel verilerin çekilmesi
        /// </summary>
        /// <param name="lastDate"></param>
        public void MakeExchangesUpToDate(DateTime? lastDate)
        {
            
            var allExchanges= _unitOfWorksRepository.ExchangeRepository.GetAll();
            var lastExchange = allExchanges.Where(x => x.CurrentDate == lastDate).FirstOrDefault();
            if(lastExchange==null)
            {
                //databasede kayıtlı son tarihin bulunması ve son tarihten itibaren güncel verilerin çekilmesi
                var startdate = FindLastDate();
                var updateExchanges = GetFinmaksExchangeRates(startdate);
                
            }         
             
        } 
        /// <summary>
        /// Database'de kayıtlı son tarihin bulunması ve yoksa eğer 2021-12-01 tarihinden itibaren güncel verilerin çekilmesi
        /// </summary>
        /// <returns>DateTime</returns>
        public DateTime FindLastDate()
        {
            var allExchanges = _unitOfWorksRepository.ExchangeRepository.GetAll();
            var lastExchange = allExchanges.OrderByDescending(x => x.CurrentDate).FirstOrDefault();
            var findLastDate = allExchanges.OrderByDescending(x => x.CurrentDate).FirstOrDefault();
            if(findLastDate==null)
            {
                var startdate = new DateTime(2021, 12, 01);
                return startdate;
            }
            else
            {
               var startdate = findLastDate.CurrentDate;
               return startdate;
            }  
            
        }
     
    }
}
