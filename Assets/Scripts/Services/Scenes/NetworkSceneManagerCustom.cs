﻿using Assets.Scripts.UI;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Scripts.Services.Scenes
{
    public class NetworkSceneManagerCustom : NetworkSceneManagerBase, INetworkSceneManager
	{
		#region Inject
		[Inject] private readonly MainSceneCamera mainCamera;
		[Inject] private readonly UIManager uiManager;
        #endregion

        public const int LOBBY_SCENE = 0;

		[SerializeField] private UIScreen _dummyScreen;
		[SerializeField] private UIScreen _lobbyScreen;
		[SerializeField] private CanvasFader fader;

		//public static LevelManager Instance => Singleton<LevelManager>.Instance;

		public static void LoadMenu()
		{
			//Instance.Runner.SetActiveScene(LOBBY_SCENE);
		}

		public static void LoadTrack(int sceneIndex)
		{
			//Instance.Runner.SetActiveScene(sceneIndex);
		}

		protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
		{
			Debug.Log($"Loading scene {newScene}");

			//PreLoadScene(newScene);

			List<NetworkObject> sceneObjects = new List<NetworkObject>();

			if (newScene >= LOBBY_SCENE)
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

			// Now we can safely spawn karts
			//if (GameManager.CurrentTrack != null && newScene > LOBBY_SCENE)
			//{
			//	if (Runner.GameMode == GameMode.Host)
			//	{
			//		foreach (var player in RoomPlayer.Players)
			//		{
			//			player.GameState = RoomPlayer.EGameState.GameCutscene;
			//			GameManager.CurrentTrack.SpawnPlayer(Runner, player);
			//		}
			//	}
			//}

			//PostLoadScene();
		}

		private void PreLoadScene(int scene)
		{
			if (scene > LOBBY_SCENE)
			{
				// Show an empty dummy UI screen - this will stay on during the game so that the game has a place in the navigation stack. Without this, Back() will break
				Debug.Log("Showing Dummy");
				UIScreen.Focus(_dummyScreen);
			}
			else if (scene == LOBBY_SCENE)
			{
				//foreach (RoomPlayer player in RoomPlayer.Players)
				//{
				//	player.IsReady = false;
				//}
				UIScreen.ActiveScreen.BackTo(_lobbyScreen);
			}
			else
			{
				UIScreen.BackToInitial();
			}
			fader.gameObject.SetActive(true);
			fader.FadeIn();
		}

		private void PostLoadScene()
		{
			fader.FadeOut();
		}

	}
}