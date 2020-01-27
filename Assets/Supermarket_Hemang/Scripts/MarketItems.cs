using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MarketItems : MonoBehaviour
{
    public List<string> swapList1 = new List<string>();
    public List<string> swapList2 = new List<string>();
    public List<string> changedItems = new List<string>();
    public List<string> itemList = new List<string>();
    public Dictionary<string, List<GameObject>> map = new Dictionary<string, List<GameObject>>();
    Dictionary<string, Shelf> shelves;
    Dictionary<string, ShelfSpace> shelfSpaces = new Dictionary<string, ShelfSpace>();
    Dictionary<string, string> markerMap = new Dictionary<string, string>();
    public Dictionary<string, List<string>> markerItems = new Dictionary<string, List<string>>();
    // Start is called before the first frame update
    void Start()
    {
        GameObject child = GameObject.Find("shelves");
        Transform[] markerList = GameObject.Find("MovementMarkers").GetComponentsInChildren<Transform>();
        foreach (Transform tf in child.GetComponentsInChildren<Transform>())
        {
            string thisItem = tf.gameObject.name;
            if(thisItem.ToLower().Contains("product") && !thisItem.Contains("LOD"))
            {
                //if (thisItem.ToLower().Contains("mayo"))
                //{
                    Rigidbody rb = tf.gameObject.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                //BoxCollider bc = tf.gameObject.AddComponent<BoxCollider>();
                //bc.size = new Vector3(0.1f, 0.1f, 0.1f);
                //bc.isTrigger = true;
                    SphereCollider sc = tf.gameObject.AddComponent<SphereCollider>();
                sc.radius = 0.1f;
                sc.isTrigger = true;
                //}
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
        //writeDetails();
        loadMarkerMap();
        getDetails();
        //printDetails();
        
    }

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

    public Dictionary<string, string> getMarkerMap()
    {
        return markerMap;
    }

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


    public string searchProduct(GameObject obj)
    {
        foreach(KeyValuePair<string, List<GameObject>> pair in map)
        {
            if (pair.Value.Contains(obj))
                return pair.Key;
        }
        return null;
    }

    void writeDetails()
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
        using (StreamWriter fin = new StreamWriter(@"Assets/Supermarket_Hemang/Scripts/shelfData.txt"))
        {
            foreach(string d in data)
            {
                fin.WriteLine(d);
            }
        }
    }

    void generateShelfMap()
    {
        using (var reader = new StreamReader(@"Assets/Supermarket_Hemang/Scripts/shelfData.txt"))
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

    void printDetails()
    {
        //int i = 1;
        foreach(KeyValuePair<string,Shelf> s in shelves)
        {
            Debug.Log(s.Value.getName() + ": position - " + s.Value.position);
            foreach(KeyValuePair<string,Item> item in s.Value.items)
            {
                Debug.Log("\t" + item.Value.getName() + ": count = " + item.Value.count);
                Debug.Log("\t Has container : " + item.Value.hasBox);
                if(item.Value.hasBox)
                {
                    //Debug.Log("\t Container name: " + item.Value.container.getName() + " : position = " + item.Value.container.position);
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

    }

    public void swapItems(string toItem, string fromItem)
    {
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
                //GameObject toObj = findItemByPosition(to.objectNames[i], to.positions[i]);
                //GameObject fromObj = findItemByPosition(from.objectNames[i], from.positions[i]);
                GameObject toObj = findItemByParent(to.objectNames[i], to.parentName[i]);
                GameObject fromObj = findItemByParent(from.objectNames[i], from.parentName[i]);
                Debug.Log(toObj.name + "-" + fromObj.name);
                Vector3 temp = toObj.transform.position;
                toObj.transform.position = fromObj.transform.position;
                fromObj.transform.position = temp;
                //Debug.Log(from.positions[i]);
                //toObj.transform.position.Set(from.positions[i].x, from.positions[i].y, from.positions[i].z);
                //toObj.transform.position = from.positions[i];
                //fromObj.transform.position = to.positions[i];
                toObj.transform.SetParent(GameObject.Find(from.parentName[i]).transform);
                fromObj.transform.SetParent(GameObject.Find(to.parentName[i]).transform);
            }
            writeDetails();
            getDetails();
        }
    }

    GameObject findItemByParent(string objName, string parentName)
    {
        GameObject parent = GameObject.Find(parentName);
        if (parent.transform.Find(objName) != null)
            return parent.transform.Find(objName).gameObject;
        return null;
    }

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

    void getDetails()
    {
        using (var reader = new StreamReader(@"Assets/Supermarket_Hemang/Scripts/shelfData.txt"))
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
