using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fireball : MonoBehaviour, IProjectile
{
    [Header("Burn Settings")]
    public int initialDamage;
    public int burnDamage;
    public float burnDuration;
    public float burnInterval;
    public float speed;

    [Header("other")]
    public Light2D fireballLight;
    public GameObject explosionZone;
    public float explosionDelay = 0f;

    public float destroyTime = 10f;

    private bool isDestroying = false;
    private Animator animator;
    private Rigidbody2D rb;

    private ProjectileEffectData effectData;


    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (fireballLight != null) fireballLight.enabled = true;
        if (explosionZone != null) explosionZone.SetActive(false);

        Destroy(gameObject, destroyTime);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("trapholl"), LayerMask.NameToLayer("Wall"));

        if (rb != null)
        {
            rb.velocity = transform.right * speed;
        }

    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
        if (rb != null)
            rb.velocity = transform.right * speed;
    }

    public void SetDamage(int damage)
    {
        initialDamage = damage;
    }

    public void SetEffectData(ProjectileEffectData effectData)
    {
        this.effectData = effectData;

        if (effectData != null && effectData.type == EffectType.Fire)
        {
            burnDamage = effectData.tickDamage;
            burnInterval = effectData.tickInterval;
            burnDuration = effectData.duration;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) return;
        if (isDestroying) return;
        StartCoroutine(DestroySequence());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("BoundaryBall")) return;
        if (isDestroying) return;
        StartCoroutine(DestroySequence());
    }

    private IEnumerator DestroySequence()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.isKinematic = true;
        }

        isDestroying = true;

        if (explosionZone != null)
            explosionZone.SetActive(true);

        yield return new WaitForSeconds(explosionDelay);

        if (animator != null)
            animator.SetBool("FirebollDestroy", true);

        if (fireballLight != null)
            yield return new WaitForSeconds(15f / 60f);

        if (fireballLight != null)
            fireballLight.enabled = false;

        yield return new WaitForSeconds((27f - 10f) / 60f);
        Destroy(gameObject);
    }

    public int GetInitialDamage() => initialDamage;
    public int GetBurnDamage() => burnDamage;
    public float GetBurnDuration() => burnDuration;
    public float GetBurnInterval() => burnInterval;
}
