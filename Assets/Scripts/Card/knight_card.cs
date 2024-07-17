using UnityEngine;
using UnityEngine.UI;

public class knight_card : MonoBehaviour, CardButton
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    private Text buttonText;
    public Player player;
    private Vector2Int[] knightDirections = 
    {
        new Vector2Int(2, 1), new Vector2Int(2, -1),
        new Vector2Int(-2, 1), new Vector2Int(-2, -1),
        new Vector2Int(1, 2), new Vector2Int(1, -2),
        new Vector2Int(-1, 2), new Vector2Int(-1, -2)
    };

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    public void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;
        player = FindObjectOfType<Player>();

        if (buttonText != null)
        {
            //buttonText.text = "Knight";
        }

        if (button != null)
        {
            button.onClick.AddListener(() => OnClick());
        }
    }

    private void OnClick()
    {
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                player.ShowMoveOptions(knightDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in knight_card.OnClick");
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
