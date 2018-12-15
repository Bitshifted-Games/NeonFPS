using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BitshiftedGames.Localization
{
    public class StartupManager : MonoBehaviour
    {
        [Tooltip("The name of the scene that is loaded after localization is set")]
        public string sceneToLoad = "Main Menu";

        /// <summary>
        /// Will be called and started automatically due to being named Start 
        /// </summary>
        /// <returns> Loads the target scene once LocalizationManager is ready</returns>
        private IEnumerator Start ()
        {
            while ( !LocalizationManager.instance.GetIsReady () )
            {
                yield return null;
            }

            SceneManager.LoadScene ( sceneToLoad );
        }
    }
}