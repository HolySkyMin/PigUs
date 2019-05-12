using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Result
{
    public class ResultLogElement : MonoBehaviour
    {
        public Graphic ColorRepresent;
        public Text Day, Context, Food, Health, HumanFav, PigFav;
        public CanvasGroup HumanPanel, PigPanel;

        public void Set(int phase, int day, string context, int[] variables)
        {
            if (phase == 0)
                ColorRepresent.color = Color.white;
            else if (phase == 1)
                ColorRepresent.color = Color.cyan;
            else if (phase == 2)
                ColorRepresent.color = Color.yellow;
            else
                ColorRepresent.color = Color.red;

            Day.text = $"DAY {day}";
            Context.text = context;
            Food.text = $"{(variables[0] >= 0 ? "<color=lime>+" : "<color=red>")}{variables[0]}</color>";
            if (variables[0] == 0)
                Food.text = "-";
            Health.text = $"{(variables[1] >= 0 ? "<color=lime>+" : "<color=red>")}{variables[1]}</color>";
            if (variables[1] == 0)
                Health.text = "-";
            
            if(variables[2] != 0)
            {
                HumanPanel.alpha = 1;
                HumanFav.text = $"<color=cyan>+{variables[2]}</color>";
            }
            if(variables[3] != 0)
            {
                PigPanel.alpha = 1;
                PigFav.text = $"<color=yellow>+{variables[3]}</color>";
            }
        }
    }
}