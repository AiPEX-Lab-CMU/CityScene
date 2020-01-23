using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class customerHandler : MonoBehaviour
{
    public GameObject customer1;
    public GameObject customer2;
    public GameObject customer3;
    public GameObject customer4;
    public GameObject customer5;
    public GameObject customer6;
    public GameObject customer7;
    GameObject[] customers = new GameObject[7];
    GameObject[] players = new GameObject[20];
    int playerCount;
    int minWaitTime = 10;
    int maxWaitTime = 20;
    // Start is called before the first frame update
    void Start()
    {
        customers[0] = customer1;
        customers[1] = customer2;
        customers[2] = customer3;
        customers[3] = customer4;
        customers[4] = customer5;
        customers[5] = customer6;
        customers[6] = customer7;
        System.Random random = new System.Random();
        GameObject market = GameObject.Find("Green_Market");
        for(int i=0; i<20; i++)
        {
            int index = random.Next(7);
            players[i] = Instantiate(customers[index]);
            players[i].transform.parent = market.transform;
            players[i].SetActive(false);
            playerCount++;
        }
        //StartCoroutine(startController());
        //GameObject market = GameObject.Find("Green_Market");
        //GameObject trolley = Instantiate(myPrefab);
        //trolley.transform.parent = market.transform;
    }

    public void startSim()
    {
        Button startButton = GameObject.Find("StartButton").GetComponent<Button>();
        Button changeButton = GameObject.Find("ChangeButton").GetComponent<Button>();
        Button backButton = GameObject.Find("BackButton").GetComponent<Button>();
        backButton.interactable = false;
        startButton.interactable = false;
        changeButton.interactable = false;
        
        StartCoroutine(startController());
    }

    //IEnumerator startController()
    //{
    //    System.Random random = new System.Random();
    //    while (true)
    //    {
    //        Debug.Log("Creating Character");
    //        int index = random.Next(7);
    //        GameObject market = GameObject.Find("Green_Market");
    //        GameObject trolley = Instantiate(customers[index]);
    //        trolley.transform.parent = market.transform;
    //        int wait = 0;
    //        do
    //        {
    //            wait = random.Next(maxWaitTime);
    //        } while (wait < minWaitTime);
    //        Debug.Log("Next customer in " + wait + " seconds.");
    //        yield return new WaitForSecondsRealtime(wait);
    //    }
    //}




    IEnumerator startController()
    {
        System.Random random = new System.Random();
        for(int i=0; i<20; i++)
        {
            Debug.Log("Creating Character");
            //int index = random.Next(7);
            //GameObject market = GameObject.Find("Green_Market");
            //GameObject trolley = Instantiate(customers[index]);
            //trolley.transform.parent = market.transform;
            players[i].SetActive(true);
            players[i].GetComponent<MoveTrolley>().load();
            //players[i].GetComponent<MoveTrolley>().isLoaded = true;
            int wait = 0;
            do
            {
                wait = random.Next(maxWaitTime);
            } while (wait < minWaitTime);
            Debug.Log("Next customer in " + wait + " seconds.");
            yield return new WaitForSecondsRealtime(wait);
        }
    }


    // Update is called once per frame
    void Update()
    {
    }
}
