const apiUrl = 'https://localhost:7245';

const endpoints = {
  AUTH: {
    LOGIN: `${apiUrl}/api/auth/Login`,
  },
  BOOKS: {
    SEARCH: `${apiUrl}/api/Books/Search`,
  },
};

export default endpoints;
