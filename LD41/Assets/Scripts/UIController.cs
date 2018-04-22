using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : Singleton<UIController>
{
    [SerializeField]
    Text MovementCostText;
    [SerializeField]
    Text RemainingMovementPointsText;
    [SerializeField]
    Text HitpointText;
    [SerializeField]
    Text EndLevelText;
    public void SetMovementCostTextValue(int cost)
    {
        MovementCostText.text = cost.ToString();
    }

    public void SetRemainingMovmentPointsTextValue(int points)
    {
        RemainingMovementPointsText.text = points.ToString();
    }

    internal void SetPlayerHealthTextValue(int playerHealth)
    {
        HitpointText.text = playerHealth.ToString();
    }

    internal void EnableEndLevelText()
    {
        EndLevelText.enabled = true;
    }
}
