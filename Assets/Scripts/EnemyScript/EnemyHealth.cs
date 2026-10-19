using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IHealth
{
    public GameObject healthBarObject;
    private Slider healthBar;
    public GameObject coinPrefab;

    public GameObject damagePopupPrefab;
    public Transform popupSpawnPoint;

    public float parryTime = 2f;

    private EnemyStats stats;
    private int currentHealth;
    private bool IsDie = false;
    private Animator animator;
    private IEnemyMovement movement;
    private IEnemyAttack attack;

    private Collider2D collider;

    private Rigidbody2D rigidbody2;

    private bool isParryWindow = false;





    void Start() 
    {
        stats = GetComponent<EnemyStats>();
        currentHealth = stats.maxHealth;
        animator = GetComponent<Animator>();
        movement = GetComponent<IEnemyMovement>();
        attack = GetComponent<IEnemyAttack>();
        rigidbody2 = GetComponent<Rigidbody2D>();

        if (healthBarObject != null )
            healthBar = healthBarObject.GetComponent<Slider>();

        if (healthBar != null)
        {
            healthBar.maxValue = stats.maxHealth;
            healthBar.value = currentHealth;
        }

        collider = GetComponent<Collider2D>();
    }

    public void ApplyDamage(int amount)
    {
        ApplyDamage(amount, Color.white); 
    }

    public void ApplyDamage(int amount, Color color)
    {
        if (IsDie) return;

        currentHealth -= amount;

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (isParryWindow)
        {
            // 🎵 Звук успешного парирования
            SoundManager.Instance?.PlayParry();

            if (animator != null)
                animator.SetTrigger("IsHurt");

            movement?.StartHurtFreeze(parryTime); //заморозка
            attack.CanAttack(false);
            StartCoroutine(UnParryAttack(parryTime));
            ShowDamagePopup(amount, Color.yellow); // желтый цвет за парирование
        }
        else
        {
            // Просто урон — анимация атаки продолжается
            ShowDamagePopup(amount, color);
        }

        if (currentHealth <= 0)
        {
            IsDie = true;
            if (healthBarObject != null)
                healthBarObject.SetActive(false);
            Die();
        }
    }


    private IEnumerator UnParryAttack(float parryTime)
    {
        yield return new WaitForSeconds(parryTime); 
        attack.CanAttack(true);
    }
    private void Die()
    {
        animator.ResetTrigger("IsHurt"); 
        animator.SetTrigger("IsDeath");

        if (movement != null)
            movement.FreezeMovement(true);
        if (attack != null)
            attack.CanAttack(false);
        if (rigidbody2 != null)
        {
            rigidbody2.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody2.mass = 10000;
        }
        DropCoins();
        if(!collider) collider.isTrigger = true;
        animator.SetTrigger("IsDeath");
    }

    private void DropCoins()
    {
        int coinsToDrop = Random.Range(0, 3);
        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.1f, 0.5f), 0);
            GameObject coin = Instantiate(coinPrefab, transform.position + offset, Quaternion.identity);

            MoneyDrop cp = coin.GetComponent<MoneyDrop>();
            if (cp != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    cp.LaunchTo(player.transform);
            }
        }
    }

    public void Resurrection()
    {
        IsDie = false;
    }

    private void ShowDamagePopup(int amount, Color color)
    {
        if (damagePopupPrefab == null || popupSpawnPoint == null)
            return;

        GameObject popup = Instantiate(damagePopupPrefab, popupSpawnPoint.position, Quaternion.identity);

        TextMeshPro text = popup.GetComponentInChildren<TextMeshPro>();

        if (text != null)
        {
            text.text = amount.ToString();
            text.color = color; // цвет урона
        }

        StartCoroutine(MoveAndFade(popup));
    }

    private void ShowDamagePopup(int amount)
    {
        ShowDamagePopup(amount, Color.white); // или любой дефолтный цвет
    }


    private IEnumerator MoveAndFade(GameObject popup)
    {
        float duration = 0.35f; // уменьшаем длительность жизни (было 0.8f)
        float elapsed = 0f;

        Vector3 start = popup.transform.position;

        // Задаём радиус смещения (на сколько далеко полетит)
        float moveDistance = 0.15f;  // можно подкорректировать под желаемую дальность

        // Случайный угол в радианах от 45° (PI/4) до 135° (3*PI/4)
        float angle = Random.Range(Mathf.PI / 4, 3 * Mathf.PI / 4);

        // Вычисляем направление движения по этому углу
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * moveDistance;

        Vector3 end = start + offset;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = popup.AddComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Линейно перемещаем popup из start в end
            popup.transform.position = Vector3.Lerp(start, end, t);

            // Плавно уменьшаем прозрачность
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(popup);
    }




    public void ParryOn()
    {
        isParryWindow = true;
    }

    public void ParryOff()
    {
        isParryWindow = false;
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
    }

    public bool IsDead => IsDie;
    public int CurrentHealth => currentHealth;
}
