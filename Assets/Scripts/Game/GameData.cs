/*
 * Stores all game data that would need to be persisted.
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public enum Month {
    January,
    February,
    March,
    April,
    May,
    June,
    July,
    August,
    September,
    October,
    November,
    December
}

[System.Serializable]
public class GameData : ScriptableObject {

    // ===============================================
    // Data ==========================================
    // ===============================================

    public Company company;

    public TheBoard board;

    public UnlockSet unlocked;

    // Research stuff.
    public Technology technology;
    public float research;

    // Other companies in the world.
    public List<AICompany> otherCompanies;

    // Time
    public Month month;
    public int week;
    public int year;

    // How many years and months you have to play.
    public int lifetimeYear;
    public int lifetimeMonth;
    public int lifetimeWeek;

    // Economy health
    public float economyMultiplier;


    // ===============================================
    // Management ====================================
    // ===============================================

    void Start() {
        // Initialize editable stuff for the inspector.
        unlocked       = new UnlockSet();
        otherCompanies = new List<AICompany>();
    }
    public static GameData New(string companyName) {
        Debug.Log("Creating new game data!");

        GameData data;

        // Try to load starting/default game data.
        data = Instantiate(Resources.Load<GameData>("Starting Data")) as GameData;
        if (data == null) {
            data = ScriptableObject.CreateInstance<GameData>();
            data.unlocked = new UnlockSet();
            data.otherCompanies = new List<AICompany>();
        }

        // Initialize new game stuff.
        data.company  = new Company(companyName);
        data.board    = new TheBoard();
        data.research = 0;

        data.month = Month.January;
        data.year  = 1;
        data.week  = 0;

        data.economyMultiplier = 1f;

        // You start your business at 25,
        // so the amount of time you have really ranges from 40-60.
        float lifetime = Random.Range(65f, 85f) - 25;

        // Translate the lifetime into a year, month, & week.
        data.lifetimeYear = (int)lifetime;

        float month_ = (lifetime - data.lifetimeYear) * 12;
        data.lifetimeMonth = (int)month_;

        data.lifetimeWeek = (int)((month_ - data.lifetimeMonth) * 4);

        return data;
    }

    public static void Save(GameData gd) {
        Save(gd, SavePath);
    }
    public static void Save(GameData gd, string savePath) {
        Debug.Log("Saving game...");
        Serializer.Replica data = Serializer.Serialize(gd);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, data);
        file.Close();
    }

    public static GameData Load() {
        return Load(SavePath);
    }
    public static GameData Load(string savePath) {
        GameData gd = null;
        if(SaveExists) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);

            Serializer.Replica data = (Serializer.Replica)bf.Deserialize(file);
            gd = Serializer.Deserialize<GameData>(data);
            file.Close();
        }
        return gd;
    }

    private static string SavePath {
        get { return Application.persistentDataPath + "/founders_save.dat"; }
    }

    public static bool SaveExists {
        get { return File.Exists(SavePath); }
    }
}
