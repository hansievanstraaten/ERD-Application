using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViSo.Common;

namespace WPF.Tools.Dictionaries
{
    public static class TranslationDictionary
    {
        private static string lastTraslationLoaded;

        private static Dictionary<string, string> translations = new Dictionary<string, string>();

        public static bool TranslationLoaded
        {
            get;
            private set;
        }

        public static string Translate(string english)
        {
            if (english.IsNullEmptyOrWhiteSpace())
            {
                return string.Empty;
            }

            if (TranslationDictionary.translations.ContainsKey(english))
            {
                return TranslationDictionary.translations[english];
            }

            return english;
        }

        public static Dictionary<string, string> GetTranslationsDictionary
        {
            get
            {
                return TranslationDictionary.translations;
            }
        }

        public static void LoadTransaltionFile(string filePah)
        {
            if (TranslationDictionary.lastTraslationLoaded != filePah)
            {
                Dictionary<string, string> holdTranslations = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> oldKey in TranslationDictionary.translations)
                {
                    if (!holdTranslations.ContainsKey(oldKey.Value))
                    {
                        holdTranslations.Add(oldKey.Value, oldKey.Key);
                    }
                }

                TranslationDictionary.translations = holdTranslations;
            }

            if (filePah.IsNullEmptyOrWhiteSpace() || !File.Exists(filePah))
            {
                TranslationDictionary.TranslationLoaded = false;

                return;
            }

            TranslationDictionary.lastTraslationLoaded = filePah;

            Paths.WaitFileRelease(filePah);

            string translationfile = File.ReadAllText(filePah);

            translationfile = translationfile.Replace("\r", string.Empty).Replace("\n", string.Empty);

            string[] translationItems = translationfile.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            char[] splitBy = new char[] {'|'};

            foreach (string item in translationItems)
            {
                string[] splitItem = item.Split(splitBy, StringSplitOptions.None);

                if (!TranslationDictionary.translations.ContainsKey(splitItem[0]))
                {
                    TranslationDictionary.translations.Add(splitItem[0], splitItem[1]);
                }
            }

            TranslationDictionary.TranslationLoaded = true;
        }
    }
}
