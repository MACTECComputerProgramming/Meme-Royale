using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfTutorials : MonoBehaviour {

	
	public void Back() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}
	
	
	public void Back2Menu() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 7);
		
	}
}
