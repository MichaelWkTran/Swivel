using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

//Note that this system is only loading and saving data to m_data and does not interact with external information
public static class SaveSystem
{
    const string m_fileName = "/Save.bb";

    [Serializable] public class SaveData
    {
        public SaveData() { Load(); }

        //Game Variables
        public float m_money;

        public string m_currentEnviromentGUID;
        public string m_currentUIThemeGUID;
        
        public uint m_unlockedLevels;
        public List<string> m_unlockedEnviromentGUIDs;
        public List<string> m_unlockedUIThemeGUIDs;

        //Settings
        public float m_sfxVolume;
        public float m_musicVolume;
        public int m_qualityLevel;
    }
    public static SaveData m_data = new SaveData();

    //Saves the data from m_data to the file, Application.persistentDataPath + m_fileName
    public static void Save()
    {
        //Get the file directory of the save data
        string path = Application.persistentDataPath + m_fileName;
        
        //Create and open file stream
        FileStream stream = new FileStream(path, FileMode.Create);

        //Create binary formatter and serialize the game data
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, m_data);

        //Close the file stream
        stream.Close();
    }

    //Loads the data stored in Application.persistentDataPath + m_fileName to m_data
    public static void Load()
    {
        //Get the file directory of the save data
        string path = Application.persistentDataPath + m_fileName;

        //Check whether the file in the searched path exists, if not then exit the function
        if (!File.Exists(path)) { Debug.LogError("Save file not found in " + path); return; }

        //Create and open file stream
        FileStream stream = new FileStream(path, FileMode.Open);

        //Create binary formatter and deserialize the game data
        BinaryFormatter formatter = new BinaryFormatter();
        m_data = formatter.Deserialize(stream) as SaveData;

        //Close the file stream
        stream.Close();
    }
}