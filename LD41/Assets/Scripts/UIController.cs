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
    [SerializeField]
    Text EnemyTurnText;
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

    internal void EnemyTurnNotification(bool v)
    {
        EnemyTurnText.enabled = v;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
