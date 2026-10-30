using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LevelLifetimeScope : LifetimeScope
{
    //[SerializeField] private Player _playerInstance;
    protected override void Configure(IContainerBuilder builder)
    {
        if (Parent == null)
        {
            ///
        }

        //builder.RegisterComponent(_playerInstance);
    }
}
