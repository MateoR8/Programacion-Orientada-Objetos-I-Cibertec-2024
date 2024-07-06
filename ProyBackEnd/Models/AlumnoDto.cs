using CapaEntidad;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Data;
using System.Transactions;
using System.Data.Common;

namespace ProyBackEnd.Models
{
    public class AlumnoDto
    {
        public async Task<List<AlumnoEnt>> ListarAlumno(int orden, int idAlumno)
        {
            List<AlumnoEnt> objAlumno = new List<AlumnoEnt>();
            using (SqlConnection cnn = new SqlConnection(MetaGlobal.Cnx))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand("USP_AlumnoCurso_List", cnn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@orden", orden);
                        cmd.Parameters.AddWithValue("@IdAlumno", idAlumno);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while(reader.Read())
                            {
                                AlumnoEnt obj = new AlumnoEnt();
                                obj.IdAlumno = Convert.ToInt32(reader["IdAlumno"].ToString());
                                obj.Nombres = reader["Nombres"].ToString();
                                obj.Apellidos = reader["Apellidos"].ToString();
                                obj.Ciclo = reader["Ciclo"].ToString();
                                obj.Carrera = reader["Carrera"].ToString();
                                obj.FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"].ToString());
                                objAlumno.Add(obj);
                            }
                        }
                    }
                    catch (Exception ex) {
                        objAlumno = new List<AlumnoEnt>();
                    }
                }
            }
                return objAlumno;
        }

        public async Task<AlumnoEnt> ListarAlumnoXID(int orden, int idAlumno)
        {
            AlumnoEnt obj = new AlumnoEnt();
            using (SqlConnection cnn = new SqlConnection(MetaGlobal.Cnx))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand("USP_AlumnoCurso_List", cnn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@orden", orden);
                        cmd.Parameters.AddWithValue("@idAlumno", idAlumno);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                obj.IdAlumno = Convert.ToInt32(reader["IdAlumno"].ToString());
                                obj.Nombres = reader["Nombres"].ToString();
                                obj.Apellidos = reader["Apellidos"].ToString();
                                obj.Ciclo = reader["Ciclo"].ToString();
                                obj.Carrera = reader["Carrera"].ToString();
                                obj.FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"].ToString());

                                obj.alumnoCursos = new List<AlumnoCursoEnt>();

                                if (await reader.NextResultAsync())
                                {
                                    while(reader.Read())
                                    {
                                        AlumnoCursoEnt alumnoCurso = new AlumnoCursoEnt();
                                        alumnoCurso.IdAlumnoCurso = Convert.ToInt32(reader["IdAlumnoCurso"].ToString());
                                        alumnoCurso.IdAlumno = Convert.ToInt32(reader["IdAlumno"].ToString());
                                        alumnoCurso.idCurso = Convert.ToInt32(reader["IdCurso"].ToString());
                                        alumnoCurso.Nota = Convert.ToInt32(reader["Nota"].ToString());
                                        obj.alumnoCursos.Add(alumnoCurso);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        obj = new AlumnoEnt();
                    }
                }
            }
            return obj;
        }

        public async Task<ResultadoTransationEnt> RegistrarAlumno(AlumnoEnt entidad)
        {
            ResultadoTransationEnt resultado = new ResultadoTransationEnt();
            using (SqlConnection cnn = new SqlConnection(MetaGlobal.Cnx))
            {
                cnn.Open();
                SqlTransaction trans = cnn.BeginTransaction();
                using (SqlCommand cmd = new SqlCommand("USP_Alumno_Insert", cnn, trans))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                        cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                        cmd.Parameters.AddWithValue("@Ciclo", entidad.Ciclo);
                        cmd.Parameters.AddWithValue("@Carrera", entidad.Carrera);
                        cmd.Parameters.Add("@IdAlumno", SqlDbType.Int, 11).Direction = ParameterDirection.Output;
                        await cmd.ExecuteNonQueryAsync();

                        int _IdAlumno = Convert.ToInt32(cmd.Parameters["@IdAlumno"].Value.ToString());

                        if(entidad.alumnoCursos.Count() > 0)
                        {
                            foreach(var item in entidad.alumnoCursos)
                            {
                                var insertarData = await InsertarAlumnoCurso(item, _IdAlumno, cnn, trans);
                                if(insertarData.IdRegistro == -1) { 
                                    trans.Rollback();
                                    resultado.IdRegistro = -1;
                                    resultado.Mensaje = insertarData.Mensaje;
                                    return resultado;
                                }
                            }
                        } 

                        resultado.IdRegistro = 0;
                        resultado.Mensaje = "Registro Correcto";

                        trans.Commit();
                        trans.Dispose();
                    }
                    catch (Exception ex) {
                        trans.Rollback();
                        resultado.IdRegistro = -1;
                        resultado.Mensaje = ex.Message;
                    }
                }
            }
            return resultado;
        }

        public async Task<ResultadoTransationEnt> EditarAlumno(AlumnoEnt entidad)
        {
            ResultadoTransationEnt resultado = new ResultadoTransationEnt();
            using (SqlConnection cnn = new SqlConnection(MetaGlobal.Cnx))
            {
                cnn.Open();
                SqlTransaction trans = cnn.BeginTransaction();
                using (SqlCommand cmd = new SqlCommand("USP_Alumno_Update", cnn, trans))
                {
                    try
                    {
                        if (entidad.alumnoCursos.Count() > 0)
                        {
                            foreach (var item in entidad.alumnoCursos.Where(w => w.IdAlumnoCurso > 0))
                            {
                                var insertarData = await ActualizarAlumnoCurso(item, cnn, trans);
                                if (insertarData.IdRegistro == -1)
                                {
                                    trans.Rollback();
                                    resultado.IdRegistro = -1;
                                    resultado.Mensaje = insertarData.Mensaje;
                                    return resultado;
                                }
                            }

                            foreach (var item in entidad.alumnoCursos.Where(w => w.IdAlumnoCurso == 0)) {
                                var insertarData = await InsertarAlumnoCurso(item, entidad.IdAlumno, cnn, trans);
                                if (insertarData.IdRegistro == -1)
                                {
                                    trans.Rollback();
                                    resultado.IdRegistro = -1;
                                    resultado.Mensaje = insertarData.Mensaje;
                                    return resultado;
                                }
                            }
                        }

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Nombres", entidad.Nombres);
                        cmd.Parameters.AddWithValue("@Apellidos", entidad.Apellidos);
                        cmd.Parameters.AddWithValue("@Ciclo", entidad.Ciclo);
                        cmd.Parameters.AddWithValue("@Carrera", entidad.Carrera);
                        cmd.Parameters.AddWithValue("@IdAlumno", entidad.IdAlumno);
                        await cmd.ExecuteNonQueryAsync();

                        trans.Commit();
                        trans.Dispose();

                        resultado.IdRegistro = 0;
                        resultado.Mensaje = "Actualizacion Correcta";

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        resultado.IdRegistro = -1;
                        resultado.Mensaje = ex.Message;

                    }
                }
            }
            return resultado;
        }

        public async Task<ResultadoTransationEnt> EliminarAlumno(int idAlumno)
        {
            ResultadoTransationEnt resultado = new ResultadoTransationEnt();
            using (SqlConnection cnn = new SqlConnection(MetaGlobal.Cnx))
            {
                cnn.Open();
                SqlTransaction transaction = cnn.BeginTransaction();
                using (SqlCommand cmd = new SqlCommand("USP_Alumno_Delete", cnn, transaction))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdAlumno", idAlumno);
                        await cmd.ExecuteNonQueryAsync();

                        resultado.IdRegistro = 0;
                        resultado.Mensaje = "Eliminación exitosa.";

                        transaction.Commit();
                        transaction.Dispose();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        resultado.IdRegistro = -1;
                        resultado.Mensaje = ex.Message;
                    }
                }
            }
            return resultado;
        }

        public async Task<ResultadoTransationEnt> ActualizarAlumnoCurso(AlumnoCursoEnt cursos, SqlConnection cnn, SqlTransaction trans)
        {
            ResultadoTransationEnt resultado = new ResultadoTransationEnt();

            using (SqlCommand cmd = new SqlCommand("USP_AlumnoCurso_Update", cnn, trans))
            {
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAlumnoCurso", cursos.IdAlumnoCurso);
                    cmd.Parameters.AddWithValue("@IdCurso", cursos.idCurso);
                    cmd.Parameters.AddWithValue("@Nota", cursos.Nota);
                    await cmd.ExecuteNonQueryAsync();

                    resultado.IdRegistro = 0;
                    resultado.Mensaje = "OK";
                }
                catch (Exception ex)
                {
                    resultado.IdRegistro = -1;
                    resultado.Mensaje = ex.Message;
                }
            }
            return resultado;
        }

        public async Task<ResultadoTransationEnt> InsertarAlumnoCurso(AlumnoCursoEnt cursos, int IdAlumno, SqlConnection cnn, SqlTransaction trans)
        {
            ResultadoTransationEnt resultado = new ResultadoTransationEnt();

            using (SqlCommand cmd = new SqlCommand("USP_AlumnoCurso_Insert", cnn, trans)){
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAlumno", cursos.IdAlumno);
                    cmd.Parameters.AddWithValue("@IdCurso", cursos.idCurso);
                    cmd.Parameters.AddWithValue("@Nota", cursos.Nota);

                    resultado.IdRegistro = 0;
                    resultado.Mensaje = "OK";
                }
                catch (Exception ex)
                {
                    resultado.IdRegistro = -1;
                    resultado.Mensaje = ex.Message;
                }
            }
            return resultado;
        }
    }
}
