using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Services.Scenes
{
    public static class SceneService
    {
        #region Public static
        public static event Action<Scene> SceneLoaded = null;
        public static event Action<Scene> SceneUnLoaded = null;
        public static event Action<float> SceneLoadProgress = null;
        #endregion

        #region Private static
        private static readonly List<Scene> scenes = new List<Scene>() { };
        #endregion

        #region Public static methods
        public static bool Contains(string _sceneName) =>
            scenes.Any(x => x.name == _sceneName);

        public static async Task LoadScene(string _sceneName, bool _unloadAllScenes = true, LoadSceneMode _loadSceneMode = LoadSceneMode.Additive)
        {
            if (string.IsNullOrEmpty(_sceneName))
                return;

            if (Contains(_sceneName))
                return;

            if (_unloadAllScenes)
                await UnloadAllScenes();

            var loading = SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);
            while (!loading.isDone)
            {
                SceneLoadProgress?.Invoke(loading.progress);
                await Awaiters.NextFrame;
            }

            var scene = SceneManager.GetSceneByName(_sceneName);
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);

                SceneLoaded?.Invoke(scene);

                scenes.Add(scene);
            }
        }

        public static async Task LoadScenes(string[] _scenesNames, bool _unloadAllScenes = true, LoadSceneMode _loadSceneMode = LoadSceneMode.Additive)
        {
            if (_unloadAllScenes)
                await UnloadAllScenes();

            foreach (var sceneName in _scenesNames)
            {
                if (Contains(sceneName))
                    return;

                var loading = SceneManager.LoadSceneAsync(sceneName, _loadSceneMode);
                while (!loading.isDone)
                {
                    SceneLoadProgress?.Invoke(loading.progress);
                    await Awaiters.NextFrame;
                }

                var scene = SceneManager.GetSceneByName(sceneName);
                if (scene.IsValid())
                {
                    SceneManager.SetActiveScene(scene);

                    SceneLoaded?.Invoke(scene);

                    scenes.Add(scene);
                }
            }
        }

        public static async Task UnloadScene(string sceneName)
        {
            if (!Contains(sceneName))
                return;

            var scene = scenes.FirstOrDefault(x => x.name == sceneName);

            var ao = SceneManager.UnloadSceneAsync(scene);
            await Awaiters.While(() => !ao.isDone);

            scenes.Remove(scene);
        }

        public static async Task UnloadAllScenes()
        {
            // удаляем все кроеме главной сцены асинхронно
            var length = scenes.Count;
            for (int i = 0; i < length; i++)
            {
                var scene = scenes[i];

                SceneUnLoaded?.Invoke(scene);

                var aop = SceneManager.UnloadSceneAsync(scene);
            }

            scenes.Clear();

            // чтобы не ждать удаления каждой из сцен последовательно, ждем здесь удаления их всех сразу
            await Awaiters.While(() => SceneManager.sceneCount > 1);
        }
        #endregion
    }
}