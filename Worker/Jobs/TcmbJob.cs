using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Worker.Services;

namespace Worker.Jobs
{
    public class TcmbJob
    {
        // Bağımlılık
        private readonly TcmbService _tcmbService;

        public TcmbJob(TcmbService tcmbService)
        {
            _tcmbService = tcmbService;
        }

        public async Task Run()
        {
            Console.WriteLine("TCMB döviz verisi çekiliyor...");
            try
            {
                await _tcmbService.FetchAndSaveTcmbRatesAsync();
                Console.WriteLine("TCMB veri işlemi tamamlandı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCMB job sırasında hata oluştu: {ex.Message}");

                throw; // Hangfire'ın tekrar denemesi için hatayı tekrar fırlatıyoruz
            }
        }
    }
}