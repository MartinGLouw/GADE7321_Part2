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
    private int lengthOfBoard = 13;

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

        if (!success)
        {
            Debug.Log($"Column {ColumnIndex} is full! Cannot spawn more pucks.");
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
        for (int row = heightOfBoard - 1; row >= 0; row--)
        {
            if (boardState[column, row] == 0)
            {
                //Update board with current player's value
                if (Player1Turn)
                    boardState[column, row] = 1;
                else
                    boardState[column, row] = 2;

                return true;
            }
        }

        return false;
    }
}