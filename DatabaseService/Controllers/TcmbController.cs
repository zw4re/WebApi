using Microsoft.AspNetCore.Mvc;
using DatabaseService.Context;
using Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TcmbController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TcmbController(AppDbContext context)
        {
            _context = context;
        }

        // POST: /api/tcmb >Veri eker
        [HttpPost]
        public async Task<IActionResult> PostRates([FromBody] List<TcmbExchangeRate> rates)
        {
            if (rates == null || !rates.Any())
                return BadRequest("Boş veri gönderildi.");

            await _context.TcmbExchangeRates.AddRangeAsync(rates);
            await _context.SaveChangesAsync();

            return Ok("Veriler başarıyla eklendi.");
        }

        // GET: /api/tcmb > Tüm verileri listeler
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TcmbExchangeRate>>> GetAll()
        {
            return await _context.TcmbExchangeRates.ToListAsync();
        }

        // GET: /api/tcmb/{date}/{currencyCode}/{type} > Tekil veri getirir
        [HttpGet("{date}/{currencyCode}/{type}")]
        public async Task<ActionResult<TcmbExchangeRate>> GetOne(string date, string currencyCode, string type)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Geçersiz tarih formatı.");

            var result = await _context.TcmbExchangeRates.FindAsync(parsedDate, currencyCode, type);
            if (result == null)
                return NotFound();

            return result;
        }

        // PUT: /api/tcmb > Tüm veriyi günceller
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TcmbExchangeRate updated)
        {
            var existing = await _context.TcmbExchangeRates.FindAsync(updated.Date, updated.CurrencyCode, updated.Type);
            if (existing == null)
                return NotFound();

            existing.Value = updated.Value;

            await _context.SaveChangesAsync();
            return Ok("Veri güncellendi.");
        }

        // DELETE: /api/tcmb/{date}/{currencyCode}/{type} > Belirli veriyi siler
        [HttpDelete("{date}/{currencyCode}/{type}")]
        public async Task<IActionResult> Delete(string date, string currencyCode, string type)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Geçersiz tarih.");

            var existing = await _context.TcmbExchangeRates.FindAsync(parsedDate, currencyCode, type);
            if (existing == null)
                return NotFound();

            _context.TcmbExchangeRates.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok("Veri silindi.");
        }
    }
}
