using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residential : Building
{
    private void Start()
    {
        this.buildingStringName = "Residential Area";
        this.buildingEnumName = Buildings.RESIDENTIAL;
        this.buildingOpeningTime = 0f;
        this.buildingClosingTime = 0f;

        BuildingManager.Instance.onBuildingBtnClicked += CheckBtnClicked;
    }


    private void OnDestroy()
    {
        BuildingManager.Instance.onBuildingBtnClicked -= CheckBtnClicked;
    }


    public override void CheckBtnClicked(Buttons clickedBtn)
    {
        if (BuildingManager.Instance.CurrentSelectedBuilding.buildingEnumName == this.buildingEnumName)
            switch (clickedBtn)
            {
                case Buttons.ENTER:
                    BuildingManager.Instance.EnterResidentialArea();
                    break;
            }
    }


    public override void CheckButtons()
    {
        this.actionButtons = new List<Buttons>(){Buttons.ENTER};
    }
}
