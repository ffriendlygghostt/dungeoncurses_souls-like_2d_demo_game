using UnityEngine;

public class UIInputBlocker : MonoBehaviour
{
    public GameObject[] trackedPanels;
    public PlayerAttack playerAttack;

    void Update()
    {
        bool isAnyPanelOpen = false;

        foreach (var panel in trackedPanels)
        {
            if (panel != null && panel.activeInHierarchy)
            {
                isAnyPanelOpen = true;
                break;
            }
        }

        if (playerAttack != null)
            playerAttack.CanAttack(!isAnyPanelOpen);
    }
}
