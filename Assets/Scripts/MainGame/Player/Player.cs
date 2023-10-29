using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum PlayerStats
{
    ALL,
    HAPPINESS,
    HUNGER,
    ENERGY,
    MONEY
}

public enum Gender
{
    MALE,
    FEMALE
}


public class Player : MonoBehaviour
{
    private IDictionary<PlayerStats, float> playerStatsDict = new Dictionary<PlayerStats, float>();
    public List<int> itemsBought = new List<int>(); //list to store bought items
    private string playerName;
    private Gender playerGender;
    private string courseEnrolled;
    private float hospitalFee = 1500; // hospital fee per day
    private float numOfdays = 0;
    private float currentTime = 7; //time will start at 7AM
    private int toggleCounter;
    private float currentDayCount = 1;// number of days that have passed
    
    [SerializeField] private GameObject hospitalizedPrompt;
    [SerializeField] private TextMeshProUGUI daysHospitalized;
    [SerializeField] private TextMeshProUGUI totalBill; 
    [SerializeField] private TextMeshProUGUI clockValue;
    [SerializeField] private TextMeshProUGUI indicatorAMPM;
    [SerializeField] private TextMeshProUGUI dayValue;
    [SerializeField] private TextMeshProUGUI studyHours;

    public string PlayerName { set{playerName = value;} get{return playerName;}}
    public float PlayerCash { set; get;}
    public string PlayerEnrolledCourse { set{courseEnrolled = value;} get{return courseEnrolled;}}
    public float PlayerEnrolledCourseDuration { set; get;}
    public float PlayerStudyHours { set; get;}
    public float PlayerBankSavings { set; get;}
    public Gender PlayerGender { set{playerGender = value;} get{return playerGender;}}
    public static Player Instance { get; private set; }


    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }


    private void Start()
    {
        playerStatsDict.Add(PlayerStats.HAPPINESS, 100f);
        playerStatsDict.Add(PlayerStats.HUNGER, 100f);
        playerStatsDict.Add(PlayerStats.ENERGY, 100f);
        playerStatsDict.Add(PlayerStats.MONEY, 5000f);
        PlayerBankSavings = 20000f;
    }


    public void EatDrink(float happinessAddValue, float energyLevelAddValue, float hungerLevelCutValue, float amount, float eatingTimeValue)
    {
        AddClockTime(eatingTimeValue);
        playerStatsDict[PlayerStats.HAPPINESS] += happinessAddValue;
        playerStatsDict[PlayerStats.ENERGY] += energyLevelAddValue;
        playerStatsDict[PlayerStats.HUNGER] += hungerLevelCutValue;
        playerStatsDict[PlayerStats.MONEY] -= amount;

        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ALL, playerStatsDict);
        //Debug.Log("HAPPY:" + playerStatsDict[PlayerStats.HAPPINESS]);
        StatsChecker();
        //Debug.Log("HAPPY AFTER:" + playerStatsDict[PlayerStats.HAPPINESS]);
    }


    public void Sleep(float energyLevelAddValue)
    {
        playerStatsDict[PlayerStats.ENERGY] += energyLevelAddValue;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ENERGY, playerStatsDict);
    }


    public void Work(float salary)
    {
        playerStatsDict[PlayerStats.MONEY] += salary;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.MONEY, playerStatsDict);
    }

    public float checkInputDuration(float duration)
    {
        float targetTime = duration + currentTime;
        if (targetTime > 17.0f)
        {
            float excessTime = targetTime - 17;
            duration -= excessTime;
        }

        return duration;
    }


    public void Study(float studyDurationValue)
    {
        studyDurationValue = checkInputDuration(studyDurationValue);
        AddClockTime(studyDurationValue);
        playerStatsDict[PlayerStats.ENERGY] -= studyDurationValue * 10;
        playerStatsDict[PlayerStats.HAPPINESS] -= studyDurationValue * 3;
        playerStatsDict[PlayerStats.HUNGER] -= studyDurationValue * 5;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ENERGY, playerStatsDict);
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.HAPPINESS, playerStatsDict);
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.HUNGER, playerStatsDict);

        StatsChecker();
        University.Instance.UpdateStudyHours(studyDurationValue);
    }

    public void UpdateStudyHours(float studyDurationValue)
    {
        PlayerStudyHours += studyDurationValue;
        studyHours.text = PlayerStudyHours.ToString();
    }

    public void Enroll(string courseName, float courseDuration)
    {
        UpdateStudyHours(0);
    }

    public void Purchase(float energyLevelCutValue, float amount)
    {
        AddClockTime(0.30f);
        playerStatsDict[PlayerStats.ENERGY] -= energyLevelCutValue;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ENERGY, playerStatsDict);

        playerStatsDict[PlayerStats.MONEY] -= amount;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.MONEY, playerStatsDict);
        StatsChecker();
    }

    public void PurchaseMallItem(float energyLevelCutValue, float amount, float happinessAddValue)
    {
        AddClockTime(0.30f);
        playerStatsDict[PlayerStats.HAPPINESS] += happinessAddValue;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.HAPPINESS, playerStatsDict);

        playerStatsDict[PlayerStats.ENERGY] -= energyLevelCutValue;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ENERGY, playerStatsDict);

        playerStatsDict[PlayerStats.MONEY] -= amount;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.MONEY, playerStatsDict);
        StatsChecker();
    }


    public void Walk(float energyLevelCutValue)
    {
        AddClockTime(2);
        StatsChecker();
        playerStatsDict[PlayerStats.ENERGY] -= energyLevelCutValue;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ENERGY, playerStatsDict);
    }


    public void Ride(float energyLevelCutValue)
    {
        AddClockTime(0.5f);
        StatsChecker();
        playerStatsDict[PlayerStats.ENERGY] -= energyLevelCutValue;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ENERGY, playerStatsDict);
    }

    public void BuyGrocery(float amount)
    {
        playerStatsDict[PlayerStats.MONEY] -= amount;
        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.MONEY, playerStatsDict);
    }

    public void StatsChecker()
    {
        if (playerStatsDict[PlayerStats.ENERGY] <= 10 | playerStatsDict[PlayerStats.HAPPINESS] <= 10 | playerStatsDict[PlayerStats.HUNGER] <= 10)//checks if any of the stats reaches lower limit
        {
            Debug.Log("LUH");
            float sumOfStats = playerStatsDict[PlayerStats.ENERGY] + playerStatsDict[PlayerStats.HAPPINESS] + playerStatsDict[PlayerStats.HUNGER];

            if ((sumOfStats*100) / 300 <= 10)
            {
                Hospitalized(5);
            }
            else if ((sumOfStats*100) / 300 <= 30)
            {
                Hospitalized(4);
            }
            else if ((sumOfStats*100) / 300 <= 50)
            {
                Hospitalized(3);
            }
            else if ((sumOfStats*100) / 300 <= 70)
            {
                Hospitalized(2);
            }
            else
            {
                Hospitalized(1);
            }
            
        }

        //Checks if stats reaches the upper limit
        if (playerStatsDict[PlayerStats.ENERGY] > 100)
        {
            playerStatsDict[PlayerStats.ENERGY] = 100;
        }
        if (playerStatsDict[PlayerStats.HAPPINESS] > 100)
        {
            playerStatsDict[PlayerStats.HAPPINESS] = 100;
        }
        if (playerStatsDict[PlayerStats.HUNGER] > 100)
        {
            playerStatsDict[PlayerStats.HUNGER] = 100;
        }
    }

    public void Hospitalized(float dayCount)
    {
        hospitalizedPrompt.SetActive(true);
        daysHospitalized.text = dayCount.ToString();
        float bill = hospitalFee*dayCount;
        totalBill.text = bill.ToString();

        numOfdays = dayCount;
    }
    
    public void PayHospitalFees()
    {
        playerStatsDict[PlayerStats.MONEY] -= numOfdays*hospitalFee;
        playerStatsDict[PlayerStats.HAPPINESS] = 100;
        playerStatsDict[PlayerStats.ENERGY] = 100;
        playerStatsDict[PlayerStats.HUNGER] = 100;

        PlayerStatsObserver.onPlayerStatChanged(PlayerStats.ALL, playerStatsDict);

        hospitalizedPrompt.SetActive(false);
    }

    public void AddClockTime(float addedClockValue)
    {
        currentTime += addedClockValue;
        int minutes = 0;
        int hours = 0;
        int transposedValue;

        hours = (int)currentTime;
        float temp = (currentTime % 1) * 100;
        minutes = (int)temp;

        if (currentTime % 1 != 0) //determines if value is whole number or not
        {
            if (minutes > 59)
            {
                minutes = 0;
                hours += 1;
                if (hours > 24)
                {
                    hours = 0;
                    toggleCounter++;
                }
                currentTime = hours;
                transposedValue = TransposeTimeValue((int)currentTime);
                clockValue.text = transposedValue.ToString() + ":00";
            }
            else if (minutes < 10)
            {
                if (hours > 24)
                {
                    hours = 0;
                    toggleCounter++;
                }
                transposedValue = TransposeTimeValue((int)currentTime);
                clockValue.text = transposedValue.ToString() + ":0" + minutes.ToString();
            }
            else if (hours > 24)
            {
                hours = 0;
                currentTime = hours + (minutes / 100);
                transposedValue = TransposeTimeValue((int)currentTime);
                clockValue.text = transposedValue.ToString() + ":" + minutes.ToString();
                toggleCounter++;
            }
            else
            {
                transposedValue = TransposeTimeValue((int)currentTime);
                clockValue.text = transposedValue.ToString() + ":" + minutes.ToString();
            }
        }
        else
        {
            if (hours > 24)
            {
                hours = 0;
                currentTime = hours + (minutes / 100);
                transposedValue = TransposeTimeValue((int)currentTime);
                clockValue.text = transposedValue.ToString() + ":00";
                toggleCounter++;
            }
            else
            {
                transposedValue = TransposeTimeValue((int)currentTime);
                clockValue.text = transposedValue.ToString() + ":00";
            }
        }
        Debug.Log(currentTime);
        AmOrPm();
        IncrementDayCount();
    }

    public static int TransposeTimeValue(int currentTime)
    {
        switch (currentTime)
        {
            case 13: return 1;
            case 14: return 2;
            case 15: return 3;
            case 16: return 4;
            case 17: return 5;
            case 18: return 6;
            case 19: return 7;
            case 20: return 8;
            case 21: return 9;
            case 22: return 10;
            case 23: return 11;
            case 24: return 12;
            case 0: return 12;
            default: return (currentTime);
        }
    }

    public void AmOrPm()
    {
        if (currentTime >= 12)
        {
            indicatorAMPM.text = "PM";
        }
        else
        {
            indicatorAMPM.text = "AM";
        }
    }

    public void IncrementDayCount()
    {
        if (toggleCounter == 1)
        {
            currentDayCount++;
            Debug.Log("Current Day Count = " + currentDayCount);
            dayValue.text = currentDayCount.ToString();
            toggleCounter = 0;
        }
    }
}
