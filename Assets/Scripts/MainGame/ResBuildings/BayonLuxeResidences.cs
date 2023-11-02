using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BayonLuxeResidences : ResBuilding
{
    private void Start()
    {
        this.buildingName = ResBuildings.BAYONLUXERESIDENCES;
        this.buildingNameStr = "Bayon Luxe Residences";
        this.monthlyRent = 10000f;
        this.monthlyElecCharge = 1500f;
        this.monthlyWaterCharge = 500f;
        this.dailyAdtnlHappiness = 15f; 
        this.adtnlEnergyForSleep = 2f;
  
        this.actionButtons = new List<Buttons>(){Buttons.SLEEP, Buttons.EAT};
        BuildingManager.Instance.onBuildingBtnClicked += CheckBtnClicked;
    }


    private void OnDestroy()
    {
        BuildingManager.Instance.onBuildingBtnClicked -= CheckBtnClicked;
    } 


    public override void Sleep()
    {
        SleepManager.Instance.ShowSleepOverlay(this.adtnlEnergyForSleep);
        Debug.Log("im sleeping");
    }


    public override void Eat()
    {

    }


    public override void CheckBtnClicked(Buttons clickedBtn)
    {
        if (Player.Instance.CurrentPlayerPlace == this)
            switch (clickedBtn)
            {
                case Buttons.SLEEP:
                    Sleep();
                    break;
                case Buttons.EAT:
                    Debug.Log("i'll be eating");
                    break;
            }
    }
}