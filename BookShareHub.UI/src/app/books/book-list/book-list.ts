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

  newBook: Partial<Book> = {
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
