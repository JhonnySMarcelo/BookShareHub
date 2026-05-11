using BookShareHub.Application.Books.DTOs;
using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookShareHub.BooksAPI.Controllers
{
    /// <summary>
    /// Provides endpoints to manage books in the system.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController"/>.
        /// </summary>
        /// <param name="bookService">The service responsible for book operations.</param>
        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Creates a new book record.
        /// </summary>
        /// <param name="request">
        /// The data transfer object containing the book details such as title, author, description, availability, and owner ID.
        /// </param>
        /// <returns>
        /// Returns the created <see cref="Book"/> with status code 201 if successful,
        /// or a <see cref="ValidationProblemDetails"/> with status code 400 if validation fails.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> Create([FromBody] CreateBookDto request)
        {
            var book = await _bookService.CreateAsync(request);

            return Created($"/api/books/{book.Id}", book);
        }

    }
}
