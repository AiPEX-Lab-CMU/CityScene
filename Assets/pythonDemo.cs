using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetMQ.Sockets;
using AsyncIO;
using NetMQ;

public class pythonDemo : MonoBehaviour
{
    float speed = 0.3f;
    int a = 0;
    Vector3 moveDir = Vector3.zero;
    Animator anim;
    CharacterController controller;
    static System.Random r;
    int count = 0;
    int lastWait = 0;
    bool waiting = true;
    //Text crossCount = GameObject.FindGameObjectWithTag("Road_Cross").GetComponent<Text>();
    //Text avg = GameObject.Find("Canvas").;
    Text crossCount;
    Text avg;
    RequestSocket client;


    // Start is called before the first frame update
    void Start()
    {
        client = new RequestSocket();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        anim.SetInteger("condition", 0);
        Debug.Log("yo");
        r = new System.Random();
        GameObject can = GameObject.Find("Canvas/Road_Cross");
        crossCount = can.GetComponent<Text>();
        avg = GameObject.Find("Canvas/Ave_Wait").GetComponent<Text>();
        client.Connect("tcp://localhost:5555");
    }

    IEnumerator Example()
    {
        int x = r.Next(15);
        waiting = false;
        yield return new WaitForSeconds(x);
        a = 1;
        anim.SetInteger("condition", 1);
        lastWait = x;
        
        Debug.Log(count + "," + x);
        client.SendFrame(count + "," + x);
        string message = null;
        bool gotMessage = false;
        Debug.Log("Receiving");
        while (true)
        {
            gotMessage = client.TryReceiveFrameString(out message);
            if (gotMessage) break;
        }
        if (gotMessage)
        {
            crossCount.text = ("Road Cross Count: " + message);
            avg.text = ("Average Wait Time: " + x);
        }
        //Debug.Log("Received Message: " + message);
    }

    // Update is called once per frame
    void Update()
    {
        if (waiting == true)
        {
            StartCoroutine(Example());
        }

        moveDir = new Vector3(0, 0, a);
        moveDir *= speed;
        moveDir = transform.TransformDirection(moveDir);
        controller.Move(moveDir * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "roads_motorway.001")
        {
            hit.collider.isTrigger = true;
            //a *= -1;
            transform.Rotate(new Vector3(0, (transform.rotation.y + 180), 0));
            anim.SetInteger("condition", 0);
            a = 0;
            hit.collider.isTrigger = false;
            count++;
            waiting = true;
        }
    }
}
