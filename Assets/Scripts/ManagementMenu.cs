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
    public List<GameObject> playerUnits = new List<GameObject>();
    const int UNIT_CAP = 5; //the maximum number of units the player can have in their roster
    static int money = 999;//the amount of money the player has to spend on recruiting/upgrading units
    static bool confirmOpen = false;
    int unitCount;

    //map images as prefabs
    public GameObject plains, river, lake, island;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    //z positions of map tiles, units, and visual effects (working as layers so that effects are drawn over units, which are drawn over tiles)
    const int tileLayer = 2;
    const int unitLayer = 1;
    const int effectLayer = 0;

    //the text that appears when confirming/denying an action
    static string confirmMessage;

    //list of maps, stored as 2D arrays of bools, where true indicates walkable tiles and false indicates impassable tiles
    public static List<bool[,]> mapList = new List<bool[,]>();

    enum gameState { mainMenu, management, combat, gameOver} //enum for the game's current state

    static gameState currentState = gameState.mainMenu;

    public List<GameObject> enemyList = new List<GameObject>();

    public static int currentMap = 0;

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
        unitCount = 0;
        createMaps();
        generateMissions();
        /*
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                
                //generates and hides land tiles
                landTiles[i, j] = landTilePrefab;
                Instantiate(landTiles[i, j]);
                landTiles[i, j].transform.position = new Vector3((float)(i-4) * 0.6f, (float)(j-4) * 0.6f, tileLayer);
                landTiles[i, j].GetComponent<mapTile>().tileInit();
                landTiles[i, j].GetComponent<SpriteRenderer>().enabled = true;
                landTiles[i, j].GetComponent<mapTile>().setLayer(-11);

                //generates and hides water tiles
                waterTiles[i, j] = waterTilePrefab;
                Instantiate(waterTiles[i, j]);
                waterTiles[i,j].transform.position = new Vector3((float)(i - 4) * 0.6f, (float)(j - 4) * 0.6f, tileLayer);
                waterTiles[i,j].GetComponent<mapTile>().tileInit();
                waterTiles[i, j].GetComponent<mapTile>().setLayer(tileLayer);
                waterTiles[i, j].GetComponent<SpriteRenderer>().enabled = true;
                waterTiles[i, j].SetActive(true);
                
            }
        }*/
        for (int i = 0; i < 4; i++) //creates top-level menu buttons
        {
            mainOptions.Add(gameObject.AddComponent<TopLevelMenuButton>());

        }
        mainOptions[0].buttonName = "Recruit";
        mainOptions[1].buttonName = "Upgrade";
        mainOptions[2].buttonName = "Mission";
        mainOptions[3].buttonName = "System";

        for (int i = 0; i < 2; i++)//quit game (options to return to the main menu or close the entire game)
        {
            mainOptions[3].AddSubmenuButton();
        }

        mainOptions[3].menuOptions[0].buttonName = "Quit to Main Menu";
        mainOptions[3].menuOptions[1].buttonName = "Quit to OS";

        for(int i = 0; i < 2; i++)//adds items to recruitment menu
        {
            mainOptions[0].AddSubmenuButton();
        }

        mainOptions[0].menuOptions[0].buttonName = "Fighter";
        mainOptions[0].menuOptions[1].buttonName = "Mage";

        for(int i = 0; i < playerUnits.Count; i++) //add upgrade buttons to the menu, based on the number of units the player has recruited (this will be empty at the start)
        {
            mainOptions[1].AddSubmenuButton();
        }
        for(int i = 0; i < 3; i++) //add buttons to select mission
        {
            mainOptions[2].AddSubmenuButton();
            mainOptions[2].menuOptions[i].buttonName = missionDescriptions[i];
        }
        CursorPositionSubLevel = 0;
        displayMap(-1);
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
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Application.Quit();
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
                        mainOptions[2].menuOptions[i].buttonName = missionDescriptions[i];
                    }
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    switch (CursorPositionTopLevel)//TODO: Make these options function correctly
                    {
                        case 3: //quit
                            createConfirm("Quit?");
                            break;
                        case 0: //add units
                            if (CursorPositionSubLevel == 0)
                            {
                                createConfirm("Recruit a melee fighter?\nCost: 5");
                            }
                            if (CursorPositionSubLevel == 1)
                            {
                                createConfirm("Recruit a spellcaster?\nCost: 5");
                            }
                            break;
                        case 1: //upgrade units
                            if(playerUnits[CursorPositionSubLevel].GetComponent<PlayerUnit>().getLevel() < PlayerUnit.LEVEL_CAP)
                            {
                                createConfirm("Upgrade this unit?\nCost: " + LEVELUP_COST[playerUnits[CursorPositionSubLevel].GetComponent<PlayerUnit>().getLevel() - 1]);
                            }
                            break;
                        case 2: //select mission
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
                            case 0: //add units
                                switch (CursorPositionSubLevel)
                                {
                                    case 0:
                                        if (unitCount < UNIT_CAP)
                                        {
                                            AddUnit(playerUnits[unitCount], PlayerUnit.characterClass.fighter);
                                            unitCount++;
                                        }
                                        break;
                                    case 1:
                                        if (unitCount < UNIT_CAP)
                                        {
                                            AddUnit(playerUnits[unitCount], PlayerUnit.characterClass.mage);
                                            unitCount++;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 1: //upgrade units
                                UpgradeUnit(playerUnits[CursorPositionSubLevel]);
                                break;
                            case 2: //select mission
                                currentState = gameState.combat; //currently just sets to combat mode
                                displayMap(missionMaps[CursorPositionSubLevel]);
                                currentMap = missionMaps[CursorPositionSubLevel];
                                break;
                            case 3: //Quit (either to main menu or to OS)
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

                }
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {

                }
                break;
            default:
                break;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(GameObject i in playerUnits)
            {
                Debug.Log(i.GetComponent<PlayerUnit>().getLevel());
            }
        }
    }

    private void OnGUI()
    {
        switch (currentState)
        {
            case gameState.mainMenu:
                GUI.Box(new Rect(200, 200, 400, 200), "Titles Be Difficult (Working Title)  \nEnter: start game\nQ: Quit to OS\nManagement mode controls:\nWASD/arrow keys: move cursor\nEnter: select\nY: confirm action\nN: cancel action");
                break;
            case gameState.management:
                GUI.color = Color.white;
                if (confirmOpen)
                {
                    GUI.Box(new Rect(860, 600, 200, 100), confirmMessage);
                }
                GUI.Box(new Rect(600, 600, 300, 100), "Money: " + money + "\nUnits: " + unitCount + "/" + UNIT_CAP + "\nControls\nArrow keys/WASD: move cursor\nEnter: select\nY/N: confirm/cancel");
                //TODO: Add UI elements for each menu button (placeholders until I make better UI elements)

                //in order: quit, recruit, upgrade, mission select

                if (CursorPositionTopLevel == 0)//recruit
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(200, 10, 200, 75), "Recruit");

                if (CursorPositionTopLevel == 1)//upgrade
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(400, 10, 200, 75), "Upgrade");

                if (CursorPositionTopLevel == 2)//mission
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(600, 10, 200, 75), "Missions");

                if (CursorPositionTopLevel == 3)//system
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUI.Box(new Rect(800, 10, 200, 75), "System");

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
                            GUI.color = Color.white;
                            if (CursorPositionTopLevel == i)
                            {

                                if (CursorPositionSubLevel == j)
                                {
                                    GUI.color = Color.green;
                                }

                            }
                            GUI.Box(new Rect(200 * (i + 1), 90 + (100 * j), 200, 100), mainOptions[i].menuOptions[j].buttonName);
                            GUI.color = Color.white;
                        }
                    }

                }
                break;
            case gameState.combat:
                GUI.Box(new Rect(400, 200, 600, 400), "Battle things go here");
                for(int i = 0; i < unitCount; i++)
                {
                    GUI.Box(new Rect(10, 90 * (i + 1), 200, 85), mainOptions[1].menuOptions[i].buttonName);
                }
                break;
            default:
                break;
        }


    }

    void updateUnits(GameObject toUpdate)
    {
        PlayerUnit updateUnit = toUpdate.GetComponent<PlayerUnit>();
        switch (updateUnit.myClass)
        {
            case PlayerUnit.characterClass.fighter:
                mainOptions[1].menuOptions[playerUnits.IndexOf(toUpdate)].buttonName = "Fighter\nLevel: " + updateUnit.getLevel() + "\nHP: " + updateUnit.getHealthCurrent + "/" + updateUnit.getHealthMax + "\nAttack: " + updateUnit.getAttack + "\nDefense: " + updateUnit.getDefense + "\nSpeed: " + updateUnit.getSpeed;
                break;
            case PlayerUnit.characterClass.mage:
                mainOptions[1].menuOptions[playerUnits.IndexOf(toUpdate)].buttonName = "Mage\nLevel: " + updateUnit.getLevel() + "\nHP: " + updateUnit.getHealthCurrent + "/" + updateUnit.getHealthMax + "\nAttack: " + updateUnit.getAttack + "\nDefense: " + updateUnit.getDefense + "\nSpeed: " + updateUnit.getSpeed;
                break;
            default:
                break;
        }
    }

    void AddUnit(GameObject recruitObj, PlayerUnit.characterClass unitClass)
    {
        if(money >= RECRUITMENT_COST && playerUnits.Count < UNIT_CAP)
        {

            PlayerUnit unitToRecruit = recruitObj.GetComponent<PlayerUnit>();
            unitToRecruit.setClass(unitClass) ;
            playerUnits.Add(recruitObj);
            mainOptions[1].AddSubmenuButton();
            unitToRecruit.restart();//This actually just calls Start() so the starting stats are proplerly assigned
            
            updateUnits(recruitObj);
            money -= RECRUITMENT_COST;
        }
    }

    void UpgradeUnit(GameObject toUpgrade)
    {
        PlayerUnit unitToUpgrade = toUpgrade.GetComponent<PlayerUnit>();
        int unitLevel = unitToUpgrade.getLevel(); //gets level to avoid calling a function multiple times

        if(unitLevel > 0 && unitLevel < PlayerUnit.LEVEL_CAP) // checks if the unit is within a valid level range
        {
            if(money > LEVELUP_COST[unitLevel - 1]) //checks that the player has enough money to upgrade
            {
                money -= LEVELUP_COST[unitLevel - 1]; //subtracts money based on the level of the unit
                unitToUpgrade.increaseLevel(); //increases the level of the unit
               /* switch (unitToUpgrade.myClass)
                {
                    case PlayerUnit.characterClass.fighter:
                        mainOptions[2].menuOptions[playerUnits.IndexOf(unitToUpgrade)].buttonName = "Fighter\nLevel: " + unitToUpgrade.getLevel() + "\nHP: " + unitToUpgrade.getHealthCurrent + "/" + unitToUpgrade.getHealthMax + "\nAttack: " + unitToUpgrade.getAttack + "\nDefense: " + unitToUpgrade.getDefense + "\nSpeed: " + unitToUpgrade.getSpeed;
                        break;
                    case PlayerUnit.characterClass.mage:
                        mainOptions[2].menuOptions[playerUnits.IndexOf(unitToUpgrade)].buttonName = "Mage\nLevel: " + unitToUpgrade.getLevel() + "\nHP: " + unitToUpgrade.getHealthCurrent + "/" + unitToUpgrade.getHealthMax + "\nAttack: " + unitToUpgrade.getAttack + "\nDefense: " + unitToUpgrade.getDefense + "\nSpeed: " + unitToUpgrade.getSpeed;
                        break;
                    default:
                        break;
                }*/
            }
        }
        updateUnits(toUpgrade);
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
            {true, true, true, false, false, true, true, true },
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
    void displayMap(int mapSelect)
    {
        switch (mapSelect)
        {
            case 0:
                plains.GetComponent<SpriteRenderer>().enabled = true;
                river.GetComponent<SpriteRenderer>().enabled = false;
                lake.GetComponent<SpriteRenderer>().enabled = false;
                island.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 1:
                plains.GetComponent<SpriteRenderer>().enabled = false;
                river.GetComponent<SpriteRenderer>().enabled = true;
                lake.GetComponent<SpriteRenderer>().enabled = false;
                island.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 2:
                plains.GetComponent<SpriteRenderer>().enabled = false;
                river.GetComponent<SpriteRenderer>().enabled = false;
                lake.GetComponent<SpriteRenderer>().enabled = true;
                island.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 3:
                plains.GetComponent<SpriteRenderer>().enabled = false;
                river.GetComponent<SpriteRenderer>().enabled = false;
                lake.GetComponent<SpriteRenderer>().enabled = false;
                island.GetComponent<SpriteRenderer>().enabled = true;
                break;
            default:
                plains.GetComponent<SpriteRenderer>().enabled = false;
                river.GetComponent<SpriteRenderer>().enabled = false;
                lake.GetComponent<SpriteRenderer>().enabled = false;
                island.GetComponent<SpriteRenderer>().enabled = false;
                break;
        }
        /*
        for(int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            { 
                if (mapList[mapSelect][i, j])
                {
                    landTiles[i, j].GetComponent<mapTile>().setLayer(tileLayer);
                    waterTiles[i, j].GetComponent<mapTile>().setLayer(-11);
                }
                else
                {
                    landTiles[i, j].GetComponent<mapTile>().setLayer(-11);
                    waterTiles[i, j].GetComponent<mapTile>().setLayer(tileLayer);
                }

            }
        }*/
    }

    void generateMissions()
    {
        for(int i = 0; i < 3; i++)
        {
            missionDescriptions[i] = "";
            missionLevels[i] = Random.Range(1, PlayerUnit.LEVEL_CAP);
            missionMaps[i] = Random.Range(0, mapList.Count);
            missionMelee[i] = Random.Range(0, UNIT_CAP - 2);
            missionRanged[i] = Random.Range(0, UNIT_CAP - 2);
            missionRewards[i] = missionLevels[i] * (missionMelee[i] + missionRanged[i]);
            while(missionMelee[i] + missionRanged[i] <= 0)
            {
                missionMelee[i] = Random.Range(0, UNIT_CAP - 2);
                missionRanged[i] = Random.Range(0, UNIT_CAP - 2);
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

    void incrementTurns()
    {
        foreach(GameObject foo in playerUnits)
        {
            PlayerUnit unit = foo.GetComponent<PlayerUnit>();
            unit.turnCounter += unit.getSpeed;
            if(unit.turnCounter >= 100)
            {
                unit.StartTurn();
            }
        }
        foreach(GameObject bar in enemyList)
        {
            Enemy enemy = bar.GetComponent<Enemy>();
            enemy.turnCounter += enemy.speedStat;
            if(enemy.turnCounter >= 100)
            {
                enemy.enemyTurn(Random.Range(0, playerUnits.Count));
            }
        }
    }

}
