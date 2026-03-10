import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login } from '../../services/auth.service';
import { useAuth } from '../../context/AuthContext';

interface FormErrors {
  username?: string;
  password?: string;
}

export default function Login() {
  const navigate = useNavigate();
  const { setUser } = useAuth();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});
  const [loading, setLoading] = useState(false);
  const [serverError, setServerError] = useState('');

  function validate(): FormErrors {
    const errs: FormErrors = {};
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!username || !emailRegex.test(username)) {
      errs.username = 'Enter a valid email address.';
    }
    if (!password || password.length < 8) {
      errs.password = 'Password must be at least 8 characters.';
    }
    return errs;
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setTouched({ username: true, password: true });
    const errs = validate();
    setErrors(errs);
    if (Object.keys(errs).length > 0) return;

    setLoading(true);
    setServerError('');
    try {
      const authState = await login({ userName: username, password });
      if (authState.valid) {
        setUser(authState);
        navigate('/books/search');
      } else {
        setServerError(authState.message || 'Login failed.');
      }
    } catch {
      setServerError('An error occurred. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  const liveErrors = validate();

  return (
    <div className="row" style={{ height: '90vh' }}>
      <div className="col-lg-7 col-sm-3 bg-primary" />
      <div className="col-lg-5 col-sm-9 d-flex align-items-center justify-content-center">
        <div className="middle-box text-center w-100 px-4">
          <h3 className="mb-4">Welcome</h3>

          <form noValidate onSubmit={handleSubmit}>
            {/* Username */}
            <div className={`mb-3 text-start ${touched.username && liveErrors.username ? 'has-error' : ''}`}>
              <label className="form-label fw-bold" htmlFor="username">User</label>
              <input
                type="email"
                className={`form-control ${touched.username && liveErrors.username ? 'is-invalid' : ''}`}
                id="username"
                placeholder="Email"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                onBlur={() => setTouched((t) => ({ ...t, username: true }))}
              />
              {touched.username && liveErrors.username && (
                <div className="invalid-feedback">{liveErrors.username}</div>
              )}
            </div>

            {/* Password */}
            <div className={`mb-3 text-start ${touched.password && liveErrors.password ? 'has-error' : ''}`}>
              <label className="form-label fw-bold" htmlFor="password">Password</label>
              <input
                type="password"
                className={`form-control ${touched.password && liveErrors.password ? 'is-invalid' : ''}`}
                id="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                onBlur={() => setTouched((t) => ({ ...t, password: true }))}
              />
              {touched.password && liveErrors.password && (
                <div className="invalid-feedback">{liveErrors.password}</div>
              )}
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100 mb-3"
              disabled={loading}
            >
              {loading ? 'Logging in...' : 'Login'}
            </button>

            {serverError && (
              <div className="alert alert-danger" role="alert">
                {serverError}
              </div>
            )}
          </form>

          <p className="mt-3 text-muted">
            <small>Alemus — &copy; Copyright 2026</small>
          </p>
        </div>
      </div>
    </div>
  );
}
