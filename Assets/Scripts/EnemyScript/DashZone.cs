using System;
using UnityEngine;

public class DashZone : MonoBehaviour
{
    private GameObject playerObject = null;

    public bool IsPlayerInZone => playerObject != null;
    public GameObject PlayerObject => playerObject;

    public event Action OnPlayerEnterZone;
    public event Action OnPlayerExitZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerObject == null)
            {
                playerObject = other.gameObject;
                OnPlayerEnterZone?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject == playerObject)
        {
            playerObject = null;
            OnPlayerExitZone?.Invoke();
        }
    }

    public void ForceExit()
    {
        playerObject = null;
        OnPlayerExitZone?.Invoke();
    }
}
