import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookService } from '../book-service';
import { Book } from '../Book';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './book-list.html',
  styleUrl: './book-list.scss',
})
export class BookList implements OnInit {
  books = signal<Book[]>([]);
  editingBookId = signal<string | null>(null);

  newBook: Partial<Book> = {
    title: '',
    author: '',
    description: '',
    available: true,
  };

  editBook: Partial<Book> = {
    title: '',
    author: '',
    description: '',
    available: true,
  };

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.bookService.getAll().subscribe({
      next: (data) => {
        console.log(data);
        this.books.set(data);
      },
      error: (err) => {
        console.error(err);
        alert('Failed to load books');
      },
    });
  }

  createBook(): void {
    this.bookService.create(this.newBook).subscribe({
      next: (book) => {
        this.books.update((books) => [...books, book]);

        this.newBook = {
          title: '',
          author: '',
          description: '',
          available: true,
        };
      },
      error: (err) => {
        console.error(err);

        alert('Failed to create book');
      },
    });
  }

  startEdit(book: Book): void {
    this.editingBookId.set(book.id);

    this.editBook = {
      title: book.title,
      author: book.author,
      description: book.description,
      available: book.available,
    };
  }

  cancelEdit(): void {
    this.editingBookId.set(null);

    this.editBook = {
      title: '',
      author: '',
      description: '',
      available: true,
    };
  }

  updateBook(id: string): void {
    this.bookService.update(id, this.editBook).subscribe({
      next: (updatedBook) => {
        this.books.update((books) => books.map((book) => (book.id === id ? updatedBook : book)));

        this.cancelEdit();
      },
      error: (err) => {
        console.error(err);

        alert('Failed to update book');
      },
    });
  }

  deleteBook(id: string): void {
    this.bookService.delete(id).subscribe({
      next: () => {
        this.books.update((books) => books.filter((b) => b.id !== id));
      },
      error: (err) => {
        console.error(err);

        alert('Failed to delete book');
      },
    });
  }
}
