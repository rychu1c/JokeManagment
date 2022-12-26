using Dapper;
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
            bool isUserInMenu = true;
            while (isUserInMenu)
            {
                List<string> ListStrings = new List<string>();
                ListStrings.Add("1.Wypożycz książkę");
                ListStrings.Add("2.Sprawdz swoje zadłużenie");
                ListStrings.Add("3.Oddaj książkę");
                ListStrings.Add("0.Wypożycz książkę");

                foreach (string str in ListStrings)
                {
                    Console.WriteLine(str);
                }

                bool isValid = int.TryParse(Console.ReadLine(), out int inputUserInt);
                if (!isValid || ListStrings.Count - 1 < inputUserInt || 0 > inputUserInt)
                {
                    Console.WriteLine("Błąd wpisanej wartości");
                    return;
                }

                switch (inputUserInt)
                {
                    case 0:
                        isUserInMenu = false;
                        break;
                    case 1:
                        BorrowBook();
                        break;
                    case 2:
                        CalculatePayment();
                        break;
                    case 3:
                        ReturnBook();
                        break;
                    default:
                        Console.WriteLine("Błąd");
                        isUserInMenu = false;
                        return;
                }
                Console.ReadLine();
                Console.Clear();
            }
        }

        private void BorrowBook()
        {
            if (isCountBookTakenCorrect() == false)
            {
                return;
            }

            string sqlStingCheckBooks = $"SELECT * FROM BookShelf WHERE user_id = NULL";
            List<BookShelf> ListAvailableBooks = StaticMethods.GetListFormDB<BookShelf>(sqlStingCheckBooks);
            if (ListAvailableBooks == null) { return; }

            foreach (BookShelf book in ListAvailableBooks)
            {
                Console.WriteLine($"{ListAvailableBooks.IndexOf(book)+1}. {book.bookname}");
            }

            bool isValid = int.TryParse(Console.ReadLine(), out int userInput);
            if (!isValid || ListAvailableBooks.Count < userInput || 0 >= userInput)
            {
                Console.WriteLine("Błąd wpisanej wartości");
                return;
            }

            DateTime CurrentTime = DateTime.Now;
            BookShelf pickedBook = ListAvailableBooks.ElementAt(userInput - 1);
            string sqlStringTakeBook = $"UPDATE BookShelf SET user_id = {currentUser.Id} AND borrow_date = {CurrentTime};";
            if (StaticMethods.isExecutSqlString(sqlStringTakeBook))
            {
                return;
            }
        }

        private bool isCountBookTakenCorrect()
        {
            string sqlStringCountBooks = $"SELECT COUNT(user_id)FROM BookShelf WHERE user_id = {currentUser.Id}";

            int? userCountBook = GetCount(sqlStringCountBooks);
            if (userCountBook == null)
            {
                return false;
            }
            else if (userCountBook >= 2)
            {
                Console.WriteLine("Limit wypożyczonych ksiażek na raz osiągniety");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CalculatePayment()
        {
            string sqlStingCheckYourBooks = $"SELECT * FROM BookShelf WHERE user_id = {currentUser.Id}";
            List<BookShelf> ListAvailableBooks = StaticMethods.GetListFormDB<BookShelf>(sqlStingCheckYourBooks);
            if (ListAvailableBooks == null) { return; }

            foreach (BookShelf book in ListAvailableBooks)
            {
                Console.WriteLine($"{ListAvailableBooks.IndexOf(book) + 1}. {book.bookname} Zaległa kwota to {CalculateDebt(book.borrow_date)}");
            }
        }

        private decimal CalculateDebt(DateTime booktime)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan diff1 = currentTime.Subtract(booktime);
            decimal moneyPerWeek = 10M;
            decimal moneyToPay = ((int)diff1.TotalDays / 7) * moneyPerWeek;
            return moneyToPay;
        }

        private void ReturnBook()
        {
            string sqlStingCheckYourBooks = $"SELECT * FROM BookShelf WHERE user_id = {currentUser.Id}";
            List<BookShelf> ListAvailableBooks = StaticMethods.GetListFormDB<BookShelf>(sqlStingCheckYourBooks);
            if (ListAvailableBooks == null) { return; }

            foreach (BookShelf book in ListAvailableBooks)
            {
                Console.WriteLine($"{ListAvailableBooks.IndexOf(book) + 1}. {book.bookname} Zaległa kwota to {CalculateDebt(book.borrow_date)}");
            }

            bool isValid = int.TryParse(Console.ReadLine(), out int userInput);
            if (!isValid || ListAvailableBooks.Count < userInput || 0 >= userInput)
            {
                Console.WriteLine("Błąd wpisanej wartości");
                return;
            }

            string sqlStringReturnBook = $"UPDATE BookShelf SET user_id = NULL AND borrow_date = NULL;";
            if (!StaticMethods.isExecutSqlString(sqlStringReturnBook))
            {
                return;
            }
        }

        private int? GetCount(string SQLCommand)
        {
            using (var Connection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    var DBOutput = Connection.QuerySingleOrDefault<int>($"SQLCommand");
                    return DBOutput;
                }
                catch
                {
                    Console.WriteLine("Błąd pobrania danych z bazy");
                    return null;
                }
            }
        }
    }
}
