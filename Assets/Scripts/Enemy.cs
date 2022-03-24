using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int healthMax;
    int healthCurrent;
    int attackStat;
    int defense;
    int speed;

    int enemyLevel;

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

    enemyType myType;

    // Start is called before the first frame update
    void Start()
    {
        healthMax = START_HP + (enemyLevel - 1) * HP_GROWTH;
        attackStat = START_ATK + (enemyLevel - 1) * ATK_GROWTH;
        defense = START_DEF + (enemyLevel - 1) * DEF_GROWTH;
        speed = START_SPD + (enemyLevel - 1) * SPD_GROWTH;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
