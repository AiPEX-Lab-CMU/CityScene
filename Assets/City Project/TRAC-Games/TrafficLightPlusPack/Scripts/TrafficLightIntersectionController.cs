// Copyright (c)2017 Reece Robinson. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class TrafficLightIntersectionController : MonoBehaviour {

	// This controller provides the automated phase logic for two light controlled intersecting roadways.

	public GameObject directionOneSetController = null; // Typically an Empty that is a parent of zero or more traffic light assets.
	public GameObject directionTwoSetController = null; // Typically an Empty that is a parent of zero or more traffic light assets.
	public bool outOfOrder= false; // Used to bypass the normal control phasing and flash the lights instead.
	public float primaryPhaseDuration= 20.0f; // Default value for go and stop phase in seconds.
	public float transitionPhaseDuration= 3.0f; // Default value for transition phases in seconds.

	// Array of PhaseStates that execute a complete traffic light control sequence. Each traffic direction has an offset phase sequence.
	private List<PhaseState> fourWaySequenceOne = new List<PhaseState>{PhaseState.Walk, PhaseState.DontWalk, PhaseState.Stop, PhaseState.Go, PhaseState.Warn, PhaseState.Stop};
	private List<PhaseState> fourWaySequenceTwo = new List<PhaseState>{PhaseState.Go, PhaseState.Warn, PhaseState.Stop, PhaseState.Walk, PhaseState.DontWalk, PhaseState.Stop};
	private List<float> stateIntervals; // Timing in seconds for each of the phases in the sequence.
	private float outOfOrderInterval = 1.0f; // Flash period in seconds.
	private float timer = 0;
	private int index= 0;
	private bool lastOutOfOrder= false;

	void  Start (){
		// Set the interection control timing values based on the values given in the properties panel.
		this.stateIntervals = new List<float>{primaryPhaseDuration,transitionPhaseDuration,transitionPhaseDuration,primaryPhaseDuration,transitionPhaseDuration,transitionPhaseDuration}; // timing in seconds for each of the phases in the sequence.
		this.lastOutOfOrder = this.outOfOrder;
	}

	void  Update (){
		timer -= Time.deltaTime;
		if(lastOutOfOrder != outOfOrder) {
			timer = 0; // Calcel the current phase.
			lastOutOfOrder = outOfOrder;
		}
		if(timer <= 0) {
			index ++;
			if(index >= stateIntervals.Count) {
				index = 0;
			}
			if(outOfOrder) {
				timer = outOfOrderInterval;
				NotifyState(directionOneSetController,PhaseState.Flash);
				NotifyState(directionTwoSetController,PhaseState.Flash);
			} else {
				timer = stateIntervals[index];
				NotifyState(directionOneSetController, fourWaySequenceOne[index]);
				NotifyState(directionTwoSetController, fourWaySequenceTwo[index]);
			}
		}
	}

	void  NotifyState ( GameObject controller ,   PhaseState newState  ){
		controller.BroadcastMessage("ApplyState",newState,SendMessageOptions.DontRequireReceiver); 
	}
}