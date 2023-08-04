using UnityEngine;

public static class MonoUtility
{
    public static T FindComponentInObjects<T>(this GameObject[] objects) where T : UnityEngine.Object
    {
        for (int i = 0; i < objects.Length; i++)
        {
            var c = objects[i].GetComponentInChildren<T>(true);
            if (c)
                return c;
        }
        return null;
    }

    public static T GetOrAddComponent<T>(this GameObject o) where T : Component
    {
        if (o.TryGetComponent(out T c))
            return c;
        return o.AddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this Component t) where T : Component
    {
        return t.gameObject.GetOrAddComponent<T>();
    }
}