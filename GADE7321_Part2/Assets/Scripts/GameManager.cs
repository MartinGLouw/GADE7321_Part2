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
    private bool player1PowerPuckUsed = false; // Track if Player 1 has used their PowerPuck
    private bool player2PowerPuckUsed = false; // Track if Player 2 has used their PowerPuck
    public bool powerPuckActive = false; // Power-up to spawn PowerPuck
    private int[,] boardState;
    private int heightOfBoard = 12;
    private int lengthOfBoard = 12;
    private GameObject[,] boardStateObjects; // To keep track of the GameObject instances
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (turn == true && player1PowerPuckUsed == false)
                {
                    powerPuckActive = true;
                    player1PowerPuckUsed = true;
                    Debug.Log("Player 1 activated PowerPuck power-up");
                }
                else if (turn == false && player2PowerPuckUsed == false)
                {
                    powerPuckActive = true;
                    player2PowerPuckUsed = true;
                    Debug.Log("Player 2 activated PowerPuck power-up");
                }
            }
            //if 3 is pressed
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (turn == true && player1SkipUsed == false)
                {
                    turn = false;
                    player1SkipUsed = true;
                    Debug.Log("Player 1 has skipped their turn");
                }
                //If it's Player 2's turn and they haven't used their skip yet
                else if (turn == false && player2SkipUsed == false)
                {
                    turn = true;
                    player2SkipUsed = true;
                    Debug.Log("Player 2 has skipped their turn");
                }
            }
        }
        private void ClearRow(int row)
        {
            Debug.Log($"Clearing row {row}");
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
                    Debug.Log($"Added puck to boardStateObjects at column {column}, row {row}");
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
        Debug.Log(powerPuckActive);
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardState[column, row] == 0)
            {
                Debug.Log($"Updating board state at column {column}, row {row}");

                Debug.Log($"powerPuckActive: {powerPuckActive}"); 

                if (wasPowerPuckActive)
                {
                    Debug.Log("PowerPuck spawned and clearing row!"); 
                    boardState[column, row] = 3; 
                    ClearRow(row);
                    powerPuckActive = false;
                    return true;
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
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                if (boardState[x, y] == player &&
                    boardState[x + 1, y] == player &&
                    boardState[x + 2, y] == player &&
                    boardState[x + 3, y] == player)
                {
                    return true;
                }
            }
        }

        // Check vertical wins
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
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

        // Check diagonal wins (top-left to bottom-right)
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
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

        // Check diagonal wins (bottom-left to top-right)
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 3; y < heightOfBoard; y++)
            {
                if (boardState[x, y] == player &&
                    boardState[x + 1, y - 1] == player &&
                    boardState[x + 2, y - 2] == player &&
                    boardState[x + 3, y - 3] == player)
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