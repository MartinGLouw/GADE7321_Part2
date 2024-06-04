using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneManager : MonoBehaviour
{
    public void LoadPVP()
    {
        SceneManager.LoadScene("PVPScene");
    }
}