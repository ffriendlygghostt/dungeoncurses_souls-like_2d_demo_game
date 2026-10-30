using Cysharp.Threading.Tasks;

public interface ISaveManager
{
    SaveData Progress { get; }
    SettingsData Settings { get; }

    bool HasSaveData { get; }

    UniTask LoadSettingsAsync();
    UniTask SaveSettingsAsync();

    UniTask LoadGameAsync();
    UniTask SaveGameAsync();
}
