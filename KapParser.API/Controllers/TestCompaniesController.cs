using Microsoft.AspNetCore.Mvc;
using KapParser.API.Services;

namespace KapParser.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestCompaniesController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public TestCompaniesController(CompanyService companyService)
        {
            _companyService = companyService;
        }

        // GET: /api/testcompanies
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }

        // GET: /api/testcompanies/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }
    }
}
