using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    public ManagementMenu menu;

    int healthCurrent, healthMax; //current and max HP
    int attackStat, defenseStat, speedStat; //stats for attack, defense, and speed
    int level = 1; //this character's level
    public const int LEVEL_CAP = 5; //the maximum level that a character can be
    const int TURN_THRESHOLD = 100; //when turnCounter reaches this number, this unit takes a turn
    public int turnCounter = 0; //increases at a rate based on the unit's speed
    bool fighterDefBuff = false; //is the fighter's defense buff skill active?
    bool movementUsed = false; //has this unit moved?
    bool actionUsed = false; //has this unit attacked or used an ability?
    public int gridX, gridY; //position on the grid

    SpriteRenderer unitSprite;

    public Sprite meleeImage;
    public Sprite rangedImage;

    #region startingStats
    //starting stats by class (currently placeholder values)
    const int HP_START_FIGHTER = 20;
    const int ATK_START_FIGHTER = 10;
    const int DEF_START_FIGHTER = 10;
    const int SPD_START_FIGHTER = 10;

    const int HP_START_MAGE = 15;
    const int ATK_START_MAGE = 15;
    const int DEF_START_MAGE = 5;
    const int SPD_START_MAGE = 10;
    #endregion

    #region statGrowths
    //stat growths per level
    const int HP_GROWTH_FIGHTER = 10;
    const int ATK_GROWTH_FIGHTER = 2;
    const int DEF_GROWTH_FIGHTER = 2;
    const int SPD_GROWTH_FIGHTER = 2;

    const int HP_GROWTH_MAGE = 5;
    const int ATK_GROWTH_MAGE = 3;
    const int DEF_GROWTH_MAGE = 1;
    const int SPD_GROWTH_MAGE = 2;
    #endregion
    public enum characterClass { fighter, mage} //enum of character classes (fighter and mage, with the possibility of adding more as a stretch goal)
    public characterClass myClass; //this unit's character class

    // Start is called before the first frame update
    void Start()
    {
        unitSprite = GetComponent<SpriteRenderer>();
        level = 1;
        switch (myClass) // sets starting stats based on class
        {
            case characterClass.fighter:
                healthMax = HP_START_FIGHTER;
                healthCurrent = healthMax;
                attackStat = ATK_START_FIGHTER;
                defenseStat = DEF_START_FIGHTER;
                speedStat = SPD_START_FIGHTER;
                unitSprite.sprite = meleeImage;
                gridX = -1;
                break;
            case characterClass.mage:
                healthMax = HP_START_MAGE;
                healthCurrent = healthMax;
                attackStat = ATK_START_MAGE;
                defenseStat = DEF_START_MAGE;
                speedStat = SPD_START_MAGE;
                unitSprite.sprite = rangedImage;
                gridX = 1;
                break;
            default:
                unitSprite.sprite = meleeImage;
                break;
                
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(gridX * ManagementMenu.GRID_SIZE + ManagementMenu.offsetX, gridY * ManagementMenu.GRID_SIZE + ManagementMenu.offsetY, ManagementMenu.unitLayer); 
    }

    void UnitMovement()
    {
        //not implemented
    }

    void AttackAction(int targetX, int targetY)
    {
        switch (myClass)
        {
            case characterClass.fighter: //fighters deal full damage with basic attacks but can only attack adjacent squares (including diagonals)
                if(Mathf.Abs(targetX - gridX) <= 1 && Mathf.Abs(targetX - gridX) <= 1)
                {
                    if(attackSquare(targetX, targetY, level))//attacks and sets actionUsed to true if the attack hits something
                    {
                        actionUsed = true;
                    }
                }
                break;
            case characterClass.mage: //mages deal half damage with basic attacks but can attack anywhere on screen
                if(attackSquare(targetX,targetY,Mathf.RoundToInt(level / 2)))//attacks and sets actionUsed to true if the attack hits something
                {
                    actionUsed = true;
                }
                break;
        }
    }

    bool attackSquare(int targetX, int targetY, int baseDamage)//checks the target square for enemies, deals damage if there are enemies in the square, then returns true if there was an enemy there or false if there wasn't
    {
        for(int i = 0; i < menu.enemyList.Count; i++)
        {
            if(menu.enemyList[i].GetComponent<Enemy>().gridX == targetX && menu.enemyList[i].GetComponent<Enemy>().gridY == targetY)
            {
                menu.enemyList[i].GetComponent<Enemy>().takeDamage(baseDamage * attackStat / menu.enemyList[i].GetComponent<Enemy>().defenseStat);
                return true;
            }
        }
        return false;
    }

    void FirstAbility(int targetX, int targetY) //first ability
    {
        switch (myClass) //checks this unit's character class then uses the corresponding ability
        {
            case characterClass.fighter: //attacks for double base damage, but takes 1/4 max health as damage
                if (attackSquare(targetX, targetY, level * 2))
                {
                    modifyHealth(-healthMax / 4); //take damage equal to 1/4 of max health (checks if it hits first so that invalid selections don't damage the unit)
                    if (healthCurrent <= 0)
                    {
                        healthCurrent = 1;
                    }
                    actionUsed = true;
                }
                break;
            case characterClass.mage: //attacks in a 3x3 square centered on (targetX, targetY) for 1/3 base damage to all enemies
                for(int i = targetX - 1; i <= targetX + 1; i++)
                {
                    for(int j = targetY - 1; i <= targetY + 1; i++)
                    {
                        attackSquare(targetX, targetY, Mathf.RoundToInt(level / 3));
                    }
                }
                actionUsed = true;
                break;
            default:
                break;
        }
    }

    void SecondAbility(int targetX, int targetY) //second ability
    {
        switch (myClass) //checks this unit's character class then uses the corresponding ability
        {
            case characterClass.fighter: //doubles defense stat until the start of this unit's next turn
                fighterDefBuff = true;
                defenseStat *= 2;
                actionUsed = true;
                break;
            case characterClass.mage: //heals a single unit for half of its max health
                for(int i = 0; i < menu.playerUnits.Count; i++)
                {
                    if(menu.playerUnits[i].GetComponent<PlayerUnit>().gridX == targetX && menu.playerUnits[i].GetComponent<PlayerUnit>().gridY == targetY)
                    {
                        modifyHealth(menu.playerUnits[i].GetComponent<PlayerUnit>().getHealthMax / 2);
                        actionUsed = true;
                    }
                }
                break;
            default:
                break;
        }
    }

    public void StartTurn() //everything that happens at the start of the turn
    {
        turnCounter -= 100;
        movementUsed = false;
        actionUsed = false;
        if (fighterDefBuff)
        {
            fighterDefBuff = false;
            defenseStat /= 2;
        }
    }

    void EndTurn() //everything that happens at the end of the turn
    {
        turnCounter = 0;
    }

    public int getLevel() //returns level for other functions (this could also be done with get/set)
    {
        return level;
    }

    public void increaseLevel() //increases level by 1
    {
        if(level < LEVEL_CAP)
        {
            level++;//increases level
        }
        //recalculates stats based on character class and new level
        switch (myClass)
        {
            case characterClass.fighter:
                healthMax += HP_GROWTH_FIGHTER;
                attackStat += ATK_GROWTH_FIGHTER;
                defenseStat += DEF_GROWTH_FIGHTER;
                speedStat += SPD_GROWTH_FIGHTER;
                break;
            case characterClass.mage:
                healthMax += HP_GROWTH_MAGE;
                attackStat += ATK_GROWTH_MAGE;
                defenseStat += DEF_GROWTH_MAGE;
                speedStat += SPD_GROWTH_MAGE;
                break;
            default:break;
        }
        healthCurrent = healthMax;
    }

    public void modifyHealth(int amount)
    {
        healthCurrent += amount;
    }

    public int getHealthMax
    {
        get
        {
            return healthMax;
        }
    }
    public int getHealthCurrent
    {
        get
        {
            return healthCurrent;
        }
    }

    public int getAttack
    { 
        get
        {
            return attackStat;
        } 
    }
    public int getDefense
    {
        get
        {
            return defenseStat;
        }
    }
    public int getSpeed
    { 
        get
        { 
            return speedStat;
        }
    }

    public void restart()
    {
        this.Start();
    }

    public void setClass(characterClass input)
    {
        myClass = input;
    }

    public void setHealth(int value)
    {
        healthCurrent = value;
    }
}
