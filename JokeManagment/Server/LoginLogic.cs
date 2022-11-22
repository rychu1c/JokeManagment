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
            CurrentUser currentUser1 = new CurrentUser();
            //Take user login and password
            Console.WriteLine("Podaj swój login.");
            string? inputLogin = Console.ReadLine();
            Console.WriteLine("Podaj swoje hasło");
            string? inputPassword = Console.ReadLine();
            if (string.IsNullOrEmpty(inputLogin) || string.IsNullOrEmpty(inputPassword)) { return currentUser1; }
            using (var loginConnection = ConnectionSQL.EstablishConnection())
            {
                //Send it to DB
                //if data match return object use
                //var DBoutput = loginConnection.Query<CurrentUser>($"SELECT * FROM users WHERE login = '{inputLogin}' AND password = '{inputPassword}'").ToList();     SQL COMMAND
                currentUser1 = loginConnection.QuerySingleOrDefault<CurrentUser>($"SELECT * FROM login('{inputLogin}', '{inputPassword}')");//Stored Procedure
            }
            return currentUser1;
        }

    }
}
