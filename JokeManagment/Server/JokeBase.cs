using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Server
{
    public class JokeBase
    {
        public int Joke_id {get; set;}
        public string Joke_message { get; set;}
        public int Category_joke_id { get; set;}
    }
}
