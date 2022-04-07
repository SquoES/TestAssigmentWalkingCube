using System;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelUI : UIPanel_Handle
{
    [SerializeField] private Button b_Restart;
    [SerializeField] private Button b_NextLevel;
    [SerializeField] private Button b_Exit;

    internal EventHandler OnRestart { get; set; }
    internal EventHandler OnNextLevel { get; set; }
    internal EventHandler OnExit { get; set; }

    protected override void SetListeners()
    {
        if (b_Restart)
        {
            b_Restart.onClick.AddListener(() =>
            {
                if (OnRestart != null)
                {
                    OnRestart.Invoke(this, null);
                }
            });
        }
        if (b_NextLevel)
        {
            b_NextLevel.onClick.AddListener(() =>
            {
                if (OnNextLevel != null)
                {
                    OnNextLevel.Invoke(this, null);
                }
            });
        }
        if (b_Exit)
        {
            b_Exit.onClick.AddListener(() =>
            {
                if (OnExit != null)
                {
                    OnExit.Invoke(this, null);
                }
            });
        }
    }

    protected override void RemoveListeners()
    {
        if (b_Restart)
        {
            b_Restart.onClick.RemoveAllListeners();
        }
        if (b_NextLevel)
        {
            b_NextLevel.onClick.RemoveAllListeners();
        }
        if (b_Exit)
        {
            b_Exit.onClick.RemoveAllListeners();
        }
    }
}
