using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private UIManager _uiManagerPrefab;


    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ISaveManager, SaveManagerMock>(Lifetime.Singleton);
        builder.Register<SettingsManager>(Lifetime.Singleton);

        builder.Register<SceneLoader>(Lifetime.Singleton);

        builder.RegisterComponentInNewPrefab(_uiManagerPrefab, Lifetime.Singleton)
                .DontDestroyOnLoad()
                .AsSelf();

        builder.RegisterEntryPoint<GameInitializer>();
        

        
    }
}
