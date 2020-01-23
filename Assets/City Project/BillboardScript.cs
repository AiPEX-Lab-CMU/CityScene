using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    [SerializeField] GameObject background;
    [SerializeField] List<GameObject> billboards;

    int current = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.B))
        {

            ToggleBackground();

            print("b pressed");

        }

        if (Input.GetKeyDown(KeyCode.V))
        {

            foreach(GameObject b in billboards)
            {

                b.SetActive(false);

            }

            billboards[current].SetActive(true);

            current++;

            print("v pressed");
        }


    }

    void ToggleBackground()
    {

        background.SetActive(!background.activeSelf);

        if (!background.activeSelf)
        {
            foreach (GameObject b in billboards)
            {

                b.SetActive(false);

            }

        }

    }



}
