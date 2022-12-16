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
                Console.WriteLine("Wpisz 1 by się zalogowac, 2 jeżeli chcesz się zarejestrować, a 0 żeby wyjść z aplikacji");
                string _input = Console.ReadLine();
                int inputNumber = _input.CheckIfNumberInRange(2);//If user input number is valid ,convert input to string by method 
                if (inputNumber.Equals(-1)) return;

                if (inputNumber == 1)
                {
                    Console.WriteLine("wpisałeś 1");
                    LoginLogic log = new LoginLogic();

                    _currentUser = log.MenuLogin();
                    if (_currentUser != null)
                    {
                        Console.WriteLine("current user not null");
                        IsCurrentUserNull = false;
                    }
                    else 
                    {
                        Console.WriteLine("current user null");
                        IsCurrentUserNull= false;
                    }
                }
                if (inputNumber == 2)
                {
                    Console.WriteLine("wpisałeś 2");
                    RegistrationLogic reg = new RegistrationLogic();
                    reg.MenuRegistration();
                }
            }
        }

        private void MenuAfterLogin()
        {
            bool IsUserLogin = true;
            while (IsUserLogin)
            {
                Console.WriteLine("Wpisz numer menu do którego chcesz przejść");
                Console.WriteLine("1.Żarty");
                Console.WriteLine("2.Korepetycje");
                Console.WriteLine("3.Biblioteka");
                Console.WriteLine("4.Menu Administratora");
                Console.WriteLine("5.Statystyka");
                Console.WriteLine("0.Wyloguj z aplikacji");

                string _input = Console.ReadLine();
                int inputNumber = _input.CheckIfNumberInRange(5);

                switch (inputNumber)
                {
                    case -1:
                        Console.WriteLine("Wybrano zły numer!");
                        continue;
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
                        IsUserLogin = false;
                        _currentUser = null;
                        break;
                }
            }
            LoginOrRegisterMenu();
        }
    }
}
