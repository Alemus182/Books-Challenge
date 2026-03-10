import React, { useState } from 'react';
import { Outlet } from 'react-router-dom';
import Navigation from '../common/Navigation';
import TopNavbar from '../common/TopNavbar';
import AlertNotification from '../common/AlertNotification';

export default function BasicLayout() {
  const [navCollapsed, setNavCollapsed] = useState(false);

  return (
    <div id="wrapper" className={navCollapsed ? 'mini-navbar' : ''}>
      <Navigation />
      <div id="page-wrapper" className="gray-bg">
        <TopNavbar onToggleNav={() => setNavCollapsed((v) => !v)} />
        <AlertNotification />
        <Outlet />
      </div>
    </div>
  );
}
