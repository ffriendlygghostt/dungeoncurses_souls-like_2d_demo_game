//using UnityEngine;
//using TMPro; // Используем TextMeshPro
//using System.Collections;

//public class NPCInteraction : MonoBehaviour
//{
//    public GameObject dialoguePanel; // Панель диалога
//    public GameObject hintCanvas; // Канвас с подсказкой
//    public TMP_Text dialogueText; // Ссылка на текстовый элемент TextMeshPro
//    private bool isPlayerInRange = false;
//    private float cooldownTime = 2f; // Время кулдауна в секундах
//    private float lastInteractionTime = 0f; // Время последнего взаимодействия
//    private PlayerController playerController; // Ссылка на компонент PlayerController

//    // Массив фраз для ведьмы
//    private string[] witchPhrases = new string[]
//    {
//        "Добро пожаловать в царство теней. Я жду тех, кто заблудился между мирами.",
//        "Ах, ты уже здесь. Я чувствую холод смерти в воздухе... и в тебе.",
//        "Смерть — не конец, а всего лишь переход. Но ты, похоже, уже это знаешь.",
//        "Здесь, в подземелье, тайны жизни и смерти переплетаются. Готов узнать свои?",
//        "Каждый, кто входит сюда, сталкивается с тем, что скрыто в тени. Ты готов к этому?",
//        "Я вижу, ты не боишься темноты. Или, возможно, ты просто потерял страх?",
//        "В этом месте жизнь и смерть танцуют в вечном объятии. Какова твоя роль в этом танце?",
//        "Добро пожаловать в подземелье, где даже тени шепчут о твоей судьбе.",
//        "Ты пришёл за истиной, но помни: некоторые истины могут быть опасны."
//    };

//    private void Start()
//    {
//        dialoguePanel.SetActive(false); // Скрываем панель диалога изначально
//        hintCanvas.SetActive(false); // Скрываем подсказку
//        playerController = FindObjectOfType<PlayerController>(); // Кэшируем ссылку на PlayerController
//    }

//    private void Update()
//    {
//        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && Time.time >= lastInteractionTime + cooldownTime)
//        {
//            lastInteractionTime = Time.time; // Сохраняем текущее время как время последнего взаимодействия
//            ToggleDialogue();
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            isPlayerInRange = true; // Игрок находится в пределах
//            hintCanvas.SetActive(true); // Показываем подсказку
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            isPlayerInRange = false; // Игрок вышел из зоны
//            HideDialogue(); // Скрываем диалог
//            hintCanvas.SetActive(false); // Скрываем подсказку

//            // Включаем возможность движения игрока
//            if (playerController != null)
//            {
//                playerController.SetCanMove(true);
//            }
//        }
//    }

//    private void ToggleDialogue()
//    {
//        bool isActive = !dialoguePanel.activeSelf; // Проверяем текущее состояние
//        dialoguePanel.SetActive(isActive); // Переключаем состояние панели

//        if (playerController != null)
//        {
//            playerController.SetCanMove(!isActive);  // Управляем движением игрока
//            playerController.canAttack = !isActive; // Управляем атакой игрока
//        }

//        if (isActive)
//        {
//            hintCanvas.SetActive(false); // Скрываем подсказку при открытии диалога
//            UpdateDialogueText();       // Обновляем текст диалога
//        }
//    }



//    private void UpdateDialogueText()
//    {
//        if (dialogueText != null)
//        {
//            // Выбираем случайную фразу из массива
//            string randomPhrase = witchPhrases[Random.Range(0, witchPhrases.Length)];
//            StartCoroutine(TypeText(randomPhrase)); // Используем выбранную фразу
//        }
//    }

//    private IEnumerator TypeText(string line)
//    {
//        dialogueText.text = ""; // Сбрасываем текст
//        foreach (char letter in line.ToCharArray())
//        {
//            dialogueText.text += letter; // Добавляем по одной букве
//            yield return new WaitForSeconds(0.02f); // Задержка между буквами
//        }
//    }

//    private void HideDialogue()
//    {
//        dialoguePanel.SetActive(false);
//    }
//}