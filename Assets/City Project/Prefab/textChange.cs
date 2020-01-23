using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textChange : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    private string[] str;
    private int size;
    private int maxTimeInterval;
    //public GameObject gameObject;
    private string prehead = "<font=\"Bangers SDF\" material=\"Bangers SDF - Drop Shadow\"><mark=#ffffff80>";
    //<font="Bangers SDF" material="Bangers SDF - Drop Shadow"><mark=#ff000010>R1<mark=#00ff0010>G2<mark=#0000ff10>B3<mark=#00000064>A4

    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        size = 7;
        maxTimeInterval = 10;
        str = new string[size];
        initialStr(str, size);
        StartCoroutine(change(str, size, maxTimeInterval));
    }

    //tweeter sample database
    void initialStr(string[] str, int size)
    {
        str[0] = "The First Tweet!";
        str[1] = "The New Year’s Eve party is something you wouldn’t want to miss! This December 31 in the Taipei City Hall Square we will be showcasing the diverse walks of life who make Taipei the land of infinite possibilities.";
        str[2] = "In God We Trust";
        str[3] = "Thank you Ritchie!";
        str[4] = "Thinking of starting Intermittent Fasting? 🍽Install App And Get Your Personal Fasting Plan👇 https://dofasting.com";
        str[5] = "MAN i wish I walked into the locker room tomorrow and we had Christmas Day uniforms! It’s a MUST we bring those back @Nike";
        str[6] = "Turn on auto navigate feature & car will activate traffic-based navigation to work, home or calendar, depending on context, as soon as you sit down. No input required at all.";
    }
    IEnumerator change(string[] str, int size, int maxTimeInterval)
    {
        while (true)
        {
            //randomly generate a number
            int rand = Random.Range(0, 10);
            //if the number is 0 (probability of 1/10 to occur this event)
            //Debug.Log("rand =" + rand.ToString());
            if (rand == 0) {
                //gameObject.SetActive(true);
                //display random text from tweet database
                int strRandom = Random.Range(0, size);
                textMeshPro.text = prehead + str[strRandom] + "</mark>";
            }
            else
            {
                //gameObject.SetActive(false);
                textMeshPro.text = "";
            }
            yield return new WaitForSeconds(5);
        }
    }
}
