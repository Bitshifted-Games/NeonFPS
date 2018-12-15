using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BitshiftedGames.Localization
{
    /// <summary>
    /// Language file object containing the raw data that is loaded to and from json files
    /// </summary>
    [System.Serializable]
    public class LocalizedLanguageData
    {
        public SupportedLanguages Language; // Ensures the language is assigned properly
        //public string languageCode; // Standardized two letter language code
        public LocalizationItem[] items; // Array containing the localized contents of the file

        /// <summary>
        /// Returns true if the passed key exists in the array
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>Existence of key</returns>
        public bool ContainsKey (string key)
        {
            for ( int i = 0; i < items.Length; i++ )
            {
                if ( items[i].key == key ) return true;
            }
            return false;
        }

        /// <summary>
        /// Resizes the items array and adds the passed item to the end
        /// </summary>
        /// <param name="item">LocalizationItem to add</param>
        public void AddNewItem(LocalizationItem item )
        {
            System.Array.Resize ( ref items, items.Length + 1 );
            items[items.Length-1] = item;
        }
    }

    /// <summary>
    /// Single string of localized text and the associated key
    /// </summary>
    [System.Serializable]
    public struct LocalizationItem
    {
        public string key;
        public string value;
    }
}