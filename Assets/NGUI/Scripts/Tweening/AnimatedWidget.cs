//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>Makes it possible to animate the widget's width and height using Unity's animations.</summary>
[ExecuteInEditMode]
public class AnimatedWidget : MonoBehaviour {
	public float width = 1f;
	public float height = 1f;

	private UIWidget mWidget;

	private void OnEnable() {
		mWidget = GetComponent<UIWidget>();
		LateUpdate();
	}

	private void LateUpdate() {
		if(mWidget != null) {
			mWidget.width = Mathf.RoundToInt(width);
			mWidget.height = Mathf.RoundToInt(height);
		}
	}
}