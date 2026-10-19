using UnityEngine;

public class ActivityManager : MonoBehaviour
{
    public ActivitySlotUI[] activitySlots;

    public GameObject player;

    private void Start()
    {
        for (int i = 0; i < activitySlots.Length; i++)
        {
            activitySlots[i].Init(this, i);
        }
    }

    public void SetActivitySlot(int index, ItemData item)
    {
        if (index < 0 || index >= activitySlots.Length) return;
        activitySlots[index].SetItem(item);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) TryActivate(0);
        if (Input.GetKeyDown(KeyCode.X)) TryActivate(1);
        if (Input.GetKeyDown(KeyCode.C)) TryActivate(2);
    }

    private void TryActivate(int index)
    {
        if (index < 0 || index >= activitySlots.Length) return;
        var slot = activitySlots[index];

        if (slot.CanUseActivity)
        {
            slot.UseActivity(player);
        }
    }
}
