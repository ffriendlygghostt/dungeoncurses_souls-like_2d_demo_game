using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public string targetTag = "Player";
    public float loseTargetDelay = 2f;
    public Collider2D detectionZone;

    private IEnemyMovement movement;
    private Transform playerTransform;
    private Coroutine loseTargetCoroutine;

    void Start()
    {
        movement = GetComponent<IEnemyMovement>();

        if (detectionZone == null) detectionZone = FindZoneByName("DetectionZone");

        if (detectionZone == null)
            Debug.LogWarning($"[Detection] DetectionZone not found on {gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            playerTransform = collision.transform;
            movement.SetTarget(playerTransform);
            movement.SetRunning(true);

            if (loseTargetCoroutine != null)
                StopCoroutine(loseTargetCoroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled || !gameObject.activeInHierarchy) return;

        if (collision.CompareTag(targetTag))
        {
            if (loseTargetCoroutine != null)
                StopCoroutine(loseTargetCoroutine);

            loseTargetCoroutine = StartCoroutine(LoseTargetAfterDelay(loseTargetDelay));
        }
    }


    private IEnumerator LoseTargetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!PlayerStillInZone())
        {

            movement.SetTarget(null);
            movement.SetRunning(false);
            playerTransform = null;
        }
    }

    private bool PlayerStillInZone()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(detectionZone.bounds.center, detectionZone.bounds.size, 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTag))
                return true;
        }return false;
    }

    private Collider2D FindZoneByName(string name)
    {
        foreach (var col in GetComponentsInChildren<Collider2D>(true))
        {
            if (col.gameObject.name == name) return col;
        }
        return null;
    }
}
