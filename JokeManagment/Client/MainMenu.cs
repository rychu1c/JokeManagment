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
        }
        private void LoginOrRegisterMenu()
        {
            bool IsCurrentUserNull = true;
            while (IsCurrentUserNull)
            {
                Console.WriteLine("Wpisz 1 by się zalogowac, 2 jeżeli chcesz się zarejestrować, a 0 żeby wyjść z aplikacji");
                string _input = Console.ReadLine();
                int inputNumber = _input.CheckInput(2);//If user input number is valid ,convert input to string by method 

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
    }
}
