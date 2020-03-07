using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasHandler : MonoBehaviour
{
    public GameObject changePanel;
    public int currentList = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getDefault()
    {
        GameObject.Find("SwapButton").GetComponent<Button>().interactable = false;
        GameObject.Find("List1Text").GetComponent<Text>().text = "List 1";
        GameObject.Find("List2Text").GetComponent<Text>().text = "List 2";
        currentList = 1;
        MarketItems mi = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        mi.swapList1 = new List<string>();
        mi.swapList2 = new List<string>();
    }

    public void finaliseList1()
    {
        currentList = 2;
        GameObject.Find("Clear1Button").GetComponent<Button>().interactable = false;
        GameObject.Find("List1Button").GetComponent<Button>().interactable = false;
    }

    public void finaliseList2()
    {
        GameObject.Find("Clear2Button").GetComponent<Button>().interactable = false;
        GameObject.Find("List2Button").GetComponent<Button>().interactable = false;
        GameObject.Find("SwapButton").GetComponent<Button>().interactable = true;
    }

    public void clear1()
    {
        Text listObj = GameObject.Find("List1Text").GetComponent<Text>();
        listObj.text = "List 1";
        GameObject.Find("Green_Market").GetComponent<MarketItems>().swapList1.Clear();
    }

    public void clear2()
    {
        Text listObj = GameObject.Find("List2Text").GetComponent<Text>();
        listObj.text = "List 2";
        GameObject.Find("Green_Market").GetComponent<MarketItems>().swapList2.Clear();
    }
    public void openChangePanel()
	{
        Button startbtn = GameObject.Find("StartButton").GetComponent<Button>();
        startbtn.interactable = false;
        Button changebtn = GameObject.Find("ChangeButton").GetComponent<Button>();
        changebtn.interactable = false;
        Button backbtn = GameObject.Find("BackButton").GetComponent<Button>();
        backbtn.interactable = true;
        changePanel.SetActive(true);
        //populateLists();
        GameObject.Find("Cameras").GetComponent<CamControl>().goToEditMode();
	}

    public void addToList(string item)
    {
        Text listObj = GameObject.Find("List" + currentList + "Text").GetComponent<Text>();
        listObj.text = listObj.text + "\n" + item;
        MarketItems mi = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        if(currentList==1)
        {
            if(mi.swapList1.Count>0)
            {
                GameObject.Find("Clear1Button").GetComponent<Button>().interactable = true;
                GameObject.Find("List1Button").GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            if (mi.swapList2.Count > 0)
            {
                GameObject.Find("Clear2Button").GetComponent<Button>().interactable = true;
                GameObject.Find("List2Button").GetComponent<Button>().interactable = true;
            }
        }
    }

    public void back()
    {
        Button startbtn = GameObject.Find("StartButton").GetComponent<Button>();
        startbtn.interactable = true;
        Button changebtn = GameObject.Find("ChangeButton").GetComponent<Button>();
        changebtn.interactable = true;
        Button backbtn = GameObject.Find("BackButton").GetComponent<Button>();
        backbtn.interactable = false;
        changePanel.SetActive(false);
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
