using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandGroupController : MonoBehaviour
{
    public List<WaypointMovement> groupMembers;

    //Variable to store who is NOT going to walk next (the person who just completed his walk will not walk next)
    WaypointMovement nextWalkerException;

    //Stores the waypoint movement of the guy who will walk next
    public WaypointMovement nextWalker;

    [SerializeField] bool shouldWalk;

    // Start is called before the first frame update
    void Start()
    {
        groupMembers = new List<WaypointMovement>();

        foreach (WaypointMovement wm in this.gameObject.GetComponentsInChildren<WaypointMovement>())
        {
            if (wm.isStanding)
            {
                groupMembers.Add(wm);

                //Binding the group member walk complete function to all group members
                wm.onWalkComplete += GroupMemberWalkComplete;
            }
        }

        //Randomly decide a walker at the start
        DecideNextWalkerAndDelay();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GroupMemberWalkComplete(WaypointMovement wm)
    {

        nextWalkerException = wm;

        DecideNextWalkerAndDelay();

    }

    void DecideNextWalkerAndDelay()
    {
        if (!shouldWalk)
            return;

        //first check the count of people already moving

        int walkCounter = 0;

        foreach(WaypointMovement wm in groupMembers)
        {

            if (wm.shouldStartMove)
                walkCounter++;
        }

        Debug.Log("reached");

        if (walkCounter > 0)
            return;

        //decide next walker first
        //using temp walker var as safety against wrong assignment of the next walker
        WaypointMovement tempNextWalker;

        do
        {
            //picking a random wm from the group members list
            tempNextWalker = groupMembers[Random.Range(0, groupMembers.Count)];

            //repeat if the next walker exception is the same as the temp walker
        } while (tempNextWalker == nextWalkerException);

        nextWalker = tempNextWalker;


        //now decide the delay
        float delay = Random.Range(5.0f, 15.0f);

        Debug.Log("Decided delay for" + nextWalker.gameObject.name + " is " + delay.ToString());

        StartCoroutine(StartWalkAfterDelay(nextWalker, delay));

        //Dereference the next walker once it's function is over
        nextWalker = null;

    }

    IEnumerator StartWalkAfterDelay(WaypointMovement wm, float delay)
    {

        yield return new WaitForSeconds(delay);

        wm.StartMove();

    }

}
