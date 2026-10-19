using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDoorPuzzle : MonoBehaviour
{
    public CandleControllerPuzzle[] candles;
    public int[] expectedOrder; // Пример: { 1, 0, 2 }
    public Animator doorAnimator;
    public float resetDelay = 2f;
    public GameObject Light;

    private bool puzzleSolved = false;
    private List<int> allExtinguishedOrder = new List<int>();

    private BoxCollider2D box;
    

    void Start()
    {
        if (candles == null || candles.Length == 0)
        {
            Debug.LogError("Нет свечей в массиве!");
        }
        box = GetComponent<BoxCollider2D>();
        if (Light != null) Light.SetActive(false);
    }

    public void OnCandleExtinguished(int candleIndex)
    {
        if (puzzleSolved)
            return;

        if (allExtinguishedOrder.Contains(candleIndex))
            return;

        allExtinguishedOrder.Add(candleIndex);

        if (allExtinguishedOrder.Count == expectedOrder.Length)
        {
            if (CheckSequence(allExtinguishedOrder))
            {
                OpenDoor();
                puzzleSolved = true;
            }
            else
            {
                StartCoroutine(ResetAfterDelay());
            }
        }
    }

    private bool CheckSequence(List<int> playerSequence)
    {
        for (int i = 0; i < expectedOrder.Length; i++)
        {
            if (playerSequence[i] != expectedOrder[i])
                return false;
        }
        return true;
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);
        ResetCandles();
    }

    private void ResetCandles()
    {
        allExtinguishedOrder.Clear();
        foreach (var candle in candles)
        {
            candle.Ignite();
        }
    }







    private void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpening", true);
        }
        box.isTrigger = true;
        if (Light != null) Light.SetActive(true);
    }
}
