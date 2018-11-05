using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial3 : MonoBehaviour {

	
	public void Back2Main () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 4);
		
	}
	
	
	public void BackButton () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
		
	}
    public void NextButton () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
