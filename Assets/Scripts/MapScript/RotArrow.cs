using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotArrow : MonoBehaviour, IProjectile
{
    public float speed = 5f;
    public int arrowDamage = 10;
    public GameObject rotPuddlePrefab;
    public float destroyDelayAfterImpact = 0.5f;

    private bool isFalling = true;
    private Animator animator;
    private ProjectileEffectData effectData;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isFalling)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    public void SetEffectData(ProjectileEffectData data)
    {
        this.effectData = data;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyHit.Contains(collision.gameObject))
            return;

        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        {
            alreadyHit.Add(collision.gameObject);

            IHealth health = collision.GetComponent<IHealth>();
            if (health != null)
            {
                health.ApplyDamage(arrowDamage);
            }

            IRoting rot = collision.GetComponent<IRoting>();
            if (rot != null && effectData != null)
            {
                rot.ApplyRot(effectData.duration, effectData.tickInterval, effectData.tickDamage);
            }

            var visual = collision.GetComponent<DamageVisuals>();
            visual?.ShowEffect(DamageVisuals.EffectType.Rot);
        }

        if (collision.CompareTag("StopArrow"))
        {
            if (isFalling)
                StartCoroutine(SmoothStop());
        }
    }

    private void StopArrow()
    {
        if (!isFalling) return;

        isFalling = false;
        animator.SetBool("Down", true);

        if (rotPuddlePrefab != null)
        {
            Instantiate(rotPuddlePrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject, destroyDelayAfterImpact);
    }

    private IEnumerator SmoothStop()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float startSpeed = speed;

        while (elapsed < duration)
        {
            speed = Mathf.Lerp(startSpeed, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        speed = 0f;
        StopArrow();
    }


    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDamage(int dmg)
    {
        arrowDamage = dmg;
    }

    public int GetInitialDamage() => arrowDamage;

}
