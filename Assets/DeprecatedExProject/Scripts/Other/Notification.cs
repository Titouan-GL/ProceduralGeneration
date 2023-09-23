using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    private List<int> notificationsLeft = new List<int>();

    private float timeDisplaying = 6;
    private float transitionTime = 0.5f;
    private float lerpSpeed = 0.1f;

    private float currentTimeDisplaying = 0f;

    [SerializeField] private RectTransform notif1;
    [SerializeField] private RectTransform notif2;

    [SerializeField] private RectTransform PositionShown;

    [SerializeField] private RectTransform PositionHidden;

    private void Update()
    {
        if (Input.GetButtonDown("Notif1"))
        {
            notificationsLeft.Add(1);
        }
        if (Input.GetButtonDown("Notif2"))
        {
            notificationsLeft.Add(2);
        }
    }

    void FixedUpdate()
    {
        if (notificationsLeft.Count > 0)
        {
            if (notificationsLeft[0] == 1)
            {
                MoveMessage(notif1);
            }
            if (notificationsLeft[0] == 2)
            {
                MoveMessage(notif2);
            }
        }
    }

    void MoveMessage(RectTransform notif)
    {
        if (currentTimeDisplaying == 0)
        {
            notif.anchoredPosition = PositionHidden.anchoredPosition;
        }
        else if (currentTimeDisplaying < timeDisplaying - transitionTime)
        {
            notif.anchoredPosition = Vector2.Lerp(notif.anchoredPosition, PositionShown.anchoredPosition, lerpSpeed);
        }
        else if (currentTimeDisplaying < timeDisplaying)
        {
            notif.anchoredPosition = Vector2.Lerp(notif.anchoredPosition, PositionHidden.anchoredPosition, lerpSpeed);
        }
        else
        {
            notif.anchoredPosition = PositionHidden.anchoredPosition;
            currentTimeDisplaying = 0;
            notificationsLeft.RemoveAt(0);
        }
        currentTimeDisplaying += Time.fixedDeltaTime;
    }
}
