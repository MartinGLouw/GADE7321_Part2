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
    private bool Player1Turn = false;
    private bool Player2Turn = false;

    //New variables for board state
    private int[,] boardState;
    private int heightOfBoard = 12;
    private int lengthOfBoard = 12;

    private void Start()
    {
        boardState = new int[lengthOfBoard, heightOfBoard];
    }

    private void OnMouseDown()
    {
        if (!isSpawning && !IsColumnFull(ColumnIndex))
        {
            StartCoroutine(SpawnColumn());
        }
    }

    public IEnumerator SpawnColumn()
    {
        isSpawning = true;
        if (gameManager.turn == false)
        {
            Player2Turn = false;
            Instantiate(puckblue, SpawnPoint.transform.position, Quaternion.identity);
            gameManager.turn = true;
            Player1Turn = true;
        }
        else
        {
            Player1Turn = false;
            Instantiate(puckred, SpawnPoint.transform.position, Quaternion.identity);
            gameManager.turn = false;
            Player2Turn = true;
        }

        bool success = UpdateBoardState(ColumnIndex);

        if (success)
        {
            int currentPlayer = Player1Turn ? 1 : 2;
            if (CheckWinCondition(currentPlayer))
            {
                Debug.Log($"Player {currentPlayer} wins!");
            }
        }

        yield return new WaitForSeconds(spawnCooldown);
        isSpawning = false;
    }

    private bool IsColumnFull(int column)
    {
        
        column -= 1;

        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardState[column, row] == 0)
            {
                return false;
            }
        }

        return true;
    }

    private bool UpdateBoardState(int column)
    {
        
        column -= 1;
        
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardState[column, row] == 0)
            {
                //Update board with current player's value
                if (Player1Turn)
                    boardState[column, row] = 1;
                else
                    boardState[column, row] = 2;

                
                Debug.Log("Updated board state" + (column) + " " + (row));
                return true;
            }
        }

        return false;
    }

    private bool CheckWinCondition(int player)
    {
        for (int i = 0; i < lengthOfBoard; i++)
        {
            for (int j = 0; j < heightOfBoard; j++)
            {
                //Check horizontal cells
                if (i + 3 < lengthOfBoard &&
                    boardState[i, j] == player &&
                    boardState[i + 1, j] == player &&
                    boardState[i + 2, j] == player &&
                    boardState[i + 3, j] == player)
                {
                    return true;
                }

                //Check vertical cells
                if (j + 3 < heightOfBoard &&
                    boardState[i, j] == player &&
                    boardState[i, j + 1] == player &&
                    boardState[i, j + 2] == player &&
                    boardState[i, j + 3] == player)
                {
                    return true;
                }
                //Check diagonal cells (bottom left to top right)
                if (i + 3 < lengthOfBoard && j + 3 < heightOfBoard &&
                    boardState[i, j] == player &&
                    boardState[i + 1, j + 1] == player &&
                    boardState[i + 2, j + 2] == player &&
                    boardState[i + 3, j + 3] == player)
                {
                    return true;
                }

                //Check diagonal cells (top left to bottom right)
                if (i + 3 < lengthOfBoard && j - 3 >= 0 &&
                    boardState[i, j] == player &&
                    boardState[i + 1, j - 1] == player &&
                    boardState[i + 2, j - 2] == player &&
                    boardState[i + 3, j - 3] == player)
                {
                    return true;
                }
            }
        }

       
        return false;
    }
}