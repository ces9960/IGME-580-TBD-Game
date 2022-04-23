using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapTile : MonoBehaviour
{
    //Doesn't do anything, an alternative map solution was implemented
    SpriteRenderer tileRenderer;
    //public Sprite landTile;
    //public Sprite waterTile;

    //static string path;

    // Start is called before the first frame update
    void Start()
    {
        //path = Application.dataPath;
        //landTile = Resources.Load<Sprite>(path+ "/Sprites/grass_tile.png");
        //waterTile = Resources.Load<Sprite>(path + "/Sprites/water_tile.png");
        tileRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setLayer(int value)
    {
            transform.position = new Vector3(transform.position.x, transform.position.y, value);
    }

    public void tileInit()
    {
        Start();
    }
}
