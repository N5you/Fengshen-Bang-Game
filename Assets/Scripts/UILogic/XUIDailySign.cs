using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XUIDailySign")]
public class XUIDailySign : XDefaultFrame
{
	public UISprite bigBkObject;
	
	public UISprite[] JiangLiSprite;
	public UISprite[] LiangeBianSprite;
	public UISprite[] TianShuSprite;
	
	private string[] liangbianStr = {"11003004", "11003005", "11003006", "11003007", "11003008", "11003009"};
	
	private JiangLiItem[] jiangLiItems = new JiangLiItem[30];
	
	public static bool OnShowIng = false;
	
	public class JiangLiItem
	{
		public JiangLiItem(GameObject go, int index)
		{
			UIEventListener lis = UIEventListener.Get(go);
			lis.onClick += onClickGetAward;
			
			mindex = index;
			spriteGo = go;
		}
		
		private void onClickGetAward(GameObject go)
		{
			if ( DailySignManager.SP.GetCanAward(mindex) )
				DailySignManager.SP.GetAward(mindex);
			else
				XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1083));
		}
		
		public void StartEffect(uint effectid, int size)
		{
			if ( effect != null )
				effect.Destroy();
			
			effectSize = size;
			effect = new XU3dEffect(effectid, EffectLoadedHandle);
		}
		
		private void EffectLoadedHandle(XU3dEffect effect)
   		{
			GameObject obj = spriteGo;
        	effect.Layer = GlobalU3dDefine.Layer_UI_2D;
        	effect.Parent = obj.transform;
        	effect.LocalPosition = new Vector3(0, 0, -50);
        	effect.Scale = new Vector3(effectSize, effectSize, 1);
   		}
		
		public void DestroyEffect()
		{
			if ( null != effect )
				effect.Destroy();
		}
		
		int mindex;
		private XU3dEffect effect;
		private int effectSize = 10;
		private GameObject spriteGo;
	}
	
	public override bool Init ()
	{
		base.Init();
		
		for( int i = 0; i < 30; i++ )
			jiangLiItems[i] = new JiangLiItem(JiangLiSprite[i].gameObject, i);
		
		UIEventListener lis = UIEventListener.Get(bigBkObject.gameObject);
		lis.onClick += OnclickBigBk;
		

		
		return true;
	}
	
	public override void Show ()
	{
		OnShowIng = true;
		
		base.Show ();
		
		if ( XForceGuide.OnShowIng )
			XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eDailySign);
	}
	
	public override void Hide()
	{
		OnShowIng = false;
		
		base.Hide();
		for( int i = 0; i < 30; i++ )
			JiangLiSprite[i].Disable();
	}
	
	private void OnclickBigBk(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.ToolTip_CenterTip, ECenterTipStyle.Up, XStringManager.SP.GetString(1083));
	}
	
	public void SetDailySignStatus(ulong dailySigned, ulong dailyStatus)
	{
		for ( int i = 0; i < 30; i++ )
		{
			XCfgDailySign config = XCfgDailySignMgr.SP.GetConfig((byte)(i + 1));
			if ( null == config )
				continue;
			
			ulong tag1 = 1;
			tag1 = (dailySigned >> i) & tag1;
			ulong tag2 = 1;
			tag2 = (dailyStatus >> i) & tag2;
			
			// 已签到且奖励已领取
			if ( 1 == tag2 )
			{
				JiangLiSprite[i].spriteName = "11001070";
				LiangeBianSprite[i].gameObject.SetActive(false);
			}
			else
			{
				JiangLiSprite[i].spriteName = config.IconID.ToString();
				JiangLiSprite[i].CommonTips = XGameColorDefine.Quality_Color[config.ColorLevel] + config.Tips1;
				LiangeBianSprite[i].spriteName = liangbianStr[config.ColorLevel - 1];
			}
			
			if( 1 == tag1 && 0 == tag2 )
				jiangLiItems[i].StartEffect(900049, 10);
		}
	}
	
	public void HideSelf()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eDailySign);
	}
	
	public void StartFinishEffect(ulong dailyStatus)
	{
	//	return;
		
		int pos2addeffet = -1;
		for( int i = 0 ; i < 30; i++ )
		{
			ulong tag = 1;
			tag = tag << 1;
			if ( 0 == (tag & dailyStatus) )
			{
				pos2addeffet = i;
				break;
			}
		}
		if ( -1 == pos2addeffet )
			return;
		
		TweenScale scaleEffect = JiangLiSprite[pos2addeffet].gameObject.GetComponent<TweenScale>();
		if(scaleEffect != null)
		{
			scaleEffect.Reset();
			scaleEffect.enabled	= true;
		}
		jiangLiItems[pos2addeffet].DestroyEffect();
	}
}
