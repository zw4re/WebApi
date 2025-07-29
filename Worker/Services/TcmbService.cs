using Entities.DbModels;
using System.Text.Json;

namespace Worker.Services
{
    public class TcmbService
    {
        //http istekleri ve appsettings.json erişimi
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _dbServiceUrl;
        private readonly IConfiguration _configuration;

        public TcmbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _apiKey = _configuration["Evds:ApiKey"];
            _dbServiceUrl = _configuration["URL:DatabaseService"];
        }

        public async Task FetchAndSaveTcmbRatesAsync()
        {
            try
            {
                // HEADER Ayarları

                _httpClient.DefaultRequestHeaders.Clear(); // varsa önce siler
                _httpClient.DefaultRequestHeaders.Add("key", _apiKey);
                string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36";
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                _httpClient.DefaultRequestHeaders.Add("Accept", "/");
                //_httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");


                string series = "TP.DK.USD.S,TP.DK.USD.A,TP.DK.EUR.S,TP.DK.EUR.A,TP.DK.CHF.S,TP.DK.CHF.A,TP.DK.GBP.S,TP.DK.GBP.A,TP.DK.JPY.S,TP.DK.JPY.A";
                string startDate = "01-10-2017";
                string endDate = "01-11-2017";

                // tam api urlsi
                string url = $"https://evds2.tcmb.gov.tr/service/evds/series={series}&startDate={startDate}&endDate={endDate}&type=json";

                // veri çekme
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Hata içeriği:");
                    Console.WriteLine(errorContent);

                    //Console.WriteLine("Veri çekilemedi!");
                    return;
                }

                // gelen json yanıtı okuma
                var content = await response.Content.ReadAsStringAsync();

                // json metni parse etme
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                var items = root.GetProperty("items");

                // yukarıda olusan nesne listye  eklenecek
                var rates = new List<TcmbExchangeRate>();

                foreach (var item in items.EnumerateArray())
                {
                    var date = DateTime.ParseExact(item.GetProperty("Tarih").GetString(), "dd-MM-yyyy", null);

                    foreach (var prop in item.EnumerateObject())
                    {
                        if (prop.Name.StartsWith("TP_DK")) //tp_dk ile başlayan alanlar parse edilir
                        {
                            var parts = prop.Name.Split('_'); // TP_DK_EUR_S
                            string currency = parts[2];       // EUR
                            string typeCode = parts[3];       // S or A
                            string type = typeCode == "S" ? "Sell" : "Buy";

                            if (decimal.TryParse(prop.Value.GetString(), out decimal value))
                            {
                                rates.Add(new TcmbExchangeRate
                                {
                                    Date = date,
                                    CurrencyCode = currency,
                                    Type = type,
                                    Value = value
                                });
                            }
                        }
                    }
                }

                // veriyi API üzerinden DatabaseService gönderiyoruz
                var postResponse = await _httpClient.PostAsJsonAsync($"{_dbServiceUrl}/api/tcmb", rates);
                if (postResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Veriler başarıyla DatabaseService API'ye gönderildi.");
                }
                else
                {
                    Console.WriteLine("Veri API'ye gönderilemedi. Durum: " + postResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TcmbService içinde hata oluştu: {ex.Message}");
            }
        }
    }
}