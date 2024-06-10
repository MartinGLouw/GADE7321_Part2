using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


public class MinMaxSpawning : MonoBehaviour
{
    public GameObject Column;
    public int ColumnIndex;
    public GameObject SpawnPoint;
    public GameObject puckblue;
    public GameObject puckred;
    public GameObject PowerPuck;
    public HardGameManager gameManagerHard;
    public TextMeshProUGUI playerTurnText;
    public GameObject[] spawnPoints; //An array to hold all the spawn points
    public Transform[] spawnPointsPosition;

    private bool isSpawning = false;
    private float spawnCooldown = 1.0f;


    private void Start()
    {
        gameManagerHard.turn = true;
        SwitchTurns();
    }

    private void OnMouseDown()
    {
        if (!isSpawning && !gameManagerHard.IsColumnFull(ColumnIndex) && gameManagerHard.turn)
        {
            StartCoroutine(SpawnColumn(ColumnIndex, SpawnPoint));
        }
    }

    public IEnumerator SpawnColumn(int columnIndex, GameObject spawnPoint = null)
    {
        isSpawning = true;

        GameObject puckToSpawn = gameManagerHard.powerPuckActive
            ? PowerPuck
            : (gameManagerHard.GetCurrentPlayer() == 1 ? puckblue : puckred);

        GameObject newPuck;
        if (gameManagerHard.GetCurrentPlayer() == 1)
        {
            //For Player 1, use the provided spawnPoint
            newPuck = Instantiate(puckToSpawn, spawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            //For AI, use the columnIndex to determine the spawn position from the array
            newPuck = Instantiate(puckToSpawn, spawnPoints[columnIndex].transform.position, Quaternion.identity);
        }

        Debug.Log("Placing puck in column: " + columnIndex);
        gameManagerHard.AddPuckToBoardStateObjects(columnIndex, newPuck);
        bool wasPowerPuckActive = gameManagerHard.powerPuckActive;
        gameManagerHard.powerPuckActive = false;

        if (gameManagerHard.UpdateBoardState(columnIndex, wasPowerPuckActive))
        {
            gameManagerHard.CheckWinCondition();
        }

        StartCoroutine(DelayThenSwitchTurns());

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }

    private IEnumerator DelayThenSwitchTurns()
    {
        yield return new WaitForSeconds(1f);
        gameManagerHard.SwitchTurns();
        SwitchTurns();

        if (!gameManagerHard.turn) //Check if it's AI turn
        {
            int enemyColumnIndex = gameManagerHard.GetRandomAvailableColumn();
            GameObject enemySpawnPoint = spawnPointsPosition[enemyColumnIndex].gameObject;
            Debug.Log("AI placing puck in column: " + enemyColumnIndex);
            StartCoroutine(SpawnColumn(enemyColumnIndex, enemySpawnPoint));
        }
    }


    public void SwitchTurns()
    {
        if (gameManagerHard.turn)
        {
            playerTurnText.text = "Player 1's turn";
        }
        else
        {
            playerTurnText.text = "Player 2's turn";
        }
    }
}