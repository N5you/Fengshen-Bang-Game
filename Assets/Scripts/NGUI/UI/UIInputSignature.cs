using UnityEngine;

/// <summary>
/// Editable text input field.
/// </summary>

[AddComponentMenu("NGUI/UI/Input (Signature)")]
public class UIInputSignature : UIInput
{
	public override string text {
		get {
			return base.text;
		}
		set {
			
			if (mDoInit)
				base.Init();
			
			mText = value;

			if (label != null)
			{
				// label.supportEncoding = false;
				label.text = value;
				label.showLastPasswordChar = selected;
				label.color = (selected || value != mDefaultText) ? activeColor : mDefaultColor;
			}
		}
	}
	
//	protected sealed override void OnSelect (bool isSelected)
//	{
//		if (mDoInit) base.Init();
//
//		if (label != null && base.enabled && NGUITools.GetActive(base.gameObject))
//		{
//			if (isSelected)
//			{
//				mText = (!useLabelTextAtStart && label.text == mDefaultText) ? "" : label.text;
//				label.color = activeColor;
//				if (base.isPassword) label.password = true;
//
//#if UNITY_IPHONE || UNITY_ANDROID
//				if (Application.platform == RuntimePlatform.IPhonePlayer ||
//					Application.platform == RuntimePlatform.Android)
//				{
//#if UNITY_3_4
//					mKeyboard = iPhoneKeyboard.Open(mText, (iPhoneKeyboardType)((int)type), autoCorrect);
//#else
//					if (isPassword)
//					{
//						mKeyboard = TouchScreenKeyboard.Open(mText, TouchScreenKeyboardType.Default, false, false, true);
//					}
//					else
//					{
//						mKeyboard = TouchScreenKeyboard.Open(mText, (TouchScreenKeyboardType)((int)type), autoCorrect);
//					}
//#endif
//				}
//				else
//#endif
//				{
//					Input.imeCompositionMode = IMECompositionMode.On;
//					Transform t = label.cachedTransform;
//					Vector3 offset = label.pivotOffset;
//					offset.y += label.relativeSize.y;
//					offset = t.TransformPoint(offset);
//					Input.compositionCursorPos = UICamera.currentCamera.WorldToScreenPoint(offset);
//				}
//				UpdateLabel();
//			}
//			else
//			{
//#if UNITY_IPHONE || UNITY_ANDROID
//				if (mKeyboard != null)
//				{
//					mKeyboard.active = false;
//				}
//#endif
//				if (string.IsNullOrEmpty(mText))
//				{
//					label.text = "";
//					label.color = mDefaultColor;
//					if (isPassword) label.password = false;
//				}
//				else label.text = mText;
//
//				label.showLastPasswordChar = false;
//				Input.imeCompositionMode = IMECompositionMode.Off;
//				RestoreLabel();
//			}
//		}
//		
//	}
}
