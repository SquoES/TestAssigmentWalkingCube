using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private CinemachineBrain _brain;
    [SerializeField] private CinemachineVirtualCamera[] cam_Views;
    
    [SerializeField] private CamTransition[] camTransitions;
    private Dictionary<CamTransitionState, int> _transitions = new Dictionary<CamTransitionState, int>();

    private CameraView _curView;

    private void Start()
    {
        _transitions = new Dictionary<CamTransitionState, int>();
        for (int i = 0; i < camTransitions.Length; i++)
        {
            CamTransitionState state = new CamTransitionState(camTransitions[i].From, camTransitions[i].To);
            if (!_transitions.ContainsKey(state))
            {
                _transitions.Add(state, camTransitions[i].TransitionIndex);
            }
        }
    }

    internal static float SetCameraView(CameraView setView)
    {
        return _instance.ChangeCameraView(setView);
    }

    private float ChangeCameraView(CameraView setView)
    {
        if (setView == _curView) return 0f;

        CinemachineVirtualCamera from = cam_Views[(int)_curView];
        CinemachineVirtualCamera to = cam_Views[(int)setView];

        bool exists = _transitions.TryGetValue(new CamTransitionState(_curView, setView), out int index);
        if (!exists) return 0f;
        float time = _brain.m_CustomBlends.m_CustomBlends[index].m_Blend.m_Time;

        _curView = setView;
        from.Priority = 0;
        to.Priority = 1;
        return time;
    }

    [Serializable]
    internal struct CamTransition
    {
        [SerializeField] private CameraView from;
        internal CameraView From => from;
        
        [SerializeField] private CameraView to;        
        internal CameraView To => to;
        
        [SerializeField] private int transitionIndex;        
        internal int TransitionIndex => transitionIndex;
    }

    private struct CamTransitionState
    {
        private CameraView _from;
        private CameraView _to;

        public CamTransitionState(CameraView from, CameraView to)
        {
            _from = from;
            _to = to;
        }
    }
}

public enum CameraView
{
    General,
    Game
}