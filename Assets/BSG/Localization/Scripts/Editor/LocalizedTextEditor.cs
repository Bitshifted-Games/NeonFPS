using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BitshiftedGames.Localization
{
    public class LocalizedTextEditor : EditorWindow
    {
        public LocalizedLanguageData localizationData; // LocalizedLanguageData to be operated on

        // Displays the associated window in Unity, calling OnGUI
        [MenuItem ( "Bitshifted Games/Localization/Localized Text Editor" )]
        static void Init ()
        {
            GetWindow ( typeof ( LocalizedTextEditor ) ).Show ();
        }

        // Handles the actual drawing of the window and its contents
        private void OnGUI ()
        {
            if ( localizationData != null )
            {
                SerializedObject serializedObject = new SerializedObject ( this );
                SerializedProperty serializedProperty = serializedObject.FindProperty ( "localizationData" );
                EditorGUILayout.PropertyField ( serializedProperty, true );
                serializedObject.ApplyModifiedProperties ();

                if ( GUILayout.Button ( "Save data" ) ) SaveLanguageFile ();
            }

            if ( GUILayout.Button ( "Load data" ) ) LoadLanguageFile ();

            if ( GUILayout.Button ( "Create new data" ) ) CreateNewData ();
        }

        /// Simply creates an empty language data object
        private void CreateNewData ()
        {
            localizationData = new LocalizedLanguageData ();

            string unlocalizedJson = File.ReadAllText ( Path.Combine ( Application.streamingAssetsPath, "unlocalizedText_en.json" ) );
            LocalizedLanguageData unlocalizedData = JsonUtility.FromJson<LocalizedLanguageData> ( unlocalizedJson );
            localizationData.items = unlocalizedData.items;
        }

        #region File I/O
        // Deserializes localization data from disk
        private void LoadLanguageFile ()
        {
            string filePath = EditorUtility.OpenFilePanel ( "Select localization data file",Path.Combine(Application.streamingAssetsPath,"lang"), "json" );

            if ( !string.IsNullOrEmpty ( filePath ) )
            {
                string dataAsJson = File.ReadAllText ( filePath );
                localizationData = JsonUtility.FromJson<LocalizedLanguageData> ( dataAsJson );
                MergeNewUnlocalizedStrings ();                
            } else
            {
                //TODO throw error cleanly due to illegal file path
                Debug.LogError ("Could not find localization data at "+filePath+".");
            }
        }

        // Serializes localization data to disk
        private void SaveLanguageFile ()
        {
            string filePath = EditorUtility.SaveFilePanel ( "Save localization data file", Path.Combine ( Application.streamingAssetsPath, "lang" ), "localizedText_XX", "json" );

            if ( !string.IsNullOrEmpty ( filePath ) )
            {
                string dataAsJson = JsonUtility.ToJson ( localizationData );
                File.WriteAllText ( filePath, dataAsJson );
            }
        }
        #endregion

        private void MergeNewUnlocalizedStrings ()
        {
            string unlocalizedJson = File.ReadAllText ( Path.Combine ( Path.Combine ( Application.streamingAssetsPath, "lang" ), "unlocalizedText_en.json" ) );
            LocalizedLanguageData unlocalizedData = JsonUtility.FromJson<LocalizedLanguageData> ( unlocalizedJson );

            foreach ( LocalizationItem item in unlocalizedData.items )
            {
                if ( !localizationData.ContainsKey ( item.key ) ) localizationData.AddNewItem ( item );
            }
        }

        private static void MergeNewUnlocalizedStrings (LocalizedLanguageData target)
        {
            string unlocalizedJson = File.ReadAllText ( Path.Combine (Path.Combine( Application.streamingAssetsPath,"lang"), "unlocalizedText_en.json" ) );
            LocalizedLanguageData unlocalizedData = JsonUtility.FromJson<LocalizedLanguageData> ( unlocalizedJson );

            foreach ( LocalizationItem item in unlocalizedData.items )
            {
                if ( !target.ContainsKey ( item.key ) ) target.AddNewItem ( item );
            }
        }

        [MenuItem("Bitshifted Games/Localization/Update all language files with new strings")]
        public static void UpdateAllLocalizationData ()
        {
            string unlocalizedJson = File.ReadAllText ( Path.Combine ( Path.Combine ( Application.streamingAssetsPath, "lang" ), "unlocalizedText_en.json" ) );
            LocalizedLanguageData unlocalizedData = JsonUtility.FromJson<LocalizedLanguageData> ( unlocalizedJson );
            SupportedLanguages currentLanguage = (SupportedLanguages)System.Enum.GetValues ( typeof ( SupportedLanguages ) ).GetValue ( 1 );
            LocalizedLanguageData target;
            while ( (int)currentLanguage < System.Enum.GetValues ( typeof ( SupportedLanguages ) ).Length+1 )
            {
                string langFile=string.Empty;
                switch ( currentLanguage )
                {
                    case SupportedLanguages.UNLOCALIZED:
                        currentLanguage++;
                        continue;
                    case SupportedLanguages.English:
                        langFile = "localizedText_en.json";
                        break;
                    case SupportedLanguages.French:
                        langFile = "localizedText_fr.json";
                        break;
                    case SupportedLanguages.German:
                        langFile = "localizedText_de.json";
                        break;
                    case SupportedLanguages.Italian:
                        langFile = "localizedText_it.json";
                        break;
                    default:
                        break;
                }
                target = JsonUtility.FromJson<LocalizedLanguageData> (Path.Combine( Path.Combine ( Application.streamingAssetsPath, "lang" ),langFile ));
                MergeNewUnlocalizedStrings ( target );
                target = null;
                currentLanguage++;
            }
        }
    }
}