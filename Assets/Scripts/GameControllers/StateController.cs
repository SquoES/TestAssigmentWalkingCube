using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class StateController : MonoBehaviour, IState
{
    protected UIPanel_Handle _currentPanel;
    public abstract void PreInitState();
    public abstract void InitState();

    public virtual void DisposeState()
    {
        DisposeUI();
    }
    
    protected virtual void SetupUI(AssetReference reference)
    {
        CoroutineHandler.Start(NetExtensions.InstantiateAsync(reference, GlobalManager.Instance.Tr_Canvas, handle =>
        {
            DisposeUI();
            _currentPanel = (handle.Result as GameObject)!.GetComponentInChildren<UIPanel_Handle>();
            _currentPanel.transform.SetParent(GlobalManager.Instance.Tr_Canvas);
            _currentPanel.SetHandle(handle);
            
            SetupPanel();
        }));
    }

    protected virtual void DisposeUI()
    {
        if (_currentPanel != null)
        {
            _currentPanel.Dispose();
        }
    }

    protected virtual void SetupPanel()
    {
    }
}