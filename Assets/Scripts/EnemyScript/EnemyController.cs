using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyStats stats;
    private IEnemyMovement movement;
    private IEnemyAttack attack;
    private IHealth health;

    private Transform player;


    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        movement = GetComponent<IEnemyMovement>();
        attack = GetComponent<IEnemyAttack>();
        health = GetComponent<IHealth>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        movement?.Initialize(stats);
        attack?.Initialize(stats);
    }

    private void Update()
    { 
        if (health != null && health.IsDead) return;
        if (movement != null && movement.IsFrozen) return;
        movement?.Move();
    }
}
