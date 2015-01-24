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

public enum WorkerInsight {
    Basic,
    Fuzzy,
    Quant,
    Detailed,
    Precise
}

[System.Serializable]
public class GameData : ScriptableObject {

    // ===============================================
    // Data ==========================================
    // ===============================================

    public Company company;
    public TheBoard board;
    public UnlockSet unlocked;

    // Onboarding progress.
    public OnboardingState onboardingState;

    // The state of various universal effects.
    public WorkerInsight workerInsight;

    // Maximum product types that can be combined.
    // Initially this is just 1.
    public int maxProductTypes;

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

    // The canonical pool of workers not at companies.
    public List<Worker> unemployed;

    // Events which are waiting to resolve.
    public List<GameEvent> specialEventsPool;
    public List<GameEvent> eventsPool;

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
        }

        // Initialize new game stuff.
        data.company  = new Company(companyName).Init();
        data.board    = new TheBoard();
        data.research = 0;
        data.unemployed = Worker.LoadAll();
        data.otherCompanies = AICompany.LoadAll();

        data.specialEventsPool = GameEvent.LoadSpecialEvents();
        data.eventsPool = new List<GameEvent>();

        data.workerInsight = WorkerInsight.Basic;

        data.month = Month.January;
        data.year  = 1;
        data.week  = 0;

        data.maxProductTypes   = 1;
        data.economyMultiplier = 1;

        // You start your business at 25,
        // so the amount of time you have really ranges from 40-60.
        float lifetime = Random.Range(65f, 85f) - 25;

        // Translate the lifetime into a year, month, & week.
        data.lifetimeYear = (int)lifetime;

        float month_ = (lifetime - data.lifetimeYear) * 12;
        data.lifetimeMonth = (int)month_;

        data.lifetimeWeek = (int)((month_ - data.lifetimeMonth) * 4);

        // The starting location is San Francisco.
        Location startingLocation = Location.Load("San Francisco");
        startingLocation.cost = 0;
        data.company.ExpandToLocation(startingLocation);

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
