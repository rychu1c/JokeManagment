using JokeManagment.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Client
{
    public class MainMenu
    {
        public CurrentUser _currentUser;

        public void StartMenu()
        {
            Console.WriteLine("Witaj w aplikacji");
            LoginOrRegisterMenu();
            MenuAfterLogin();
        }

        private void LoginOrRegisterMenu()
        {
            bool IsCurrentUserNull = true;
            while (IsCurrentUserNull)
            {
                List<string> ListOptions= new List<string>();
                ListOptions.Add("1.Zaloguj się");
                ListOptions.Add("2.Zarejestruj się");
                ListOptions.Add("0.Wyjdz z aplikacji");

                bool isVaildInput = int.TryParse(Console.ReadLine(),out int inputNumber);
                if (!isVaildInput) 
                {
                    Console.WriteLine("Błędnie wpisana wartość");
                    continue;
                }

                switch (inputNumber)
                {
                    case 1:
                        LoginLogic log = new LoginLogic();
                        _currentUser = log.MenuLogin();
                        break;
                    case 2:
                        RegistrationLogic reg = new RegistrationLogic();
                        reg.MenuRegistration();
                        break;
                    case 0:
                        Console.WriteLine("Wyjdz z aplikcaji");
                        IsCurrentUserNull = false;
                        return;
                    default:
                        Console.WriteLine("Wpisana wartość niepoprawna");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }
                if (_currentUser != null) 
                {
                    IsCurrentUserNull = false;
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private void MenuAfterLogin()
        {
            
            bool IsUserLogin = true;
            while (IsUserLogin)
            {
                

                List<string> Liststrings= new List<string>();
                Liststrings.Add("1.Żarty");
                Liststrings.Add("2.Korepetycje");
                Liststrings.Add("3.Biblioteka");
                Liststrings.Add("4.Menu Administratora");
                Liststrings.Add("5.Statystyka");
                Liststrings.Add("0.Wyloguj z aplikacji");

                Console.WriteLine("Wpisz numer menu do którego chcesz przejść");
                PrintOutList(Liststrings);

                bool isValid = int.TryParse(Console.ReadLine(), out int userInputInt);
                if (!isValid || Liststrings.Count < userInputInt || 0 > userInputInt)
                {
                    Console.WriteLine("Wprowadzono nie poprawną wartość, spróbuj ponownie");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                switch (userInputInt)
                {
                    case 1:
                        new JokeMenu(_currentUser).Menu();
                        continue;
                    case 2:
                        new LessonMenu(_currentUser).Menu();
                        continue;
                    case 3:
                        new LibraryMenu(_currentUser).Menu();
                        continue;
                    case 4:
                        new AdministationMenu(_currentUser).Menu();
                        continue;
                    case 5:
                        new StatisticMenu(_currentUser).Menu();
                        continue;
                    default:
                        Console.WriteLine("Wyjscie z aplikacji");
                        IsUserLogin = false;
                        _currentUser = null;
                        return;
                }
            }
            LoginOrRegisterMenu();
        }

        private void PrintOutList(List<string> Liststring)
        {
            foreach (string str in Liststring)
            {
                Console.WriteLine(str);
            }
        }
    }
}
