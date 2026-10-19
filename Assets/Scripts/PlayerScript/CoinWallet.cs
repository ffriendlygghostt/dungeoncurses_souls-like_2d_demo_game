using UnityEngine;
using TMPro;

public class CoinWallet : MonoBehaviour
{
    public static int coins = 50;
    private TextMeshProUGUI coinsText;

    private void Awake()
    {
        Transform coinsObject = transform.parent.Find("MainInterface/coinsText");
        if (coinsObject != null)
        {
            coinsText = coinsObject.GetComponent<TextMeshProUGUI>();
        }
    }
    private void Start()
    {
        UpdateCoinsText();
    }


    public static void AddCoins(int amount)
    {
        coins += amount;
        FindObjectOfType<CoinWallet>().UpdateCoinsText();
    }

    public static void RemoveCoins(int amount)
    {
        coins -= amount;
        if (coins < 0)
        {
            coins = 0;
        }
        FindObjectOfType<CoinWallet>().UpdateCoinsText();
    }

    void UpdateCoinsText()
    {
        if (coinsText != null)
        {
            coinsText.text = coins.ToString();
        }
    }

    public static void UpdateWallet()
    {
        FindObjectOfType<CoinWallet>().UpdateCoinsText();
    }
}
