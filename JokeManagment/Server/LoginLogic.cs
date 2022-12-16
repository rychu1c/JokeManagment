﻿using JokeManagment.Client;
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
            using (var loginConnection = ConnectionSQL.EstablishConnection())
            {
                //Send it to DB
                //if data match return object use
                //var DBoutput = loginConnection.Query<CurrentUser>($"SELECT * FROM users WHERE login = '{inputLogin}' AND password = '{inputPassword}'").ToList();     SQL COMMAND
                try
                {
                    currentUser = loginConnection.QuerySingleOrDefault<CurrentUser>($"SELECT * FROM users WHERE login = '{inputLogin}' AND password = '{inputPassword}'");//Stored Procedure
                }
                catch
                {
                    Console.WriteLine("Błąd logowania,spróbuj ponownie.");
                    return currentUser = null;
                }
            }
            return currentUser;
        }

    }
}
