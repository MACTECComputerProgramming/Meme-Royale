using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CardsToPanels : MonoBehaviour {


    public GameObject cardPrefab;
    public int numberOfCards;
    public GameObject Player1Panel;

    public void OnStartServer()
    {
        Player1Panel = GameObject.Find("Player1Hand");
        GameObject cardPrefabInstance = Instantiate(cardPrefab) as GameObject; 
        cardPrefabInstance.transform.parent = Player1Panel.transform;

    }
}
