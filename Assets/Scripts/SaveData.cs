using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int DayCount;
    public bool SkipTutorial;
    public int[] Variables;
    public List<SelectionLog> DayLog;

    public SaveData()
    {
        Variables = new int[3] { 50, 50, 50 };
        DayLog = new List<SelectionLog>();
    }
}

[Serializable]
public class SelectionLog
{
    public int Day;
    public string SelectedContext;
    public int[] DeltaValue;

    public SelectionLog(int day, string context, int[] deltas)
    {
        Day = day;
        SelectedContext = context;
        DeltaValue = deltas.Clone() as int[];
    }
}