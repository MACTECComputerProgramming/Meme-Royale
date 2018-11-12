﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class HeroBehaviourScript : CardGameBase ,ICloneable
{

    public int health = 30;
    public bool CanAttack = true;
    public int _Attack = 0;

    public TextMesh healthText;
    public TextMesh AttackText;
    public TextMesh DebugText;

    public delegate void CustomAction();
    public void OnMouseDown()
    {
        if (BoardBehaviourScript.instance.currentCard)//Card [Magic,Monster] VS Hero
        {
            if (BoardBehaviourScript.instance.currentCard.canPlay)
            {
                if (BoardBehaviourScript.instance.currentCard.cardtype==CardBehaviourScript.CardType.Monster)
                {
                    BoardBehaviourScript.instance.currentCard.AttackHero(BoardBehaviourScript.instance.currentCard, this,true, delegate
                    {
                        BoardBehaviourScript.instance.currentCard.canPlay = false;
                    });
                }
                else if ((BoardBehaviourScript.instance.currentCard.cardtype == CardBehaviourScript.CardType.Magic))
                {
                    BoardBehaviourScript.instance.currentCard.AddToHero(BoardBehaviourScript.instance.currentCard, this, delegate
                    {
                        BoardBehaviourScript.instance.currentCard.Destroy(BoardBehaviourScript.instance.currentCard);
                    });
                }
                
            }
        }
        else if ( !BoardBehaviourScript.instance.currentHero)
        {
            {
                BoardBehaviourScript.instance.currentHero = this;
                Debug.Log(name + "   Hero Selected");
            }
        }
        else if (BoardBehaviourScript.instance.currentHero && CanAttack)//Hero Vs Hero
        {
            BoardBehaviourScript.instance.targetHero = this;

            if (BoardBehaviourScript.instance.currentHero.CanAttack && BoardBehaviourScript.instance.targetHero!= BoardBehaviourScript.instance.currentHero)
            {
                AttackHero(BoardBehaviourScript.instance.currentHero, BoardBehaviourScript.instance.targetHero, delegate
                {
                    BoardBehaviourScript.instance.currentHero.CanAttack = false;
                });
            }
            else print("Hero cannot attack");
        }
    }

    void FixedUpdate()
    {
        //Update Health and Attack
        healthText.text = health.ToString();
        AttackText.text = _Attack.ToString();

    }
    public void AttackCard(HeroBehaviourScript attacker, CardBehaviourScript target, CustomAction action)
    {
        //Card attack another card
        if (attacker.CanAttack)
        {
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;

            if (target.health <= 0)
            {
                target.Destroy(target);
            }
            action();
            BoardBehaviourScript.instance.AddHistory(attacker, target);
        }
    }
    public void AttackHero(HeroBehaviourScript attacker, HeroBehaviourScript target, CustomAction action)
    {
        //Hero attack another hero
        if (attacker.CanAttack)
        {
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;

            if (target.health <= 0)
            {
                Destroy(target.gameObject);
                BoardBehaviourScript.instance.EndGame(attacker);
            }
            action();
            BoardBehaviourScript.instance.AddHistory(attacker, target);
        }
    }

    public object Clone()
    {
        //Display Name, Health, and Attack
        HeroBehaviourScript temp = new HeroBehaviourScript();
        temp._name = _name;
        temp.health = health;
        temp.CanAttack = CanAttack;
        temp._Attack = _Attack;
        return temp;
    }
}
