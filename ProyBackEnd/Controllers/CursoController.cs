using CapaEntidad;
using Microsoft.AspNetCore.Mvc;
using ProyBackEnd.Models;

namespace ProyBackEnd.Controllers
{
    [ApiController]
    [Route("Curso")]
    public class CursoController : Controller
    {
        [Route("listarCurso")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> listarCurso()
        {
            try
            {
                var lista = await new CursoDto().listarCurso();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
