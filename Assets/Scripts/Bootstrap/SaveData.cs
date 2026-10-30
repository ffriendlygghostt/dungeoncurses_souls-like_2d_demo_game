using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData : MonoBehaviour
{
    public PlayerSaveData Player = new();
    public WorldSaveData World = new();
}


[Serializable]
public class PlayerSaveData
{
    public Vector3 Position = Vector3.zero;
    public int CurrentHealth = 100;
}

[Serializable]
public class WorldSaveData
{
    public string CurrentSceneName = "Level_1";
    public Dictionary<string, bool> UnlockedTeleports = new();
    public string LastActiveTeleportId = string.Empty;
}

[Serializable]
public class SettingsData
{
    public float MusicVolume = 0.7f;
    public float SfxVolume = 0.7f;
}