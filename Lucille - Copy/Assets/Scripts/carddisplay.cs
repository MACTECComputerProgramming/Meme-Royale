using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class carddisplay : MonoBehaviour {
    public Card card;

    public Text manaText;
    public Text attackText;
    public Text healthText;

    
	// Use this for initialization
	void Start () {

        manaText.text = card.manaCost.ToString();
        attackText.text = card.attack.ToString();
        healthText.text = card.health.ToString();
	}


}
