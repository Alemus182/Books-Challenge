import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import endpoints from './endpoints';
import { Observable } from 'rxjs';
import { BookSearchResponse } from 'app/models/book-search-response.model';

@Injectable()
export class BooksService {
  constructor(private http: HttpClient) {}

  searchBooks(query: string): Observable<BookSearchResponse> {
    return this.http.post<BookSearchResponse>(endpoints.BOOKS.SEARCH, { query });
  }
}
