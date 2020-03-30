using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Core;
using GameCreator.Playables;
using GameCreator.Characters;
using GameCreator.Variables;

public class Restock : MonoBehaviour
{
    private int nextMarkerVal = 0;
    private Camera cam;
    public float moveSpeed = 1.5f;
    public Dictionary<string, List<Snapshot>> snapshotLocations;
    SendMessage messageSender;
    bool isMoving = false;
    bool pictured = false;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        isMoving = true;
        cam = gameObject.transform.GetChild(0).GetComponent<Camera>();
        cam.enabled = false;
        messageSender = (SendMessage)GameObject.Find("messageSender").GetComponent<SendMessage>();
        snapshotLocations = new Dictionary<string, List<Snapshot>>();
        generateSnapshotLocations();
    }

    void moveToNextMarker()
    {
        isMoving = true;
        Actions actions = gameObject.transform.GetComponent<Actions>();
        ActionCharacterMoveTo charMove = actions.actionsList.actions[0].GetComponent<ActionCharacterMoveTo>();
        nextMarkerVal++;
        if (nextMarkerVal > 39)
            return;
        charMove.marker = GameObject.Find("Marker" + nextMarkerVal).GetComponent<NavigationMarker>();
        actions.Execute();
    }

    /* This function should be called when going to deploy the robot */
    public void generateSnapshotLocations()
    {
        MarketItems market = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        List<string> itemsNeeded = new List<string>();
        itemsNeeded.Add("orange");
        itemsNeeded.Add("potatoes");
        itemsNeeded.Add("yogurt");
        itemsNeeded.Add("Cheese");
        itemsNeeded.Add("butter");
        itemsNeeded.Add("beef");
        itemsNeeded.Add("chicken");
        itemsNeeded.Add("onion");
        itemsNeeded.Add("garlic");
        itemsNeeded.Add("pepper_green");
        itemsNeeded.Add("papertowels");
        itemsNeeded.Add("bread_");
        itemsNeeded.Add("milk");
        itemsNeeded.Add("cola");
        itemsNeeded.Add("beer");
        itemsNeeded.Add("wvine");
        foreach (string item in itemsNeeded)
        {
            string marker = market.markerMap[item];
            Debug.Log(marker + "+" + item);
            float distance = Random.Range(-3.0f, 1.0f);
            if (distance > -1.0f)
                distance += 2.0f;
            if (distance < 0.0f && marker == "Marker0")
                distance += 3.0f;
            List<GameObject> randomPickedOnes = new List<GameObject>();
            Transform markerPos = GameObject.Find(marker).transform;
            foreach(GameObject singleItem in market.map[item])
            {
                if (Vector3.Distance(singleItem.transform.position, markerPos.position) < 3)
                    randomPickedOnes.Add(singleItem);
            }
            int rndIdx = Random.Range(0, randomPickedOnes.Count - 1);
            Snapshot snap = new Snapshot(distance, randomPickedOnes[rndIdx]);
            if(!snapshotLocations.ContainsKey(marker))
            {
                List<Snapshot> snapshots = new List<Snapshot>();
                snapshotLocations.Add(marker, snapshots);
            }
            snapshotLocations[marker].Add(snap);
        }
        isMoving = false;
    }

    public void setIsMoving(bool moving)
    {
        Debug.Log("Called is moving");
        isMoving = moving;
    }

    /* Zooms in or out at a random coefficient and focuses on the object and takes a snapshot */
    private void takeSnapshot(GameObject product)
    {
        /* Focus on the product */
        cam.transform.LookAt(product.transform);
        cam.enabled = true;
        float zoomDistance = Random.Range(-15.0f, 15.0f);
        cam.fieldOfView += zoomDistance;
        int resWidth = 1280;
        int resHeight = 720;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        cam.targetTexture = rt;
        Texture2D screen = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        screen.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screen.EncodeToJPG();
        //messageSender.sendBytesInMemory("000", bytes);
        //call sendToPython function
        string filename = string.Format("{0}/screen" + count + ".jpg", Application.dataPath);
        System.IO.File.WriteAllBytes(filename, bytes);
        cam.fieldOfView -= zoomDistance;
        count++;
        cam.enabled = false;
    }

    private void takePicturesForMarker(int markerVal)
    {
        string markerStr = "Marker" + markerVal;
        if (snapshotLocations.ContainsKey(markerStr))
        {
            float actualDistance = Vector3.Distance(transform.position, GameObject.Find(markerStr).transform.position);
            List<Snapshot> snapshots = snapshotLocations[markerStr];
            for (int i = snapshots.Count - 1; i >= 0; i--)
            {
                float targetDistance = (markerVal < nextMarkerVal) ? snapshots[i].distance : -snapshots[i].distance;
                if(targetDistance >= 0.0f && targetDistance - 0.1f <= actualDistance && actualDistance<= targetDistance + 0.1f)
                {
                    takeSnapshot(snapshots[i].center);
                    snapshots.RemoveAt(i);
                }
            }
        }
    }

    //Update is called once per frame
    void Update()
    {
        if (nextMarkerVal > 39)
            return;
        GameObject nextMarker = GameObject.Find("Marker" + nextMarkerVal);
        Debug.Log("Look at Marker" + nextMarkerVal + " with distance of" + Vector3.Distance(gameObject.transform.position, nextMarker.transform.position));
        if (!isMoving)
        {
            Debug.Log("Go to next marker");
            moveToNextMarker();
        }
        Character character = gameObject.GetComponent<Character>();
        character.characterLocomotion.runSpeed = 0.0f;
        /* Take pictures for possible locations between the previous marker and the nextmarker) */
        takePicturesForMarker(nextMarkerVal - 1);
        takePicturesForMarker(nextMarkerVal);
        character.characterLocomotion.runSpeed = moveSpeed;
    }
}
