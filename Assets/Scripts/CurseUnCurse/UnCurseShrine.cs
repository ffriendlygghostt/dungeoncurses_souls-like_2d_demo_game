using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnCurseShrine : MonoBehaviour
{
    [Tooltip("Ссылка на Player")]
    [SerializeField] private GameObject player;

    [Tooltip("Ссылка на текстовую подсказку типа [ E ]")]
    [SerializeField] private TextMeshProUGUI speechText;

    private bool playerInZone = false;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            var healingBook = player.GetComponent<HealingBook>();
            if (healingBook != null)
            {
                healingBook.RemoveCurse();
            }

            var curseManager = FindObjectOfType<CurseManager>();
            if (curseManager != null)
            {
                curseManager.RemoveAllCurses();
            }


            if (speechText != null)
                speechText.gameObject.SetActive(false);

            Debug.Log("Проклятие очищено статуей.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInZone = true;

            if (speechText != null)
            {
                speechText.text = "Нажмите [ E ] чтобы помолиться...";
                speechText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInZone = false;

            if (speechText != null)
                speechText.gameObject.SetActive(false);
        }
    }
}
