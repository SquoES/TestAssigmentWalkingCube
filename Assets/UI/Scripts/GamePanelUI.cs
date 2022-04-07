using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePanelUI : UIPanel_Handle
{
    [SerializeField] private Button b_Pause;
    internal EventHandler OnPause { get; set; }

    protected override void SetListeners()
    {
        if (b_Pause)
        {
            b_Pause.onClick.AddListener(() =>
            {
                if (OnPause != null)
                {
                    OnPause.Invoke(this, null);
                }
            });
        }
    }

    protected override void RemoveListeners()
    {
        if (b_Pause)
        {
            b_Pause.onClick.RemoveAllListeners();
        }
    }
}
