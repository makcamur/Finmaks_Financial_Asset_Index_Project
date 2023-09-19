using Finmaks_Financial_Asset_Index_Project.Api.Services.Abstract;
using Hangfire;

namespace Finmaks_Financial_Asset_Index_Project.Api.Services
{
    public class MyDailyJob
    {
        private readonly IFinmaksApiService _finmaksApiService;
        public MyDailyJob(IFinmaksApiService finmaksApiService)
        {
            _finmaksApiService = finmaksApiService;
        }
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)] // Başarısız işleri otomatik olarak kaldır
        public void Execute()
        {
            try
            {
                _finmaksApiService.MakeExchangesUpToDate(null);
                // Günlük görevinizi burada çalıştırın
                // Örneğin: MakeExchangesUpToDate(null); // null, en son tarihi kontrol etmek için
            }
            catch (Exception ex)
            {
                // Hata yönetimi burada yapılabilir
                Console.WriteLine("Hata oluştu: " + ex.Message);
            }
        }
    }
}
