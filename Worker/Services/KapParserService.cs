using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using DatabaseService;
using Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;


namespace Worker.Services
{
    public class KapParseService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _db;

        public KapParseService(HttpClient httpClient, AppDbContext db)
        {
            _httpClient = httpClient;
            _db = db;
        }

        public async Task FetchAndSaveCompaniesAsync()
        {
            var url = "https://www.kap.org.tr/tr/bist-sirketler";
            var html = await _httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var scripts = doc.DocumentNode.SelectNodes("//script");
            string jsonString = null;

            // İlgili veriyi barındıran script içeriğini bul
            foreach (var script in scripts)
            {
                var content = script.InnerText;

                if (content.Contains("kapMemberTitle") && content.Contains("relatedMemberTitle"))
                {
                    // JSON array kısmını regex ile al
                    var match = Regex.Match(content, @"\[\{.*?\}\]");

                    if (match.Success)
                    {
                        jsonString = match.Value;
                        break;
                    }
                }
            }

            if (jsonString == null)
            {
                Console.WriteLine("Veri bulunamadı.");
                return;
            }

            Console.WriteLine("JSON STRING (ilk 500 karakter):");
            Console.WriteLine(jsonString.Substring(0, Math.Min(500, jsonString.Length)));

            try
            {
                // Escape edilmiş karakterleri düzeltmek için Replace kullanıyoruz
                jsonString = jsonString.Replace("\\\"", "\""); // Escape karakterlerini düzeltme
                jsonString = jsonString + "}]";

                // JSON'u düzgün şekilde deserialize ediyoruz
                var wrapperList = JsonSerializer.Deserialize<List<CompanyWrapperJsonModel>>(jsonString);

                if (wrapperList == null || wrapperList.Count == 0)
                {
                    Console.WriteLine("Veri çözümlenemedi.");
                    return;
                }

                // JSON'u düzgün formatta (Pretty-print) alt alta yazdırma
                var formattedJson = JsonSerializer.Serialize(wrapperList, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine("Formatted JSON (Alt Alta Yazılmış):");
                Console.WriteLine(formattedJson);

                // İçerideki şirketleri alıyoruz ve veritabanına ekliyoruz
                foreach (var wrapper in wrapperList)
                {
                    foreach (var parsed in wrapper.content)
                    {
                        bool hasNullValue = false;
                        foreach (var property in parsed.GetType().GetProperties())
                        {
                            var value = property.GetValue(parsed);
                            // Eğer herhangi bir özellik null ise, döngüyü kır
                            if (value == null)
                            {
                                hasNullValue = true;
                                break;
                            }
                        }
                        if (hasNullValue)
                        {
                            break;
                        }
                        var company = new Company
                        {
                            MkkMemberOid = parsed.mkkMemberOid,
                            KapMemberTitle = parsed.kapMemberTitle,
                            RelatedMemberTitle = parsed.relatedMemberTitle,
                            StockCode = parsed.stockCode,
                            CityName = parsed.cityName,
                            RelatedMemberOid = parsed.relatedMemberOid,
                            KapMemberType = parsed.kapMemberType
                        };

                        _db.Companies.Add(company);
                    }
                }


                // Verileri veritabanına kaydet
                await _db.SaveChangesAsync();
                Console.WriteLine("Veriler başarıyla kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON ayrıştırma hatası: {ex}");
            }
        }
    }
}