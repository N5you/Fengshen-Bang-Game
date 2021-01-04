using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XFunctionBottomTR")]
public class XFunctionBottomTR : XUIBaseLogic
{
	public static GameObject RootParent;
	
	private Vector2	startPos = new Vector2(-220.0f,-24.0f);
	private float FuncWidth = 60;
	private float FuncHeight= 60;
	private static int MAX_FUNC_ROW_NUM		= 3;
	private static int MAX_FUNC_LIST_NUM	= 10;
	
	private static Vector2[,]	sFuncPos = new Vector2[MAX_FUNC_ROW_NUM,MAX_FUNC_LIST_NUM];	
	public GameObject		mBtnRoot;
	public UIImageButton	BtnExample;
	
	private SortedList<uint,SingleFuncObj>	mFuncGO = new SortedList<uint,SingleFuncObj>();
	private SortedList<EFeatureID, UIEventListener.VoidDelegate> FeatureList		= new SortedList<EFeatureID, UIEventListener.VoidDelegate>();
	private SortedList<uint, XU3dEffect>	m_effectStopByClick = new SortedList<uint,XU3dEffect>();
	private SortedList<uint, XU3dEffect>	m_effectStopByMsg = new SortedList<uint,XU3dEffect>();
	private SortedList<EFeatureID,FuncItem>		mFuncEffect = new SortedList<EFeatureID,FuncItem>();
	
	private LinkedList<GameObject> linkedEffectObj = new LinkedList<GameObject>();
	
	class  SingleFuncObj
	{
		public GameObject	mGO;
		public uint		mSortID;
		public int		CurX;
		public int		CurY;
		
		public void MoveNext()
		{
			if(mGO == null)
				return ;
			
			TweenPosition posEffect = mGO.GetComponent<TweenPosition>();
			if(posEffect == null)
				return ;
			
			Vector3 temp = mGO.transform.localPosition;
			
			posEffect.Reset();
			CurX++;
			if(CurX >= MAX_FUNC_LIST_NUM)
			{
				CurX	= 0;
				CurY++;
			}			
			posEffect.from	= temp;
			posEffect.to	= new Vector3(sFuncPos[CurY,CurX].x,sFuncPos[CurY,CurX].y,mGO.transform.localPosition.z);
			
			posEffect.enabled	= true;
		}
	}
	
	class FuncItem
	{
		public uint FeatureID;
		public XFunctionBottomTR	UI;
		
		public void ClickFuncBtn(GameObject go)
		{
			UI.StopEffect(FeatureID,900039u, 1);
			UIEventListener ls = go.GetComponent<UIEventListener>();
			if(ls != null)
				ls.onClick -= ClickFuncBtn;
		}
	}
	
	public override bool Init()
	{
		RootParent = this.gameObject;
			
		FeatureList.Add(EFeatureID.EFeatureID_BaFangPai,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_HuoYueDu,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_ShanHeTu,new UIEventListener.VoidDelegate(ClickShanHT));
		FeatureList.Add(EFeatureID.EFeatureID_ShouChong,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_MeiRiQianDao,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_JingJiChang,new UIEventListener.VoidDelegate(ClickPVP));
		FeatureList.Add(EFeatureID.EFeatureID_XianDaoHui,new UIEventListener.VoidDelegate(ClickPVP));
		FeatureList.Add(EFeatureID.EFeatureID_PaiHang,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_FuLinMen,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Cultivate,new UIEventListener.VoidDelegate(ClickPractise));
		
		FeatureList.Add(EFeatureID.EFeatureID_WuZiTS,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_BossBat,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_TeamDup,new UIEventListener.VoidDelegate(ClickBtn));
		FeatureList.Add(EFeatureID.EFeatureID_MoneyTree,new UIEventListener.VoidDelegate(ClickMoneyTree));
		FeatureList.Add(EFeatureID.EFeatureID_AddAward,new UIEventListener.VoidDelegate(ClickGetAward));
		FeatureList.Add(EFeatureID.EFeatureID_SevenTarget, new UIEventListener.VoidDelegate(ClickSevenTarget));
		FeatureList.Add(EFeatureID.EFeatureID_HillSeal, new UIEventListener.VoidDelegate(ClickHillSeal));
		FeatureList.Add(EFeatureID.EFeatureID_KillMonster, new UIEventListener.VoidDelegate(ClickKillMonster));
		FeatureList.Add(EFeatureID.EFeatureID_Auciton, new UIEventListener.VoidDelegate(ClickAuction));
		
		for(int i = 0; i < MAX_FUNC_ROW_NUM; i++)
		{
			for(int j = 0;j < MAX_FUNC_LIST_NUM; j++)
			{
				float x = startPos.x - j*FuncWidth;
				float y = startPos.y - i*FuncHeight;
				sFuncPos[i,j]	= new Vector2(x,y);
			}
		}
		
		for(EFeatureID index = EFeatureID.EFeatureID_BaFangPai; index <= EFeatureID.EFeatureID_HillSeal; index++)
		{
			mFuncEffect[index]	= new FuncItem();
			
			FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig((uint)index);
			if(unLock == null)
				continue ;
			mFuncEffect[index].FeatureID	= (uint)index;
			mFuncEffect[index].UI			= this;
		}
		
		//special
		mFuncEffect[EFeatureID.EFeatureID_AddAward]			= new FuncItem();
		mFuncEffect[EFeatureID.EFeatureID_AddAward].FeatureID	= (uint)EFeatureID.EFeatureID_AddAward;
		mFuncEffect[EFeatureID.EFeatureID_AddAward].UI			= this;	

		return base.Init();
	}
	
	public void AddAwardEventHandler(EEvent evt, params object[] args)
	{
		AddAwardTail();
	}
	
	public void AddBtnDirect(List<uint> canUnLockList)
	{
		SortedDictionary<uint,uint>	tempDict = new SortedDictionary<uint, uint>();
		//添加的按钮数据
		foreach(uint temp in canUnLockList)
		{
			FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig(temp);
			if(unLock == null)
				continue;
			
			if(unLock.Group != (uint)FeatureType.EFeatureType_Activity)
				continue;
			
			if(unLock.AnimationType == 1 && !tempDict.ContainsKey(unLock.SortID))
				tempDict.Add(unLock.SortID,temp);
			else
			{
				//Log.Write(LogLevel.WARN,"repeat id {0}",unLock.SortID);
			}
		}
		
		foreach(int id in FeatureDataUnLockMgr.SP.mOpenFeatureIDList)
		{
			FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig((uint)id);
			if(unLock == null)
				continue;
			
			if(unLock.Group != (uint)FeatureType.EFeatureType_Activity)
				continue;
			
			if(!tempDict.ContainsKey(unLock.SortID))
				tempDict.Add(unLock.SortID,(uint)id);
		}
		
		foreach(SingleFuncObj obj in mFuncGO.Values)
		{
			NGUITools.Destroy(obj.mGO);
		}
		
		mFuncGO.Clear();
		
		int count = 0;
		foreach(KeyValuePair<uint,uint> temp1 in tempDict)
		{
			
			FeatureUnLock unLock1 = FeatureUnLockMgr.SP.GetConfig(temp1.Value);
			if(unLock1 == null)
				continue;
			
			if(unLock1.MixID > 0)
			{
				FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
				continue ;
			}
			SingleFuncObj	newsfo = new SingleFuncObj();
			newsfo.mGO	= XUtil.Instantiate(BtnExample.gameObject,mBtnRoot.transform,new Vector3(0,0,0),new Vector3(0,0,0));
			newsfo.mSortID	= temp1.Value;
			
			newsfo.mGO.SetActive(true);
			
			UIImageButton imageBtn = newsfo.mGO.GetComponent<UIImageButton>();
			if(imageBtn == null)
				continue;
			
			XUIDynamicAtlas.SP.SetSprite(imageBtn.target, (int)unLock1.AtlasID, unLock1.IconID_Com, true, null);
			imageBtn.normalSprite	= unLock1.IconID_Com;
			imageBtn.hoverSprite	= unLock1.IconID_Hover;
			imageBtn.pressedSprite	= unLock1.IconID_Pressed;
			
			UIEventListener listen = UIEventListener.Get(imageBtn.gameObject);
			if(FeatureList.ContainsKey((EFeatureID)temp1.Value))
				listen.onClick	+= FeatureList[(EFeatureID)temp1.Value];
			
			int indexX	= 0;
			int indexY	= 0;
			
			indexX = count / MAX_FUNC_LIST_NUM;
			indexY = count % MAX_FUNC_LIST_NUM;
			
			newsfo.mGO.transform.localPosition	= new Vector3(sFuncPos[indexX,indexY].x,sFuncPos[indexX,indexY].y,newsfo.mGO.transform.localPosition.z);
			newsfo.CurX	= indexY;
			newsfo.CurY	= indexX;
			mFuncGO[newsfo.mSortID] = newsfo;
			count++;
			
			TweenAlpha AlphaEffect = newsfo.mGO.GetComponent<TweenAlpha>();
			if(AlphaEffect != null)
			{				
				AlphaEffect.enabled	= false;
			}
			
			FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
			
			XNewPlayerGuideManager.SP.handleFunBtnTr((EFeatureID)temp1.Value, newsfo.mGO);
			
			// 七日目标每次打开都需要一个特效提示
			if ( (EFeatureID)temp1.Value == EFeatureID.EFeatureID_SevenTarget )
			{
				StartEffect(newsfo.mSortID, 900039u, 1);
			}
		}
	}
	
	public void AddBtn(uint canUnLockID,bool isNeedAnim)
	{
		SortedDictionary<uint,uint>	tempDict = new SortedDictionary<uint, uint>();
		//添加的按钮数据
		
		FeatureUnLock CurUnLock = FeatureUnLockMgr.SP.GetConfig(canUnLockID);
		if(CurUnLock == null)
			return ;
		
		if(CurUnLock.Group != (uint)FeatureType.EFeatureType_Activity)
			return ;
		
		if(!tempDict.ContainsKey(CurUnLock.SortID))
			tempDict.Add(CurUnLock.SortID,canUnLockID);
		else
		{
			Log.Write(LogLevel.WARN,"repeat id {0}",CurUnLock.SortID);
		}		
		
		foreach(int id in FeatureDataUnLockMgr.SP.mOpenFeatureIDList)
		{
			FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig((uint)id);
			if(unLock == null)
				continue;
			
			if(unLock.Group != (uint)FeatureType.EFeatureType_Activity)
				continue;
			
			if(!tempDict.ContainsKey(unLock.SortID))
				tempDict.Add(unLock.SortID,(uint)id);
		}
		
		foreach(SingleFuncObj obj in mFuncGO.Values)
		{
			NGUITools.Destroy(obj.mGO);
		}
		
		mFuncGO.Clear();
		
		int count = 0;
		foreach(KeyValuePair<uint,uint> temp1 in tempDict)
		{
			
			FeatureUnLock unLock1 = FeatureUnLockMgr.SP.GetConfig(temp1.Value);
			if(unLock1 == null)
				continue;
			
			if(unLock1.MixID > 0)
			{
				FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
				continue ;
			}
			SingleFuncObj	newsfo = new SingleFuncObj();
			newsfo.mGO	= XUtil.Instantiate(BtnExample.gameObject,mBtnRoot.transform,new Vector3(0,0,0),new Vector3(0,0,0));
			newsfo.mSortID	= temp1.Value;
			
			newsfo.mGO.SetActive(true);
			
			UIImageButton imageBtn = newsfo.mGO.GetComponent<UIImageButton>();
			if(imageBtn == null)
				continue;
			
			XUIDynamicAtlas.SP.SetSprite(imageBtn.target, (int)unLock1.AtlasID, unLock1.IconID_Com, true, null);
			imageBtn.normalSprite	= unLock1.IconID_Com;
			imageBtn.hoverSprite	= unLock1.IconID_Hover;
			imageBtn.pressedSprite	= unLock1.IconID_Pressed;
			
			UIEventListener listen = UIEventListener.Get(imageBtn.gameObject);
			if(FeatureList.ContainsKey((EFeatureID)temp1.Value))
			{
				listen.onClick	+= FeatureList[(EFeatureID)temp1.Value];
			}
			
			int indexX	= 0;
			int indexY	= 0;
			
			indexX = count / MAX_FUNC_LIST_NUM;
			indexY = count % MAX_FUNC_LIST_NUM;
			
			newsfo.mGO.transform.localPosition	= new Vector3(sFuncPos[indexX,indexY].x,sFuncPos[indexX,indexY].y,newsfo.mGO.transform.localPosition.z);
			newsfo.CurX	= indexY;
			newsfo.CurY	= indexX;
			mFuncGO[newsfo.mSortID] = newsfo;
			count++;
			
			TweenAlpha AlphaEffect = newsfo.mGO.GetComponent<TweenAlpha>();
			if(AlphaEffect != null)
			{				
				AlphaEffect.enabled	= false;
			}
			
			FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
			
			if(temp1.Value == canUnLockID)
			{
				//listen.onClick	+= new UIEventListener.VoidDelegate(mFuncEffect[(EFeatureID)temp1.Value].ClickFuncBtn);
				//XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT,newsfo.mSortID, 900039u, 1);
				StartEffect(newsfo.mSortID, 900039u, 1);
			}
			
			XNewPlayerGuideManager.SP.handleFunBtnTr((EFeatureID)temp1.Value, newsfo.mGO);
		}
	}
	
	
	public void StartEffect(uint fetureid, uint effectid, int stopByClick)
	{
		if ( !mFuncGO.ContainsKey(fetureid) || !mFuncEffect.ContainsKey((EFeatureID)fetureid))
			return;
		
		if ( 1 == stopByClick && m_effectStopByClick.ContainsKey(fetureid) )
			return;
		
		if ( 0 == stopByClick )
		{
			if ( m_effectStopByMsg.ContainsKey(fetureid) )
				return;
			if ( m_effectStopByClick.ContainsKey(fetureid) )
			{
				m_effectStopByMsg[fetureid] = m_effectStopByClick[fetureid];
				m_effectStopByClick.Remove(fetureid);
				return;
			}
		}
		
		SingleFuncObj obj = mFuncGO[fetureid];
		linkedEffectObj.AddLast(obj.mGO);
		
		if ( 1 == stopByClick )
		{
			UIEventListener listen = UIEventListener.Get(obj.mGO);
			listen.onClick	+= new UIEventListener.VoidDelegate(mFuncEffect[(EFeatureID)fetureid].ClickFuncBtn);
		}
		
		XU3dEffect effect = new XU3dEffect(effectid, EffectLoadedHandle);
		effect.Parent = obj.mGO.transform;
		
		if ( 1 == stopByClick )
			m_effectStopByClick[fetureid] = effect;
		else
			m_effectStopByMsg[fetureid] = effect;
	}
	
	public void StopEffect(uint fetureid, uint effectid, int stopByClick)
	{
		if ( 1 == stopByClick && !m_effectStopByClick.ContainsKey(fetureid) )
			return;
		if ( 0 == stopByClick && !m_effectStopByMsg.ContainsKey(fetureid) )
			return;
		
		XU3dEffect effect = null;
		if ( 1 == stopByClick )
		{
			effect = m_effectStopByClick[fetureid];
			m_effectStopByClick.Remove(fetureid);
		}
		else
		{
			effect = m_effectStopByMsg[fetureid];
			m_effectStopByMsg.Remove(fetureid);
		}
		effect.Destroy();
	}
	
	private void EffectLoadedHandle(XU3dEffect effect)
    {
		if ( 0 == linkedEffectObj.Count )
			return;
		
		GameObject obj = linkedEffectObj.First.Value;
		
        effect.Layer = GlobalU3dDefine.Layer_UI_2D;
        effect.Parent = obj.transform;
        effect.LocalPosition = new Vector3(0, -10, -10);
        effect.Scale = new Vector3(500, 500, 1);
		linkedEffectObj.RemoveFirst();
    }
	
	public Vector3 GetAwardPos()
	{
		if(!mFuncGO.ContainsKey(FeatureDataUnLockMgr.AddAward_SortID))
			return Vector3.zero;
		
		SingleFuncObj	obj = mFuncGO[FeatureDataUnLockMgr.AddAward_SortID];
		return obj.mGO.transform.position;
	}
	
	public void AddAwardTail()
	{
		if(mFuncGO.ContainsKey(FeatureDataUnLockMgr.AddAward_SortID))
		{
			StartEffect(FeatureDataUnLockMgr.AddAward_SortID, 900039u, 1);
			return ;
		}
		
		SortedDictionary<uint,uint>	tempDict = new SortedDictionary<uint, uint>();
		tempDict.Add(FeatureDataUnLockMgr.AddAward_SortID,(uint)EFeatureID.EFeatureID_AddAward);
		int count = 0;
		foreach(KeyValuePair<uint,uint> temp1 in tempDict)
		{
			
			FeatureUnLock unLock1 = FeatureUnLockMgr.SP.GetConfig(temp1.Value);
			if(unLock1 == null)
				continue;

			SingleFuncObj	newsfo = new SingleFuncObj();
			newsfo.mGO	= XUtil.Instantiate(BtnExample.gameObject,mBtnRoot.transform,new Vector3(0,0,0),new Vector3(0,0,0));
			newsfo.mSortID	= temp1.Value;
			
			newsfo.mGO.SetActive(true);
			
			UIImageButton imageBtn = newsfo.mGO.GetComponent<UIImageButton>();
			if(imageBtn == null)
				continue;
			
			XUIDynamicAtlas.SP.SetSprite(imageBtn.target, (int)unLock1.AtlasID, unLock1.IconID_Com, true, null);
			imageBtn.normalSprite	= unLock1.IconID_Com;
			imageBtn.hoverSprite	= unLock1.IconID_Hover;
			imageBtn.pressedSprite	= unLock1.IconID_Pressed;
			
			UIEventListener listen = UIEventListener.Get(imageBtn.gameObject);
			if(FeatureList.ContainsKey((EFeatureID)temp1.Value))
				listen.onClick	+= FeatureList[(EFeatureID)temp1.Value];
			
			int indexX	= 0;
			int indexY	= 0;
			
			indexX = count / MAX_FUNC_LIST_NUM;
			indexY = count % MAX_FUNC_LIST_NUM;
			
			newsfo.mGO.transform.localPosition	= new Vector3(sFuncPos[indexX,indexY].x,sFuncPos[indexX,indexY].y,newsfo.mGO.transform.localPosition.z);
			newsfo.CurX	= indexY;
			newsfo.CurY	= indexX;
			mFuncGO[newsfo.mSortID] = newsfo;
			count++;
			
			TweenAlpha AlphaEffect = newsfo.mGO.GetComponent<TweenAlpha>();
			if(AlphaEffect != null)
			{				
				AlphaEffect.enabled	= false;
			}
			
			FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
			
			listen.onClick	+= new UIEventListener.VoidDelegate(mFuncEffect[(EFeatureID)temp1.Value].ClickFuncBtn);
			XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT,FeatureDataUnLockMgr.AddAward_SortID,(uint)900039, 1);
		}
	}
	
	public void DelButton(uint featurid)
	{
		if(!mFuncGO.ContainsKey(featurid))
			return ;
		
		SingleFuncObj obj = mFuncGO[featurid];
		NGUITools.Destroy(obj.mGO);
		mFuncGO.Remove(featurid);
	}
	
	public void ClickBtn(GameObject _go)
	{
		
	}
	
	public void ClickMoneyTree(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Open);
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eMoneyTree);
	}
	
	public void ClickGetAward(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eGetAward);
	}
		
	public void ClickSevenTarget(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eSevenTarget);
	}
	
	public void ClickPractise(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.ePractice);
	}
	
	public void ClickHillSeal(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_OpenSealHill);
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eHillSeaBook);
	}
	
	public void ClickKillMonster(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_OpenZhanYaoLu);
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eZhanYaoLu);
	}
	
	public void ClickAuction(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_Auction);
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eAuction);
	}
	
	public void ClickShanHT(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_OpenShanHeTu);
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eShanHT);
	}
	
	public void ClickPVP(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_OpenXianDaoHui);
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eXianDH);
	}
	
	
}
