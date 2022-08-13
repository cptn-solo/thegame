using System.Threading.Tasks;
using UnityEngine;

public static class ResourcesManager
{
    #region Public static methods
    public static TObject Load<TObject>(string basePath, params object[] parameters) where TObject : Object
    {
        var path = ToPath(basePath, parameters);

        var result = Resources.Load<TObject>(path);
        if (result == null)
        {
            Debug.LogFormat($"Loading from resources failed, path: {path}");

            var defaultPath = ToPathDefault<TObject>();
            if (!string.IsNullOrEmpty(defaultPath))
                result = Resources.Load<TObject>(defaultPath);
        }

        return result;
    }

    public static async Task<TObject> LoadAsync<TObject>(string basePath, params object[] parameters) where TObject : Object
    {
        var path = ToPath(basePath, parameters);

        var result = Resources.LoadAsync<TObject>(path);
        if (result == null)
            result = Resources.LoadAsync<TObject>(ToPathDefault<TObject>());

        while (!result.isDone)
            await Task.Delay(100);

        return (TObject)result.asset;
    }

    public static GameObject LoadPrefab(string basePath, params object[] parameters) =>
        Load<GameObject>(basePath, parameters);

    public static GameObject InstantiatePrefab(Transform parent, string basePath, params object[] parameters)
    {
        var prefab = Load<GameObject>(basePath, parameters);
        if (prefab == null)
            return null;

        var instantiated = parent == null ? GameObject.Instantiate(prefab) : GameObject.Instantiate(prefab, parent, false);
        return instantiated;
    }

    public static TComponent InstantiatePrefab<TComponent>(Transform parent, string basePath, params object[] parameters) where TComponent : Component
    {
        var prefab = Load<TComponent>(basePath, parameters);
        if (prefab == null)
            return null;

        return Object.Instantiate(prefab, parent, false);
    }
    #endregion

    #region Private static methods
    private static string ToPathDefault<T>() where T : Object =>
        string.Empty;

    private static string ToPath(string basePath, params object[] parameters) =>
        parameters.Length > 0 ? string.Format(basePath, parameters) : basePath;
    #endregion
}