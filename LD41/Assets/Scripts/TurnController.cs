using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnController : Singleton<TurnController>
{
    List<EnemyScript> enemies;
    PlayerScript player;
    void Start()
    {
        enemies = new List<EnemyScript>();
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        StartCoroutine(ManageTurns());

    }

    public void RegisterEnemy(EnemyScript enemy)
    {
        enemies.Add(enemy);
    }

    IEnumerator ManageTurns()
    {
        while (true)
        {
           
            yield return StartCoroutine(player.ExecuteTurn());
            //Debug.Log(enemies.Count);
            foreach (var enemy in enemies)
            {
                yield return StartCoroutine(enemy.ExecuteTurn());
            }
            if (enemies.Count == 0)
            {
                LoadNextLevel();
                break;
            }
        }
    }

    private void LateUpdate()
    {
        if (enemies.Count == 0)
        {
            UIController.Instance.EnableEndLevelText();
        }
    }

    private void LoadNextLevel()
    {

        if (!(SceneManager.sceneCountInBuildSettings == SceneManager.GetActiveScene().buildIndex + 1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    internal void DeregisterEnemy(EnemyScript enemyScript)
    {
        enemies.Remove(enemyScript);
    }
}
