// System Configuration File

export const API_BASE_URL = import.meta.env.VITE_API_URL || 
  (typeof window !== 'undefined' && window.location.port === '3000' 
    ? `http://${window.location.hostname}:5000` 
    : 'http://localhost:5013');

export const APP_CONFIG = {
  version: '1.0.0',
  defaultLocale: 'vi',
  localesSupported: ['vi', 'en'],
  currencyLocale: 'vi-VN',
  currencyCode: 'VND'
};
