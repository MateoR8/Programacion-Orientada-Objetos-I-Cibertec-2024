using CapaEntidad;
using System.Data.SqlClient;
using System.Data;

namespace ProyBackEnd.Models
{
    public class CursoDto
    {
        public async Task<List<CursoEnt>> listarCurso()
        {
            List<CursoEnt> objCurso = new List<CursoEnt>();
            using (SqlConnection cnn = new SqlConnection(MetaGlobal.Cnx))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand("USP_Curso_List", cnn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read()) {  
                            CursoEnt obj = new CursoEnt();  
                            obj.IdCurso = Convert.ToInt32(reader["IdCurso"].ToString());
                            obj.CodCurso = reader["CodCurso"].ToString();
                            obj.NombreCurso = reader["NombreCurso"].ToString();
                            objCurso.Add(obj);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        objCurso = new List<CursoEnt>();
                    }
                }
            }
                return objCurso;
        }
    }
}
