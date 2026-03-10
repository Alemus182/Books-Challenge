import React, { useState } from 'react';
import { searchBooks } from '../../services/books.service';
import { BookCandidateDto } from '../../models/book-search-response.model';
import { useAlert } from '../../context/AlertContext';

export default function BookSearch() {
  const { info, error: alertError } = useAlert();

  const [query, setQuery] = useState('');
  const [queryTouched, setQueryTouched] = useState(false);
  const [candidates, setCandidates] = useState<BookCandidateDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const queryInvalid = query.trim().length < 2;

  async function handleSearch(e: React.FormEvent) {
    e.preventDefault();
    setQueryTouched(true);
    if (queryInvalid) return;

    setIsLoading(true);
    setHasSearched(false);
    setCandidates([]);

    try {
      const response = await searchBooks(query.trim());
      setCandidates(response.candidates ?? []);
      setHasSearched(true);
      if (!response.candidates?.length) {
        info('No books found for your query.');
      }
    } catch {
      alertError('An error occurred while searching for books.');
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="book-search-wrapper wrapper wrapper-content animated fadeInRight">
      <div className="ibox">
        <div className="ibox-title">
          <h2>Book Search</h2>
        </div>
        <div className="ibox-content">
          <form noValidate onSubmit={handleSearch}>
            <div className="input-group mb-3">
              <input
                type="text"
                className={`form-control${queryTouched && queryInvalid ? ' is-invalid' : ''}`}
                placeholder="Search by title, author, or keywords… e.g. tolkien hobbit 1937"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                onBlur={() => setQueryTouched(true)}
                disabled={isLoading}
              />
              <button
                type="submit"
                className="btn btn-primary"
                disabled={isLoading}
              >
                {isLoading ? (
                  <>
                    <span
                      className="spinner-border spinner-border-sm me-1"
                      role="status"
                      aria-hidden="true"
                    />
                    Searching…
                  </>
                ) : (
                  'Search'
                )}
              </button>
              {queryTouched && queryInvalid && (
                <div className="invalid-feedback">
                  Please enter at least 2 characters.
                </div>
              )}
            </div>
          </form>

          {isLoading && (
            <div className="progress mb-3" style={{ height: 4 }}>
              <div
                className="progress-bar progress-bar-striped progress-bar-animated w-100"
                role="progressbar"
              />
            </div>
          )}

          {hasSearched && !isLoading && (
            <>
              {candidates.length === 0 ? (
                <p className="text-muted">No results found.</p>
              ) : (
                <div className="row g-3 results-grid">
                  {candidates.map((book, idx) => (
                    <div key={idx} className="col-12 col-md-6 col-xl-4">
                      <div className="card book-card h-100 shadow-sm">
                        <div className="card-body d-flex gap-3">
                          {/* Cover */}
                          <div className="cover-container flex-shrink-0">
                            {book.coverImageUrl ? (
                              <img
                                src={book.coverImageUrl}
                                alt={book.title}
                                className="book-cover rounded"
                              />
                            ) : (
                              <div className="no-cover d-flex align-items-center justify-content-center">
                                <i className="fa fa-book fa-3x text-muted" />
                              </div>
                            )}
                          </div>

                          {/* Info */}
                          <div className="book-info flex-grow-1">
                            <h5 className="card-title book-title mb-1">{book.title}</h5>
                            <h6 className="card-subtitle text-muted mb-2">{book.author}</h6>

                            <div className="d-flex flex-wrap gap-2 mb-2">
                              {book.firstPublishYear && (
                                <span className="badge bg-secondary">
                                  <i className="fa fa-calendar me-1" />
                                  {book.firstPublishYear}
                                </span>
                              )}
                              {book.openLibraryId && (
                                <a
                                  href={`https://openlibrary.org${book.openLibraryId}`}
                                  target="_blank"
                                  rel="noopener noreferrer"
                                  className="badge bg-primary text-decoration-none"
                                >
                                  <i className="fa fa-external-link-alt me-1" />
                                  Open Library
                                </a>
                              )}
                            </div>

                            <p className="card-text explanation small text-secondary">
                              {book.explanation}
                            </p>
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}
