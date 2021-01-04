using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XFuncUnLock")]
public class XFuncUnLock : XUIBaseLogic
{
	public GameObject		go;
	public UIImageButton 	ImageBtn;
	public UISprite			SpriteBK;
	public UILabel			Label;
	private Vector3			OrignalPos = new Vector3();
	private Vector3			TargetPos = new Vector3();
	
	public bool				IsMix;
	private GameObject		mNewObject;
	
	public override bool Init()
	{
		base.Init();
		OrignalPos	= ImageBtn.transform.position;
		
		UIEventListener ls = UIEventListener.Get(ImageBtn.gameObject);
		ls.onClick	= Click;
		
		UIEventListener ls2 = UIEventListener.Get(SpriteBK.gameObject);
		ls2.onClick	= Click;
		return true;
	}
	
	public void Click(GameObject go)
	{
		FeatureDataUnLockMgr.SP.IsCanContinue	= true;
	}
	
	public override void Show()
	{
		base.Show();
		ImageBtn.transform.position	= OrignalPos;
	}
	
	public void Finish()
	{
		Hide();
	}	
	
	public void FlySprite(Vector3 target)
	{		
		TargetPos	= new Vector3(target.x,target.y,-200.0f);
		_DelayFly();
	}
	
	public void _DelayFly()
	{
		mNewObject = XUtil.Instantiate(ImageBtn.gameObject,null,ImageBtn.transform.position,ImageBtn.transform.localScale);
		TweenPosition PosEffect = mNewObject.GetComponent<TweenPosition>();
		if(PosEffect != null)
		{
			PosEffect.Reset();
			PosEffect.from	= OrignalPos;
			PosEffect.to	= TargetPos;
			PosEffect.enabled	= true;
		}
		
		if(IsMix)
			Invoke("OnFlyFinish",2);
		
		NcAutoDestruct AlpahEffect = mNewObject.GetComponent<NcAutoDestruct>();
		if(AlpahEffect != null)
		{
			AlpahEffect.enabled	= true;
		}
		
		Hide();
	}
	
	public void OnFlyFinish()
	{		
		TweenPosition PosEffect = mNewObject.GetComponent<TweenPosition>();
		if(PosEffect != null)
		{
			PosEffect.Reset();
			PosEffect.from	= TargetPos;
			PosEffect.to	= new Vector3(TargetPos.x,TargetPos.y - 50,TargetPos.z);
			PosEffect.enabled	= true;
		}
		
		TweenAlpha AlpahEffect = mNewObject.GetComponent<TweenAlpha>();
		if(AlpahEffect != null)
		{
			AlpahEffect.Reset();
			AlpahEffect.from	= 1.0f;
			AlpahEffect.to		= 0.0f;
			AlpahEffect.enabled	= true;
		}
	}
}
