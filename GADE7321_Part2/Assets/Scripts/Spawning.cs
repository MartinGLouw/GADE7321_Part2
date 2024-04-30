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
        //Check horizontal cells
        
        for (int x = 0; x < lengthOfBoard - 6; x++)
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                if (boardState[x, y] == player &&
                    boardState[x + 1, y] == player &&
                    boardState[x + 2, y] == player &&
                    boardState[x + 3, y] == player)
                {Debug.Log("win");
                    
                    return true;
                    
                }

            }
        }

        //vertical check

        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y < heightOfBoard-6; y++)
            {
                if (boardState[x, y] == player &&
                    boardState[x, y + 1] == player &&
                    boardState[x, y + 2] == player &&
                    boardState[x, y + 3] == player)
                {
                    return true;
                }
            }


        }
        //Diagonal check
        for (int x = 0; x < lengthOfBoard - 6; x++)
        {
            for (int y = 0; y < heightOfBoard - 6; y++)
            {
                if (boardState[x, y] == player &&
                    boardState[x + 1, y + 1] == player &&
                    boardState[x + 2, y + 2] == player &&
                    boardState[x + 3, y + 3] == player)
                {
                    return true;
                }
            }


        }
        //Diagonal check
        for (int x = 0; x < lengthOfBoard - 6; x++)
        {
            for (int y = 0; y < heightOfBoard - 6; y++)
            {
                if (boardState[x, y + 3] == player &&
                    boardState[x + 1, y + 2] == player &&
                    boardState[x + 2, y + 1] == player &&
                    boardState[x + 3, y] == player)
                {
                    return true;
                }
            }


        }
        return false;
    }
}
    



    


