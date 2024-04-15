using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // Add a 2D array to represent the grid
    private GameObject[,] grid = new GameObject[12, 12];
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSpawning)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Clicked on " + hit.collider.name);
                string name = hit.collider.name;
                if (name == "Column")
                {
                    ColumnIndex = 0;
                }
                else if (name.StartsWith("Column ("))
                {
                    string indexStr = name.Substring(8, name.Length - 9); // Extract the number between the parentheses
                    ColumnIndex = int.Parse(indexStr);
                }
                StartCoroutine(SpawnColumn());
            }
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked" + ColumnIndex);
        if (!isSpawning) 
        {
            StartCoroutine(SpawnColumn());
        }
    }

    public IEnumerator SpawnColumn() 
    {
        isSpawning = true; 
        GameObject puck;
        int emptyRow = GetEmptyRow(ColumnIndex);
        if (gameManager.turn == false)
        {
            puck = Instantiate(puckblue, SpawnPoint.transform.position, Quaternion.identity);
            gameManager.turn = true;
        }
        else
        {
            puck = Instantiate(puckred, SpawnPoint.transform.position, Quaternion.identity);
            gameManager.turn = false;
        }

        // Update the grid with the new puck
        grid[ColumnIndex, emptyRow] = puck;

        // Print out the state of the grid
        Debug.Log("After spawning a puck at column " + ColumnIndex + " and row " + emptyRow + ":");
        for (int i = 0; i < 12; i++)
        {
            string row = "";
            for (int j = 0; j < 12; j++)
            {
                if (grid[j, i] == null)
                {
                    row += "0 ";
                }
                else if (grid[j, i] == puckblue)
                {
                    row += "B ";
                }
                else
                {
                    row += "R ";
                }
            }
            Debug.Log(row);
        }

        yield return new WaitForSeconds(spawnCooldown); 
        isSpawning = false; 
    }

    // This function returns the index of the first empty row in the given column
    private int GetEmptyRow(int columnIndex)
    {
        for (int i = 0; i < 12; i++)
        {
            if (grid[columnIndex, i] == null)
            {
                return i;
            }
        }
        // If no empty row is found, return -1
        return -1;
    }

    // Draw the grid using Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 startPosition = new Vector3(-5.55999994f, -5.40417767f, 1.4626421e-06f);
        for (int i = 0; i <= 12; i++)
        {
            // Draw vertical lines
            Gizmos.DrawLine(startPosition + new Vector3(i, 0, 0), startPosition + new Vector3(i, 12, 0));
            // Draw horizontal lines
            Gizmos.DrawLine(startPosition + new Vector3(0, i, 0), startPosition + new Vector3(12, i, 0));
        }
    }
}
