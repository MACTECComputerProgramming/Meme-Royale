﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu2Game : MonoBehaviour {

	
	public void  PlayGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);	
	}
    public void Tutorial()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
	
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
