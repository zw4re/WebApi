using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            await _kapParseService.FetchAndSaveCompaniesAsync();
            Console.WriteLine("İşlem tamamlandı.");
        }
    }
}
