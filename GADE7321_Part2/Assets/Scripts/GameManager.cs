using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool turn = true; // Player 1's turn
    public bool skipTurnPowerUp = false;
    private bool player1SkipUsed = false; // Track if Player 1 has used their skip
    private bool player2SkipUsed = false; // Track if Player 2 has used their skip

    public bool powerPuckActive = false; // Power-up to spawn PowerPuck
    private int[,] boardState;
    private int heightOfBoard = 12;
    private int lengthOfBoard = 12;
    public bool clearColumnPowerUp = false; // Power-up to clear a column
    private bool player1PowerPuckUsed = false; // Track if Player 1 has used their PowerPuck
    private bool player2PowerPuckUsed = false; // Track if Player 2 has used their PowerPuck
    private bool player1ClearColumnUsed = false; // Track if Player 1 has used their Clear Column power-up
    private bool player2ClearColumnUsed = false; // Track if Player 2 has used their Clear Column power-up
    private GameObject[,] boardStateObjects; // To keep track of the GameObject instances

    public void Update()
    {
        // If 1 is pressed and the current player hasn't used their PowerPuck yet, activate PowerPuck power-up
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (turn == true && player1PowerPuckUsed == false)
            {
                powerPuckActive = true;
                player1PowerPuckUsed = true;
            }
            else if (turn == false && player2PowerPuckUsed == false)
            {
                powerPuckActive = true;
                player2PowerPuckUsed = true;
            }
        }

        //if 3 is pressed
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (turn == true && player1SkipUsed == false)
            {
                turn = false;
                player1SkipUsed = true;
            }
            //If it's Player 2's turn and they haven't used their skip yet
            else if (turn == false && player2SkipUsed == false)
            {
                turn = true;
                player2SkipUsed = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (turn == true && player1ClearColumnUsed == false)
            {
                clearColumnPowerUp = true;
                player1ClearColumnUsed = true;
            }
            else if (turn == false && player2ClearColumnUsed == false)
            {
                clearColumnPowerUp = true;
                player2ClearColumnUsed = true;
            }
        }
    }

    private void ClearRow(int row)
    {
        for (int i = 0; i < lengthOfBoard; i++)
        {
            Destroy(boardStateObjects[i, row]);

            boardStateObjects[i, row] = null;

            boardState[i, row] = 0;
        }
    }

    private void Start()
    {
        boardState = new int[lengthOfBoard, heightOfBoard];
        boardStateObjects = new GameObject[lengthOfBoard, heightOfBoard];
    }

    public void AddPuckToBoardStateObjects(int column, GameObject puck)
    {
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardStateObjects[column, row] == null)
            {
                boardStateObjects[column, row] = puck;
                break;
            }
        }
    }


    public bool IsColumnFull(int column)
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

    public bool UpdateBoardState(int column, bool wasPowerPuckActive)
    {
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardState[column, row] == 0)
            {
                // If PowerPuck power-up is active, spawn PowerPuck and clear the row
                if (wasPowerPuckActive)
                {
                    boardState[column, row] = 3; // Assuming 3 represents a PowerPuck
                    ClearRow(row);
                    powerPuckActive = false;
                    return true;
                }
                // If Clear Column power-up is active, clear the column
                else if (clearColumnPowerUp)
                {
                    ClearColumn(column);
                    clearColumnPowerUp = false;
                    return false; // Return false because no puck is placed
                }
                else
                {
                    boardState[column, row] = turn ? 1 : 2;
                    return true;
                }
            }
        }

        return false;
    }

    private void ClearColumn(int column)
    {
        for (int i = 0; i < heightOfBoard; i++)
        {
            // Destroy the GameObject in the scene
            Destroy(boardStateObjects[column, i]);
            // Clear the reference in the array
            boardStateObjects[column, i] = null;
            // Clear the board state
            boardState[column, i] = 0; // Assuming 0 represents an empty space
        }
    }

    public void CheckWinCondition()
    {
        int currentPlayer = turn ? 1 : 2;
        if (CheckWinConditionForPlayer(currentPlayer))
        {
            Debug.Log($"Player {currentPlayer} wins!");
        }
    }

    private bool CheckWinConditionForPlayer(int player)
{
    // Check horizontal wins
    for (int x = 0; x < lengthOfBoard - 4; x++)
    {
        for (int y = 0; y < heightOfBoard; y++)
        {
            if (boardState[x, y] == player &&
                boardState[x + 1, y] == player &&
                boardState[x + 2, y] == player &&
                boardState[x + 3, y] == player &&
                boardState[x + 4, y] == player)
            {
                return true;
            }
        }
    }

    // Check vertical wins
    for (int x = 0; x < lengthOfBoard; x++)
    {
        for (int y = 0; y < heightOfBoard - 4; y++)
        {
            if (boardState[x, y] == player &&
                boardState[x, y + 1] == player &&
                boardState[x, y + 2] == player &&
                boardState[x, y + 3] == player &&
                boardState[x, y + 4] == player)
            {
                return true;
            }
        }
    }

    // Check diagonal wins (top-left to bottom-right)
    for (int x = 0; x < lengthOfBoard - 4; x++)
    {
        for (int y = 0; y < heightOfBoard - 4; y++)
        {
            if (boardState[x, y] == player &&
                boardState[x + 1, y + 1] == player &&
                boardState[x + 2, y + 2] == player &&
                boardState[x + 3, y + 3] == player &&
                boardState[x + 4, y + 4] == player)
            {
                return true;
            }
        }
    }

    // Check diagonal wins (bottom-left to top-right)
    for (int x = 0; x < lengthOfBoard - 4; x++)
    {
        for (int y = 4; y < heightOfBoard; y++)
        {
            if (boardState[x, y] == player &&
                boardState[x + 1, y - 1] == player &&
                boardState[x + 2, y - 2] == player &&
                boardState[x + 3, y - 3] == player &&
                boardState[x + 4, y - 4] == player)
            {
                return true;
            }
        }
    }

    // No win condition found
    return false;
}


    public int GetCurrentPlayer()
    {
        return turn ? 1 : 2;
    }

    public void SwitchTurns()
    {
        turn = !turn;
    }
}