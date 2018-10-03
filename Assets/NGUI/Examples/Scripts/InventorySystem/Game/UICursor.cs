//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>Selectable sprite that follows the mouse.</summary>
[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/Examples/UI Cursor")]
public class UICursor : MonoBehaviour {
	public static UICursor instance;

	// Camera used to draw this cursor
	public Camera uiCamera;

	private Transform mTrans;
	private UISprite mSprite;

	private UIAtlas mAtlas;
	private string mSpriteName;

	/// <summary>Keep an instance reference so this class can be easily found.</summary>
	private void Awake() {
		instance = this;
	}

	private void OnDestroy() {
		instance = null;
	}

	/// <summary>Cache the expected components and starting values.</summary>
	private void Start() {
		mTrans = transform;
		mSprite = GetComponentInChildren<UISprite>();

		if(uiCamera == null)
			uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);

		if(mSprite != null) {
			mAtlas = mSprite.atlas;
			mSpriteName = mSprite.spriteName;
			if(mSprite.depth < 100) mSprite.depth = 100;
		}
	}

	/// <summary>Reposition the widget.</summary>
	private void Update() {
		var pos = Input.mousePosition;

		if(uiCamera != null) {
			// Since the screen can be of different than expected size, we want to convert
			// mouse coordinates to view space, then convert that to world position.
			pos.x = Mathf.Clamp01(pos.x / Screen.width);
			pos.y = Mathf.Clamp01(pos.y / Screen.height);
			mTrans.position = uiCamera.ViewportToWorldPoint(pos);

			// For pixel-perfect results
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (uiCamera.isOrthoGraphic)
#else
			if(uiCamera.orthographic)
#endif 
			{
				var lp = mTrans.localPosition;
				lp.x = Mathf.Round(lp.x);
				lp.y = Mathf.Round(lp.y);
				mTrans.localPosition = lp;
			}
		}
		else {
			// Simple calculation that assumes that the camera is of fixed size
			pos.x -= Screen.width * 0.5f;
			pos.y -= Screen.height * 0.5f;
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);
			mTrans.localPosition = pos;
		}
	}

	/// <summary>Clear the cursor back to its original value.</summary>
	public static void Clear() {
		if(instance != null && instance.mSprite != null)
			Set(instance.mAtlas, instance.mSpriteName);
	}

	/// <summary>Override the cursor with the specified sprite.</summary>
	public static void Set(UIAtlas atlas, string sprite) {
		if(instance != null && instance.mSprite) {
			instance.mSprite.atlas = atlas;
			instance.mSprite.spriteName = sprite;
			instance.mSprite.MakePixelPerfect();
			instance.Update();
		}
	}
}