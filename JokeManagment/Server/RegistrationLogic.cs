using Dapper;
using JokeManagment.Client;
using static JokeManagment.Server.CurrentUser;
using static System.Net.Mime.MediaTypeNames;

namespace JokeManagment.Server
{
    public class RegistrationLogic
    {
        public void MenuRegistration()
        {
            //User fill registration formula
            //Send formula to DB
            //DB check if user login is already taken
            //if its free write user to DB and return true
            Console.WriteLine("Podaj login nowego użytkownika");
            string? inputLogin = Console.ReadLine();
            if (string.IsNullOrEmpty(inputLogin) || !inputLogin.isStringLengthCorrect(40)) { return; }

            Console.WriteLine("Podaj hasło");
            string? inputPassword = Console.ReadLine();
            if (string.IsNullOrEmpty(inputPassword) || !inputPassword.isStringLengthCorrect(40)) { return; }

            Console.WriteLine("Podaj Imię");
            string? inputName = Console.ReadLine();
            if (string.IsNullOrEmpty(inputName) || !inputName.isStringLengthCorrect(40)) { return; }

            Console.WriteLine("Podaj Nazwisko");
            string? inputSurname = Console.ReadLine();
            if (string.IsNullOrEmpty(inputSurname) || !inputSurname.isStringLengthCorrect(40)) { return; }

            Console.WriteLine("Wpisz 1 jeżeli jesteś uczniem ,2 jeżeli jesteś nauczycielem");
            string? inputStatus = Console.ReadLine();
            LearningLevel StatusConverted;
            bool isEnum = Enum.TryParse<LearningLevel>(inputStatus, out StatusConverted);
            if (!isEnum) { return; }
            Console.WriteLine(StatusConverted);

            if (!isCityTakenFromDB())
            {
                Console.WriteLine("Błąd Połączenia");
                return;
            }

            Console.WriteLine("Wpisz z jakiego miasta się logujesz");
            //Printout list of cities.
            foreach (var list in Location.All)
            {
                Console.WriteLine($"{list.Location_id} to {list.City}");
            }

            string? inputLocation = Console.ReadLine();
            int Locationint;
            bool isNumber = int.TryParse(inputLocation, out Locationint);
            if (!isNumber) 
            {
                Console.WriteLine("Nieprawidłowy numer miasta");
                return; 
            }
            if (!isListContainsInput(Locationint)) 
            {
                return; 
            }

            CurrentUser User = new CurrentUser(inputLogin, inputPassword, inputName, inputSurname, StatusConverted, Locationint);
            SendFormula(User);
        }

        private void SendFormula(CurrentUser currentUser)
        {
            bool isRegistrationSuccess;
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute("INSERT INTO  VALUES(DEFAULT, @Login, @Password, @Name, @Surname, @LearningStatus, @LocationId, @LevelOfAccess)", currentUser);
                }
                catch
                {
                    Console.WriteLine("Nie udało się wysłać formularza. Spróbuj jeszcze raz");
                    Console.ReadLine();
                    Console.Clear();
                    return;
                }
                Console.WriteLine("Rejestracja powiodła się!");
                Console.ReadLine();
                Console.Clear();
                return;
            }
        }
        private bool isCityTakenFromDB()
        {
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    Location.All = RegistrationConnection.Query<Location>($"SELECT * FROM location").ToList();
                    //var inputsql = RegistrationConnection.QueryMultiple($"SELECT city FROM Location;");
                }
                catch 
                {
                    Location.All.Clear();
                    return false;
                }
                return true;
            }
        }
        private bool isListContainsInput(int input) 
        {
            foreach (var list in Location.All)
            {
                if (list.Location_id.Equals(input))
                {
                    return true;
                }
            }
            Console.WriteLine("Brak miasta w bazie! Spróbuj Ponownie");
            Console.ReadLine();
            Console.Clear();
            return false;
        }
    }
}
