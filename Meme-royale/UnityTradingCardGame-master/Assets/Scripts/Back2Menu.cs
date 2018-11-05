using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back2Menu : MonoBehaviour {

	
	public void Back2TheMenu () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 5);
		
	}
	
	
	public void Back () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}
}
