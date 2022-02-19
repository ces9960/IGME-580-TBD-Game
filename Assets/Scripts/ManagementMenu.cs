using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementMenu : MonoBehaviour
{
    static List<TopLevelMenuButton> mainOptions = new List<TopLevelMenuButton>();
    static int CursorPositionTopLevel = 0;
    static int CursorPositionSubLevel = 0;
    static readonly int[] LEVELUP_COST = { 5, 10, 15, 20 }; // the cost of leveling up a unit
    static List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    const int UNIT_CAP = 10; //the maximum number of units the player can have in their roster
    static int money;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++) //creates top-level menu buttons
        {
            mainOptions.Add(gameObject.AddComponent<TopLevelMenuButton>());
        }
        mainOptions[0].AddSubmenuButton(); //for debug purposes to avoid array out of bounds errors
        for(int i = 0; i < 2; i++)//adds items to recruitment menu
        {
            mainOptions[1].AddSubmenuButton();
        }
        for(int i = 0; i < playerUnits.Count + 1; i++) //add upgrade buttons to the menu, based on the number of units the player has recruited
        {
            mainOptions[2].AddSubmenuButton();
        }
        for(int i = 0; i < 3; i++) //add buttons to select mission
        {
            mainOptions[3].AddSubmenuButton();
        }
        CursorPositionSubLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ConfirmWindow.ConfirmMenuOpen)
        {
            //move left/right
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                CursorPositionTopLevel--;
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
                CursorPositionTopLevel++;
            }

            //loop cursor if it reaches ends
            if(CursorPositionTopLevel < 0)
            {
                CursorPositionTopLevel = mainOptions.Count - 1;
            }
            if(CursorPositionTopLevel >= mainOptions.Count)
            {
                CursorPositionTopLevel = 0;
            }

            //sets cursor position to the end of the submenu if it exceeds it
            if(CursorPositionSubLevel >= mainOptions[CursorPositionTopLevel].menuSize)
            {
                CursorPositionSubLevel = mainOptions[CursorPositionTopLevel].menuSize - 1;
            }

            //moves submenu cursor
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                CursorPositionSubLevel++;
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                CursorPositionSubLevel--;
            }

            //loops submenu cursor
            if (CursorPositionSubLevel >= mainOptions[CursorPositionTopLevel].menuSize)
            {
                CursorPositionSubLevel = 0;
            }
            if (CursorPositionSubLevel < 0)
            {
                CursorPositionSubLevel = mainOptions[CursorPositionTopLevel].menuSize - 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Main: " + CursorPositionTopLevel + " Sub: " + CursorPositionSubLevel);
        }
    }

    void AddUnit(PlayerUnit.characterClass unitClass)
    {
        PlayerUnit unitToRecruit = gameObject.AddComponent<PlayerUnit>();
        unitToRecruit.myClass = unitClass;
        playerUnits.Add(unitToRecruit);
    }

    void UpgradeUnit(PlayerUnit unitToUpgrade)
    {
        int unitLevel = unitToUpgrade.getLevel(); //gets level to avoid calling a function multiple times

        if(unitLevel > 0 && unitLevel < PlayerUnit.LEVEL_CAP) // checks if the unit is within a valid level range
        {
            if(money > LEVELUP_COST[unitLevel + 1]) //checks that the player has enough money to upgrade
            {
                money -= LEVELUP_COST[unitLevel + 1]; //subtracts money based on the level of the unit
                unitToUpgrade.increaseLevel(); //increases the level of the unit
            }
        }
    }
}
