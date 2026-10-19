using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class TradePanelManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tradePanel;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI coinCalcText;

    [Header("Interaction UI")]
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI interactionTextChoice;


    [Header("Items & UI")]
    public ItemData[] availableItems;
    public Transform itemSlotContainer;
    public GameObject itemSlotPrefab;

    [Header("Item Info Panel")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Button buyButton;

    [Header("Inventory")]
    public InventoryManager inventory;

    [Header("Player")]
    public PlayerMovement playermove;
    public PlayerAttack playerattack;
    public GameObject playerUI;

    private bool playerNearby = false;
    private bool tradePanelActive = false;

    private List<ItemSlotController> itemSlots = new();
    private ItemSlotController currentSelectedSlot;


    private enum InteractionState { Idle, Choosing, InShop, InDialogue }
    private InteractionState currentState = InteractionState.Idle;


    [Header("Cinemachine Cameras")]
    public Cinemachine.CinemachineVirtualCamera defaultCam;
    public Cinemachine.CinemachineVirtualCamera dialogueCam;

    [Header("Dialogue")]
    public TraderController currentTrader;
    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;


    private string[] scriptedDialogue = {
    "О, привет!?",
    "А... Ты из неразговорчивых...",
    "Добро пожаловать в бесконечное подземелье, место в котором каждый грешник вынужден мучится до искончания самой жизни",
    "Мой грех - жадность... Наверное... В любом случае, я люблю собирать всякие вещички!",
    "Прибывание здесь не назовёшь сказкой",
    "Но если ты ищешь что-то особенное — я могу предложить кое-что.",
    "Хотя, твоя смерть мне выгоднее — всё вернется на склад.",
    "А-ха-аха-ах!",
    "Шучу! Выгоднее вложится в будущее, поэтому я готов отдавать тебе свои ценности за разумную стоимость.",
    "Буду рад одарить тебя проклятыми артефактами, чтобы ты нашёл ещё чего интересного и пробыл здесь подольше.",
    "Глядишь поладим. Предыдущий мой напарник продержался не слишком долго. Его душа превратилась в пепел, а остатки поселились в скелете.",
    "Постарайся потерпеть подольше. Ты ведь теперь моя инвестиция!",
};

    private Coroutine typingCoroutine;
    private bool isTyping = false;




    void Start()
    {
        //currentTrader = GetComponent<TraderController>();

        if (currentTrader == null)
        {
            Debug.LogError("TraderController не найден на объекте с TradePanelManager!");
        }

        interactionText.gameObject.SetActive(false);
        interactionTextChoice.gameObject.SetActive(false);
        tradePanel.SetActive(false);
        ResetItemInfoUI();

        GenerateAllItemSlots();
        FillSlotsWithAvailableItems();

        buyButton.onClick.AddListener(BuySelectedItem);
        buyButton.interactable = false;
    }



    void GenerateAllItemSlots()
    {
        itemSlots.Clear();

        foreach (Transform child in itemSlotContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < 20; i++)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemSlotContainer);
            ItemSlotController slot = slotObj.GetComponent<ItemSlotController>();
            slot.Setup(null, this);
            itemSlots.Add(slot);
        }
    }

    void FillSlotsWithAvailableItems()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < availableItems.Length)
            {
                itemSlots[i].Setup(availableItems[i], this);
            }
            else
            {
                itemSlots[i].Setup(null, this);
            }
        }
    }

    public void UpdateMarket(ItemData[] newItems)
    {
        availableItems = newItems;
        FillSlotsWithAvailableItems();
        DeselectAll();
    }

    public void SelectItem(ItemData item, ItemSlotController slot)
    {
        foreach (var s in itemSlots)
            s.Highlight(false);

        slot.Highlight(true);
        currentSelectedSlot = slot;

        if (item != null)
        {
            itemPriceText.gameObject.SetActive(true);
            coinCalcText.gameObject.SetActive(true);

            bool canAfford = CoinWallet.coins >= item.price;
            buyButton.interactable = canAfford;

            itemPriceText.text = $"-{item.price}";
            coinCalcText.text = $"={CoinWallet.coins - item.price}";

            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;
        }
        else
        {
            DeselectAll();
        }
    }


    void DeselectAll()
    {
        currentSelectedSlot = null;

        foreach (var s in itemSlots)
            s.Highlight(false);

        ResetItemInfoUI();
    }

    void ResetItemInfoUI()
    {
        itemPriceText.gameObject.SetActive(false);
        coinCalcText.gameObject.SetActive(false);
        buyButton.interactable = false;

        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    void Update()
    {
        if (currentState == InteractionState.Choosing)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OpenTradePanel();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartDialogue();
            }
        }
        else if (currentState == InteractionState.Idle && playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            currentState = InteractionState.Choosing;
            interactionText.gameObject.SetActive(false);
            interactionTextChoice.gameObject.SetActive(true);
        }
        else if (currentState == InteractionState.InDialogue && Input.GetMouseButtonDown(0))
        {
            ContinueDialogue();
        }
        else if (currentState == InteractionState.InShop && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            CloseTradePanel();
        }
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))){
            EndDialogue();
        }
    }


    void OpenTradePanel()
    {
        tradePanel.SetActive(true);
        tradePanelActive = true;
        currentState = InteractionState.InShop;

        interactionText.gameObject.SetActive(false);
        interactionTextChoice.gameObject.SetActive(false);

        coinText.text = $"{CoinWallet.coins}";

        playermove?.FreezeMovement(true);
        playerattack?.CanAttack(false);
    }

    void CloseTradePanel()
    {
        tradePanel.SetActive(false);
        tradePanelActive = false;

        currentState = InteractionState.Idle;

        interactionText.gameObject.SetActive(true);
        interactionTextChoice.gameObject.SetActive(false);

        DeselectAll();

        playermove?.FreezeMovement(false);
        playerattack?.CanAttack(true);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            interactionText.gameObject.SetActive(true);
            interactionTextChoice.gameObject.SetActive(false);

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            interactionText.gameObject.SetActive(false);
            interactionTextChoice.gameObject.SetActive(false);

            currentState = InteractionState.Idle;
        }
    }


    public void BuySelectedItem()
    {
        if (currentSelectedSlot == null || currentSelectedSlot.CurrentItem == null)
            return;

        ItemData item = currentSelectedSlot.CurrentItem;

        if (CoinWallet.coins < item.price)
        {
            Debug.Log("Не хватает монет!");
            buyButton.interactable = false;
            return;
        }

        bool added = inventory.AddItem(item);

        CoinWallet.coins -= item.price;
        CoinWallet.UpdateWallet();
        coinText.text = $"{CoinWallet.coins}";

        currentSelectedSlot.ClearSlot();
        DeselectAll();
    }





    void StartDialogue()
    {
        if (currentTrader == null || currentTrader.textMesh == null)
        {
            Debug.LogError("Невозможно начать диалог: TraderController или textMesh не назначены.");
            return;
        }

        currentTrader.StopAllCoroutines();

        currentState = InteractionState.InDialogue;
        interactionText.gameObject.SetActive(false);
        interactionTextChoice.gameObject.SetActive(false);
        currentTrader.textMesh.text = "";
        isDialogueActive = true;
        currentDialogueIndex = 0;

        playermove?.CanMove(false);
        playerattack?.CanAttack(false);

        defaultCam.Priority = 5;
        dialogueCam.Priority = 10;
        playerUI.SetActive(false);

        ShowNextDialogue();
    }


    void EndDialogue()
    {
        currentState = InteractionState.Idle;
        currentTrader.textMesh.text = "";
        interactionText.gameObject.SetActive(true);
        interactionTextChoice.gameObject.SetActive(false);

        dialogueCam.Priority = 0;
        defaultCam.Priority = 10;
        playerUI.SetActive(true);

        playermove?.CanMove(true);
        playerattack?.CanAttack(true);
    }


    void ContinueDialogue()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            currentTrader.textMesh.text = scriptedDialogue[currentDialogueIndex];
            isTyping = false;
        }
        else
        {
            currentDialogueIndex++;
            if (currentDialogueIndex >= scriptedDialogue.Length)
            {
                EndDialogue();
            }
            else
            {
                ShowNextDialogue();
            }
        }
    }


    void ShowNextDialogue()
    {
        if (currentTrader != null && currentDialogueIndex < scriptedDialogue.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeDialogue(scriptedDialogue[currentDialogueIndex]));
        }
    }

    IEnumerator TypeDialogue(string text)
    {
        isTyping = true;
        currentTrader.textMesh.text = "";

        foreach (char c in text)
        {
            currentTrader.textMesh.text += c;
            yield return new WaitForSeconds(0.03f); // скорость печатания (0.03 = быстро)
        }

        isTyping = false;
    }



}
