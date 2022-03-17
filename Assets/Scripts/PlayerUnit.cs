using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    int healthCurrent, healthMax; //current and max HP
    int attackStat, defenseStat, speedStat; //stats for attack, defense, and speed
    int level; //this character's level
    public const int LEVEL_CAP = 5; //the maximum level that a character can be

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
        switch (myClass) // sets starting stats based on class
        {
            case characterClass.fighter:
                healthMax = HP_START_FIGHTER;
                healthCurrent = healthMax;
                attackStat = ATK_START_FIGHTER;
                defenseStat = DEF_START_FIGHTER;
                speedStat = SPD_START_FIGHTER;
                break;
            case characterClass.mage:
                healthMax = HP_START_MAGE;
                healthCurrent = healthMax;
                attackStat = ATK_START_MAGE;
                defenseStat = DEF_START_MAGE;
                speedStat = SPD_START_MAGE;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UnitMovement()
    {

    }

    void AttackAction()
    {

    }

    void FirstAbility() //first ability
    {
        switch (myClass) //checks this unit's character class then uses the corresponding ability
        {
            case characterClass.fighter: //currently unimplemented, will be an attack that does bonus damage to the target, but deals a small amount of damage to this unit
                break;
            case characterClass.mage: //currently unimplemented, will be an attack that deals damage to all enemies within a 3x3 square centered on the target square
                break;
            default:
                break;
        }
    }

    void SecondAbility() //second ability
    {
        switch (myClass) //checks this unit's character class then uses the corresponding ability
        {
            case characterClass.fighter: //currently unimplemented, will be an ability that reduces damage taken by this unit until the next turn
                break;
            case characterClass.mage: //currently unimplemented, will be an attack that deals damage to all enemies within a 3x3 square centered on the target square
                break;
            default:
                break;
        }
    }

    void StartTurn() //everything that happens at the start of the turn
    {

    }

    void EndTurn() //everything that happens at the end of the turn
    {

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
        //stat recalculations go here
    }
}
