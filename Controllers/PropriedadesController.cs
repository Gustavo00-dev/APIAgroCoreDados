using Microsoft.AspNetCore.Mvc;
using APIAgroCoreDados.Data;
using APIAgroCoreDados.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace APIAgroCoreDados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropriedadesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Injeta o ApplicationDbContext para acessar o banco MySQL
        public PropriedadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Busca todas as propriedades cujo IdUsers seja igual ao Id informado
        [HttpGet("BuscarPropriedades")]
        public async Task<IActionResult> BuscarPropriedades([FromQuery] int Id)
        {
            try
            {
                var propriedades = await _context.Propriedades
                    .Where(p => p.IdUsers == Id)
                    .ToListAsync();

                return Ok(propriedades);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Erro ao consultar propriedades: {ex.Message}");
            }
        }
    }
}
