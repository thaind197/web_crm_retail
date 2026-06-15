import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import viTranslation from './locales/vi.json';
import enTranslation from './locales/en.json';

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      vi: {
        translation: viTranslation
      },
      en: {
        translation: enTranslation
      }
    },
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
  await i18n.changeLanguage(langCode);
};

export default i18n;
