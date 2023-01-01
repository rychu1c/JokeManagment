using Dapper;
using JokeManagment.Server;

namespace JokeManagment.Client
{
    public static class StaticMethods
    {
        public static List<T>? GetListFormDB<T>(string SQLCommand)
        {
            var list = new List<T>();
            using (var loginConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    list = loginConnection.Query<T>($"{SQLCommand}").ToList();//Stored Procedure
                }
                catch
                {
                    Console.WriteLine("Błąd pobrania listy.");
                    return list = null;
                }
            }
            return list;
        }

        public static bool isExecutSqlString(string SqlString)
        {
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute($"{SqlString}");
                }
                catch
                {
                    Console.WriteLine("Nie udało się wykonac zawołnia. Naciśnij dowolny klawisz by kontynuować.");
                    return false;
                }
                Console.WriteLine("Zapis powiodł się. Naciśnij dowolny klawisz by kontynuować.");
                return true;
            }
        }

        public static object? GetObject<T>(string SQLCommand)
        {
            using (var Connection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    var DBOutput = Connection.QuerySingleOrDefault<T>($"SQLCommand");
                    return DBOutput;
                }
                catch
                {
                    Console.WriteLine("Błąd pobrania danych z bazy");
                    return null;
                }
            }
        }
    }
}
