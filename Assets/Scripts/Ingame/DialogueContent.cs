using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class DialogueContent 
{
    public int Type;
    public string Talker;
    public string Context;
    public string ImageKey;
    public DialogueSelectData[] Selects;
}

public class DialogueSelectData
{
    public int Length { get { return AfterDialogues.Length; } }

    public string Context;
    public int[] VariableType;
    public int[] VariableDelta;
    public DialogueContent[] AfterDialogues;

    public DialogueContent this[int index]
    {
        get { return AfterDialogues[index]; }
        set { AfterDialogues[index] = value; }
    }
}