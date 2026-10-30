using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class GameInitializer : IAsyncStartable
{
    private readonly ISaveManager _saveManager;
    private readonly SceneLoader _sceneLoader;
    private readonly UIManager _uiManager;

    public GameInitializer(ISaveManager saveManager, SceneLoader sceneLoader, UIManager uiManager)
    {
        _saveManager = saveManager;
        _sceneLoader = sceneLoader;
        _uiManager = uiManager;
    }

    public async UniTask StartAsync(System.Threading.CancellationToken cancellationToken)
    {
        Debug.Log("[GameInitializer] Game starting! Initializing systems...");

        await _saveManager.LoadSettingsAsync();

        await _saveManager.LoadGameAsync();

        _uiManager.Initialize();

        string lastSavedScene = _saveManager.Progress.World.CurrentSceneName;
        Debug.Log("[GameInitializer] Start load game world...");
        await _sceneLoader.LoadSceneWithFadeAsync(lastSavedScene, isInitialLoad: true);
    }
}
