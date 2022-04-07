using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : Singleton<CoroutineHandler>
{
    private static Dictionary<string, IEnumerator> _routines = new Dictionary<string, IEnumerator>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeType()
    {
        _instance = new GameObject($"#{nameof(CoroutineHandler)}").AddComponent<CoroutineHandler>();
        DontDestroyOnLoad(_instance);
    }

    public static Coroutine Start(IEnumerator routine) => _instance.StartCoroutine(routine);
    public static Coroutine Start(string layout, IEnumerator routine)
    {
        var coroutine = _instance.StartCoroutine(routine);
        if (!_routines.ContainsKey(layout)) _routines.Add(layout, routine);
        else
        {
            _instance.StopCoroutine(_routines[layout]);
            _routines[layout] = routine;
        }

        return coroutine;
    }

    public static void Stop(IEnumerator routine) => _instance.StopCoroutine(routine);

    public static void Stop(string layout)
    {
        if (_routines.TryGetValue(layout, out var routine))
        {
            _instance.StopCoroutine(routine);
            _routines.Remove(layout);
        }
        else Debug.LogError($"coroutine '{layout}' not found");
    }

    public static void StopAll() => _instance.StopAllCoroutines();
}
