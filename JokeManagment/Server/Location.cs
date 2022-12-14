using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Server
{
    public class Location
    {
        public int Location_id { get; set; }
        public string? City { get; set; }

        public static List<Location> All { get; set; } = new List<Location>();
    }
}
