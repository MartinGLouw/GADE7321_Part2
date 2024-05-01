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
    public GameObject powerupButton1; 
    public GameObject powerupButton2;
    public GameObject powerupButton3;

    
    private bool isSelectingPowerup = false; // Flag for power-up selection 
    private int powerupIndexSelected = 0;
    private bool isSpawning = false;
    private float spawnCooldown = 1.0f;

    private void OnMouseDown()
    {
        if (isSelectingPowerup) return; // Don't process clicks during power-up selection

        if (!isSpawning && !gameManager.IsColumnFull(ColumnIndex))
        {
            if (gameManager.GetCurrentPlayerHasPowerups())
            {
                ShowPowerupSelectionUI(); 
            }
            else
            {
                StartCoroutine(SpawnColumn());
            }
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
            gameManager.EndTurn(); // End the turn
        }

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }
    private void ShowPowerupSelectionUI()
    {
        powerupButton1.SetActive(true);
        powerupButton2.SetActive(true);
        powerupButton3.SetActive(true);
        isSelectingPowerup = true;
    }

    // Call these functions from your power-up button's OnClick event
    public void SelectPowerup1() 
    {
        powerupIndexSelected = 1;
        UseSelectedPowerup();
    }

    public void SelectPowerup2()
    {
        powerupIndexSelected = 2;
        UseSelectedPowerup();
    }

    public void SelectPowerup3()
    {
        powerupIndexSelected = 3;
        UseSelectedPowerup();
    }

    private void UseSelectedPowerup()
    {
        HidePowerupSelectionUI(); 
        gameManager.UsePowerup(powerupIndexSelected);
        powerupIndexSelected = 0;
    }

    private void HidePowerupSelectionUI()
    {
        powerupButton1.SetActive(false);
        powerupButton2.SetActive(false);
        powerupButton3.SetActive(false);
        isSelectingPowerup = false;
    }
}
