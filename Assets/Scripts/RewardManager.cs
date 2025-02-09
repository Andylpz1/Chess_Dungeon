using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardPanel; // Reference to the RewardPanel in the scene

    // References to the card GameObjects and their corresponding Texts
    public Image card1;
    public Image card2;
    public Image card3;
    public Text text1;
    public Text text2;
    public Text text3;
    public Button refreshButton;
    public Button skipButton;

    private DeckManager deckManager; // Reference to the DeckManager
    public bool isRewardPanelOpen = false;
    private List<Card> rewardCards; // List of cards to choose from
    private Dictionary<string, List<Card>> rarityPools = new Dictionary<string, List<Card>>();

    public event System.Action OnRewardSelectionComplete;

    void Start()
    {
        rewardPanel.SetActive(false); // Hide the panel at the start
        deckManager = FindObjectOfType<DeckManager>();
        refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
    }

    private void InitializeRarityPools()
    {
        rarityPools["Common"] = new List<Card>
        {
            //new PawnCard(),
            new KnightCard(),
            new SwordCard(),
            new BladeCard(),
        };

        rarityPools["Uncommon"] = new List<Card>
        {
            new BishopCard(),
            new RookCard(),
            new SpearCard(),
            new BowCard(),
            new PotionCard(),
            new EnergyCore(),
            new SickleCard(),
            new RitualSpear(),
            new Vine(),
            new Assassin(),
            new TwoBladeCard()
            // Add more uncommon cards here
        };

        rarityPools["Epic"] = new List<Card>
        {
            new FlailCard(),
            new FloatSword(),
            new DarkEnergy(),
            new MadnessEcho()
            // Add more epic cards here
        };

        rarityPools["Legendary"] = new List<Card>
        {
            new FlailCard()
        };
    }

    private string GetRandomRarity()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < 80f)
        {
            return "Common";
        }
        else if (randomValue < 95f)
        {
            return "Uncommon";
        }
        else if (randomValue < 99f)
        {
            return "Epic";
        }
        else
        {
            return "Legendary";
        }
    }


    // Method to open the reward panel with three cards
    public void OpenRewardPanel()
    {
        isRewardPanelOpen = true;
        rewardPanel.SetActive(true);
        rewardCards = GenerateRewardCards(); // Generate three random cards

        // Assign each card to a button and set the image and description
        SetCard(card1, text1, rewardCards[0]);
        SetCard(card2, text2, rewardCards[1]);
        SetCard(card3, text3, rewardCards[2]);
    }

    // Method to generate three random cards (customize as needed)
    private List<Card> GenerateRewardCards()
    {
        InitializeRarityPools(); // Make sure rarity pools are initialized

        List<Card> rewardCards = new List<Card>();

        for (int i = 0; i < 3; i++)
        {
            string rarity = GetRandomRarity();

            // Get the list of cards for the chosen rarity
            List<Card> cardPool = rarityPools[rarity];

            if (cardPool.Count > 0)
            {
                // Select a random card from the chosen rarity pool
                int randomIndex = Random.Range(0, cardPool.Count);
                Card selectedCard = cardPool[randomIndex];

                rewardCards.Add(selectedCard);
            }
            else
            {
                Debug.LogWarning($"No cards available in the {rarity} rarity pool.");
            }   
        }

        return rewardCards;
    }


    // Method to set the card image and description
    private void SetCard(Image cardImage, Text cardText, Card card)
    {
        cardImage.sprite = card.GetSprite(); // Set the card image
        cardText.text = card.GetDescription(); // Set the card description
        cardImage.GetComponent<Button>().onClick.RemoveAllListeners(); // Remove previous listeners
        cardImage.GetComponent<Button>().onClick.AddListener(() => OnCardSelected(card)); // Bind the click event to select the card
    }

    // Method called when a card is selected
    private void OnCardSelected(Card selectedCard)
    {
        deckManager.deck.Add(selectedCard); // Add the selected card to the deck
        deckManager.UpdateDeckCountText();
        deckManager.UpdateDeckPanel();

        CloseRewardPanel();
        OnRewardSelectionComplete?.Invoke();
    }

    private void OnRefreshButtonClicked()
    {
        rewardCards = GenerateRewardCards(); // Generate three new random cards

        // Assign the new cards to the UI
        SetCard(card1, text1, rewardCards[0]);
        SetCard(card2, text2, rewardCards[1]);
        SetCard(card3, text3, rewardCards[2]);
    }

    // Method to close the reward panel without selecting a card
    private void OnSkipButtonClicked()
    {
        CloseRewardPanel();
        OnRewardSelectionComplete?.Invoke();
    }

    // Method to close the reward panel
    private void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);
    }



    
}
