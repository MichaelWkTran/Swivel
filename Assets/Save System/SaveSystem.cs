using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

//Note that this system is only loading and saving data to m_data and does not interact with external information
public static class SaveSystem
{
    const string m_fileName = "/Save.json";//"Save.bb";

    [Serializable] public class SaveData
    {
        //Game Variables
        public float m_money;

        public string m_currentEnviromentGUID;
        public string m_currentSpriteGroupGUID;
        public string m_currentUIThemeGUID;
        
        public uint m_unlockedLevels; //How many levels have the player currently unlocked
        public List<string> m_unlockedEnviromentGUIDs = new List<string>();
        public List<string> m_unlockedSpriteGroupGUIDs = new List<string>();
        public List<string> m_unlockedUIThemeGUIDs = new List<string>();

        public float[] m_highScores = new float[7]; //Each index in the vector represents a level from which the highscore belongs to.

        //Settings
        public float m_sfxVolume;
        public float m_musicVolume;
        public int m_qualityLevel;
    }
    static SaveData m_data;
    public static SaveData m_Data
    {
        get
        {
            //Initilize and load data if none currently exists
            if (m_data == null)
            {
                m_data = new SaveData();
                Load();
            }

            return m_data;
        }
    }

    //Saves the data from m_data to the file, Application.persistentDataPath + m_fileName
    public static void Save()
    {
        if (m_data == null) return;
        
        //Get the file directory of the save data
        string path = Application.persistentDataPath + m_fileName;
        
        //Load from Json
        string saveDataJson = JsonUtility.ToJson(m_data);
        File.WriteAllText(path, saveDataJson);

        ////Create and open file stream
        //FileStream stream = new FileStream(path, FileMode.Create);
        
        ////Create binary formatter and serialize the game data
        //BinaryFormatter formatter = new BinaryFormatter();
        //formatter.Serialize(stream, m_data);
        
        ////Close the file stream
        //stream.Close();
    }

    //Loads the data stored in Application.persistentDataPath + m_fileName to m_data
    public static void Load()
    {
        //Ensure that the data is initialized before loading
        if (m_data == null) m_data = new SaveData();
        
        //Get the file directory of the save data
        string path = Application.persistentDataPath + m_fileName;
        
        //Check whether the file in the searched path exists, if not then exit the function
        if (!File.Exists(path)) { Debug.LogError("Save file not found in " + path); return; }
        
        //Load to Json
        string saveDataJson = File.ReadAllText(path);
        m_data = JsonUtility.FromJson<SaveData>(saveDataJson);
        
        ////Create and open file stream
        //FileStream stream = new FileStream(path, FileMode.Open);
        
        ////Create binary formatter and deserialize the game data
        //BinaryFormatter formatter = new BinaryFormatter();
        //m_data = formatter.Deserialize(stream) as SaveData;
        
        ////Close the file stream
        //stream.Close();
    }
}