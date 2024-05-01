using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawning : MonoBehaviour
{
    public GameObject Column;
    public int ColumnIndex;
    public GameObject SpawnPoint;
    public GameObject puckblue;
    public GameObject puckred;
    public GameManager gameManager;

    private bool isSpawning = false;
    private float spawnCooldown = 1.0f;

    private void OnMouseDown()
    {
        if (!isSpawning && !gameManager.IsColumnFull(ColumnIndex)) 
        {
            StartCoroutine(SpawnColumn());
        }
    }

    public IEnumerator SpawnColumn()
    {
        isSpawning = true;

        GameObject puckToSpawn = gameManager.GetCurrentPlayer() == 1 ? puckblue : puckred;
        Instantiate(puckToSpawn, SpawnPoint.transform.position, Quaternion.identity);

        if (gameManager.UpdateBoardState(ColumnIndex))
        {
            gameManager.CheckWinCondition();
            gameManager.SwitchTurns();
        }

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
}