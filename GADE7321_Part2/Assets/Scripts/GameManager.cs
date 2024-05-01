using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool turn = true; // Player 1's turn

    private int[,] boardState;
    private int heightOfBoard = 12;
    private int lengthOfBoard = 12;

    // Power-up tracking
    private int[] player1Powerups = { 1, 1, 1 }; 
    private int[] player2Powerups = { 1, 1, 1 }; 

    private bool isPowerUpSelected = false;
    private int selectedPowerup = 0; 
    public int ColumnIndex; // Assuming Spawning sets this when a puck lands

    private void Start()
    {
        boardState = new int[lengthOfBoard, heightOfBoard];
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

    public bool UpdateBoardState(int column)
    {
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardState[column, row] == 0)
            {
                boardState[column, row] = turn ? 1 : 2;
                return true;
            }
        }
        return false;
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
        for (int x = 0; x < lengthOfBoard - 4; x++) // Changed from -3 to -4
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                if (boardState[x, y] == player &&
                    boardState[x + 1, y] == player &&
                    boardState[x + 2, y] == player &&
                    boardState[x + 3, y] == player && 
                    boardState[x + 4, y] == player)  // Check for 5 in a row
                {
                    return true;
                }
            }
        }

        // Check vertical wins
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y < heightOfBoard - 4; y++) // Changed from -3 to -4
            {
                if (boardState[x, y] == player &&
                    boardState[x, y + 1] == player &&
                    boardState[x, y + 2] == player &&
                    boardState[x, y + 3] == player &&
                    boardState[x, y + 4] == player) // Check for 5 in a row
                {
                    return true;
                }
            }
        }

        // Check diagonal wins (top-left to bottom-right)
        for (int x = 0; x < lengthOfBoard - 4; x++) // Changed from -3 to -4
        {
            for (int y = 0; y < heightOfBoard - 4; y++) // Changed from -3 to -4
            {
                if (boardState[x, y] == player &&
                    boardState[x + 1, y + 1] == player &&
                    boardState[x + 2, y + 2] == player &&
                    boardState[x + 3, y + 3] == player &&
                    boardState[x + 4, y + 4] == player) // Check for 5 in a row
                {
                    return true;
                }
            }
        }

        // Check diagonal wins (bottom-left to top-right)
        for (int x = 0; x < lengthOfBoard - 4; x++)  // Changed from -3 to -4
        {
            for (int y = 4; y < heightOfBoard; y++) // Changed from 3 to 4
            {
                if (boardState[x, y] == player &&
                    boardState[x + 1, y - 1] == player &&
                    boardState[x + 2, y - 2] == player &&
                    boardState[x + 3, y - 3] == player &&
                    boardState[x + 4, y - 4] == player) // Check for 5 in a row
                {
                    return true;
                }
            }
        }

        // No win condition found
        return false;
    }public int GetCurrentPlayer()
    {
        return turn ? 1 : 2;
    }

    public void SwitchTurns()
    {
        turn = !turn;
    }

    public void UsePowerup(int powerupIndex) 
    {
        if (!turn || isPowerUpSelected) return;

        int[] currentPlayerPowerups = turn ? player1Powerups : player2Powerups;
        if (currentPlayerPowerups[powerupIndex - 1] > 0)
        {
            isPowerUpSelected = true;
            selectedPowerup = powerupIndex;
            currentPlayerPowerups[powerupIndex - 1]--; 
        }
    }
    public bool GetCurrentPlayerHasPowerups()
    {
        int[] currentPlayerPowerups = turn ? player1Powerups : player2Powerups;
        return currentPlayerPowerups.Any(count => count > 0);
    }

    public void EndTurn()
    {
        if (isPowerUpSelected)
        {
            ExecutePowerup();
        } else
        {
            CheckWinCondition();
            SwitchTurns();
        }
    }

    private void ExecutePowerup()
    {
        switch (selectedPowerup)
        {
            case 1: 
                DeleteHorizontalRow(GetAffectedRow());
                break;
            case 2:
                DeleteVerticalColumn(GetAffectedColumn());
                break;
            case 3:
                // Do nothing, turn is skipped
                break;
            default: 
                Debug.LogError("Invalid power-up selection");
                break;
        }
        isPowerUpSelected = false; 
        selectedPowerup = 0;
        SwitchTurns(); 
    }

    private void DeleteHorizontalRow(int rowToDelete)
    {
        if (rowToDelete >= 0)
        {
            for (int x = 0; x < lengthOfBoard; x++)
            {
                boardState[x, rowToDelete] = 0;
            }

            for (int x = 0; x < lengthOfBoard; x++)
            {
                ShiftColumnDown(x, rowToDelete);
            }
        }
    }

    private void DeleteVerticalColumn(int colToDelete)
    {
        if (colToDelete >= 0)
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                boardState[colToDelete, y] = 0;
            }
        }
    }

    private int GetAffectedColumn() 
    {
        return ColumnIndex; 
    }

    private int GetAffectedRow()
    {
        for (int y = heightOfBoard - 1; y >= 0; y--)
        {
            if (boardState[GetAffectedColumn(), y] == 0)
            {
                return y; 
            }
        }
        return -1; 
    }

    private void ShiftColumnDown(int column, int deletedRow)
    {
        for (int y = deletedRow; y > 0; y--)
        {
            boardState[column, y] = boardState[column, y - 1];
        }
        boardState[column, 0] = 0; 
    }
}