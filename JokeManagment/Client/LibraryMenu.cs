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
                ListStrings.Add("0.Wróć");

                foreach (string str in ListStrings)
                {
                    Console.WriteLine(str);
                }

                bool isValid = int.TryParse(Console.ReadLine(), out int inputUserInt);
                if (!isValid || ListStrings.Count - 1 < inputUserInt || 0 > inputUserInt)
                {
                    Console.WriteLine("Błąd wpisanej wartości. Wciśnij dowolny klawisz by kontynuować.");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                Console.Clear();

                switch (inputUserInt)
                {
                    case 0:
                        isUserInMenu = false;
                        return;
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
                Console.WriteLine("Osiągnieto już limit pobranych książek lub błąd pobrania danych. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            string sqlStingCheckBooks = $"SELECT * FROM BookShelf WHERE user_id = NULL";
            List<BookShelf> ListAvailableBooks = StaticMethods.GetListFormDB<BookShelf>(sqlStingCheckBooks);
            if (ListAvailableBooks == null) 
            {
                Console.WriteLine("Błąd pobrania pobrania danych");
                return;
            }
            if (ListAvailableBooks.Count == 0)
            {
                Console.WriteLine("Nie ma wolnych książek do wypozyczenia. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            foreach (BookShelf book in ListAvailableBooks)
            {
                Console.WriteLine($"{ListAvailableBooks.IndexOf(book)+1}. {book.bookname}");
            }

            bool isValid = int.TryParse(Console.ReadLine(), out int userInput);
            if (!isValid || ListAvailableBooks.Count < userInput || 0 >= userInput)
            {
                Console.WriteLine("Błąd wpisanej wartości. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            DateTime CurrentTime = DateTime.Now;
            BookShelf pickedBook = ListAvailableBooks.ElementAt(userInput - 1);
            string sqlStringTakeBook = $"UPDATE BookShelf SET user_id = {currentUser.user_id} AND borrow_date = {CurrentTime};";
            if (StaticMethods.isExecutSqlString(sqlStringTakeBook))
            {
                return;
            }
        }

        private bool isCountBookTakenCorrect()
        {
            string sqlStringCountBooks = $"SELECT COUNT(*) FROM BookShelf WHERE user_id = {currentUser.user_id}";

            int? userCountBook = GetCount(sqlStringCountBooks);
            if (userCountBook == null )
            {
                return false;
            }
            else if (userCountBook >= 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CalculatePayment()
        {
            string sqlStingCheckYourBooks = $"SELECT * FROM BookShelf WHERE user_id = {currentUser.user_id}";
            List<BookShelf> ListAvailableBooks = StaticMethods.GetListFormDB<BookShelf>(sqlStingCheckYourBooks);
            if (ListAvailableBooks == null)
            {
                Console.WriteLine("Błąd pobrania danych. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            if (ListAvailableBooks.Count == 0) 
            {
                Console.WriteLine("Nie masz żadnych wypożyczonych książek. Wciśnij dowolny klawisz by kontynuować.");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            foreach (BookShelf book in ListAvailableBooks)
            {
                Console.WriteLine($"{ListAvailableBooks.IndexOf(book) + 1}. {book.bookname} Zaległa kwota to {CalculateDebt(book.borrow_date)}");
            }
            Console.WriteLine("Wciśnij dowolny klawisz by kontynuować");
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
            string sqlStingCheckYourBooks = $"SELECT * FROM BookShelf WHERE user_id = {currentUser.user_id}";
            List<BookShelf> ListAvailableBooks = StaticMethods.GetListFormDB<BookShelf>(sqlStingCheckYourBooks);
            if (ListAvailableBooks == null) 
            {
                Console.WriteLine("Błąd serwera. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
            if (ListAvailableBooks.Count == 0)
            {
                Console.WriteLine("Nie masz żadnej ksiązki do zwrócenia. Wciśij dowolny klawisz by kontynuować.");
                return;
            }

            Console.WriteLine("Wybierz którą ksiażkę chcesz zwrócić.");
            foreach (BookShelf book in ListAvailableBooks)
            {
                Console.WriteLine($"{ListAvailableBooks.IndexOf(book) + 1}. {book.bookname} Zaległa kwota to {CalculateDebt(book.borrow_date)}");
            }

            bool isValid = int.TryParse(Console.ReadLine(), out int userInput);
            if (!isValid || ListAvailableBooks.Count < userInput || 0 >= userInput)
            {
                Console.WriteLine("Błąd wpisanej wartości. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            string sqlStringReturnBook = $"UPDATE BookShelf SET user_id = NULL AND borrow_date = NULL;";
            if (StaticMethods.isExecutSqlString(sqlStringReturnBook))
            {
                return;
            }
            Console.WriteLine("Wciśnij dowolny klawisz by kontynuować.");
        }

        private int? GetCount(string SQLCommand)
        {
            using (var Connection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    int? DBOutput = Connection.QueryFirst<int>(SQLCommand);
                    return DBOutput;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
