export type AlertType = 'success' | 'error' | 'info' | 'warning';

export interface Alert {
  id: string;
  type: AlertType;
  message: string;
}
