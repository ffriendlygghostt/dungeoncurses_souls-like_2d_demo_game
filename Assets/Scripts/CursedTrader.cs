using UnityEngine;
using TMPro;
using System.Collections;

public class CursedTrader : MonoBehaviour
{
    public string[] phrases = {
        "Мрак подобен голосу безмолвия, зовущему тебя в свои объятия",
        "Ты близок к встрече со своим кошмаром.",
        "Твоя душа будет моей, хочешь ты этого или нет.",
        "Ты стоишь на краю бесконечного мрака, его тень уже обнимает тебя",
        "Никто не вышел отсюда живым, ты не будешь исключением.",
        "Скоро ты поймешь, что нет ничего страшнее меня.",
        "Подготовься к встрече с неизбежным.",
        "Здесь твои мечты превращаются в кошмары.",
        "Я — последний преграда перед вечным сном.",
        "Под звездным светом моих крыльев, ты стоишь перед последней дорогой."
    }; // Массив фраз
    private int currentPhraseIndex = 0; // Индекс текущей фразы
    public TMP_Text dialogueText; // Ссылка на TextMeshPro текстовое поле

    public float phraseDelay = 7f; // Задержка между сменой фраз

    public Transform player; // Ссылка на персонажа

    public Animator animator; // Ссылка на аниматор босса

    public float moveSpeed = 2f; // Скорость движения босса
    public float attackDistance = 1.5f; // Дистанция до игрока для атаки

    public int damageAmount = 200; // Урон, наносимый игроку

    public float attackDelay = 2f; // Задержка между атаками
    private bool canAttack = true; // Флаг, разрешающий атаку
    private bool isAttacking = false; // Флаг атаки босса

    private bool isFlying = false; // Флаг полета босса
    private bool shouldAttack = false; // Флаг, указывающий, должен ли босс атаковать

    void Start()
    {
        // Находим триггер по имени в сцене
        GameObject trigger = GameObject.Find("DeathComming");
        // Получаем компонент Collider2D у триггера
        Collider2D triggerCollider = trigger.GetComponent<Collider2D>();
        // Проверяем, что триггер и его коллайдер найдены
        if (trigger != null && triggerCollider != null)
        {
            // Вешаем обработчик на событие входа в триггер
            triggerCollider.isTrigger = true;
            triggerCollider.gameObject.AddComponent<TriggerHandler>().OnTriggerEnterEvent += OnTriggerEntered;
        }
        else
        {
            Debug.LogError("Trigger 'DeathComming' or its Collider2D not found!");
        }
    }

    private void OnTriggerEntered(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowPhrasesWithDelay());
            isFlying = true;
            animator.SetBool("Flying", true);
            shouldAttack = true; // Включаем атаку, когда игрок входит в триггер
        }
    }

    IEnumerator ShowPhrasesWithDelay()
    {
        while (true)
        {
            ShowNextPhrase();
            yield return new WaitForSeconds(phraseDelay);
        }
    }

    public void ShowNextPhrase()
    {
        if (phrases.Length > 0)
        {
            // Установите текст в UI
            dialogueText.text = phrases[currentPhraseIndex];

            // Перейти к следующей фразе
            currentPhraseIndex = (currentPhraseIndex + 1) % phrases.Length;
        }
    }

    void Update()
    {
        if (player != null && isFlying)
        {
            // Определяем направление к персонажу
            Vector3 direction = (player.position - transform.position).normalized;
            float distance = Vector3.Distance(player.position, transform.position);

            // Двигаемся к игроку, независимо от расстояния до игрока
            transform.position += direction * moveSpeed * Time.deltaTime;
            // Воспроизводим анимацию движения
            animator.SetBool("isMoving", true);

            // Флипаем торговца в сторону персонажа, если он находится слева
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            // Иначе, если персонаж справа, оставляем без изменений
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            // Атакуем игрока, если можем атаковать
            if (!isAttacking && canAttack && shouldAttack)
            {
               
                isAttacking = true;
                animator.SetTrigger("Attack1");
                StartCoroutine(ResetAttack());
            }
        }
    }

    // Метод, вызываемый событием анимации для нанесения урона
    public void DealDamage()
    {
        if (player == null)
        {
            // Если игрок уничтожен, выходим из метода
            isAttacking = false;
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= attackDistance)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount);
            }
        }
    }

    IEnumerator ResetAttack()
    {
        // Ждем заданную задержку
        yield return new WaitForSeconds(attackDelay);
        // Сбрасываем флаг атаки и разрешаем новую атаку
        isAttacking = false;
        canAttack = true;
    }
}

// Класс для обработки событий триггера
public class TriggerHandler : MonoBehaviour
{
    // Событие входа в триггер
    public event System.Action<Collider2D> OnTriggerEnterEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Вызываем событие входа в триггер
        OnTriggerEnterEvent?.Invoke(other);
    }
}
