import api from './api';
import endpoints from './endpoints';
import { BookSearchResponse } from '../models/book-search-response.model';

export async function searchBooks(query: string): Promise<BookSearchResponse> {
  const response = await api.post<BookSearchResponse>(endpoints.BOOKS.SEARCH, { query });
  return response.data;
}
