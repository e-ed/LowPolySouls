using System.IO;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    private static DataHandler instance;
    private string filePath;

    public static DataHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataHandler>();
                if (instance == null)
                {
                    GameObject go = new GameObject("DataHandler");
                    instance = go.AddComponent<DataHandler>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make it persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        filePath = Application.persistentDataPath + "/playerData.txt";
    }

    public void SaveData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to " + filePath);
    }

    public PlayerData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Data loaded from " + filePath);
            return data;
        }
        else
        {
            Debug.LogError("No save file found!");
            return null;
        }
    }
}
