using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class bow_card : CardButtonBase
{

    private List<Vector2Int> bowDirections = new List<Vector2Int>();


    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
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

    protected override void OnClick()
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
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null)
            {
                Vector2Int relativePosition = monster.position - player.position;
                monsterPositions.Add(relativePosition);
            }
        }
        return monsterPositions;
    }
}
