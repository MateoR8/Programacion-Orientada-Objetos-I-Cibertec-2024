namespace ProyBackEnd.Models
{
    public class MetaGlobal
    {
        public static String Cnx = "";

        public static void LoadConnectionString(string conexion)
        {
            Cnx = conexion; 
        }

    }
}
