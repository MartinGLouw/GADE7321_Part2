using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;

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
    public GameObject[] spawnPoints; //An array to hold all the spawn points
    public Transform[] spawnPointsPosition;

    private bool isSpawning = false;
    private float spawnCooldown = 1.0f;


    private void Start()
    {
        spawnPointsPosition = GameObject.FindGameObjectsWithTag("Spawnpoint").Select(go => go.transform).ToArray();
        gameManager.turn = true; 
        SwitchTurns(); 
    }

    private void OnMouseDown()
    {
        if (!isSpawning && !gameManager.IsColumnFull(ColumnIndex) && gameManager.turn) 
        {
            StartCoroutine(SpawnColumn(ColumnIndex, SpawnPoint)); 
        }
    }
    
    public IEnumerator SpawnColumn(int columnIndex, GameObject spawnPoint = null)
    {
        isSpawning = true;
        GameObject puckToSpawn = gameManager.powerPuckActive
            ? PowerPuck
            : (gameManager.GetCurrentPlayer() == 1 ? puckblue : puckred);
        
        GameObject newPuck = Instantiate(puckToSpawn, spawnPoint ? spawnPoint.transform.position : GetRandomSpawnPoint().position, Quaternion.identity); 
        gameManager.AddPuckToBoardStateObjects(columnIndex, newPuck);
        bool wasPowerPuckActive = gameManager.powerPuckActive;
        gameManager.powerPuckActive = false;

        if (gameManager.UpdateBoardState(columnIndex, wasPowerPuckActive))
        {
            gameManager.CheckWinCondition();
        }

        StartCoroutine(DelayThenSwitchTurns()); 

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }

    private IEnumerator DelayThenSwitchTurns()
    {
        yield return new WaitForSeconds(1.0f); 
        gameManager.SwitchTurns();
        SwitchTurns();
        if (gameManager.GetCurrentPlayer() == 2)
        {
            StartCoroutine(SpawnColumn(GetRandomAvailableColumn())); 
        }
    }

    private int GetRandomAvailableColumn()
    {
        int[] availableColumns = new int[gameManager.lengthOfBoard];
        int availableCount = 0;

        for (int i = 0; i < gameManager.lengthOfBoard; i++)
        {
            if (!gameManager.IsColumnFull(i))
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
        
        if (gameManager.turn)
        {
            playerTurnText.text = "Player 1's turn";
        }
        else
        {
            playerTurnText.text = "Player 2's turn";
        }
    }
}

