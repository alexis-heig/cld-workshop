using Npgsql;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Launcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
			// Read the SQL connection string from the CS.txt file which should be in the same directory than the launcher.
			var connectionString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CS.txt"));

			// Extract the database name from the connection string.
			var databaseName = ExtractDatabaseName(connectionString);
            Console.WriteLine("Nom de la base de données: " + databaseName);

            // We make sure that the name of the database is in lower case.
            connectionString = connectionString.Replace(databaseName, databaseName.ToLower());
            databaseName = databaseName.ToLower();

            try
            {
                // The following code first determines whether the database exists and, if not, creates it and runs the setup scripts on it.
                using var createConnection = new NpgsqlConnection(connectionString.Replace(databaseName, "postgres"));
                createConnection.Open();

                var databaseExists = DatabaseExists(databaseName, createConnection);
                if (!databaseExists)
                {
                    Console.WriteLine($"Création de la base de donnée: {databaseName}");
                    SendRequest($"CREATE DATABASE {databaseName}", createConnection);
                    ExecuteSetupScripts(connectionString);
                }
                else
                {
                    Console.WriteLine("La base de données existe déjà.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite lors de l'initialisation de l'application. Détails de l'exception :");
                Console.WriteLine(ex);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Execute all setup scripts to setup the database.
        /// </summary>
        /// <param name="connectionString">SQL connection string.</param>
        /// <returns></returns>
        private static void ExecuteSetupScripts(string connectionString)
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup");
            foreach (var sqlScript in Directory.GetFiles(scriptsPath))
            {
                Console.WriteLine($"Execution du script: {sqlScript}");
                SendRequest(File.ReadAllText(sqlScript), connection);
            }
        }

        /// <summary>
        /// Start the DrugLordManager app.
        /// </summary>
        /// <param name="connectionString">SQL connection string.</param>
        private static void StartApplication(string connectionString)
        {
            var appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app");
            var psi = new ProcessStartInfo(Path.Combine(appPath, "DrugLordManager.exe"))
            {
                WorkingDirectory = appPath,
                Arguments = connectionString
            };
            Process.Start(psi);
        }

        /// <summary>
        /// Extract the database's name from a connection string.
        /// </summary>
        /// <param name="connectionString">The SQL connection string.</param>
        /// <returns>The database's name</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static string ExtractDatabaseName(string connectionString)
        {
            var regex = new Regex("[Dd][Aa][Tt][Aa][Bb][Aa][Ss][Ee]=([^;]+)");
            var match = regex.Match(connectionString);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            throw new InvalidOperationException("La chaîne de connexion ne contient pas le nom de la base de données.");
        }

        /// <summary>
        /// Return a boolean indicating wether the given database already exists or not.
        /// </summary>
        /// <param name="databaseName">Database's name</param>
        /// <param name="connection">PostegreSQL connection</param>
        /// <returns>True if the database exists. False otherwise.</returns>
        private static bool DatabaseExists(string databaseName, NpgsqlConnection connection)
        {
            using var cmd = new NpgsqlCommand($"select exists(SELECT datname FROM pg_catalog.pg_database WHERE lower(datname) = lower('{databaseName}'));", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                return Convert.ToBoolean(reader[0]);
            }
            return false;
        }

        /// <summary>
        /// Send a SQL request with the given connection to a PostgreSQL database.
        /// </summary>
        /// <param name="request">SQL request</param>
        /// <param name="connection">PostgreSQL connection</param>
        private static void SendRequest(string request, NpgsqlConnection connection)
        {
            using var cmd = new NpgsqlCommand(request, connection);
            cmd.ExecuteNonQuery();
        }
    }
}