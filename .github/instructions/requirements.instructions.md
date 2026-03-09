---
name: requirements
description: Requirements for the library book discovery application
applyTo: "**"
---

## Overview

Build a library discovery application: given a messy plain-text blob (title, author, and/or keywords), identify the most likely book and present results in a modern UI backed by a .NET 8 Web API.

**Goals:** full-stack skills, real LLM usage, text processing, clean architecture, product thinking.

---

## 1. Query Input

Accept free-text queries in any of these forms:

| Type | Example |
|------|---------|
| Sparse (title or author only) | `dickens`, `tale two cities` |
| Dense/noisy | `tolkien hobbit illustrated deluxe 1937` |
| Ambiguous (partial names / character hints) | `mark huckleberry`, `austen bennet` |

---

## 2. AI Processing

Use an LLM to:

- **Extract & normalize** fields from the raw blob → `{ title?, author?, keywords[] }`
- **Explain** each candidate match in 1–2 sentences, grounded in fetched fields
- **Re-rank** (optional) top candidates into a final ordered list

---

## 3. Open Library Search

**Base API:** `https://openlibrary.org/search.json`

### Endpoints to use

| Endpoint | Purpose |
|----------|---------|
| `/search.json` | Search works/editions |
| `/works/{work_id}.json` | Work details |
| `/authors/{author_id}.json` | Author details |
| `/authors/{author_id}/works.json` | Author's works |

### Normalization rules

- Lowercase, strip punctuation/diacritics, handle partials
- Handle subtitle variants (e.g., *The Hobbit* vs. *There and Back Again*)
- De-duplicate to canonical works; resolve primary authors from `works.authors`

---

## 4. Matching Hierarchy

Ranked from strongest to weakest signal:

1. Exact/normalized title + **primary author** match
2. Exact/normalized title + contributor-only author (lower rank)
3. Near-match title + author match
4. Author-only fallback → return top works by that author
5. No clear winner → return up to **5 ordered candidates** with explanations

---

## 5. Data Quality

Open Library may include contributors (illustrators, editors, adaptors) in `author_name` alongside the primary author.

- **Prioritize** primary authors from canonical work records
- **Treat contributors as lower signal**
- **Surface this in explanations** (e.g., *"Exact title; Tolkien is primary author; Dixon listed as adaptor."*)

---

## 6. AI API Setup

- By default, use [Gemini](https://ai.google.dev/gemini-api/docs/api-key) (free tier available) this is the key AIzaSyDSr5DybLa6oAwh-oO-yeBqgyZU3rFkvBI
---

## 7. Response Schema

Return an ordered list of candidates. Numeric confidence scores are not required.

```json
{
  "title": "The Hobbit",
  "author": "J.R.R. Tolkien",
  "first_publish_year": 1937,
  "open_library_id": "/works/OL262758W",
  "cover_image_url": "https://covers.openlibrary.org/b/id/8406786-L.jpg",
  "explanation": "Exact title match; Tolkien is primary author; Dixon listed as adaptor."
}
```

### Required fields per candidate

| Field | Description |
|-------|-------------|
| `title` | Book title |
| `author` | Primary author(s) |
| `first_publish_year` | Year of first publication |
| `open_library_id` | Open Library work ID/link |
| `cover_image_url` | Cover image URL (if available) |
| `explanation` | One sentence citing concrete matched fields |