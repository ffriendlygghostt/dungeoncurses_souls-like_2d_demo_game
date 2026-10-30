using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class UIManager : MonoBehaviour
{
    public enum UILayer { HUD, Screens, Overlay };

    [Serializable]
    public struct ScreenConfig
    {
        public UIScreen Prefab;
        public UILayer Layer;
    }


    [Header("UI Layers Containers")]
    [SerializeField] private Transform _hudLayer;
    [SerializeField] private Transform _screensLayer;
    [SerializeField] private Transform _overlayLayer;

    [Header("All Game Screens Registrations")]
    [SerializeField] private List<ScreenConfig> _screenConfigs;

    private IObjectResolver _container;
    private readonly Dictionary<Type, UIScreen> _activeScreens =
        new Dictionary<Type, UIScreen>();
    private readonly Dictionary<Type, ScreenConfig> _configCache =
        new Dictionary<Type, ScreenConfig>();

    [Inject] 
    public void Construct(IObjectResolver container)
    {
        _container = container;

        foreach (var config in _screenConfigs)
        {
            if (config.Prefab != null)
                _configCache[config.Prefab.GetType()] = config;
        }
    }

    public void Initialize()
    {
        ShowScreen<FadeScreen>().HideImmediate();
    }

    public T ShowScreen<T>() where T : UIScreen
    {
        if (_activeScreens.TryGetValue(typeof(T), out var existingScreen))
        {
            existingScreen.Show();
            return (T)existingScreen;
        }

        if (!_configCache.TryGetValue(typeof(T), out var config))
        {
            Debug.LogError($"Prefabs for Screen {typeof(T).Name} not specified in UIManager!");
            return null;
        }

        Transform targetContainer = GetLayerContainer(config.Layer);
        T newScreen = _container.Instantiate((T)config.Prefab, targetContainer);

        _activeScreens.Add(typeof(T), newScreen);
        newScreen.Show();
        return newScreen;
    }

    public void HideScreen<T>(bool destroy = false) where T : UIScreen
    {
        if (_activeScreens.TryGetValue(typeof(T), out var screen))
        {
            screen.Hide();

            if (destroy)
            {
                _activeScreens.Remove(typeof(T));
                Destroy(screen.gameObject);
            }
        }
    }

    private Transform GetLayerContainer(UILayer layer)
    {
        return layer switch
        {
            UILayer.HUD => _hudLayer,
            UILayer.Screens => _screensLayer,
            UILayer.Overlay => _overlayLayer,
            _ => _overlayLayer
        };
    }

    public FadeScreen Fade
    {
        get
        {
            if (_activeScreens.TryGetValue(typeof(FadeScreen), out var screen))
            {
                return (FadeScreen)screen;
            }
            return ShowScreen<FadeScreen>();
        }
    }

}
