﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class CardBehaviourScript : CardGameBase, System.ICloneable
{

    public string description = "Description";
    public Texture2D image;
    public int health;
    public int _Attack;
    public int mana;

    public TextMesh nameText;
    public TextMesh healthText;
    public TextMesh AttackText;
    public TextMesh manaText;
    public TextMesh DescriptionText;
    public TextMesh DebugText;

    public bool GenerateRandomeData = false;
    public bool canPlay = false;
    public enum CardStatus { InDeck, InHand, OnTable, Destroyed };
    public CardStatus cardStatus = CardStatus.InDeck;
    public enum CardType { Monster, Magic };
    public CardType cardtype;
    public enum CardEffect { ToAll, ToEnemies, ToSpecific };
    public CardEffect cardeffect;
    public int AddedHealth;
    public int AddedAttack;
    public enum Team { P1, P2 };
    public Team team = Team.P1;

    public Vector3 newPos;

    float distance_to_screen;
    bool Selected = false;
    public delegate void CustomAction();
    void Start()
    {
        distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z - 1;
        DescriptionText.text = description.ToString();      
    }
    void FixedUpdate()
    {
        //Raycast
        if (!Selected)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 3);
            if (cardStatus != CardStatus.InDeck)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime*3);
            }
        }
        if (cardtype==CardType.Monster)
        {
            if (health <= 0)
            {
                Destroy(this);
            }
        }
        //Update Visuals
        nameText.text = _name.ToString();
        healthText.text = health.ToString();
        AttackText.text = _Attack.ToString();
        manaText.text = mana.ToString();
        DebugText.text = canPlay ? "Ready to attack" : "Nope";
    }
    public void PlaceCard()
    {
        //Place card on your turn
        if (BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P1Turn && cardStatus == CardStatus.InHand && team == Team.P1)
        {
            BoardBehaviourScript.instance.PlaceCard(this);
        }
        else if (BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P2Turn && cardStatus == CardStatus.InHand && team == Team.P2)
        {
            BoardBehaviourScript.instance.PlaceCard(this);
        }
    }
    void OnMouseDown()
    {
        if (cardStatus == CardStatus.InHand)
        {
            Selected = true;
        }
        if (!BoardBehaviourScript.instance.currentCard && cardStatus==CardStatus.OnTable)
        {
            //clicked on your card on table to attack another table card
            BoardBehaviourScript.instance.currentCard = this;
            print("Selected card: " + _Attack + ":" + health);
        }
        
        else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Magic && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P1Turn && cardStatus == CardStatus.OnTable)
        {
            if (BoardBehaviourScript.instance.currentCard.cardeffect == CardEffect.ToSpecific)//Magic VS Card
            {//What Magic Card Will Do To MonsterCard
                BoardBehaviourScript.instance.targetCard = this;
                print("Target card: " + _Attack + ":" + health);
                if (BoardBehaviourScript.instance.currentCard.canPlay)
                {
                    AddToMonster(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard,true, delegate
                    {
                        BoardBehaviourScript.instance.currentCard.Destroy(BoardBehaviourScript.instance.currentCard);
                    });
                }
            }
            

        }
        else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Magic && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P2Turn && cardStatus == CardStatus.OnTable)
        {
            if (BoardBehaviourScript.instance.currentCard.cardeffect == CardEffect.ToSpecific)//Magic VS Card
            {//What Magic Card Will Do To MonsterCard
                BoardBehaviourScript.instance.targetCard = this;
                print("Target card: " + _Attack + ":" + health);
                if (BoardBehaviourScript.instance.currentCard.canPlay)
                {
                    AddToMonster(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard, true, delegate
                    {
                        BoardBehaviourScript.instance.currentCard.Destroy(BoardBehaviourScript.instance.currentCard);
                    });
                }
            }


        }
        else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Monster && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P2Turn && cardStatus == CardStatus.OnTable && BoardBehaviourScript.instance.currentCard!=this)//Card VS Card
        {
            //Click your card on your tunr to attack
            if (BoardBehaviourScript.instance.currentCard != null && BoardBehaviourScript.instance.currentCard.canPlay)
            {
                BoardBehaviourScript.instance.targetCard = this;
                print("Target card: " + _Attack + ":" + health);
                if (BoardBehaviourScript.instance.currentCard.canPlay)
                {
                    AttackCard(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard, true, delegate
                     {
                         BoardBehaviourScript.instance.currentCard.canPlay = false;
                     });
                }
                else print("Card cannot attack");
            }
            print("Cannot Attack this Target card: ");
        }
        else if (BoardBehaviourScript.instance.currentCard && BoardBehaviourScript.instance.currentCard.cardtype == CardType.Monster && BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P1Turn && cardStatus == CardStatus.OnTable && BoardBehaviourScript.instance.currentCard != this)//Card VS Card
        {
            //Click your card on your tunr to attack
            if (BoardBehaviourScript.instance.currentCard != null && BoardBehaviourScript.instance.currentCard.canPlay)
            {
                BoardBehaviourScript.instance.targetCard = this;
                print("Target card: " + _Attack + ":" + health);
                if (BoardBehaviourScript.instance.currentCard.canPlay)
                {
                    AttackCard(BoardBehaviourScript.instance.currentCard, BoardBehaviourScript.instance.targetCard, true, delegate
                    {
                        BoardBehaviourScript.instance.currentCard.canPlay = false;
                    });
                }
                else print("Card cannot attack");
            }
            print("Cannot Attack this Target card: ");
        }
        else if ((BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P1Turn && BoardBehaviourScript.instance.currentHero && cardStatus == CardStatus.OnTable))//Hero VS Card
        {
            //Attack the opponent Hero
            if (BoardBehaviourScript.instance.currentHero.CanAttack)
            {
                BoardBehaviourScript.instance.targetCard = this;
                print("Target card: " + _Attack + ":" + health);
                BoardBehaviourScript.instance.currentHero.AttackCard(BoardBehaviourScript.instance.currentHero, BoardBehaviourScript.instance.targetCard, delegate
                {
                    BoardBehaviourScript.instance.currentHero.CanAttack = false;
                });
            }
        }
        else if ((BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.P2Turn && BoardBehaviourScript.instance.currentHero && cardStatus == CardStatus.OnTable))//Hero VS Card
        {
            //Attack the opponent Hero
            if (BoardBehaviourScript.instance.currentHero.CanAttack)
            {
                BoardBehaviourScript.instance.targetCard = this;
                print("Target card: " + _Attack + ":" + health);
                BoardBehaviourScript.instance.currentHero.AttackCard(BoardBehaviourScript.instance.currentHero, BoardBehaviourScript.instance.targetCard, delegate
                {
                    BoardBehaviourScript.instance.currentHero.CanAttack = false;
                });
            }
        }
        else
        {
            //Reset
            BoardBehaviourScript.instance.currentCard = null;
            BoardBehaviourScript.instance.currentHero = null;
            BoardBehaviourScript.instance.targetCard = null;
            BoardBehaviourScript.instance.targetHero = null;
            Debug.Log("Action Reset");
        }

    }
    void OnMouseUp()
    {
        Selected = false;
    }
    void OnMouseOver()
    {
    }
    void OnMouseEnter()
    {
    }
    void OnMouseExit()
    {
    }
    void OnMouseDrag()
    {
        GetComponent<Rigidbody>().MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen)));
    }
    public void SetCardStatus(CardStatus status)
    {
        cardStatus = status;
    }
    public void AttackCard(CardBehaviourScript attacker, CardBehaviourScript target,bool addhistory, CustomAction action)
    {
        //How card attack opponent card
        if (attacker.canPlay)
        {
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;
           

            if (target.health <= 0)
            {
                    Destroy(target);
            }

            if (attacker.health <= 0)
            {
                    attacker.Destroy(attacker);
            }

            action();
            if(addhistory)
            BoardBehaviourScript.instance.AddHistory(attacker, target);
        }

    }
    public void AttackHero(CardBehaviourScript attacker, HeroBehaviourScript target, bool addhistory, CustomAction action)
    {
        //How card attack Hero
        if (attacker.canPlay)
        {
            target.health -= attacker._Attack;
            attacker.health -= target._Attack;

            action();
            if (addhistory)
                BoardBehaviourScript.instance.AddHistory(attacker, target);
        }
    }
    public void Destroy(CardBehaviourScript card)
    {
        //Card disappear
        if (card)
        {
            if (card.gameObject != null)
            {
                if (card.team == CardBehaviourScript.Team.P1)
                    BoardBehaviourScript.instance.P1TableCards.Remove(card.gameObject);
                else if (card.team == CardBehaviourScript.Team.P2)
                    BoardBehaviourScript.instance.P2TableCards.Remove(card.gameObject);
                Destroy(card.gameObject);

                BoardBehaviourScript.instance.TablePositionUpdate();
            }

        }else
            {
        }
    }
    public void AddToHero(CardBehaviourScript magic, HeroBehaviourScript target, CustomAction action)
    {
        if (magic.canPlay)
        {
            //Magic on Hero
            target._Attack += magic.AddedAttack;
            if (target.health + magic.AddedHealth <= 30)
                target.health += magic.AddedHealth;
            else
                target.health = 30;
            action();
            BoardBehaviourScript.instance.AddHistory(magic, target);
        }
    }
    public void AddToMonster(CardBehaviourScript magic, CardBehaviourScript target,bool addhistory, CustomAction action)
    {
        if (magic.canPlay)
        {
            //Magic on card
            target._Attack += magic.AddedAttack;
            target.health += magic.AddedHealth;
            action();
            if(addhistory)
            BoardBehaviourScript.instance.AddHistory(magic, target);
        }
    }
    public void AddToAll(CardBehaviourScript magic, bool addhistory, CustomAction action)
    {
        if (magic.canPlay)
        {
            //Magic on all object
            foreach (var target in BoardBehaviourScript.instance.P2TableCards)
            {
                AddToMonster(magic, target.GetComponent<CardBehaviourScript>(), addhistory, delegate { });
            }
            foreach (var target in BoardBehaviourScript.instance.P1TableCards)
            {
                AddToMonster(magic, target.GetComponent<CardBehaviourScript>(), addhistory, delegate { });
            }
            action();
        }
    }
    public void AddToEnemies(CardBehaviourScript magic, List<GameObject> targets, bool addhistory, CustomAction action)
    {
        if (magic.canPlay)
        {
            //Magic on enemies 
            foreach (var target in targets)
            {
                AddToMonster(magic, target.GetComponent<CardBehaviourScript>(), addhistory, delegate { });
            }
            action();
        }
    }
    public void AddToEnemies(CardBehaviourScript magic, List<CardBehaviourScript> targets, bool addhistory, CustomAction action)
    {
        if (magic.canPlay)
        {
            foreach (var target in targets)
            {
                AddToMonster(magic, target, addhistory, delegate { });
            }
            action();
        }
    }
    public object Clone()
    {
        CardBehaviourScript temp = new CardBehaviourScript();
        temp._name = _name;
        temp.description = this.description;
        temp.health = this.health;
        temp._Attack = this._Attack;
        temp.mana = this.mana;
        temp.canPlay = this.canPlay;
        temp.cardStatus = this.cardStatus;
        temp.cardtype = this.cardtype;
        temp.cardeffect = this.cardeffect;
        temp.AddedHealth = this.AddedHealth;
        temp.AddedAttack = this.AddedAttack;
        temp.team = this.team;
        temp.newPos = this.newPos;
        temp.distance_to_screen = this.distance_to_screen;
        temp.Selected = this.Selected;
        return temp;
    }
}
