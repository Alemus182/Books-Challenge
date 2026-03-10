import React, { useState } from 'react';

interface Props {
  onToggleNav: () => void;
}

export default function TopNavbar({ onToggleNav }: Props) {
  return (
    <div className="row border-bottom" id="topnavbar">
      <nav className="navbar navbar-static-top" role="navigation" style={{ marginBottom: 0 }}>
        <div className="navbar-header">
          <button
            type="button"
            className="btn btn-link minimalize-styl-2"
            onClick={onToggleNav}
          >
            <i className="fa fa-bars" />
          </button>
        </div>
      </nav>
    </div>
  );
}
