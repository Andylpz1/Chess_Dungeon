using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }
    private HashSet<string> unlockedCards = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadCollection();
    }

    public void UnlockCard(string cardId)
    {
        if (!unlockedCards.Contains(cardId))
        {
            unlockedCards.Add(cardId);
            Debug.Log($"Card Unlocked: {cardId}");
            SaveCollection();
        }
    }

    public bool IsCardUnlocked(string cardId)
    {
        return unlockedCards.Contains(cardId);
    }

    void SaveCollection()
    {
        CollectionData collectionData = new CollectionData();
        collectionData.unlockedCardIds = new List<string>(unlockedCards);
        SaveSystem.SaveCollection(collectionData);
    }

    void LoadCollection()
    {
        CollectionData collectionData = SaveSystem.LoadCollection();
        unlockedCards = new HashSet<string>(collectionData.unlockedCardIds);
    }
}
