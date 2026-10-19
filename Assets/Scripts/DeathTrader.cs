using UnityEngine;
using TMPro;
using System.Collections;
using TriggerNamespace; // Подключаем пространство имен с TriggerHandler

public class DeathTrader : MonoBehaviour
{
    public string[] phrases = {
        "Твоя судьба уже решена.",
        "Ты думаешь, что выбор есть? Ошибаешься.",
        "Ты играешь с огнем, маленький смертный.",
        "Ты думаешь, что ты уникален? Ты лишь капля в океане мрака.",
        "Все, что ты знал, станет пеплом.",
        "Темные силы жаждут твоей души.",
        "Сделка с дьяволом? Я знаю таких, как ты.",
        "Проклятье тянется за тобой, как тень.",
        "Твой страх — моя сила.",
        "Смерть — это всего лишь начало моего влияния."
    }; // Массив фраз
    private int currentPhraseIndex = 0; // Индекс текущей фразы
    public TMP_Text dialogueText; // Ссылка на TextMeshPro текстовое поле

    public float phraseDelay = 10f; // Задержка между сменой фраз
    private Transform playerTransform; // Ссылка на трансформ игрока

    void Start()
    {
        // Находим триггер по имени в сцене
        GameObject trigger = GameObject.Find("TriggerDialogDD");
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
            Debug.LogError("Trigger 'TriggerDialogDD' or its Collider2D not found!");
        }

        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    private void OnTriggerEntered(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowPhrasesWithDelay());
        }
    }

    IEnumerator ShowPhrasesWithDelay()
    {
        while (true)
        {
            ShowNextPhrase();
            FlipTowardsPlayer();
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

    private void FlipTowardsPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 direction = playerTransform.position - transform.position;
            if (direction.x >= 0.1f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x <= -0.1f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}

// Переносим класс в отдельное пространство имен
namespace TriggerNamespace
{
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
}
