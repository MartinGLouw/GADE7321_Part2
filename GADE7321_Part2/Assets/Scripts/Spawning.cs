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
    public GameObject PowerPuck;
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

        // If power-up is active, spawn PowerPuck; otherwise, spawn the current player's puck
        GameObject puckToSpawn = gameManager.powerPuckActive ? PowerPuck : (gameManager.GetCurrentPlayer() == 1 ? puckblue : puckred);
        GameObject newPuck = Instantiate(puckToSpawn, SpawnPoint.transform.position, Quaternion.identity);

        // Add the new puck to the gameManager's boardStateObjects
        gameManager.AddPuckToBoardStateObjects(ColumnIndex, newPuck);
        bool wasPowerPuckActive = gameManager.powerPuckActive;
        // Deactivate the power-up after the PowerPuck is spawned
        gameManager.powerPuckActive = false;


        if (gameManager.UpdateBoardState(ColumnIndex, wasPowerPuckActive)) 
        {
            gameManager.CheckWinCondition();
            gameManager.SwitchTurns();
        }
        

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
}