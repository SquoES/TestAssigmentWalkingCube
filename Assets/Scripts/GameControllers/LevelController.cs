using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LevelController : StateController
{
    [SerializeField] private AssetReference ref_EndGamePanel;
    [SerializeField] private AssetReference ref_InGamePanel;
    
    [SerializeField] private LevelModel levelData;
    [SerializeField] private MeshFilter _meshL;
    [SerializeField] private MeshFilter _meshR;
    private List<Vector2> _path;

    [SerializeField] private CinemachineVirtualCamera cam_GenView;
    [SerializeField] private CinemachineVirtualCamera cam_GameView;

    [SerializeField] private CharacterController character;

    public override void PreInitState()
    {
        GenerateLevel();
    }
    public override void InitState()
    {
        StartCoroutine(StartLevel());
        character.OnPathEnded += OnCharacterFinished;
    }

    public override void DisposeState()
    {
        base.DisposeState();
        character.OnPathEnded -= OnCharacterFinished;
    }

    private void GenerateLevel()
    {
        _path = CPULevelGenerator.CreateLevel(levelData, _meshL, _meshR);
    }
    private IEnumerator StartLevel()
    {
        float time = CameraController.SetCameraView(CameraView.Game);
        yield return new WaitForSeconds(time);
        RestartGame();
    }

    private void RestartGame()
    {
        character.SetPath(_path, levelData.LineWidth, levelData.LinesCount);

        SetupUI(ref_InGamePanel);
        character.SetEnable(true);
    }

    private void PauseGame()
    {
        character.SetEnable(false);
        SetupUI(ref_EndGamePanel);
    }
    
    private void OnCharacterFinished(object sender, EventArgs e)
    {
        SetupUI(ref_EndGamePanel);
    }
    
    protected override void SetupPanel()
    {
        if (_currentPanel is PausePanelUI)
        {
            SetupPausePanel();
        }
        else if (_currentPanel is GamePanelUI)
        {
            SetupGamePanel();
        }
    }

    private void SetupPausePanel()
    {
        PausePanelUI panel = _currentPanel as PausePanelUI;
        panel.OnRestart += (sender, args) => { RestartGame(); };
        panel.OnNextLevel += (sender, args) =>
        {
            GenerateLevel();
            StartCoroutine(StartLevel());
        };
        panel.OnExit += (sender, args) =>
        {
            GlobalManager.Instance.SetState(GlobalState.Menu);
        };
    }

    private void SetupGamePanel()
    {
        GamePanelUI panel = _currentPanel as GamePanelUI;
        panel.OnPause += (sender, args) => { PauseGame(); };
    }
}
