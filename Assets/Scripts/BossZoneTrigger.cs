using UnityEngine;

public class BossZoneTrigger : MonoBehaviour
{
    private bool hasEntered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasEntered && other.CompareTag("Player"))
        {
            hasEntered = true;
            SoundManager.Instance?.PlayBossMusic();
        }
    }
}
