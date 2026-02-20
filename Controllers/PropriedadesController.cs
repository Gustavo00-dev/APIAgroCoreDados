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

        public PropriedadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Buscar propriedades associadas a um usuário específico, utilizando o Id do usuário como parâmetro de consulta.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("BuscarPropriedades")]
        public async Task<IActionResult> BuscarPropriedades([FromQuery] int Id)
        {
            try
            {
                var propriedades = await _context.Propriedade
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
