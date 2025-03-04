using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance { get; private set; }

    private Dictionary<string, Card> cardLibrary = new Dictionary<string, Card>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCardDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCardDatabase()
    {
        // 在这里手动添加所有卡牌对象
        AddCard(new PawnCard());
        AddCard(new KnightCard());
        AddCard(new BishopCard());
        AddCard(new RookCard());
        AddCard(new SwordCard());
        AddCard(new BladeCard());
        AddCard(new SpearCard());
        AddCard(new BowCard());
        AddCard(new PotionCard());
        AddCard(new EnergyCore());
        AddCard(new SickleCard());
        AddCard(new RitualSpear());
        AddCard(new Assassin());
        AddCard(new TwoBladeCard());
        AddCard(new FloatSword());
        AddCard(new FlailCard());
        AddCard(new DarkEnergy());
        AddCard(new MadnessEcho());
        AddCard(new Vine());
        AddCard(new Coffin());
        AddCard(new Book());
        AddCard(new Belt());
        AddCard(new BookOfPawn());
        AddCard(new BookOfKnight());
        AddCard(new BookOfBishop());
        AddCard(new BookOfRook());
        AddCard(new BookOfQueen());
        AddCard(new Fan());
    }

    private void AddCard(Card card)
    {
        if (!cardLibrary.ContainsKey(card.Id))
        {
            cardLibrary.Add(card.Id, card);
        }
    }

    public Card GetCardById(string cardId)
    {
        if (cardLibrary.TryGetValue(cardId, out Card card))
        {
            return card;
        }
        return null;
    }
}
