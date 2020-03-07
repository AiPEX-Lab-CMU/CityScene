using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MarketItems : MonoBehaviour
{
    // holds the list of items which have to be swapped
    public List<string> swapList1 = new List<string>();
    public List<string> swapList2 = new List<string>();

    // holds the list of items whose positions have been changed
    public List<string> changedItems = new List<string>();

    // list of all items in the supermarket
    public List<string> itemList = new List<string>();

    // Holds all the game objects of a particular item
    public Dictionary<string, List<GameObject>> map = new Dictionary<string, List<GameObject>>();

    // List of shelves and their names
    Dictionary<string, Shelf> shelves;
    Dictionary<string, ShelfSpace> shelfSpaces = new Dictionary<string, ShelfSpace>();

    // mapping all the items in the list (not the market) to its nearest marker
    Dictionary<string, string> markerMap = new Dictionary<string, string>();

    // maps all the items in the market to its nearest markers
    public Dictionary<string, List<string>> markerItems = new Dictionary<string, List<string>>();
    // Start is called before the first frame update
    // adding necessary physics to the objects, and populating the above variables
    void Start()
    {
        if (File.Exists(@"shelfData.txt"))
            changeLocation();
        else
            writeDetails();
        GameObject child = GameObject.Find("shelves");
        Transform[] markerList = GameObject.Find("MovementMarkers").GetComponentsInChildren<Transform>();
        foreach (Transform tf in child.GetComponentsInChildren<Transform>())
        {
            string thisItem = tf.gameObject.name;
            if(thisItem.ToLower().Contains("product") && !thisItem.Contains("LOD"))
            {
                Rigidbody rb = tf.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                SphereCollider sc = tf.gameObject.AddComponent<SphereCollider>();
                sc.radius = 0.1f;
                sc.isTrigger = true;
                string tmp = thisItem.Substring(thisItem.IndexOf('_')+1);
                int index = tmp.IndexOfAny(new char[]{'0','1','2','3','4','5','6','7','8','9',' '});
                string product;
                if (index != -1)
                    product = tmp.Substring(0, index);
                else
                    product = tmp;
                if (!itemList.Contains(product))
                {
                    itemList.Add(product);
                }
                if (map.ContainsKey(product))
                {
                    map[product].Add(tf.gameObject);
                }
                else
                {
                    List<GameObject> temp = new List<GameObject>();
                    temp.Add(tf.gameObject);
                    map.Add(product, temp);
                }
                foreach (Transform t in markerList)
                {
                    if (Vector3.Distance(tf.position, t.position) < 3)
                    {
                        if(!markerItems.ContainsKey(t.name))
                        {
                            List<string> temp = new List<string>();
                            markerItems.Add(t.name, temp);
                        }
                        markerItems[t.name].Add(product);
                        break;
                    }
                }
            }
        }
        loadMarkerMap();
        reloadMarkerMap();
        getDetails();
    }

    /* Change location of products only, suppose that crates and shelves are never moved */
    void changeLocation()
    {
        Dictionary<string, List<Transform>> locations = new Dictionary<string, List<Transform>>();
        GameObject child = GameObject.Find("shelves");
        foreach (Transform tf in child.GetComponentsInChildren<Transform>())
        {
            string thisItem = tf.gameObject.name;
            if (thisItem.ToLower().Contains("product") && !thisItem.Contains("LOD"))
            {
                if (!locations.ContainsKey(thisItem))
                {
                    List<Transform> temp = new List<Transform>();
                    locations.Add(thisItem, temp);
                }
                locations[thisItem].Add(tf);
            }
        }
        using (var reader = new StreamReader(@"shelfData.txt"))
        {
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] parts = line.Split(';');
                string shelfName = parts[0].Split(':')[0];
                Transform parent = GameObject.Find(shelfName).transform;
                //No crates
                if(parts.Length == 2)
                {
                    string[] parsed = parts[1].Split(':');
                    string[] coordinates = parsed[1].Split(',');
                    Transform product = locations[parsed[0]][0];
                    product.SetParent(parent);
                    float x = float.Parse(coordinates[0], System.Globalization.CultureInfo.InvariantCulture);
                    float y = float.Parse(coordinates[1], System.Globalization.CultureInfo.InvariantCulture);
                    float z = float.Parse(coordinates[2], System.Globalization.CultureInfo.InvariantCulture);
                    product.position = new Vector3(x, y, z);
                    locations[parsed[0]].Remove(product);
                }
                else
                {
                    string[] parsedCrates = parts[1].Split(':');
                    string[] parsedProducts = parts[2].Split(':');
                    string[] crateCoordicates = parsedCrates[1].Split(',');
                    string[] productCoordinates = parsedProducts[1].Split(',');
                    Transform crate = GameObject.Find(parsedCrates[0]).transform;
                    Transform product = locations[parsedProducts[0]][0];
                    product.SetParent(crate);
                    float x = float.Parse(productCoordinates[0], System.Globalization.CultureInfo.InvariantCulture);
                    float y = float.Parse(productCoordinates[1], System.Globalization.CultureInfo.InvariantCulture);
                    float z = float.Parse(productCoordinates[2], System.Globalization.CultureInfo.InvariantCulture);
                    product.position = new Vector3(x, y, z);
                    locations[parsedProducts[0]].Remove(product);
                }
            }
        }
        locations.Clear();
    }

    // populates the markerMap
    void loadMarkerMap()
    {
        markerMap.Add("orange", "Marker1");
        markerMap.Add("potatoes", "Marker13");
        markerMap.Add("yogurt", "Marker4");
        markerMap.Add("Cheese", "Marker4");
        markerMap.Add("butter", "Marker5");
        markerMap.Add("beef", "Marker7");
        markerMap.Add("chicken", "Marker8");
        markerMap.Add("onion", "Marker12");
        markerMap.Add("garlic", "Marker12");
        markerMap.Add("pepper_green", "Marker12");
        markerMap.Add("papertowels", "Marker19");
        markerMap.Add("bread_", "Marker29");
        markerMap.Add("milk", "Marker31");
        markerMap.Add("cola", "Marker35");
        markerMap.Add("beer", "Marker35");
        markerMap.Add("wvine", "Marker37");
    }

    void reloadMarkerMap()
    {
        Transform[] markerList = GameObject.Find("MovementMarkers").GetComponentsInChildren<Transform>();
        foreach (var entry in map)
        {
            foreach (Transform t in markerList)
            {
                if (Vector3.Distance(entry.Value[0].transform.position, t.position) < 3)
                {
                    string s = entry.Key;
                    markerMap[s] = t.name;
                    Debug.Log(s + ":" + markerMap[s]);
                    break;
                }
            }
        }
    }

    public Dictionary<string, string> getMarkerMap()
    {
        return markerMap;
    }

    // updates the markerMap for items whose positions have been changed (changedItems)
    public void updateMarkerMap()
    {
        Transform[] markerList = GameObject.Find("MovementMarkers").GetComponentsInChildren<Transform>();
        foreach (string s in changedItems)
        {
            foreach (Transform t in markerList)
            {
                if (Vector3.Distance(map[s][0].transform.position,t.position) < 3)
                {
                    markerMap[s] = t.name;
                    Debug.Log(s + ":" + markerMap[s]);
                    break;
                }
            }
        }
    }

    // search the product name based on the gameObject
    public string searchProduct(GameObject obj)
    {
        foreach(KeyValuePair<string, List<GameObject>> pair in map)
        {
            if (pair.Value.Contains(obj))
                return pair.Key;
        }
        return null;
    }

    // write position and parent details of each item in the supermarket to shelfData.txt
    public void writeDetails()
    {
        GameObject mainObj = GameObject.Find("shelves");
        Transform[] childTransforms = mainObj.GetComponentsInChildren<Transform>();
        List<string> data = new List<string>();
        foreach(Transform t in childTransforms)
        {
            if(t.gameObject.name.ToLower().Contains("product") && !t.gameObject.name.Contains("LOD"))
            {
                data.Add(getChildDetails(t));
            }
        }
        using (StreamWriter fin = new StreamWriter(@"shelfData.txt"))
        {
            foreach(string d in data)
            {
                fin.WriteLine(d);
            }
        }
    }

    //Load the shelfMap based on positions in shelfData.txt
    void generateShelfMap()
    {
        using (var reader = new StreamReader(@"shelfData.txt"))
        {
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string shelfName = line.Split(':')[0];
                if (!shelfSpaces.ContainsKey(shelfName))
                {
                    ShelfSpace s = new ShelfSpace(shelfName);
                    shelfSpaces.Add(shelfName, s);
                }
                string[] positions = line.Split(':')[line.Split(':').Length - 1].Split(',');
                Vector3 pos = new Vector3(float.Parse(positions[0]), float.Parse(positions[1]), float.Parse(positions[2]));
                shelfSpaces[shelfName].capacity++;
                shelfSpaces[shelfName].itemPositions.Add(pos);
            }
        }
    }

    // print details of the supermarket
    void printDetails()
    {
        foreach(KeyValuePair<string,Shelf> s in shelves)
        {
            Debug.Log(s.Value.getName() + ": position - " + s.Value.position);
            foreach(KeyValuePair<string,Item> item in s.Value.items)
            {
                Debug.Log("\t" + item.Value.getName() + ": count = " + item.Value.count);
                Debug.Log("\t Has container : " + item.Value.hasBox);
                if(item.Value.hasBox)
                {
                    foreach(KeyValuePair<string,Box> box in item.Value.containers)
                    {
                        Debug.Log("\t Container name: " + box.Key + " : position - " + box.Value.position);
                    }
                }
                for(int j=0; j<item.Value.objectNames.Count;j++)
                {
                    Debug.Log("\t\t Object Name - " + item.Value.objectNames[j] + " : position - " + item.Value.positions[j]);
                }
            }
        }
    }

    // adds the object which was clicked on to the list
    public void addToList(GameObject obj)
    {
        string item = searchProduct(obj);
        if(obj.name.Equals("ray_store_fruits_002"))
        {
            item = "salad";
        }
        else if(obj.name.Equals("ray_store_fruits_002 (1)"))
        {
            item = "potatoes";
        }
        if (item != null)
        {
            canvasHandler ch = GameObject.Find("MainCanvas").GetComponent<canvasHandler>();
            if (ch.currentList == 1)
            {
                if (!swapList1.Contains(item))
                {
                    swapList1.Add(item);
                    ch.addToList(item);
                    Debug.Log(item + " added to list 1.");
                }
            }
            else
            {
                if (!swapList2.Contains(item) && !swapList1.Contains(item))
                {
                    swapList2.Add(item);
                    ch.addToList(item);
                    Debug.Log(item + " added to list 2.");
                }
            }
        }
    }

    // swaps the items in the swap lists, gets all the gameobjects in to and from lists
    // swaps the positions and parents of corresponding items. Excess items are discarded
    public void swap()
    {
        List<GameObject> from = new List<GameObject>();
        List<GameObject> to = new List<GameObject>();
        int toCount, fromCount;
        foreach(string s in swapList1)
        {
            if (!changedItems.Contains(s))
                changedItems.Add(s);
            from.AddRange(map[s]);
        }
        foreach (string s in swapList2)
        {
            if (!changedItems.Contains(s))
                changedItems.Add(s);
            to.AddRange(map[s]);
        }
        toCount = to.Count;
        fromCount = from.Count;
        int swapCount = 0;
        if (toCount < fromCount)
        {
            swapCount = toCount;
        }
        else
        {
            swapCount = fromCount;
        }
        for(int i=0; i<swapCount; i++)
        {
            Vector3 temp = new Vector3();
            temp = from[i].transform.position;
            from[i].transform.position = to[i].transform.position;
            to[i].transform.position = temp;
            Transform parent1 = to[i].transform.parent;
            Transform parent2 = from[i].transform.parent;
            from[i].transform.parent = parent1;
            to[i].transform.parent = parent2;
        }
        writeDetails();
    }

    // old method to swap individual items and not lists of items
    public void swapItems(string toItem, string fromItem)
    {
        Debug.Log("swapItems called");
        if (!toItem.Equals(fromItem))
        {
            Item to = new Item("none");
            Item from = new Item("none");
            foreach (KeyValuePair<string, Shelf> shlf in shelves)
            {
                foreach (KeyValuePair<string, Item> item in shlf.Value.items)
                {
                    if (item.Value.getName().Equals(toItem))
                    {
                        //toParent = GameObject.Find(shlf.Value.getName());
                        to = item.Value;
                    }
                    if (item.Value.getName().Equals(fromItem))
                    {
                        //fromParent = GameObject.Find(shlf.Value.getName());
                        from = item.Value;
                    }
                }
            }
            int minCount=0;
            if (to.count < from.count)
                minCount = to.count;
            else
                minCount = from.count;
            Debug.Log(minCount);
            for(int i=0; i<minCount;i++)
            {
                GameObject toObj = findItemByParent(to.objectNames[i], to.parentName[i]);
                GameObject fromObj = findItemByParent(from.objectNames[i], from.parentName[i]);
                Debug.Log(toObj.name + "-" + fromObj.name);
                Vector3 temp = toObj.transform.position;
                toObj.transform.position = fromObj.transform.position;
                fromObj.transform.position = temp;
                toObj.transform.SetParent(GameObject.Find(from.parentName[i]).transform);
                fromObj.transform.SetParent(GameObject.Find(to.parentName[i]).transform);
            }
            writeDetails();
            getDetails();
        }
    }

    // gets an item based by matching name and parent object name
    GameObject findItemByParent(string objName, string parentName)
    {
        GameObject parent = GameObject.Find(parentName);
        if (parent.transform.Find(objName) != null)
            return parent.transform.Find(objName).gameObject;
        return null;
    }

    // gets an item based on the object name and position
    GameObject findItemByPosition(string objName, Vector3 position)
    {
        GameObject shelf = GameObject.Find("shelves");
        foreach(Transform tf in shelf.GetComponentsInChildren<Transform>())
        {
            if (tf.gameObject.name.Equals(objName) && tf.position.Equals(position))
            {
                Debug.Log("match " + objName);
                return tf.gameObject;
            }
        }
        Debug.Log("nomatch - " + objName + ":" + position);
        return null;
    }

    // load the shelves dictionary, boxes and items for every line in shelfData.txt
    // some items are placed inside a box while some are direclty placed on the
    // shelf. Splitting the line based on ';'. If splits into 3, that means that
    // the items are placed inside boxes. For example, Onions are placed inside a
    // container which is placed in a shelf. If splits into 2, it means that the item
    // is placed directly on the shelf. Shelf, Boxes and Items together form the data
    // structure which represents the supermarket.
    void getDetails()
    {
        using (var reader = new StreamReader(@"shelfData.txt"))
        {
            shelves = new Dictionary<string, Shelf>();
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] parts = line.Split(';');
                string shelfName = parts[0].Split(':')[0];
                if (!shelves.ContainsKey(shelfName))
                {
                    Shelf s = new Shelf(shelfName);
                    s.position.x = float.Parse(parts[0].Split(':')[1].Split(',')[0]);
                    s.position.y = float.Parse(parts[0].Split(':')[1].Split(',')[1]);
                    s.position.z = float.Parse(parts[0].Split(':')[1].Split(',')[2]);
                    shelves.Add(shelfName, s);
                }
                if (parts.Length==3)
                {
                    string itemName = getName(parts[2].Split(':')[0]);
                    if(shelves[shelfName].items.ContainsKey(itemName))
                    {
                        shelves[shelfName].items[itemName].count++;
                        string[] pos = parts[2].Split(':')[1].Split(',');
                        Vector3 v = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                        shelves[shelfName].items[itemName].objectNames.Add(parts[2].Split(':')[0]);
                        shelves[shelfName].items[itemName].positions.Add(v);
                        shelves[shelfName].items[itemName].parentName.Add(parts[1].Split(':')[0]);
                        if(!shelves[shelfName].items[itemName].containers.ContainsKey(parts[1].Split(':')[0]))
                        {
                            string[] boxP = parts[1].Split(':')[1].Split(',');
                            Vector3 boxPositions = new Vector3(float.Parse(boxP[0]), float.Parse(boxP[1]), float.Parse(boxP[2]));
                            Box b = new Box(parts[1].Split(':')[0]);
                            b.position = boxPositions;
                            shelves[shelfName].items[itemName].containers.Add(parts[1].Split(':')[0], b);
                        }
                    }
                    else
                    {
                        Item i = new Item(itemName);
                        i.hasBox = true;
                        Box b = new Box(parts[1].Split(':')[0]);
                        string[] boxP = parts[1].Split(':')[1].Split(',');
                        Vector3 boxPosition = new Vector3(float.Parse(boxP[0]), float.Parse(boxP[1]), float.Parse(boxP[2]));
                        b.position = boxPosition;
                        i.containers.Add(parts[1].Split(':')[0], b);
                        string[] itemP = parts[2].Split(':')[1].Split(',');
                        Vector3 itemPosition = new Vector3(float.Parse(itemP[0]), float.Parse(itemP[1]), float.Parse(itemP[2]));
                        i.objectNames.Add(parts[2].Split(':')[0]);
                        i.positions.Add(itemPosition);
                        //Debug.Log(parts[1].Split(':'));
                        i.parentName.Add(parts[1].Split(':')[0]);
                        shelves[shelfName].items.Add(itemName, i);
                    }
                }
                else if(parts.Length ==2)
                {
                    string itemName = getName(parts[1].Split(':')[0]);
                    if(shelves[shelfName].items.ContainsKey(itemName))
                    {
                        shelves[shelfName].items[itemName].count++;
                        string[] pos = parts[1].Split(':')[1].Split(',');
                        Vector3 v = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                        shelves[shelfName].items[itemName].objectNames.Add(parts[1].Split(':')[0]);
                        shelves[shelfName].items[itemName].positions.Add(v);
                        shelves[shelfName].items[itemName].parentName.Add(parts[0].Split(':')[0]);
                    }
                    else
                    {
                        Item i = new Item(itemName);
                        i.hasBox = false;
                        string[] itemP = parts[1].Split(':')[1].Split(',');
                        Vector3 itemPosition = new Vector3(float.Parse(itemP[0]), float.Parse(itemP[1]), float.Parse(itemP[2]));
                        i.objectNames.Add(parts[1].Split(':')[0]);
                        i.positions.Add(itemPosition);
                        i.parentName.Add(parts[0].Split(':')[0]);
                        shelves[shelfName].items.Add(itemName, i);
                    }
                }
            }
        }
    }

    // String manipulation to get the item name from the name of its gameobject
    string getName(string thisItem)
    {
        string tmp = thisItem.Substring(thisItem.IndexOf('_') + 1);
        int index = tmp.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' });
        string product;
        if (index != -1)
            product = tmp.Substring(0, index);
        else
            product = tmp;
        return product;
    }

    // Function to print a gameobject's heirarchy details, i.e shelf details;box details;item details
    string getChildDetails(Transform tf)
    {
        string data = tf.gameObject.name + ":" + tf.position.x + "," + tf.position.y + "," + tf.position.z;
        tf = tf.parent;
        while(!tf.gameObject.name.Equals("shelves"))
        {
            data = tf.gameObject.name + ":" + tf.position.x + "," + tf.position.y + "," + tf.position.z + ";" + data;
            tf = tf.parent;
        }
        return data;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
