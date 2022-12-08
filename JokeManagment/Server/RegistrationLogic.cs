using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using JokeManagment.Client;
using static JokeManagment.Server.CurrentUser;
using System.Xml.Linq;
using Dapper;

namespace JokeManagment.Server
{
    internal class RegistrationLogic
    {
        public void MenuRegistration()
        {
            //User fill registration formula
            //Send formula to DB
            //DB check if user login is already taken
            //if its free write user to DB and return true
            Console.WriteLine("Podaj login nowego użytkownika");
            string? inputLogin = Console.ReadLine();
            if (string.IsNullOrEmpty(inputLogin)) { return; }

            Console.WriteLine("Podaj hasło");
            string? inputPassword = Console.ReadLine();
            if (string.IsNullOrEmpty(inputPassword)) { return; }

            Console.WriteLine("Podaj Imię");
            string? inputName = Console.ReadLine();
            if (string.IsNullOrEmpty(inputName)) { return; }

            Console.WriteLine("Podaj Nazwisko");
            string? inputSurname = Console.ReadLine();
            if (string.IsNullOrEmpty(inputSurname)) { return; }

            Console.WriteLine("Wpisz 1 jeżeli jesteś nauczycielem ,2 jeżeli jesteś uczniem");
            string? inputStatus = Console.ReadLine();
            LearningLevel StatusConverted;
            bool isEnum = Enum.TryParse<LearningLevel>(inputStatus, out StatusConverted);
            if (!isEnum) { return ; }
            Console.WriteLine(StatusConverted);

            var lista = TakeCity();
            foreach (var list in lista)
            {
                Console.WriteLine(list);
            }
            Console.WriteLine();
            Console.WriteLine("Wpisz z jakiego miasta się logujesz");
            string? inputLocation = Console.ReadLine();
            if (string.IsNullOrEmpty(inputLocation)) { return; }
            int Locationint;
            bool isNumber = int.TryParse(inputLocation, out Locationint);
            if (!isNumber) { return ; }

            CurrentUser User = new CurrentUser(inputLogin,inputPassword,inputName, inputSurname, StatusConverted, Locationint);
            SendFormula(User);
        }

        private void SendFormula(CurrentUser currentUser)
        {
            bool isRegistrationSuccess;
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            { 

            }
        }
        private IEnumerable<string> TakeCity()
        {
            string? input;
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                var Cities = RegistrationConnection.Query<string>($"SELECT city FROM Location;");
                //var inputsql = RegistrationConnection.QueryMultiple($"SELECT city FROM Location;");


                return Cities;
            }
            
        }

    }
}
