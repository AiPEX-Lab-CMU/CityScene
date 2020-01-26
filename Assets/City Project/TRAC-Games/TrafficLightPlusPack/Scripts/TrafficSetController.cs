// Copyright (c)2017 Reece Robinson. All rights reserved.

using UnityEngine;
using System.Collections;

public class TrafficSetController : MonoBehaviour {

	// This script works with the TrafficLightController and provides the ability to manually set the light phase 
	// of the associated traffic light.


	public PhaseState state; // Set this dropdown from the properties panel for the desired traffic light state.

	private PhaseState lastState = PhaseState.Off;

	void  Update (){

		if( state != lastState) {

			switch (state) {
			case PhaseState.Walk:
				lastState = PhaseState.Walk;
				break;
			case PhaseState.DontWalk:
				lastState = PhaseState.DontWalk;
				break;
			case PhaseState.Stop:
				lastState = PhaseState.Stop;
				break;
			case PhaseState.Warn:
				lastState = PhaseState.Warn;
				break;
			case PhaseState.Go: 
				lastState = PhaseState.Go;
				break;
			case PhaseState.Off: 
				lastState = PhaseState.Off;
				break;
			case PhaseState.Flash: 
				lastState = PhaseState.Flash;
				break;
			default:
				Debug.Log ("Unknown State");
				break;
			}
			NotifyState(state);
		}
	}

	void  NotifyState ( PhaseState newState  ){
		BroadcastMessage("ApplyState",newState,SendMessageOptions.DontRequireReceiver); 
	}
}