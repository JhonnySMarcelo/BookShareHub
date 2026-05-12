import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../book-service';
import { Book } from '../Book';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './book-list.html',
})
export class BookList implements OnInit {
  books: Book[] = [];
  newBook: Partial<Book> = { title: '', author: '', description: '', available: true };

  constructor(private bookService: BookService) {}

  ngOnInit() {
    this.loadBooks();
  }

  loadBooks() {
    this.bookService.getAll().subscribe({
      next: (data) => (this.books = data),
      error: () => alert('Failed to load books'),
    });
  }

  createBook() {
    this.bookService.create(this.newBook).subscribe({
      next: (book) => {
        this.books.push(book);
        this.newBook = { title: '', author: '', description: '', available: true };
      },
      error: () => alert('Failed to create book'),
    });
  }

  deleteBook(id: string) {
    this.bookService.delete(id).subscribe({
      next: () => (this.books = this.books.filter((b) => b.id !== id)),
      error: () => alert('Failed to delete book'),
    });
  }
}
