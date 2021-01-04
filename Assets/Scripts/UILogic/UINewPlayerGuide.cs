using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/UINewPlayerGuide")]
public class UINewPlayerGuide : XUIBaseLogic
{	
	private Vector3 ShowPosition;
	
	
	private uint EffectId;
	
	private bool showImage = false;
	private bool showLabel = false;
	private bool showItem1 = false;
	private bool showItem2 = false;
	
	public UILabel Label_Des;
	public UISprite image;
	public GameObject item1Obj;
	public GameObject item2Obj;
	public UILabel item1label;
	public UILabel item2label;
	
	private int key;
	public bool deltaFinishing = false;
	
	public override bool Init()
	{
		return true;
	}
	
	public override void Show()
	{
		ShowPosition.z = ShowPosition.z - 1;
		transform.localPosition = ShowPosition;
		if ( showLabel )
		{
			StartEffect(EffectId);
		}
		image.gameObject.SetActive(showImage);
		Label_Des.gameObject.SetActive(showLabel);
		item1Obj.SetActive(showItem1);
		item2Obj.SetActive(showItem2);
		
		base.Show();
	}
	
	public void DisapperDelta(float time, int k)
	{
		key = k;
		deltaFinishing = true;
		Invoke("DisapperSelf", time);
	}
	
	public void DisapperSelf()
	{
		deltaFinishing = false;
		XNewPlayerGuideManager.SP.handleGuideFinish(key);
	}
	
	public override void ShowData(params object[] args)
	{
		if ( args.Length < 3 )
			return;
		
		ShowPosition = (Vector3)args[2];
		if ( 0u == (uint)args[3] )							// 显示特效和文字
		{
			showImage = false;
			showLabel = true;
			showItem1 = false;
			showItem2 = false;
			Label_Des.text = (string)args[0];
			EffectId = (uint)args[1];
		}
		if ( 1u == (uint)args[3] )							// 只显示图片
		{
			showImage = true;
			showLabel = false;
			showItem1 = false;
			showItem2 = false;
			image.spriteName = ((uint)args[1]).ToString();
		}
		else if ( 2u == (uint)args[3] )						// 显示左侧提示
		{
			showImage = false;
			showLabel = false;
			showItem1 = true;
			showItem2 = false;
			item1label.text = (string)args[0];
		}
		else if ( 3u == (uint)args[3] )						// 显示右侧提示
		{
			showImage = false;
			showLabel = false;
			showItem1 = false;
			showItem2 = true;
			item2label.text = (string)args[0];
		}
	}
	
	private void StartEffect(uint effectid)
	{
		XU3dEffect effect = new XU3dEffect(effectid, EffectLoadedHandle);
	}
	
	private void EffectLoadedHandle(XU3dEffect effect)
    {
		if ( null == transform )
			return;
		
        effect.Layer = GlobalU3dDefine.Layer_UI_2D;
        effect.Parent = transform;
        effect.LocalPosition = new Vector3(0, -10, -50);
        effect.Scale = new Vector3(900, 800, 1);
    }
}

