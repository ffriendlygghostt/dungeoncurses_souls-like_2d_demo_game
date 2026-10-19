using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
    public float delay = 2.0f; // Время задержки между появлениями шипов
    public float duration = 1.0f; // Время, в течение которого шипы находятся в активном состоянии
    public int damageAmount = 10; // Количество урона, наносимого игроку

    private bool isActive = false;
    private float timer = 0.0f;

    private Animator animator;
    private Collider2D spikeCollider; // Коллайдер шипов

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spikeCollider = GetComponentInChildren<Collider2D>();

        // Убедитесь, что коллайдер изначально выключен
        if (spikeCollider != null)
        {
            spikeCollider.enabled = false;
        }

        SetActiveState(false);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (isActive)
        {
            if (timer >= duration)
            {
                SetActiveState(false);
                timer = 0.0f;
            }
        }
        else
        {
            if (timer >= delay)
            {
                SetActiveState(true);
                timer = 0.0f;
            }
        }
    }

    private void SetActiveState(bool active)
    {
        isActive = active;

        if (animator != null)
        {
            animator.SetBool("IsActive", isActive);
        }

        if (spikeCollider != null)
        {
            spikeCollider.enabled = isActive; // Включаем или выключаем коллайдер
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Добавьте отладочные сообщения, чтобы увидеть, что происходит
        Debug.Log($"Trigger detected with {other.gameObject.name}");

        // Проверка, если объект - игрок
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by spikes!");

            // Попробуйте найти компонент PlayerController на объекте игрока и нанести урон
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount); // Обращаемся к методу, который у тебя отвечает за нанесение урона
            }
            else
            {
                Debug.LogError("PlayerController component not found on player.");
            }
        }
    }
}
