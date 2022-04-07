using System;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour, IDisposable, IReusable
{
    [Header("UIPanel fields")]
    [SerializeField] private RectTransform rt_InSafeArea;
    
    //----------Public methods----------
    public virtual void Reuse()
    {
        ApplySafeArea();
        SetListeners();
    }
    public virtual void Dispose()
    {
        RemoveListeners();
    }
    
    //---------Private methods---------
    protected abstract void SetListeners();
    protected abstract void RemoveListeners();
    protected void ApplySafeArea()
    {
        if (rt_InSafeArea == null)
        {
            rt_InSafeArea = GetComponent<RectTransform>();
        }
        
        Rect safeRect = Screen.safeArea;

        Vector2 anchorMin = safeRect.position;
        Vector2 anchorMax = safeRect.position + safeRect.size;
        
        Rect pixelRect = GetComponentInParent<Canvas>().pixelRect;
        anchorMin.x /= pixelRect.width;
        anchorMin.y /= pixelRect.height;

        anchorMax.x /= pixelRect.width;
        anchorMax.y /= pixelRect.height;

        rt_InSafeArea.anchorMin = anchorMin;
        rt_InSafeArea.anchorMax = anchorMax;
    }
}
