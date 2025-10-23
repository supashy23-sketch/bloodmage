using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceUI : MonoBehaviour
{
    public Slider expSlider;
    public TMP_Text levelText;

    public void SetMaxExp(int maxExp)
    {
        expSlider.maxValue = maxExp;
    }

    public void SetExp(int currentExp)
    {
        expSlider.value = currentExp;
    }

    public void SetLevelText(int level)
    {
        levelText.text = "Lv. " + level.ToString();
    }
}
