using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class manMovement : MonoBehaviour
{
    float speed = 0.3f;
    int a = 0;
    Vector3 moveDir = Vector3.zero;
    Animator anim;
    CharacterController controller;
    static System.Random r;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        anim.SetInteger("condition", 1);
        Debug.Log("yo");
        r = new System.Random();
    }

    IEnumerator Example()
    {
        int x = r.Next(10);
        yield return new WaitForSeconds(5);
        a = 1;
        anim.SetInteger("condition", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(a==0)
        {
            StartCoroutine(Example());
        }
        
        moveDir = new Vector3(0, 0, a);
        moveDir *= speed;
        moveDir = transform.TransformDirection(moveDir);
        controller.Move(moveDir * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "roads_motorway.001")
            Debug.Log("hi");
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
        }
    }
}
