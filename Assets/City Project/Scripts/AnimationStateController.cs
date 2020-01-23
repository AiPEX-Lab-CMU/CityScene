using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public enum HumanoidStates { Idle, Walk, Run, Dead};

    WaypointMovement wm;

    [SerializeField] Animator animator;

    public HumanoidStates animationState;

    // Start is called before the first frame update
    void Start()
    {

        wm = this.gameObject.GetComponent<WaypointMovement>();

        if (wm.thisType != WaypointMovement.TypeOfObject.Person)
            this.enabled = false;

        animator = this.transform.GetChild(0).gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (animationState == HumanoidStates.Idle)
            animator.SetInteger("AnimationStateInt", 0);
        
        else if (animationState == HumanoidStates.Walk)
            animator.SetInteger("AnimationStateInt", 1);
        
        else if (animationState == HumanoidStates.Run)
            animator.SetInteger("AnimationStateInt", 2);
        
        else if (animationState == HumanoidStates.Dead)
            animator.SetInteger("AnimationStateInt", 3);
        
    }
}
