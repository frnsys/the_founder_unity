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

public enum Economy {
    Depression,
    Recession,
    Neutral,
    Expansion
}

[System.Serializable]
public class GameData : ScriptableObject {

    // ===============================================
    // Data ==========================================
    // ===============================================

    public Company company;
    public TheBoard board;
    public UnlockSet unlocked;

    // "World" variables.
    public Economy economy;
    public float spendingMultiplier;
    public float wageMultiplier;
    public float forgettingRate;
    public float economicStability;
    public float taxRate;
    public float expansionCostMultiplier;
    public float costMultiplier;

    // Onboarding progress.
    public NarrativeManager.OnboardingState ob;

    // Other companies in the world.
    public List<AAICompany> otherCompanies;

    // Time
    public Month month;
    public int week;
    public int year;

    // How many years and months you have to play.
    public int lifetimeYear;
    public int lifetimeMonth;
    public int lifetimeWeek;

    // The canonical pool of workers not at companies.
    public List<AWorker> unemployed;

    // Events which are waiting to resolve.
    public List<AGameEvent> specialEventsPool;
    public List<AGameEvent> eventsPool;

    // Special effects.
    public bool immortal;
    public bool cloneable;
    public bool prescient;
    public bool automation;
    public bool workerInsight;
    public bool workerQuant;

    // ===============================================
    // Management ====================================
    // ===============================================

    void Start() {
        // Initialize editable stuff for the inspector.
        unlocked       = new UnlockSet();
        otherCompanies = new List<AAICompany>();
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
        data.unemployed = Worker.LoadAll();
        data.otherCompanies = AICompany.LoadAll();

        data.economy = Economy.Neutral;
        data.spendingMultiplier = 1f;
        data.wageMultiplier = 1f;
        data.forgettingRate = 1f;
        data.economicStability = 1f;
        data.expansionCostMultiplier = 1f;
        data.taxRate = 0.3f;
        data.costMultiplier = 1f;

        data.specialEventsPool = GameEvent.LoadSpecialEvents();
        data.eventsPool = new List<AGameEvent>();

        data.month = Month.January;
        data.year  = 1;
        data.week  = 0;

        data.immortal = false;
        data.cloneable = false;
        data.prescient = false;
        data.automation = false;
        data.workerInsight = false;
        data.workerQuant = false;

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
        FileStream file = File.Create(savePath);
        try {
            Serializer.Replica data = Serializer.Serialize(gd);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
        } finally {
            file.Close();
        }
    }

    public static GameData Load() {
        return Load(SavePath);
    }
    public static GameData Load(string savePath) {
        GameData gd = null;
        if(SaveExists) {
            Debug.Log("Creating file stream");
            FileStream file = File.Open(savePath, FileMode.Open);
            try {
                Debug.Log("Trying to load game");
                BinaryFormatter bf = new BinaryFormatter();

                Debug.Log("Deserializing replica");
                Serializer.Replica data = (Serializer.Replica)bf.Deserialize(file);

                Debug.Log("Deserializing game data");
                gd = Serializer.Deserialize<GameData>(data);

                Debug.Log("Closing file");
            } finally {
                file.Close();
            }
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
