using UnityEngine;
using UnityEngine.UI;

public class bishop_card : MonoBehaviour, CardButton
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    private Text buttonText;
    public Player player;
    private Vector2Int[] bishopDirections = 
    { 
        new Vector2Int(1, 1), new Vector2Int(1, -1),
        new Vector2Int(-1, 1), new Vector2Int(-1, -1),
        new Vector2Int(2, 2), new Vector2Int(2, -2),
        new Vector2Int(-2, 2), new Vector2Int(-2, -2),
        new Vector2Int(3, 3), new Vector2Int(3, -3),
        new Vector2Int(-3, 3), new Vector2Int(-3, -3),
        new Vector2Int(4, 4), new Vector2Int(4, -4),
        new Vector2Int(-4, 4), new Vector2Int(-4, -4)
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
            //buttonText.text = "Bishop";
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
            player.ShowMoveOptions(bishopDirections, card);
        }
        else
        {
            Debug.LogError("Card is null in bishop_card.OnClick");
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
