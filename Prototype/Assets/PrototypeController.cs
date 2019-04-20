using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeController : MonoBehaviour
{

	public Camera camera;

	public GameObject DetectedPlanePrefab;

	private List<DetectedPlane> planes = new List<DetectedPlane>();

	private bool isQuitting = false;

	public GameObject prefab;

	private const float k_ModelRotation = 270.0f;

	public void Update()
	{
		EventDetection();

		Session.GetTrackables<DetectedPlane>(planes);
		bool showSearchingUI = true;
		for (int i = 0; i < planes.Count; i++)
		{
			if (planes[i].TrackingState == TrackingState.Tracking)
			{
				showSearchingUI = false;
				break;
			}
		}

		// If the player has not touched the screen, we are done with this update.
		Touch touch;
		if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
		{
			return;
		}

		// Raycast against the location the player touched to search for planes.
		TrackableHit hit;
		TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
			TrackableHitFlags.FeaturePointWithSurfaceNormal;

		if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
		{
			// Use hit pose and camera pose to check if hittest is from the
			// back of the plane, if it is, no need to create the anchor.
			if ((hit.Trackable is DetectedPlane) &&
				Vector3.Dot(camera.transform.position - hit.Pose.position,
					hit.Pose.rotation * Vector3.up) < 0)
			{
				Debug.Log("Hit at back of the current DetectedPlane");
			}
			else
			{
				if ((hit.Trackable is DetectedPlane)){
					DetectedPlane plane=(DetectedPlane)hit.Trackable;
					if (plane.PlaneType == DetectedPlaneType.Vertical)
					{
						var andyObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
						andyObject.transform.position = new Vector3(hit.Pose.position.x + 0.06f, hit.Pose.position.y + 0.06f, hit.Pose.position.z + 0.06f);
						// Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
						//andyObject.transform.localEulerAngles = new Vector3(0, plane.CenterPose.rotation.y+180, 0);
						var placedObjectForward = Vector3.up;
						var placedObjectUp = plane.CenterPose.rotation * Vector3.up;
						Vector3 normal = plane.CenterPose.rotation * Vector3.up;
						//andyObject.transform.Rotate(k_ModelRotation, 0 , k_ModelRotation);
						andyObject.transform.rotation = Quaternion.LookRotation(placedObjectForward, placedObjectUp);
						andyObject.transform.Rotate(-90, 0, 0);
						//andyObject.transform.localEulerAngles = new Vector3(andyObject.transform.rotation.x, 0, andyObject.transform.rotation.z);

						// Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
						// world evolves.
						var anchor = hit.Trackable.CreateAnchor(hit.Pose);

						// Make Andy model a child of the anchor.
						andyObject.transform.parent = anchor.transform;
					}
					else
					{
						Handheld.Vibrate();
					}
					
				}
				
			}
		}
	}

	private void EventDetection()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
		if (Session.Status != SessionStatus.Tracking)
		{
			const int trackingSleep = 15;
			Screen.sleepTimeout = trackingSleep;
		}
		else
		{
			_ShowAndroidToastMessage("tracking");
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
		if (isQuitting)
		{
			return;
		}

		if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
		{
			_ShowAndroidToastMessage("Camera permission is needed to run this application.");
			isQuitting = true;
			Invoke("_DoQuit", 0.5f);
		}
		else if (Session.Status.IsError())
		{
			_ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
			isQuitting = true;
			Invoke("_DoQuit", 0.5f);
		}
	}

	private void _ShowAndroidToastMessage(string message)
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		if (unityActivity != null)
		{
			AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
			unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
				message, 0);
				toastObject.Call("show");
			}));
		}
	}
}
