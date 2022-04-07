using UnityEngine;
using UnityEngine.AddressableAssets;

public class MenuController : StateController
{
    [SerializeField] private AssetReference ref_MenuUIPanel;

    public override void PreInitState()
    {
        
    }

    public override void InitState()
    {
        SetupUI(ref_MenuUIPanel);
    }

    public override void DisposeState()
    {
        base.DisposeState();
    }
    
    protected override void SetupPanel()
    {
        if (_currentPanel is MenuPanelUI)
        {
            SetupMenuPanel();
        }
    }

    private void SetupMenuPanel()
    {
        MenuPanelUI panel = _currentPanel as MenuPanelUI;
        panel.OnStart += (sender, args) =>
        {
            GlobalManager.Instance.SetState(GlobalState.Game);
        };
    }
}
