using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

internal static class NetExtensions
{
    #region Addressables

    internal static IEnumerator LoadAsync<T>(AssetReference reference, Action<AsyncOperationHandle<T>> Result)
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(reference);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Result.Invoke(handle);
        }
        else
        {
            Debug.LogError($"Net extensions: Can't load {reference.SubObjectName} asset reference");
        }
    }
    internal static IEnumerator InstantiateAsync(AssetReference reference, Transform root, Action<AsyncOperationHandle> Result)
    {
        AsyncOperationHandle handle = Addressables.InstantiateAsync(reference, root);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Result.Invoke(handle);
        }
        else
        {
            Debug.LogError($"Net extensions: Can't load {reference.SubObjectName} asset reference");
        }
    }

    #endregion
}