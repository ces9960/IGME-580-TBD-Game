using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmWindow : MonoBehaviour
{
    //setting keys to confirm/cancel
    static KeyCode confirmKey = KeyCode.Y;
    static KeyCode cancelKey = KeyCode.N;

    //is the confirm menu open?
    public static bool ConfirmMenuOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool openConfirmMenu() //handles confirm menu
    {
        ConfirmMenuOpen = true; //sets confirm menu to open
        Debug.Log("Confirm? Y/N");

        while(!Input.GetKey(confirmKey) && !Input.GetKey(cancelKey)) //loops through until the action is confirmed/cancelled
        {
            if (Input.GetKey(confirmKey))
            {
                ConfirmMenuOpen = false;
                return true;
            }
            if (Input.GetKey(cancelKey))
            {
                ConfirmMenuOpen = false;
                return false;
            }
        }
        ConfirmMenuOpen = false;
        return false;
    }
}
