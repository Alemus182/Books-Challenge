import React, { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export default function Navigation() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [collapsed, setCollapsed] = useState(false);

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <nav
      className={`navbar-default navbar-static-side${collapsed ? ' collapsed' : ''}`}
      role="navigation"
    >
      <div className="sidebar-collapse">
        <ul className="nav metismenu" id="side-menu">
          <li className="nav-header">
            <div className="logo-element">Books App</div>
          </li>

          <li>
            <NavLink
              to="/books/search"
              className={({ isActive }) => (isActive ? 'active' : '')}
            >
              <i className="fa fa-search" />
              <span className="nav-label"> Search Books</span>
            </NavLink>
          </li>

          <li>
            <button className="btn btn-link nav-logout" onClick={handleLogout}>
              <i className="fa fa-sign-out-alt" />
              <span className="nav-label"> Exit</span>
            </button>
          </li>
        </ul>
      </div>
    </nav>
  );
}
