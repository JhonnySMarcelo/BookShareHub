using BookShareHub.Application.Books.DTOs;
using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShareHub.BooksAPI.Controllers
{
    /// <summary>
    /// Provides endpoints to manage books in the system.
    /// </summary>
    [Authorize]
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
        [Authorize]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> Create([FromBody] CreateBookDto request)
        {
            var book = await _bookService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        /// <summary>
        /// Retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <returns>
        /// Returns the <see cref="Book"/> with status code 200 if found,
        /// or 404 if not found.
        /// Validation errors (e.g., empty Guid) are automatically returned as <see cref="ValidationProblemDetails"/> with status code 400.
        /// </returns>
        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> GetById(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);

            if (book == null) return NotFound();

            return Ok(book);
        }

        /// <summary>
        /// Retrieves all books in the system.
        /// </summary>
        /// <returns>
        /// Returns a list of <see cref="Book"/> with status code 200 if any are found,
        /// or 404 if no books exist.
        /// </returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        {
            var books = await _bookService.GetAllAsync();

            if (books == null || !books.Any())
                return NotFound();

            return Ok(books);
        }

        /// <summary>
        /// Partially updates a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <param name="dto">The fields to update (only non-null values will be applied).</param>
        /// <returns>
        /// Returns the updated <see cref="Book"/> with status code 200 if successful,
        /// 404 if not found,
        /// or 400 if validation fails.
        /// </returns>
        [HttpPatch("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> Update(Guid id, [FromBody] UpdateBookDto dto)
        {
            var updatedBook = await _bookService.PatchAsync(id, dto);

            if (updatedBook == null)
                return NotFound();

            return Ok(updatedBook);
        }

        /// <summary>
        /// Deletes a book by its unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the book to be deleted.
        /// </param>
        /// <returns>
        /// Returns:
        /// - <see cref="StatusCodes.Status204NoContent"/> if the book was successfully deleted.
        /// - <see cref="StatusCodes.Status404NotFound"/> if the book does not exist.
        /// - <see cref="ValidationProblemDetails"/> with <see cref="StatusCodes.Status400BadRequest"/> 
        ///   if the provided <paramref name="id"/> is invalid or the book is unavailable.
        /// </returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _bookService.DeleteAsync(id);

            if (result == null)
                return NotFound();

            return NoContent();
        }
    }
}
