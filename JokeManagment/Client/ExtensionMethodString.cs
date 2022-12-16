using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Client
{
    public static class ExtensionMethodString
    {
        //Check if number is in range then return if is not return -1
        public static int CheckIfNumberInRange(this string str,int range)
        {
            bool _isNumber = int.TryParse(str, out int result);
            if (!_isNumber || range < result || result <= 0)
            {
                Console.WriteLine("Wartość niepoprawna");
                return result = -1;
            }
            return result;
        }
        public static string TakeInputWithCheck()
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception("Nie wpisano wartości");
            }
            return input;
        }
        //Check input if it's too long
        public static bool isStringLengthCorrect(this string str,int numberOfCharLim)
        {
            if (str.Length > numberOfCharLim) 
            {
                Console.WriteLine("Błąd, wpisano zbyt wiele znaków.");
                return false;
            }
            return true;
        }
    }
}
