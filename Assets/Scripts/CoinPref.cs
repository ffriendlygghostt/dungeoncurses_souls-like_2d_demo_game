using System.Collections;
using UnityEngine;

public class CoinPref : MonoBehaviour
{
    public float flySpeed = 5f; // Скорость полета монеты
    public float hangTime = 1f; // Время, которое монета висит в воздухе перед тем как лететь к игроку

    private Transform playerTransform; // Ссылка на трансформ игрока
    private bool isFlying = false; // Флаг для определения, летит ли монета к игроку
    private Collider2D coinCollider; // Коллайдер монеты

    private void Start()
    {
        // Находим игрока по тегу "Player" и получаем ссылку на его трансформ
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Если игрок не найден, выводим сообщение об ошибке
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
        }
        else
        {
            // Запускаем корутину, чтобы монета зависла в воздухе на некоторое время, а затем начала лететь к игроку
            StartCoroutine(HangThenFly());
        }

        // Активируем объект монеты 
        gameObject.SetActive(true);

        // Получаем коллайдер монеты
        coinCollider = GetComponent<Collider2D>();
        if (coinCollider == null)
        {
            Debug.LogError("Collider2D not found on the coin prefab.");
        }
    }

    private void Update()
    {
        // Если монета в полете к игроку
        if (isFlying)
        {
            // Вычисляем направление к игроку
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            // Перемещаем монету в этом направлении с заданной скоростью
            transform.Translate(direction * flySpeed * Time.deltaTime, Space.World);
        }
    }

    private IEnumerator HangThenFly()
    {
        // Задержка, чтобы монета зависла в воздухе
        yield return new WaitForSeconds(hangTime);

        // Устанавливаем флаг в true, чтобы монета начала лететь к игроку
        isFlying = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Если монета столкнулась с игроком
        if (other.CompareTag("Player"))
        {
            // Уничтожаем монету
            Destroy(gameObject);

            // Добавляем один коин в кошелек
            CoinWallet.AddCoins(1);
        }
    }
}
