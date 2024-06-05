using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.UI;

public class Spawning1 : MonoBehaviour
{
    public GameObject Column;
    public int ColumnIndex;
    public GameObject SpawnPoint;
    public GameObject puckblue;
    public GameObject puckred;
    public GameObject PowerPuck;
    public GameManagerEAI gameManagerEAI;
    public TextMeshProUGUI playerTurnText;
    private bool isSpawning = false;
    private float spawnCooldown = 1.0f;
    public void Start()
    {
        playerTurnText.text = "Player 1's turn";
    }
    private void OnMouseDown()
    {
        if (!isSpawning && !gameManagerEAI.IsColumnFull(ColumnIndex))
        {
            StartCoroutine(SpawnColumn());
            SwitchTurns();
        }
    }
    public void SwitchTurns()
    {
        // Update the Text element to display whose turn it is
        if (gameManagerEAI.turn)
        {
            playerTurnText.text = "Player 1's turn";
        }
        else
        {
            playerTurnText.text = "Player 2's turn";
        }
    }
    public IEnumerator SpawnColumn()
    {
        isSpawning = true;
        GameObject puckToSpawn = gameManagerEAI.powerPuckActive
            ? PowerPuck
            : (gameManagerEAI.GetCurrentPlayer() == 1 ? puckblue : puckred);
        GameObject newPuck = Instantiate(puckToSpawn, SpawnPoint.transform.position, Quaternion.identity);
        gameManagerEAI.AddPuckToBoardStateObjects(ColumnIndex, newPuck);
        gameManagerEAI.AddPuckToBoardStateObjects(ColumnIndex, newPuck);
        bool wasPowerPuckActive = gameManagerEAI.powerPuckActive;
        gameManagerEAI.powerPuckActive = false;
        if (gameManagerEAI.UpdateBoardState(ColumnIndex, wasPowerPuckActive))
        {
            gameManagerEAI.CheckWinCondition();
            gameManagerEAI.SwitchTurns();
        }
        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
}