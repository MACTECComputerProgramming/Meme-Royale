using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour {
    public AudioClip click;
    public AudioSource MusicClick;
    // Use this for initialization

    void Start() {
        MusicClick.clip = click;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MusicClick.Play();
        }
    }
}
