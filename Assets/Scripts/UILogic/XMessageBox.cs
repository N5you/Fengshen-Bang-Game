using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XMessageBox")]
public class XMessageBox : XUIBaseLogic
{
	public UILabel LabelContent = null;
	public UIImageButton ButtonConfirm = null;
	public UIImageButton ButtonCancel = null;
	
	private object arg1 = null;
	private object arg2 = null;
	
	public override bool Init()
	{
		base.Init();
		UIEventListener listen = UIEventListener.Get(ButtonConfirm.gameObject);
		listen.onClick += OnClickConfirm;
		
		listen = UIEventListener.Get(ButtonCancel.gameObject);
		listen.onClick += OnClickCancel;
		return true;
	}
	
	public void MessageBox(object arg1, object arg2, object arg3)
	{
		UIEventListener listen1 = UIEventListener.Get(ButtonConfirm.gameObject);
		UIEventListener listen2 = UIEventListener.Get(ButtonCancel.gameObject);
		if(this.arg1 != null)
		{
			UIEventListener.VoidDelegate OKDelegate = this.arg1 as UIEventListener.VoidDelegate;
			listen1.onClick -= OKDelegate;
		}
		
		if(this.arg2 != null)
		{
			UIEventListener.VoidDelegate CancelDelegate = this.arg2 as UIEventListener.VoidDelegate;
			listen2.onClick -= CancelDelegate;
		}
		
		this.arg1	= arg1;
		this.arg2	= arg2;
		
		UIEventListener.VoidDelegate OKDelegateF = arg1 as UIEventListener.VoidDelegate;
		UIEventListener.VoidDelegate CancelDelegateF = arg2 as UIEventListener.VoidDelegate;		
		
		listen1.onClick	+= OKDelegateF;		
		listen2.onClick	+= CancelDelegateF;
		
		LabelContent.text = (string)arg3;
	}
	
	private void OnClickConfirm(GameObject go)
	{
		Hide();
	}
	
	private void OnClickCancel(GameObject go)
	{
		Hide();
	}
}