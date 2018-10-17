using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

    public Sprite artwork;

    public int manaCost;
    public int attack;
    public int health;

    public void Print()
    {
        Debug.Log("; " + " The card costs: " + manaCost);
    }
}
