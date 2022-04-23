using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public ManagementMenu menu;
    int healthMax;
    int healthCurrent;
    int attackStat;
    public int defenseStat;
    public int speedStat;

    public int gridX;
    public int gridY;

    public int enemyLevel;

    public int turnCounter = 0;

    SpriteRenderer enemySprite;

    public Sprite meleeImage;
    public Sprite rangedImage;

    //enemy base stats (placeholder values)
    const int START_HP = 10;
    const int START_ATK = 10;
    const int START_DEF = 10;
    const int START_SPD = 10;

    //enemy stat growths (placeholder values)
    const int HP_GROWTH = 5;
    const int ATK_GROWTH = 2;
    const int DEF_GROWTH = 2;
    const int SPD_GROWTH = 2;
    enum enemyType { melee, ranged }//specifies which type of enemy this is

    Vector3 posVector = Vector3.zero;

    enemyType myType;

    // Start is called before the first frame update
    void Start()
    {

        healthMax = START_HP + (enemyLevel - 1) * HP_GROWTH;
        attackStat = START_ATK + (enemyLevel - 1) * ATK_GROWTH;
        defenseStat = START_DEF + (enemyLevel - 1) * DEF_GROWTH;
        speedStat = START_SPD + (enemyLevel - 1) * SPD_GROWTH;
    }

    // Update is called once per frame
    void Update()
    {
        posVector.x = gridX;
        posVector.y = gridY;
        posVector.z = 1;
        enemySprite.transform.position = posVector;
    }

    public void takeDamage(int amount)
    {
        healthCurrent -= amount;
    }

    public void enemyTurn(int target)
    {
        int tx = menu.playerUnits[target].GetComponent<PlayerUnit>().gridX;
        int ty = menu.playerUnits[target].GetComponent<PlayerUnit>().gridY;

        switch (myType)
        {
            case enemyType.melee:
                //calculate which direction (u/d/l/r) and amount of squares moved (up to the maximum movement amount) would get this enemy into melee range of the target with the least squares moved (if possible)
                int[] maxDistance = getMaxDistance();
                
                //if the target is in melee range, attack it
                break;
            case enemyType.ranged:
                //check which direction would get this enemy as far from the nearest player unit as possible, then move that direction
                //attack the target unit
                break;
            default:
                break;
        }
    }

    int[] getMaxDistance()//gets the maximum distance that this unit can move in each direction
    {
        int[] directions = { 0, 0, 0, 0 };//up, down, left, right
        for(int i = 0; i < speedStat/3; i++)
        {
            directions[0] = i;
            if (isImpassable(gridX + i, gridY))
            {
                break;
            }
        }
        for(int i = 0; i < speedStat/3; i++)
        {
            directions[1] = i;
            if (isImpassable(gridX - i, gridY))
            {
                break;
            }
        }
        for (int i = 0; i < speedStat / 3; i++)
        {
            directions[2] = i;
            if (isImpassable(gridX, gridY + i))
            {
                break;
            }
        }
        for (int i = 0; i < speedStat / 3; i++)
        {
            directions[3] = i;
            if (isImpassable(gridX, gridY- i))
            {
                break;
            }
        }
        //check up to this unit's theoretical maximum movement distance in all 4 directions
        //stop at water tiles and player/enemy units
        //update values to reflect how far this unit can move
        return directions;//returns an array of how far the unit can move in each direction
    }

    bool isImpassable(int x, int y)
    {
        foreach(GameObject i in menu.playerUnits)
        {
            if(i.GetComponent<PlayerUnit>().gridX == x && i.GetComponent<PlayerUnit>().gridY == y)
            {
                return true;
            }
        }
        if (!ManagementMenu.mapList[ManagementMenu.currentMap][x, y])
        {
            return true;
        }
        if(x < 0 || y < 0 || x > 8 || y > 8)
        {
            return true;
        }
        return false;
    }

    float compareDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
}
