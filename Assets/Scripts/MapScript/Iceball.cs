using UnityEngine.Rendering.Universal;
using UnityEngine;
using System.Collections;

public class Iceball : MonoBehaviour, IProjectile
{
    [Header("Freeze Settings")]
    public int initialDamage = 10;

    public float speed;

    [Header("Other")]
    public Light2D iceballLight;
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

        if (iceballLight != null) iceballLight.enabled = true;
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
            animator.SetBool("IceballDestroy", true);

        if (iceballLight != null)
            yield return new WaitForSeconds(15f / 60f);

        if (iceballLight != null)
            iceballLight.enabled = false;

        yield return new WaitForSeconds((27f - 10f) / 60f);
        Destroy(gameObject);
    }

    public int GetInitialDamage() => initialDamage;

    public float GetFreezeDuration()
    {
        if (effectData != null)
            return effectData.duration;
        else
            return 7f;
    }
}
