using UnityEngine;

public class ShineEffect : MonoBehaviour
{
    public RectTransform shine;
    public Vector2 startLocalPos;
    public Vector2 endLocalPos;
    public float speed = 100f;
    public float delay = 3f;

    private bool moving = false;

    void Start()
    {
        shine.localPosition = startLocalPos;
        InvokeRepeating(nameof(StartMove), delay, delay + (Vector2.Distance(startLocalPos, endLocalPos) / speed));
    }

    void StartMove()
    {
        shine.localPosition = startLocalPos;
        moving = true;
    }

    void Update()
    {
        if (!moving) return;

        shine.localPosition = Vector2.MoveTowards(shine.localPosition, endLocalPos, speed * Time.deltaTime);

        if ((Vector2)shine.localPosition == endLocalPos)
        {
            moving = false;
        }
    }
}
