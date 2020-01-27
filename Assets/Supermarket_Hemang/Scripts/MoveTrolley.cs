using System.Collections;
using System.Collections.Generic;
using GameCreator.Core;
using GameCreator.Playables;
using GameCreator.Characters;
using GameCreator.Variables;
using UnityEngine;
using System;

public class MoveTrolley : MonoBehaviour
{
    Dictionary<string, List<string>> markerItems;
    int waitTime = 40;
    ShoppingList sl;
    bool isMoving = false;
    bool isPicking = false;
    bool isComplete = false;
    bool isRoundComplete = true;
    string nextItem = "";
    string nextMarker = "";
    GameObject targetObj;
    int markerCount = 0;
    LocalVariables lv;
    int itemsPicked = 0;
    int totalItems;
    int index;
    string message;
    DateTime start = new DateTime();
    DateTime end = new DateTime();
    public bool isLoaded;
    int salesManID;
    int unavailable = 0;
    Dictionary<string, string> markerMap = new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        //isLoaded = false;
        //StartCoroutine(load());
        //markerItems = new Dictionary<string, List<string>>();
        //markerCount = 0;
        //sl = transform.GetComponent<ShoppingList>();
        //do
        //{
        //    sl.generateShoppingList();
        //} while (sl.items.Count <= 0);
        //lv = transform.Find("TrolleyLocal").GetComponent<LocalVariables>();
        //lv.Get("isMoving").Set(Variable.DataType.Bool, false);
        //lv.Get("isPicking").Set(Variable.DataType.Bool, false);
        //lv.Get("isComplete").Set(Variable.DataType.Bool, false);
        //lv.Get("isRoundComplete").Set(Variable.DataType.Bool, true);
        //itemsPicked = 0;
        //totalItems = sl.items.Count;
        //getNearbyItems();
        //message = "Items bought: ";
        //Debug.Log("Total items = " + totalItems);
    }



    public void load()
    {
        isLoaded = false;
        markerItems = new Dictionary<string, List<string>>();
        //markerCount = 0;
        sl = transform.GetComponent<ShoppingList>();
        //do
        //{
        //    sl.generateShoppingList();
        //} while (sl.items.Count <= 0);
        sl.generateShoppingListBySoftmax();
        //lv = transform.Find("TrolleyLocal").GetComponent<LocalVariables>();
        //lv.Get("isMoving").Set(Variable.DataType.Bool, false);
        //lv.Get("isPicking").Set(Variable.DataType.Bool, false);
        //lv.Get("isComplete").Set(Variable.DataType.Bool, false);
        //lv.Get("isRoundComplete").Set(Variable.DataType.Bool, true);
        //itemsPicked = 0;
        totalItems = sl.items.Count;
        if (totalItems <= 3)
        {
            Destroy(transform.Find("helper_trolley_body").gameObject);
            Actions actions = transform.Find("MoveItem").GetComponent<Actions>();
            ActionTransformMove move = actions.actionsList.actions[0].GetComponent<ActionTransformMove>();
            move.moveTo.targetTransform = transform.Find("Basket");
        }
        else
        {
            Destroy(transform.Find("Basket").gameObject);
        }
        loadMarkerMap();
        getNearbyItems();
        message = "Items bought: ";
        isLoaded = true;
        start = DateTime.Now;
        StartCoroutine(randPickItem());
    }
    // Update is called once per frame
    void Update()
    {
        if (isLoaded)
        {
            if (!isComplete && !isMoving)
            {
                if (isRoundComplete)
                {
                    if (markerCount <= 39)
                    {
                        //move cart to next market
                        isRoundComplete = false;
                        isMoving = true;
                        nextMarker = "Marker" + markerCount;
                        Actions actions = transform.Find("Character").GetComponent<Actions>();
                        actions.Execute();
                        moveToMarker();
                    }
                    else
                    {
                        isComplete = true;
                        checkout();
                    }
                }

                else if (!isRoundComplete && !isPicking)
                {

                    if (markerItems.ContainsKey(nextMarker) && index < markerItems[nextMarker].Count)
                    {
                        isPicking = true;
                        nextItem = markerItems[nextMarker][index];
                        //message = message + "\t" + nextItem;
                        index++;
                        moveItemToCart();
                    }
                    else
                    {
                        endRound();
                    }
                }
            }
        }

    }



    void loadMarkerMap()
    {
        MarketItems mi = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        markerMap = mi.getMarkerMap();
    }

    IEnumerator randPickItem()
    {
        MarketItems mi = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        System.Random rand = new System.Random();
        while (!isComplete)
        {
            yield return new WaitForSeconds(waitTime);
            if (!isComplete)
            {
                // get player's next marker
                nextMarker = "Marker" + markerCount;
                // find items near next marker (use the markerMap variable)
                int x = mi.markerItems[nextMarker].Count;
                int ind = rand.Next(x);
                if(!markerItems.ContainsKey(nextMarker))
                {
                    List<string> temp = new List<string>();
                    markerItems.Add(nextMarker, temp);
                }
                markerItems[nextMarker].Add(mi.markerItems[nextMarker][ind]);
                totalItems++;
                Debug.Log("Added item " + mi.markerItems[nextMarker][ind] + "to the list.");
                //foreach (KeyValuePair<string, string> entry in markerMap)
                //{
                //    if (entry.Value == nextMarker) // verify the same marker as nextMarker
                //    {
                //        // Add the item to that marker (Add to markerItem variable)
                //        if (!markerItems[nextMarker].Contains(entry.Key)) // item is currently not in the markerItems
                //        {
                //            markerItems[nextMarker].Add(entry.Key);
                //        }
                //    }
                //}
                // to add or remove from dictionary, refer other functions in the MoveTrolley.cs script
            }
        }
    }

    public void checkShoppingComplete()
    {
        if(itemsPicked>=totalItems)
        {
            isComplete = true;
            Debug.Log("Shopping Complete");
            checkout();
        }
    }


    void endRound()
    {
        markerCount++;
        index = 0;
        isRoundComplete = true;
    }

    void moveItemToCart()
    {
        Actions actions = transform.Find("MoveItem").GetComponent<Actions>();
        ActionTransformMove itemMoveAction = actions.actionsList.actions[0].GetComponent<ActionTransformMove>();
        targetObj = findItemObject(nextItem);
        itemsPicked++;
        if (targetObj != null)
        {
            message = message + "\t" + nextItem;
            itemMoveAction.target.gameObject = targetObj;
            ActionRigidbody actionRigidbody = actions.actionsList.actions[1].GetComponent<ActionRigidbody>();
            actionRigidbody.target.gameObject = targetObj;
            //itemsPicked++;
            actions.Execute();
        }
        else
        {
            Debug.Log("Customer wanted " + nextItem + ", but it is not available.");
            unavailable++;
            isPicking = false;
            checkShoppingComplete();
        }
        
    }


    void moveToMarker()
    {
        Actions actions = transform.Find("MoveTrolley").GetComponent<Actions>();
        ActionCharacterMoveTo charMove = actions.actionsList.actions[0].GetComponent<ActionCharacterMoveTo>();
        charMove.marker = GameObject.Find(nextMarker).GetComponent<NavigationMarker>();
        actions.Execute();
    }


    public void getNearbyItems()
    {

        foreach(string item in sl.items)
        {
            if(markerItems.ContainsKey(markerMap[item]))
            {
                markerItems[markerMap[item]].Add(item);
            }
            else
            {
                List<string> temp = new List<string>();
                temp.Add(item);
                markerItems.Add(markerMap[item], temp);
            }
        }
    }

    GameObject findItemObject(string item)
    {
        GameObject child = GameObject.Find("shelves");
        foreach (Transform tf in child.GetComponentsInChildren<Transform>())
        {
            string thisItem = tf.gameObject.name;
            if(thisItem.ToLower().Contains(item.ToLower()) && thisItem.ToLower().Contains("product") && !thisItem.ToLower().Contains("LOD"))
            {
                return tf.gameObject;
            }
        }
        return null;
    }

    void checkout()
    {
        Actions actions1 = transform.Find("Character").GetComponent<Actions>();
        actions1.Execute();
        double minDist = double.PositiveInfinity;
        int checkoutIndex = 0;
        GameObject chkout = GameObject.Find("Checkout");
        for(int i=0; i<=6; i=i+2)
        {
            double x = System.Math.Pow((chkout.transform.GetChild(i).transform.position.x - transform.position.x), 2);
            double z = System.Math.Pow((chkout.transform.GetChild(i).transform.position.z - transform.position.z), 2);
            double dist = System.Math.Sqrt((x + z));
            if(dist<minDist)
            {
                minDist = dist;
                checkoutIndex = i/2;
            }
        }
        salesManID = checkoutIndex;
        Actions actions = transform.Find("MoveToCheckout").GetComponent<Actions>();
        ActionCharacterMoveTo[] moveTrolley = actions.actionsList.actions[0].GetComponents<ActionCharacterMoveTo>();
        moveTrolley[0].marker = chkout.transform.Find("CheckoutMarker" + checkoutIndex).GetComponent<NavigationMarker>();
        moveTrolley[1].marker = chkout.transform.Find("CheckoutMarker"+ checkoutIndex +"Counter").GetComponent<NavigationMarker>();
        moveTrolley[2].marker = chkout.transform.Find("Exit0").GetComponent<NavigationMarker>();
        moveTrolley[3].marker = chkout.transform.Find("Exit1").GetComponent<NavigationMarker>();
        actions.Execute();
        //Change the actions for checkout
        //execute the actions
    }

    public void sendCheckoutMessage()
    {
        Actions actions = GameObject.Find("Checkout").transform.Find("Sales" + salesManID).GetComponent<Actions>();
        actions.Execute();
        end = DateTime.Now;
        TimeSpan time = end - start;
        //Data to be sent to Python
        Debug.Log(message);
        Debug.Log("Unavailable items: " + unavailable);
        Debug.Log("Time Spent in the market = " + time.TotalSeconds + " seconds.");
    }

    //write a function to send checkout message 

    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.parent = null;
        collision.transform.SetParent(transform);
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Destroy(rb);
        SphereCollider sc = collision.gameObject.GetComponent<SphereCollider>();
        Destroy(sc);
        isPicking = false;
        checkShoppingComplete();
    }

    public void removeTrigger()
    {
        targetObj.GetComponent<SphereCollider>().isTrigger = false;
    }


    public void setIsMoving(bool val)
    {
        isMoving = val;
        Actions[] actions = transform.Find("Character").GetComponents<Actions>();
        actions[1].Execute();
    }

    public void setIsPicking(bool val)
    {
        isPicking = val;
    }

    public void setIsComplete(bool val)
    {
        isComplete = val;
    }

    public void setIsRoundComplete(bool val)
    {
        isRoundComplete = val;
    }
}
