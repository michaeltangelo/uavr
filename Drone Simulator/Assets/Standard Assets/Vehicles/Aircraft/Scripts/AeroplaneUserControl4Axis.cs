using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof (AeroplaneController))]
	public class AeroplaneUserControl4Axis : MonoBehaviour
	{
		// these max angles are only used on mobile, due to the way pitch and roll input are handled
		public float maxRollAngle = 80;
		public float maxPitchAngle = 80;
		public bool controllerEnabled = false;
		private bool pilotMode = true;

		public string getThrottleClamped()
		{
			return m_Throttle.ToString();
		}

		// reference to the aeroplane that we're controlling
		private AeroplaneController m_Aeroplane;
		private float m_Throttle;
		private bool m_AirBrakes;
		private float m_Yaw;
		private bool m_ToggleOperatorMode;
        private bool m_ToggleLock;
        private bool squarePressed;
        private bool trianglePressed;

        // control room references

        // reference to the underbelly camera and speed settings
        public GameObject CameraBay;
        public float rotateSpeedScale;
        private Camera cam;
        public Vector3 lockPoint;
        private bool isLocked;
        LineRenderer line;
        public Material lineMat;
        public Vector3 cameraPosition;
        public GameObject Sphere;
        public Material[] materials;

        // missile properties
        public Rigidbody Missile;
        public Transform[] Launchers;
        private Transform Launcher;
        public GameObject Target;
        private bool canFire;

        // material switches for screen
        public Material[] screenMats;
        public GameObject QuadScreen;

        float roll;
        float pitch;

        public void setPilotMode(bool boolean)
		{
			pilotMode = boolean;
		}

		public bool getPilotMode()
		{
			return pilotMode;
		}

		public void togglePilotMode()
		{
			pilotMode = !pilotMode;
            if (pilotMode)
            {
                QuadScreen.GetComponent<Renderer>().material = screenMats[0];
            }
            else
            {
                QuadScreen.GetComponent<Renderer>().material = screenMats[1];
            }
        }
        
        public void toggleLockMode()
        {
            Debug.Log("ToggleLockMode called");
            isLocked = !isLocked;
        }

		private void Awake()
		{
            // configure initial launcher (right)
            Launcher = Launchers[0];

			// Set up the reference to the aeroplane controller.
			m_Aeroplane = GetComponent<AeroplaneController>();
            CameraBay = GameObject.Find("UnderbellyCam");
            cam = CameraBay.GetComponent<Camera>();
            isLocked = false;
            line = GetComponent<LineRenderer>();
            line.SetVertexCount(2);
            line.GetComponent<Renderer>().material= lineMat;
            line.SetWidth(0.01f, 0.05f);
            canFire = false;

		}

        private void LockLaser()
        {
            canFire = true;

            GameObject go = GameObject.FindGameObjectWithTag("target");
            Destroy(go);
            RaycastHit hit;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out hit))
            {
                isLocked = true;
                lockPoint = hit.point;
                GameObject TargetCopy = Instantiate(Target, hit.point, Quaternion.identity) as GameObject;
                TargetCopy.tag = "target";
                if (hit.collider.tag == "sphere")
                {
                    Sphere.GetComponent<Renderer>().material = materials[1];
                }
            }
            
        }

        private void Update()
        {
            // Read input for the pitch, yaw, roll and throttle of the aeroplane.
            if (!controllerEnabled)
            {
                roll = CrossPlatformInputManager.GetAxis("Horizontal");
                pitch = CrossPlatformInputManager.GetAxis("Vertical");
                m_AirBrakes = CrossPlatformInputManager.GetButton("Fire1");
                m_Yaw = CrossPlatformInputManager.GetAxis("Horizontal");
                m_Throttle = CrossPlatformInputManager.GetAxis("Throttle_Vertical");
            }
            else
            {
                // ps4 controller map https://www.reddit.com/r/Unity3D/comments/1syswe/ps4_controller_map_for_unit/
                roll = CrossPlatformInputManager.GetAxis("PS3ControllerRightX");
                pitch = CrossPlatformInputManager.GetAxis("PS3ControllerRightY");
                m_AirBrakes = CrossPlatformInputManager.GetButtonDown("Fire1"); // Joystick 4 aka L1 on PS4 Controller
                m_Yaw = CrossPlatformInputManager.GetAxis("PS3ControllerRightX");
                m_Throttle = CrossPlatformInputManager.GetAxis("PS3ControllerThrottleY");
                m_ToggleOperatorMode = CrossPlatformInputManager.GetButtonDown("PS3ControllerOButton");
                m_ToggleLock = CrossPlatformInputManager.GetButtonDown("PS3ControllerLock");
                squarePressed = CrossPlatformInputManager.GetButton("PS3ControllerSquare");
                trianglePressed = CrossPlatformInputManager.GetButtonDown("PS3ControllerTriangle");

            }
            if (squarePressed)
            {
                UnityEngine.VR.InputTracking.Recenter();
            }
            if (trianglePressed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (m_ToggleOperatorMode) togglePilotMode();
            // firing
            if (m_AirBrakes)
            {
                if (canFire)
                {
                    Rigidbody missileClone = (Rigidbody)Instantiate(Missile, Launcher.transform.position, Launcher.transform.rotation);

                    // switch launchers
                    if (Launcher == Launchers[0]) Launcher = Launchers[1];
                    else if (Launcher == Launchers[1]) Launcher = Launchers[0];
                }
            }
            if (m_ToggleLock)
            {
                toggleLockMode();
                if (isLocked) LockLaser();
                else
                {
                    Sphere.GetComponent<Renderer>().material = materials[0];
                }
            }
        }

		private void FixedUpdate()
		{
			#if MOBILE_INPUT
			AdjustInputForMobileControls(ref roll, ref pitch, ref m_Throttle);
			#endif

            // Pass the input to the aeroplane
            // if pilot mode disabled, send no signals to plane except throttle and turn camera instead
            if (pilotMode)
            {
                m_Aeroplane.Move(roll, pitch, m_Yaw, m_Throttle, m_AirBrakes);
                line.enabled = false;
            }
            else
            // in sensor operator mode
            {
                /*
                cameraPosition = cam.transform.position;
                RaycastHit hit;
                Vector3 rayStartOffset = cam.transform.position;
                Ray ray = new Ray(rayStartOffset, cam.transform.forward);
                if (Physics.Raycast(ray, out hit))
                {
                    line.enabled = true;
                    line.SetPosition(0, cam.transform.position);
                    line.SetPosition(1, hit.point);
                    //Debug.Log("Hit!");
                }
                else
                {
                    line.enabled = false;
                }
                */
                // move airplane
                m_Aeroplane.Move(0, 0, 0, m_Throttle, m_AirBrakes);

                // lock camera to target
                if (isLocked)
                {
                    CameraBay.transform.LookAt(lockPoint);
                }
                else
                {
                    // rotates right
                    CameraBay.transform.Rotate(Vector3.up * rotateSpeedScale * Time.deltaTime * roll, Space.Self);
                    // rotates down
                    CameraBay.transform.Rotate(Vector3.left * rotateSpeedScale * Time.deltaTime * pitch);
                }
			}
		}


		private void AdjustInputForMobileControls(ref float roll, ref float pitch, ref float throttle)
		{
			// because mobile tilt is used for roll and pitch, we help out by
			// assuming that a centered level device means the user
			// wants to fly straight and level!

			// this means on mobile, the input represents the *desired* roll angle of the aeroplane,
			// and the roll input is calculated to achieve that.
			// whereas on non-mobile, the input directly controls the roll of the aeroplane.

			float intendedRollAngle = roll*maxRollAngle*Mathf.Deg2Rad;
			float intendedPitchAngle = pitch*maxPitchAngle*Mathf.Deg2Rad;
			roll = Mathf.Clamp((intendedRollAngle - m_Aeroplane.RollAngle), -1, 1);
			pitch = Mathf.Clamp((intendedPitchAngle - m_Aeroplane.PitchAngle), -1, 1);
		}
	}
}
