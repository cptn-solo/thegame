using Assets.Scripts.Services.Scenes;
using Fusion;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Services.App
{
    public class ApplicationService : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private string levelName = "Level01";
        #endregion

        public NetworkSceneManagerCustom SceneManager { get; set; }

        #region Unity methods
        #endregion
    }
}
