using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Packets;
using resource;

class XUTNewPlayerGuide : XUICtrlTemplate<UINewPlayerGuide>
{
	public static SortedList<int, GameObject> m_allShowGuides = new SortedList<int, GameObject>();
	
	private LinkedList<GuidePosInfo> m_all2ShowGuides = new LinkedList<GuidePosInfo>();
	
	public static SortedList<int, UINewPlayerGuide> m_allDeltaUI = new SortedList<int, UINewPlayerGuide>();
	
	private GameObject lastShowPanelObj = null;
	
	public XUTNewPlayerGuide()
	{
		XEventManager.SP.AddHandler(On_PlayerGuide_Start, EEvent.PlayerGuide_Start);
		XEventManager.SP.AddHandler(On_PlayerGuide_Finish, EEvent.PlayerGuide_Finish);
		XEventManager.SP.AddHandler(On_PlayerGuide_Hide, EEvent.PlayerGuide_Hide);
		XEventManager.SP.AddHandler(On_Guide_NcpDisapper, EEvent.PlayerNpc_Disapper);
		XEventManager.SP.AddHandler(On_PlayerGuide_StepStart, EEvent.PlayerGuide_StepStart);
		XEventManager.SP.AddHandler(On_PlayerGuide_StepFinish, EEvent.PlayerGuide_StepFinish);
		XEventManager.SP.AddHandler(On_PlayerGuide_DeltaFinish, EEvent.PlayerMove_DisapperDelta);
		XEventManager.SP.AddHandler(ReposPanelZPos, EEvent.PlayerGuide_ReposPanel);
	}
	
	private void On_PlayerGuide_Start(EEvent evt, params object[] args)
	{
		if ( args.Length < 5 )
			return;
		
		int key = (int)args[0];
		
		if ( key == (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide )
		{
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eForceGuide);
			return;
		}
		
		if ( m_allShowGuides.ContainsKey(key) && null != m_allShowGuides[key] )
		{
			m_allShowGuides[key].SetActive(true);
			return;
		}
		m_all2ShowGuides.AddLast(new GuidePosInfo((Vector3)args[3], (int)args[0], (int)args[1], (int)args[2], (GameObject)args[4]));
		
		showGuide();
	}
	
	private void On_PlayerGuide_StepStart(EEvent evt, params object[] args)
	{
		if ( args.Length < 5 )
			return;
		
		int key = (int)args[0];
		if ( key == (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step1 )
		{
			XForceGuide.OnFirstForceGuideIng = true;
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eForceGuide);
		}
		else
		{
			if ( m_allShowGuides.ContainsKey(key) && null != m_allShowGuides[key] )
			{
				m_allShowGuides[key].SetActive(true);
				return;
			}
		
			m_all2ShowGuides.AddLast(new GuidePosInfo((Vector3)args[3], (int)args[0], (int)args[1], (int)args[2], (GameObject)args[4]));
			
			showGuide();
		
			// 隐藏最终的引导目标
			int key1 = (int)args[1];
			if ( m_allShowGuides.ContainsKey(key1) && null != m_allShowGuides[key1] )
				m_allShowGuides[key1].SetActive(false);
		}
	}
	
	private void On_PlayerGuide_Hide(EEvent evt, params object[] args)
	{
		if ( args.Length < 1 )
			return;
		
		int key = (int)args[0];
		if ( m_allShowGuides.ContainsKey(key) )
		{
			GameObject obj = m_allShowGuides[key];
			if ( null != obj )
			{
				obj.SetActive(false);
			}
		}
	}
	
	private void On_PlayerGuide_Finish(EEvent evt, params object[] args)
	{
		if ( args.Length < 1 )
			return;
		
		int key = (int)args[0];
		
		XNewPlayerGuideManager.SP.UpdateGuideStatus(key);
		if ( m_allShowGuides.ContainsKey(key) )
		{
			switch( (XNewPlayerGuideManager.GuideType)key )
			{
			case XNewPlayerGuideManager.GuideType.Guide_Friend:
			case XNewPlayerGuideManager.GuideType.Guide_OpenProduct:
				ShowFunctionButtonForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_Skill_Select:
				ShowSkillSelectForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_Skill_Use:
				ShowSkillUseForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStart:
				ShowSHStartFightForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_Product_Learn:
			case XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn:
				ShowProductLearnForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);				
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_StartSealHill:
				ShowSealHillBookForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);		
			
				break;
			case XNewPlayerGuideManager.GuideType.Guide_StartZhanYaoLu:
				ShowZhanYaoLuForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_OpenShanHeTu:
			case XNewPlayerGuideManager.GuideType.Guide_Auction:
				ShowOpenFunctionButtonTRFinish(m_allShowGuides[key].transform.parent.gameObject);
						
				break;
			case XNewPlayerGuideManager.GuideType.Guide_XianDaoHuiStart:
				ShowXianDHForceGuideForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);							
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_StartPeiYang:
				ShowMountPeiYangForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);	
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click:
			case XNewPlayerGuideManager.GuideType.Guide_StrengthEquip:
				BagItemUseForceGuideFinish(key, XUIBagWindow.OnShowForceGuideObj);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_Strength_Click:
				ShowStrengthClickForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_LingDan_Use:
				ShowUseLingDangPageForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use:
				ShowHuaLingClickForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_FuYin_Equip:
				ShowFuYinEquipForceGuideForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_BuZhenPetUse:
				ShowFormationForceGuideForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Click:
				ShowMoneyTreeClickForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			default:
				break;
			}

			GameObject obj = m_allShowGuides[key];
			if ( null != obj )
			{
				obj.SetActive(false);
				Object.Destroy(obj);
			}
			m_allShowGuides.Remove(key);
			m_allDeltaUI.Remove(key);;
		}
		else
		{
			if ( ((XNewPlayerGuideManager.GuideType)key == XNewPlayerGuideManager.GuideType.Guide_Product_Learn &&
				 m_allShowGuides.ContainsKey((int)XNewPlayerGuideManager.GuideType.Guide_Product_Select)) )
				ShowProductLearnForceGuideFinish(m_allShowGuides[(int)XNewPlayerGuideManager.GuideType.Guide_Product_Select]);
			else if ( ((XNewPlayerGuideManager.GuideType)key == XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn &&
				 m_allShowGuides.ContainsKey((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeItem))
				)
				ShowProductLearnForceGuideFinish(m_allShowGuides[(int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeItem]);
			else if ( key == (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide )
			{
				XForceGuide.OnFirstForceGuideIng = false;
				XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eForceGuide);
			}
		}
		
		if ( key > 4 && key != 60 && key != 61)
		{
			XNewPlayerGuideManager.SP.RemoveLastShowedData();
		}
	}
	
	private void On_PlayerGuide_StepFinish(EEvent evt, params object[] args)
	{
		if ( args.Length < 1 )
			return;
		
		int key = (int)args[0];
		if ( m_allShowGuides.ContainsKey(key) )
		{
			switch( (XNewPlayerGuideManager.GuideType)key )
			{
			case XNewPlayerGuideManager.GuideType.Guide_OpenMount:
			case XNewPlayerGuideManager.GuideType.Guide_StrengthOpenBag:
			case XNewPlayerGuideManager.GuideType.Guide_OpenStrength:
			case XNewPlayerGuideManager.GuideType.Guide_LingDan_OpenRole:
			case XNewPlayerGuideManager.GuideType.Guide_HuaLing_OpenRole:
			case XNewPlayerGuideManager.GuideType.Guide_FuYin_OpenRole:
			case XNewPlayerGuideManager.GuideType.Guide_OpenSkill:
			case XNewPlayerGuideManager.GuideType.Guide_OpenBuZhen:
				ShowFunctionButtonForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStartStep:
				ShowSHEnterFightForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_Product_Select:
			case XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeItem:
				ShowProductSelectForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_Product_SelMake:
				ShowProductMakeSelForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_OpenSealHill:
			case XNewPlayerGuideManager.GuideType.Guide_OpenZhanYaoLu:
			case XNewPlayerGuideManager.GuideType.Guide_OpenXianDaoHui:
			case XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Open:
				ShowOpenFunctionButtonTRFinish(m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_LingDan_Select:
			case XNewPlayerGuideManager.GuideType.Guide_HuaLing_Select:
			case XNewPlayerGuideManager.GuideType.Guide_FuYin_Select:
				ShowSelectRolePageForceGuideFinish(key, m_allShowGuides[key].transform.parent.gameObject);
				
				break;
			case XNewPlayerGuideManager.GuideType.Guide_HuaLing_Click:
				ShowHuaLingTypeSelectForceGuideFinish(m_allShowGuides[key].transform.parent.gameObject);
			
				break;
			case XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step2:
			case XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step3:
			case XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step4:
				m_allShowGuides[key].transform.parent.transform.localPosition 
					= new Vector3(m_allShowGuides[key].transform.parent.transform.localPosition.x,
					m_allShowGuides[key].transform.parent.transform.localPosition.y,
					0);
					
				XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eForceGuide);
				
				break;
			default:
				break;
			}
			
			GameObject obj = m_allShowGuides[key];
			if ( null != obj )
			{
				obj.SetActive(false);
				Object.Destroy(obj);
			}
			m_allShowGuides.Remove(key);
		}
		else if ( key == (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step1 )
		{
			XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eForceGuide);
		}
		
		if ( key > 4 && key != 60 && key != 61)
		{
			XNewPlayerGuideManager.SP.RemoveLastShowedData();
		}
	}
	
	private void On_PlayerGuide_DeltaFinish(EEvent evt, params object[] args)
	{
		if ( args.Length < 2 )
			return;
		int key = (int)args[1];
		
		if ( !m_allDeltaUI.ContainsKey(key) )
		{
			XNewPlayerGuideManager.SP.handleGuideFinish(key);
			return;
		}
		if ( m_allDeltaUI[key].deltaFinishing )
		{
			return;
		}
		
		m_allDeltaUI[key].DisapperDelta((float)args[0], key);
	}
	
	private void On_Guide_NcpDisapper(EEvent evt, params object[] args)
	{
		if ( args.Length < (int)XNewPlayerGuideManager.GuideType.Guide_DialogWithNpc )
			return;
		
		if ( !m_allShowGuides.ContainsKey((int)XNewPlayerGuideManager.GuideType.Guide_DialogWithNpc) )
			return;
		
		m_allShowGuides.Remove((int)XNewPlayerGuideManager.GuideType.Guide_DialogWithNpc);
	}
	
	private void  showGuide()
	{
		XResourcePanel panel = (XResourcePanel)XResourceManager.GetResource(XResourcePanel.ResTypeName, (int)EUIPanel.eNewPlayerGuide);
		if(panel == null)
		{
			Log.Write(LogLevel.ERROR,"cant find UI ID {0}",(int)EUIPanel.eNewPlayerGuide);
			return ;
		}
		if( panel.IsLoadDone() )
		{
			CreateUI(panel.MainAsset.DownLoad);
		}
		else
		{
			XResourceManager.StartLoadResource(XResourcePanel.ResTypeName, (int)EUIPanel.eNewPlayerGuide);
			panel.ResLoadEvent += new XResourceBase.LoadCompletedDelegate(CreateUI);
		}
	}
	
	private void CreateUI(DownloadItem item)
	{
#if RES_DEBUG
			GameObject go = item.go as GameObject;
#else
			GameObject go = item.ab.mainAsset as GameObject;
#endif
			if ( m_all2ShowGuides.Count <= 0 )
				return;
			
			GuidePosInfo _currentInfo = m_all2ShowGuides.First.Value;
			m_all2ShowGuides.RemoveFirst();
			if ( -1 == _currentInfo.key || 
				( m_allShowGuides.ContainsKey(_currentInfo.key) && null != m_allShowGuides[_currentInfo.key]) 
				)
				return;
		
			Transform parent = _currentInfo.parent.transform;
			if ( _currentInfo.key  == (int)XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click ||
				 _currentInfo.key  == (int)XNewPlayerGuideManager.GuideType.Guide_StrengthEquip )
			{
				parent =_currentInfo.parent.transform.parent.parent;
			}
		
			if ( _currentInfo.key == (int)XNewPlayerGuideManager.GuideType.Guide_FuYin_Equip )
			{
				parent =_currentInfo.parent.transform.parent;
			}
		
	        GameObject PanelObject = XUtil.Instantiate(go, parent, _currentInfo.pos, go.transform.localRotation.eulerAngles);
			do
			{
		        if (null == PanelObject)
		            break;
		        PanelObject.SetActive(false);
		        XUIBaseLogic uiBaseLogic = PanelObject.GetComponent<XUIBaseLogic>();
		        if (null == uiBaseLogic)
				{
					GameObject.Destroy(PanelObject);
		            break;
				}
				
				// 设置显示文本
				XCfgNewPlayerGuide guideInfo = XCfgNewPlayerGuideMgr.SP.GetConfig(_currentInfo.key1, _currentInfo.key2);
				if ( null == guideInfo )
				{
					PanelObject.SetActive(false);
					GameObject.Destroy(PanelObject);
					break;
				}
				// 设置显示图片
				uint showType = 0u;
				if ( ((int)XNewPlayerGuideManager.GuideType.Guide_Mouse == _currentInfo.key) ||  
					((int)XNewPlayerGuideManager.GuideType.Guide_KeyBord == _currentInfo.key) )
				{
					showType = 1u;
					m_allDeltaUI[_currentInfo.key] = (UINewPlayerGuide)uiBaseLogic;
				}
				else if ( (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step2 == _currentInfo.key ||
				(int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step4 == _currentInfo.key )
				{
					showType = 2u;
				}				
				else if ( (int)XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step3 == _currentInfo.key )
				{
					showType = 3u;
				}
			
				uiBaseLogic.ShowData(guideInfo.Description, guideInfo.UIResourcesID, _currentInfo.pos, showType);
				uiBaseLogic.Show();
				m_allShowGuides[_currentInfo.key] = PanelObject;
			
				bool needFrontParent = true;
				lastShowPanelObj = _currentInfo.parent;
				switch( (XNewPlayerGuideManager.GuideType)_currentInfo.key )
				{
				case XNewPlayerGuideManager.GuideType.Guide_OpenSkill:
				case XNewPlayerGuideManager.GuideType.Guide_Friend:
				case XNewPlayerGuideManager.GuideType.Guide_OpenProduct:
				case XNewPlayerGuideManager.GuideType.Guide_OpenMount:
				case XNewPlayerGuideManager.GuideType.Guide_StrengthOpenBag:
				case XNewPlayerGuideManager.GuideType.Guide_OpenStrength:
				case XNewPlayerGuideManager.GuideType.Guide_LingDan_OpenRole:
				case XNewPlayerGuideManager.GuideType.Guide_HuaLing_OpenRole:
				case XNewPlayerGuideManager.GuideType.Guide_FuYin_OpenRole:
				case XNewPlayerGuideManager.GuideType.Guide_OpenBuZhen: 
					ShowFunctionButtonForceGuide(_currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_Skill_Select:
					ShowSkillSelectForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_Skill_Use:
					ShowSkillUseForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Open:
				case XNewPlayerGuideManager.GuideType.Guide_OpenSealHill:
				case XNewPlayerGuideManager.GuideType.Guide_OpenZhanYaoLu:
				case XNewPlayerGuideManager.GuideType.Guide_OpenShanHeTu:
				case XNewPlayerGuideManager.GuideType.Guide_OpenXianDaoHui:
				case XNewPlayerGuideManager.GuideType.Guide_Auction:
					ShowOpenFunctionButtonTR(_currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStartStep:
					ShowSHEnterFightForceGuide(_currentInfo.key, _currentInfo.parent);
					
					break;
				case XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStart:
					ShowSHStartFightForceGuide(_currentInfo.key, _currentInfo.parent);
	
					break;
				case XNewPlayerGuideManager.GuideType.Guide_Product_Select:
				case XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeItem:
					ShowProductSelectForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_Product_Learn:
				case XNewPlayerGuideManager.GuideType.Guide_Product_SelMakeLearn:
					ShowProductLearnForceGuide(_currentInfo.key, _currentInfo.parent);
						
					break;
				case XNewPlayerGuideManager.GuideType.Guide_Product_SelMake:
					ShowProductMakeSelForceGuide(_currentInfo.key, _currentInfo.parent);
					
					break;
				case XNewPlayerGuideManager.GuideType.Guide_StartSealHill:
					ShowSealHillBookForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_StartZhanYaoLu:
					ShowZhanYaoLuForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_XianDaoHuiStart:
					ShowXianDHForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_StartPeiYang:
					ShowMountPeiYangForceGuide(_currentInfo.key, _currentInfo.parent);					
					
					break;
				case XNewPlayerGuideManager.GuideType.Guide_XiangQ_Click:
				case XNewPlayerGuideManager.GuideType.Guide_StrengthEquip:
					BagItemUseForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_Strength_Click:
					ShowStrengthClickForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_LingDan_Select:
				case XNewPlayerGuideManager.GuideType.Guide_HuaLing_Select:
				case XNewPlayerGuideManager.GuideType.Guide_FuYin_Select:
					ShowSelectRolePageForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_LingDan_Use: 
					ShowUseLingDangPageForceGuide(_currentInfo.key, _currentInfo.parent);					
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_HuaLing_Click: 
					ShowHuaLingTypeSelectForceGuide(_currentInfo.key, _currentInfo.parent);					
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use: 
					ShowHuaLingClickForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step2:
				case XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step3:
				case XNewPlayerGuideManager.GuideType.Guide_FirstGuide_Step4:
					XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eForceGuide);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_FuYin_Equip:
					ShowFuYinEquipForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_BuZhenPetUse:
					ShowFormationForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				case XNewPlayerGuideManager.GuideType.Guide_MoneyTree_Click:	
					ShowMoneyTreeClickForceGuide(_currentInfo.key, _currentInfo.parent);
				
					break;
				default:
					lastShowPanelObj = null;
					needFrontParent = false;
					break;
				}
			
				parent.gameObject.SetActive(false);
				parent.gameObject.SetActive(true);
			} while (false);
			_currentInfo.key = -1;
	}
	
	public void ReposPanelZPos(EEvent evt, params object[] args)
	{
		Vector3 v = new Vector3((float)args[0], (float)args[1], (float)args[2]);
		if ( null == lastShowPanelObj )
			return;
		
		lastShowPanelObj.transform.localPosition = new Vector3(lastShowPanelObj.transform.localPosition.x, 
			lastShowPanelObj.transform.localPosition.y, v.z - 50);
		
		//lastShowPanelObj = null;
	}
	
	#region 摇钱树
	private void ShowMoneyTreeClickForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-350);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(600, 250, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowMoneyTreeClickForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use);
	}
	#endregion
	
	#region 布阵
	private void ShowFormationForceGuide(int key, GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowFormationForceGuideForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
	}
	#endregion
	
	#region 装备符印
	private void ShowFuYinEquipForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-350);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(450, 450, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowFuYinEquipForceGuideForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_FuYin_Equip);
	}
	#endregion
	
	#region 化灵培养
	private void ShowHuaLingClickForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-350);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(600, 250, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowHuaLingClickForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use);
	}
	#endregion
	
	#region 化灵方式选择引导
	private void ShowHuaLingTypeSelectForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(600, 700, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowHuaLingTypeSelectForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Click);
	}
	#endregion
	
	#region 灵丹选择页签
	private void ShowSelectRolePageForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(600, 250, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowSelectRolePageForceGuideFinish(int key, GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect(key);
	}
	#endregion
	
	#region 灵丹使用
	private void ShowUseLingDangPageForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(250, 200, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowUseLingDangPageForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Use);
	}
	#endregion
	
	#region 强化
	private void ShowStrengthClickForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		StartEffect(key, 900049, parent, new Vector3(0, 2, -10), new Vector3(700, 300, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowStrengthClickForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Strength_Click);
	}
	#endregion
	
	#region 强化装备武器
	private void BagItemUseForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(450, 450, 1));
		
		parent.transform.parent.gameObject.SetActive(false);
		parent.transform.parent.gameObject.SetActive(true);
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void BagItemUseForceGuideFinish(int key, GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		
		StopEffect(key);
		
		XNewPlayerGuideManager.SP.ShowStrengthGuide();
	}
	#endregion
	
	#region 坐骑培养
	private void ShowMountPeiYangForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(600, 250, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowMountPeiYangForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_StartPeiYang);
	}
	#endregion
	
	#region 斩妖录
	private void ShowXianDHForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		//StartEffect(key, 900040, parent, new Vector3(0, 0, -10), new Vector3(700, 300, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowXianDHForceGuideForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		//StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMake);
	}
	#endregion
	
	#region 斩妖录
	private void ShowZhanYaoLuForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		parent.transform.parent.gameObject.SetActive(false);
		parent.transform.parent.gameObject.SetActive(true);
		//StartEffect(key, 900040, parent, new Vector3(0, 0, -10), new Vector3(700, 300, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowZhanYaoLuForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		//StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMake);
	}
	#endregion
	
	#region 山海经
	private void ShowSealHillBookForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		parent.transform.parent.gameObject.SetActive(false);
		parent.transform.parent.gameObject.SetActive(true);
		//StartEffect(key, 900040, parent, new Vector3(0, 0, -10), new Vector3(700, 300, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowSealHillBookForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		//StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMake);
	}
	#endregion
	
	#region 生产系统强制引导
	private void ShowProductMakeSelForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		parent.transform.parent.gameObject.SetActive(false);
		parent.transform.parent.gameObject.SetActive(true);
		//StartEffect(key, 900040, parent, new Vector3(0, 0, -10), new Vector3(700, 300, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowProductMakeSelForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		//StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Product_SelMake);
	}
	private void ShowProductSelectForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
		//StartEffect(key, 900064, parent, new Vector3(0, 0, -10), new Vector3(700, 300, 1));
	}
	private void ShowProductSelectForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		//StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Product_Select);
	}
	private void ShowProductLearnForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		GameObject obj = parent.transform.Find("Button_Learn").gameObject;
		 StartEffect(key, 900049, obj, new Vector3(0, 2, -10), new Vector3(500, 250, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowProductLearnForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Product_Learn);
	}
	#endregion
	
	#region shanhe 开始挑战
	private void ShowSHStartFightForceGuide(int key, GameObject parent)
	{
		if ( null == parent.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(700, 300, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowSHStartFightForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStart);
	}
	#endregion
	
	#region shanhe 进入挑战
	private void ShowSHEnterFightForceGuide(int key, GameObject parent)
	{
		if ( null == parent.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		
		StartEffect(key, 900049, parent, new Vector3(0, 0, -10), new Vector3(700, 300, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowSHEnterFightForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_ShanHeTuStartStep);
	}
	#endregion
	
	#region function button tr
	private void ShowOpenFunctionButtonTR(GameObject parent)
	{
		if ( null == parent.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		parent.transform.parent.gameObject.SetActive(false);
		parent.transform.parent.gameObject.SetActive(true);
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowOpenFunctionButtonTRFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
	}
	#endregion
	
	#region function button
	private void ShowFunctionButtonForceGuide(GameObject parent)
	{
		if ( null == parent.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowFunctionButtonForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
	}
	#endregion
	
	#region learn skill
	private void ShowSkillSelectForceGuide(int key, GameObject parent)
	{
		if ( null == parent.GetComponent<UIPanel>() )
			parent.AddComponent<UIPanel>();
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 
			-500);
		
		StartEffect(key, 900039, parent, new Vector3(0, -5, -10), new Vector3(500, 500, 1));
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowSkillSelectForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.GetComponent<UIPanel>());
		}
		parent.transform.localPosition = new Vector3(parent.transform.localPosition.x, 
			parent.transform.localPosition.y, 0);
		StopEffect((int)XNewPlayerGuideManager.GuideType.Guide_Skill_Select);
	}
	#endregion
	
	#region use skill
	private void ShowSkillUseForceGuide(int key, GameObject parent)
	{
		if ( null == parent.transform.parent.gameObject.GetComponent<UIPanel>() )
			parent.transform.parent.gameObject.AddComponent<UIPanel>();
		parent.transform.parent.gameObject.transform.localPosition = new Vector3(parent.transform.parent.gameObject.transform.localPosition.x, 
			parent.transform.parent.gameObject.transform.localPosition.y, 
			-500);
		
		parent.transform.parent.transform.Find("Sprite_SkillIcon").gameObject.SetActive(false);
		parent.transform.parent.transform.Find("Sprite_SkillIcon").gameObject.SetActive(true);
		
		XEventManager.SP.SendEvent(EEvent.UI_Show, (int)EUIPanel.eForceGuide);
	}
	private void ShowSkillUseForceGuideFinish(GameObject parent)
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
		if ( null != parent.transform.parent.gameObject.GetComponent<UIPanel>() )
		{
			NGUITools.Destroy(parent.transform.parent.gameObject.GetComponent<UIPanel>());
		}
		parent.transform.parent.gameObject.transform.localPosition = new Vector3(parent.transform.parent.gameObject.transform.localPosition.x, 
			parent.transform.parent.gameObject.transform.localPosition.y, 
			0);
		
		XEventManager.SP.SendEvent(EEvent.UI_Hide, (int)EUIPanel.eForceGuide);
	}
	#endregion
	
	#region effect
	private SortedList<int, XU3dEffect>	m_effect = new SortedList<int, XU3dEffect>();
	private SortedList<int, GameObject>	mFuncGO = new SortedList<int, GameObject>();
	private GameObject lastGameObj = null;
	private Vector3 lastPos;
	private Vector3 lastSize;
	public void StartEffect(int key, uint effectid, GameObject go, Vector3 pos, Vector3 size)
	{
		lastPos = new Vector3(pos.x, pos.y, pos.z);
		lastSize = new Vector3(size.x, size.y, size.z);
		lastGameObj = go;
		XU3dEffect effect = new XU3dEffect(effectid, EffectLoadedHandle);
		m_effect[key] = effect;
		mFuncGO[key] = lastGameObj;
	}
	private void EffectLoadedHandle(XU3dEffect effect)
    {
		if ( null == lastGameObj )
			return;
		
        effect.Layer = GlobalU3dDefine.Layer_UI_2D;
        effect.Parent = lastGameObj.transform;
        effect.LocalPosition =  new Vector3(lastPos.x, lastPos.y, lastPos.z); // new Vector3(0, -5, -10);
        effect.Scale = new Vector3(lastSize.x, lastSize.y, lastSize.z); // new Vector3(500, 500, 1);
		lastGameObj = null;
    }
	public void StopEffect(int key)
	{
		if ( !m_effect.ContainsKey(key) )
			return;
		
		XU3dEffect effect = m_effect[key];
		effect.Destroy();
		m_effect.Remove(key);
		mFuncGO.Remove(key);
	}
	#endregion
}

