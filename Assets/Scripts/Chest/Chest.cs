using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Chest : MonoBehaviour
{
    [Header("Drops")]
    public ItemData itemInsideChest;
    public GameObject itemPopup;
    public Transform itemPopupSpawnPoint;
    public int coinAmount = 50;
    public int coinMin;
    public int coinMax;
    public GameObject coinsText;


    [Header("Other")]
    public Animator animator;
    public GameObject lightIndicator;
    public GameObject interactionText;

    private ShineEffect shineEffectScript;
    private Transform parent;
    private InventoryManager inventory;



    private bool isPlayerRange = false;
    private bool isChestOpened = false;

    public GameObject coinPopup;
    public Transform popupSpawnPoint;



    [SerializeField] private ParticleSystem firefly;
    [SerializeField] private ParticleSystem chestFountain;
    [SerializeField] private GameObject shineEffect;
    [SerializeField] private Light2D chestLight;


    private void Start()
    {
        GameObject escManager = GameObject.FindWithTag("ESCManager");
        if (escManager != null)
        {
            inventory = escManager.GetComponentInChildren<InventoryManager>(true);
            if (inventory == null)
                Debug.LogWarning("InventoryManager не найден внутри ESCManager!");
        }
        else
        {
            Debug.LogWarning("ESCManager с тегом не найден!");
        }

        interactionText.SetActive(false);

        if (coinPopup != null)
            coinPopup.SetActive(false);
        if (itemPopup != null)
            itemPopup.SetActive(false);


        if (coinMin != 0 && coinMax != 0)
        {
            coinAmount = Random.Range(coinMin, coinMax);
        }

        parent = transform.parent;

        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                if (child == transform) continue;

                if (firefly == null && child.GetComponent<ParticleSystem>() != null)
                    firefly = child.GetComponent<ParticleSystem>();

                if (shineEffect == null && child.name.ToLower().Contains("shine"))
                    shineEffect = child.gameObject;

                if (chestLight == null && child.GetComponent<Light2D>() != null)
                    chestLight = child.GetComponent<Light2D>();
            }
        }

        shineEffectScript = GetComponent<ShineEffect>();
        chestFountain = GetComponentInChildren<ParticleSystem>();
    }


    private void Update()
    {
        if (isPlayerRange && !isChestOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isChestOpened = true;
        animator.SetTrigger("OpenChest");

        if (coinAmount > 0)
        {
            CoinWallet.AddCoins(coinAmount);
            ShowCoinPopup(coinAmount);
        }
        if (itemInsideChest != null && inventory != null)
        {
            inventory.AddItem(itemInsideChest);
            StartCoroutine(DelayedItemPopup(itemInsideChest, 0.4f));

        }

        Destroy(lightIndicator);
        Destroy(interactionText);
        if (firefly != null) Destroy(firefly.gameObject);
        if (chestLight != null) Destroy(chestLight);
        if (shineEffect != null) Destroy(shineEffect);
        if (shineEffectScript != null) Destroy(shineEffectScript);
        if (chestFountain != null)
        {
            chestFountain.Play();
            Destroy(chestFountain.gameObject, chestFountain.main.duration + chestFountain.main.startLifetime.constantMax);
        }

        Destroy(GetComponent<Collider2D>());

        if (itemInsideChest == null && itemPopup != null)
        {
            Destroy(itemPopup);
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerRange = true;
            interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerRange = false;
            interactionText.SetActive(false);
        }
    }

    private void ShowCoinPopup(int amount)
    {
        coinPopup.SetActive(true);
        GameObject popupInstance = Instantiate(coinPopup, popupSpawnPoint.position, Quaternion.identity, transform);
        
        TextMeshProUGUI coinText = popupInstance.GetComponentInChildren<TextMeshProUGUI>();
        coinText.text = amount.ToString();

        popupInstance.transform.localScale *= 3.1f;
        Destroy(coinPopup);
        StartCoroutine(MoveAndDestroyPopup(popupInstance)); 
    }

    private void ShowItemPopup(ItemData item)
    {
        GameObject popupInstance = Instantiate(itemPopup, itemPopupSpawnPoint.position, Quaternion.identity);
        popupInstance.SetActive(true);

        var frame = popupInstance.transform.Find("Frame")?.GetComponent<SpriteRenderer>();
        var icon = popupInstance.transform.Find("Icon")?.GetComponent<SpriteRenderer>();

        if (icon != null && frame != null && item.icon != null)
        {
            icon.sprite = item.icon;

            // Размер рамки в юнитах
            Vector2 frameSize = frame.bounds.size;
            // Размер иконки в юнитах
            Vector2 iconSize = icon.sprite.bounds.size;

            // Масштаб подгонки: находим, во сколько раз нужно уменьшить иконку
            float scaleX = frameSize.x / iconSize.x;
            float scaleY = frameSize.y / iconSize.y;
            float finalScale = Mathf.Min(scaleX, scaleY) * 0.9f; // 90% для небольшого отступа

            icon.transform.localScale = new Vector3(finalScale, finalScale, 1f);
        }

        var rb = popupInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 impulse = new Vector2(Random.Range(-0.3f, 0.3f), 1.2f);
            rb.AddForce(impulse * 100f);
        }

        StartCoroutine(MoveAndDestroyPopup(popupInstance));
    }










    private IEnumerator MoveAndDestroyPopup(GameObject popup)
    {
        float duration = 1.2f;
        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 0.5f, 0);
        float t = 0f;

        while (t < duration)
        {
            popup.transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(popup);
    }
    private IEnumerator DelayedItemPopup(ItemData item, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowItemPopup(item);
    }

}

