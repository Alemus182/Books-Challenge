import axios from 'axios';

const api = axios.create();

// Attach JWT token to every request
api.interceptors.request.use((config) => {
  const raw = sessionStorage.getItem('userData');
  if (raw && raw !== 'undefined') {
    const userData = JSON.parse(raw);
    if (userData?.token) {
      config.headers['Authorization'] = `Bearer ${userData.token}`;
    }
  }
  return config;
});

// On 401, clear session and redirect to login
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      sessionStorage.removeItem('userData');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  },
);

export default api;
