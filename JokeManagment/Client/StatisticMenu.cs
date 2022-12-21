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
            string sqlStringCheckCityStatistics = $"";
        }
    }
}
