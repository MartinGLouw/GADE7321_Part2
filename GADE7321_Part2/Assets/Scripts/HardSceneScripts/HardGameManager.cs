using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


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
        // Powerup activation input
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
            if (boardState[column, row] == 0) //Check if the cell is empty
            {
                if (wasPowerPuckActive)
                {
                    boardState[column, row] = 3; //Power puck
                    ClearRow(row);
                    powerPuckActive = false;
                    return true;
                }
                else if (clearColumnPowerUp)
                {
                    ClearColumn(column);
                    clearColumnPowerUp = false;
                    return false; //Don't switch turns after clearing a column
                }
                else
                {
                    boardState[column, row] = turn ? 1 : 2; // Place the puck
                    return true; //Puck placed successfully
                }
            }
        }

        DebugBoardState("Current Board:");
        return false; //Column is full
    }

    private void DebugBoardState(string message = "")
    {
        Debug.Log(message);

        string boardString = "";
        for (int row = heightOfBoard - 1; row >= 0; row--)
        {
            for (int column = 0; column < lengthOfBoard; column++)
            {
                boardString += boardState[column, row] + " ";
            }

            boardString += "\n";
        }

        Debug.Log(boardString);
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
            for (int x = 0; x <= lengthOfBoard - CONNECT_LENGTH; x++)
            {
                if (CheckLine(x, y, 1, 0, CONNECT_LENGTH, player)) //Check horizontal line
                    return true;
            }
        }

        //Vertical win
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y <= heightOfBoard - CONNECT_LENGTH; y++)
            {
                if (CheckLine(x, y, 0, 1, CONNECT_LENGTH, player)) //Check vertical line
                    return true;
            }
        }

        //Diagonal win (top-left to bottom-right)
        for (int x = 0; x <= lengthOfBoard - CONNECT_LENGTH; x++)
        {
            for (int y = 0; y <= heightOfBoard - CONNECT_LENGTH; y++)
            {
                if (CheckLine(x, y, 1, 1, CONNECT_LENGTH, player)) //Check diagonal line
                    return true;
            }
        }

        //Diagonal win (bottom-left to top-right)
        for (int x = 0; x <= lengthOfBoard - CONNECT_LENGTH; x++)
        {
            for (int y = CONNECT_LENGTH - 1; y < heightOfBoard; y++)
            {
                if (CheckLine(x, y, 1, -1, CONNECT_LENGTH, player)) // Check diagonal line
                    return true;
            }
        }

        //No win condition found
        return false;
    }

    private bool CheckLine(int x, int y, int dx, int dy, int length, int player)
    {
        for (int i = 0; i < length; i++)
        {
            if (x + i * dx < 0 || x + i * dx >= lengthOfBoard || y + i * dy < 0 || y + i * dy >= heightOfBoard ||
                boardState[x + i * dx, y + i * dy] != player)
            {
                return false;
            }
        }

        return true;
    }


    public int GetCurrentPlayer()
    {
        return turn ? 1 : 2;
    }

    public void SwitchTurns()
    {
        turn = !turn;
    }

//Minimax algorithm 
    public int GetRandomAvailableColumn()
    {
        return Minimax(boardState, MAX_DEPTH, AI_PLAYER).Item1; //Get the best column from minimax
    }

    private (int, int) Minimax(int[,] board, int depth, int player)
    {
        if (depth == 0 || CheckWinConditionForPlayer(player) || IsBoardFull(board))
        {
            return (-1, EvaluateBoard(board, AI_PLAYER)); //Return score and dummy column for leaf nodes
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
        int opponent = 3 - player;

        //Check for immediate wins/losses
        if (CheckWinConditionForPlayer(player)) return 1000000;
        if (CheckWinConditionForPlayer(opponent)) return -1000000;

        int score = 0;

        //Center control
        int centerCol = lengthOfBoard / 2;
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (board[centerCol, row] == player) score += 3;
            if (board[centerCol, row] == opponent) score -= 3;
        }

        //Potential winning lines (adjusted for Connect 5)
        score += CountPotentialLines(board, player, WIN_LENGTH, true) * 10000;
        score += CountPotentialLines(board, player, WIN_LENGTH - 1, true) * 1000;
        score += CountPotentialLines(board, player, WIN_LENGTH - 1, false) * 500;
        score += CountPotentialLines(board, player, 3, true) * 50;
        score += CountPotentialLines(board, player, 2, true) * 10;

        //Blocking opponent's moves (adjusted for Connect 5)
        score -= CountPotentialLines(board, opponent, WIN_LENGTH) * 900;
        score -= CountPotentialLines(board, opponent, WIN_LENGTH - 1, true) * 900;
        score -= CountPotentialLines(board, opponent, WIN_LENGTH - 1, false) * 450;
        score -= CountPotentialLines(board, opponent, 3, true) * 45;
        score -= CountPotentialLines(board, opponent, 2, true) * 9;

        return score;
    }

    private int CountPotentialLines(int[,] board, int player, int length, bool isOpenEnded = false)
    {
        int count = 0;
        int opponent = 3 - player;

        //Check horizontal lines
        for (int row = 0; row < heightOfBoard; row++)
        {
            for (int col = 0; col <= lengthOfBoard - length; col++)
            {
                if (IsLineForPlayer(board, row, col, 0, 1, length, player) &&
                    (!isOpenEnded || IsLineOpenEnded(board, row, col, 0, 1, length)))
                {
                    count++;
                }
            }
        }

        //Check vertical lines
        for (int row = 0; row <= heightOfBoard - length; row++)
        {
            for (int col = 0; col < lengthOfBoard; col++)
            {
                if (IsLineForPlayer(board, row, col, 1, 0, length, player) &&
                    (!isOpenEnded || IsLineOpenEnded(board, row, col, 1, 0, length)))
                {
                    count++;
                }
            }
        }

        //Check diagonal lines (top-left to bottom-right)
        for (int row = 0; row <= heightOfBoard - length; row++)
        {
            for (int col = 0; col <= lengthOfBoard - length; col++)
            {
                if (IsLineForPlayer(board, row, col, 1, 1, length, player) &&
                    (!isOpenEnded || IsLineOpenEnded(board, row, col, 1, 1, length)))
                {
                    count++;
                }
            }
        }

        //Check diagonal lines (top-right to bottom-left)
        for (int row = 0; row <= heightOfBoard - length; row++)
        {
            for (int col = length - 1; col < lengthOfBoard; col++)
            {
                if (IsLineForPlayer(board, row, col, 1, -1, length, player) &&
                    (!isOpenEnded || IsLineOpenEnded(board, row, col, 1, -1, length)))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool IsLineForPlayer(int[,] board, int row, int col, int rowIncrement, int colIncrement, int length,
        int player)
    {
        for (int i = 0; i < length; i++)
        {
            int r = row + i * rowIncrement;
            int c = col + i * colIncrement;
            if (r < 0 || r >= heightOfBoard || c < 0 || c >= lengthOfBoard || board[c, r] != player)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsLineOpenEnded(int[,] board, int startRow, int startCol, int rowIncrement, int colIncrement,
        int length)
    {
        int beforeRow = startRow - rowIncrement;
        int beforeCol = startCol - colIncrement;
        int afterRow = startRow + length * rowIncrement;
        int afterCol = startCol + length * colIncrement;

        bool beforeValid = (beforeRow >= 0 && beforeRow < heightOfBoard && beforeCol >= 0 &&
                            beforeCol < lengthOfBoard && board[beforeCol, beforeRow] == 0);
        bool afterValid = (afterRow >= 0 && afterRow < heightOfBoard && afterCol >= 0 && afterCol < lengthOfBoard &&
                           board[afterCol, afterRow] == 0);
        return beforeValid || afterValid;
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
}