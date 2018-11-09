using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial5 : MonoBehaviour {

	
	public void  Back() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
		
	}
	
	
	public void Next() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		
	}
}
