using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class rook_card : MonoBehaviour, CardButton
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    private Text buttonText;
    public Player player;

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
            //buttonText.text = "Rook";
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
                ShowRookMoveOptions();
            }
        }
        else
        {
            Debug.LogError("Card is null in rook_card.OnClick");
        }
    }

    private void ShowRookMoveOptions()
    {
        if (player == null)
        {
            Debug.LogError("Player is null in ShowRookMoveOptions.");
            return;
        }

        if (card == null)
        {
            Debug.LogError("Card is null in ShowRookMoveOptions.");
            return;
        }

        player.ClearMoveHighlights();
        List<Vector2Int> validPositions = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), // 横向移动
            new Vector2Int(0, 1), new Vector2Int(0, -1)  // 纵向移动
        };

        Debug.Log($"Current Location: {player.position}");

        foreach (Vector2Int direction in directions)
        {
            Vector2Int currentPos = player.position;
            for (int i = 1; i < player.boardSize; i++)
            {
                Vector2Int newPosition = currentPos + direction * i;
                if (!player.IsValidPosition(newPosition))
                {
                    Debug.Log($"Invalid position: {newPosition}");
                    break;
                }
                if (IsBlockedByMonster(newPosition))
                {
                    Debug.Log($"Blocked by monster at position: {newPosition}");
                    break;
                }
                validPositions.Add(newPosition);
            }
        }

        List<Vector2Int> rookDirections = new List<Vector2Int>();
        foreach (var pos in validPositions)
        {
            // 计算每个有效位置相对于玩家位置的偏移量
            Vector2Int relativeDirection = new Vector2Int(pos.x - player.position.x, pos.y - player.position.y);
            rookDirections.Add(relativeDirection);
        }

        player.ShowMoveOptions(rookDirections.ToArray(), card);
    }

    private bool IsBlockedByMonster(Vector2Int position)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Slime slime = monster.GetComponent<Slime>();
            if (slime != null && slime.position == position)
            {
                return true;
            }
        }
        return false;
    }

    public Card GetCard()
    {
        return card;
    }
}
