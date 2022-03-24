using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementMenu : MonoBehaviour
{
    static List<TopLevelMenuButton> mainOptions = new List<TopLevelMenuButton>();
    static int CursorPositionTopLevel = 0;
    static int CursorPositionSubLevel = 0;
    static readonly int[] LEVELUP_COST = { 5, 10, 15, 20 }; // the cost of leveling up a unit
    const int RECRUITMENT_COST = 5;
    public static List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    const int UNIT_CAP = 5; //the maximum number of units the player can have in their roster
    static int money = 999;//the amount of money the player has to spend on recruiting/upgrading units
    static bool confirmOpen = false;

    //the text that appears when confirming/denying an action
    static string confirmMessage;

    //list of maps, stored as 2D arrays of bools, where true indicates walkable tiles and false indicates impassable tiles
    static List<bool[,]> mapList = new List<bool[,]>();

    enum gameState { mainMenu, management, combat} //enum for the game's current state

    static gameState currentState = gameState.mainMenu; //game starts in management mode, at least until I make a main menu

    public static List<Enemy> enemyList = new List<Enemy>();

    static int currentMap = 0;

    //info about missions
    static int[] missionMaps = {0,0,0 };
    static int[] missionLevels = {0,0,0 };
    static int[] missionMelee = { 0, 0, 0 };
    static int[] missionRanged = { 0, 0, 0 };
    static int[] missionRewards = { 0, 0, 0 };
    static string[] missionDescriptions = { "","","" };

    // Start is called before the first frame update
    void Start()
    {
        createMaps();
        generateMissions();

        for (int i = 0; i < 4; i++) //creates top-level menu buttons
        {
            mainOptions.Add(gameObject.AddComponent<TopLevelMenuButton>());

        }
        mainOptions[0].buttonName = "System";
        mainOptions[1].buttonName = "Recruit";
        mainOptions[2].buttonName = "Upgrade";
        mainOptions[3].buttonName = "Mission";

        for (int i = 0; i < 2; i++)//quit game (options to return to the main menu or close the entire game)
        {
            mainOptions[0].AddSubmenuButton();
        }

        mainOptions[0].menuOptions[0].buttonName = "Quit to Main Menu";
        mainOptions[0].menuOptions[1].buttonName = "Quit to OS";

        for(int i = 0; i < 2; i++)//adds items to recruitment menu
        {
            mainOptions[1].AddSubmenuButton();
        }

        mainOptions[1].menuOptions[0].buttonName = "Fighter";
        mainOptions[1].menuOptions[1].buttonName = "Mage";

        for(int i = 0; i < playerUnits.Count; i++) //add upgrade buttons to the menu, based on the number of units the player has recruited (this will be empty at the start)
        {
            mainOptions[2].AddSubmenuButton();
        }
        for(int i = 0; i < 3; i++) //add buttons to select mission
        {
            mainOptions[3].AddSubmenuButton();
            mainOptions[3].menuOptions[i].buttonName = missionDescriptions[i];
        }
        CursorPositionSubLevel = 0;

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case gameState.mainMenu:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    currentState = gameState.management;
                }
                break;
            case gameState.management:
                if (!confirmOpen)
                {
                    //move left/right
                    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                    {
                        CursorPositionTopLevel--;
                        //skip empty top-level menu items
                        if (mainOptions[CursorPositionTopLevel].menuSize < 1)
                        {
                            CursorPositionTopLevel--;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    {
                        CursorPositionTopLevel++;
                        //skip empty top-level menu items
                        if (mainOptions[CursorPositionTopLevel].menuSize < 1)
                        {
                            CursorPositionTopLevel++;
                        }
                    }

                    //loop cursor if it reaches ends
                    if (CursorPositionTopLevel < 0)
                    {
                        CursorPositionTopLevel = mainOptions.Count - 1;
                    }
                    if (CursorPositionTopLevel >= mainOptions.Count)
                    {
                        CursorPositionTopLevel = 0;
                    }

                    //sets cursor position to the end of the submenu if it exceeds it
                    if (CursorPositionSubLevel >= mainOptions[CursorPositionTopLevel].menuSize)
                    {
                        CursorPositionSubLevel = mainOptions[CursorPositionTopLevel].menuSize - 1;
                    }

                    //moves submenu cursor
                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    {
                        CursorPositionSubLevel--;
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    {
                        CursorPositionSubLevel++;
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
                    generateMissions();

                    for (int i = 0; i < 3; i++)
                    {
                        Debug.Log(missionDescriptions[i]);
                        mainOptions[3].menuOptions[i].buttonName = missionDescriptions[i];
                    }
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    switch (CursorPositionTopLevel)//TODO: Make these options function correctly
                    {
                        case 0: //TBD, probably game options (quit)
                            createConfirm("Quit?");
                            break;
                        case 1: //add units
                            if (CursorPositionSubLevel == 0)
                            {
                                createConfirm("Recruit a melee fighter?");
                            }
                            if (CursorPositionSubLevel == 1)
                            {
                                createConfirm("Recruit a spellcaster?");
                            }
                            break;
                        case 2: //upgrade units
                            createConfirm("Upgrade this unit?");
                            break;
                        case 3: //select mission
                            createConfirm("Choose this mission?");
                            break;
                        default:
                            break;
                    }
                }

                if (confirmOpen)
                {
                    if (Input.GetKeyDown(KeyCode.Y))
                    {
                        switch (CursorPositionTopLevel)//TODO: Make these options function correctly
                        {
                            case 0: //Quit (either to main menu or to OS)
                                switch (CursorPositionSubLevel)
                                {
                                    case 0: //quit to main menu
                                        currentState = gameState.mainMenu;
                                        break;
                                    case 1: //quit to OS
                                        Application.Quit();
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 1: //add units
                                switch (CursorPositionSubLevel)
                                {
                                    case 0:
                                        AddUnit(PlayerUnit.characterClass.fighter);
                                        break;
                                    case 1:
                                        AddUnit(PlayerUnit.characterClass.mage);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 2: //upgrade units
                                UpgradeUnit(playerUnits[CursorPositionSubLevel]);
                                break;
                            case 3: //select mission
                                currentState = gameState.combat; //currently just sets to combat mode
                                break;
                            default:
                                break;
                        }
                        closeConfirm();
                    }
                    if (Input.GetKeyDown(KeyCode.N))
                    {
                        closeConfirm();
                    }
                }
                break;
            case gameState.combat:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    currentState = gameState.management;

                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentMap--;
                    if(currentMap < 0)
                    {
                        currentMap = mapList.Count - 1;
                    }
                }
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentMap++;
                    if (currentMap > mapList.Count - 1)
                    {
                        currentMap = 0;
                    }
                }
                break;
            default:
                break;
        }

    }

    private void OnGUI()
    {
        switch (currentState)
        {
            case gameState.mainMenu:
                GUI.Box(new Rect(200, 200, 400, 200), "This will be the main menu in the final version.\nCurrently, this doesn't do anything.  \nPress enter to enter the management mode.\nManagement mode controls:\nWASD/arrow keys: move cursor\nEnter: select\nY: confirm action\nN: cancel action");
                break;
            case gameState.management:
                GUI.color = Color.white;
                if (confirmOpen)
                {
                    GUI.Box(new Rect(860, 600, 200, 100), confirmMessage);
                }
                GUI.Box(new Rect(600, 600, 100, 100), "main " + CursorPositionTopLevel + " sub " + CursorPositionSubLevel + "\nMoney: " + money + "\nUnits: " + playerUnits.Count + "/" + UNIT_CAP);
                //TODO: Add UI elements for each menu button (placeholders until I make better UI elements)

                //in order: quit, recruit, upgrade, mission select

                if (CursorPositionTopLevel == 0)//system
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(200, 10, 200, 75), "System");

                if (CursorPositionTopLevel == 1)//recruit
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(400, 10, 200, 75), "Recruit");

                if (CursorPositionTopLevel == 2)//upgrade
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(600, 10, 200, 75), "Upgrade");

                if (CursorPositionTopLevel == 3)//mission
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(800, 10, 200, 75), "Missions");

                //quit options: main menu, exit game

                //recruit options: melee fighter (specify cost), spellcaster (specify cost)

                //upgrade options: show all units (class, level, upgrade cost)

                //mission options: missions 1-3

                for (int i = 0; i < mainOptions.Count; i++)//draws submenu items, with selected items shown in green
                {
                    if (mainOptions[i].menuSize > 0)
                    {
                        for (int j = 0; j < mainOptions[i].menuSize; j++)
                        {
                            if (CursorPositionTopLevel == i)
                            {
                                GUI.color = Color.white;
                                if (CursorPositionSubLevel == j)
                                {
                                    GUI.color = Color.green;
                                }
                                GUI.Box(new Rect(200 * (i + 1), 90 + (90 * j), 200, 85), mainOptions[i].menuOptions[j].buttonName);
                                GUI.color = Color.white;
                            }
                        }
                    }

                }
                break;
            case gameState.combat:
                GUI.Box(new Rect(400, 200, 600, 400), displayMapAsText(currentMap) + "\nThese are the map layouts that will be implemented in the final game.\nX represents water tiles, while O represents land tiles.\nUnits can move across land tiles, but not water tiles.\nAt the moment, the combat system isn't fully working yet.\nPress A/D or use the arrow keys to cycle between maps.\nPress enter to return to management mode.\nOn the left side of the screen is a list of player units and their stats.\nA menu will appear next to each unit when it is that unit's turn in battle.");
                for(int i = 0; i < playerUnits.Count; i++)
                {
                    GUI.Box(new Rect(10, 90 * (i + 1), 200, 85), mainOptions[2].menuOptions[i].buttonName);
                }
                break;
            default:
                break;
        }

    }


    void AddUnit(PlayerUnit.characterClass unitClass)
    {
        if(money >= RECRUITMENT_COST && playerUnits.Count < UNIT_CAP)
        {
            PlayerUnit unitToRecruit = gameObject.AddComponent<PlayerUnit>();
            unitToRecruit.myClass = unitClass;
            playerUnits.Add(unitToRecruit);
            mainOptions[2].AddSubmenuButton();
            switch (unitToRecruit.myClass)
            {
                case PlayerUnit.characterClass.fighter:
                    mainOptions[2].menuOptions[playerUnits.Count - 1].buttonName = "Fighter\nLevel: " + unitToRecruit.getLevel() + "\nHP: " + unitToRecruit.getHealthCurrent() + "/" + unitToRecruit.getHealthMax() + "\nAttack: " + unitToRecruit.getAttack() + "\nDefense: " + unitToRecruit.getDefense() + "\nSpeed: " + unitToRecruit.getSpeed();
                    break;
                case PlayerUnit.characterClass.mage:
                    mainOptions[2].menuOptions[playerUnits.Count - 1].buttonName = "Mage\nLevel: " + unitToRecruit.getLevel() + "\nHP: " + unitToRecruit.getHealthCurrent() + "/" + unitToRecruit.getHealthMax() + "\nAttack: " + unitToRecruit.getAttack() + "\nDefense: " + unitToRecruit.getDefense() + "\nSpeed: " + unitToRecruit.getSpeed();
                    break;
                default:
                    break;
            }
            money -= RECRUITMENT_COST;
        }
    }

    void UpgradeUnit(PlayerUnit unitToUpgrade)
    {
        int unitLevel = unitToUpgrade.getLevel(); //gets level to avoid calling a function multiple times

        if(unitLevel > 0 && unitLevel < PlayerUnit.LEVEL_CAP) // checks if the unit is within a valid level range
        {
            if(money > LEVELUP_COST[unitLevel - 1]) //checks that the player has enough money to upgrade
            {
                money -= LEVELUP_COST[unitLevel - 1]; //subtracts money based on the level of the unit
                unitToUpgrade.increaseLevel(); //increases the level of the unit
                switch (unitToUpgrade.myClass)
                {
                    case PlayerUnit.characterClass.fighter:
                        mainOptions[2].menuOptions[playerUnits.IndexOf(unitToUpgrade)].buttonName = "Fighter\nLevel: " + unitToUpgrade.getLevel() + "\nHP: " + unitToUpgrade.getHealthCurrent() + "/" + unitToUpgrade.getHealthMax() + "\nAttack: " + unitToUpgrade.getAttack() + "\nDefense: " + unitToUpgrade.getDefense() + "\nSpeed: " + unitToUpgrade.getSpeed();
                        break;
                    case PlayerUnit.characterClass.mage:
                        mainOptions[2].menuOptions[playerUnits.IndexOf(unitToUpgrade)].buttonName = "Mage\nLevel: " + unitToUpgrade.getLevel() + "\nHP: " + unitToUpgrade.getHealthCurrent() + "/" + unitToUpgrade.getHealthMax() + "\nAttack: " + unitToUpgrade.getAttack() + "\nDefense: " + unitToUpgrade.getDefense() + "\nSpeed: " + unitToUpgrade.getSpeed();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void createConfirm(string newMessage)
    {
        confirmMessage = newMessage;
        confirmOpen = true;
    }

     void closeConfirm()
    {
        confirmOpen = false;

    }

    //generates the battle maps and adds them to the 
    void createMaps()
    {
        //flat plains map (all tiles are walkable)
        bool[,] plainsMap =
        {
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true }
        };

        //bridge map (split horizontally so that there's a 2 tile wide chokepoint in the middle of the map)
        bool[,] bridgeMap =
        {
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {false, false, false, true, true, false, false, false },
            {false, false, false, true, true, false, false, false },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true }
        };

        //lake map (a 4x4 square of impassable tiles in the middle of the map)
        bool[,] lakeMap =
        {
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, false, false, false, false, true, true },
            {true, true, false, false, false, false, true, true },
            {true, true, false, false, false, false, true, true },
            {true, true, false, false, false, false, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, true, true, true, true, true }
        };

        //4 islands map (4 3x3 islands connected by 1 tile wide bridges
        bool[,] islandMap =
            {
            {true, true, true, false, false, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, false, false, false, true, true },
            {false, true, false, false, false, false, true, false },
            {false, true, false, false, false, false, true, false },
            {true, true, true, false, false, true, true, true },
            {true, true, true, true, true, true, true, true },
            {true, true, true, false, false, true, true, true }
        };

        mapList.Add(plainsMap);
        mapList.Add(bridgeMap);
        mapList.Add(lakeMap);
        mapList.Add(islandMap);
    }
    string displayMapAsText(int mapSelect)
    {
        string output = "";
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (mapList[mapSelect][i, j])
                {
                    output += "O";
                }
                else
                {
                    output += "X";
                }
            }
            output += "\n";
        }
        return output;
    }

    void generateMissions()
    {
        for(int i = 0; i < 3; i++)
        {
            missionLevels[i] = Random.Range(1, PlayerUnit.LEVEL_CAP);
            missionMaps[i] = Random.Range(0, mapList.Count - 1);
            missionMelee[i] = Random.Range(0, UNIT_CAP - 2);
            missionRanged[i] = Random.Range(0, UNIT_CAP - 2);
            missionRewards[i] = missionLevels[i] * (missionMelee[i] + missionRanged[i]);
            if(missionMelee[i] + missionRanged[i] <= 0)
            {
                generateMissions();
            }
            missionDescriptions[i] += "Level: " + missionLevels[i];
            missionDescriptions[i] += "\nTotal enemies: " + (missionMelee[i] + missionRanged[i]);
            missionDescriptions[i] += "\nMelee: " + missionMelee[i];
            missionDescriptions[i] += "\nRanged: " + missionRanged[i];
            missionDescriptions[i] += "\nReward: " + missionRewards[i];
            switch (missionMaps[i])
            {
                case 0:
                    missionDescriptions[i] += "\nMap: Plains";
                    break;
                case 1:
                    missionDescriptions[i] += "\nMap: Bridge";
                    break;
                case 2:
                    missionDescriptions[i] += "\nMap: Lake";
                    break;
                case 3:
                    missionDescriptions[i] += "\nMap: Corners";
                    break;
                default:
                    break;
            }
        }
    }
}
