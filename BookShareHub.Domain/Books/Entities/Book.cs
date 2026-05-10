using System;
using System.Collections.Generic;
using System.Text;

namespace BookShareHub.Domain.Books.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
    }
}
