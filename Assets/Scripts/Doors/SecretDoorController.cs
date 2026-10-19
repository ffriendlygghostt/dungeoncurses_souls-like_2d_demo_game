using UnityEngine;
using System.Collections;
using TMPro;

public class SecretDoorController : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D collisionBox;
    private bool isPlayerInTrigger = false;
    private bool isDoorOpen = false;
    private bool isInteract = true;

    public GameObject interactionText;
    public string requiredKeyTag = "SecretDoorKey";
    public InventoryManager inventory;
    public TextMeshProUGUI playerSpeechText; // ← текст над игроком

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject collisionObject = GameObject.Find("collisionSD");
        if (collisionObject != null)
        {
            collisionBox = collisionObject.GetComponent<BoxCollider2D>();
        }

        if (inventory == null)
            Debug.LogError("InventoryManager не присвоен вручную!");

        if (interactionText != null)
            interactionText.SetActive(false);

        if (playerSpeechText != null)
            playerSpeechText.gameObject.SetActive(false); // Скрыть текст при старте
    }

    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isInteract)
        {
            if (inventory != null && inventory.HasItemWithKeyTag(requiredKeyTag))
            {
                ToggleDoor();
            }
            else
            {
                if (playerSpeechText != null)
                {
                    playerSpeechText.gameObject.SetActive(true);
                    playerSpeechText.text = "Нужен ключ...";
                    StartCoroutine(ClearPlayerTextAfterDelay(2f));
                }
            }
        }
    }

    private void ToggleDoor()
    {
        if (isDoorOpen)
        {
            animator.SetBool("isOpening", false);
            animator.SetBool("isClosing", true);
            StartCoroutine(DisableColliderWithDelay(0.3f));
            StartCoroutine(InteractionTextDelay(0.5f));
            isDoorOpen = false;
        }
        else
        {
            animator.SetBool("isOpening", true);
            animator.SetBool("isClosing", false);
            StartCoroutine(DisableColliderWithDelay(0.4f));
            StartCoroutine(InteractionTextDelay(0.5f));
            isDoorOpen = true;

            // 💥 Удаляем ключ после открытия
            if (inventory != null)
                inventory.RemoveItemByKeyTag(requiredKeyTag);
        }
    }


    private IEnumerator DisableColliderWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (collisionBox != null)
            collisionBox.enabled = !collisionBox.enabled;
    }

    private IEnumerator InteractionTextDelay(float delay)
    {
        isInteract = false;
        if (interactionText != null)
            interactionText.SetActive(false);
        yield return new WaitForSeconds(delay);
        if (isPlayerInTrigger && interactionText != null)
            interactionText.SetActive(true);
        isInteract = true;
    }

    private IEnumerator ClearPlayerTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerSpeechText != null)
        {
            playerSpeechText.text = "";
            playerSpeechText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            if (interactionText != null)
                interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (interactionText != null)
                interactionText.SetActive(false);
        }
    }
}
