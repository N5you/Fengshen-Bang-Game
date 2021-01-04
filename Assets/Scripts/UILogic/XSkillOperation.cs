using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XSkillOperation")]
public class XSkillOperation : XDefaultFrame
{
	private static readonly int UniqueSkillSum = 7;
	private static uint NormalEffect 	= 900032;
	private static uint SelEffect 		= 900030;
	private static uint ActiveEffect 	= 900030;
	private static uint LearnEffect		= 900060;
	private static uint EquipEffect 		= 900034;

	
	public UIImageButton	LevelUpBtn;
	public UIImageButton	ResetBtn;
	
	public static int FreeLevel = 20;
	public static int CostRealMoney = 100;
	
	public class XSkillIcon
	{
		public static string[]  PassivityNotLearn =  {"","11001013","11001015","11001011"};
		public static string[]  PassivityCanLearn =  {"","11001013","11001015","11001011"};
		public static string[]  PassivityLearn    =  {"","11001012","11001014","11001010"};
		
		public static string[]   SkillNameBack   = {"","11001017","11001018","11001016"};
		public static string[]   SkillIconBack   = {"","11001008","11001020","11001007"};
		
		public static string   EquipBtnNor   = "11001004";
		public static string   EquipBtnHov   = "11001005";
		public static string   EquipBtnPre  =   "11001006";
		
		public static string   EquipSprite  =   "11001000";
		
		protected ushort m_wSkillId;
		public UISprite m_SkillIcon;
		protected ESkill_State	mState;
		protected XU3dEffect mNormalEffect	= null;
		protected XU3dEffect mEquipEffect		= null;
		protected XU3dEffect mLearnEffect	= null;
		//protected XU3dEffect mSelEffect		= null;
		protected GameObject	mParent;
		
		public XSkillIcon(ushort skillId, UISprite skillIcon,GameObject parent)
		{
			m_wSkillId 	= skillId;
			mParent		= parent;
			if(null != skillIcon)
			{
				m_SkillIcon = skillIcon;
				UIEventListener listen = UIEventListener.Get(m_SkillIcon.gameObject);
				listen.onHover += OnHover;
				listen.onClick += OnClick;
			}
		}
		
		protected void CreateEffect(ref XU3dEffect effectObj,uint effectID)
		{
			if(mParent == null)
				return ;
			
			effectObj 					= new XU3dEffect(effectID,EffectLoadedHandle);
			effectObj.Layer 			= GlobalU3dDefine.Layer_UI_2D;			
			return ;
		}
		
		protected void CreateEffect2(ref XU3dEffect effectObj,uint effectID)
		{
			if(mParent == null)
				return ;
			
			effectObj 					= new XU3dEffect(effectID,EffectLoadedHandle2);
			effectObj.Layer 			= GlobalU3dDefine.Layer_UI_2D;			
			return ;
		}
		
		private void EffectLoadedHandle(XU3dEffect effect)
		{
			effect.Parent			= mParent.transform;
			effect.LocalPosition 	= new Vector3(m_SkillIcon.transform.parent.localPosition.x,m_SkillIcon.transform.parent.localPosition.y,-30f);
			effect.Scale			= new Vector3(500,500,1);
		}
		
		private void EffectLoadedHandle2(XU3dEffect effect)
		{
			effect.Parent			= mParent.transform;
			effect.LocalPosition 	= new Vector3(m_SkillIcon.transform.parent.localPosition.x,m_SkillIcon.transform.parent.localPosition.y,-30f);
			effect.Scale			= new Vector3(1,1,1);
		}
		
		protected void SetSprite(UIAtlas atlas, string strSpriteName)
		{
			if(null == m_SkillIcon) return;
			m_SkillIcon.atlas = atlas;
			m_SkillIcon.spriteName = strSpriteName;
		}
		
		public virtual void RefreshSprite()
		{
			if(null == m_SkillIcon) return;
			XSkillOper skillOper = SkillManager.SP.GetSkillOper(m_wSkillId, 1);
			if(null == skillOper) return;			
			XUIDynamicAtlas.SP.SetSprite(m_SkillIcon, (int)skillOper.AtlasID, skillOper.SpriteName,true,OnSpriteLoadDone);
			
		}
		
		public void OnSpriteLoadDone()
		{
			NGUITools.AddWidgetCollider(m_SkillIcon.gameObject,true);
		}
		
		private string GetLevelUpCondition(ushort id, byte level)
		{
			string str = "[color=ffffff]";
			XSkillOper skillOper = SkillManager.SP.GetSkillOper(id, level);
			str += XStringManager.SP.GetString(6);
			if(XLogicWorld.SP.SkillManager.m_uSkillPoint < skillOper.SkillPoint)
				str += "[color=ff0000]" + skillOper.SkillPoint.ToString() + "\n";
			else
				str	+= "[color=00ff00]" + skillOper.SkillPoint.ToString() + "\n";
			
			XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(id);  
			
			str += "[color=ffffff]" + XStringManager.SP.GetString(69);
			uint learnLevel = skillDef.GetLearnLevel(level);
			if(XLogicWorld.SP.MainPlayer.Level < learnLevel)
				str += "[color=ff0000]" + learnLevel.ToString() + "\n";
			else
				str	+= "[color=00ff00]" + learnLevel.ToString() + "\n";
			
			return str;
		}
		
		
		public virtual void ShowIconToolTip()
		{
				byte level = SkillManager.SP.GetActiveSkill(m_wSkillId);
				if( level == 0)
					level = 1;
				
				XSkillOper skillOper = SkillManager.SP.GetSkillOper(m_wSkillId, level);
				if(null == skillOper) return;
				
				// Tooltip 显示的文字生成
				string strTitle = "[color=00ff00]";
				XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(m_wSkillId);
				strTitle += skillDef.Name;
				
				string strTip = "";
				string strLevel = "";
				
				byte nowLevel = SkillManager.SP.GetActiveSkill(m_wSkillId);
				if(0 == nowLevel)
				{
					strLevel = XStringManager.SP.GetString(2) + ": 1\n";
					strTip += "[color=ffffff]" + XStringManager.SP.GetString(68) + skillDef.GetAnger(level)+ "\n";
					strTip += "[color=f89a4b]" +  skillDef.Levels[1].TextEffect + "\n";
					strTip += GetLevelUpCondition(m_wSkillId, 1) + "\n";
				}
				else
				{
					strLevel = XStringManager.SP.GetString(1) + ": " + nowLevel + "\n";
					strTip += "[color=ffffff]" + XStringManager.SP.GetString(68) + skillDef.GetAnger(level)+ "\n";
					strTip += "[color=f89a4b]" +  skillDef.Levels[level].TextEffect + "\n";				
					
					
					byte nextLevel = (byte)(nowLevel + 1);
					if(nextLevel > skillDef.Levels.Count)
					{
						strTip += "[color=ffffff]" + XStringManager.SP.GetString(5);
					}
					else
					{
						strTip += "[color=ffffff]" + XStringManager.SP.GetString(3) + "             " + XStringManager.SP.GetString(68) + skillDef.GetAnger((byte)(level + 1))+ "\n";
						//strTip += XStringManager.SP.GetString(2) + ": " + (nowLevel + 1) + "\n";
						strTip +=  "[color=ff5400]" +  skillDef.Levels[nextLevel].TextEffect + "\n";
						strTip += GetLevelUpCondition(m_wSkillId, nextLevel);
					}		
				}				
				XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eToolTipB);
				XEventManager.SP.SendEvent(EEvent.ToolTip_B,strTitle,strLevel,strTip);
		}
		
		protected virtual void OnHover(GameObject go, bool bHover)
		{
			if(bHover)
			{
				ShowIconToolTip();
			}
			else
			{
				XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eToolTipB);
			}
		}
		
		protected virtual void OnClick(GameObject go)
		{
		}
	}
	
	public class XSkillStar : XSkillIcon
	{
		//private UIAtlas m_DefaultAtlas ;
		private XSkillOperation	mRoot;
		//主动技能 的描述
		private GameObject		mObjSkill_Desc;
		private UILabel				mLabSkill_Name;
		private UILabel				mLabSkill_Level;
		private UIImageButton	mBtnSkill_Equip;
		private UISprite               mSprite_Equip;
		private bool						mIsPassivity = false;
		
		
		public XSkillStar(uint AtlasID, ushort skillId, UISprite skillIcon,GameObject go,XSkillOperation root,GameObject SkillTemp) : base(skillId, skillIcon,go)
		{
			//m_DefaultAtlas 	= atlas;
			mRoot			=  root;
			
			mObjSkill_Desc =  SkillTemp.transform.Find("Icon_Desc").gameObject;
			mLabSkill_Name = mObjSkill_Desc.transform.Find("Label_SkillName").GetComponent<UILabel>();
			mLabSkill_Level  = mObjSkill_Desc.transform.Find("Label_SkillLevel").GetComponent<UILabel>();
			mBtnSkill_Equip  = mObjSkill_Desc.transform.Find("Image Button").GetComponent<UIImageButton>();
			//mBtnSkill_Equip.gameObject.SetActive(true);
			mBtnSkill_Equip.Hide();
			mBtnSkill_Equip.onHide += hideEquipButtion;
			
			XMainPlayer playSelf = XLogicWorld.SP.MainPlayer;
			int playerClass = playSelf.DynGet(EShareAttr.esa_Class);
			UISprite  SprIconBac = mObjSkill_Desc.transform.Find("Sprite_IconBac").GetComponent<UISprite>();	
			if(null != SprIconBac)
			{
				XUIDynamicAtlas.SP.SetSprite(SprIconBac,(int)AtlasID,XSkillIcon.SkillIconBack[playerClass],true,null);
			}
			UISprite NameBac = mObjSkill_Desc.transform.Find("Sprite_ZiDiTu").GetComponent<UISprite>();
			if(null != NameBac)
			{
				XUIDynamicAtlas.SP.SetSprite(NameBac,(int)AtlasID,XSkillIcon.SkillNameBack[playerClass],true,null);
			}
			
			mSprite_Equip = mObjSkill_Desc.transform.Find("Sprite_Equip").GetComponent<UISprite>();
			if(null != mSprite_Equip)
			{
				XUIDynamicAtlas.SP.SetSprite(mSprite_Equip,(int)AtlasID,XSkillIcon.EquipSprite,true,null);
				mSprite_Equip.enabled = false;
			}
			if(null != mBtnSkill_Equip)
			{
				XUIDynamicAtlas.SP.SetSprite(mBtnSkill_Equip.target,(int)AtlasID,XSkillIcon.EquipBtnNor,true,null);
				mBtnSkill_Equip.normalSprite = XSkillIcon.EquipBtnNor;
				mBtnSkill_Equip.hoverSprite = XSkillIcon.EquipBtnHov;
				mBtnSkill_Equip.pressedSprite= XSkillIcon.EquipBtnPre;
				
				mBtnSkill_Equip.Hide();
				UIEventListener listen = UIEventListener.Get(mBtnSkill_Equip.gameObject);
				listen.onClick += OnClickEquip;
				listen.onHover+= OnHoverEquip;
			}
			
			XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(m_wSkillId);
			if(null != skillDef)
			{
				mLabSkill_Name.text = skillDef.Name;
				UpdateLevel();
				mIsPassivity = (ESkillFuncType.eSFuncType_Passivity == skillDef.FuncType ? true : false);
			}
			
			if(null != m_SkillIcon)
			{
				RefreshSprite();
			}
			
			if(!mIsPassivity && mState == ESkill_State.ESkill_State_Learn)
			{
				if( SkillManager.SP.m_uSkillEquip == 0 )
				{
					showEquipButtion();
				}
			}
		}
		
		public void RefreshState()
		{
			if(mBtnSkill_Equip != null)
				mBtnSkill_Equip.Hide();
		}
		public override void RefreshSprite()
		{
			XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(m_wSkillId);
			if(null == skillDef) return;
			mState	= SkillManager.SP.GetSkillState(m_wSkillId);
			
			if(null == m_SkillIcon) return;				
			XSkillOper skillOper = SkillManager.SP.GetSkillOper(m_wSkillId, 1);
			if(null == skillOper) return;
			
			XMainPlayer playSelf = XLogicWorld.SP.MainPlayer;
			int playerClass = playSelf.DynGet(EShareAttr.esa_Class);
			if(playSelf == null ) return;
				
			
			mObjSkill_Desc.SetActive(false);
			if(mIsPassivity)
			{
				//必须在一个图集里，现在一个控件的图片可能出现在多个图集中，没法处理	
				if(mState == ESkill_State.ESkill_State_Not_Learn)					
				{
					if(mNormalEffect != null)
						mNormalEffect.Destroy();
					XUIDynamicAtlas.SP.SetSprite(m_SkillIcon, (int)skillOper.AtlasID,XSkillIcon.PassivityNotLearn[playerClass],true,OnSpriteLoadDone);
				}					
				else if(mState == ESkill_State.ESkill_State_Can_Learn)
				{
					if(mNormalEffect != null)
						mNormalEffect.Destroy();
					CreateEffect(ref mNormalEffect,NormalEffect);
					XUIDynamicAtlas.SP.SetSprite(m_SkillIcon, (int)skillOper.AtlasID,XSkillIcon.PassivityCanLearn[playerClass],true,OnSpriteLoadDone);
				}
				else
				{
					if(mNormalEffect != null)
						mNormalEffect.Destroy();
					
					//CreateEffect(mNormalEffect,ActiveEffect);
					XUIDynamicAtlas.SP.SetSprite(m_SkillIcon, (int)skillOper.AtlasID,XSkillIcon.PassivityLearn[playerClass],true,OnSpriteLoadDone);
				}
			}
			else
			{
				if(mNormalEffect != null)
					mNormalEffect.Destroy();		
				if(mState == ESkill_State.ESkill_State_Not_Learn)
				{
					XUIDynamicAtlas.SP.SetSprite(m_SkillIcon, (int)skillOper.AtlasID,skillOper.NotLearnSpriteName,true,OnSpriteLoadDone);		
				}
				else if(mState == ESkill_State.ESkill_State_Can_Learn)
				{	
					CreateEffect(ref mNormalEffect,NormalEffect);
					XUIDynamicAtlas.SP.SetSprite(m_SkillIcon, (int)skillOper.AtlasID,skillOper.NotLearnSpriteName,true,OnSpriteLoadDone);
				}					
				else
				{
					mObjSkill_Desc.SetActive(true);
					base.RefreshSprite();
				}					
			}
		}
		
		protected override void OnHover(GameObject go, bool bHover)
		{
			if(!mIsPassivity && mState == ESkill_State.ESkill_State_Learn)
			{
				if( (bHover && SkillManager.SP.m_uSkillEquip != m_wSkillId) ||
					 SkillManager.SP.m_uSkillEquip == 0 )
				{
					showEquipButtion();
				}
				else
				{
					mBtnSkill_Equip.Invoke("Hide", 2);
				}
			}
			base.OnHover(go, bHover);
		}
		
		public void showEquipButtion()
		{
			if(mRoot)
				mRoot.RefreshIconState();
			mBtnSkill_Equip.Show();
			mBtnSkill_Equip.CancelInvoke("Hide");
			
			GameObject obj = mBtnSkill_Equip.transform.parent.gameObject;
			Vector3 showPos = new Vector3(140, 40, 0);
			
			if ( (10001u == m_wSkillId || 20001u == m_wSkillId || 30001u == m_wSkillId) && obj.activeSelf  )
			{
				XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_Skill_Use, 2, showPos, obj);
			}
		}
		
		public void hideEquipButtion()
		{
			//mBtnSkill_Equip.target.enabled = false;
			mBtnSkill_Equip.gameObject.SetActive(false);
			//mBtnSkill_Equip.Hide();
		}
		
		public IEnumerator EquipBtn()
		{
			yield return new WaitForSeconds(2);
			mBtnSkill_Equip.Hide();
		}
		
		public ushort GetSkillId()
		{
			return m_wSkillId;
		}
		
		protected override void OnClick(GameObject go)
		{
			if(mRoot)
			{
				mRoot.OnSkillSel(this);
			
				XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(m_wSkillId);
				if(null == skillDef) return;
				byte nowLevel = SkillManager.SP.GetActiveSkill(m_wSkillId);
				if((int)nowLevel >= skillDef.LevelLimit)
					return;
				mRoot.ImproveSkill(go);
			}
			
			//技能界面更换  新手提示
			if ( 10001u == m_wSkillId || 20001u == m_wSkillId || 30001u == m_wSkillId  )
			{
				XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_Skill_Select);
			}
			
			//XEventManager.SP.SendEvent(EEvent.Skill_ImproveSkill, m_wSkillId);
		}
		
		void OnClickEquip(GameObject go)
		{
			XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_Skill_Use);
			
			XEventManager.SP.SendEvent(EEvent.Skill_EquipSkill, m_wSkillId);
			mBtnSkill_Equip.Hide();
		}
		
		void OnHoverEquip(GameObject go,bool bHover)
		{
			if(bHover || SkillManager.SP.m_uSkillEquip == 0 )
			{
				mBtnSkill_Equip.CancelInvoke("Hide");
			}
			else
			{
				mBtnSkill_Equip.Invoke("Hide", 2);
			}
		}
		public void OnSelect(bool isSel)
		{
			//现在选中不用特效了
//			if(isSel)
//			{
//				if(mSelEffect != null)
//					mSelEffect.Destroy();
//			
//				CreateEffect(ref mSelEffect,SelEffect);
//			}
//			else
//			{
//				if(mSelEffect != null)
//					mSelEffect.Destroy();
//			}
		}
		
		public void OnEquip(bool isEquip)
		{
			mSprite_Equip.enabled = isEquip;
			if(isEquip)
			{
				if(mEquipEffect != null)
					mEquipEffect.Destroy();
				CreateEffect(ref mEquipEffect,EquipEffect);
			}
			else
			{
				if(mEquipEffect != null)
					mEquipEffect.Destroy();
			}
		}	
		public void OnLearn()
		{
			if(mIsPassivity == false)
			{
				if(mLearnEffect != null)
					mLearnEffect.Destroy();
				CreateEffect2(ref mLearnEffect,LearnEffect);
			}
			UpdateLevel();
			if(mBtnSkill_Equip != null && UICamera.hoveredObject == m_SkillIcon.gameObject)
			{
				showEquipButtion();
			}
			RefreshSprite();
		}
		public void OnInit()
		{
			UpdateLevel();
			if(mBtnSkill_Equip != null && UICamera.hoveredObject == m_SkillIcon.gameObject)
			{
				showEquipButtion();
			}
			RefreshSprite();
		}
		
		public void OnUpgrade()
		{
			if(mLearnEffect != null)
				mLearnEffect.Destroy();
			CreateEffect2( ref mLearnEffect,LearnEffect);
			UpdateLevel();
		}
		
		public void UpdateLevel()
		{
			byte SkillLevel = SkillManager.SP.GetActiveSkill(m_wSkillId);
			mLabSkill_Level.text = SkillLevel.ToString();
		}
		public void OnForget()
		{			
			RefreshSprite();
		}
	}
	
	public class XUniqueSkill : XSkillIcon
	{
		//private bool m_bLocked = true;
		
		public XUniqueSkill(UIAtlas AtlasID, ushort skillId, UISprite skillIcon,GameObject go) : base(skillId, skillIcon,go)
		{
			Disable();
		}
		
		public void Disable()
		{
			//SetSprite(mLockAtlas, "11399999");
			if(m_SkillIcon != null)
			{
				m_SkillIcon.atlas	= null;
			}
			//m_bLocked = true;
		}
		
		public void Enable()
		{
			RefreshSprite();
			//m_bLocked = false;
		}
		
		protected override void OnHover(GameObject go, bool bHover)
		{
			//if(m_bLocked) return;
			base.OnHover(go, bHover);
		}
		
		protected override void OnClick(GameObject go)
		{
			//if(m_bLocked) return;
			XEventManager.SP.SendEvent(EEvent.Skill_EquipSkill, m_wSkillId);
		}
	}
	
	public class XEquipSkill : XSkillIcon
	{
		private UIAtlas mSkillAtlas;
		
		public XEquipSkill(ushort skillId, UISprite skillIcon,GameObject go) : base(skillId, skillIcon,go)
		{

		}
		
		public ushort GetSkillId()
		{
			return m_wSkillId;
		}
		
		public void OnEquip(ushort skillId)
		{
			m_wSkillId = skillId;
			RefreshSprite();
		}
		
		public void OnUnEquip()
		{
			m_wSkillId = 0;
			SetSprite(null, "");
		}
	}
	
	
	public class XSuperSkill : XSkillIcon
	{
		public XSuperSkill(ushort skillId, UISprite skillIcon,GameObject go) : base(skillId, skillIcon,go)
		{
			if(null != m_SkillIcon)
			{
				RefreshSprite();
			}
		}
		public override void ShowIconToolTip()
		{
			byte Level = 1;
			XSkillOper skillOper = SkillManager.SP.GetSkillOper(m_wSkillId, Level);
			if(null == skillOper) return;
			
			// Tooltip 显示的文字生成
			string strTitle = "[color=00ff00]";
			XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(m_wSkillId);
			strTitle += skillDef.Name;
			
			string strTip = "";

			strTip += "[color=f89a4b]" +  skillDef.Levels[Level].TextEffect + "\n";				
			
			XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eToolTipB);
			XEventManager.SP.SendEvent(EEvent.ToolTip_B,strTitle,"",strTip);
		}
	}
	
	
	public UIAtlas	CommonAtlas;				// 公用图集
	public UILabel 	SkillPoint;					// 当前技能点
	//public UISprite SkillIconSample;			// 技能星图样本
	public GameObject SkillIconSample;			// 技能星图样本
	public UISprite SkillBackground;			// 技能星图背景
	// 绝技技能栏的技能图标们
	public UISprite[] UniqueSkillIcons = new UISprite[UniqueSkillSum];
	public UISprite EquipSkillIcon;				// 已装备技能的图标
	public GameObject SkillMapGO;
	private ushort mCurSelSkill = 0;
	
	private SortedList<ushort, XSkillStar> m_SkillStars = new SortedList<ushort, XSkillStar>();
	//private SortedList<ushort, XUniqueSkill> m_UniqueSkills = new SortedList<ushort, XUniqueSkill>();
	
	//private XEquipSkill	m_EquipSkill;
	
	private	XSuperSkill	 m_SuperSkill;
	
	public override bool Init()
	{
		base.Init();
		
		XMainPlayer playSelf = XLogicWorld.SP.MainPlayer;
		int playerClass = playSelf.DynGet(EShareAttr.esa_Class);
		XCfgPlayerBase playerBase = XCfgPlayerBaseMgr.SP.GetConfig((byte)playerClass);
		if(playerBase != null)
		{
			m_SuperSkill =  new XSuperSkill(playerBase.SuperSkill, EquipSkillIcon,null);
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		//m_EquipSkill = new XEquipSkill(0, EquipSkillIcon,null);

		//更换技能界面                                 技能升级 技能重置
		//UIEventListener l1 = UIEventListener.Get(LevelUpBtn.gameObject);
		//l1.onClick	+= OnSkillLevelUp;
		
		//UIEventListener l2 = UIEventListener.Get(ResetBtn.gameObject);
		//l2.onClick	+= OnSkillReset;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eSkillOpertation);
	}
	
	public override void Show()
	{
		base.Show();
		//SkillIconSample.gameObject.SetActive(false);
		
		DoNewPlayerGuide(XNewPlayerGuideManager.GuideType.Guide_Skill_Select);
	}
	
	public void SetBKSprite(int atlasID, string spriteName)
	{
		if(SkillBackground == null)
			return ;
		XUIDynamicAtlas.SP.SetSprite(SkillBackground, (int)atlasID, spriteName, true, null);
	}
	
	public void ReflashUI()
	{
		foreach(KeyValuePair<ushort, XSkillStar> index in m_SkillStars)
		{
			XSkillStar star = index.Value;
			if(star != null)
				star.RefreshSprite();
		}
	}
	
	public void RefreshIconState()
	{
		foreach(KeyValuePair<ushort, XSkillStar> index in m_SkillStars)
		{
			XSkillStar star = index.Value;
			if(star != null)
				star.RefreshState();
		}
	}
	public void OnAddSkillOperConfig(XSkillOper skillOper,ESkill_State state)
	{
		if(null == skillOper) return;
		if(m_SkillStars.ContainsKey(skillOper.ID)) return;
		
		XSkillDefine skillDef = SkillManager.SP.GetSkillDefine(skillOper.ID);
		if(null == skillDef) return;
		if(skillDef.FuncType == ESkillFuncType.eSFuncType_SuperKill)
			return;

		SkillIconSample.SetActive(false);
		GameObject SkillIconObj = XUtil.Instantiate(SkillIconSample) as GameObject;
		
		Vector3 vec = SkillIconObj.transform.localPosition;
		vec.x = Mathf.RoundToInt(SkillBackground.transform.localScale.x * skillOper.PosX / 100f ) - 3;
		vec.y = Mathf.RoundToInt(SkillBackground.transform.localScale.y * skillOper.PosY / 100f ) - 5;
		SkillIconObj.transform.localPosition 	= vec;
		
		UISprite skillIcon = SkillIconObj.transform.Find("Sprite_SkillIcon").GetComponent<UISprite>();
		XSkillStar newStar = new XSkillStar(skillOper.AtlasID,skillOper.ID,skillIcon,SkillMapGO,this,SkillIconObj);
		m_SkillStars.Add(skillOper.ID, newStar);

//		if(ESkillFuncType.eSFuncType_Unique == skillDef.FuncType)
//		{
//			if(!m_UniqueSkills.ContainsKey(skillOper.ID))
//			{
//				m_UniqueSkills.Add(skillOper.ID, new XUniqueSkill(CommonAtlas, skillDef.ID, UniqueSkillIcons[skillOper.FieldID - 1],null));
//			}
//		}
		NGUITools.SetActiveSelf(SkillIconObj,true);
	}
	
	public void OnInitSkill(ushort wSkillId, byte byteLevel)
	{
		if(m_SkillStars.ContainsKey(wSkillId)) 
		{
			m_SkillStars[wSkillId].OnInit();
		}
	}
	public void OnLearnSkill(ushort wSkillId, byte byteLevel)
	{
		if(m_SkillStars.ContainsKey(wSkillId)) 
		{
			m_SkillStars[wSkillId].OnLearn();
		}
		
//		if(m_UniqueSkills.ContainsKey(wSkillId))
//		{
//			m_UniqueSkills[wSkillId].Enable();
//		}
	}
	
	public void OnUpgradeSkill(ushort wSkillId, byte byteLevel)
	{
		if(!m_SkillStars.ContainsKey(wSkillId)) 
			return;
		m_SkillStars[wSkillId].OnUpgrade();		
	}
	
	public void OnForgetSkill(ushort wSkillId)
	{
		if(m_SkillStars.ContainsKey(wSkillId)) 
			m_SkillStars[wSkillId].OnForget();
		
//		if(m_UniqueSkills.ContainsKey(wSkillId))
//			m_UniqueSkills[wSkillId].Disable();
		
//		if(wSkillId == m_EquipSkill.GetSkillId())
//			m_EquipSkill.OnUnEquip();
	}
	
	public void OnEquipSkill(ushort wSkillId)
	{
		//m_EquipSkill.OnEquip(wSkillId);
		foreach(KeyValuePair<ushort, XSkillStar> temp in m_SkillStars)
		{
			if(temp.Value.GetSkillId() == wSkillId)
			{
				temp.Value.OnEquip(true);
			}
			else
			{
				temp.Value.OnEquip(false);
			}
		}
	}
	
	public void OnSkillPoint(uint uSkillPoint)
	{
		//SkillPoint.text = string.Format(XStringManager.SP.GetString(18), uSkillPoint);
		SkillPoint.text = uSkillPoint.ToString();
		ReflashUI();
	}
	
	public void OnSkillSel(XSkillStar star)
	{
		foreach(KeyValuePair<ushort, XSkillStar> temp in m_SkillStars)
		{
			if(temp.Value == star)
			{
				mCurSelSkill	= temp.Key;
				temp.Value.OnSelect(true);
			}
			else
			{
				temp.Value.OnSelect(false);
			}
		}
	}
	
	public void OnSkillLevelUp(GameObject go)
	{
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(ImproveSkill);
		
		XSkillDefine skillDefine = XLogicWorld.SP.SkillManager.GetSkillDefine(mCurSelSkill);
		if(skillDefine == null)
			return ;
		
		ushort skillLevel = XLogicWorld.SP.SkillManager.GetActiveSkill(mCurSelSkill);
		if(skillLevel == 0)
			skillLevel	= 1;
		
		XSkillOper skillOper = XLogicWorld.SP.SkillManager.GetSkillOper(mCurSelSkill,(byte)skillLevel);
		if(skillOper == null)
			return ;
		
		string content = "[color=ffffff]";
		content += string.Format(XStringManager.SP.GetString(74),skillDefine.Name,skillOper.SkillPoint);
		XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,content);
	}
	
	public void ImproveSkill(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.Skill_ImproveSkill, mCurSelSkill);
	}
	
	public void OnSkillReset(GameObject go)
	{
		UIEventListener.VoidDelegate	funcOK = new UIEventListener.VoidDelegate(OnClickOK);
		string content = "";
		if(XLogicWorld.SP.MainPlayer.Level <= FreeLevel)		
			content = string.Format(XStringManager.SP.GetString(70),FreeLevel);
		else
			content = string.Format(XStringManager.SP.GetString(71),CostRealMoney);
				
		XEventManager.SP.SendEvent(EEvent.MessageBox,funcOK,null,content);		
	}
	
	private void OnClickOK(GameObject go)
	{
		CS_Empty.Builder	builder = CS_Empty.CreateBuilder();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_ResetSkill,builder.Build());
	}
	
	public void DoNewPlayerGuide(XNewPlayerGuideManager.GuideType type)
	{
		ushort startSkillPos = 0;
		switch ( (EShareClass)XLogicWorld.SP.MainPlayer.DynGet(EShareAttr.esa_Class) )
		{
		case EShareClass.eshClass_ZhanShi:
			startSkillPos = 10001;
			break;
					
		case EShareClass.eshClass_GongJianShou:
			startSkillPos = 20001;
			break;
					
		case EShareClass.eshClass_FaShi:
			startSkillPos = 30001;
			break;
		
		default:
			break;
		}
		if ( 0 >= startSkillPos || !m_SkillStars.ContainsKey(startSkillPos) )
			return;
		
		XSkillStar skill = m_SkillStars[startSkillPos];
		GameObject obj = skill.m_SkillIcon.gameObject;
		Vector3 showPos = new Vector3(Mathf.Round(obj.transform.localPosition.x - 140) , 
			Mathf.Round(obj.transform.localPosition.y + 10),
			Mathf.Round(obj.transform.localPosition.z));
		
		XNewPlayerGuideManager.SP.handleShowGuide2((int)type, 1, showPos, obj.transform.parent.gameObject);
	}
	
}
