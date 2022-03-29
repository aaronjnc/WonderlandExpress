using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownDictionary : MonoBehaviour
{
    [Tooltip("List of all town names \nMust have the same order as towns")]
    public List<string> townNames = new List<string>();
    [Tooltip("List of all towns \nMust have the same order as townNames")]
    public List<Town> towns = new List<Town>();
    //the dictionary to be used.
    public Dictionary<string, Town> dict = new Dictionary<string, Town>();

    [Tooltip("the current town")]
    public Town currentTown;

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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < townNames.Count && i < towns.Count; i++)
        {
            dict.Add(townNames[i], towns[i]);

        }
        Debug.Log(dict.Count);
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

    // Update is called once per frame
    void Update()
    {
        
    }

    //Generates a random destination town from all towns available to the current town
    public Town GenerateDestination()
    {
        List<Town> banList = currentTown.getBanList();
        if(banList == null)
        {
            banList = new List<Town>();
        }
        banList.Add(currentTown);
        List<Town> validTowns = new List<Town>();
        foreach(Town town in dict.Values)
        {
            if (!banList.Contains(town))
            {
                validTowns.Add(town);
            }
        }
        return validTowns[UnityEngine.Random.Range(0, validTowns.Count)];
    }
}