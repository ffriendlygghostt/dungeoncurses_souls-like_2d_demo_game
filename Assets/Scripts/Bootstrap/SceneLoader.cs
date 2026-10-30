using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private readonly UIManager _uiManager;
    private readonly ISaveManager _saveManager;

    public SceneLoader(UIManager uiManager, ISaveManager saveManager)
    {
        _uiManager = uiManager;
        _saveManager = saveManager;
    }

    public async UniTask LoadSceneWithFadeAsync(string sceneName, bool isInitialLoad = false)
    {
        var token = _uiManager.destroyCancellationToken;

        await _uiManager.Fade.FadeInAsync(token);

        await SceneManager.LoadSceneAsync(sceneName).WithCancellation(token);

        if (isInitialLoad)
        {
            //_uiManager.ShowScreen<MainMenu>();
        }
        else
        {
            
        }

        await UniTask.Delay(System.TimeSpan.FromMilliseconds(300), cancellationToken: token);

        await _uiManager.Fade.FadeOutAsync(token);
    }
}
