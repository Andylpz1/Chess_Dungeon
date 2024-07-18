using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class bow_card : MonoBehaviour, CardButton, IPointerEnterHandler, IPointerExitHandler
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    public Player player;
    private Text buttonText;
    private List<Vector2Int> bowDirections = new List<Vector2Int>();

    public HintManager hintManager; // 引用HintManager

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        if (hintManager == null)
        {
            Debug.LogError("HintManager not found in the scene.");
        }

        InitializeBowDirections();
    }

    public void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;
        player = FindObjectOfType<Player>();

        if (buttonText != null)
        {
            //buttonText.text = "Bow";
        }

        if (button != null)
        {
            button.onClick.AddListener(() => OnClick());
        }
    }

    private void InitializeBowDirections()
    {
        int boardSize = player.boardSize;

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (x != player.position.x || y != player.position.y) // Exclude player's current position
                {
                    Vector2Int direction = new Vector2Int(x - player.position.x, y - player.position.y);
                    bowDirections.Add(direction);
                }
            }
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
                List<Vector2Int> validBowDirections = GetMonsterPositions();
                player.ShowAttackOptions(validBowDirections.ToArray(), card);
            }
        }
        else
        {
            Debug.LogError("Card is null in bow_card.OnClick");
        }
    }

    private List<Vector2Int> GetMonsterPositions()
    {
        List<Vector2Int> monsterPositions = new List<Vector2Int>();
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Slime slime = monster.GetComponent<Slime>();
            if (slime != null)
            {
                Vector2Int relativePosition = slime.position - player.position;
                monsterPositions.Add(relativePosition);
            }
        }
        return monsterPositions;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("攻击地图上的任意位置", transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.HideHint();
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
