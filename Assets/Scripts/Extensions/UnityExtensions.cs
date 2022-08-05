using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Extensions
{
    public static class UnityExtensions
    {
        public static void DestroyAllChildren(this Transform _transform)
        {
            if (_transform == null)
                return;

            for (int i = 0; i < _transform.childCount; i++)
            {
                var child = _transform.GetChild(i);
                if (child != null)
#if UNITY_EDITOR
                    if (Application.isEditor && !Application.isPlaying)
                        Object.DestroyImmediate(child.gameObject);
                    else
#endif
                        Object.Destroy(child.gameObject);
            }
        }

        public static void DestroyImmediateAllChildren(this Transform _transform)
        {
            if (_transform == null)
                return;

            for (int i = _transform.childCount - 1; i >= 0; i--)
            {
                var child = _transform.GetChild(i);
                if (child != null)
                    Object.DestroyImmediate(child.gameObject);
            }
        }

        public static Vector3 GetForward(this Rigidbody rigidbody) =>
            rigidbody.rotation * Vector3.forward;

        public static Vector3 GetUp(this Rigidbody rigidbody) =>
            rigidbody.rotation * Vector3.up;

        public static Vector3 GetRight(this Rigidbody rigidbody) =>
            rigidbody.rotation * Vector3.right;

        public static float GetMaxTime(this AnimationCurve _curve)
        {
            var length = _curve.length;
            var keys = _curve.keys;
            return length > 0 ? keys[length - 1].time : 0f;
        }

        public static T SafeDestroy<T>(this T _object) where T : Object
        {
            if (_object != null)
            {
#if UNITY_EDITOR
                if (Application.isEditor && !Application.isPlaying)
                    Object.DestroyImmediate(_object);
                else
#endif
                    Object.Destroy(_object);
            }

            return _object;
        }

        public static void SetActiveLayout(this CanvasGroup group, bool flag)
        {
            group.alpha = flag ? 1f : 0f;
            group.blocksRaycasts = flag;
            group.interactable = flag;
        }

        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }

        public static Rect RectTransformToScreenSpace(this RectTransform rectTransform)
        {
            Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
            return new Rect((Vector2)rectTransform.position - size * 0.5f, size);
        }

        public static Vector3 GetRandomDirection(this Vector3 direction, Vector3 variance)
        {
            direction += new Vector3(
                Random.Range(-variance.x, variance.x),
                Random.Range(-variance.y, variance.y),
                Random.Range(-variance.z, variance.z)
            );
            direction.Normalize();

            return direction;
        }

    }
}
