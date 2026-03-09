export interface BookCandidateDto {
  title: string;
  author: string;
  firstPublishYear: number | null;
  openLibraryId: string;
  coverImageUrl: string | null;
  explanation: string;
}

export interface BookSearchResponse {
  candidates: BookCandidateDto[];
}
