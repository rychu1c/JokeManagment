using Dapper;
using JokeManagment.Server;

namespace JokeManagment.Client
{
    public class StatisticMenu
    {
        CurrentUser currentUser { get; set; }

        public StatisticMenu(CurrentUser _currentUser)
        {
            currentUser = _currentUser;
        }

        public void Menu()
        {
            CheckStatisticForCities();
            Console.ReadKey();
            Console.Clear();
        }

        private void CheckStatisticForCities()
        {
            string sqlStringCheckCityStatistics = $"SELECT Location.location_id, JokeLocationStatistic.readjokecount,  Location.City FROM JokeLocationStatistic FULL JOIN Location ON Location.location_id = JokeLocationStatistic.location_id ORDER BY Location.location_id ";

            List<StatisticCities> CitiesList = GetListFromDB<StatisticCities>(sqlStringCheckCityStatistics);
            if (CitiesList == null || CitiesList.Count == 0)
            {
                Console.WriteLine("Błąd listy. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            foreach (StatisticCities city in CitiesList)
            {
                Console.WriteLine($"{city.city} = {city.readjokecount}");
            }
            Console.WriteLine("Wciśnij dowolny klawisz by kontynuować.");
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
                    Console.WriteLine("Błąd pobrania listy. Wciśnij dowolny klawisz by kontynuować.");
                    return list = null;
                }
            }
            return list;
        }
    }
}
