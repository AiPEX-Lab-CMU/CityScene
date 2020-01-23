using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasHandler : MonoBehaviour
{
    public GameObject changePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openChangePanel()
	{
        Button startbtn = GameObject.Find("StartButton").GetComponent<Button>();
        startbtn.interactable = false;
        Button changebtn = GameObject.Find("ChangeButton").GetComponent<Button>();
        changebtn.interactable = false;
        Button backbtn = GameObject.Find("BackButton").GetComponent<Button>();
        backbtn.interactable = true;
        //changePanel.SetActive(true);
        //populateLists();
        GameObject.Find("Cameras").GetComponent<CamControl>().goToEditMode();
	}

    public void back()
    {
        Button startbtn = GameObject.Find("StartButton").GetComponent<Button>();
        startbtn.interactable = true;
        Button changebtn = GameObject.Find("ChangeButton").GetComponent<Button>();
        changebtn.interactable = true;
        Button backbtn = GameObject.Find("BackButton").GetComponent<Button>();
        backbtn.interactable = false;
        GameObject.Find("Cameras").GetComponent<CamControl>().goToPlayerMode();
    }

    public void populateLists()
    {
        MarketItems mi = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        Dropdown list1 = GameObject.Find("ObjList1").GetComponent<Dropdown>();
        Dropdown list2 = GameObject.Find("ObjList2").GetComponent<Dropdown>();
        list1.AddOptions(mi.itemList);
        list2.AddOptions(mi.itemList);
    }

    public void closeChangePanel()
    {
        changePanel.SetActive(false);
        Button startbtn = GameObject.Find("StartButton").GetComponent<Button>();
        startbtn.interactable = true;
        Button changebtn = GameObject.Find("ChangeButton").GetComponent<Button>();
        changebtn.interactable = true;
    }

    public void swap()
    {
        Dropdown list1 = GameObject.Find("ObjList1").GetComponent<Dropdown>();
        Dropdown list2 = GameObject.Find("ObjList2").GetComponent<Dropdown>();
        string obj1 = list1.options[list1.value].text;
        string obj2 = list2.options[list2.value].text;
        MarketItems mi = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        mi.swapItems(obj1, obj2);
    }
}
