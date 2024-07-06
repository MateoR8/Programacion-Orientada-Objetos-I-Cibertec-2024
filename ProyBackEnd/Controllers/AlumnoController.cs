using CapaEntidad;
using Microsoft.AspNetCore.Mvc;
using ProyBackEnd.Models;

namespace ProyBackEnd.Controllers
{
    [ApiController]
    [Route("Alumno")]
    public class AlumnoController : Controller
    {
        [Route("ListarAlumno")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ListarAlumno(int orden, int idAlumno)
        {
            try
            {
                var lista = await new AlumnoDto().ListarAlumno(orden, idAlumno);
                return Ok(lista);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ListarAlumnoXID")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ListarAlumnoXID(int orden, int idAlumno)
        {
            try
            {
                var lista = await new AlumnoDto().ListarAlumnoXID(orden, idAlumno);
                return Ok(lista);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("RegistrarAlumno")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> RegistrarAlumno([FromBody] AlumnoEnt entidad)
        {
            try
            {
                ResultadoTransationEnt respuesta = new ResultadoTransationEnt();  

                if(entidad.IdAlumno == 0)
                {
                    respuesta = await new AlumnoDto().RegistrarAlumno(entidad);
                }
                else
                {
                    respuesta = await new AlumnoDto().EditarAlumno(entidad);
                }

                return Ok(respuesta);

            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);  
            }
        }

        [Route("EliminarAlumno")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EliminarAlumno(int idAlumno)
        {
            try
            {
                var respuesta = await new AlumnoDto().EliminarAlumno(idAlumno);
                return Ok(respuesta);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
