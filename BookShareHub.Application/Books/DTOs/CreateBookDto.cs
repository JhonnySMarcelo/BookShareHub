using System;
using System.Collections.Generic;
using System.Text;

namespace BookShareHub.Application.Books.DTOs
{
    public record CreateBookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
    }
}
