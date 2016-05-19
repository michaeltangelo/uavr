using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	enum TutorialNode{Intro01, Intro02, Intro03, PilotCtrlLeft, PilotCtrlRight, SwitchMode, OperatorCtrlRight, MissileFire }

	[SerializeField] private GameObject dialogBoxRoot;
	private RectTransform dialogBoxTransform;
	[SerializeField] private Text dialogBoxText;
	[SerializeField] private GameObject continueMsg;

	[SerializeField] private RectTransform InitDialogAnchor;
	[SerializeField] private GameObject pilotLeftCtrlMrkr;
	[SerializeField] private RectTransform PilotCtrlLeftDialogAnchor;
	[SerializeField] private GameObject pilotRightCtrlMrkr;
	[SerializeField] private RectTransform PilotCtrlRightDialogAnchor;

	[SerializeField] private Text opModeText;


//	Welcome to UAVR, a VR drone flight simulator! Today, you will be flying the MQ-9 Reaper. Press 'x' to get started.
//
//	2. The MQ-9 Reaper is an unmanned aerial vehicle (UAV) and is operated by two people at a time: the pilot, and the sensor operator. Press 'x' to continue.
//
//	3. If you look to your right, you'll notice an empty seat beside you. This is where the sensor operator sits, and it also means that you're the pilot! Press 'x' to continue.
//
//	4. Let's begin! Take a look at the control stick on the left, known as the "throttle". Try pushing the left joystick of your PS4 controller forwards and backwards and see what happens. Press 'x' to move on.
//	// user will push the left joystick and the throttle will move forward and backwards 
//
//	5. Great! The joystick on your right controls the orientation of your drone. See how your drone reacts by moving the right joystick of your PS4 controller. Press 'x' to continue.
//	// user will play with the right ps4 joystick and see the drone's flaps move accordingly
//
//	6. As the pilot, your controls move the drone directly. Let's see what happens as the sensor operator. Press 'o' to switch into sensor operator mode.
//	// user presses O
//
//	7. Now, try moving your right joystick. Notice how the camera swivels, but the drone itself does not move. Don't be upset though, as the sensor operator you can fire missiles! Press 'x' to continue.
//
//	8. Aim the camera to a target and hit the right trigger to fire a missile! Press 'x' to continue.
//
//	9. You can switch between pilot and sensor operator modes mid-flight by pressing 'o'. Try switching to pilot mode now. Press 'o' to switch to pilot mode.
//
//	10. You're all set! Push your left stick forward to activate the throttle, and control your drone with the right. Good luck!
//
	Dictionary<TutorialNode, string> dialogs = new Dictionary<TutorialNode, string>(){
		{TutorialNode.Intro01, "Welcome to UAVR, a VR drone flight simulator! Today, you will be flying the MQ-9 Reaper."},
		{TutorialNode.Intro02, "The MQ-9 Reaper is an unmanned aerial vehicle (UAV) and is operated by two people at a time: the pilot, and the sensor operator." },
		{TutorialNode.Intro03, "In this simulation, you can operate as a pilot or a sensor operator at the flick of a switch. Let us first start as a Pilot."},
		{TutorialNode.PilotCtrlLeft, "The left joystick of your PS4 controller simulates the throttle. Try moving it forwards and backwards."},
		{TutorialNode.PilotCtrlRight, "The joystick on your right controls the orientation of your drone. Control it with the right joystick of your PS4 controller." },
		{TutorialNode.SwitchMode, "As the pilot, your controls move the drone directly. Let's see what happens as the sensor operator. Press 'o' to switch into sensor operator mode."},
		{TutorialNode.OperatorCtrlRight, "Now, try moving your right joystick. Notice how the camera swivels, but the drone itself does not move. Don't be upset though, as the sensor operator you can fire missiles!"},
		{TutorialNode.MissileFire, "Aim the camera to a target and hit the right trigger to fire a missile! Press 'x' to continue."}
	};

	TutorialNode curNode = TutorialNode.Intro01;

	int operatingMode = 0;

	void Awake(){
		if(pilotLeftCtrlMrkr) pilotLeftCtrlMrkr.SetActive(false);
		if(pilotRightCtrlMrkr) pilotRightCtrlMrkr.SetActive(false);
		dialogBoxTransform = dialogBoxRoot.GetComponent<RectTransform>();
	}

	public void Transition(){
		Debug.Log("Transitioning to next step");
		switch(curNode){
		case TutorialNode.Intro01:
		case TutorialNode.Intro02:
			curNode = (TutorialNode)((int)curNode + 1);
			dialogBoxText.text = dialogs[curNode];
			break;
		case TutorialNode.Intro03:
			curNode = (TutorialNode)((int)curNode + 1);
			dialogBoxText.text = dialogs[curNode];
			pilotLeftCtrlMrkr.SetActive(true);
			dialogBoxTransform.position = PilotCtrlLeftDialogAnchor.position;
			dialogBoxTransform.rotation = PilotCtrlLeftDialogAnchor.rotation;
			continueMsg.SetActive(false);
			break;
		case TutorialNode.PilotCtrlLeft:
			curNode = (TutorialNode)((int)curNode + 1);
			dialogBoxText.text = dialogs[curNode];
			pilotLeftCtrlMrkr.SetActive(false);
			pilotRightCtrlMrkr.SetActive(true);
			dialogBoxTransform.position = PilotCtrlRightDialogAnchor.position;
			dialogBoxTransform.rotation = PilotCtrlRightDialogAnchor.rotation;
			break;
		case TutorialNode.PilotCtrlRight:
			dialogBoxRoot.SetActive(false);
			pilotRightCtrlMrkr.SetActive(false);
			break;
		}
	}

	public void PlayPilotCtrlLeft(){
		//highlight the left control stick, 
		//tell user to push the left control stick
		//only allow user to look front
	}
		
	public void SwitchMode(){
		operatingMode = 1 - operatingMode;
		if(operatingMode == 0) opModeText.text = "Pilot Mode";
		else opModeText.text = "Sensor Ops Mode";
	}
}
