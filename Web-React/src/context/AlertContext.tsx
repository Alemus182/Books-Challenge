import React, { createContext, useContext, useState, useCallback } from 'react';
import { Alert, AlertType } from '../models/alert.model';

interface AlertContextValue {
  alerts: Alert[];
  success: (message: string) => void;
  error: (message: string) => void;
  info: (message: string) => void;
  warn: (message: string) => void;
  remove: (id: string) => void;
}

const AlertContext = createContext<AlertContextValue>({
  alerts: [],
  success: () => {},
  error: () => {},
  info: () => {},
  warn: () => {},
  remove: () => {},
});

let counter = 0;

export function AlertProvider({ children }: { children: React.ReactNode }) {
  const [alerts, setAlerts] = useState<Alert[]>([]);

  const push = useCallback((type: AlertType, message: string) => {
    const id = `alert-${++counter}`;
    setAlerts((prev) => [...prev, { id, type, message }]);
    // Auto-dismiss after 5 s
    setTimeout(() => {
      setAlerts((prev) => prev.filter((a) => a.id !== id));
    }, 5000);
  }, []);

  const remove = useCallback((id: string) => {
    setAlerts((prev) => prev.filter((a) => a.id !== id));
  }, []);

  return (
    <AlertContext.Provider
      value={{
        alerts,
        success: (m) => push('success', m),
        error: (m) => push('error', m),
        info: (m) => push('info', m),
        warn: (m) => push('warning', m),
        remove,
      }}
    >
      {children}
    </AlertContext.Provider>
  );
}

export function useAlert() {
  return useContext(AlertContext);
}
