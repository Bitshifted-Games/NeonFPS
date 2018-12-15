using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BitshiftedGames.Localization
{
    [System.Serializable]
    public enum SupportedLanguages
    {
        UNLOCALIZED,
        English,
        French,
        German,
        Italian
    }

    [System.Serializable]
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager instance;

        public SupportedLanguages SelectedLanguage;

        private Dictionary<string, string> unlocalizedText;
        private Dictionary<string, string> localizedText;
        private readonly string missingTextString = "Localized text not found";
        private bool isReady = false;

        private readonly string unlocalizedTextPath = "unlocalizedText_en.json";

        public string GetMissingTextString ()
        {
            return missingTextString;
        }
        public bool GetIsReady ()
        {
            return isReady;
        }

        public delegate void LanguageSwitchHandler ();
        public LanguageSwitchHandler LanguageSwitchEvent;
        public void ChangeLanguage () { LanguageSwitchEvent?.Invoke (); }

        // Use this for initialization
        void Awake ()
        {
            if ( instance == null )
                instance = this;
            else if ( instance != this )
                Destroy ( gameObject );

            DontDestroyOnLoad ( gameObject );
            LoadBySelectedLanguage ();
        }

        private void OnDestroy ()
        {
            isReady = false;
            unlocalizedText.Clear ();
            localizedText.Clear ();
            unlocalizedText = localizedText = null;
        }

        /// <summary>
        /// Deserializes JSON data contained within the passed filename
        /// </summary>
        /// <param name="filename"></param>
        public void LoadLocalizedText ( string fileName )
        {
            localizedText = new Dictionary<string, string> ();
            string filePath = Path.Combine ( Path.Combine ( Application.streamingAssetsPath, "lang" ), fileName );
            if ( File.Exists ( filePath ) )
            {
                string dataAsJson = File.ReadAllText ( filePath );
                LocalizedLanguageData loadedData = JsonUtility.FromJson<LocalizedLanguageData> ( dataAsJson );
                for ( int i = 0; i < loadedData.items.Length; i++ )
                    localizedText.Add ( loadedData.items[i].key, loadedData.items[i].value );

                SelectedLanguage = loadedData.Language;

                foreach ( KeyValuePair<string, string> pair in unlocalizedText )
                    if ( !localizedText.ContainsKey ( pair.Key ) ) localizedText.Add ( pair.Key, pair.Value );

                Debug.Log ( "Data loaded, dictionary contains: " + localizedText.Count + " entries" );
            } else
            {
                Debug.LogError ( "Cannot find file! Defaulting to unlocalized text" );
                LoadUnlocalizedText ();
            }

            isReady = true;
        }

        /// <summary>
        /// Allows obtaining any language file via the instance
        /// </summary>
        /// <param name="filename">Language file to load</param>
        /// <returns>Data stored in language file, or null</returns>
        public static Dictionary<string, string> GetLocalizedTextDictionary ( string filename )
        {
            Dictionary<string, string> text = null;
            string filePath = Path.Combine ( Path.Combine ( Application.streamingAssetsPath, "lang" ), filename );
            if ( File.Exists ( filePath ) )
            {
                text = new Dictionary<string, string> ();
                string dataAsJson = File.ReadAllText ( filePath );
                LocalizedLanguageData loadedData = JsonUtility.FromJson<LocalizedLanguageData> ( dataAsJson );
                for ( int i = 0; i < loadedData.items.Length; i++ )
                    text.Add ( loadedData.items[i].key, loadedData.items[i].value );
                Debug.Log ( "Data loaded, dictionary contains: " + text.Count + " entries" );
            } else
            {
                Debug.LogError ( "Cannot find file!" );
            }
            return text;
        }

        /// <summary>
        /// Retrieves the given key if it exists
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>Localized string stored in key</returns>
        public string GetLocalizedValue ( string key )
        {
            string result = missingTextString;
            if ( localizedText.ContainsKey ( key ) )
                result = localizedText[key];
            return result;
        }

        /// <summary>
        /// Retrieves the given key if it exists
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>Unlocalized string stored in key</returns>
        public string GetUnlocalizedValue ( string key )
        {
            string result = missingTextString;
            if ( unlocalizedText.ContainsKey ( key ) )
                result = unlocalizedText[key];
            return result;
        }

        private void LoadUnlocalizedText ()
        {
            unlocalizedText = new Dictionary<string, string> ();
            string filePath = Path.Combine ( Application.streamingAssetsPath, "lang" );
            filePath = Path.Combine ( filePath, unlocalizedTextPath );

            if ( File.Exists ( filePath ) )
            {
                string dataAsJson = File.ReadAllText ( filePath );
                LocalizedLanguageData loadedData = JsonUtility.FromJson<LocalizedLanguageData> ( dataAsJson );

                for ( int i = 0; i < loadedData.items.Length; i++ )
                {
                    unlocalizedText.Add ( loadedData.items[i].key, loadedData.items[i].value );
                }

                Debug.Log ( "Data loaded, dictionary contains: " + unlocalizedText.Count + " entries" );
            } else
            {
                Debug.LogError ( "Cannot find file! Defaulting to English text" );
                LoadLocalizedText ( "localizedText_en.json" );
            }
            localizedText = unlocalizedText;
        }

        private void LoadBySelectedLanguage ()
        {
            switch ( SelectedLanguage )
            {
                case SupportedLanguages.English:
                    LoadLocalizedText ( "localizedText_en.json" );
                    break;
                case SupportedLanguages.French:
                    LoadLocalizedText ( "localizedText_fr.json" );
                    break;
                case SupportedLanguages.German:
                    LoadLocalizedText ( "localizedText_de.json" );
                    break;
                case SupportedLanguages.Italian:
                    LoadLocalizedText ( "localizedText_it.json" );
                    break;
                default:
                    LoadLocalizedText ( "unlocalizedText_en.json" );
                    break;
            }
        }
    }
}