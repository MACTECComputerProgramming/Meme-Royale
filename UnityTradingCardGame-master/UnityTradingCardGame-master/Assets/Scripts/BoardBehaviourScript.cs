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
    public Transform AITablePos;

    public List<GameObject> MyDeckCards = new List<GameObject>();
    public List<GameObject> MyHandCards = new List<GameObject>();
    public List<GameObject> MyTableCards = new List<GameObject>();

    public List<GameObject> AIDeckCards = new List<GameObject>();
    public List<GameObject> AIHandCards = new List<GameObject>();
    public List<GameObject> AITableCards = new List<GameObject>();

    public TextMesh MyManaText;
    public TextMesh AIManaText;

    public HeroBehaviourScript MyHero;
    public HeroBehaviourScript AIHero;

    public enum Turn { MyTurn, AITurn };

    #region SetStartData
    public Turn turn = Turn.MyTurn;

    int maxMana = 1;
    int MyMana = 1;
    int AIMana = 1;

    public bool gameStarted = false;
    int turnNumber = 1;
    #endregion

    public CardBehaviourScript currentCard;
    public CardBehaviourScript targetCard;
    public HeroBehaviourScript currentHero;
    public HeroBehaviourScript targetHero;

    public List<Hashtable> boardHistory = new List<Hashtable>();
    public int AILEVEL = 0;
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

            if (c.team == CardBehaviourScript.Team.My)
                MyDeckCards.Add(CardObject);
            else
                AIDeckCards.Add(CardObject);
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
            DrawCardFromDeck(CardBehaviourScript.Team.My);
            DrawCardFromDeck(CardBehaviourScript.Team.AI);
        }
    }
    public void DrawCardFromDeck(CardBehaviourScript.Team team)
    {

        if (team == CardBehaviourScript.Team.My && MyDeckCards.Count != 0 && MyHandCards.Count < 10)
        {
            int random = Random.Range(0, MyDeckCards.Count);
            GameObject tempCard = MyDeckCards[random];

            //tempCard.transform.position = MyHandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().newPos = P1HandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);

            MyDeckCards.Remove(tempCard);
            MyHandCards.Add(tempCard);
        }

        if (team == CardBehaviourScript.Team.AI && AIDeckCards.Count != 0 && AIHandCards.Count < 10)
        {
            int random = Random.Range(0, AIDeckCards.Count);
            GameObject tempCard = AIDeckCards[random];

            tempCard.transform.position = P2HandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);

            AIDeckCards.Remove(tempCard);
            AIHandCards.Add(tempCard);
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
                    if(BoardBehaviourScript.instance.currentHero)
                    drawMyLine(BoardBehaviourScript.instance.currentHero.transform.position, hit.point, Color.blue, 0.1f);
                    if (BoardBehaviourScript.instance.currentCard)
                    drawMyLine(BoardBehaviourScript.instance.currentCard.transform.position, hit.point, Color.blue, 0.1f);
                }
            }
        }

        if (MyHero.health <= 0)
            EndGame(AIHero);
        if (AIHero.health <= 0)
            EndGame(MyHero);

    }
    void drawMyLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {

        StartCoroutine(drawLine(start, end, color, duration));

    }
    IEnumerator drawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Additive"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetVertexCount(3);
        lr.SetPosition(0, start);
        lr.SetPosition(1,(( (-start+ end)*0.5f+start))+new Vector3(0,0,-5.0f));
        lr.SetPosition(2, end);
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(myLine);
    }
    void UpdateGame()
    {
        MyManaText.text = MyMana.ToString() + "/" + maxMana;
        AIManaText.text = AIMana.ToString() + "/" + maxMana;

        if (MyHero.health <= 0)
            EndGame(AIHero);
        if (AIHero.health <= 0)
            EndGame(MyHero);

        //UpdateBoard();
    }

    void DecksPositionUpdate()
    {
        foreach (GameObject CardObject in MyDeckCards)
        {
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            if (c.cardStatus == CardBehaviourScript.CardStatus.InDeck)
            {
                c.newPos = P1DeckPos.position + new Vector3(Random.value, Random.value, Random.value);
            }
        }

        foreach (GameObject CardObject in AIDeckCards)
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

        foreach (GameObject card in MyHandCards)
        {
            int numberOfCards = MyHandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = P1HandPos.position + new Vector3(-numberOfCards / 2 + space, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AIHandCards)
        {
            int numberOfCards = AIHandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = P2HandPos.position + new Vector3(-numberOfCards / 2 + space2, 0, 0);
            space2 += gap;
        }
    }
    public void TablePositionUpdate()
    {
        float space = 0f;
        float space2 = 0f;
        float gap = 3;

        foreach (GameObject card in MyTableCards)
        {
            int numberOfCards = MyTableCards.Count;
            //card.transform.position = myTablePos.position + new Vector3(-numberOfCards + space - 2,0,0);
            card.GetComponent<CardBehaviourScript>().newPos = P1TablePos.position + new Vector3(-numberOfCards + space - 2, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AITableCards)
        {
            int numberOfCards = AITableCards.Count;
            //card.transform.position = AITablePos.position + new Vector3(-numberOfCards + space2,0,0);
            card.GetComponent<CardBehaviourScript>().newPos = AITablePos.position + new Vector3(-numberOfCards + space2, 0, 0);
            space2 += gap;
        }
    }

    public void PlaceCard(CardBehaviourScript card)
    {
        if (card.team == CardBehaviourScript.Team.My && MyMana - card.mana >= 0 && MyTableCards.Count < 10)
        {
            //card.gameObject.transform.position = MyTablePos.position;
            card.GetComponent<CardBehaviourScript>().newPos = P1TablePos.position;

            MyHandCards.Remove(card.gameObject);
            MyTableCards.Add(card.gameObject);

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
                    card.AddToEnemies(card,AITableCards,true, delegate { card.Destroy(card); });
                }
            }

            MyMana -= card.mana;
        }

        if (card.team == CardBehaviourScript.Team.AI && AIMana - card.mana >= 0 && AITableCards.Count < 10)
        {
            //card.gameObject.transform.position = AITablePos.position;
            card.GetComponent<CardBehaviourScript>().newPos = AITablePos.position;

            AIHandCards.Remove(card.gameObject);
            AITableCards.Add(card.gameObject);

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
                    card.AddToEnemies(card,MyTableCards,true, delegate { card.Destroy(card); });
                }
            }

            AIMana -= card.mana;
        }

        TablePositionUpdate();
        HandPositionUpdate();
        UpdateGame();
    }
    public void PlaceRandomCard(CardBehaviourScript.Team team)
    {
        if (team == CardBehaviourScript.Team.My && MyHandCards.Count != 0)
        {
            int random = Random.Range(0, MyHandCards.Count);
            GameObject tempCard = MyHandCards[random];

            PlaceCard(tempCard.GetComponent<CardBehaviourScript>());
        }

        if (team == CardBehaviourScript.Team.AI && AIHandCards.Count != 0)
        {
            int random = Random.Range(0, AIHandCards.Count);
            GameObject tempCard = AIHandCards[random];

            PlaceCard(tempCard.GetComponent<CardBehaviourScript>());
        }

        UpdateGame();
        EndTurn();

        TablePositionUpdate();
        HandPositionUpdate();
    }
    public void EndGame(HeroBehaviourScript winner)
    {
        if (winner == MyHero)
        {
            Debug.Log("MyHero");
            Time.timeScale = 0;
            winnertext.text = "You Won";
            //Destroy(this);
        }

        if (winner == AIHero)
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
            if (turn == Turn.MyTurn)
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
        MyMana = maxMana;
        AIMana = maxMana;
        turnNumber += 1;
        currentCard = new CardBehaviourScript() ;
        targetCard = new CardBehaviourScript();
        currentHero = new HeroBehaviourScript();
        targetHero = new HeroBehaviourScript();
        foreach (GameObject card in MyTableCards)
            card.GetComponent<CardBehaviourScript>().canPlay = true;

        foreach (GameObject card in AITableCards)
            card.GetComponent<CardBehaviourScript>().canPlay = true;
        MyHero.CanAttack = true;
        AIHero.CanAttack = true;

        if (turn == Turn.AITurn)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.My);
            turn = Turn.MyTurn;
        }
        else if (turn == Turn.MyTurn)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.AI);
            turn = Turn.AITurn;
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
