using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    public List<string> items;
    List<string> profiles;
    Dictionary<string, List<double>> probabilities;
    // Start is called before the first frame update
    void Start()
    {
        items = new List<string>();
        probabilities = new Dictionary<string, List<double>>();
        profiles = new List<string>();
        populateLists();
    }

    void populateLists()
    {
        items.Add("orange");
        items.Add("potatoes");
        items.Add("yogurt");
        items.Add("Cheese");
        items.Add("butter");
        items.Add("beef");
        items.Add("chicken");
        items.Add("onion");
        items.Add("garlic");
        items.Add("pepper_green");
        items.Add("papertowels");
        items.Add("bread_");
        items.Add("milk");
        items.Add("cola");
        items.Add("beer");
        items.Add("wvine");

        profiles.Add("Vegetarian");
        profiles.Add("Meat");
        profiles.Add("Dairy");
        profiles.Add("Party");
        using(var reader = new StreamReader(@"Assets/Supermarket_Hemang/Scripts/profiles.csv"))
        {
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] words = line.Split(',');
                List<double> temp = new List<double>();
                for (int i = 1; i < words.Length; i++)
                {
                    temp.Add(double.Parse(words[i].Trim()));
                }
                probabilities.Add(words[0], temp);
                //Debug.Log(words[0]);
            }
        }
    }

    public KeyValuePair<string,List<double>> getProfile(int x)
    {
        return new KeyValuePair<string, List<double>>(profiles[x], probabilities[profiles[x]]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
