using System;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    private GameObject playerObject = null;
    private IEnemyMovement move;

    public bool IsPlayerInside => playerObject != null;


    public event Action PlayerIsInside;

    public GameObject PlayerObject => playerObject;

    private void Start()
    {
        move = transform.parent?.GetComponent<IEnemyMovement>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerObject = other.gameObject;
            if (move != null)
            {
                if (!move.IsFrozenByHurt)
                    PlayerIsInside?.Invoke();
            }
            else
            {
                PlayerIsInside?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject == playerObject)
            {
                playerObject = null;
            }
        }
    }

    public void NotInside()
    {
        playerObject = null;
    }
}
