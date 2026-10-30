using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveManagerMock : ISaveManager
{
    public SaveData Progress { get; private set; } = new();
    public SettingsData Settings { get; private set; } = new();

    public bool HasSaveData => true;

    public async UniTask LoadSettingsAsync()
    {
        await UniTask.Delay(50);
        Settings.MusicVolume = 0.5f;
        Settings.SfxVolume = 0.5f;
        Debug.Log("[SaveManagerMock] Global settings loading!");
    }

    public async UniTask SaveSettingsAsync()
    {
        await UniTask.Delay(50);
        Debug.Log("[SaveManagerMock] Global settings saving!");
    }

    public async UniTask LoadGameAsync()
    {
        await UniTask.Delay(100);

        Progress.World.CurrentSceneName = "Level_1";
        Progress.Player.Position = new Vector3(5f, 2f, 0f);
        Progress.World.UnlockedTeleports["Teleport_Camp_1"] = true;
        Debug.Log("[SaveManagerMock] Game progress loading!");
    }

    public async UniTask SaveGameAsync()
    {
        await UniTask.Delay(100);
        Debug.Log("[SaveManagerMock] Game progress saving!");
    }
}
