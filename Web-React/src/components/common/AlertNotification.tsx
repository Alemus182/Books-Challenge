import React from 'react';
import { useAlert } from '../../context/AlertContext';

const typeToClass: Record<string, string> = {
  success: 'alert-success',
  error: 'alert-danger',
  info: 'alert-info',
  warning: 'alert-warning',
};

export default function AlertNotification() {
  const { alerts, remove } = useAlert();

  if (alerts.length === 0) return null;

  return (
    <div className="alert-container">
      {alerts.map((alert) => (
        <div
          key={alert.id}
          className={`alert ${typeToClass[alert.type] ?? 'alert-info'} alert-dismissible fade show`}
          role="alert"
        >
          <span dangerouslySetInnerHTML={{ __html: alert.message }} />
          <button
            type="button"
            className="btn-close"
            onClick={() => remove(alert.id)}
            aria-label="Close"
          />
        </div>
      ))}
    </div>
  );
}
