using Dapper;
using JokeManagment.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private void CheckStatisticForCities()
        {
            string sqlStringCheckCityStatistics = $"SELECT Location.location_id, JokeLocationStatistic.readjokecount,  Location.City FROM JokeLocationStatistic FULL JOIN Location ON Location.location_id = JokeLocationStatistic.location_id ORDER BY Location.location_id ";

            List < StatisticCities > CitiesList = GetListFromDB<StatisticCities>(sqlStringCheckCityStatistics);
            if (CitiesList == null || CitiesList.Count ==  0)
            {
                Console.WriteLine("Błąd listy");
                return;
            }

            foreach (StatisticCities city in CitiesList)
            {
                Console.WriteLine($"{city.city} = {city.readjokecount}");
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
