using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueGroup
{
    public int Length { get { return Dialogues.Length; } }

    public DialogueContent[] Dialogues;

    public DialogueContent this[int index]
    {
        get { return Dialogues[index]; }
        set { Dialogues[index] = value; }
    }
}

[Serializable]
public class DialogueContent 
{
    public int Type;
    public string Talker;
    public string Context;
    public string ImageKey;
    public string BgmKey;
    public string SEKey;
    public DialogueSelectData[] Selects;
}

[Serializable]
public class DialogueSelectData
{
    public int Length { get { return AfterDialogues.Length; } }

    public string Context;
    public int[] VariableDelta;
    public bool IsTrigger;
    public DialogueContent[] AfterDialogues;

    public DialogueContent this[int index]
    {
        get { return AfterDialogues[index]; }
        set { AfterDialogues[index] = value; }
    }
}