using Dapper;
using JokeManagment.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Client
{
    public class JokeMenu
    {
        CurrentUser currentUser { get; set; }

        public JokeMenu(CurrentUser _currentUser) 
        {
            currentUser = _currentUser;
        }

        public void Menu()
        {
            bool isUserQuittedMenu = false;
            while (!isUserQuittedMenu)
            {
                Console.WriteLine("1.Przeczytaj żart");
                Console.WriteLine("2.Losuj żart do przeczytania");
                Console.WriteLine("3.Dodaj nowy żart");
                Console.WriteLine("0.Wróć");

                string input = Console.ReadLine();
                int inputInt = input.CheckIfNumberInRange(3);
                if (inputInt == -1 || inputInt == 0)
                {
                    Console.WriteLine("Błąd wpisanej wartości");
                    return;
                }
                else if (inputInt == 1)
                {
                    ReadJoke();
                }
                else if (inputInt == 2)
                {
                    ReadRandomJoke();
                }
                else if (inputInt == 3 && ((int)currentUser.LevelOfAccess) == 1)//Option for Admin
                {
                    AddJoke();
                }
                else
                {
                    Console.WriteLine("Nie wystarczające uprawnienia dla tej opcji");
                }
            }
            
        }
        private void ReadJoke()
        {
            string Sqlstringgettype = $"SELECT * FROM JokeType";
            List<JokeType> jokeTypes = new List<JokeType>();
            jokeTypes = GetJokeFormDB<JokeType>(Sqlstringgettype);

            if (jokeTypes == null)
            {
                return;
            }
            Console.WriteLine("Wpisz jaką kategorie żartu chcesz przeczytać");
            foreach (JokeType joke in jokeTypes) 
            {
                Console.WriteLine($"{joke.category_joke_id}. {joke.joke_category}");
            }

            bool isVaild = int.TryParse(Console.ReadLine(), out int inputId);
            if (!isVaild)
            {
                Console.WriteLine("Niepoprawna wartośc");
                Console.ReadKey();
                return;
            }

            JokeType jokeTypeChosen = jokeTypes.FirstOrDefault(type => type.category_joke_id == inputId);
            if (jokeTypeChosen == null)
            {
                Console.WriteLine("Nie znaleziono kategorii");
                Console.ReadKey();
                return;
            }

            string Sqlstringgetjoke = $"SELECT * FROM JokeBase WHERE category_joke_id = {jokeTypeChosen.category_joke_id};";
            List<JokeBase> JokeList = new List<JokeBase>();
            JokeList = GetJokeFormDB<JokeBase>(Sqlstringgetjoke);

            if (JokeList == null)
            {
                return;
            }

            Console.WriteLine("Który numer z poniższych żartów chcesz przeczytać?");
            foreach (JokeBase joke in JokeList)
            {
                Console.WriteLine($"{joke.Joke_id}.");
            }

            bool isVaild2 = int.TryParse(Console.ReadLine(), out int inputId2);
            if (!isVaild)
            {
                Console.WriteLine("Niepoprawna wartośc");
                Console.ReadKey();
                return;
            }

            JokeBase JokeChosen = JokeList.FirstOrDefault(jokes =>  jokes.Joke_id == inputId2);
            if (JokeChosen == null)
            {
                Console.WriteLine("Nie znaleziono żartu");
                Console.ReadKey();
                return;
            }
            Console.WriteLine(JokeChosen.Joke_message);
        }

        private void ReadRandomJoke()
        {
            string SqlstringForIdJoke = $"SELECT joke_id FROM JokeBase";
            List<int> listjokeid = new List<int>();
            listjokeid = GetJokeFormDB<int>(SqlstringForIdJoke);

            if (listjokeid == null)
            {
                return;
            }

            var random = new Random();
            int indexOfRandomJoke = random.Next(0,listjokeid.Count);
            int idOfRandomJoke = listjokeid.ElementAt(indexOfRandomJoke);

            string jokemessage;
            using (var JokeConnection = ConnectionSQL.EstablishConnection())
            {
                //Send it to DB
                //if data match return object use
                //var DBoutput = loginConnection.Query<CurrentUser>($"SELECT * FROM users WHERE login = '{inputLogin}' AND password = '{inputPassword}'").ToList();     SQL COMMAND
                try
                {
                    jokemessage = JokeConnection.QuerySingleOrDefault<string>($"SELECT joke_message FROM JokeBase WHERE joke_id = {idOfRandomJoke}");//Stored Procedure
                }
                catch
                {
                    Console.WriteLine("Błąd pobrania losowego żartu,spróbuj ponownie.");
                    Console.ReadKey();
                    return;
                }
            }
            Console.WriteLine(jokemessage);
        }

        private void AddJoke() 
        {
            string Sqlstringaddjoke = $"INSERT INTO JokeBase VALUES(DEFAULT, )";

            string Sqlstringgetjokecategory = $"SELECT * FROM JokeType";
            List<JokeType> jokeTypes = new List<JokeType>();
            jokeTypes = GetJokeFormDB<JokeType>(Sqlstringgetjokecategory);
            if (jokeTypes == null)
            {
                return;
            }

            Console.WriteLine("Wybierz do jakiej kategori chcesz dodac żart");
            foreach (JokeType joke in jokeTypes)
            {
                Console.WriteLine($"{joke.category_joke_id}. {joke.joke_category}");
            }

            bool isVaild = int.TryParse(Console.ReadLine(),out int inputId);
            if (!isVaild ) 
            {
                Console.WriteLine("Niepoprawna wartośc");
                Console.ReadKey();
                return; 
            }

            JokeType jokeTypeChosen = jokeTypes.FirstOrDefault(type => type.category_joke_id == inputId);
            if (jokeTypeChosen == null)
            {
                Console.WriteLine("Nie znaleziono kategorii");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wpisz traść żartu.");
            string inputJokeMessage  = Console.ReadLine();
            if(!inputJokeMessage.isStringLengthCorrect(5000) || string.IsNullOrEmpty(inputJokeMessage)) { return; }

            using (var AddJokeConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    AddJokeConnection.Execute($"INSERT INTO JokeBase VALUES(DEFAULT, '{inputJokeMessage}', {jokeTypeChosen.category_joke_id});");
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

        private List<T> GetJokeFormDB<T>(string SQLCommand)
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
