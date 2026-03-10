# Books-Challenge

A full-stack library book discovery application. Given a free-text query (title, author, keywords, or any combination), the app uses an LLM to extract and normalise intent, searches the Open Library API for candidates, and presents ranked results with cover images and AI-generated explanations.

---

## Architecture Overview

```
┌──────────────────────────────────────────────────────┐
│  Web  (Angular 10 · Bootstrap 4 · Angular Material)  │
│  Port 4200  →  proxies API calls to localhost:7245   │
└─────────────────────────┬────────────────────────────┘
                          │ HTTP/JSON
┌─────────────────────────▼────────────────────────────┐
│  Api  (.NET 8 Minimal API · Swagger · NLog)          │
│  Port 5099 (http)  / 7245 (https)                    │
│                                                      │
│  ┌──────────────┐  ┌──────────────────────────────┐  │
│  │  Application │  │  Infrastructure              │  │
│  │  (MediatR /  │  │  - GeminiService (LLM)       │  │
│  │   CQRS)      │  │  - OpenLibraryService (REST) │  │
│  └──────────────┘  │  - TokenService (JWT)        │  │
│                    └──────────────────────────────┘  │
└──────────────────────────────────────────────────────┘
```

> A second frontend built with **React 18 + Vite** is also available in `Web-React/`. Both SPAs connect to the same .NET 8 backend. See [Frontend — React (Vite)](#3-frontend--react-vite) for setup instructions.

### Key technology choices

| Layer | Technology | Notes |
|-------|-----------|-------|
| Frontend (Angular) | Angular 10 | Standalone SPA with Hash routing |
| Frontend (React) | React 18 + Vite | Lighter alternative SPA, port 4200 |
| UI components | Angular Material + Bootstrap 4 | Pre-built data-table, cards, progress bar |
| State | NgRx / React Context | Auth slice (Angular) · AuthContext (React) |
| Backend | .NET 8 Minimal API | Endpoint groups via `RouterBase` |
| Mediator / CQRS | MediatR 11 | Request/handler pipeline with validation & logging behaviours |
| Auth | JWT Bearer | Symmetric key, configurable lifetime |
| AI | Google Gemini (free tier) | Query normalisation + candidate ranking |
| Book data | Open Library REST API | Works, authors, covers |
| Logging | NLog | Structured file + console output |
| API docs | Swagger / OpenAPI | Auto-generated, available in Development |
| Tests | xUnit + `WebApplicationFactory` | Integration tests for auth routes |

---

## Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0 or later |
| [Node.js](https://nodejs.org/) | 14.x – 16.x (Angular 10) · 18.x+ (React/Vite) |
| npm | 6.x or later (bundled with Node) |
| Angular CLI | 10.x (`npm install -g @angular/cli@10`) — Angular only |

---

## How to Run

### 1. Backend — .NET 8 API

```powershell
# From the repository root
cd Api
dotnet restore
dotnet run
```

The API starts on:
- **HTTP** → `http://localhost:5099`
- **HTTPS** → `https://localhost:7245`

Swagger UI is available at `https://localhost:7245/swagger` (Development only).

#### Configuration

Settings live in `Api/appsettings.json`. For local development you can override them in `Api/appsettings.Development.json` or via User Secrets:

```json
{
  "JwtSettings": {
    "serverSigningPassword": "<your-secret>",
    "tokenLifetime": 25
  },
  "Gemini": {
    "ApiKey": "<your-gemini-api-key>"
  }
}
```

> A free Gemini API key can be obtained at <https://ai.google.dev/gemini-api/docs/api-key>.

---

### 2. Frontend — Angular 10

```powershell
# From the repository root
cd Web
npm install
npm start          # equivalent to: ng serve
```

The app is served at **`http://localhost:4200`**.

The `environment.ts` file points to `https://localhost:7245` as the API base URL. If you run the backend on a different port, update `Web/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5099',  // adjust as needed
};
```

#### Available routes

| Path | Component | Description |
|------|-----------|-------------|
| `/login` | `LoginComponent` | Obtain a JWT token |
| `/stories/new-stories` | `NewestComponent` | Latest Hacker News stories |
| `/stories/find-stories` | `FindByFilterComponent` | Filter stories by category or ID |
| `/books/search` | `BookSearchComponent` | **AI-powered book discovery** |

---

### 3. Frontend — React (Vite)

```powershell
# From the repository root
cd Web-React
npm install
npm run dev
```

The app is served at **`http://localhost:4200`** (same port as the Angular app — run only one at a time).

It connects to the same backend at `https://localhost:7245`. To change the API base URL, update `Web-React/src/services/` (look for the `axios` base URL configuration).

#### Available routes

| Path | Component | Description |
|------|-----------|-------------|
| `/login` | `Login` | Obtain a JWT token |
| `/books/search` | `BookSearch` | **AI-powered book discovery** |
| `*` | — | Redirects to `/books/search` |

#### Tech stack

| Package | Version | Role |
|---------|---------|------|
| React | 18.x | UI rendering |
| React Router DOM | 6.x | Client-side routing |
| Axios | 1.x | HTTP calls to the API |
| Vite | 5.x | Dev server & bundler |
| TypeScript | 5.x | Type safety |

---

### 4. Running tests

```powershell
# Backend integration tests (from repository root)
cd Test
dotnet test

# Frontend unit tests (Angular)
cd Web
npm test
```

---

## Project Structure

```
Books-Challenge/
├── Api/                   # .NET 8 Minimal API host
│   ├── Routes/            # BooksRouter, AuthRouter (RouterBase pattern)
│   ├── Components/        # ApiRoutes constants, middleware
│   └── Program.cs         # App bootstrap, CORS, JWT, Swagger setup
├── Application/           # MediatR handlers, DTOs, interfaces
│   ├── Services/Books/    # SearchBooksRequest / SearchBooksHandler
│   └── Dtos/              # BookCandidateDto, ExtractedQueryDto
├── Infraestructure/       # External service implementations
│   └── Services/          # GeminiService, OpenLibraryService, TokenService
├── Test/                  # xUnit integration tests
└── Web/                   # Angular 10 SPA
    └── src/app/
        ├── components/books/book-search/   # Book search UI
        ├── services/books.service.ts       # HTTP client for /api/Books/Search
        └── models/book-search-response.model.ts
```
