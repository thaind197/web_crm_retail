import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import axios from 'axios';
import { API_BASE_URL } from './config';

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {},
    fallbackLng: 'vi',
    debug: false,
    interpolation: {
      escapeValue: false // react already safes from xss
    },
    detection: {
      order: ['localStorage', 'navigator'],
      caches: ['localStorage']
    }
  });

export const loadAndChangeLanguage = async (lng: string): Promise<void> => {
  const langCode = lng.startsWith('en') ? 'en' : 'vi';
  if (!i18n.hasResourceBundle(langCode, 'translation')) {
    try {
      const res = await axios.get(`${API_BASE_URL}/api/localization?lang=${langCode}`);
      i18n.addResourceBundle(langCode, 'translation', res.data, true, true);
    } catch (err) {
      console.error(`Failed to load translation bundle for ${langCode} from backend`, err);
    }
  }
  await i18n.changeLanguage(langCode);
};

export default i18n;
