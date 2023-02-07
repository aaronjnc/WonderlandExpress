using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveInfo
{
    int loops;
    int gold;
    int totalGold;
    int maxCap;
    int numCar;
    string currentStop;
    float[] trainPosition = new float[3];
    float[] trainRotation = new float[3];
    float[] trainLookRotation = new float[4];
    float happinessDecayRate;
    int tollPrice;
    int jabberwockyPrice;
    float[][] followCarPositions = new float[1][];
    float[][] followCarRotations = new float[1][];
    float[][] currentStops = new float[1][];
    float[][] followLinkedList = new float[1][];
    bool useMouthNoises = false;
    int carCount;
    int carLevel;
    PassengerSave[] passengers = new PassengerSave[1];
    TownSave[] towns = new TownSave[25];
    UniquePassengerSave upiSave;
    
    public void SaveGame(GameManager manager)
    {
        loops = manager.loops;
        gold = manager.GetGold();
        totalGold = manager.GetTotalGold();
        maxCap = manager.maxCap;
        numCar = manager.numCar;
        passengers = manager.GetPassengerSaves();
        currentStop = TrainMovement.Instance.nextPoint.name;
        Vector3 trainPos = manager.GetTrainPosition();
        trainPosition = new float[] {trainPos.x, trainPos.y, trainPos.z};
        Vector3 trainRot = manager.GetTrainRotation();
        trainRotation = new float[] {trainRot.x, trainRot.y, trainRot.z };
        Quaternion trainLookRot = manager.GetTrainLookRotation();
        trainLookRotation = new float[] {trainLookRot.x, trainLookRot.y, trainLookRot.z, trainLookRot.w };
        happinessDecayRate = manager.happinessDecayRate;
        tollPrice = manager.tollPrice;
        jabberwockyPrice = manager.GetJabberwockyPrice();
        List<Vector3> trainCarPositions = manager.GetFollowCarPositions();
        List<Vector3> trainCarRotations = manager.GetFollowCarRotations();
        List<FollowPoint> trainCarFollowPoint = manager.GetTrainFollowPoints();
        followCarPositions = new float[trainCarPositions.Count][];
        followCarRotations = new float[trainCarRotations.Count][];
        currentStops = new float[trainCarFollowPoint.Count][];
        for (int i = 0; i < trainCarPositions.Count && i < trainCarRotations.Count && i < trainCarFollowPoint.Count; i++)
        {
            followCarPositions[i] = new float[] {trainCarPositions[i].x, trainCarPositions[i].y, trainCarPositions[i].z };
            followCarRotations[i] = new float[] {trainCarRotations[i].x, trainCarRotations[i].y, trainCarRotations[i].z };
            Vector3 pos = trainCarFollowPoint[i].GetPos();
            currentStops[i] = new float[] {pos.x, pos.y, pos.z, i };
        }
        FollowPoint head = manager.GetHeadPoint();
        List<float[]> followPoints = new List<float[]>();
        while (head != null)
        {
            Vector3 pos = head.GetPos();
            followPoints.Add(new float[] {pos.x, pos.y, pos.z });
            head = head.GetNext();
        }
        followLinkedList = followPoints.ToArray();
        useMouthNoises = manager.mouthNoises;
        carCount = manager.carCount;
        carLevel = manager.carLevel;
        towns = TownDictionary.Instance.GetTownSaves();
        if (manager.IsRegular())
        {
            upiSave = new UniquePassengerSave();
            upiSave.Setup(manager.GetRegular());
        }
        SerializeClass();
    }

    void SerializeClass()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveInfo.txt");
        bf.Serialize(file, this);
        file.Close();
    }

    public void LoadGame(GameManager manager)
    {
        manager.loops = loops;
        manager.gold = gold;
        manager.totalGold = totalGold;
        manager.carCount = carCount;
        manager.carLevel = carLevel;
        manager.passengers = new GameObject[maxCap * carCount];
        manager.SetCurrentStop(currentStop);
        manager.SetTrainPosition(new Vector3(trainPosition[0], trainPosition[1], trainPosition[2]));
        manager.SetTrainRotation(new Vector3(trainRotation[0], trainRotation[1], trainRotation[2]));
        manager.SetTrainLookRotation(new Quaternion(trainLookRotation[0], trainLookRotation[1], trainLookRotation[2], trainLookRotation[3]));
        manager.happinessDecayRate = happinessDecayRate;
        manager.tollPrice = tollPrice;
        manager.SetJabberwockyPrice(jabberwockyPrice);
        Dictionary<Vector3, List<int>> nextLocation = new Dictionary<Vector3, List<int>>();
        for (int i = 0; i < followCarPositions.Length; i++)
        {
            manager.AddTrainCarPositionRotation(new Vector3(followCarPositions[i][0], followCarPositions[i][1], followCarPositions[i][2]), new Vector3(followCarRotations[i][0], followCarRotations[i][1], followCarRotations[i][2]));
            Vector3 pos = new Vector3(currentStops[i][0], currentStops[i][1], currentStops[i][2]);
            if (nextLocation.ContainsKey(pos))
            {
                nextLocation[pos].Add(i);
            }
            else
            {
                nextLocation.Add(pos, new List<int> { i });
            }
        }
        manager.SetMouthNoises(useMouthNoises);

        for (int i = 0; i < followLinkedList.Length; i++)
        {
            Vector3 pos = new Vector3(followLinkedList[i][0], followLinkedList[i][1], followLinkedList[i][2]);
            manager.AddFollowPoint(pos);
            if (nextLocation.ContainsKey(pos))
            {
                FollowPoint tail = manager.GetTailPoint();
                foreach (int idx in nextLocation[pos]) {
                    manager.AddTrainCarStop(tail, idx);
                } 
            }
        }
        TownDictionary townDict = TownDictionary.Instance;
        foreach(TownSave t in towns)
        {
            townDict.UpdateTown(t.townName, t);
        }
        if (upiSave != null)
            manager.InitializeUPI(upiSave);
        foreach (PassengerSave p in passengers)
        {
            if (p != null)
                manager.SpawnPassenger(p);
        }
    }
}
