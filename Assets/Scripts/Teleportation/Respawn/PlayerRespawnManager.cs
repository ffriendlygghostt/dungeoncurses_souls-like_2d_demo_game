using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    public static Vector3 LastRestPosition { get; private set; }
    public static bool HasRested => _hasRested;
    private static bool _hasRested = false;

    public static void SetRestPosition(Vector3 position)
    {
        LastRestPosition = position;
        _hasRested = true;
    }
}
