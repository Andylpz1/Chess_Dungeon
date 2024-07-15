using UnityEngine;
using UnityEngine.UI;

public class pawn_card : MonoBehaviour, CardButton
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    private Text buttonText;
    public Player player;
    private Vector2Int[] pawnDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
            //buttonText.text = "Pawn";
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
            player.ShowMoveOptions(pawnDirections, card);
        }
        else
        {
            Debug.LogError("Card is null in pawn_card.OnClick");
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
