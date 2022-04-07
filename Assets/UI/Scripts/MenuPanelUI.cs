using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelUI : UIPanel_Handle
{
    [SerializeField] private Button b_Start;
    internal EventHandler OnStart { get; set; }

    protected override void SetListeners()
    {
        if (b_Start)
        {
            b_Start.onClick.AddListener(() =>
            {
                if (OnStart != null)
                {
                    OnStart.Invoke(this, null);
                }
            });
        }
    }
    protected override void RemoveListeners()
    {
        if (b_Start)
        {
            b_Start.onClick.RemoveAllListeners();
        }
    }
}