using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private PlayerStats stats;
    private Rigidbody2D rb;
    private Animator animator;
    private SwordBeam swordBeam;
    private string[] attackAnims = { "IsAttack", "IsAttack2", "IsAttack3" };
    private bool isAttacking = false;
    private bool canAttack = true;
    private HealingBook healingBook;
    private IHealth health;
    private IMove move;


    public void Initialize(PlayerStats s)
    {
        stats = s;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        swordBeam = GetComponentInChildren<SwordBeam>();
        healingBook = GetComponent<HealingBook>();
        if (swordBeam != null)
        {
            swordBeam.Deactivate();
        }
        move = GetComponent<IMove>();
        health = GetComponent<IHealth>();
    }

    public void HandleCombat()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && !isAttacking)
        {
            if (GetComponent<PlayerMovement>().GetCurrentStamina() >= 30 && (health?.IsDead==false))
            {
                GetComponent<PlayerMovement>().SendMessage("ReduceStamina", 30f);
                Attack();
            }
        }
    }

    public void TryAttack()
    {
        Attack();
    }

    private void Attack()
    {
        string anim = attackAnims[Random.Range(0, attackAnims.Length)];
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

        if (dir.x != 0)
        {
            Flip(dir.x < 0);
        }

        rb.velocity = Vector2.zero;
        GetComponent<PlayerMovement>().SetAttackingState(true, dir);

        rb.AddForce(dir * stats.attackImpulse, ForceMode2D.Impulse);
        animator.SetTrigger(anim);
        swordBeam.Activate(dir);
        isAttacking = true;

        healingBook.CancelHeal();

        move?.SetCanWalk(false);

        TriggerAttackListeners(gameObject);

        // Проверка — попал ли игрок по врагу
        Collider2D hit = Physics2D.OverlapCircle(transform.position + (Vector3)(dir.normalized * 1f), 0.8f, LayerMask.GetMask("Enemy"));
        if (hit == null)
        {
            SoundManager.Instance?.PlaySwordSwing(); // звук промаха
        }
        else
        {
            SoundManager.Instance?.PlayHitEnemy(); // звук попадания
        }

        Invoke(nameof(EnableMovement), 0.25f);
        Invoke(nameof(ResetAttack), 0.3f);
        StartCoroutine(AttackCooldown());
    }



    private void EnableMovement()
    {
        move?.SetCanWalk(true);
    }


    private void ResetAttack()
    {
        isAttacking = false;
        GetComponent<PlayerMovement>().SetAttackingState(false, Vector2.zero);
        swordBeam.Deactivate();
    }


    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.8f);
        canAttack = true;
    }

    private void Flip(bool faceLeft)
    {
        Vector3 scale = transform.localScale;
        scale.x = faceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void CanAttack(bool isAttacking)
    {
        canAttack = isAttacking;

        if (!isAttacking)
        {
            this.isAttacking = false;
            GetComponent<PlayerMovement>().SetAttackingState(false, Vector2.zero);
            swordBeam.Deactivate(); 
            //move?.SetCanWalk(true);
        }
    }

    private void TriggerAttackListeners(GameObject target)
    {
        foreach (var listener in GetComponents<IAttackListener>())
        {
            listener.OnAttack(target);
        }
    }

    private void TrySelfIgniteOnAttack()
    {
        var flameEffect = GetComponent<FlameHeartEffect>();
        if (flameEffect != null && Random.value <= flameEffect.igniteChance)
        {
            var burnable = GetComponent<Burnable>();
            burnable?.Ignite(); 
        }
    }


}