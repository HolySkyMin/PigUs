using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool IsHumanStackFull { get { return Phase1Progress[0] >= 5; } }
    public bool IsPigStackFull { get { return Phase1Progress[1] >= 5; } }

    public int DayCount;
    public int StoryPhase; // 0: neutral (init), 1: human story, 2: pig story, -1: game over @ phase 1, -9: game over @ general
    public int CurrentStoryType;
    public int PhaseChangedDate;
    public int Phase1StoryDay;
    public int[] Phase1Progress;
    public bool SkipTutorial;
    public bool BlendRandomNormal;
    public bool IsGameOver;
    public int[] Variables;
    public List<SelectionLog> DayLog;
    public List<DialogueGroup> RandomNormal;
    public Queue<DialogueGroup> RandomGeneralBag, RandomStoryBag;
    public Queue<List<DialogueGroup>> StoryQueue;
    public Queue RandomQueue;
    public Queue<int> StoryTypeQueue;

    public SaveData()
    {
        Variables = new int[] { 50, 50, 0, 0 };
        Phase1Progress = new[] { 0, 0 };
        DayLog = new List<SelectionLog>();
        RandomNormal = new List<DialogueGroup>();
        RandomGeneralBag = new Queue<DialogueGroup>();
        RandomStoryBag = new Queue<DialogueGroup>();
        StoryQueue = new Queue<List<DialogueGroup>>();
        RandomQueue = new Queue();
        StoryTypeQueue = new Queue<int>();
    }
}

[Serializable]
public class SelectionLog
{
    public int Phase;
    public int Day;
    public string SelectedContext;
    public int[] DeltaValue;

    public SelectionLog(int phase, int day, string context, int[] deltas)
    {
        Phase = phase;
        Day = day;
        SelectedContext = context;
        DeltaValue = deltas.Clone() as int[];
    }
}