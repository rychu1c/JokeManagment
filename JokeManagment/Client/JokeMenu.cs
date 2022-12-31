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
                List<string> ListString= new List<string>();
                ListString.Add("1.Przeczytaj żart");
                ListString.Add("2.Losuj żart do przeczytania");
                ListString.Add("3.Dodaj nowy żart");
                ListString.Add("0.Wróć");

                Console.WriteLine("Wybierz z dostępnych opcji.");
                foreach (string strOption in ListString)
                {
                    Console.WriteLine(strOption);
                }

                bool isValid = int.TryParse(Console.ReadLine(), out int inputUserInt);
                if (!isValid || ListString.Count <= inputUserInt || 0 > inputUserInt)
                {
                    Console.WriteLine("Wpisano niepoprawną wartość. Wciśnij dowolny klawisz by kontynuować.");
                    Console.ReadKey();
                    Console.Clear();
                    isUserQuittedMenu = true;
                    continue;
                }
                Console.Clear();

                switch (inputUserInt)
                {
                    case 0:
                        isUserQuittedMenu = true;
                        break;
                    case 1:
                        ReadJoke();
                        break;
                    case 2:
                        ReadRandomJoke();
                        break;
                    case 3:
                        if (((int)currentUser.LevelOfAccess) != 1)
                        {
                            Console.WriteLine("Nie wystarczające uprwanienie dla wybranej czynnosci. Wciśnij dowolny klawisz by kontynuować.");
                            Console.ReadKey();
                            break;
                        }
                        AddJoke();
                        break;
                }
                Console.Clear();
            }
            
        }
        private void ReadJoke()
        {
            //Get list of joke type
            string Sqlstringgettype = $"SELECT * FROM JokeType";
            List<JokeType> jokeTypes = new List<JokeType>();
            jokeTypes = GetJokeFormDB<JokeType>(Sqlstringgettype);
            if (jokeTypes == null || jokeTypes.Count == 0)
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
                Console.WriteLine("Niepoprawna wartośc. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                return;
            }
            
            //Based on user choice pick specific type of joke 
            JokeType jokeTypeChosen = jokeTypes.FirstOrDefault(type => type.category_joke_id == inputId);
            if (jokeTypeChosen == null)
            {
                Console.WriteLine("Nie znaleziono kategorii. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                return;
            }

            //Get list of jokes with specific type
            string Sqlstringgetjoke = $"SELECT * FROM JokeBase WHERE category_joke_id = {jokeTypeChosen.category_joke_id};";
            List<JokeBase> JokeList = new List<JokeBase>();
            JokeList = GetJokeFormDB<JokeBase>(Sqlstringgetjoke);

            if (JokeList == null || JokeList.Count == 0)
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
                Console.WriteLine("Niepoprawna wartość. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                return;
            }

            JokeBase JokeChosen = JokeList.FirstOrDefault(jokes =>  jokes.Joke_id == inputId2);
            if (JokeChosen == null)
            {
                Console.WriteLine("Nie znaleziono żartu. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine(JokeChosen.Joke_message);
            CountJoke();
        }

        private void ReadRandomJoke()
        {
            string SqlstringForIdJoke = $"SELECT joke_id FROM JokeBase";
            List<int> listjokeid = new List<int>();
            listjokeid = GetJokeFormDB<int>(SqlstringForIdJoke);

            if (listjokeid == null || listjokeid.Count == 0)
            {
                return;
            }

            var random = new Random();
            int indexOfRandomJoke = random.Next(0,listjokeid.Count);
            int idOfRandomJoke = listjokeid.ElementAt(indexOfRandomJoke);

            string jokemessage;
            using (var JokeConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    jokemessage = JokeConnection.QuerySingleOrDefault<string>($"SELECT joke_message FROM JokeBase WHERE joke_id = {idOfRandomJoke}");
                }
                catch
                {
                    Console.WriteLine("Błąd pobrania losowego żartu. Wciśnij dowolny klawisz by kontynuować..");
                    Console.ReadKey();
                    return;
                }
            }
            Console.WriteLine(jokemessage);
            CountJoke();
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
                Console.WriteLine("Niepoprawna wartośc. Wciśnij dowolny kalwisz by kontynuować.");
                Console.ReadKey();
                return; 
            }

            JokeType jokeTypeChosen = jokeTypes.FirstOrDefault(type => type.category_joke_id == inputId);
            if (jokeTypeChosen == null)
            {
                Console.WriteLine("Nie znaleziono kategorii. Wciśnij dowolny klawisz by kontynuować.");
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
                    Console.WriteLine("Nie udało się wysłać formularza. Wciśnij dowolny klawisz by kontynuować.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                Console.WriteLine("Rejestracja powiodła się. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
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
                    Console.WriteLine("Błąd pobrania listy. Wciśnij dowolny klawisz by kontynuować.");
                    Console.ReadKey();
                    return list = null;
                }
            }
            return list;
        }

        private void CountJoke()
        {
            string SqlstringCountJoke = $"UPDATE JokeLocationStatistic SET readjokecount = readjokecount + 1 WHERE location_id = {currentUser.location_id}";
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute(SqlstringCountJoke);
                }
                catch
                {
                    Console.WriteLine("Błąd naliczenia do statystyk. Wciśnij dowolny klawisz by kontynuować.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                Console.WriteLine("przeczytanie naliczone do statystyk. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                Console.Clear();
                return;
            }

        }

    }
}
