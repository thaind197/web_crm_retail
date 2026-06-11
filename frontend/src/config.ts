// System Configuration File

const getFallbackApiUrl = () => {
  if (typeof window === 'undefined') return 'http://localhost:5013';
  
  const hostname = window.location.hostname;
  
  // If running on local development
  if (hostname === 'localhost' || hostname === '127.0.0.1') {
    return window.location.port === '3000' 
      ? `http://${hostname}:5000` 
      : 'http://localhost:5013';
  }
  
  // If deployed on Render (e.g. sales-crm-frontend-xxxx.onrender.com)
  if (hostname.includes('sales-crm-frontend')) {
    return window.location.origin.replace('sales-crm-frontend', 'sales-crm-backend');
  }
  
  // Fallback to same origin
  return window.location.origin;
};

export const API_BASE_URL = import.meta.env.VITE_API_URL || getFallbackApiUrl();

export const APP_CONFIG = {
  version: '1.0.0',
  defaultLocale: 'vi',
  localesSupported: ['vi', 'en'],
  currencyLocale: 'vi-VN',
  currencyCode: 'VND'
};
