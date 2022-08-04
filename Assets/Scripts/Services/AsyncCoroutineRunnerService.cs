using UnityEngine;

namespace FreeTeam.BP.Services
{
    public class AsyncCoroutineRunnerService : MonoBehaviour
    {
        #region Singleton
        private static AsyncCoroutineRunnerService instance;
        public static AsyncCoroutineRunnerService Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject("AsyncCoroutineRunner").AddComponent<AsyncCoroutineRunnerService>();

                return instance;
            }
        }
        #endregion

        #region Unity methods
        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}
