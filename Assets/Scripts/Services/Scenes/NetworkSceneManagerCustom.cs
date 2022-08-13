using Assets.Scripts.UI;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Scripts.Services.Scenes
{
    public class NetworkSceneManagerCustom : NetworkSceneManagerBase
	{
		#region Inject
		[Inject] private readonly MainSceneCamera mainCamera;
		[Inject] private readonly UIManager uiManager;
        #endregion
		
		protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
		{
			Debug.Log($"Loading scene {newScene} over scene {prevScene}" );

			List<NetworkObject> sceneObjects = new List<NetworkObject>();

			if (newScene != prevScene)
			{
				yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);

				mainCamera.gameObject.SetActive(false);

				Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
				Debug.Log($"Loaded scene {newScene}: {loadedScene}");

				SceneManager.SetActiveScene(loadedScene);
				sceneObjects = FindNetworkObjects(loadedScene, disable: false);

				uiManager.ToggleHUD(true);
			}

			finished(sceneObjects);

			// Delay one frame, so we're sure level objects has spawned locally
			yield return null;
		}
	}
}