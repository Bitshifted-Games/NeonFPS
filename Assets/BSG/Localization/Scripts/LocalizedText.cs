using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BitshiftedGames.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip ( "This is the key in your language file for this string" )]
        public string key;

        public string UnlocalizedValue
        {
            get { return LocalizationManager.instance.GetUnlocalizedValue ( key ); }
        }

        private Text myTextComponent;

        private void OnEnable ()
        {
            LocalizationManager.instance.LanguageSwitchEvent += SetLocalizedString;
        }

        private void OnDisable ()
        {
            LocalizationManager.instance.LanguageSwitchEvent -= SetLocalizedString;
        }

        private void OnDestroy ()
        {
            LocalizationManager.instance.LanguageSwitchEvent -= SetLocalizedString;
        }

        // Use this for initialization
        void Start ()
        {
            myTextComponent = GetComponent<Text> ();
            SetLocalizedString ();
        }

        void SetLocalizedString ()
        {
                myTextComponent.text = LocalizationManager.instance.GetLocalizedValue ( key );
        }

    }
}