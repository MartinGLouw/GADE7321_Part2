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
        spawnPointsPosition = GameObject.FindGameObjectsWithTag("Spawnpoint").Select(go => go.transform).ToArray();
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
        
        GameObject newPuck = Instantiate(puckToSpawn, spawnPoint ? spawnPoint.transform.position : GetRandomSpawnPoint().position, Quaternion.identity); 
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
        yield return new WaitForSeconds(1.0f); //Wait for 1 second
        gameManagerHard.SwitchTurns();
        SwitchTurns();
        if (gameManagerHard.GetCurrentPlayer() == 2)
        {
            StartCoroutine(SpawnColumn(GetRandomAvailableColumn())); //Don't pass a spawn point for the AI
        }
    }

    private int GetRandomAvailableColumn()
    {
        int[] availableColumns = new int[gameManagerHard.lengthOfBoard];
        int availableCount = 0;

        for (int i = 0; i < gameManagerHard.lengthOfBoard; i++)
        {
            if (!gameManagerHard.IsColumnFull(i))
            {
                availableColumns[availableCount] = i;
                availableCount++;
            }
        }

        if (availableCount > 0)
        {
            return availableColumns[Random.Range(0, availableCount)];
        }
        else
        {
            return 0; 
        }
    }
    private Transform GetRandomSpawnPoint()
    {
        return spawnPointsPosition[Random.Range(0, spawnPointsPosition.Length)];
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
