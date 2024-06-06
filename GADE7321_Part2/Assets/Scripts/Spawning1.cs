using System.Collections;
using TMPro;
using UnityEngine;

public class Spawning1 : MonoBehaviour
{
    public GameObject puckBlue;
    public GameObject puckRed;
    public GameObject powerPuck;
    public GameManagerEAI gameManager;
    public TextMeshProUGUI playerTurnText;
    public float aiTurnDelay = 1.0f; 

    // Array of Transform references for spawn positions (set in the Unity Inspector)
    public Transform[] spawnPoints;

    private float spawnCooldown = 1.0f;
    private bool isSpawning = false;
    private int columnIndex; // Add this line

    private int currentPlayer = 1;
    private bool isTurnActive = false;

    private void Start()
    {
        // Determine the column index based on position in the spawnPoints array.
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == transform) // Check if this spawn point is the one we're attached to.
            {
                columnIndex = 1;
                break;
            }
        }

        playerTurnText.text = "Player 1's turn";
        StartCoroutine(ManageTurns()); 
    }

    private void OnMouseDown()
    {
        if (currentPlayer == 1 && !isSpawning && !gameManager.IsColumnFull(columnIndex)) 
        {
            StartCoroutine(SpawnColumn(columnIndex)); 
        }
    }

    private IEnumerator ManageTurns()
    {
        while (true)
        {
            // Activate the turn for the current player
            isTurnActive = true;
            
            if (currentPlayer == 2)
            {
                yield return new WaitForSeconds(aiTurnDelay); 

                int randomColumnIndex;
                do
                {
                    randomColumnIndex = Random.Range(0, spawnPoints.Length);
                } while (gameManager.IsColumnFull(randomColumnIndex));

                StartCoroutine(SpawnColumn(randomColumnIndex)); 
            }
            
            // Wait until the turn is complete (puck is spawned and processed)
            yield return new WaitUntil(() => !isTurnActive);

            // Switch to the next player
            currentPlayer = (currentPlayer == 1) ? 2 : 1;

            // Update UI for the next player's turn
            if (currentPlayer == 1)
            {
                playerTurnText.text = "Player 1's turn";
            }
            else
            {
                playerTurnText.text = "Player 2's (AI) turn";
            }

            // Update power-up UI elements (same as before)
        }
    }

    private IEnumerator SpawnColumn(int columnIndex)
    {
        isSpawning = true;

        GameObject puckToSpawn = gameManager.powerPuckActive
            ? powerPuck
            : (gameManager.GetCurrentPlayer() == 1 ? puckBlue : puckRed);
        GameObject newPuck = Instantiate(puckToSpawn, spawnPoints[columnIndex].position, Quaternion.identity); 
        gameManager.AddPuckToBoardStateObjects(columnIndex, newPuck);

        bool wasPowerPuckActive = gameManager.powerPuckActive;
        gameManager.powerPuckActive = false;

        if (gameManager.UpdateBoardState(columnIndex, wasPowerPuckActive))
        {
            gameManager.CheckWinCondition();
            gameManager.SwitchTurns();
        }

        isTurnActive = false; // Mark the turn as complete after spawning

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
}
