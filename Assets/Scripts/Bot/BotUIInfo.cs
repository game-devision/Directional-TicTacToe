using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotUIInfo : MonoBehaviour
{
    public BotAI2 bot;
    public Slider OffenseSlider;
    public Slider DefenseSlider;
    public Slider RandomSlider;
    public TMP_Text OffenseValue;
    public TMP_Text DefenseValue;
    public TMP_Text RandomValue;
    public TMP_Dropdown BotTeam;
    public GameObject botInfo;

    public void OnSliderValueChange()
    {
        OffenseValue.text = roundFloat(OffenseSlider.value);
        bot.Offensiveness = OffenseSlider.value;

        DefenseValue.text = roundFloat(DefenseSlider.value);
        bot.Defensiveness = DefenseSlider.value;

        RandomValue.text = roundFloat(RandomSlider.value);
        bot.Randomness = RandomSlider.value;
    }
    public void ToggleBot()
    {
        BotAI2.botEnabled = !BotAI2.botEnabled;
    }
    public void ChangeBotTeam()
    {
        if(BotTeam.value == 0)
        {
            bot.type = SpaceState.O;
        }
        else
        {
            bot.type = SpaceState.X;
        }
    }
    private string roundFloat(float input)
    {
        string outputFloat = (Mathf.Round(input * 100)/100).ToString();
        if(outputFloat.Length == 1)
        {
            outputFloat += ".00";
        }
        else if(outputFloat.Length == 3)
        {
            outputFloat += "0";
        }
        return outputFloat;
    }
    private void Update()
    {
        if(GameController.Instance.GameStarted == true)
        {
            botInfo.SetActive(false);
        }
        else
        {
            botInfo.SetActive(true);
        }
    }
}
