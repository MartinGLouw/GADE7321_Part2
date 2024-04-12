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
        if (gameManager.turn == false)
        {
            Instantiate(puckblue, SpawnPoint.transform.position, Quaternion.identity);
            gameManager.turn = true;
        }
        else
        {
            Instantiate(puckred, SpawnPoint.transform.position, Quaternion.identity);
            gameManager.turn = false;
        }
        yield return new WaitForSeconds(spawnCooldown); 
        isSpawning = false; 
    }
}