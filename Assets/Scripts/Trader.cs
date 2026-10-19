using UnityEngine;
using TMPro;
using System.Collections;

public class TraderController : MonoBehaviour
{
    public Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    public TextMeshProUGUI textMesh;
    public string[] traderDialogue = {
        "Привет, грешник!",
        "Ты ещё не умер?",
        "Беру от жизни все, что по акции...",
        "Как и большинство грешников, я решил отложить раскаяние до завтра.",
        "Ты ищешь что-то, что оправдает твои сомнительные жизненные решения?",
        "Заглядывай почаще, предметы, как и подземелье - бесконечны!",
        "Здесь ты найдешь всё, что нужно, чтобы выжить. Ну, кроме надежды. Её придется искать где-то еще...",
        "Здесь нет никаких скрытых платежей! Хотя, возможно, некоторые из моих товаров могут быть проклятыми. Но это не точно...",
        "С каждой покупкой вы получаете шанс на счастье! Или, по крайней мере, шанс на не такую ужасную смерть...",
        "Тут ты найдешь всё, кроме спасения. Оно, к сожалению, давно вне ассортимента.",
        "Если тебе кажется, что хуже быть не может — просто подожди новую распродажу!",
    };
    public float displayTime = 7f;
    public float nextDialogueDelay = 20f;

    private bool playerNearby = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(DisplayDialogue());
    }

    void Update()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.x < transform.position.x)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            textMesh.text = "";
        }
    }

    IEnumerator DisplayDialogue()
    {
        while (true)
        {
            yield return new WaitUntil(() => playerNearby);

            if (traderDialogue.Length > 0)
            {
                int randomIndex = Random.Range(0, traderDialogue.Length);


                if (randomIndex >= 0 && randomIndex < traderDialogue.Length)
                {
                    textMesh.text = traderDialogue[randomIndex];

                }
                else
                {
                    Debug.LogError("Random index out of bounds!");
                }
            }
            else
            {
                Debug.LogError("Trader dialogue array is empty!");
            }

            yield return new WaitForSeconds(displayTime);

            textMesh.text = "";


            yield return new WaitForSeconds(nextDialogueDelay);

        }
    }
}
