using System;
using System.Data.SQLite;
using System.IO;

namespace ReconKitApp
{
    public static class DataAccess
    {
        public static string DbFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reconkit.db");
        public static string ConnectionString => $"Data Source={DbFile};Version=3;Pooling=True;Max Pool Size=100;";

        public static void EnsureDatabase()
        {
            // ensure file exists (CreateFile will create an empty sqlite file)
            if (!File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);
            }

            using (var c = new SQLiteConnection(ConnectionString))
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS admins (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    level INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    dni TEXT,
    apellido TEXT,
    nombre TEXT,
    id_familia TEXT,
    fecha_nac TEXT,
    direccion TEXT,
    provincia TEXT,
    localidad TEXT,
    integrantes TEXT,
    notas TEXT,
    biometrics BLOB
);
";
                    cmd.ExecuteNonQuery();
                }

                // seed admin if none exist
                using (var cmdCheck = c.CreateCommand())
                {
                    cmdCheck.CommandText = "SELECT COUNT(*) FROM admins";
                    var count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    if (count == 0)
                    {
                        var defaultUser = "admin";
                        var defaultPass = "admin123"; // cambialo al primer login
                        var hash = Security.HashPassword(defaultPass);

                        using (var cmdInsert = c.CreateCommand())
                        {
                            cmdInsert.CommandText = "INSERT INTO admins (username, password_hash, level) VALUES (@u,@p,@l)";
                            cmdInsert.Parameters.AddWithValue("@u", defaultUser);
                            cmdInsert.Parameters.AddWithValue("@p", hash);
                            cmdInsert.Parameters.AddWithValue("@l", 3);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
