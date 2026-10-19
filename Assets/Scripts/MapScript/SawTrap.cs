using UnityEngine;
using System.Collections;

public class SawTrap : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.right;
    public float moveSpeed = 3f;
    public float pauseDuration = 1f;
    public string[] blockerTags = { "LeftBlock", "RightBlock", "TopBlock", "BottomBlock" };

    public int damageOnContact = 10;
    public float bleedDuration = 7.5f;
    public int defaultBleedDamage = 2;
    public float defaultTickInterval = 1f;


    private bool isPaused = false;

    void Update()
    {
        if (!isPaused)
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var tag in blockerTags)
        {
            if (other.CompareTag(tag))
            {
                StartCoroutine(PauseAndReverse());
                return;
            }
        }

        var health = other.GetComponentInParent<IHealth>();
        if (health != null)
        {
            health.ApplyDamage(damageOnContact);

            var bleedable = other.GetComponentInParent<IBleeding>();
            if (bleedable != null)
                bleedable.ApplyBleed(bleedDuration, defaultTickInterval, defaultBleedDamage );
        }
    }

    private IEnumerator PauseAndReverse()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        moveDirection *= -1;
        isPaused = false;
    }
}
