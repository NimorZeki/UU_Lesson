//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
///     This script can be used to restrict camera rendering to a specific part of the screen by specifying the two
///     corners.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/UI/Viewport Camera")]
public class UIViewport : MonoBehaviour {
	public Camera sourceCamera;
	public Transform topLeft;
	public Transform bottomRight;
	public float fullSize = 1f;

	private Camera mCam;

	private void Start() {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		mCam = camera;
#else
		mCam = GetComponent<Camera>();
#endif
		if(sourceCamera == null) sourceCamera = Camera.main;
	}

	private void LateUpdate() {
		if(topLeft != null && bottomRight != null) {
			if(topLeft.gameObject.activeInHierarchy) {
				var tl = sourceCamera.WorldToScreenPoint(topLeft.position);
				var br = sourceCamera.WorldToScreenPoint(bottomRight.position);

				var rect = new Rect(tl.x / Screen.width, br.y / Screen.height,
				                    (br.x - tl.x) / Screen.width, (tl.y - br.y) / Screen.height);

				var size = fullSize * rect.height;

				if(rect != mCam.rect) mCam.rect = rect;
				if(mCam.orthographicSize != size) mCam.orthographicSize = size;
				mCam.enabled = true;
			}
			else {
				mCam.enabled = false;
			}
		}
	}
}