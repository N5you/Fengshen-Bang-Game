using System;
using UnityEngine;
using XGame.Client.Packets;
using System.Collections.Generic;
// 影响模型显示的元素
public enum EModelCtrlType
{
	eModelCtrl_ByBuff,		// buff影响模型
	eModelCtrl_ByFashion,	// 时装影响模型
	eModelCtrl_ByArmour,	// 装备影响模型
	eModelCtrl_Original,	// 原始模型
	eModelCtrl_End,
}

/* 类名: XCharacter
 * 描述: 客户端所有角色对象的基类
 * 功能:
 * 		1. 角色基础行为状态机: 待机/死亡/战斗/移动/跳跃/打坐
 * 		2. 角色基础属性与动态属性
 * 		3. 物品
 */
public class XCharacter : XGameObject
{
	public static readonly float DEFAULT_RUN_SPEED = 5.0f;

	public XCharacter (ulong id)
        : base(id)
	{
		ObjectType = EObjectType.Character;
		initBehaviourSM ();
		initItemMgr ();
		initBuffOper ();
		Speed = DEFAULT_RUN_SPEED;
	}

	~XCharacter ()
	{
		m_BehaviourSM = null;
		m_AttrCharacter = null;
		ItemManager = null;
		BuffOper = null;
	}

	public override void Breathe()
	{
		base.Breathe ();
		m_BehaviourSM.Breathe ();
		BuffOper.Breathe ();
	}

	// 将角色位置矫正到地面
	public void PushOnTerrain()
	{
		Vector3 posNow = Position;
		XLogicWorld.SP.SceneManager.FixExactHeight (ref posNow);
		Position = posNow;
	}

	public override void OnLevelLoaded(int nLevelId, ESceneType sceneType)
	{
		m_BehaviourSM.OnEvent ((int)EStateEvent.esLevelLoaded, nLevelId, sceneType);
	}

	public override void Appear()
	{
		if (IsAppear)
			return;
		base.Appear ();
		SendModelEvent (EModelEvent.evtTitle, Title);
		SendModelEvent (EModelEvent.evtNickName, NickName);
		SendModelEvent (EModelEvent.evtHp, Hp);
		SendModelEvent (EModelEvent.evtMaxHp, MaxHp);
		//SendModelEvent(EModelEvent.evtWeaponId, WeaponModelId);
		((XCharStateBase)m_BehaviourSM.CurrentState).OnAppear ();
		BuffOper.Appear ();
	}

	public override void DisAppear()
	{
		base.DisAppear ();
		BuffOper.Disappear ();
	}

    #region 角色基础行为状态机
	protected XStateMachince m_BehaviourSM;

	protected virtual void initBehaviourSM()
	{
		m_BehaviourSM = new XStateMachince (new XCharStateIdle (this));
		m_BehaviourSM.RegState (new XCharStateSit (this));
		m_BehaviourSM.RegState (new XCharStateDead (this));
		m_BehaviourSM.RegState (new XCharStateFight (this));
		m_BehaviourSM.RegState (new XCharStateMove (this));
	}

	// 线段移动
	public bool SegmentMoveTo(Vector3 toPos, float speed, XCharStateMove.OnMoveDone cb, EAnimName anim)
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esMoveTo, EProtoMoveType.eMoveType_Segment, toPos, speed, cb, anim);
	}

	public bool Rotato(Vector3 toPos)
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esRotateTo, toPos);
	}

	// 滑行
	public bool SlideMoveTo(Vector3 toPos, float dir, float speed, XCharStateMove.OnMoveDone cb)
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esMoveTo, EProtoMoveType.eMoveType_Slide, toPos, dir, speed, cb);
	}

	// 原地踏步
	public bool StepMove(float speed, float dir)
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esMoveTo, EProtoMoveType.eMoveType_Step, speed, dir);
	}

	// 停止移动(XZ)
	public bool StopMove()
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esStopMove);
	}

	// 从当前点起跳, 处理整个跳跃过程
	public bool Jump(XCharStateMove.OnJumpDone cb)
	{
		return Jump (Position.x, Position.z, 0f, cb);
	}

	// 起跳(给起跳点和跳跃时间, 忽略time之前的跳跃过程)
	public bool Jump(float x, float z, float time, XCharStateMove.OnJumpDone cb)
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esJump, x, z, time, cb);
	}

	public bool JumpOver()
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esJumpOver);
	}

	public void Idle()
	{
		m_BehaviourSM.TranslateToState((int)EStateId.esIdle);
	}

	public virtual void ForceSetPosition(Vector3 pos, Vector3 dir)
	{
		Position = pos;
		Direction = dir;
		m_BehaviourSM.OnEvent ((int)EStateEvent.esStopMove);
	}

	public virtual void OnEnterSit()
	{

	}

	public virtual void OnExitSit()
	{

	}

	// 进入生产, 高32位: 采集or制造, 低32位: 采集物ID or 配方ID
	public bool StartProduct(Int64 data)
	{
		if (0 == data)
			return false;

		EProductCareerType pType = (EProductCareerType)(data >> 32);

		int nId = (int)data;
		switch (pType) {
			case EProductCareerType.eProductCareerType_Gather:
				{
					XCfgGatherObject cfgGO = XCfgGatherObjectMgr.SP.GetConfig (nId);
					if (null == cfgGO)
						return false;
					XCfgProductCareer cfgPC = XCfgProductCareerMgr.SP.GetConfig (cfgGO.NeedCareer, 1);
					StartProduct ((EAnimName)cfgPC.AnimName);
				}
				break;

			case EProductCareerType.eProductCareerType_Produce:
				{
					return StartProduct (EAnimName.Idle);
				}
				break;

			default:
				break;
		}
		return false;
	}

	// 进入生产, 参数1: 生产动画 (生产状态嵌入在Idle里面)
	public bool StartProduct(EAnimName anim)
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esStartProduct, anim);
	}

	// 退出生产
	public bool QuitProduct()
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esQuitProduct);
	}

	// 进入战斗
	public bool StartFight()
	{
		if(m_BehaviourSM.OnEvent((int)EStateEvent.esStartFight))
		{
			SendModelEvent(EModelEvent.evtShowBlood, true);
			return true;
		}
		return false;
	}

	// 退出战斗
	public bool QuitFight()
	{
		if (m_BehaviourSM.OnEvent ((int)EStateEvent.esQuitFight)) {
			SendModelEvent (EModelEvent.evtShowBlood, false);
			return true;
		}
		return false;
	}

	// 播放动画
	public bool PlayAnimation(EAnimName anim)
	{
		return PlayAnimation (anim, 1.0f, false);
	}

	public bool PlayAnimation(EAnimName anim, float fSpeed)
	{
		return PlayAnimation (anim, fSpeed, false);
	}

	public bool PushAnimation(EAnimName anim)
	{
		return PlayAnimation (anim, 1.0f, true);
	}

	public bool PushAnimation(EAnimName anim, float fSpeed)
	{
		return PlayAnimation (anim, fSpeed, true);
	}

	public bool PlayAnimation(EAnimName anim, float fSpeed, bool bIsPush)
	{
		return m_BehaviourSM.OnEvent ((int)EStateEvent.esAnimation, anim, fSpeed, bIsPush);
	}

	// 直接播放动画, 这个接口非原创者不要调用 by WuZhenqiang
	public void _playAnimation(EAnimName anim, float fSpeed, bool bIsPush)
	{
		SendModelEvent (EModelEvent.evtAnimation, anim, fSpeed, bIsPush);
	}
    #endregion


    #region 物品=============================================================================================================================
	public XItemManager ItemManager { get; private set; }

	private void initItemMgr()
	{
		ItemManager = new XItemManager (this);
	}
	
	public bool CanSellItem(uint boxType, short pos)
	{
		return ItemManager.CanSellItem(boxType, pos);
	}
	
	public int GetItmeCount(uint itemId)
	{
		return ItemManager.GetItemByDataID(itemId);
	}
    #endregion================================================================================================================================

    #region Buff
	public XBuffOper BuffOper { get; private set; }

	private void initBuffOper()
	{
		BuffOper = new XBuffOper (this);
	}
    #endregion

    #region 角色属性
	protected XAttrCharacter m_AttrCharacter = new XAttrCharacter ();

	public float Speed {
		get { return m_AttrCharacter.Speed; }
		set {
			if (m_AttrCharacter.Speed != value) {
				float oldSpeed = Speed;
				m_AttrCharacter.Speed = value;
				m_BehaviourSM.OnEvent ((int)EStateEvent.esSpeed, Speed, oldSpeed);
			}
		}
	}

	public string Title {
		get { return m_AttrCharacter.Title; }
		set {
			if (m_AttrCharacter.Title != value) {
				m_AttrCharacter.Title = value;
				SendModelEvent (EModelEvent.evtTitle, value);
			}
		}
	}

	public virtual string NickName {
		get { return m_AttrCharacter.NickName; }
		set {
 			if (m_AttrCharacter.NickName != value) {
				m_AttrCharacter.NickName = value;
				SendModelEvent (EModelEvent.evtNickName, value);
			}
		}
	}

	public byte Sex {
		get { return m_AttrCharacter.Sex; }
		set {
			if (m_AttrCharacter.Sex != value) {
				m_AttrCharacter.Sex = value;
			}
		}
	}

	public int Hp {
		get { return m_AttrCharacter.Hp; }
		set 
		{
			if(m_AttrCharacter.Hp != value)
			{
				if (value < 0)
	            {
	                m_AttrCharacter.Hp = 0;
					BuffOper.OnDead();
					this.OnDead();
	            }
	            else if (value > MaxHp)
	            {
	                m_AttrCharacter.Hp = MaxHp;
	            }
	            else
	            {
	                m_AttrCharacter.Hp = value;
	            }
				
				SendModelEvent(EModelEvent.evtHp, m_AttrCharacter.Hp);
				//m_BehaviourSM.OnEvent((int)EStateEvent.esHp, m_AttrCharacter.Hp);
				XEventManager.SP.SendEvent(EEvent.Attr_Hp, this, m_AttrCharacter.Hp);
			}
		}
	}

	public int MaxHp {
		get { return DynGet (EShareAttr.esa_MaxHp); }
		set {
			if (MaxHp != value) {
				DynSet (EShareAttr.esa_MaxHp, value);
				SendModelEvent (EModelEvent.evtMaxHp, value);
			}
		}
	}

	public virtual int Level {
		get { return DynGet (EShareAttr.esa_Level); }
		set {
			if (Level != value) {
				DynSet (EShareAttr.esa_Level, value);
			}
		}
	}

	public virtual uint ArmourItemID 
	{
		get 
		{ 
			return m_AttrCharacter.ArmourItemID; 
		}
		set 
		{
			if (m_AttrCharacter.ArmourItemID != value) 
			{
				m_AttrCharacter.ArmourItemID = value;
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig (ArmourItemID);
				uint id = 0;
				if (cfgItem != null && 
					cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_ARMOR && 
					cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR) 
				{
					id = cfgItem.ArmourID;
				}
				if ( m_AttrCharacter.ShowFashion == 1 ||
					m_AttrCharacter.FashionId == 0 )
					SetModel (EModelCtrlType.eModelCtrl_ByArmour, id);
				XEventManager.SP.SendEvent (EEvent.Attr_ArmourItemID, this, value);
			}
		}
	}
	
	public virtual uint FashionId 
	{
		get 
		{ 
			return m_AttrCharacter.FashionId; 
		}
		set 
		{
			if (m_AttrCharacter.FashionId != value) 
			{
				m_AttrCharacter.FashionId = value;
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(FashionId);
				uint id = 0;
				if (cfgItem != null && 
					cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_ARMOR && 
					cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOP_FASHION) 
				{
					id = cfgItem.ArmourID;
				}
				if ( XLogicWorld.SP.MainPlayer.ShowFashion == 0 && id > 0 )
					SetModel (EModelCtrlType.eModelCtrl_ByFashion, id);
				else
					ArmourItemID = ArmourItemID;
				
				XEventManager.SP.SendEvent(EEvent.Attr_ArmourItemID, this, value);
			}
		}
	}
	
	public uint WeaponModelId
	{
		get { return m_AttrCharacter.WeaponModelID; }
		set
		{
			if(m_AttrCharacter.WeaponModelID != value)
			{
				m_AttrCharacter.WeaponModelID	= value;
				SendModelEvent(EModelEvent.evtWeaponId, value);
				XEventManager.SP.SendEvent(EEvent.Attr_WeaponItemID, this, value);
			}
			
		}
	}
	
	protected virtual void OnDead()
	{
		
	}

	public int DynGet(EShareAttr aIndex)
	{
		return m_AttrCharacter.DynamicAttrs.Get (aIndex);
	}

	public void DynSet(EShareAttr aIndex, int value)
	{
		if (DynGet (aIndex) != value) {
			m_AttrCharacter.DynamicAttrs.Set ((int)aIndex, value);
			XEventManager.SP.SendEvent (EEvent.Attr_Dynamic, this, aIndex, value);
		}
	}

	public void UpdateDynAttrs(IList<Msg_PairII> lst)
	{
		foreach (Msg_PairII info in lst) {
			this.DynSet ((EShareAttr)info.First, info.Second);
		}
	}
    #endregion

	public bool IsDead { get { return Hp <= 0; } }

	// 角色模型显示
	private EModelCtrlType m_eCurModelType = EModelCtrlType.eModelCtrl_Original;
	private uint[] m_ModelIdArr = new uint[(int)EModelCtrlType.eModelCtrl_End];

	public void SetModel(EModelCtrlType ct, uint modelId)
	{
		if (ct >= EModelCtrlType.eModelCtrl_End
            || m_ModelIdArr [(int)ct] == modelId)
			return;

		m_ModelIdArr [(int)ct] = modelId;
		if (ct < m_eCurModelType) {
			if (modelId > 0) {
				ModelId = modelId;
				m_eCurModelType = ct;
				return;
			}
		}
		else if (ct == m_eCurModelType) {
				if (modelId > 0) {
					ModelId = modelId;
					return;
				}
				else {
					for (EModelCtrlType i = m_eCurModelType + 1; i < EModelCtrlType.eModelCtrl_End; i++) {
						uint id = m_ModelIdArr [(int)i];
						if (id > 0) {
							ModelId = id;
							m_eCurModelType = i;
							return;
						}
					}
				}
			}

	}
	
}



/*public abstract class XCharacter : XLiveObject
{
    public XCharacter(ulong id) : base(id)
    {
        ObjectType = EObjectType.Character;
		DeadRockon = 0f;
		ItemManager= new XItemManager(this);
    }

	public override bool Breathe()
	{
        if (base.Breathe())
        {
            // 怪物死亡, 销毁
            if (IsDead && Time.time - DeadRockon > Define.DEAD_DESTROY_DELAY)
            {
                XLogicWorld.SP.ObjectManager.RemoveObject(ObjectType, ID);
            }
            return true;
        }
		return false;
	}

    public override void OnMouseUpAsButton()
    {
		base.OnMouseUpAsButton();
    }
	
	public XItemManager ItemManager {get;private set;}	
	
    #region Model About Functions
	// 显示血条
	public void ShowBlood()
	{
		if(!HasModel()) return;
		m_Model.ShowBlood();
	}
	
	// 隐藏血条
	public void HideBlood()
	{
		if(!HasModel()) return;
		m_Model.HideBlood();
	}	
	#endregion
	
	#region attr setting

    protected XAttrCharacter m_AttrCharacter = new XAttrCharacter();

    public int DynGet(EShareAttr aIndex)
    {
        return m_AttrCharacter.DynamicAttrs.Get(aIndex);
    }

    protected void DynSet(EShareAttr aIndex, int value)
    {
        m_AttrCharacter.DynamicAttrs.Set((int)aIndex, value);
		XEventManager.SP.SendEvent(EEvent.Attr_Dynamic, this, aIndex);
    }

    public void UpdateDynAttrs(IList<Msg_PairII> lst)
    {
		foreach (Msg_PairII info in lst)
		{
            this.DynSet((EShareAttr)info.First, info.Second);
		}
    }

    public virtual int Level
    {
        get { return DynGet(EShareAttr.esa_Level); }
        set
        {
            int preLevel = Level;
            DynSet(EShareAttr.esa_Level, value);

			//--4>TODO: 下面代码存在问题, 可能动态属性全部统一更新
			//	考虑取消这个这个属性的直接存取接口, 统一用动态属性处理?
            if (preLevel < Level)
            {
                // 播放特效
                EffectManager.SP.PlayLevelUpEffect(GetSkeleton(ESkeleton.eCapsuleHalf));
            }
        }
    }

    public virtual byte Sex
    {
        get { return m_AttrCharacter.Sex; }
        set { m_AttrCharacter.Sex = value; }
    }

    public virtual int Hp
    {
        get { return m_AttrCharacter.Hp; }
        set
        {
            if (value < 0)
            {
                m_AttrCharacter.Hp = 0;
            }
            else if (value > MaxHp)
            {
                m_AttrCharacter.Hp = MaxHp;
            }
            else
            {
                m_AttrCharacter.Hp = value;
            }
		
            if (Hp == 0 && !IsDead)	// 死亡
            {
				ToDead();
            }
            else if (Hp > 0 && IsDead) // 复活
            {
				ToRevive();
            }

			// 更新头顶面板
            if (HasModel())
            {
                m_Model.SetHp(Hp, MaxHp);
			}
			XEventManager.SP.SendEvent(EEvent.Attr_Hp, this, m_AttrCharacter.Hp);
        }
    }

    public virtual int MaxHp
    {
        get { return DynGet(EShareAttr.esa_MaxHp); }
        set
        {
            DynSet(EShareAttr.esa_MaxHp, value < 0 ? 0 : value);
			
			//--4>TODO: 下面代码存在问题, 可能动态属性全部统一更新
			//	考虑取消这个这个属性的直接存取接口, 统一用动态属性处理?
            if (m_AttrCharacter.Hp > MaxHp)
            {
                Hp = MaxHp;
            }
			
			// 更新头顶面板
            if (HasModel())
            {
                m_Model.SetHp(Hp, MaxHp);
			}
        }
    }

	#endregion
	
	#region Death About
	public float DeadRockon { get; private set; }
	public virtual bool IsDead {  get { return 0 != DeadRockon; } }	
	 
	protected virtual void ToDead()
	{
		DeadRockon = Time.time;
	}
	
	protected virtual void ToRevive()
	{
		DeadRockon = 0f;
	}
	#endregion
	
	public virtual void RefreshWeaponModel()
	{
		if (HasModel())
		{
			//挂接武器
			uint md = 0;
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(WeaponItemID);
			
			//临时显示自己的强化显示效果
			//--------------------------------------------------
			byte 	strengthenLevel = 0;
			uint   	colorLevel = 0;				
			XItem item = ItemManager.GetItem((uint)EItemBoxType.Equip,(short)EQUIP_POS.EQUIP_POS_WEAPON);
			if(item != null)
			{
				strengthenLevel	= (byte)item.mEquipAttr.mStrengthenLevel;
				colorLevel		= (uint)item.mEquipAttr.mColor;
			}
			//----------------------------------------------------
			
			
			if (cfgItem != null && cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_WEAPON && cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOT_WEAPON)
			{
				//临时显示自己的强化显示效果
				//--------------------------------------------------
               	int appearLevel = 0;
				XCfgStrengthen cfgStrengthen = XCfgStrengthenMgr.SP.GetConfig(colorLevel,strengthenLevel);
				if(cfgStrengthen != null)
					appearLevel	= cfgStrengthen.AppearLevel;
				//--------------------------------------------------
				uint tempModelID = XItemManager.GetArmourID(cfgItem.ArmourID,appearLevel,1,1);
				
				md = tempModelID;
			}

			if (0 == md)
			{
				m_Model.RemoveWeaponObject();
			}
			else
			{
				m_Model.SetWeaponModel(this, md);
			}
		}
	}

    public virtual uint ArmourItemID
    {
        get { return m_AttrCharacter.ArmourItemID; }
        set
        {
			//--4>: 直接刷新模型, 因为下线再上线时逻辑数据是一致的, 而模型必须重置
            //if (m_AttrCharacter.ArmourItemID != value)
            {
                m_AttrCharacter.ArmourItemID = value;

                // 刷新显示模型(铠甲模型或者角色裸模)
                XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(value);
                if (cfgItem != null && cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_ARMOR && cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR)
                {
                    ModelId = cfgItem.ArmourID;
                }
            }
        }
    }

    public virtual uint WeaponItemID
    {
        get { return m_AttrCharacter.WeaponItemID; }
        set
        {
			//--4>: 直接刷新模型, 因为下线再上线时逻辑数据是一致的, 而模型必须重置
            // if (m_AttrCharacter.WeaponItemID != value)
            {
                m_AttrCharacter.WeaponItemID = value;
                RefreshWeaponModel();
            }
        }
    }

}*/
