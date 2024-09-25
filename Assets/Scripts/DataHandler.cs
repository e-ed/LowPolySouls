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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        filePath = Application.persistentDataPath + "/playerData.txt";
    }

    public void SaveData(PlayerData data)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is null or empty");
            return;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to " + filePath);
        Debug.Log("Data saved:" + json);
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

            PlayerData defaultData = new PlayerData
            {
                level = 1,
                souls = 0,
                strength = 5,
                dexterity = 5,
                intelligence = 5,
                positionX = 2,
                positionY = 2,
                positionZ = 107
            };

            string json = JsonUtility.ToJson(defaultData, true);

            File.WriteAllText(filePath, json);
            Debug.Log("Default data created and saved to " + filePath);

            return defaultData;
        }
    }
}
