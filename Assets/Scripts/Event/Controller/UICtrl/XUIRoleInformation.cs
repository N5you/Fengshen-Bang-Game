using UnityEngine;
using System;
using System.Collections;
using XGame.Client.Packets;
using System.Collections.Generic;

class XUIRoleInformation : XUICtrlTemplate<XRoleInformation>
{
	private static int FuYin_Need_Level = 16;
	private static int LingDan_Need_Level = 19;
	public static uint MAX_DAY_PEIYANG_COUNT = 5;
	private static uint MAX_CLASS_LEVEL = 6;
	private static string[] VIPLevelSpriteName = new string[6]{
		"11000190",
		"11000191",
		"11000192",
		"11000193",
		"11000194",
		"11000195",
	};
	
	private static string[] PetLoyalType = new string[5]{
		"",
		"11000480",
		"11000481",
		"11000482",
		"11000483",
	};
	
	private static string[] PetLoyalDes = new string[5]{
		"",
		"反抗",
		"顺从",
		"信赖",
		"忠诚",
	};
	
	private static string[] PetAptitudeType = new string[6]{
		"11000484",
		"11000485",
		"11000486",
		"11000487",
		"11000488",
		"11000489",
	};
	
	private static string[] PetAptitudeDes = new string[7]{
		"",
		"凡品",
		"良品",
		"上品",
		"极品",
		"神品",
		"圣品",
	};	
	private LogicRoleBtn 	LeftBtn 	= new LogicRoleBtn();
	private LogicRoleInfo 	RoleInfo 	= new LogicRoleInfo();
	private LogicAttrInfo	AttrInfo 	= new LogicAttrInfo();
	private LogicRRoleBtn	RightBtn	= new LogicRRoleBtn();
	private ItemSelContainer Container	= new ItemSelContainer();
	
	private LogicHuaLingInfo HuaLingInfo = new LogicHuaLingInfo();
	private LogicFuYinInfo FuYinInfo = new LogicFuYinInfo();
	private LogicLingDanInfo LingDanInfo = new LogicLingDanInfo();
	
	public static XCharacter CurRoInfoSelChar = null;
	
	public static XMainPlayer    currPlayer		  = null;
	
	public static bool bShowCurrentUser = true;
	
	public static XMainPlayer CurrPlayer
	{
		get{return currPlayer;}
		set
		{
			currPlayer = value;
			if( XLogicWorld.SP.MainPlayer.ID != value.ID )
			{
				bShowCurrentUser = false;
			}
			else
			{
				bShowCurrentUser = true;
			}
		}
	}
	
	public XCharacter GetCurSelObject()
	{
		return LeftBtn.GetCurSelObject();
	}
	
	public static int CurSelIndex = -1;
	//获得当前选中的对象的索引(与显示位置无关)	
	public static int GetCurSelObjIndex()
	{
		if(CurRoInfoSelChar == null)
			return 0;
		XCharacter	CurSelObj	= CurRoInfoSelChar;
		int ObjectIndex = 0;
		XMainPlayer player = CurSelObj as XMainPlayer;
		if(player != null)
			ObjectIndex = 0;
		else
		{
			XPet pet = CurSelObj as XPet;
			ObjectIndex	= (int)pet.Index;		
		}
		return ObjectIndex;
	}
	
	//计算战斗力
	public static int CalcBattleValue(XCharacter ch)
	{
		int attackValue = 0;
		int defendValue = 0;
		
		int attack = ch.DynGet(EShareAttr.esa_AttackValue);
		float CRIT = ch.DynGet(EShareAttr.esa_CritRate) / 10000.0f; 
		float CRIT_Level = ch.DynGet(EShareAttr.esa_CritLevel) / 10000.0f;
		float Hit_Rate = ch.DynGet(EShareAttr.esa_HitRate)/ 10000.0f; ;
		float Accuracy_Rate = ch.DynGet(EShareAttr.esa_AccuracyRate)/ 10000.0f;
		float Lucky_Rate = ch.DynGet(EShareAttr.esa_LuckRate)/ 10000.0f;
		float morale_Rate = ch.DynGet(EShareAttr.esa_MoraleRate)/ 10000.0f;
		
		attackValue = (int)((attack * (1 + CRIT *(CRIT_Level - 1)) * Hit_Rate * (1+ Accuracy_Rate)* (1+ Lucky_Rate/2)*(1+morale_Rate)) * 5);
		
		int MAX_HP = ch.DynGet(EShareAttr.esa_MaxHp);
		int phy_defend = ch.DynGet(EShareAttr.esa_Defence);
		int shufa_defend = ch.DynGet(EShareAttr.esa_ShuFaDefence);
		int level = ch.DynGet(EShareAttr.esa_Level);
		float dodge_rate = ch.DynGet(EShareAttr.esa_DodgeRate)/ 10000.0f;;
		float fend_rate = ch.DynGet(EShareAttr.esa_HoldRate) / 10000.0f;;
		
		int temp1 = phy_defend + shufa_defend;
		float temp2 = Mathf.Pow(level + 9,2);
		float temp3 = temp1 / (temp2 * 5 + temp1);
		defendValue	= (int)(MAX_HP / ((1 - temp3) * (1 - dodge_rate - fend_rate)));
		
		return attackValue + defendValue;
	}
	
	//角色宠物按钮
	public class LogicRoleBtn
	{
		public UISpriteGroup	spriteGroup;
		public LogicRoleInfo	RoleInfo;
		public LogicRRoleBtn	RightBtn;
		private int[]			mPetIndex = new int[9];
		
		public XCharacter	GetCurSelObject()
		{
			if(CurSelIndex <= 0)
				return XLogicWorld.SP.MainPlayer;
			
			return XLogicWorld.SP.PetManager.AllPet[mPetIndex[CurSelIndex]];
		}
		
		public void Show()
		{
			if(spriteGroup	== null)
				return ;
			
			int petCount = XLogicWorld.SP.PetManager.GetPetCount();
			spriteGroup.SetVisible(1 + petCount);
			string tempStr = XLogicWorld.SP.MainPlayer.Name;
			tempStr += "\n";
			tempStr += XLogicWorld.SP.MainPlayer.Level.ToString() + "级";
			spriteGroup.SetText(0,tempStr);
			
			int count = 1;
			for(uint i = XPetManager.PET_INDEX_BEGIN; i < XPetManager.PET_INDEX_END && count <= 8 ; i++)
			{
				XPet pet = XLogicWorld.SP.PetManager.AllPet[(int)i];
				if(pet == null)
				{					
					continue;
				}
				mPetIndex[count]	= (int)pet.Index;
				string TempPetName = pet.Name;
				TempPetName	+= "\n";
				TempPetName	+= pet.Level.ToString() + "级";
				spriteGroup.SetText((int)count,TempPetName); 
				count++;
			}
			
			spriteGroup.ReSet();
			SetObjectData(0);
			
		}
		
		public void SetObjectData(int index)
		{	
			if(CurSelIndex == index)
			{
				RoleInfo.ReSetRotation();
				return ;
			}
			
			CurSelIndex	= index;
			if(index == 0)
			{				
				RoleInfo.SetPlayerData();
			}				
			else
			{	
				
				RoleInfo.SetPetData(mPetIndex[index]);
			}
			
			RightBtn.Show();
		}
		
		public void Reflash(EEvent evt, params object[] args)
		{
			CurSelIndex = -1;
			Show();
		}
		
		public void ReflashRight(EEvent evt, params object[] args)
		{
			if(RightBtn == null)
				return ;
			RightBtn.Show();
		}
		
		public void SetName(EEvent evt, params object[] args)
		{
			uint index  = (uint)args[0];			
			
			XPet pet = XLogicWorld.SP.PetManager.AllPet[(int)index];
			if(pet == null)
				return ;
			
			spriteGroup.SetText((int)index,pet.Name + "\n" + pet.Level + "级");
		}
		
		public void ReflashBattleValue(EEvent evt, params object[] args)
		{
			XCharacter ch = GetCurSelObject();
			if(ch != null )
				RoleInfo.mUI.BattleValue.text	= Convert.ToString(XUIRoleInformation.CalcBattleValue(ch));
		}
	}
	
	public class ItemSelContainer
	{
		public XBagActionIcon[] mActionList = new XBagActionIcon[XRoleInformation.MAX_ITEM_SEL_NUM];
		public ArrayList		mAllTypeItemList = new ArrayList();
		private uint 			CurSelPage 	= 0;
		private uint			PageNum		= 1;
		private bool			isInit = false;
		public  uint			CurSelFunc = 0;
		public UILabel			LabelPage;
		public UIImageButton	LeftBtn;
		public UIImageButton	RightBtn;
		
		public void Init(XActionIcon[] UIList)
		{
			if(isInit)
				return ;
			isInit	= true;
			for(int i = 0; i < XRoleInformation.MAX_ITEM_SEL_NUM; i++)
			{
				mActionList[i] = new XBagActionIcon();
				mActionList[i].SetUIIcon(UIList[i]);
				mActionList[i].IsCanDBClick	= true;
				mActionList[i].IsCanToolTip	= true;
				mActionList[i].IsCanPopMenu	= true;
				mActionList[i].IsCanDrag	= false;
				mActionList[i].IsCanDrop	= false;
			}
		}
		
		public void UpdateContainerHandle(EEvent evt, params object[] args)
		{
			Show(CurSelFunc);
		}
		
		public void LeftBtnHandle(EEvent evt, params object[] args)
		{
			if(CurSelPage == 0)
				return ;
			
			CurSelPage -= 1;
			
			Show(CurSelFunc);
		}
		
		public void RightBtnHandle(EEvent evt, params object[] args)
		{
			if(CurSelPage >= PageNum - 1)
				return ;
			
			CurSelPage += 1;
			
			Show(CurSelFunc);
		}
		
		//其他类型，宝石，符印，灵丹，靠子类来判断
		private void GetTypeItem(ushort type)
		{
			mAllTypeItemList.Clear();
			//根据类型找到背包中的所有相同类型的物品
			int beginIndex 	= XItemManager.GetBeginIndex(EItemBoxType.Bag);
			int endIndex	= XItemManager.GetEndIndex(EItemBoxType.Bag);
			int i = 0;
			for(i = beginIndex; i <= endIndex; i++)
			{
				XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)i);
				if(LogicItem == null || LogicItem.IsEmpty())
					continue;
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
				if(cfgItem == null)
					continue;
				
				if(cfgItem.ItemSubType == type)
					mAllTypeItemList.Add(LogicItem.ItemIndex);
			}
		}
		
		//装备类型，靠TypeTag来判断
		private void GetEquipItem()
		{
			mAllTypeItemList.Clear();
			//根据类型找到背包中的所有相同类型的物品
			int beginIndex 	= XItemManager.GetBeginIndex(EItemBoxType.Bag);
			int endIndex	= XItemManager.GetEndIndex(EItemBoxType.Bag);
			int i = 0;
			for(i = beginIndex; i <= endIndex; i++)
			{
				XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)i);
				if(LogicItem == null || LogicItem.IsEmpty())
					continue;
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
				if(cfgItem == null)
					continue;
				
				if(cfgItem.TypeTag == 1)
					mAllTypeItemList.Add(LogicItem.ItemIndex);
			}
		}
		
		private void ShowItem()
		{
			//清空所有按钮数据
			for(int i = 0;i < XRoleInformation.MAX_ITEM_SEL_NUM;i++)
			{
				mActionList[i].ResetUIAndLogic();
			}
			
			PageNum = (uint)(mAllTypeItemList.Count / (XRoleInformation.MAX_ITEM_SEL_NUM + 1) + 1);
			LabelPage.text = (CurSelPage + 1).ToString() + "/" + PageNum.ToString();
			
			if(CurSelPage == 0)
			{
				LeftBtn.isEnabled	= false;
				LeftBtn.UpdateImage();
			}
			else
			{
				LeftBtn.isEnabled	= true;
				LeftBtn.UpdateImage();
			}
			
			if(CurSelPage < PageNum - 1)
			{
				RightBtn.isEnabled	= true;
				RightBtn.UpdateImage();
			}
			else
			{
				RightBtn.isEnabled	= false;
				RightBtn.UpdateImage();
			}			
			
			int Base = (int)CurSelPage * (int)XRoleInformation.MAX_ITEM_SEL_NUM;
			if(Base >= mAllTypeItemList.Count)
				return ;
			
			for(int i = 0;i < XRoleInformation.MAX_ITEM_SEL_NUM && (Base + i) < mAllTypeItemList.Count; i++)
			{
				int tempIndex = Base + i;
				ushort itemIndex = (ushort)mAllTypeItemList[tempIndex];
				mActionList[i].SetLogicData(ActionIcon_Type.ActionIcon_Bag,(uint)itemIndex);
				
				XItem LogicItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)itemIndex);
				if(LogicItem == null || LogicItem.IsEmpty())
					continue;
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(LogicItem.DataID);
				if(cfgItem == null)
					continue;
				
				mActionList[i].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,LogicItem.Color,LogicItem.ItemCount,true,LogicItem.mEquipAttr.mStrengthenLevel);
			}
		}
		
		//index 标示第几个标签
		public void Show(uint index)
		{
			if(index == 0 || index == 2)
				GetEquipItem();
			else if(index == 1)
				GetTypeItem((ushort)EITEM_GOODS_SUB_TYPE.EITEM_GOODS_SUB_TYPE_LD);
			else if(index == 3)
				GetTypeItem((ushort)EITEM_FUYIN_SUB_TYPE.EITEM_FUYIN_SUB_TYPE_FUYIN);
			
			CurSelFunc	= index;
			
			
			ShowItem();
		}
	}
	
	//功能按钮
	public class LogicRRoleBtn
	{
		public UISpriteGroup	spriteGroup;
		public XRoleInformation	LogicUI;
		public delegate void BtnClick();
		public BtnClick[] EventBtnClick	= new BtnClick[XRoleInformation.MAX_FUNC_SEL_NUM];
		public ItemSelContainer	Container;
		
		
		public void Show()
		{
			if(spriteGroup	== null)
				return ;
			
			//XCharacter CurSel = XUIRoleInformation.CurRoInfoSelChar;
			XCharacter CurSel = XLogicWorld.SP.MainPlayer;
			
			if(CurSel != null)
			{
				spriteGroup.SetVisible(1);
				if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_lingdan))
					spriteGroup.SetVisible(2);
				if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_hualing))
					spriteGroup.SetVisible(3);
				if(FeatureDataUnLockMgr.SP.IsUnLock((int)EFeatureID.EFeatureID_fuyin))
					spriteGroup.SetVisible(4);
				
			}
			
			spriteGroup.ReSet();
			SetFuncData(0);
		}
		
		public void SetFuncData(int index)
		{
			LogicUI.ShowFuncSel(index);
			if(EventBtnClick[index] != null)
				EventBtnClick[index]();
			
			//显示不同类型的物品
			Container.Show((uint)index);
		}
		
		public void OnShowLingDan(EEvent evt, params object[] args)
		{			
			if(spriteGroup	== null)
				return ;
			
			XCharacter CurSel = XUIRoleInformation.CurRoInfoSelChar;
			
			if(CurSel != null)
			{
				if(CurSel.Level < LingDan_Need_Level)
					spriteGroup.SetVisible(3);
				
				if(CurSel.Level < FuYin_Need_Level)
					spriteGroup.SetVisible(2);
				
				if(CurSel.Level >= LingDan_Need_Level)
					spriteGroup.SetVisible(4);
			}
			
			spriteGroup.Select(1);
			SetFuncData(1);
		}
	}
	
	//角色面板数据
	public class LogicRoleInfo
	{
		public XRoleInformation.RoleInfoLeft mUI = null;
		public XRoleActionIcon[] RoleActionIcon = new XRoleActionIcon[XRoleInformation.MAX_CHAR_ICON_NUM];
		bool IsInitEquip = false;
		int index = 0;
		
		//private XU3dModel m_mainModel = null;
		//private XU3dModel m_weaponModel = null;	
		//private XUIModel mUIModel = new XUIModel();
		
		//private XObjectModel mObjectModel = new XObjectModel();
		private XObjectModel m_objectModel = new XObjectModel();
		private SingleModelRTT m_rtt = null;		
		
		public static int[] EquipPosToUIPos = new int[7]
		{
			1,
			4,
			0,
			2,
			3,
			5,
			6,
		};
		
		public static int[] UIPosToEquipPos = new int[7]
		{
			2,
			0,
			3,
			4,
			1,
			5,
			6,
		};
		
		public void SetPlayerData()
		{
			XMainPlayer player = CurrPlayer;
			
			mUI.ShengWangDes.gameObject.SetActive(true);
			mUI.ShengWangValue.gameObject.SetActive(true);
			mUI.ShengWangValue.text	= player.ShengWangValue.ToString();
			
			mUI.ClothesGO.gameObject.SetActive(true);
			UpdateFashionSelStatus(mUI);
			
			//职业
			int index = player.DynGet(EShareAttr.esa_Class);
			mUI.CareerName.text = XGameColorDefine.Job_Color[index - 1];
			mUI.CareerName.text += GlobalU3dDefine.JobName[index];
			//境界
			mUI.StateName.text = XGameColorDefine.JJ_Color[player.ClassLevel - 1];
			mUI.StateName.text += XCfgClassLvUpMgr.SP.GetConfig(player.ClassLevel).Name;
			XCfgClassLvUp CfgCL = XCfgClassLvUpMgr.SP.GetConfig(player.ClassLevel);
			if(CfgCL != null)
				mUI.StateName.CommonTips	= string.Format(XStringManager.SP.GetString(358),CfgCL.GrowthRequire.ToString("N1"));
			//无双
			mUI.TeZHiDes.text	= "无双";
			XCfgPlayerBase cfgPlayer = XCfgPlayerBaseMgr.SP.GetConfig((byte)index);
			if(cfgPlayer == null)
				return ;
			XCfgSkillBase CfgSuperSkill = XCfgSkillBaseMgr.SP.GetConfig(cfgPlayer.SuperSkill);
			if(CfgSuperSkill != null)
			{
				mUI.TeZhiName.text	= CfgSuperSkill.Name;
				//SkillManager.SP.get
				mUI.TeZhiName.CommonTips	= "[color=00ff00]" + CfgSuperSkill.Name + "\n";
				
				byte level = SkillManager.SP.GetActiveSkill(cfgPlayer.SuperSkill);
				if( level == 0)
					level = 1;
				
				
				XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(cfgPlayer.SuperSkill);
				mUI.TeZhiName.CommonTips += "[color=f89a4b]" + skillDef.Levels[level].TextEffect;
			}
			
			//灵力(成长)
			mUI.GrowValue.CommonTips	= "成长上限为" +  XCfgClassLvUpMgr.SP.GetConfig(player.ClassLevel).MaxGrowth.ToString("N1");

			mUI.GrowValue.text	= player.Grow.ToString("F1");
			//绝招
			XSkillDefine skillDefine =  SkillManager.SP.GetSkillDefine(SkillManager.SP.m_uSkillEquip);
			mUI.JueZhaoName.text	= "";
			mUI.JueZhaoName.CommonTips	= "";
			if(skillDefine != null)
			{
				mUI.JueZhaoName.text = skillDefine.Name;
				mUI.JueZhaoName.CommonTips	= "[color=00ff00]" + skillDefine.Name + "\n";
				
				byte level = SkillManager.SP.GetActiveSkill(SkillManager.SP.m_uSkillEquip);
				if( level == 0)
					level = 1;
				
				XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(SkillManager.SP.m_uSkillEquip);
				mUI.JueZhaoName.CommonTips += "[color=f89a4b]" + skillDef.Levels[level].TextEffect;				
			}
			else
				mUI.JueZhaoName.text	= "无";
			
			//战力
			mUI.BattleValue.text	= Convert.ToString(CalcBattleValue(player));
			
			XCfgPlayerLvUp levelUp = XCfgPlayerLvUpMgr.SP.GetConfig((uint)player.Level);
			if(levelUp == null)
				mUI.ExpValue.sliderValue	= 0.001f;
			else			
				mUI.ExpValue.sliderValue	= player.Exp * 1.0f/ levelUp.MaxExp;
			
			mUI.ExpRate.text	= (mUI.ExpValue.sliderValue * 100).ToString("N1") + "%";
			
			//mUI.AptitudeMark.gameObject.SetActive(false);
			//mUI.LabelL.gameObject.SetActive(false);
			//mUI.LabelR.gameObject.SetActive(false);
			
			UILabel text = mUI.FuncBtn[0].GetComponentInChildren<UILabel>();
			if(text != null)
				text.text	= "背包";
			UILabel text1 = mUI.FuncBtn[1].GetComponentInChildren<UILabel>();
			if(text1 != null)
				text1.text	= "星图";
			
			mUI.NickName.gameObject.SetActive(true);
			mUI.ZizhiName.gameObject.SetActive(false);
			
			CurRoInfoSelChar	= player;
			UpdateUIModel();
			showAllEquipItem();
			
			if (XNickNameManager.SP.GetNickNameList ().Count != 0) {
				mUI.NickName.items.Clear ();
				mUI.NickName.items.Add ("无称号");
				foreach (KeyValuePair<uint, XNickNameInfo> s in XNickNameManager.SP.GetNickNameList()) {
					
					mUI.NickName.items.Add (s.Value.name);
					if(s.Value.name == XNickNameManager.SP.GetCurNickNameStr())
						mUI.NickName.selection = s.Value.name;
				}
			}else
			{
				mUI.NickName.items.Clear ();
				mUI.NickName.items.Add ("无称号");
			}
			
		}
		
		public void SetPetData(int index)
		{			
			XPet pet = XLogicWorld.SP.PetManager.AllPet[index];
			if(pet == null)
				return ;
			XCfgPetBase cfgPet = XCfgPetBaseMgr.SP.GetConfig(pet.PetID);
			if(cfgPet == null)
			{
				Log.Write(LogLevel.ERROR,"PET CONFIG ID {0} not find",pet.PetID);
				return ;
			}
			
			mUI.ShengWangDes.gameObject.SetActive(false);
			mUI.ShengWangValue.gameObject.SetActive(false);	
			
			mUI.ClothesGO.gameObject.SetActive(false);
			mUI.FashionSel.gameObject.SetActive(false);
			
			//职业
			mUI.CareerName.text = XGameColorDefine.Job_Color[cfgPet.ClassType2 - 1];
			mUI.CareerName.text = GlobalU3dDefine.JobName[cfgPet.ClassType2];
			//境界
			XCfgClassLvUp CfgLvUp = XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel);
			mUI.StateName.text = XGameColorDefine.JJ_Color[pet.ClassLevel - 1];
			mUI.StateName.text += XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel).Name;
			XCfgClassLvUp CfgCL = XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel);
			if(CfgCL != null)
				mUI.StateName.CommonTips	= string.Format(XStringManager.SP.GetString(358),CfgCL.GrowthRequire.ToString("N1"));
			//特质
			mUI.TeZHiDes.text	= "特质:";
			mUI.TeZhiName.text	= cfgPet.AptitudeName;
			mUI.TeZhiName.CommonTips	= cfgPet.AptitudeName + "\n" + cfgPet.AptitudeDes;
			
			//灵力（成长）
			mUI.GrowValue.CommonTips	= "成长上限为" +  CfgLvUp.MaxGrowth.ToString("N1");

			mUI.GrowValue.text	= (pet.DynGet(EShareAttr.esa_Grow)/ Define.CONFIG_RATE_BASE).ToString("N1");
			
			XSkillDefine skillDefine =  SkillManager.SP.GetSkillDefine(cfgPet.BattleSkill[2]);
			mUI.JueZhaoName.text	= "";
			mUI.JueZhaoName.CommonTips	= "";
			if(skillDefine != null)
			{
				mUI.JueZhaoName.text = skillDefine.Name;
				
				mUI.JueZhaoName.CommonTips	= "[color=00ff00]" + skillDefine.Name + "\n";
				
				//宠物的境界值 == 技能的等级
				byte level = (byte)pet.ClassLevel;
				
				XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(cfgPet.BattleSkill[2]);
				mUI.JueZhaoName.CommonTips += "[color=f89a4b]" + skillDef.Levels[level].TextEffect;
			}
			else
				mUI.JueZhaoName.text	= "无";
				
				
			mUI.BattleValue.text	= Convert.ToString(CalcBattleValue(pet));
			
			
			XCfgPlayerLvUp levelUp = XCfgPlayerLvUpMgr.SP.GetConfig((uint)pet.Level);
			if(levelUp == null)
				mUI.ExpValue.sliderValue	= 0.001f;
			else			
				mUI.ExpValue.sliderValue	= pet.Exp * 1.0f/ levelUp.MaxExp;
			
			mUI.ExpRate.text	= (mUI.ExpValue.sliderValue * 100).ToString("N1") + "%";
		
			//mUI.AptitudeMark.spriteName	= PetAptitudeType[(int)pet.Aptitude];
			//mUI.AptitudeMark.gameObject.SetActive(true);
			//mUI.LoyalMark.spriteName	= PetLoyalType[(int)XPet.GetLoyalType(pet)];
			//mUI.LoyalMark.gameObject.SetActive(true);
			//mUI.LabelL.text	= PetAptitudeDes[(int)pet.Aptitude];
			//mUI.LabelL.gameObject.SetActive(true);
			//mUI.LabelR.text	= PetLoyalDes[(int)XPet.GetLoyalType(pet)];
			//mUI.LabelR.gameObject.SetActive(true);
			
			UILabel text = mUI.FuncBtn[0].GetComponentInChildren<UILabel>();
			if(text != null)
				text.text	= "放生";
			UILabel text1 = mUI.FuncBtn[1].GetComponentInChildren<UILabel>();
			if(text1 != null)
				text1.text	= "改名";
			
			mUI.NickName.gameObject.SetActive(false);
			mUI.ZizhiName.gameObject.SetActive(true);
			mUI.ZizhiName.text	= XGameColorDefine.Quality_Color[(int)pet.Aptitude] + "资质:" + PetAptitudeDes[(int)pet.Aptitude];
			
			CurRoInfoSelChar	= pet;
			UpdateUIModel();
			showAllEquipItem();
		}
		
		public void OnTurnLeft(EEvent evt, params object[] args)
		{
			//mUIModel.SetRotationL();
			if(m_rtt != null)
				m_rtt.RotLeft();
		}
		
		public void OnChangeFashionState(EEvent evt, params object[] args)
		{
			uint state = 0u;
			if ( 0 == XLogicWorld.SP.MainPlayer.ShowFashion )
			{
				state = 1u;
			}
			
			XLogicWorld.SP.MainPlayer.ShowFashion = state;
			CS_UInt.Builder msg = CS_UInt.CreateBuilder();
			msg.SetData(state);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_FashionStateChange, msg.Build());
			UpdateFashionSelStatus(mUI);
		}
		
		public void OnTurnRight(EEvent evt, params object[] args)
		{
			//mUIModel.SetRotationR();
			if(m_rtt != null)
				m_rtt.RotRight();
		}
		
		public void OnUIModel(EEvent evt, params object[] args)
		{
			if(mUI == null)
				return ;
			UpdateUIModel();
		}
		
		public void ReSetRotation()
		{
//			XCharacter CurSelObj	= CurRoInfoSelChar;
//			if(CurSelObj == null || m_mainModel == null)
//				return ;
//			
//			if(mUIModel.Init(m_mainModel))
//			{
//				mUIModel.SetPosition(mUI.mUIModelBK,new Vector3(151,-18,-40));
//				mUIModel.SetScale(new Vector3(100,100,100));
//			}
			
		}
		
		private void UpdateUIModel()
		{
//			if(m_mainModel != null)
//			{
//				m_mainModel.Destroy();
//				m_mainModel	= null;
//			}
//			
//			if(m_weaponModel != null)
//			{
//				m_weaponModel.Destroy();
//				m_weaponModel = null;
//			}
//			
			XCharacter CurSelObj	= CurRoInfoSelChar;
			if(CurSelObj == null)
				return ;
			
			m_objectModel.HandleEvent(EModelEvent.evtModelId, CurSelObj.ModelId);
			m_objectModel.HandleEvent(EModelEvent.evtWeaponId,CurSelObj.WeaponModelId);
			if(m_rtt != null)
				m_rtt.DestoryNotModel();
			m_rtt = XModelRTTMgr.SP.AddObjectRTT(m_objectModel.mainModel, mUI.mUITexture, 1.5f, new Vector3(0f, -1.5f, 2f),Vector3.zero,new Vector3(1, 1, 1), true);
		
			//m_objectModel.mountLoaded += loadMountFinish;
			
			//显示模型
//			if(null == m_mainModel)
//			{
//				m_mainModel = new XU3dModel("roleInfo_mainPlayer",CurSelObj.ModelId);
//			}
//			else
//			{
//				m_mainModel.RefreshDynObject(CurSelObj.ModelId);
//			}
//			
//			XMainPlayer mainPlayer = CurSelObj as XMainPlayer;
//			if(mainPlayer != null)
//			{
//				if(null == m_weaponModel)
//				{
//					m_weaponModel = new XU3dModel("roleInfo_weapon",XLogicWorld.SP.MainPlayer.WeaponModelId,OnModelWeaponLoadDone);
//					
//				}
//				else
//				{
//					m_weaponModel.RefreshDynObject(XLogicWorld.SP.MainPlayer.WeaponModelId);
//				}
//			}
//			
//			if(mUIModel.Init(m_mainModel))
//			{
//				mUIModel.SetPosition(mUI.mUIModelBK,new Vector3(151,-18,-40));
//				mUIModel.SetScale(new Vector3(100,100,100));
//			}
			
		}
		
		public void OnModelWeaponLoadDone(XU3dModel self)
		{
			//m_weaponModel	= self;
			//m_weaponModel.Layer = GlobalU3dDefine.Layer_UI_2D;
			//m_mainModel.AttachU3dModel(ESkeleton.eWeapon, m_weaponModel, ESkeleton.eMainObject);
			//m_weaponModel.Scale	= new Vector3(1,1,1);
			
//			if(mUIModel.Init(m_mainModel))
//			{
//				mUIModel.SetPosition(mUI.mUIModelBK,new Vector3(151,-18,-40));
//				mUIModel.SetScale(new Vector3(100,100,100));
//			}
		}
		
		private void showAllEquipItem()
		{
			//显示装备中的物品
			int ObjectIndex = GetCurSelObjIndex();
			for(short i = 0;i < XRoleInformation.MAX_CHAR_EQUIP_NUM; i++)
			{
				int UIPos = EquipPosToUIPos[i];
				RoleActionIcon[UIPos].Reset();
				
				EItemBoxType type;
				int ItemIndex;
				if(ObjectIndex == 0)
				{
					type = EItemBoxType.Equip;
					ItemIndex = (int)i;
				}
				else
				{
					type = EItemBoxType.PetEquip;
					ItemIndex = (int)i + (ObjectIndex - 1) * (int)EBAG_DATA.ROLE_MAX_EQUIP_NUM;
				}
				
				RoleActionIcon[UIPos].SetLogicData((ActionIcon_Type)type,ItemIndex,ObjectIndex);
				RoleActionIcon[UIPos].IsCanDBClick	= true;
				
				XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)type,(short)ItemIndex);
				if(tempItem == null || tempItem.IsEmpty())
					continue;
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(tempItem.DataID);
				if(cfgItem == null)
					continue;
				
				RoleActionIcon[UIPos].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,tempItem.Color,tempItem.ItemCount);
				
				if(tempItem.mEquipAttr.mStrengthenLevel > 1)
				{
					RoleActionIcon[UIPos].EnableEffect(true);
				}
			}
		}
		
		public void OnBottomBtn1(EEvent evt, params object[] args)
		{
			XMainPlayer	CurSelObj	= CurRoInfoSelChar as XMainPlayer;
			if(CurSelObj != null )
			{				
				XEventManager.SP.SendEvent(EEvent.UI_Toggle,EUIPanel.eBagWindow);
			}
			else
			{
				XPet curSelPet = CurRoInfoSelChar as XPet;
				if(curSelPet == null)
					return ;
				
				UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(OnClickOK);
				string content = "确定要放生" + curSelPet.Name + "吗?";
				
				XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,content);
			}
			
		}
		
		public void OnBottomBtn2(EEvent evt, params object[] args)
		{
			XMainPlayer	CurSelObj	= CurRoInfoSelChar as XMainPlayer;
			if(CurSelObj != null )
			{				
				XEventManager.SP.SendEvent(EEvent.UI_Toggle,EUIPanel.eSkillOpertation);
			}
			else
			{
				XPet curSelPet = CurRoInfoSelChar as XPet;
				if(curSelPet == null)
					return ;
				
				UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(OnChangeName);
				string content = "确定要改名吗?";
				
				//XEventManager.SP.SendEvent(EEvent.InputMessageBox,funcOK,null,content);	
				XEventManager.SP.SendEvent(EEvent.InputMessageBox, funcOK, null, content, UIInputValidator.Validation.None, "");

			}
		}
		
		public void OnBottomBtn3(EEvent evt, params object[] args)
		{
			//食物
		}
		
		public void OnReflashRoleInfo(EEvent evt, params object[] args)
		{
			if ( !bShowCurrentUser )
				return;
			
			XMainPlayer player = CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{
				mUI.StateName.text = XGameColorDefine.JJ_Color[player.ClassLevel - 1];
				mUI.StateName.text += XCfgClassLvUpMgr.SP.GetConfig(player.ClassLevel).Name;
				
				mUI.GrowValue.text	= player.Grow.ToString("N1");
			}
			else
			{				
				XPet pet = CurRoInfoSelChar as XPet;							
				mUI.StateName.text = XGameColorDefine.JJ_Color[pet.ClassLevel - 1];
				mUI.StateName.text += XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel).Name;				
				mUI.GrowValue.text	= (pet.DynGet(EShareAttr.esa_Grow) / Define.CONFIG_RATE_BASE).ToString("N1");
			}
			
		}
		
		
		private void OnClickOK(GameObject go)
		{
			XPet curSelPet = CurRoInfoSelChar as XPet;
			
			for(int i = 0; i < (int)EQUIP_POS.EQUIP_POS_NUM; ++i)
			{
				int itemIndex = XItemManager.GetRealItemIndex(curSelPet,i);
				XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)itemIndex);
				if(tempItem == null)
					continue;
				if(!tempItem.IsEmpty())
				{
					XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 10054);
					return;
				}
			}				
			//宠物放生
			CS_UInt.Builder	msg = CS_UInt.CreateBuilder();
			msg.SetData(curSelPet.Index);
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_DelPet, msg.Build());
		}
		
		private void OnChangeName(GameObject go)
		{
			XPet curSelPet = CurRoInfoSelChar as XPet;
			//宠物改名
			CS_PetChangeName.Builder msg = CS_PetChangeName.CreateBuilder();
			msg.PetIndex = curSelPet.Index;
			msg.Name	 = XInputMessageBox.Content;
			XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_PetChangeName, msg.Build());
		}
		
		public void InitEquip(XRoleInformation UI)
		{
			if(IsInitEquip)
				return ;
			
			for(int i = 0; i < XRoleInformation.MAX_CHAR_ICON_NUM; i++)
			{
				RoleActionIcon[i] = new XRoleActionIcon();
				RoleActionIcon[i].SetUIIcon(UI.UIRoleInfoLeft.ActionIconArray[i]);
				RoleActionIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Equip,(uint)(UIPosToEquipPos[i]) );
			}
			IsInitEquip	= true;	
		}
		
		public void UpdateFashionSelStatus(XRoleInformation.RoleInfoLeft UI)
		{
			if ( 0 == XLogicWorld.SP.MainPlayer.FashionId ||
				!UI.ActionIconArray[6].gameObject.activeSelf )
			{
				UI.FashionSel.gameObject.SetActive(false);
			}
			else
			{
				UI.FashionSel.gameObject.SetActive(true);
				if ( XLogicWorld.SP.MainPlayer.ShowFashion == 0 )
				{
					UI.FashionSel.CommonTips = XStringManager.SP.GetString(1078u);
					UI.FashionSel.spriteName = "11000859";
				}
				else
				{
					UI.FashionSel.CommonTips = XStringManager.SP.GetString(1077u);
					UI.FashionSel.spriteName = "11000858";
				}
			}
		}
		
		public void OnSelectCurrentNickName(EEvent evt, params object[] args)
		{
			string str = (string)args[0];
			uint nid = 0;
			foreach(KeyValuePair<uint,XCfgNickName> s in XCfgNickNameMgr.SP.ItemTable)
			{
				if(s.Value.NickName == str)
				{
					nid = s.Value.nID;
					break;
				}
			}
			XNickNameManager.SP.HandleSetCurNickName(nid);
		}
	}
	
	
	//属性面板数据
	class LogicAttrInfo
	{
		private void InitAttrLabelMap()
	    {
			if(m_UIRoleAttr == null )
				return ;
			
			// 将属性和 Label进行挂接	
			m_AttrLabelMap.Add(EShareAttr.esa_WuLi, m_UIRoleAttr.AttrArray[0]);
	        m_AttrLabelMap.Add(EShareAttr.esa_LingQiao, m_UIRoleAttr.AttrArray[1]);
			m_AttrLabelMap.Add(EShareAttr.esa_TiZhi, m_UIRoleAttr.AttrArray[2]);
	        m_AttrLabelMap.Add(EShareAttr.esa_ShuFa,m_UIRoleAttr.AttrArray[3]);
	        m_AttrLabelMap.Add(EShareAttr.esa_TongLing,m_UIRoleAttr.AttrArray[4]);
	        m_AttrLabelMap.Add(EShareAttr.esa_MaxHp,m_UIRoleAttr.AttrArray[5]);
	
			m_AttrLabelMap.Add(EShareAttr.esa_LuckValue,m_UIRoleAttr.AttrArray[6]);
			m_AttrLabelMap.Add(EShareAttr.esa_MoraleValue,m_UIRoleAttr.AttrArray[7]);
			m_AttrLabelMap.Add(EShareAttr.esa_AttackValue,m_UIRoleAttr.AttrArray[8]);	
			m_AttrLabelMap.Add(EShareAttr.esa_CritValue,m_UIRoleAttr.AttrArray[9]);
			m_AttrLabelMap.Add(EShareAttr.esa_HitValue,m_UIRoleAttr.AttrArray[10]);
			m_AttrLabelMap.Add(EShareAttr.esa_AccuracyValue,m_UIRoleAttr.AttrArray[11]);
			m_AttrLabelMap.Add(EShareAttr.esa_AttackSpeed,m_UIRoleAttr.AttrArray[12]);	
			
	       
	        m_AttrLabelMap.Add(EShareAttr.esa_Defence,m_UIRoleAttr.AttrArray[13]);
	        m_AttrLabelMap.Add(EShareAttr.esa_ShuFaDefence,m_UIRoleAttr.AttrArray[14]);
	        m_AttrLabelMap.Add(EShareAttr.esa_DodgeValue,m_UIRoleAttr.AttrArray[15]);
	        m_AttrLabelMap.Add(EShareAttr.esa_HoldValue,m_UIRoleAttr.AttrArray[16]);
	        m_AttrLabelMap.Add(EShareAttr.esa_AntiCritRate,m_UIRoleAttr.AttrArray[17]);
			m_AttrLabelMap.Add(EShareAttr.esa_CounterRate,m_UIRoleAttr.AttrArray[18]);
			//m_AttrLabelMap.Add(EShareAttr.esa_LimbsResistRate,m_UIRoleAttr.AttrArray[19]);
			//m_AttrLabelMap.Add(EShareAttr.esa_SoulResistRate,m_UIRoleAttr.AttrArray[20]);
			
			
			
			m_UIRoleAttr.AttrMarkSprite[0].gameObject.SetActive(false);
			m_UIRoleAttr.AttrMarkSprite[1].gameObject.SetActive(false);
	    }
		
		public void showDetailAttr()
		{
	        if (null == m_AttrLabelMap)
	        {
				m_AttrLabelMap = new Hashtable(XRoleInformation.MAX_CHAR_ATTR_NUM);
	            this.InitAttrLabelMap();
	        }
			
			XCharacter CurChar	= 	XUIRoleInformation.CurRoInfoSelChar;
			if(CurChar == null)
				return ;
			
	        EShareAttr att;
	        UILabel xal;
			foreach (object k in m_AttrLabelMap.Keys)
			{
	            att = (EShareAttr)k;
	            xal = m_AttrLabelMap[k] as UILabel;
				if(att == EShareAttr.esa_Grow)
	            	xal.text = (CurChar.DynGet(att) / Define.CONFIG_RATE_BASE).ToString();
				else if(att == EShareAttr.esa_CounterRate)
					xal.text = (CurChar.DynGet(att) / 100.0f).ToString("N1") + "%";
				else
					xal.text = (CurChar.DynGet(att)).ToString();
			}
			
			ShowMark();
			
			//显示属性TIP
			XPlayer player = CurChar as XPlayer;
			int index = 0;
			if(player != null)
			{
				index = player.DynGet(EShareAttr.esa_Class);
			}
			else
			{
				XPet pet = CurChar as XPet;
				XCfgPetBase cfgPet = XCfgPetBaseMgr.SP.GetConfig(pet.PetID);
				if(cfgPet == null)
				{
					Log.Write(LogLevel.ERROR,"PET CONFIG ID {0} not find",pet.PetID);
					return ;
				}
				index	= cfgPet.ClassType2;
			}
			
			if(index == (int)EShareClass.eshClass_ZhanShi)
			{
				//武力
				m_UIRoleAttr.AttrArray[0].CommonTips	= XStringManager.SP.GetString(331);
				//灵巧
				m_UIRoleAttr.AttrArray[1].CommonTips	= XStringManager.SP.GetString(334);
				//术法
				m_UIRoleAttr.AttrArray[3].CommonTips	= XStringManager.SP.GetString(338);
				
				
			}
			else if(index == (int)EShareClass.eshClass_FaShi)
			{
				//武力
				m_UIRoleAttr.AttrArray[0].CommonTips	= XStringManager.SP.GetString(332);
				//灵巧
				m_UIRoleAttr.AttrArray[1].CommonTips	= XStringManager.SP.GetString(336);
				//术法
				m_UIRoleAttr.AttrArray[3].CommonTips	= XStringManager.SP.GetString(340);
				
			}
			else if(index == (int)EShareClass.eshClass_GongJianShou)
			{
				//武力
				m_UIRoleAttr.AttrArray[0].CommonTips	= XStringManager.SP.GetString(333);
				//灵巧
				m_UIRoleAttr.AttrArray[1].CommonTips	= XStringManager.SP.GetString(335);
				//术法
				m_UIRoleAttr.AttrArray[3].CommonTips	= XStringManager.SP.GetString(339);
			}
			
			//体力
			m_UIRoleAttr.AttrArray[2].CommonTips	= XStringManager.SP.GetString(337);
			//统领
			m_UIRoleAttr.AttrArray[4].CommonTips	= XStringManager.SP.GetString(341);
			//生命
			m_UIRoleAttr.AttrArray[5].CommonTips	= XStringManager.SP.GetString(342);
			
			//幸运
			m_UIRoleAttr.AttrArray[6].CommonTips	= string.Format(XStringManager.SP.GetString(343),(CurChar.DynGet(EShareAttr.esa_LuckRate)/100.0f).ToString("N1"));
			//士气
			m_UIRoleAttr.AttrArray[7].CommonTips	= string.Format(XStringManager.SP.GetString(344),(CurChar.DynGet(EShareAttr.esa_MoraleRate)/100.0f).ToString("N1"));
			//攻击
			m_UIRoleAttr.AttrArray[8].CommonTips	= XStringManager.SP.GetString(345);
			//暴击
			m_UIRoleAttr.AttrArray[9].CommonTips	= string.Format(XStringManager.SP.GetString(346),(CurChar.DynGet(EShareAttr.esa_CritRate)/100.0f).ToString("N1"));
			//命中
			m_UIRoleAttr.AttrArray[10].CommonTips	= string.Format(XStringManager.SP.GetString(347),(CurChar.DynGet(EShareAttr.esa_HitRate)/100.0f).ToString("N1"));
			//准确
			m_UIRoleAttr.AttrArray[11].CommonTips	= string.Format(XStringManager.SP.GetString(348),(CurChar.DynGet(EShareAttr.esa_AccuracyRate)/100.0f).ToString("N1"));
			//速度
			m_UIRoleAttr.AttrArray[12].CommonTips	= XStringManager.SP.GetString(349);
			//护甲
			m_UIRoleAttr.AttrArray[13].CommonTips	= XStringManager.SP.GetString(350);
			//术防
			m_UIRoleAttr.AttrArray[14].CommonTips	= XStringManager.SP.GetString(351);
			//闪避
			m_UIRoleAttr.AttrArray[15].CommonTips	= string.Format(XStringManager.SP.GetString(352),(CurChar.DynGet(EShareAttr.esa_DodgeRate)/100.0f).ToString("N1"));
			//韧性
			m_UIRoleAttr.AttrArray[16].CommonTips	= string.Format(XStringManager.SP.GetString(354),(CurChar.DynGet(EShareAttr.esa_HoldRate)/100.0f).ToString("N1"));
			//抵挡
			m_UIRoleAttr.AttrArray[17].CommonTips	= string.Format(XStringManager.SP.GetString(353),(CurChar.DynGet(EShareAttr.esa_AntiCritRate)/100.0f).ToString("N1"));
			//心神抵抗
			m_UIRoleAttr.AttrArray[18].CommonTips	= XStringManager.SP.GetString(355);
			//形体抵抗
			//m_UIRoleAttr.AttrArray[19].CommonTips	= XStringManager.SP.GetString(356);
			//魂魄抵抗
			//m_UIRoleAttr.AttrArray[20].CommonTips	= XStringManager.SP.GetString(357);	
	    }
		
		private void ShowMark()
		{
			XCharacter CurChar	= 	XUIRoleInformation.CurRoInfoSelChar;
			
			XCfgAttrMark CfgMark = XCfgAttrMarkMgr.SP.GetConfig((uint)CurChar.DynGet(EShareAttr.esa_Class));
			if(CfgMark == null)
				return ;
			
			for(int i = 0; i < 5; i++)
			{
				if(CfgMark.AttrMark[i] == 2 || CfgMark.AttrMark[i] == 3)
					m_UIRoleAttr.AttrSprite[i].spriteName	= m_UIRoleAttr.AttrMarkSprite[1].spriteName;
				else if(CfgMark.AttrMark[i] == 1)
					m_UIRoleAttr.AttrSprite[i].spriteName	= m_UIRoleAttr.AttrMarkSprite[0].spriteName;
				else
					m_UIRoleAttr.AttrSprite[i].gameObject.SetActive(false);
			}
			
		}
		
		public Hashtable 	m_AttrLabelMap = null;
		public XRoleInformation.RoleAttr	m_UIRoleAttr = null;
	}
	
	//化灵面板数据
	class LogicHuaLingInfo
	{
		public XRoleInformation.HuaLing UI;
		private uint CurSel = 0;
		static private uint CurComTrainNum = 100;
		static private uint CurHighTrainNum = 100;
		
		public void OnRadioChanged(int nIndex)
		{
			CurSel	= (uint)nIndex;
			Show();
		}
		
		public void Show()
		{
			//培养部分
			uint leftCount = MAX_DAY_PEIYANG_COUNT - XLogicWorld.SP.MainPlayer.DayPeiYangCount;
			UI.TrainDes.text	= "今天可培养" + leftCount.ToString() + "次";
			
			XCfgClassLvUp cfgClassLvUp = null;
			int Level = 0;
			int ClassLevel = 0;
			float Grow = 0;
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{				
				Level	= player.Level;
				ClassLevel	= (int)player.ClassLevel;
				Grow		= player.Grow;
				cfgClassLvUp = XCfgClassLvUpMgr.SP.GetConfig(player.ClassLevel);
				if(cfgClassLvUp != null)
				{
					if(player.Grow < cfgClassLvUp.MaxGrowth)
						UI.CurGrow.text	= "[color=FFF337]" + player.Grow.ToString("N1");
					else
						UI.CurGrow.text	= "[color=FF1515]" + player.Grow.ToString("N1");
				}
				
				Vector3 showPos2 = new Vector3(-150, 10, 0);
				XNewPlayerGuideManager.SP.handleShowGuideExt2((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Click, 
					(int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use, 3, showPos2, UI.GuideObj);
				
				XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Select);
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				cfgClassLvUp = XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel);
				if(cfgClassLvUp != null)
				{
					if((pet.DynGet(EShareAttr.esa_Grow)/ Define.CONFIG_RATE_BASE)< cfgClassLvUp.MaxGrowth)
						UI.CurGrow.text	= "[color=FFF337]" + (pet.DynGet(EShareAttr.esa_Grow)/ Define.CONFIG_RATE_BASE).ToString("N1");
					else
						UI.CurGrow.text	= "[color=FF1515]" + (pet.DynGet(EShareAttr.esa_Grow)/ Define.CONFIG_RATE_BASE).ToString("N1");
				}
				
				Level		= pet.Level;
				ClassLevel	= (int)pet.ClassLevel;
				Grow		= pet.DynGet(EShareAttr.esa_Grow) / Define.CONFIG_RATE_BASE;
			}
			
			bool isEnoughMoney = false;
			XCfgTrain cfgTrain = XCfgTrainMgr.SP.GetConfig((uint)Level);
			if(cfgTrain != null)
			{
				if(UI.TrainRadio.CurrentSelect == 0)
				{
					if(XLogicWorld.SP.MainPlayer.GameMoney >= cfgTrain.CostGameMoney)
					{
						UI.NeedMoney.text	= "[color=FFF337]" + cfgTrain.CostGameMoney.ToString() + "金";
						isEnoughMoney	= true;
					}
					else
					{
						UI.NeedMoney.text	= "[color=FF1515]" + cfgTrain.CostGameMoney.ToString() + "金";
					}
				}					
				else
				{
					if(XLogicWorld.SP.MainPlayer.RealMoney >= cfgTrain.CostRealMoney)
					{
						UI.NeedMoney.text	= "[color=FFF337]" + cfgTrain.CostRealMoney.ToString() + "元宝";
						isEnoughMoney	= true;
					}
					else
						UI.NeedMoney.text	= "[color=FF1515]" + cfgTrain.CostRealMoney.ToString() + "元宝";
				}
			}
			
			if((Mathf.Abs(Grow - cfgClassLvUp.MaxGrowth) < 0.1f)|| !isEnoughMoney || leftCount == 0)
			{
				UI.PeiYangBtn.isEnabled	= false;
				UI.PeiYangBtn.UpdateImage();
			}
			else
			{
				UI.PeiYangBtn.isEnabled	= true;
				UI.PeiYangBtn.UpdateImage();
			}
			
			//化境部分
//			UI.CurClassLevel.text	= XGameColorDefine.JJ_Color[ClassLevel] + cfgClassLvUp.Name;
//			
//			if(ClassLevel < 6)
//			{
//				XCfgClassLvUp cfgNextClassLvUp = XCfgClassLvUpMgr.SP.GetConfig((uint)(ClassLevel + 1));
//				if(cfgNextClassLvUp != null)
//					UI.NextClassLevel.text	=  XGameColorDefine.JJ_Color[ClassLevel + 1] + cfgNextClassLvUp.Name;
//			}				
//			else
//			{				
//				UI.NextClassLevel.text	=  XGameColorDefine.JJ_Color[ClassLevel] + "MAX";
//			}
				
			bool isEnoughLevel = false;
			if(cfgClassLvUp.LvRequire > Level)
			{
				isEnoughLevel	= false;
				//UI.NeedLevel.text	= "[color=FF1515]" + cfgClassLvUp.LvRequire.ToString();
			}
			else
			{
				isEnoughLevel	= true;
				//UI.NeedLevel.text	= "[color=FFF337]" + cfgClassLvUp.LvRequire.ToString();
			}
			
			bool isEnoughGrow = false;
			if(cfgClassLvUp.GrowthRequire > Grow)
			{
			}
				//UI.NeedGrow.text	= "[color=FF1515]" + cfgClassLvUp.GrowthRequire.ToString("N1");
			else
			{
				//UI.NeedGrow.text	= "[color=FFF337]" + cfgClassLvUp.GrowthRequire.ToString("N1");
				isEnoughGrow	= true;
			}
			
			UI.HLBtn.CommonTips	= "等级达到" + cfgClassLvUp.LvRequire.ToString() + "级,灵力达到" + cfgClassLvUp.GrowthRequire.ToString() + "才能提升境界";
			
			if(!isEnoughGrow || !isEnoughLevel)
			{
				//UI.HLBtn.isEnabled	= false;
				//UI.HLBtn.enabled	= false;
			}
			else
			{
				//UI.HLBtn.isEnabled	= true;
				//UI.HLBtn.enabled	= true;	
				
				
				// 新手引导
//				Vector3 showPos = new Vector3(Mathf.Round(UI.HLBtn.transform.localPosition.x - 140),
//					Mathf.Round(UI.HLBtn.transform.localPosition.y + 10),
//					Mathf.Round(UI.HLBtn.transform.localPosition.z));
//				XNewPlayerGuideManager.SP.handleShowGuide((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Click, 
//					1, showPos, UI.HLBtn.transform.parent.gameObject);
			}
		}
		
		public void OnClickPY(EEvent evt, params object[] args)
		{
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Use);
			
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			XCfgClassLvUp cfgClassLvUp = null;
			if(player != null)
			{
				cfgClassLvUp = XCfgClassLvUpMgr.SP.GetConfig(player.ClassLevel);
				if(cfgClassLvUp != null)
				{
					if(player.Grow >= cfgClassLvUp.MaxGrowth)
						return ;
					
					if(UI.TrainRadio.CurrentSelect == 0)
					{
						if(CurComTrainNum <= 0)
							return ;
					}
					else
					{
						if(CurHighTrainNum <= 0)
							return ;
					}
					
					CS_Int.Builder	msg = CS_Int.CreateBuilder();
					//0普通培养 1高级培养
					msg.SetData(UI.TrainRadio.CurrentSelect);
					XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Player_PeiYang, msg.Build());
				}
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				cfgClassLvUp = XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel);
				if(cfgClassLvUp != null)
				{
					if((pet.DynGet(EShareAttr.esa_Grow)/ Define.CONFIG_RATE_BASE) >= cfgClassLvUp.MaxGrowth)
						return ;
					
					if(UI.TrainRadio.CurrentSelect == 0)
					{
						if(CurComTrainNum <= 0)
							return ;
					}
					else
					{
						if(CurHighTrainNum <= 0)
							return ;
					}
					
					CS_Pet_PeiYang.Builder msg = CS_Pet_PeiYang.CreateBuilder();
					msg.SetPetIndex(pet.Index);
					msg.SetType((uint)UI.TrainRadio.CurrentSelect);
					XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Pet_PeiYang, msg.Build());
				}
			}
		}
		
		public void OnClickHL(EEvent evt, params object[] args)
		{
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{
				CS_Empty.Builder msg = CS_Empty.CreateBuilder();
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Player_HJ, msg.Build());
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				CS_Int.Builder msg = CS_Int.CreateBuilder();
				msg.SetData((int)pet.Index);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Pet_HJ, msg.Build());
			}
			
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_HuaLing_Click);
		}
		
		public void OnReflashJJ(EEvent evt, params object[] args)
		{
			XEventManager.SP.SendEvent(EEvent.CharInfo_Reflash_RoleInfo);
			Show();
			
		}
		
		public void OnReflashPeiYang(EEvent evt, params object[] args)
		{
			float value = (float)args[0];
			uint type = (uint)args[1];
			UI.PeiYValue.text	= "+" + value.ToString("N1");
			
			TweenAlpha alpha = UI.PeiYValue.GetComponent<TweenAlpha>();
			if(alpha != null)
			{
				alpha.Reset();
				alpha.enabled	= true;
			}
			
			if(type == 0)
				CurComTrainNum--;
			else
				CurHighTrainNum--;
			
			Show();
			
			XEventManager.SP.SendEvent(EEvent.CharInfo_Reflash_RoleInfo);
		}
	}
	
	//符印面板数据
	class LogicFuYinInfo
	{
		public XRoleInformation.FuYin UI;
		private Dictionary<ushort,uint> mAttrMap = new Dictionary<ushort,uint>();
		public XRoleActionIcon[] mLogicActionIcon = new XRoleActionIcon[6];
		private bool mIsInit = false;
		
		// for guide
		public GameObject actionGuideIcon;
		
		private void showAllFuYinEquip()
		{
			//显示装备中的物品
			XCharacter	CurSelObj	= CurRoInfoSelChar;
			int level = CurSelObj.Level;
			
			int ObjectIndex = GetCurSelObjIndex();
			for(int i = 0; i < 6; i++)
			{
				if(level >= GlobalU3dDefine.FuYinOpenLevel[i])
					mLogicActionIcon[i].ShowUnOpen(true);
				else
					mLogicActionIcon[i].ShowUnOpen(false);
			}
			
			//显示符印装备
			for(int n = (int)EQUIP_POS.EQUIP_POS_FUYIN_START; n <= (int)EQUIP_POS.EQUIP_POS_FUYIN_END; n++)
			{
				uint realIndex = (uint)XItemManager.GetRealItemIndex(CurSelObj,n);
				XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)realIndex);
				if(item == null || item.IsEmpty())
				{
					mLogicActionIcon[n - (int)EQUIP_POS.EQUIP_POS_FUYIN_START].Reset();
					mLogicActionIcon[n - (int)EQUIP_POS.EQUIP_POS_FUYIN_START].ObjectIndex	= ObjectIndex;
					continue ;
				}
				
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
				if(cfgItem == null)
					continue;
				
				mLogicActionIcon[n - (int)EQUIP_POS.EQUIP_POS_FUYIN_START].SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,item.Color,item.ItemCount);
				
				XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_FuYin_Equip);
			}
		}
		
		public void Show()
		{
			showAllFuYinEquip();
			
			XCharacter ch = XUIRoleInformation.CurRoInfoSelChar;
			if(ch == null)
				return ;

			//显示所有符印属性
			mAttrMap.Clear();
			int ObjectIndex = GetCurSelObjIndex();
			for(int i = 0; i < 6; i++)
			{
				uint realIndex = (uint)XItemManager.GetRealItemIndex(ch,i+(int)EQUIP_POS.EQUIP_POS_FUYIN_START);
				XItem item = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)realIndex);
				if(item == null || item.IsEmpty())
				{
					continue ;
				}
				
				//基本属性
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(item.DataID);
				if(cfgItem == null)
					continue ;
				
				if(cfgItem.BaseAttrType != 0)
				{
					if(mAttrMap.ContainsKey((ushort)cfgItem.BaseAttrType))
					{
						mAttrMap[(ushort)cfgItem.BaseAttrType]	= (uint)mAttrMap[(ushort)cfgItem.BaseAttrType] + cfgItem.BaseAttrValue;
					}
					else
					{
						mAttrMap.Add((ushort)cfgItem.BaseAttrType,(uint)cfgItem.BaseAttrValue);
					}
				}				
				
				//随机属性
				if(item.IsHasDynamicMagicAttr())
				{
					for(int j = 0; j < (int)EItem_Equip.MAX_RANDOM_ATTR_NUM; j++)
					{
						if(item.mEquipAttr.mAttrID[j] == 0)
							continue;
						if(mAttrMap.ContainsKey(item.mEquipAttr.mAttrID[j]))
						{
							mAttrMap[item.mEquipAttr.mAttrID[j]] = (uint)mAttrMap[item.mEquipAttr.mAttrID[j]] + item.mEquipAttr.mAttrValue[j];
						}
						else
						{
							mAttrMap.Add(item.mEquipAttr.mAttrID[j],item.mEquipAttr.mAttrValue[j]);
						}						
					}
				}
				else
				{
					for(int j = 0; j < (int)EItem_Equip.MAX_RANDOM_ATTR_NUM; j++)
					{
						if(cfgItem.MagicAttrType[j] == 0)
							continue;
						if(mAttrMap.ContainsKey(cfgItem.MagicAttrType[j]))
						{
							mAttrMap[cfgItem.MagicAttrType[j]]	= (uint)mAttrMap[cfgItem.MagicAttrType[j]] + cfgItem.MagicAttrValue[j];
						}
						else
						{
							mAttrMap.Add(cfgItem.MagicAttrType[j],cfgItem.MagicAttrValue[j]);
						}
					}
				}
			}
			
			UI.Clear();
			
			XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_FuYin_Select);
			
			XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_FuYin_Equip, 3,
				new Vector3(actionGuideIcon.transform.localPosition.x + 140, actionGuideIcon.transform.localPosition.y + 10, actionGuideIcon.transform.localPosition.z)
				, actionGuideIcon);

			//显示
			if(mAttrMap.Count == 0)
				return ;
			
			int count = 0;
			foreach(KeyValuePair<ushort,uint> k in mAttrMap)
			{				
				ushort key = k.Key;
				XCfgAttrID CfgAttrID = XCfgAttrIDMgr.SP.GetConfig((uint)key);
				if(CfgAttrID == null)
					continue;
				
				UI.attrName[count].text		= CfgAttrID.EffectDes;
				UI.attrValue[count].text	= k.Value.ToString();
				count++;
			}
		}
		
		public void Init()
		{
			if(mIsInit)
				return ;
			
			for(int i = 0; i < 6; i++)
			{
				mLogicActionIcon[i] = new XRoleActionIcon();
				mLogicActionIcon[i].SetUIIcon(UI.action[i]);
				mLogicActionIcon[i].SetLogicData(ActionIcon_Type.ActionIcon_Equip,(uint)(EQUIP_POS.EQUIP_POS_FUYIN_START + i) );
			}
		}
	}
	
	//灵丹面板数据
	class LogicLingDanInfo
	{
		public XRoleInformation.LingDan UI;
		public void Show()
		{
			if(UI == null)
				return ;
			
			XMainPlayer ch = CurRoInfoSelChar as XMainPlayer;
			XCfgClassLvUp CfgLevelUp = null;
			if(ch == null)
			{
				XPet pet = CurRoInfoSelChar as XPet;
				
				uint classLevel = pet.ClassLevel;
				CfgLevelUp = XCfgClassLvUpMgr.SP.GetConfig(classLevel);
				if(CfgLevelUp == null)
					return ;
				
				uint WuLi = pet.WuLi;
				UI.AttrSlider[0].sliderValue	= WuLi * 1.0f/ CfgLevelUp.PanaceaCount;
				UI.AttrValue[0].text			= WuLi.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();				
				
				uint LingQiao = pet.LingQiao;
				UI.AttrSlider[1].sliderValue	= LingQiao * 1.0f/ CfgLevelUp.PanaceaCount;
				UI.AttrValue[1].text			= LingQiao.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();
				
				
				uint TiZhi = pet.TiZhi;
				UI.AttrSlider[2].sliderValue	= TiZhi * 1.0f/ CfgLevelUp.PanaceaCount;
				UI.AttrValue[2].text			= TiZhi.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();
				
				
				uint ShuFa = pet.ShuFa;
				UI.AttrSlider[3].sliderValue	= ShuFa * 1.0f/ CfgLevelUp.PanaceaCount;
				UI.AttrValue[3].text			= ShuFa.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();
				
				if(CfgLevelUp.CostRealMoney > XLogicWorld.SP.MainPlayer.RealMoney)
				{
					UI.RandomAddBtn.isEnabled = false;
					UI.RandomAddBtn.UpdateImage();
				}
				else
				{
					UI.RandomAddBtn.isEnabled = true;
					UI.RandomAddBtn.UpdateImage();
				}
				
				UI.RandomAddBtn.CommonTips	= "花费" + CfgLevelUp.CostRealMoney.ToString() + "元宝进行内丹修炼";
			}
			else
			{
				XNewPlayerGuideManager.SP.handleGuideFinishExt((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Select);
				
				uint classLevel = ch.ClassLevel;
				CfgLevelUp = XCfgClassLvUpMgr.SP.GetConfig(classLevel);
				if(CfgLevelUp == null)
					return ;
				
				uint WuLi = ch.WuLi;
				UI.AttrSlider[0].sliderValue	= WuLi * 1.0f/ CfgLevelUp.PanaceaCount;
				if(UI.AttrSlider[0].sliderValue == 1.0f)
					UI.AttrValue[0].text	= "[color=FFF337]";
				else
					UI.AttrValue[0].text	= "[color=00FF00]";
				
				UI.AttrValue[0].text			+= WuLi.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();
				
				uint LingQiao = ch.LingQiao;
				UI.AttrSlider[1].sliderValue	= LingQiao * 1.0f/ CfgLevelUp.PanaceaCount;
				if(UI.AttrSlider[1].sliderValue == 1.0f)
					UI.AttrValue[1].text	= "[color=FFF337]";
				else
					UI.AttrValue[1].text	= "[color=00FF00]";
				UI.AttrValue[1].text			+= LingQiao.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();
				
				
				uint TiZhi = ch.TiZhi;
				UI.AttrSlider[2].sliderValue	= TiZhi * 1.0f/ CfgLevelUp.PanaceaCount;
				if(UI.AttrSlider[2].sliderValue == 1.0f)
					UI.AttrValue[2].text	= "[color=FFF337]";
				else
					UI.AttrValue[2].text	= "[color=00FF00]";
				UI.AttrValue[2].text			+= TiZhi.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();
				
				uint ShuFa = ch.ShuFa;
				UI.AttrSlider[3].sliderValue	= ShuFa * 1.0f/ CfgLevelUp.PanaceaCount;
				if(UI.AttrSlider[3].sliderValue == 1.0f)
					UI.AttrValue[3].text	= "[color=FFF337]";
				else
					UI.AttrValue[3].text	= "[color=00FF00]";
				UI.AttrValue[3].text			+= ShuFa.ToString() + "/" + CfgLevelUp.PanaceaCount.ToString();
				
				if(CfgLevelUp.CostRealMoney > XLogicWorld.SP.MainPlayer.RealMoney)
				{
					UI.RandomAddBtn.isEnabled = false;
					UI.RandomAddBtn.UpdateImage();
				}
				else
				{
					UI.RandomAddBtn.isEnabled = true;
					UI.RandomAddBtn.UpdateImage();
				}
				
				UI.RandomAddBtn.CommonTips	= "花费" + CfgLevelUp.CostRealMoney.ToString() + "元宝进行内丹修炼";
			}		
			
			uint itemCount = (uint)XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(CfgLevelUp.ItemID[0]);
			UI.AddBtn[0].gameObject.SetActive(itemCount > 0? true : false);
			
			itemCount = (uint)XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(CfgLevelUp.ItemID[1]);				
			UI.AddBtn[1].gameObject.SetActive(itemCount > 0? true : false);
			
			itemCount = (uint)XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(CfgLevelUp.ItemID[2]);				
			UI.AddBtn[2].gameObject.SetActive(itemCount > 0? true : false);
			
			itemCount = (uint)XLogicWorld.SP.MainPlayer.ItemManager.GetItemByDataID(CfgLevelUp.ItemID[3]);				
			UI.AddBtn[3].gameObject.SetActive(itemCount > 0? true : false);	
			
			// 新手引导
			int index = 0;
			int cls = XLogicWorld.SP.MainPlayer.Class;
			switch (cls)
			{
			case 1:
				index = 0;
			
				break;
			case  2:
				index = 1;
			
				break;;
			case 3:
				index = 2;
			
				break;
			default:
				break;
			}
			if ( UI.AddBtn[index].gameObject.activeSelf )
			{
				Vector3 showPos = new Vector3(120, 10, 0);
				XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Use, 3, showPos, UI.AddBtn[index].gameObject);
			}
		}
		
		public void Reflash(EEvent evt, params object[] args)
		{
			Show();
		}
		
		public void OnClickWuLi(EEvent evt, params object[] args)
		{
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{
				CS_Player_AddLingDan.Builder	msg = CS_Player_AddLingDan.CreateBuilder();
				msg.SetIndex(0);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Player_AddLingDan, msg.Build());
				
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				CS_Pet_AddLingDan.Builder	msg = CS_Pet_AddLingDan.CreateBuilder();
				msg.SetIndex(0);
				msg.SetPetIndex(pet.Index);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Pet_AddLingDan, msg.Build());
			}
			
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Use);
		}
		
		public void OnClickLingQiao(EEvent evt, params object[] args)
		{
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{
				CS_Player_AddLingDan.Builder	msg = CS_Player_AddLingDan.CreateBuilder();
				msg.SetIndex(1);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Player_AddLingDan, msg.Build());
				
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				CS_Pet_AddLingDan.Builder	msg = CS_Pet_AddLingDan.CreateBuilder();
				msg.SetIndex(1);
				msg.SetPetIndex(pet.Index);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Pet_AddLingDan, msg.Build());
			}
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Use);
		}
		
		public void OnClickTiZhi(EEvent evt, params object[] args)
		{
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{
				CS_Player_AddLingDan.Builder	msg = CS_Player_AddLingDan.CreateBuilder();
				msg.SetIndex(2);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Player_AddLingDan, msg.Build());
				
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				CS_Pet_AddLingDan.Builder	msg = CS_Pet_AddLingDan.CreateBuilder();
				msg.SetIndex(2);
				msg.SetPetIndex(pet.Index);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Pet_AddLingDan, msg.Build());
			}
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Use);
		}
		
		public void OnClickShuFa(EEvent evt, params object[] args)
		{
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{
				CS_Player_AddLingDan.Builder	msg = CS_Player_AddLingDan.CreateBuilder();
				msg.SetIndex(3);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Player_AddLingDan, msg.Build());
				
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				CS_Pet_AddLingDan.Builder	msg = CS_Pet_AddLingDan.CreateBuilder();
				msg.SetIndex(3);
				msg.SetPetIndex(pet.Index);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Pet_AddLingDan, msg.Build());
			}
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_LingDan_Use);
		}
		
		public void OnClickRandomAddLD(EEvent evt, params object[] args)
		{
			XMainPlayer player = XUIRoleInformation.CurRoInfoSelChar as XMainPlayer;
			if(player != null)
			{
				XCfgClassLvUp classLvUp = XCfgClassLvUpMgr.SP.GetConfig(player.ClassLevel);
				if(classLvUp == null)
					return ;
				
				if(classLvUp.CostRealMoney > player.RealMoney)
				{
					XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,26);
					return ;
				}
				
				CS_Empty.Builder msg = CS_Empty.CreateBuilder();
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Player_Random_AddLD, msg.Build());
				
			}
			else
			{
				XPet pet = XUIRoleInformation.CurRoInfoSelChar as XPet;
				XCfgClassLvUp classLvUp = XCfgClassLvUpMgr.SP.GetConfig(pet.ClassLevel);
				if(classLvUp == null)
					return ;
				
				if(classLvUp.CostRealMoney > XLogicWorld.SP.MainPlayer.RealMoney)
				{
					XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,26);
					XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
					return ;
				}
				
				CS_UInt.Builder msg = CS_UInt.CreateBuilder();
				msg.SetData(pet.Index);
				XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Pet_Random_AddLD, msg.Build());
			}
		}
		
	}
	
	public XUIRoleInformation()
	{
		XEventManager.SP.AddHandler(OnFirstOpenCharInfo, EEvent.CharInfo_FirstOpen);
		XEventManager.SP.AddHandler(OnUpdateSprite, EEvent.charInfo_SetSprite);
		XEventManager.SP.AddHandler(OnUpdateRoleInformation, EEvent.charInfo_Update);		

		XEventManager.SP.AddHandler(OnDynamicAttrUpdate, EEvent.Attr_Dynamic);
		XEventManager.SP.AddHandler(RoleInfo.OnUIModel, EEvent.Attr_ModelId);
		XEventManager.SP.AddHandler(RoleInfo.OnUIModel, EEvent.Attr_WeaponItemID);
		
		XEventManager.SP.AddHandler(RoleInfo.OnTurnLeft, EEvent.CharInfo_TurnLeft);
		XEventManager.SP.AddHandler(RoleInfo.OnTurnRight, EEvent.CharInfo_TurnRight);
		
		XEventManager.SP.AddHandler(RoleInfo.OnBottomBtn1, EEvent.CharInfo_BottomBtn1);
		XEventManager.SP.AddHandler(RoleInfo.OnBottomBtn2, EEvent.CharInfo_BottomBtn2);
		XEventManager.SP.AddHandler(RoleInfo.OnBottomBtn3, EEvent.CharInfo_BottomBtn3);
		
		XEventManager.SP.AddHandler(LeftBtn.Reflash, EEvent.CharInfo_DelPet);
		XEventManager.SP.AddHandler(LeftBtn.SetName, EEvent.CharInfo_ChangeName);
		
		XEventManager.SP.AddHandler(LingDanInfo.OnClickWuLi,EEvent.CharInfo_WuLi);
		XEventManager.SP.AddHandler(LingDanInfo.OnClickLingQiao,EEvent.CharInfo_LingQiao);
		XEventManager.SP.AddHandler(LingDanInfo.OnClickTiZhi,EEvent.CharInfo_TiZhi);
		XEventManager.SP.AddHandler(LingDanInfo.OnClickShuFa,EEvent.CharInfo_FaShu);
		XEventManager.SP.AddHandler(LingDanInfo.OnClickRandomAddLD, EEvent.CharInfo_RandomAddLD);
		
		XEventManager.SP.AddHandler(LingDanInfo.Reflash,EEvent.CharInfo_UpdateLingDan);
		RegEventAgent_CheckCreated(EEvent.CharInfo_LingDan, RightBtn.OnShowLingDan);
		
		XEventManager.SP.AddHandler(HuaLingInfo.OnClickHL,EEvent.CharInfo_HL);
		XEventManager.SP.AddHandler(HuaLingInfo.OnClickPY,EEvent.CharInfo_PY);
		XEventManager.SP.AddHandler(HuaLingInfo.OnReflashJJ,EEvent.CharInfo_Reflash_JJ);
		RegEventAgent_CheckCreated(EEvent.CharInfo_OpenHuaLing, OpenHuaLingPanel);
		
		XEventManager.SP.AddHandler(HuaLingInfo.OnReflashPeiYang,EEvent.CharInfo_Reflash_PeiYang);
		XEventManager.SP.AddHandler(RoleInfo.OnReflashRoleInfo,EEvent.CharInfo_Reflash_RoleInfo);
		RegEventAgent_CheckCreated(EEvent.CharInfo_NickNameSelect, RoleInfo.OnSelectCurrentNickName);
		RegEventAgent_CheckCreated(EEvent.UI_ShengWangValue, LeftBtn.Reflash);
		RegEventAgent_CheckCreated(EEvent.Update_Level, LeftBtn.Reflash);
		RegEventAgent_CheckCreated(EEvent.UI_UpdateBattleValue, LeftBtn.ReflashBattleValue);
		
		XEventManager.SP.AddHandler(RoleInfo.OnChangeFashionState, EEvent.CharInfo_FashionChange);
		XEventManager.SP.AddHandler(LeftBtn.Reflash, EEvent.CharInfo_AddPet);
		XEventManager.SP.AddHandler(LeftBtn.ReflashRight, EEvent.Attr_Changed);
		
		XEventManager.SP.AddHandler(Container.UpdateContainerHandle, EEvent.Bag_UpdateItemContainer);
		XEventManager.SP.AddHandler(Container.LeftBtnHandle, EEvent.CharInfo_LeftPageBtn);
		XEventManager.SP.AddHandler(Container.RightBtnHandle, EEvent.CharInfo_RightPageBtn);		
	}

	public override void OnShow()
    {
		if(!XItemPacketProcess.SP.IsOpenedRole)
		{
			XItemPacketProcess.SP.IsOpenedRole = true;
			XEventManager.SP.SendEvent(EEvent.CharInfo_FirstOpen, EUIPanel.eRoleInformation);
		}
		
		CurSelIndex	= -1;
		
		//固定数据
		//ShowStaticData();
		
		//左侧角色数据
		RoleInfo.mUI	= LogicUI.UIRoleInfoLeft;
		RoleInfo.InitEquip(LogicUI);
		
		RoleInfo.UpdateFashionSelStatus(RoleInfo.mUI);
		
		Container.Init(LogicUI.ItemSelIconList);
		Container.LeftBtn	= LogicUI.LeftBtn;
		Container.RightBtn	= LogicUI.RightBtn;
		Container.LabelPage	= LogicUI.LabelPage;
		
		//左侧按钮
		LeftBtn.spriteGroup	= LogicUI.LSpriteGroup;
		LeftBtn.RoleInfo	= RoleInfo;
		LeftBtn.RightBtn	= RightBtn;		
		LeftBtn.spriteGroup.mModify	= new UISpriteGroup.OnSelectModify(LeftBtn.SetObjectData);	
		
		//右侧数据
		AttrInfo.m_UIRoleAttr	= LogicUI.UIRoleAttr;
		
		//右侧按钮
		RightBtn.LogicUI		= LogicUI;
		RightBtn.spriteGroup	= LogicUI.TSpriteGroup;
		RightBtn.Container		= Container;
		RightBtn.spriteGroup.mModify = new UISpriteGroup.OnSelectModify(RightBtn.SetFuncData);	
		RightBtn.EventBtnClick[0]	= new XUIRoleInformation.LogicRRoleBtn.BtnClick(AttrInfo.showDetailAttr);
		RightBtn.EventBtnClick[1]	= new XUIRoleInformation.LogicRRoleBtn.BtnClick(LingDanInfo.Show);
		RightBtn.EventBtnClick[2]	= new XUIRoleInformation.LogicRRoleBtn.BtnClick(HuaLingInfo.Show);
		RightBtn.EventBtnClick[3]	= new XUIRoleInformation.LogicRRoleBtn.BtnClick(FuYinInfo.Show);
		
		
		LingDanInfo.UI			= LogicUI.UILingDan;
		HuaLingInfo.UI			= LogicUI.UIHuaLing;
		HuaLingInfo.UI.TrainRadio.onRadioChanged	= new UIRadioButton.OnRadioChanged(HuaLingInfo.OnRadioChanged);
		
		FuYinInfo.UI			= LogicUI.UIFuYin;
		FuYinInfo.actionGuideIcon = LogicUI.ItemSelIconList[0].gameObject;
		FuYinInfo.Init();
		
		LogicUI.UIRoleInfoLeft.FuncBtn[0].gameObject.SetActive(bShowCurrentUser);
		LogicUI.UIRoleInfoLeft.FuncBtn[1].gameObject.SetActive(bShowCurrentUser);		
		
		LeftBtn.Show();	
		if ( !bShowCurrentUser )
			LogicUI.TSpriteGroup.SetVisible(1);
    }
	
	public override void OnHide()
	{
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipA);
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipC);
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.ePopMenu);
		base.OnHide();
	}
	
//	private void ShowStaticData()
//	{
//		LogicUI.HeadTitle.text	= XLogicWorld.SP.MainPlayer.Name;
//	}
	
	public void OnUpdateRoleInformation(EEvent evt, params object[] args)
	{
		if(LogicUI != null)
			Container.Show(Container.CurSelFunc);
	}
	
	public void OpenHuaLingPanel(EEvent evt, params object[] args)
	{
		if(LogicUI == null)
			return;
		
		RightBtn.SetFuncData(2);
		RightBtn.spriteGroup.SetSelect(0, false);
		RightBtn.spriteGroup.SetSelect(1, false);
		RightBtn.spriteGroup.SetSelect(2, true);
		RightBtn.spriteGroup.SetSelect(3, false);
		HuaLingInfo.Show();
	}
	
	public void OnFirstOpenCharInfo(EEvent evt, params object[] args)
	{
		CS_FirstOpenRole.Builder builder = CS_FirstOpenRole.CreateBuilder();		
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_FirstOpenRole, builder.Build());
		if ( bShowCurrentUser )
			CurrPlayer = XLogicWorld.SP.MainPlayer;
	}  
	
    public void OnDynamicAttrUpdate(EEvent evt, params object[] args)
    {
		// 注意 args 长度为 3, 传过来的还包含数值, 虽然目前的机制没用到
        if (null == LogicUI || !LogicUI.gameObject.activeSelf || args.Length != 3) return;
		
		XCharacter ch = CurRoInfoSelChar;	
        EShareAttr att = (EShareAttr)args[1];
		
		if(ch == null)
			return ;

        if (!Enum.IsDefined(typeof(EShareAttr), att))
			return;
		
        switch (att)
        {
			case EShareAttr.esa_Level:
				LeftBtn.Show();
                break;
            case EShareAttr.esa_MaxHp:
                break;
            default:
                break;
        }
		
		if(AttrInfo.m_AttrLabelMap.Contains(att))
		{
	        UILabel xal = AttrInfo.m_AttrLabelMap[att] as UILabel;
			xal.text = ch.DynGet(att).ToString();
		}
    }

	 public void OnUpdateSprite(EEvent evt, params object[] args)
	 {
		if(LogicUI == null)
			return ;
		
		if ( !bShowCurrentUser )
			return;
			
		EItemBoxType srcType = (EItemBoxType)args[1];
		ushort		 srcIndex = (ushort)args[2];
		
		
		XItem srcItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)srcType, (short)srcIndex);
		if(srcItem == null)
			return;
		
		int EquipPos = srcIndex  % (int)EBAG_DATA.ROLE_MAX_EQUIP_NUM;
		
		if(EquipPos >= (int)EQUIP_POS.EQUIP_POS_FUYIN_START &&
			EquipPos <= (int)EQUIP_POS.EQUIP_POS_FUYIN_END)
		{
			Container.Show(3);
			
			int UIPos = EquipPos - (int)EQUIP_POS.EQUIP_POS_FUYIN_START;
			if(srcItem.IsEmpty())
			{			
				FuYinInfo.Show();
				return ;
			}
			
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(srcItem.DataID);
			if(cfgItem == null)
				 return;
			
			FuYinInfo.Show();
			
//			if(LogicUI != null)
//				FuYinInfo.mLogicActionIcon[UIPos].SetSprite(cfgItem.IconAtlasID, cfgItem.IconID,(EItem_Quality)cfgItem.QualityLevel,srcItem.ItemCount);
			
		}
		else
		{
			Container.Show(0);
			
			int UIPos = LogicRoleInfo.EquipPosToUIPos[EquipPos];
			if ( EquipPos == (int)EQUIP_POS.EQUIP_POS_FASHION && null != LogicUI)
				RoleInfo.UpdateFashionSelStatus(LogicUI.UIRoleInfoLeft);
			
			if (srcItem.IsEmpty())
			{
				if(LogicUI.UIRoleInfoLeft.ActionIconArray != null)
					LogicUI.UIRoleInfoLeft.ActionIconArray[UIPos].Reset();
				return ;
			}
	
			 XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(srcItem.DataID);
			 if (cfgItem == null)
				 return;
			
			if(LogicUI != null)
			{
			 	LogicUI.UIRoleInfoLeft.ActionIconArray[UIPos].SetSprite(cfgItem.IconAtlasID, cfgItem.IconID,srcItem.Color,srcItem.ItemCount);
				if(srcItem.mEquipAttr.mStrengthenLevel > 1)
				{
					LogicUI.UIRoleInfoLeft.ActionIconArray[UIPos].EnableEffect(true);
				}
			}
		}
	 }
}
