using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;

public class XFunctionButton : XUIBaseLogic
{
	private Vector2	startPos = new Vector2(650.0f,0.0f);
	private float FuncWidth = 60;
	private float FuncHeight= 60;
	private static int MAX_FUNC_ROW_NUM		= 3;
	private static int MAX_FUNC_LIST_NUM	= 10;
	
	private static Vector2[,]	sFuncPos = new Vector2[MAX_FUNC_ROW_NUM,MAX_FUNC_LIST_NUM];
	public GameObject	mBtnRoot;	
	public UISlider	RoleExp = null;
	public UILabel	ExpLabel;
	
	private LinkedList<GameObject> linkedEffectObj = new LinkedList<GameObject>();
	private uint CurOpBtn;
	
	class  SingleFuncObj
	{
		public GameObject	mGO;
		public uint		FuncBtnID;
		public uint		SortID;
		public int		CurX;
		public int		CurY;
		public Vector3  CurPos;
		
		
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
			CurPos			= posEffect.to;
			posEffect.enabled	= true;
		}
	}
	
	class FuncItem
	{
		public uint FeatureID;
		public XFunctionButton	UI;
		
		public void ClickFuncBtn(GameObject go)
		{
			UI.StopEffect(FeatureID,900049u);
			UIEventListener ls = go.GetComponent<UIEventListener>();
			if(ls != null)
				ls.onClick -= ClickFuncBtn;
		}
	}
	
	private SortedList<EFeatureID, UIEventListener.VoidDelegate> FeatureList		= new SortedList<EFeatureID, UIEventListener.VoidDelegate>();
	
	private SortedList<uint,SingleFuncObj>	mFuncGO = new SortedList<uint,SingleFuncObj>();
	private SortedList<EFeatureID,FuncItem>		mFuncEffect = new SortedList<EFeatureID,FuncItem>();
	
	private SortedList<uint, XU3dEffect>	m_effect = new SortedList<uint,XU3dEffect>();
	
	public UIImageButton	BtnExample;

	public override bool Init()
	{		
		FeatureList.Add(EFeatureID.EFeatureID_Char,new UIEventListener.VoidDelegate(ClickRoleBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Bag,new UIEventListener.VoidDelegate(ClickBagBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Skill,new UIEventListener.VoidDelegate(ClickSkillBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Product,new UIEventListener.VoidDelegate(ClickProduct));
		FeatureList.Add(EFeatureID.EFeatureID_FaBao,new UIEventListener.VoidDelegate(ClickFaBao));
		FeatureList.Add(EFeatureID.EFeatureID_Quest,new UIEventListener.VoidDelegate(ClickQuestBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Strengthen,new UIEventListener.VoidDelegate(ClickStrengthenBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Friend,new UIEventListener.VoidDelegate(ClickFriendBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Garden,new UIEventListener.VoidDelegate(ClickRoleBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Formation,new UIEventListener.VoidDelegate(ClickFormationBtn));
		FeatureList.Add(EFeatureID.EFeatureID_Family,new UIEventListener.VoidDelegate(ClickFamilyBtn));
//		FeatureList.Add(EFeatureID.EFeatureID_Cultivate,new UIEventListener.VoidDelegate(ClickPractise));
		
		
		for(int i = 0; i < MAX_FUNC_ROW_NUM; i++)
		{
			for(int j = 0;j < MAX_FUNC_LIST_NUM; j++)
			{
				float x = startPos.x - j*FuncWidth;
				float y = startPos.y + i*FuncHeight;
				sFuncPos[i,j]	= new Vector2(x,y);
			}
		}
		
		for(EFeatureID index = EFeatureID.EFeatureID_Char; index <= EFeatureID.EFeatureID_Cultivate; index++)
		{
			mFuncEffect[index]	= new FuncItem();
			
			FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig((uint)index);
			if(unLock == null)
				continue ;
			mFuncEffect[index].FeatureID	= (uint)index;
			mFuncEffect[index].UI			= this;
		}

		return base.Init();
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
			
			if(unLock.Group != (uint)FeatureType.EFeatureType_Func)
				continue;
			
			if(unLock.AnimationType == 1 && !tempDict.ContainsKey(unLock.SortID))
			{
				tempDict.Add(unLock.SortID,temp);
			}
			else
			{
				if ( unLock.MixID > 0 )
				{
					FeatureDataUnLockMgr.SP.AddUnLock((int)temp);
				}
			}
		}
		
		foreach(int id in FeatureDataUnLockMgr.SP.mOpenFeatureIDList)
		{
			FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig((uint)id);
			if(unLock == null)
				continue;
			
			if(unLock.Group != (uint)FeatureType.EFeatureType_Func)
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
				if ( mFuncGO.ContainsKey(unLock1.MixID) )
					XNewPlayerGuideManager.SP.handleFunBtnGuide(temp1.Value, mFuncGO[unLock1.MixID].mGO);
				
				FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
				continue ;
			}
			SingleFuncObj	newsfo = new SingleFuncObj();
			newsfo.mGO	= XUtil.Instantiate(BtnExample.gameObject,mBtnRoot.transform,new Vector3(0,0,0),new Vector3(0,0,0));
			newsfo.SortID		= temp1.Key;
			newsfo.FuncBtnID	= temp1.Value;
			
			newsfo.mGO.SetActive(true);
			
			UIImageButton imageBtn = newsfo.mGO.GetComponent<UIImageButton>();
			if(imageBtn == null)
				continue;
			
			XUIDynamicAtlas.SP.SetSprite(imageBtn.target, (int)unLock1.AtlasID, unLock1.IconID_Com, true, null);
			imageBtn.target.depth = 1;
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
			mFuncGO[newsfo.FuncBtnID] = newsfo;
			count++;
			
			TweenAlpha AlphaEffect = newsfo.mGO.GetComponent<TweenAlpha>();
			if(AlphaEffect != null)
			{				
				AlphaEffect.enabled	= false;
			}
			
			FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
			
			XNewPlayerGuideManager.SP.handleFunBtnGuide(temp1.Value, newsfo.mGO);
		}
	}
	
	public void AddBtn(uint canUnLockID,bool isNeedAnim)
	{
		Dictionary<uint,uint>	tempDict = new Dictionary<uint, uint>();
		//添加的按钮数据
		
		FeatureUnLock unLock = FeatureUnLockMgr.SP.GetConfig(canUnLockID);
		if(unLock == null)
			return ;
		
		if(unLock.Group != (uint)FeatureType.EFeatureType_Func)
			return ;
		
		if(!tempDict.ContainsKey(unLock.SortID))
			tempDict.Add(unLock.SortID,canUnLockID);
		else
		{
			Log.Write(LogLevel.WARN,"repeat id {0}",unLock.SortID);
		}	
		
		int count = 0;
		foreach(KeyValuePair<uint,uint> temp1 in tempDict)
		{
 			FeatureUnLock unLock1 = FeatureUnLockMgr.SP.GetConfig(temp1.Value);
			if(unLock1 == null)
				continue;
			
			if(unLock1.MixID > 0)
			{
				if ( mFuncGO.ContainsKey(unLock1.MixID) )
					XNewPlayerGuideManager.SP.handleFunBtnGuide(temp1.Value, mFuncGO[unLock1.MixID].mGO);
				//SingleFuncObj posBtn = FindFuncBtn(temp1.Key);
				FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
			}
			else
			{
				int newPos = -1;
				List<SingleFuncObj> changeList	= new List<SingleFuncObj>();
				FindChangeFunc(temp1.Key,changeList,ref newPos);
				
				//启动每个对象的动画			
				for(int i = 0; i < changeList.Count; i++)
				{
					SingleFuncObj sfo = changeList[i];
					sfo.MoveNext();
				}
				
				SingleFuncObj	newsfo = new SingleFuncObj();
				newsfo.mGO	= XUtil.Instantiate(BtnExample.gameObject,mBtnRoot.transform,new Vector3(0,0,0),new Vector3(0,0,0));
				newsfo.SortID		= temp1.Key;
				newsfo.FuncBtnID	= temp1.Value;
				
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
					//listen.onClick	+= new UIEventListener.VoidDelegate(mFuncEffect[(EFeatureID)temp1.Value].ClickFuncBtn);
				}
				
				int indexX	= 0;
				int indexY	= 0;
				if(newPos != -1)
				{
					indexX = (int)newPos / MAX_FUNC_LIST_NUM;
					indexY = (int)newPos % MAX_FUNC_LIST_NUM;
				}
				else
				{
					indexX = count / MAX_FUNC_LIST_NUM;
					indexY = count % MAX_FUNC_LIST_NUM;
				}
				
				newsfo.mGO.transform.localPosition	= new Vector3(sFuncPos[indexX,indexY].x,sFuncPos[indexX,indexY].y,newsfo.mGO.transform.localPosition.z);
				newsfo.CurX	= indexY;
				newsfo.CurY	= indexX;
				mFuncGO[newsfo.FuncBtnID] = newsfo;
				newsfo.mGO.SetActive(false);
				count++;
				
				FeatureDataUnLockMgr.SP.AddUnLock((int)temp1.Value);
				CurOpBtn	= newsfo.FuncBtnID;
				
				Invoke("DelayStartEffect",2);
			}
		}
	}
	
	private void DelayStartEffect()
	{
		mFuncGO[CurOpBtn].mGO.SetActive(true);
		// XEventManager.SP.SendEvent(EEvent.FUNCTION_BUTTON_STARTEFFECT,CurOpBtn,900049u, 1);
		
		XNewPlayerGuideManager.SP.handleFunBtnGuide(CurOpBtn, mFuncGO[CurOpBtn].mGO);
	}
	
	public Vector3 FindFinalPos(uint canUnLockID)
	{
		FeatureUnLock unLock1 = FeatureUnLockMgr.SP.GetConfig(canUnLockID);
		if(unLock1 == null)
			return Vector3.zero;
		
		if(unLock1.MixID > 0)
		{
			SingleFuncObj funcBtn = FindFuncBtn(unLock1.SortID);
			if(funcBtn == null)
			{
				Log.Write(LogLevel.ERROR,"LockID {0} SortID {1}",canUnLockID,unLock1.SortID);
				return Vector3.zero;
			}
			return new Vector3(sFuncPos[funcBtn.CurY,funcBtn.CurX].x + mBtnRoot.transform.position.x,sFuncPos[funcBtn.CurY,funcBtn.CurX].y + 50+ mBtnRoot.transform.position.y,0);
		}
		else
		{
			List<SingleFuncObj> changeList	= new List<SingleFuncObj>();
			int newPos = -1;
			FindChangeFunc(unLock1.SortID,changeList,ref newPos);
			
			int indexX	= 0;
			int indexY	= 0;
				
			if(newPos != -1)
			{
				indexX = (int)newPos / MAX_FUNC_LIST_NUM;
				indexY = (int)newPos % MAX_FUNC_LIST_NUM;
			}		
			
			return new Vector3(sFuncPos[indexX,indexY].x + mBtnRoot.transform.position.x,sFuncPos[indexX,indexY].y + mBtnRoot.transform.position.y,0);
		}
	}
	
	public void StartEffect(uint fetureid, uint effectid)
	{
		if ( !mFuncEffect.ContainsKey((EFeatureID)fetureid) )
			return;
		
		if ( !mFuncGO.ContainsKey(fetureid) || m_effect.ContainsKey(fetureid) )
			return;
		
		SingleFuncObj obj = mFuncGO[fetureid];
		linkedEffectObj.AddLast(obj.mGO);
		
		UIEventListener listen = UIEventListener.Get(obj.mGO);
		listen.onClick	+= new UIEventListener.VoidDelegate(mFuncEffect[(EFeatureID)fetureid].ClickFuncBtn);
		
		XU3dEffect effect = new XU3dEffect(effectid, EffectLoadedHandle);
		m_effect[fetureid] = effect;
	}
	
	public void StopEffect(uint fetureid, uint effectid)
	{
		if ( !m_effect.ContainsKey(fetureid) )
			return;
		
		XU3dEffect effect = m_effect[fetureid];
		effect.Destroy();
		m_effect.Remove(fetureid);
	}
	
	private void EffectLoadedHandle(XU3dEffect effect)
    {
		if ( 0 == linkedEffectObj.Count )
			return;
		
		GameObject obj = linkedEffectObj.First.Value;
        effect.Layer = GlobalU3dDefine.Layer_UI_2D;
        effect.Parent = obj.transform;
        effect.LocalPosition = new Vector3(0, -5, -10);
        effect.Scale = new Vector3(500, 500, 1);
		linkedEffectObj.RemoveFirst();
    }

	public void ClickRoleBtn(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleRoleInfoBtn();
		
		XUIRoleInformation.CurrPlayer = XLogicWorld.SP.MainPlayer;
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eRoleInformation);
	}
	
	public void ClickFamilyBtn(GameObject _go)
	{	
		XUIRoleInformation.CurrPlayer = XLogicWorld.SP.MainPlayer;
		if(XLogicWorld.SP.MainPlayer.GuildId == 0)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eGuildList);
		}
		else if(XLogicWorld.SP.MainPlayer.GuildId > 0)
		{
			XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eGuildMain);
		}
		else 
		{
			Log.Write(LogLevel.ERROR,"Guild ID ERROR");
		}
	}
	
	public void ClickProduct(GameObject _go)
	{	
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_OpenProduct);
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eProduct);
	}
	
	public void ClickFaBao(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_OpenMount);
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eFaBao);
	}
	
//	public void ClickPractise(GameObject _go)
//	{
//		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.ePractice);
//	}
	
	public void ClickFriendBtn(GameObject _go)
	{	
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_Friend);
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eFriend);
	}

	public void ClickBagBtn(GameObject _go)
	{		
		XNewPlayerGuideManager.SP.handleFunBagBtn();
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eBagWindow);
	}

	public void ClickSkillBtn(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_OpenSkill);
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eSkillOpertation);
	}

	public void ClickQuestBtn(GameObject _go)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eMissionDialog);
	}
	
	public void ClickStrengthenBtn(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleFunStrengthBtn();
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eStrengthenWindow);
	}
	
	public void ClickFormationBtn(GameObject _go)
	{
		XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_OpenBuZhen);
		
		XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eFormation);
	}
	
	public void SetRoleExp(uint curExp,uint maxExp,float p)
	{
		RoleExp.sliderValue = p;
		string rate = "("+(p * 100).ToString("N1") + "%)";
		ExpLabel.text	= curExp.ToString() + "/" + maxExp.ToString() + rate;
	}
	
	private void FindChangeFunc(uint newSortID,List<XFunctionButton.SingleFuncObj> changeList,ref int newPos)
	{
		bool isSet = false;
		int count = 0;
		newPos	= -1;
		
		SortedList<uint,SingleFuncObj>	tempList = new SortedList<uint, SingleFuncObj>();
		foreach(XFunctionButton.SingleFuncObj obj in mFuncGO.Values)
		{
			tempList.Add(obj.SortID,obj);
		}
			
		foreach(XFunctionButton.SingleFuncObj obj in tempList.Values)
		{			
			if(obj.SortID > newSortID)
			{
				if(!isSet)
				{
					isSet	= true;
					newPos	= count;
				}
					
				changeList.Add(obj);
			}
			
			count++;
		}
	}
	
	private XFunctionButton.SingleFuncObj FindFuncBtn(uint SortID)
	{		
		foreach(XFunctionButton.SingleFuncObj obj in mFuncGO.Values)
		{			
			if(obj.SortID ==  SortID)
			{
				return obj;
			}
			
		}
		
		Log.Write(LogLevel.ERROR,"cant find sort id is {0}",SortID);
		return null;
	}
	
	public Vector3 GetBagPos()
	{
		if(mFuncGO.ContainsKey((uint)EFeatureID.EFeatureID_Bag))
		{
			return mFuncGO[(uint)EFeatureID.EFeatureID_Bag].mGO.transform.position;
		}
		
		return Vector3.zero;
	}
}
