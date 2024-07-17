using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public MonsterManager monsterManager;
    public DeckManager deckManager; // 引用DeckManager

    public int turnCount = 0;
    public GameObject turnSlotPrefab;
    public Transform turnPanel;
    public int actions = 3;

    private List<GameObject> turnSlots = new List<GameObject>();
    private int currentActionIndex = 0;

    void Start()
    {
        InitializeTurnPanel();
        deckManager = FindObjectOfType<DeckManager>();

        if (monsterManager == null)
        {
            monsterManager = FindObjectOfType<MonsterManager>();
        }
        
        monsterManager.SpawnSlime();
    }

    void InitializeTurnPanel()
    {
        for (int i = 0; i < actions; i++)
        {
            GameObject turnSlot = Instantiate(turnSlotPrefab, turnPanel);
            turnSlots.Add(turnSlot);
        }

        UpdateCursor();
    }

    public void AdvanceTurn()
    {
        StartCoroutine(HandleTurnEnd());
    }

    private IEnumerator HandleTurnEnd()
    {
        // 回合结束弃牌 (暂时废弃)
        //deckManager.DiscardHand();

        turnCount++;
        monsterManager.OnTurnEnd(turnCount);
        Debug.Log("Turn end");

        // 等待所有史莱姆移动完成
        int slimeCount = monsterManager.GetSlimeCount();
        float delay = slimeCount * 0.5f + 1.0f; // 每个史莱姆移动0.5秒，再额外等待0.5秒
        yield return new WaitForSeconds(delay);

        // 回合结束抓新的手牌
        deckManager.DrawCards(deckManager.handSize-deckManager.hand.Count);

        ResetCursor();
    }

    public void MoveCursor()
    {
        if (currentActionIndex < turnSlots.Count - 1)
        {
            currentActionIndex++;
        }
        else
        {
            currentActionIndex = 0;
        }
        UpdateCursor();
    }

    void ResetCursor()
    {
        currentActionIndex = 0;
        UpdateCursor();
    }

    void UpdateCursor()
    {
        for (int i = 0; i < turnSlots.Count; i++)
        {
            Transform cursorTransform = turnSlots[i].transform.Find("Cursor");
            if (cursorTransform != null)
            {
                Image cursorImage = cursorTransform.GetComponent<Image>();
                if (cursorImage != null)
                {
                    cursorImage.enabled = (i == currentActionIndex);
                    RectTransform cursorRect = cursorTransform.GetComponent<RectTransform>();
                    cursorRect.sizeDelta = new Vector2(40, 40); // Ensure the cursor is 40x40
                }
            }
        }
    }
}
