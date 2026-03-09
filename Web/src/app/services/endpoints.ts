import {environment} from '../../environments/environment';

const api = environment.apiUrl;

const endpoints = {
  AUTH: {
    LOGIN: `${api}/api/auth/Login`
  },
  STORIES:{
    GET_NEWEST: `${api}/api/Stories/Get-Newest/`,
    FIND_BY_FILTER: `${api}/api/Stories/Find-Stories-by-Filter/`,
  },
  BOOKS: {
    SEARCH: `${api}/api/Books/Search`,
  }
};

export default endpoints;