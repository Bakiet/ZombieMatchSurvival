﻿using UnityEngine;
using System.Collections;

// It used in animation as an event to start another animation
public class ChipAnimationEvent : MonoBehaviour {
		
		void  PlayAnimation (string name){
			GetComponent<Animation>().CrossFade(name, 0.2f);
		}
	}