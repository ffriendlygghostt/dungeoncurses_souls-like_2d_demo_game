using UnityEngine;
using TMPro;

public class CurseTrigger : MonoBehaviour
{
    public CursedTrader trader; // Ссылка на скрипт торговца
    public TMP_Text traderDialogCurse; // Ссылка на TextMeshPro текстовое поле

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            trader.dialogueText = traderDialogCurse; // Передача ссылки на текстовое поле торговца
            trader.ShowNextPhrase(); // Вызов метода отображения следующей фразы торговца
        }
    }
}
