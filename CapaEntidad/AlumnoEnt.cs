using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class AlumnoEnt
    {
      public int IdAlumno { get; set; }
      public String Nombres { get; set; }
      public String Apellidos { get; set; }
      public String Ciclo { get; set; }
      public String Carrera { get; set; }
      public DateTime FechaRegistro { get; set; }
      public List<AlumnoCursoEnt> alumnoCursos { get; set; }

    }
}
