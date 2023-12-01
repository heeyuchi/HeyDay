using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BudgetSetter : MonoBehaviour
{
    [SerializeField] private List<Slider> sliders = new List<Slider>();
    [SerializeField] private List<Slider> recommSliders = new List<Slider>();
    [SerializeField] private List<TextMeshProUGUI> amountTexts = new List<TextMeshProUGUI>();
    [SerializeField] private TextMeshProUGUI playerCurrentMoneyText;
    private const float BillsRecommPercentage = 0.5f;
    private const float SavingsRecommPercentage = 0.1f;
    private const float ConsumablesRecommPercentage = 0.3f;
    private const float EmergencyRecommPercentage = 0.1f;
    private float[] oldVals = new float[4];
    private float moneyValue;


    public void PrepareBudgeSetter(float currentPlayerMoney)
    {
        this.gameObject.SetActive(true);

        playerCurrentMoneyText.text = currentPlayerMoney.ToString();
        moneyValue = currentPlayerMoney;

        for(int i = 0; i < sliders.Count; i++)
        {
            oldVals[i] = 0f;
            sliders[i].maxValue = currentPlayerMoney;
            sliders[i].value = 0f;
            recommSliders[i].maxValue = currentPlayerMoney;
            amountTexts[i].text = sliders[i].value.ToString("0");

            switch (i)
            {
                case 0:
                    recommSliders[i].value = currentPlayerMoney * BillsRecommPercentage;
                    break;
                case 1:
                    recommSliders[i].value = currentPlayerMoney * SavingsRecommPercentage;
                    break;
                case 2:
                    recommSliders[i].value = currentPlayerMoney * ConsumablesRecommPercentage;
                    break;
                case 3:
                    recommSliders[i].value = currentPlayerMoney * EmergencyRecommPercentage;
                    break;
            }
        }

        //bills
        sliders[0].onValueChanged.AddListener((v) => { UpdateSlider(0, v); });

        //savings
        sliders[1].onValueChanged.AddListener((v) => { UpdateSlider(1, v); });

        //consumables
        sliders[2].onValueChanged.AddListener((v) => { UpdateSlider(2, v); });

        //emergency
        sliders[3].onValueChanged.AddListener((v) => { UpdateSlider(3, v); });
    }


    private void UpdateSlider(int sliderIndex, float value)
    {
        if (value > GetAmountLeft(sliderIndex))
        {
            sliders[sliderIndex].value = oldVals[sliderIndex];
        }
        else
        {
            sliders[sliderIndex].value = value;
        }

        float newVal = sliders[sliderIndex].value;
        amountTexts[sliderIndex].text = newVal.ToString("0");
        oldVals[sliderIndex] = sliders[sliderIndex].value;
    }


    private float GetAmountLeft(int index)
    {
        switch (index)
        {
            case 0:
                return (moneyValue - (sliders[1].value + sliders[2].value + sliders[3].value));
            case 1:
                return (moneyValue - (sliders[0].value + sliders[2].value + sliders[3].value));
            case 2:
                return (moneyValue - (sliders[0].value + sliders[1].value + sliders[3].value));
            case 3:
                return (moneyValue - (sliders[0].value + sliders[1].value + sliders[2].value));
            default:
                return 0;
        }
    }


    public void SaveBudget()
    {
        BudgetSystem.Instance.SaveBudget(sliders[0].value, sliders[1].value, sliders[2].value, sliders[3].value);
    }
}