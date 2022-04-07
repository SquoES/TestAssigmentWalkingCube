using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class UIPanel_Handle : UIPanel
{
    private AsyncOperationHandle _handle;

    public virtual void SetHandle(AsyncOperationHandle handle)
    {
        _handle = handle; 
        Reuse();
    }
    public override void Dispose()
    {
        base.Dispose();
        Addressables.ReleaseInstance(_handle);
    }
}
