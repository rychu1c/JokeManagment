using Npgsql;
using System.Data;

namespace JokeManagment.Server
{//Connection Scoped
    public static class ConnectionSQL
    {
        static string stringConnection = "User ID = kbdqewln; Password = 0sz7xXfaP49UwO41tkddPpniCQr2b7DL; Host = surus.db.elephantsql.com; Port = 5432; Database = kbdqewln;";
        public static IDbConnection Connection = new NpgsqlConnection(stringConnection);

        public static NpgsqlConnection EstablishConnection()
        {
            return new NpgsqlConnection(stringConnection);
        }
    }

}
