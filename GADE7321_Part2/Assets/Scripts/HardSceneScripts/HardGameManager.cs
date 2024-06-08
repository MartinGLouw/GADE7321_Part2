using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HardGameManager : MonoBehaviour
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

    private const int AI_PLAYER = 2;
    private const int HUMAN_PLAYER = 1;
    private const int MAX_DEPTH = 4;
    private const int WIN_LENGTH = 5;

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
        const int CONNECT_LENGTH = 5;

        //Horizontal win
        for (int y = 0; y < heightOfBoard; y++)
        {
            for (int x = 0; x <= lengthOfBoard - CONNECT_LENGTH; x++) //check if space left
            {
                bool win = true;
                for (int i = 0; i < CONNECT_LENGTH; i++) // Check consecutive pieces
                {
                    if (boardState[x + i, y] != player)
                    {
                        win = false;
                        break;
                    }
                }

                if (win) return true; //win
            }
        }

        //Check vertical wins
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y <= heightOfBoard - CONNECT_LENGTH; y++) 
            {
                bool win = true;
                for (int i = 0; i < CONNECT_LENGTH; i++)
                {
                    if (boardState[x, y + i] != player)
                    {
                        win = false;
                        break;
                    }
                }

                if (win) return true;
            }
        }

        //Check diagonal wins (top-left to bottom-right)
        for (int x = 0; x <= lengthOfBoard - CONNECT_LENGTH; x++)
        {
            for (int y = 0; y <= heightOfBoard - CONNECT_LENGTH; y++)
            {
                bool win = true;
                for (int i = 0; i < CONNECT_LENGTH; i++)
                {
                    if (boardState[x + i, y + i] != player)
                    {
                        win = false;
                        break;
                    }
                }

                if (win) return true;
            }
        }

        //Check diagonal wins (bottom-left to top-right)
        for (int x = 0; x <= lengthOfBoard - CONNECT_LENGTH; x++)
        {
            for (int y = CONNECT_LENGTH - 1; y < heightOfBoard; y++)
            {
                bool win = true;
                for (int i = 0; i < CONNECT_LENGTH; i++)
                {
                    if (boardState[x + i, y - i] != player)
                    {
                        win = false;
                        break;
                    }
                }

                if (win) return true;
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

    private int GetRandomAvailableColumn()
    {
        return Minimax(boardState, MAX_DEPTH, AI_PLAYER).Item1; //Get the best column from minimax
    }

    private (int, int) Minimax(int[,] board, int depth, int player)
    {
        if (depth == 0 || CheckWinConditionForPlayer(player) || IsBoardFull(board))
        {
            return (-1, EvaluateBoard(board, AI_PLAYER)); //Return score and fake column for nodes
        }

        int bestScore = (player == AI_PLAYER) ? int.MinValue : int.MaxValue;
        int bestCol = -1;

        for (int col = 0; col < lengthOfBoard; col++)
        {
            if (!IsColumnFull(col))
            {
                int[,] newBoard = (int[,])board.Clone();
                DropPuck(newBoard, col, player);
                int score = Minimax(newBoard, depth - 1, 3 - player).Item2;

                if ((player == AI_PLAYER && score > bestScore) || (player == HUMAN_PLAYER && score < bestScore))
                {
                    bestScore = score;
                    bestCol = col;
                }
            }
        }

        return (bestCol, bestScore);
    }

    private int EvaluateBoard(int[,] board, int player)
    {
        //Enhanced evaluation function for Connect 5
        int opponent = 3 - player;

        //Check for immediate wins/losses
        if (CheckWinConditionForPlayer(player)) return 10000;
        if (CheckWinConditionForPlayer(opponent)) return -10000;

        int score = 0;

        //Center control
        int centerCol = lengthOfBoard / 2;
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (board[centerCol, row] == player) score += 3;
            if (board[centerCol, row] == opponent) score -= 3;
        }

        //Potential winning lines
        score += CountPotentialLines(board, player, WIN_LENGTH) * 1000; // Prioritize 5-in-a-row threats
        score += CountPotentialLines(board, player, 4) * 100;
        score += CountPotentialLines(board, player, 3) * 10;
        score += CountPotentialLines(board, player, 2);

        //Blocking opponent's moves
        score -= CountPotentialLines(board, opponent, WIN_LENGTH) * 900;
        score -= CountPotentialLines(board, opponent, 4) * 90;
        score -= CountPotentialLines(board, opponent, 3) * 9;
        score -= CountPotentialLines(board, opponent, 2);

        return score;
    }

    private void DropPuck(int[,] board, int column, int player)
    {
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (board[column, row] == 0)
            {
                board[column, row] = player;
                break;
            }
        }
    }

    private bool IsBoardFull(int[,] board)
    {
        for (int col = 0; col < lengthOfBoard; col++)
        {
            if (!IsColumnFull(col)) return false;
        }

        return true;
    }

    private int CountPotentialLines(int[,] board, int player, int length)
    {
        int count = 0;
        // Check horizontal, vertical, and diagonal lines
        for (int row = 0; row < heightOfBoard; row++)
        {
            for (int col = 0; col < lengthOfBoard; col++)
            {
                // ... (Check for potential lines of 'length' for the given player)
            }
        }

        return count;
    }
}