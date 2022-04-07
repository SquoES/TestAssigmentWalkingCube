using System;
using Cinemachine;
using UnityEngine;

public class GlobalManager : Singleton<GlobalManager>
{
    [SerializeField] private Transform tr_Canvas;
    internal Transform Tr_Canvas => tr_Canvas;


    [SerializeField] private GlobalState startState = GlobalState.Menu;
    private GlobalState _currentState = GlobalState.None;

    internal GlobalState CurrentState
    {
        get
        {
            return _currentState;
        }
        private set
        {
            GlobalState prevState = _currentState;
            _currentState = value;
            if (OnStateChanged != null)
            {
                OnStateChanged.Invoke(this, new OnStateChanged(prevState, _currentState));
            }
        }
    }

    internal EventHandler<OnStateChanged> OnStateChanged;

    [SerializeField] private StateController _menuController;
    [SerializeField] private StateController _levelController;
    private StateController _currentStateController;

    private void Start()
    {
        SetState(startState);
    }

    internal void SetState(GlobalState setState)
    {
        if (_currentState == setState) return;
        if (_currentStateController != null)
        {
            _currentStateController.DisposeState();
        }

        switch (setState)
        {
            case GlobalState.Menu:
                CameraController.SetCameraView(CameraView.General);
                _levelController.PreInitState();
                _currentStateController = _menuController;
                break;
            case GlobalState.Game:
                _currentStateController = _levelController;
                break;
            default:
                CameraController.SetCameraView(CameraView.General);
                _levelController.PreInitState();
                _currentStateController = _menuController;
                break;
        }
        _currentStateController.InitState();
        CurrentState = setState;
    }
}

internal enum GlobalState
{
    None,
    Menu,
    Game
}

internal class OnStateChanged : EventArgs
{
    public GlobalState PrevState { get; }
    public GlobalState CurrentState { get; }

    public OnStateChanged(GlobalState prevState, GlobalState currentState)
    {
        PrevState = prevState;
        CurrentState = currentState;
    }
}
