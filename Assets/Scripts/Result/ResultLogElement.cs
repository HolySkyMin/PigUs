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
        public CanvasGroup FoodPanel, HealthPanel, HumanPanel, PigPanel;

        public void Set(int phase, int day, string context, int[] variables)
        {
            if (phase == 0)
                ColorRepresent.color = Color.black;
            else if (phase == 1)
                ColorRepresent.color = Color.cyan;
            else if (phase == 2)
                ColorRepresent.color = Color.yellow;
            else
                ColorRepresent.color = Color.red;

            Day.text = $"DAY {day}";
            Context.text = context;
            
            if(variables[0] != 0)
            {
                FoodPanel.alpha = 1;
                Food.text = $"{(variables[0] >= 0 ? "+" : "")}{variables[0]}";
            }
            if(variables[1] != 0)
            {
                HealthPanel.alpha = 1;
                Health.text = $"{(variables[1] >= 0 ? "+" : "")}{variables[1]}";
            }
            if(variables[2] != 0)
            {
                HumanPanel.alpha = 1;
                HumanFav.text = $"+{variables[2]}";
            }
            if(variables[3] != 0)
            {
                PigPanel.alpha = 1;
                PigFav.text = $"+{variables[3]}";
            }
        }
    }
}