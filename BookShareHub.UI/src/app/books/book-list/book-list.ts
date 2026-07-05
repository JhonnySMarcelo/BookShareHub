import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BookService } from '../book-service';
import { NotificationService } from '../../core/services/notification.service';
import { getErrorMessage } from '../../core/erros/error-utils';
import { Book } from '../models/Book';

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
  private originalBook: Book | null = null;

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

  constructor(
    private bookService: BookService,
    private notification: NotificationService,
  ) {}

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
        this.notification.error(getErrorMessage(err));
      },
    });
  }

  createBook(): void {
    this.bookService.create(this.newBook).subscribe({
      next: (book) => {
        this.books.update((books) => [...books, book]);
        this.notification.success('Book created successfully!');

        this.newBook = {
          title: '',
          author: '',
          description: '',
          available: true,
        };
      },
      error: (err) => {
        this.notification.error(getErrorMessage(err));
      },
    });
  }

  startEdit(book: Book): void {
    if (!book.isOwner) {
      return;
    }

    this.originalBook = { ...book };
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
    const patch = this.buildPatchDto();

    if (Object.keys(patch).length === 0) {
      this.cancelEdit();
      return;
    }
    this.bookService.update(id, patch).subscribe({
      next: (updatedBook) => {
        this.books.update((books) => books.map((book) => (book.id === id ? updatedBook : book)));

        this.cancelEdit();
      },
      error: (err) => {
        this.notification.error(getErrorMessage(err));
      },
    });
  }

  private buildPatchDto(): Partial<Book> {
    if (!this.originalBook) {
      return {};
    }

    const patch: Partial<Book> = {};

    if (this.editBook.title !== this.originalBook.title) {
      patch.title = this.editBook.title;
    }

    if (this.editBook.author !== this.originalBook.author) {
      patch.author = this.editBook.author;
    }

    if (this.editBook.description !== this.originalBook.description) {
      patch.description = this.editBook.description;
    }

    if (this.editBook.available !== this.originalBook.available) {
      patch.available = this.editBook.available;
    }

    return patch;
  }

  deleteBook(id: string): void {
    this.bookService.delete(id).subscribe({
      next: () => {
        this.books.update((books) => books.filter((b) => b.id !== id));
        this.notification.success('Book deleted successfully!');
      },
      error: (err) => {
        this.notification.error(getErrorMessage(err));
      },
    });
  }
}
