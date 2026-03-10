import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { AlertProvider } from './context/AlertContext';
import PrivateRoute from './guards/PrivateRoute';
import BasicLayout from './components/layouts/BasicLayout';
import BlankLayout from './components/layouts/BlankLayout';
import Login from './components/login/Login';
import BookSearch from './components/books/BookSearch';

export default function App() {
  return (
    <AuthProvider>
      <AlertProvider>
        <BrowserRouter>
          <Routes>
            {/* Public routes */}
            <Route element={<BlankLayout />}>
              <Route path="/login" element={<Login />} />
            </Route>

            {/* Protected routes */}
            <Route element={<PrivateRoute />}>
              <Route element={<BasicLayout />}>
                <Route path="/books/search" element={<BookSearch />} />
              </Route>
            </Route>

            {/* Default redirect */}
            <Route path="*" element={<Navigate to="/books/search" replace />} />
          </Routes>
        </BrowserRouter>
      </AlertProvider>
    </AuthProvider>
  );
}
