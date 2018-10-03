﻿using UnityEngine;

/// <summary>Placing this script on the game object will make that game object pan with mouse movement.</summary>
[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
public class PanWithMouse : MonoBehaviour {
	public Vector2 degrees = new Vector2(5f, 3f);
	public float range = 1f;

	private Transform mTrans;
	private Quaternion mStart;
	private Vector2 mRot = Vector2.zero;

	private void Start() {
		mTrans = transform;
		mStart = mTrans.localRotation;
	}

	private void Update() {
		var delta = RealTime.deltaTime;
		Vector3 pos = UICamera.lastEventPosition;

		var halfWidth = Screen.width * 0.5f;
		var halfHeight = Screen.height * 0.5f;
		if(range < 0.1f) range = 0.1f;
		var x = Mathf.Clamp((pos.x - halfWidth) / halfWidth / range, -1f, 1f);
		var y = Mathf.Clamp((pos.y - halfHeight) / halfHeight / range, -1f, 1f);
		mRot = Vector2.Lerp(mRot, new Vector2(x, y), delta * 5f);

		mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * degrees.y, mRot.x * degrees.x, 0f);
	}
}