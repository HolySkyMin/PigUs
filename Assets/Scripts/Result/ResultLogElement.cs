using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Result
{
    public class ResultLogElement : MonoBehaviour
    {
        public Text Day, Context, Food, Health, Favority;

        public void Set(int day, string context, int[] variables)
        {
            Day.text = $"DAY {day}";
            Context.text = context;
            Food.text = $"{(variables[0] >= 0 ? "<color=lime>+" : "<color=red>")}{variables[0]}</color>";
            if (variables[0] == 0)
                Food.text = "-";
            Health.text = $"{(variables[1] >= 0 ? "<color=lime>+" : "<color=red>")}{variables[1]}</color>";
            if (variables[1] == 0)
                Health.text = "-";
            Favority.text = $"{(variables[2] >= 0 ? "<color=cyan>>>" : "<color=orange><<")}{Mathf.Abs(variables[2])}</color>";
            if (variables[2] == 0)
                Favority.text = "-";
        }
    }
}