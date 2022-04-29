using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownDictionary : MonoBehaviour
{
    [Tooltip("List of all town names \nMust have the same order as towns")]
    public List<string> townNames = new List<string>();
    [Tooltip("List of all towns \nMust have the same order as townNames")]
    public List<Town> towns = new List<Town>();
    //the dictionary to be used.
    public Dictionary<string, Town> dict = new Dictionary<string, Town>();
    //list of all destroyed towns
    public List<Town> destroyed = new List<Town>();

    [Tooltip("the current town")]
    public Town currentTown;

    [Tooltip("The total track length. Found by subtracting the distance of the last town from that of the first town and adding 1")]
    public int maxLength = 13;

    [Tooltip("Current Town Dictionary")]
    private static TownDictionary _instance;

    public static TownDictionary Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TownDictionary>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<TownDictionary>();
                }
            }
            return _instance;
        }
    }

    //writes values from dictionary to two lists for things like the Inspector -- test. Don't use yet
    //void OnBeforeSerialize()
    //{
    //    townNames.Clear();
    //    towns.Clear();
    //    foreach(var entry in dict)
    //     {
    //         townNames.Add(entry.Key);
    //        towns.Add(entry.Value);
    //    }
    //}

    //writes values to dictionary from the two lists to allow use of the inspector -- test. Don't use yet
    //void OnAfterDeserialize()
    //{
    //    dict = new Dictionary<string, Town>();
    //    for(int i = 0; i < townNames.Count && i < towns.Count; i++)
    //    {
    //        dict.Add(townNames[i], towns[i]);
    //    }
    //}

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(this.gameObject);
            Destroy(this);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoad;
        for (int i = 0; i < townNames.Count && i < towns.Count; i++)
        {
            dict.Add(townNames[i], towns[i]);
            //DontDestroyOnLoad(towns[i]);

        }
        Debug.Log(dict.Count);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //gets the currently stored town
    public Town GetCurrentTown()
    {
        return currentTown;
    }

    //find the associated town, sets it as the current town, and returns it
    public Town FindCurrentTown(string townName)
    {
        if(dict.TryGetValue(townName, out currentTown))
        {
            return currentTown;
        }
        else
        {
            Debug.LogError("Could not find town " + townName);
            return null;
        }
    }

    //get the associated town
    public Town FindTown(string townName)
    {
        Town town;
        if (dict.TryGetValue(townName, out town))
        {
            return town;
        }
        else
        {
            Debug.LogError("Could not find town " + townName);
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Generates a random destination town from all towns available to the current town
    public Town GenerateDestination()
    {
        List<Town> allowedList = currentTown.GetAllowedList();
        Town dest = allowedList[UnityEngine.Random.Range(0, allowedList.Count)];
        while (dest.IsDestroyed() && allowedList.Count > 1)
        {
            allowedList.Remove(dest);
            dest = allowedList[UnityEngine.Random.Range(0, allowedList.Count)];
        }
        return dest;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(this.gameObject);
        }
    }

    //Destroys the town and adds it to the destroyed list
    public void DestroyTown(string town)
    {
        Town t = FindTown(town);
        t.DestroyTown();
        destroyed.Add(t);
    }

    //Returns the distance between the two provided towns
    public int GetTownDist(string town1, string town2)
    {
        return Mathf.Abs((FindTown(town1).GetLoc() % maxLength) - (FindTown(town2).GetLoc() % maxLength));
    }

    //returns the string name corresponding to the given index
    public string GetTownNameByIndex(int index)
    {
        return townNames[index];
    }
}
