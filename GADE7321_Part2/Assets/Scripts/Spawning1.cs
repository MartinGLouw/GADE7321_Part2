using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.UI;
using System.Linq;
using Random = UnityEngine.Random;

public class Spawning1 : MonoBehaviour
{
    public GameObject Column;
    public int ColumnIndex;
    public GameObject SpawnPoint;
    public GameObject puckblue;
    public GameObject puckred;
    public GameObject PowerPuck;
    public GameManagerEAI gameManager;
    public TextMeshProUGUI playerTurnText;
    private bool isSpawning = false;
    private float spawnCooldown = 1.0f;
    private Transform[] SpawnPoints;

    public void Start()
    {
        //find all objects with same tag
        SpawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint").Select(go => go.transform).ToArray();
    }

    private void Update()
    {
        while (gameManager.turn == false)
        {
            Random.Range(0, 11);
        }
    }

    private void OnMouseDown()
    {
        if (!isSpawning && !gameManager.IsColumnFull(ColumnIndex))
        {
            StartCoroutine(SpawnColumn());
            SwitchTurns();
        }
    }

    public void SwitchTurns()
    {
        // Update the Text element to display whose turn it is
        if (gameManager.turn)
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
        GameObject puckToSpawn = gameManager.powerPuckActive
            ? PowerPuck
            : (gameManager.GetCurrentPlayer() == 1 ? puckblue : puckred);
        GameObject newPuck = Instantiate(puckToSpawn, SpawnPoint.transform.position, Quaternion.identity);
        gameManager.AddPuckToBoardStateObjects(ColumnIndex, newPuck);
        gameManager.AddPuckToBoardStateObjects(ColumnIndex, newPuck);
        bool wasPowerPuckActive = gameManager.powerPuckActive;
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