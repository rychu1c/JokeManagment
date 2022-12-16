using JokeManagment.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Client
{
    public class LibraryMenu
    {

        CurrentUser currentUser { get; set; }

        public LibraryMenu(CurrentUser _currentUser)
        {
            currentUser = _currentUser;
        }

        public void Menu()
        {

        }
    }
}
