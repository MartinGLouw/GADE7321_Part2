using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameManagerEAI : MonoBehaviour
{
    public bool turn = true;
    public bool skipTurnPowerUp = false;
    private bool player1SkipUsed = false;
    private bool player2SkipUsed = false;

    public bool powerPuckActive = false;
    private int[,] boardState;
    private int heightOfBoard = 12;
    public int lengthOfBoard = 12;
    public bool clearColumnPowerUp = false;
    private bool player1PowerPuckUsed = false;
    private bool player2PowerPuckUsed = false;
    private bool player1ClearColumnUsed = false;
    private bool player2ClearColumnUsed = false;
    private GameObject[,] boardStateObjects;
    public TextMeshProUGUI WinText;
    public GameObject WinImage;
    public GameObject P1DH;
    public GameObject P2DH;
    public GameObject P1DC;
    public GameObject P2DC;
    public GameObject P1S;
    public GameObject P2S;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (turn == true && player1PowerPuckUsed == false)
            {
                P1DH.SetActive(false);
                powerPuckActive = true;
                player1PowerPuckUsed = true;
            }
            else if (turn == false && player2PowerPuckUsed == false)
            {
                P2DH.SetActive(false);
                powerPuckActive = true;
                player2PowerPuckUsed = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (turn == true && player1SkipUsed == false)
            {
                P1S.SetActive(false);
                turn = false;
                player1SkipUsed = true;
            }

            else if (turn == false && player2SkipUsed == false)
            {
                P2S.SetActive(false);
                turn = true;
                player2SkipUsed = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (turn == true && player1ClearColumnUsed == false)
            {
                P1DC.SetActive(false);
                clearColumnPowerUp = true;
                player1ClearColumnUsed = true;
            }
            else if (turn == false && player2ClearColumnUsed == false)
            {
                P2DC.SetActive(false);
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
                if (wasPowerPuckActive)
                {
                    boardState[column, row] = 3;
                    ClearRow(row);
                    powerPuckActive = false;
                    return true;
                }
                else if (clearColumnPowerUp)
                {
                    ClearColumn(column);
                    clearColumnPowerUp = false;
                    return false;
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
            Destroy(boardStateObjects[column, i]);

            boardStateObjects[column, i] = null;

            boardState[column, i] = 0;
        }
    }

    public void CheckWinCondition()
    {
        int currentPlayer = turn ? 1 : 2;
        if (CheckWinConditionForPlayer(currentPlayer))
        {
            WinImage.SetActive(true);
            WinText.text = $"Player {currentPlayer} wins!";
            Debug.Log($"Player {currentPlayer} wins!");
        }
    }

    private bool CheckWinConditionForPlayer(int player)
    {
        //Check horizontal wins
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

        //Check vertical wins
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

        //Check diagonal wins (top-left to bottom-right)
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

        //Check diagonal wins (bottom-left to top-right)
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

        //No win condition found
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