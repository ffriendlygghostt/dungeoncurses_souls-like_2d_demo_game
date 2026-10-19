using UnityEngine;

public class InteractionHintTrigger : MonoBehaviour
{
    [Tooltip("UI объект подсказки, например иконка E")]
    public GameObject hintUI;

    private void Start()
    {
        if (hintUI != null)
            hintUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintUI != null)
            hintUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintUI != null)
            hintUI.SetActive(false);
    }
}
