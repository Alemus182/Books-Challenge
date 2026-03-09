import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BooksService } from 'app/services/books.service';
import { AlertService } from 'app/services/alert.service';
import { BookCandidateDto } from 'app/models/book-search-response.model';

@Component({
  selector: 'app-book-search',
  templateUrl: './book-search.component.html',
  styleUrls: ['./book-search.component.scss']
})
export class BookSearchComponent implements OnInit {

  searchForm: FormGroup;
  candidates: BookCandidateDto[] = [];
  isLoadingResults = false;
  hasSearched = false;

  constructor(
    private booksService: BooksService,
    private fb: FormBuilder,
    private alertService: AlertService
  ) {
    this.searchForm = this.fb.group({
      query: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  ngOnInit(): void {}

  search(): void {
    if (this.searchForm.invalid) {
      return;
    }
    const query: string = this.searchForm.get('query').value.trim();
    this.isLoadingResults = true;
    this.hasSearched = false;
    this.candidates = [];

    this.booksService.searchBooks(query).subscribe(
      (response) => {
        this.candidates = response.candidates || [];
        this.hasSearched = true;
        this.isLoadingResults = false;
        if (this.candidates.length === 0) {
          this.alertService.info('No books found for your query.');
        }
      },
      (error) => {
        this.isLoadingResults = false;
        this.alertService.error('An error occurred while searching for books.');
      }
    );
  }

  get query() {
    return this.searchForm.get('query');
  }
}
