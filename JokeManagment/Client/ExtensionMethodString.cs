using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Client
{
    public static class ExtensionMethodString
    {
        public static int CheckInput(this string str,int numberOfWays)
        {
            if (str == null)
            {
                throw new Exception("Wartość niepoprawna");
            }
            bool _isNumber = int.TryParse(str, out int result);
            if (!_isNumber)
            {
                throw new Exception("Wartość niepoprawna");
            }
            if (numberOfWays < result)
            {
                throw new Exception("Wartość niepoprawna");
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
                return false;
            }
            return true;
        }
    }
}
