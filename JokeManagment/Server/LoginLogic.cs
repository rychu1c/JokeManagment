using JokeManagment.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapper;
using System.Collections;

namespace JokeManagment.Server
{
    public class LoginLogic
    {
        public CurrentUser MenuLogin()
        {
            CurrentUser currentUser = new CurrentUser();

            //Take user login and password
            Console.WriteLine("Podaj swój login.");
            string? inputLogin = Console.ReadLine();
            Console.WriteLine("Podaj swoje hasło");
            string? inputPassword = Console.ReadLine();

            if (string.IsNullOrEmpty(inputLogin) || string.IsNullOrEmpty(inputPassword)) { return currentUser; }

            return currentUser = GetUserDB(currentUser, inputLogin, inputPassword);
        }
        private CurrentUser GetUserDB(CurrentUser user,string Login,string password)
        {
            using (var loginConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    user = loginConnection.QuerySingleOrDefault<CurrentUser>($"SELECT * FROM users WHERE login = '{Login}' AND password = '{password}'");//Stored Procedure
                }
                catch
                {
                    Console.WriteLine("Błąd logowania,spróbuj ponownie.");
                    return user = null;
                }
            }
            return user;
        }

    }
}
