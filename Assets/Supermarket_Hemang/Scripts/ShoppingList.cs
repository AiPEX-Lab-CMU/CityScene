using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingList : MonoBehaviour
{
    public List<string> items;

    // Start is called before the first frame update
    void Start()
    {
        items = new List<string>();
    }

    public void generateShoppingList()
    {
        System.Random rand = new System.Random();
        int index = rand.Next(4);
        ProfileController pc = gameObject.GetComponentInParent<ProfileController>();
        KeyValuePair<string, List<double>> prob = pc.getProfile(index);
        double final_prob = rand.NextDouble();
        Debug.Log(prob.Key + ":" + final_prob);
        for (int i = 0; i < prob.Value.Count; i++)
        {
            int rand_var = rand.Next(2);
            if ((prob.Value[i] * rand_var) >= (final_prob))
            {
                items.Add(pc.items[i]);
            }
        }
    }

    public void generateShoppingListBySoftmax()
    {
        System.Random rand = new System.Random();
        int index = rand.Next(4);
        ProfileController pc = gameObject.GetComponentInParent<ProfileController>();
        KeyValuePair<string, List<double>> prob = pc.getProfile(index);
        double final_prob = rand.NextDouble();
        int maxItems = rand.Next(15);
        while(maxItems==0)
        {
            maxItems = rand.Next(15);
        }
        int count = 0;
        
        while(count<maxItems)
        {
            List<KeyValuePair<int, double>> map = createSoftmaxMap(prob.Value);
            map.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            double remaining = final_prob;
            for(int i=0; i<map.Count; i++)
            {
                int rv = rand.Next(2);
                double probability = rv * map[i].Value;
                if(!probability.Equals(0) && probability<=remaining)
                {
                    count++;
                    items.Add(pc.items[map[i].Key]);
                    prob.Value[map[i].Key] = probability / 2;
                    remaining -= probability;
                    if (count == maxItems || remaining.Equals(0))
                        break;
                }
            }
        }
        Debug.Log("ItemCount : " + items.Count);
    }

    List<KeyValuePair<int,double>> createSoftmaxMap(List<double> probs)
    {
        List<KeyValuePair<int, double>> map = new List<KeyValuePair<int, double>>();
        double total = 0;
        foreach(double d in probs)
        {
            total += d;
        }
        for(int i=0; i<probs.Count; i++)
        {
            KeyValuePair<int, double> temp = new KeyValuePair<int, double>(i, (probs[i] / total));
            map.Add(temp);
        }
        return map;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
