using System;
using System.Threading.Tasks;
using Worker.Services;

namespace Worker.Jobs
{
    public class KapJob
    {
        private readonly KapParseService _kapParseService;

        public KapJob(KapParseService kapParseService)
        {
            _kapParseService = kapParseService;
        }

        public async Task Run()
        {
            Console.WriteLine("KAP verileri çekiliyor...");
            await _kapParseService.FetchAndSendCompaniesAsync(); // ✅ Doğru metot adı
            Console.WriteLine("İşlem tamamlandı.");
        }
    }
}
