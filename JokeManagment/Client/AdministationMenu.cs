using Dapper;
using JokeManagment.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Client
{
    public class AdministationMenu
    {
        CurrentUser currentUser { get; set; }

        public AdministationMenu(CurrentUser _currentUser)
        {
            currentUser = _currentUser;
        }

        public void Menu()
        {
            if (((int)currentUser.LevelOfAccess) == 1)
            {
                MenuLogic();
            }
            else
            {
                Console.WriteLine("Brak dostępu. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void MenuLogic()
        {
            bool isUserInMenu = true;
            while (isUserInMenu)
            {
                List<string> List = new List<string>();
                List.Add("1.Dodaj administratora");
                List.Add("2.Usuń administratora");
                List.Add("3.Wróć");

                bool isValid = int.TryParse(Console.ReadLine(), out int userInputInt);
                if (!isValid || List.Count < userInputInt || 0 > userInputInt)
                {
                    Console.WriteLine("Błąd wpisanej wartości. Wciśnij dowolny klawisz by kontynuować.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }

                switch (userInputInt)
                {
                    case 1:
                        AddAdministrator();
                        break;
                    case 2:
                        DeleteAdministrator();
                        break;
                    default:
                        isUserInMenu = true;
                        break;
                }
            }
        }

        private void AddAdministrator()
        {
            string SqlStrigGetListOfUsers = $"SELECT * FROM Users WHERE levelofaccess = 0";
            List < CurrentUser > ListUsers = GetListFromDB<CurrentUser>(SqlStrigGetListOfUsers);
            if (ListUsers == null || ListUsers.Count == 0)
            {
                Console.WriteLine("Lista użytkowników jest pusta. Wciśnij dowolny klawisz by kontynuować");
                return;
            }

            Console.WriteLine("Lista użytkowników");
            foreach (CurrentUser User in ListUsers)
            {
                Console.WriteLine($"{ListUsers.IndexOf(User) +1}. {User.Name} {User.Surname}");
            }

            bool isValid = int.TryParse(Console.ReadLine(), out int userInputInt);
            if (ListUsers.Count < userInputInt || userInputInt >= 0)
            {
                Console.WriteLine("Błąd zła wartość. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            CurrentUser PickedUser = ListUsers.ElementAt(userInputInt + 1);

            string SqlStringAddAdmin = $"UPDATE Users SET levelofaccess = 1 WHERE user_id = {PickedUser.user_id}";

            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute($"{SqlStringAddAdmin}");
                }
                catch
                {
                    Console.WriteLine("Nie udało się nadać praw. Wciśnij dowolny klawisz by kontynuować");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                Console.WriteLine("Zmieniono prawa na administratora. Wciśnij dowolny klawisz by kontynuować");
                Console.ReadKey();
                Console.Clear();
                return;
            }
        }

        private void DeleteAdministrator()
        {
            string SqlStrigGetListOfUsers = $"SELECT * FROM Users WHERE levelofaccess = 1";
            List<CurrentUser> ListUsers = GetListFromDB<CurrentUser>(SqlStrigGetListOfUsers);
            if (ListUsers == null || ListUsers.Count == 0)
            {
                Console.WriteLine("Lista użytkowników jest pusta. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            Console.WriteLine("Lista użytkowników");
            foreach (CurrentUser User in ListUsers)
            {
                Console.WriteLine($"{ListUsers.IndexOf(User) + 1}. {User.Name} {User.Surname}");
            }

            bool isValid = int.TryParse(Console.ReadLine(), out int userInputInt);
            if (ListUsers.Count < userInputInt || userInputInt >= 0)
            {
                Console.WriteLine("Błąd zła wartość. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            CurrentUser PickedUser = ListUsers.ElementAt(userInputInt + 1);

            string SqlStringDeliteUser = $"UPDATE Users SET levelofaccess = 0 WHERE user_id = {PickedUser.user_id}";

            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute($"{SqlStringDeliteUser}");
                }
                catch
                {
                    Console.WriteLine("Nie udało się zminić");
                    Console.ReadLine();
                    Console.Clear();
                    return;
                }
                Console.WriteLine("Zmieniono prawa na użytkownika");
                Console.ReadLine();
                Console.Clear();
                return;
            }
        }

        private List<T> GetListFromDB<T>(string SQLCommand)
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
    }

}
