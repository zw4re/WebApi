using Models.Entities;
using System.Net.Http;
using System.Net.Http.Json;

namespace KapParser.API.Services
{
    public class CompanyService
    {
        private readonly HttpClient _httpClient;

        public CompanyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            var companies = await _httpClient.GetFromJsonAsync<List<Company>>("/api/companies");
            return companies ?? new List<Company>();
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Company>($"/api/companies/{id}");
        }
    }
}