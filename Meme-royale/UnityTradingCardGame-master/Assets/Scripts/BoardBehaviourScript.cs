using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class BoardBehaviourScript : MonoBehaviour
{
    
    public static BoardBehaviourScript instance;
    public UnityEngine.UI.Text winnertext;
    public Transform P1DeckPos;
    public Transform P1HandPos;
    public Transform P1TablePos;

    public Transform P2DeckPos;
    public Transform P2HandPos;
    public Transform P2TablePos;

    public List<GameObject> P1DeckCards = new List<GameObject>();
    public List<GameObject> P1HandCards = new List<GameObject>();
    public List<GameObject> P1TableCards = new List<GameObject>();

    public List<GameObject> P2DeckCards = new List<GameObject>();
    public List<GameObject> P2HandCards = new List<GameObject>();
    public List<GameObject> P2TableCards = new List<GameObject>();

    public TextMesh P1ManaText;
    public TextMesh P2ManaText;

    public HeroBehaviourScript P1Hero;
    public HeroBehaviourScript P2Hero;

    public enum Turn { P1Turn, P2Turn };

    #region SetStartData
    public Turn turn = Turn.P1Turn;

    int maxMana = 1;
    int P1Mana = 1;
    int P2Mana = 1;

    public bool gameStarted = false;
    int turnNumber = 1;
    #endregion

    public CardBehaviourScript currentCard;
    public CardBehaviourScript targetCard;
    public HeroBehaviourScript currentHero;
    public HeroBehaviourScript targetHero;

    public List<Hashtable> boardHistory = new List<Hashtable>();
    public int P2LEVEL = 0;
    public LayerMask layer;
    public void AddHistory(CardGameBase a, CardGameBase b)
    {
        Hashtable hash = new Hashtable();

        hash.Add(a, b);

        boardHistory.Add(hash);
        currentCard = null;
        targetCard = null;
        currentHero = null;
        targetHero = null;
    }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {

        foreach (GameObject CardObject in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardObject.GetComponent<Rigidbody>().isKinematic = true;
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            if (c.team == CardBehaviourScript.Team.P1)
                P1DeckCards.Add(CardObject);
            else
                P2DeckCards.Add(CardObject);
        }
        //Update Deck Cards Position
        DecksPositionUpdate();
        //Update Hand Cards Position
        HandPositionUpdate();

        //Start Game
        StartGame();
    }
    public void StartGame()
    {
        gameStarted = true;
        UpdateGame();

        for (int i = 0; i < 3; i++)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.P1);
            DrawCardFromDeck(CardBehaviourScript.Team.P2);
        }
    }
    public void DrawCardFromDeck(CardBehaviourScript.Team team)
    {

        if (team == CardBehaviourScript.Team.P1 && P1DeckCards.Count != 0 && P1HandCards.Count < 10)
        {
            int random = Random.Range(0, P1DeckCards.Count);
            GameObject tempCard = P1DeckCards[random];

            //tempCard.transform.position = P1HandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().newPos = P1HandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);

            P1DeckCards.Remove(tempCard);
            P1HandCards.Add(tempCard);
        }

        if (team == CardBehaviourScript.Team.P2 && P2DeckCards.Count != 0 && P2HandCards.Count < 10)
        {
            int random = Random.Range(0, P2DeckCards.Count);
            GameObject tempCard = P2DeckCards[random];

            tempCard.transform.position = P2HandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);

            P2DeckCards.Remove(tempCard);
            P2HandCards.Add(tempCard);
        }

        UpdateGame();
        //Update Hand Cards Position
        HandPositionUpdate();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            currentCard = null;
            targetCard = null;
            currentHero = null;
            targetHero = null;
            Debug.Log("Action Revet");
        }
        //if(BoardBehaviourScript.instance.currentCard&&BoardBehaviourScript.instance.targetCard)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 250, layer))
            {
                if (hit.transform.CompareTag("Board"))
                {
                    //do whatever......
                    //Debug.Log(hit.point);
                    
                    if (BoardBehaviourScript.instance.currentCard)
                    drawP1Line(BoardBehaviourScript.instance.currentCard.transform.position, hit.point, Color.green, 0.1f);
                }
            }
        }

        if (P1Hero.health <= 0)
            EndGame(P2Hero);
        if (P2Hero.health <= 0)
            EndGame(P1Hero);

    }
    void drawP1Line(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {

        StartCoroutine(drawLine(start, end, color, duration));

    }
    IEnumerator drawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject P1Line = new GameObject();
        P1Line.transform.position = start;
        P1Line.AddComponent<LineRenderer>();
        LineRenderer lr = P1Line.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Additive"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetVertexCount(3);
        lr.SetPosition(0, start);
        lr.SetPosition(1,(( (-start+ end)*0.5f+start))+new Vector3(0,0,-5.0f));
        lr.SetPosition(2, end);
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(P1Line);
    }
    void UpdateGame()
    {
        P1ManaText.text = P1Mana.ToString() + "/" + maxMana;
        P2ManaText.text = P2Mana.ToString() + "/" + maxMana;

        if (P1Hero.health <= 0)
            EndGame(P2Hero);
        if (P2Hero.health <= 0)
            EndGame(P1Hero);

        //UpdateBoard();
    }

    void DecksPositionUpdate()
    {
        foreach (GameObject CardObject in P1DeckCards)
        {
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            if (c.cardStatus == CardBehaviourScript.CardStatus.InDeck)
            {
                c.newPos = P1DeckPos.position + new Vector3(Random.value, Random.value, Random.value);
            }
        }

        foreach (GameObject CardObject in P2DeckCards)
        {
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            if (c.cardStatus == CardBehaviourScript.CardStatus.InDeck)
            {
                c.newPos = P2DeckPos.position + new Vector3(Random.value, Random.value, Random.value);
            }
        }
    }
    public void HandPositionUpdate()
    {
        float space = 0f;
        float space2 = 0f;
        float gap = 1.3f;

        foreach (GameObject card in P1HandCards)
        {
            int numberOfCards = P1HandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = P1HandPos.position + new Vector3(-numberOfCards / 2 + space, 0, 0);
            space += gap;
        }

        foreach (GameObject card in P2HandCards)
        {
            int numberOfCards = P2HandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = P2HandPos.position + new Vector3(-numberOfCards / 2 + space2, 0, 0);
            space2 += gap;
        }
    }
    public void TablePositionUpdate()
    {
        float space = 0f;
        float space2 = 0f;
        float gap = 3;

        foreach (GameObject card in P1TableCards)
        {
            int numberOfCards = P1TableCards.Count;
            //card.transform.position = P1TablePos.position + new Vector3(-numberOfCards + space - 2,0,0);
            card.GetComponent<CardBehaviourScript>().newPos = P1TablePos.position + new Vector3(-numberOfCards + space - 2, 0, 0);
            space += gap;
        }

        foreach (GameObject card in P2TableCards)
        {
            int numberOfCards = P2TableCards.Count;
            //card.transform.position = AITablePos.position + new Vector3(-numberOfCards + space2,0,0);
            card.GetComponent<CardBehaviourScript>().newPos = P2TablePos.position + new Vector3(-numberOfCards + space2, 0, 0);
            space2 += gap;
        }
    }

    public void PlaceCard(CardBehaviourScript card)
    {
        if (card.team == CardBehaviourScript.Team.P1 && P1Mana - card.mana >= 0 && P1TableCards.Count < 10)
        {
            //card.gameObject.transform.position = P1TablePos.position;
            card.GetComponent<CardBehaviourScript>().newPos = P1TablePos.position;

            P1HandCards.Remove(card.gameObject);
            P1TableCards.Add(card.gameObject);

            card.SetCardStatus(CardBehaviourScript.CardStatus.OnTable);
            //PlaySound(cardDrop);

            if (card.cardtype == CardBehaviourScript.CardType.Magic)///Apply Magic Effect 
            {
                card.canPlay = true;
                if (card.cardeffect == CardBehaviourScript.CardEffect.ToAll)
                {
                    card.AddToAll(card,true, delegate { card.Destroy(card); });
                }
                else if (card.cardeffect == CardBehaviourScript.CardEffect.ToEnemies)
                {
                    card.AddToEnemies(card,P2TableCards,true, delegate { card.Destroy(card); });
                }
            }

            P1Mana -= card.mana;
        }

        if (card.team == CardBehaviourScript.Team.P2 && P2Mana - card.mana >= 0 && P2TableCards.Count < 10)
        {
            //card.gameObject.transform.position = AITablePos.position;
            card.GetComponent<CardBehaviourScript>().newPos = P2TablePos.position;

            P2HandCards.Remove(card.gameObject);
            P2TableCards.Add(card.gameObject);

            card.SetCardStatus(CardBehaviourScript.CardStatus.OnTable);
            //PlaySound(cardDrop);

            if (card.cardtype == CardBehaviourScript.CardType.Magic)///Apply Magic Effect 
            {
                card.canPlay = true;
                if (card.cardeffect == CardBehaviourScript.CardEffect.ToAll)
                {
                    card.AddToAll(card,true, delegate { card.Destroy(card); });
                }
                else if (card.cardeffect == CardBehaviourScript.CardEffect.ToEnemies)
                {
                    card.AddToEnemies(card,P1TableCards,true, delegate { card.Destroy(card); });
                }
            }

            P2Mana -= card.mana;
        }

        TablePositionUpdate();
        HandPositionUpdate();
        UpdateGame();
    }
    public void PlaceRandomCard(CardBehaviourScript.Team team)
    {
        if (team == CardBehaviourScript.Team.P1 && P1HandCards.Count != 0)
        {
            int random = Random.Range(0, P1HandCards.Count);
            GameObject tempCard = P1HandCards[random];

            PlaceCard(tempCard.GetComponent<CardBehaviourScript>());
        }

        if (team == CardBehaviourScript.Team.P2 && P2HandCards.Count != 0)
        {
            int random = Random.Range(0, P2HandCards.Count);
            GameObject tempCard = P2HandCards[random];

            PlaceCard(tempCard.GetComponent<CardBehaviourScript>());
        }

        UpdateGame();
        EndTurn();

        TablePositionUpdate();
        HandPositionUpdate();
    }
    public void EndGame(HeroBehaviourScript winner)
    {
        if (winner == P1Hero)
        {
            Debug.Log("P1Hero");
            Time.timeScale = 0;
            winnertext.text = "You Won";
            //Destroy(this);
        }

        if (winner == P2Hero)
        {
            Time.timeScale = 0;
            Debug.Log("AIHero");
            winnertext.text = "You Losse";
            //Destroy(this);
        }
    }
    void OnGUI()
    {
        if (gameStarted)
        {
            if (turn == Turn.P1Turn)
            {
                if (GUI.Button(new Rect(Screen.width - 200, Screen.height / 2 - 50, 100, 50), "End Turn"))
                {
                    EndTurn();
                }
            }

            if (turn == Turn.P2Turn)
            {
                if (GUI.Button(new Rect(Screen.width - 200, Screen.height / 2 - 50, 100, 50), "End Turn"))
                {
                    EndTurn();
                }
            }

            GUI.Label(new Rect(Screen.width-200, Screen.height / 2 - 100, 100, 50), "Turn: " + turn + " Turn Number: " + turnNumber.ToString());

            foreach (Hashtable history in boardHistory)
            {
                foreach (DictionaryEntry entry in history)
                {
                    CardGameBase card1 = entry.Key as CardGameBase;
                    CardGameBase card2 = entry.Value as CardGameBase;

                    GUILayout.Label(card1._name + " > " + card2._name);
                }
            }
            if (boardHistory.Count > 25)
            {
                Hashtable temp;
                temp = boardHistory[boardHistory.Count - 1];
                boardHistory.Clear();
                boardHistory.Add(temp);
            }
        }
    }
    void EndTurn()
    {
        maxMana += (turnNumber-1)%2;
        if (maxMana >= 10) maxMana = 10;
        P1Mana = maxMana;
        P2Mana = maxMana;
        turnNumber += 1;
        currentCard = new CardBehaviourScript() ;
        targetCard = new CardBehaviourScript();
        currentHero = new HeroBehaviourScript();
        targetHero = new HeroBehaviourScript();
        foreach (GameObject card in P1TableCards)
            card.GetComponent<CardBehaviourScript>().canPlay = true;

        foreach (GameObject card in P2TableCards)
            card.GetComponent<CardBehaviourScript>().canPlay = true;
        P1Hero.CanAttack = true;
        P2Hero.CanAttack = true;

        if (turn == Turn.P2Turn)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.P1);
            turn = Turn.P1Turn;
        }
        else if (turn == Turn.P1Turn)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.P2);
            turn = Turn.P2Turn;
        }

        HandPositionUpdate();
        TablePositionUpdate();

       
    
    
    }
    void OnTriggerEnter(Collider Obj)
    {
        CardBehaviourScript card = Obj.GetComponent<CardBehaviourScript>();
        if (card)
        {
            card.PlaceCard();
        }

    }
}
