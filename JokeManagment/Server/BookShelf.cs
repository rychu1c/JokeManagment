﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Server
{
    public class BookShelf
    {
        public int book_id { get; set; }
        public string bookname { get; set; }
        public int book_type { get; set; }
        public int user_id { get; set; }
        public DateTime borrow_date { get; set; }
    }
}
