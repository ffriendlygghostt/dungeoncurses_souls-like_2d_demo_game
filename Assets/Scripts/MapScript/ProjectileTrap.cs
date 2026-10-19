using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static DamageVisuals;

public class ProjectileTrap : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform spawnPoint;

    public float fireInterval = 5f;
    public float projectileSpeed = 5f;
    public int projectileDamage = 10;

    public Light2D trapLight;
    private Animator animator;

    private float nextFireTime;

    [Header("Effect Data")]
    public ProjectileEffectData effectData;


    void Start()
    {
        animator = GetComponent<Animator>();

        if (trapLight != null)
            trapLight.enabled = false;
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            StartCoroutine(Fire());
            nextFireTime = Time.time + fireInterval;
        }
    }

    IEnumerator Fire()
    {
        animator.SetBool("IsAttacking", true);
        if (trapLight != null)
            trapLight.enabled = true;

        yield return new WaitForSeconds(13f / 60f);

        var projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        var projectileScript = projectile.GetComponent<IProjectile>();

        if (projectileScript != null)
        {
            projectileScript.SetSpeed(projectileSpeed);
            projectileScript.SetDamage(projectileDamage);
            projectileScript.SetEffectData(effectData);
        }


        yield return new WaitForSeconds(2f / 60f);

        if (trapLight != null)
            trapLight.enabled = false;
        animator.SetBool("IsAttacking", false);
    }
}
