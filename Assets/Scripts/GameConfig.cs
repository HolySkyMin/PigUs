using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameConfig
{
    public int CPS;
    public bool SkipTutorial;
    public bool AllowFullscreen;
    public Dictionary<int, bool> EndingChecklist;

    public GameConfig()
    {
        CPS = 59;
        SkipTutorial = false;
        AllowFullscreen = false;
        EndingChecklist = new Dictionary<int, bool>()
        {
            { 1, false },
            { 2, false },
            { 3, false },
            { 4, false },
            { 5, false },
            { 6, false },
            { 7, false },
            { 8, false },
            { 9, false },
            { 10, false },
            { 11, false },
            { 12, false },
            { 13, false },
            { 14, false },
            { 15, false },
            { 16, false },
            { 17, false },
            { 18, false },
            { 19, false },
            { 20, false }
        };
    }
}
