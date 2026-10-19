using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D collisionBox;
    private bool isPlayerInTrigger = false; 
    private bool isDoorOpen = false;       
    private bool isInteract = true;

    public GameObject interactionText;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject collisionObject = GameObject.Find("collisionDD");
        if (collisionObject != null)
        {
            collisionBox = collisionObject.GetComponent<BoxCollider2D>();
            if (collisionBox == null)
            {
                Debug.LogError("BoxCollider2D не найден на объекте collisionDD.");
            }
        }
        else
        {
            Debug.LogError("Объект collisionDD не найден в сцене.");
        }

        interactionText.SetActive(false);

    }

    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isInteract)
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        if (isDoorOpen)
        {
            animator.SetBool("isOpening", false);
            animator.SetBool("isClosing", true);
            StartCoroutine(DisableColliderWithDelay(0.27f));
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
        }
    }

    private IEnumerator DisableColliderWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (collisionBox.enabled) { collisionBox.enabled = false; }
        else { collisionBox.enabled = true; }   
    }
    private IEnumerator InteractionTextDelay(float delay)
    {
        isInteract = false;
        interactionText.SetActive(false);
        yield return new WaitForSeconds(delay);
        if (isPlayerInTrigger) interactionText.SetActive(true); 
        isInteract = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            isPlayerInTrigger = true;
            interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            interactionText.SetActive(false);
        }
    }
}
