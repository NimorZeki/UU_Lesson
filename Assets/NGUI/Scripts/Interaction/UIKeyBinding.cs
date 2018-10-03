//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     This class makes it possible to activate or select something by pressing a key (such as space bar for
///     example).
/// </summary>
[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour {
	private static readonly List<UIKeyBinding> mList = new List<UIKeyBinding>();

#if W2
	[Beebyte.Obfuscator.SkipRename]
#endif
	public enum Action {
		PressAndClick,
		Select,
		All
	}

#if W2
	[Beebyte.Obfuscator.SkipRename]
#endif
	public enum Modifier {
		Any,
		Shift,
		Control,
		Alt,
		None
	}

	/// <summary>Key that will trigger the binding.</summary>
	public KeyCode keyCode = KeyCode.None;

	/// <summary>Modifier key that must be active in order for the binding to trigger.</summary>
	public Modifier modifier = Modifier.Any;

	/// <summary>Action to take with the specified key.</summary>
	public Action action = Action.PressAndClick;

	[System.NonSerialized]
	private bool mIgnoreUp;
	[System.NonSerialized]
	private bool mIsInput;
	[System.NonSerialized]
	private bool mPress;

	/// <summary>Key binding's descriptive caption.</summary>

	public string captionText {
		get {
			var s = NGUITools.KeyToCaption(keyCode);
			if(modifier == Modifier.Alt) return "Alt+" + s;
			if(modifier == Modifier.Control) return "Control+" + s;
			if(modifier == Modifier.Shift) return "Shift+" + s;
			return s;
		}
	}

	/// <summary>Check to see if the specified key happens to be bound to some element.</summary>
	public static bool IsBound(KeyCode key) {
		for(int i = 0, imax = mList.Count; i < imax; ++i) {
			var kb = mList[i];
			if(kb != null && kb.keyCode == key) return true;
		}
		return false;
	}

	protected virtual void OnEnable() {
		mList.Add(this);
	}

	protected virtual void OnDisable() {
		mList.Remove(this);
	}

	/// <summary>If we're bound to an input field, subscribe to its Submit notification.</summary>
	protected virtual void Start() {
		var input = GetComponent<UIInput>();
		mIsInput = input != null;
		if(input != null) EventDelegate.Add(input.onSubmit, OnSubmit);
	}

	/// <summary>Ignore the KeyUp message if the input field "ate" it.</summary>
	protected virtual void OnSubmit() {
		if(UICamera.currentKey == keyCode && IsModifierActive()) mIgnoreUp = true;
	}

	/// <summary>Convenience function that checks whether the required modifier key is active.</summary>
	protected virtual bool IsModifierActive() {
		return IsModifierActive(modifier);
	}

	/// <summary>Convenience function that checks whether the required modifier key is active.</summary>
	public static bool IsModifierActive(Modifier modifier) {
		if(modifier == Modifier.Any) return true;

		if(modifier == Modifier.Alt) {
			if(UICamera.GetKey(KeyCode.LeftAlt) ||
			   UICamera.GetKey(KeyCode.RightAlt)) return true;
		}
		else if(modifier == Modifier.Control) {
			if(UICamera.GetKey(KeyCode.LeftControl) ||
			   UICamera.GetKey(KeyCode.RightControl)) return true;
		}
		else if(modifier == Modifier.Shift) {
			if(UICamera.GetKey(KeyCode.LeftShift) ||
			   UICamera.GetKey(KeyCode.RightShift)) return true;
		}
		else if(modifier == Modifier.None) {
			return
				!UICamera.GetKey(KeyCode.LeftAlt) &&
				!UICamera.GetKey(KeyCode.RightAlt) &&
				!UICamera.GetKey(KeyCode.LeftControl) &&
				!UICamera.GetKey(KeyCode.RightControl) &&
				!UICamera.GetKey(KeyCode.LeftShift) &&
				!UICamera.GetKey(KeyCode.RightShift);
		}
		return false;
	}

	/// <summary>Process the key binding.</summary>
	protected virtual void Update() {
		if(UICamera.inputHasFocus) return;
		if(keyCode == KeyCode.None || !IsModifierActive()) return;
#if WINDWARD && UNITY_ANDROID
// NVIDIA Shield controller has an odd bug where it can open the on-screen keyboard via a KeyCode.Return binding,
// and then it can never be closed. I am disabling it here until I can track down the cause.
		if (keyCode == KeyCode.Return && PlayerPrefs.GetInt("Start Chat") == 0) return;
#endif

#if UNITY_FLASH
		bool keyDown = Input.GetKeyDown(keyCode);
		bool keyUp = Input.GetKeyUp(keyCode);
#else
		var keyDown = UICamera.GetKeyDown(keyCode);
		var keyUp = UICamera.GetKeyUp(keyCode);
#endif

		if(keyDown) mPress = true;

		if(action == Action.PressAndClick || action == Action.All) {
			if(keyDown) {
				UICamera.currentTouchID = -1;
				UICamera.currentKey = keyCode;
				OnBindingPress(true);
			}

			if(mPress && keyUp) {
				UICamera.currentTouchID = -1;
				UICamera.currentKey = keyCode;
				OnBindingPress(false);
				OnBindingClick();
			}
		}

		if(action == Action.Select || action == Action.All)
			if(keyUp) {
				if(mIsInput) {
					if(!mIgnoreUp && !UICamera.inputHasFocus)
						if(mPress)
							UICamera.selectedObject = gameObject;
					mIgnoreUp = false;
				}
				else if(mPress) {
					UICamera.hoveredObject = gameObject;
				}
			}

		if(keyUp) mPress = false;
	}

	protected virtual void OnBindingPress(bool pressed) {
		UICamera.Notify(gameObject, "OnPress", pressed);
	}

	protected virtual void OnBindingClick() {
		UICamera.Notify(gameObject, "OnClick", null);
	}

	/// <summary>Convert the key binding to its text format.</summary>
	public override string ToString() {
		return GetString(keyCode, modifier);
	}

	/// <summary>Convert the key binding to its text format.</summary>
	public static string GetString(KeyCode keyCode, Modifier modifier) {
		return modifier != Modifier.None ? modifier + "+" + keyCode : keyCode.ToString();
	}

	/// <summary>Given the ToString() text, parse it for key and modifier information.</summary>
	public static bool GetKeyCode(string text, out KeyCode key, out Modifier modifier) {
		key = KeyCode.None;
		modifier = Modifier.None;
		if(string.IsNullOrEmpty(text)) return false;

		if(text.Contains("+")) {
			var parts = text.Split('+');

			try {
				modifier = (Modifier) System.Enum.Parse(typeof(Modifier), parts[0]);
				key = (KeyCode) System.Enum.Parse(typeof(KeyCode), parts[1]);
			}
			catch (System.Exception) {
				return false;
			}
		}
		else {
			modifier = Modifier.None;
			try {
				key = (KeyCode) System.Enum.Parse(typeof(KeyCode), text);
			}
			catch (System.Exception) {
				return false;
			}
		}
		return true;
	}

	/// <summary>Get the currently active key modifier, if any.</summary>
	public static Modifier GetActiveModifier() {
		var mod = Modifier.None;

		if(UICamera.GetKey(KeyCode.LeftAlt) || UICamera.GetKey(KeyCode.RightAlt)) mod = Modifier.Alt;
		else if(UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift)) mod = Modifier.Shift;
		else if(UICamera.GetKey(KeyCode.LeftControl) || UICamera.GetKey(KeyCode.RightControl)) mod = Modifier.Control;

		return mod;
	}
}