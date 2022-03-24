using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLevelMenuButton : MonoBehaviour
{
    public List<SubMenuButton> menuOptions = new List<SubMenuButton>();
    public int menuSize { get { return menuOptions.Count; }}
    public string buttonName;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddSubmenuButton()
    {
        menuOptions.Add(gameObject.AddComponent<SubMenuButton>());
    }

}
