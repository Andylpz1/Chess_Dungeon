using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string gameSavePath = Application.persistentDataPath + "/gameSave.json";
    private static string collectionSavePath = Application.persistentDataPath + "/collectionSave.json";

    // **游戏进度存档（关卡、卡组、血量）**
    public static void SaveGame(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(gameSavePath, json);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(gameSavePath))
        {
            string json = File.ReadAllText(gameSavePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    public static bool GameSaveExists()
    {
        return File.Exists(gameSavePath);
    }

    // **收集品存档（已解锁卡牌）**
    public static void SaveCollection(CollectionData collectionData)
    {
        string json = JsonUtility.ToJson(collectionData);
        File.WriteAllText(collectionSavePath, json);
    }

    public static CollectionData LoadCollection()
    {
        if (File.Exists(collectionSavePath))
        {
            string json = File.ReadAllText(collectionSavePath);
            return JsonUtility.FromJson<CollectionData>(json);
        }
        return new CollectionData(); // 返回空收集品数据
    }

    public static bool CollectionSaveExists()
    {
        return File.Exists(collectionSavePath);
    }

    public static void DeleteSaveFile()
    {
        if (File.Exists(gameSavePath))
        {
            File.Delete(gameSavePath);
            Debug.Log("Game save file deleted.");
        }
        else
        {
            Debug.LogWarning("No game save file found to delete.");
        }
    }

    public static void DeleteCollectionFile()
    {
        if (File.Exists(collectionSavePath))
        {
            File.Delete(collectionSavePath);
            Debug.Log("Collection save file deleted.");
        }
        else
        {
            Debug.LogWarning("No collection save file found to delete.");
        }
    }

}
