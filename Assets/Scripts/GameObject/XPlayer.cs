using System;
using UnityEngine;
using XGame.Client.Packets;
using Google.ProtocolBuffers;

/* 类名: XPlayer
 * 描述: 客户端所有玩家对象的基类
 * 功能:
 *		1. 维护玩家基础属性
 *		2. 响应服务器过来的消息 开始移动.停止移动.跳跃.坐下
 */

public class XPlayer : XCharacter
{
	public XPlayer(ulong id)
		: base(id)
	{
		ObjectType = EObjectType.OtherPlayer;
		IsEnableHover	= true;
	}
	
	~XPlayer()
	{
		m_AttrPlayer = null;
	}
	
	public override void Appear()
	{
		base.Appear();		
		SendModelEvent(EModelEvent.evtWeaponId, WeaponModelId);
		SendModelEvent(EModelEvent.evtUColor, UColor);
	}
	
	public override void DisAppear ()
	{
		base.DisAppear ();
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eTargetInfo);
	}
	
	public  override void OnModelLoaded()
	{
		base.OnModelLoaded();
		
		m_ObjectModel.ReAttachMount();
		m_ObjectModel.AddBehaviourListener(EBehaviourType.e_BehaviourType_OtherPlayer,this);
	}
	
	public override EHeadBoard_Type GetHeadBoardType() { return EHeadBoard_Type.EHeadBoard_Type_Player; }
	
	public override void SetAppearData (object data)
	{
        SC_PlayerAppearData msg = data as SC_PlayerAppearData;
        if (null == msg)
        {
            return;
        }

        //--4>TODO: 考虑同步数据进行分类, 使用公用结构, 给子类调用
        Vector3 pos = Position;
        Version = msg.Version;
        if (msg.HasName) Name = msg.Name;
        if (msg.HasTitle) Title = msg.Title;
        if (msg.HasPosX) pos.x = msg.PosX;
        if (msg.HasPosZ) pos.z = msg.PosZ;
		Position = pos;
		PushOnTerrain();
		if (msg.HasDirection) Direction = new Vector3(0, msg.Direction, 0);
        if (msg.HasModel) SetModel(EModelCtrlType.eModelCtrl_Original, msg.Model);
        if (msg.HasColor) UColor = msg.Color;
        if (msg.HasSex) Sex = (byte)msg.Sex;
        if (msg.HasClass) DynSet(EShareAttr.esa_Class, (byte)msg.Class);
        if (msg.HasClassLevel) ClassLevel = msg.ClassLevel;
        if (msg.HasLevel) Level = (int)msg.Level;
        if (msg.HasRunSpeed) Speed = msg.RunSpeed;
        if (msg.HasMountIndex) m_AttrPlayer.MountIndex = (ushort)msg.MountIndex;
        if (msg.HasArmourItemID) 
			ArmourItemID = msg.ArmourItemID;
		else
			ArmourItemID = 0;
		
        if (msg.HasWeaponItemID) 
			WeaponItemID = msg.WeaponItemID;
		else
			WeaponItemID = 0;
		if (msg.HasShowFashion) ShowFashion = msg.ShowFashion;
		if (msg.HasFashionId) FashionId = msg.FashionId;
		if (msg.HasNickNameID)
		{
			if(msg.NickNameID !=0)
			{
				XCfgNickName infodata = XCfgNickNameMgr.SP.GetConfig(msg.NickNameID);
				if(infodata!= null)
				{
					try
					{
						string colorname = XGameColorDefine.Quality_Color[infodata.ColorID];
						this.NickName =string.Format("{0}{1}",colorname,infodata.NickName);
					}
					catch
					{
						Log.Write(LogLevel.ERROR,"Xplayer, the index is out of Quality_Color num");
					}
				}
			}
			else
			{
				this.NickName = "";
			}
		}
		
		// xz移动状态
		if(msg.HasMoveType)
		{
			switch((EProtoMoveType)msg.MoveType)
			{
			case(EProtoMoveType.eMoveType_Stand):
				break;
				
			case(EProtoMoveType.eMoveType_Segment):
				SegmentMoveTo(new Vector3(msg.TargetX, 0.0f, msg.TargetZ), Speed, null,EAnimName.Run);
				break;
				
			case(EProtoMoveType.eMoveType_Slide):
				SlideMoveTo(new Vector3(msg.TargetX, 0.0f, msg.TargetZ), Direction.y, Speed, null);
				break;
				
			case(EProtoMoveType.eMoveType_Step):
				StepMove(Speed, Direction.y);
				break;
			}
		}
		
		// 跳跃状态
		if(msg.HasJumpTime && msg.JumpTime >= 0)
		{
			Jump(msg.JumpX, msg.JumpZ, msg.JumpTime / 1000f, null);
		}
		
		// DynAttr
		foreach(Int64 dynAttr in msg.DynAttrList)
		{
			int key = (int)(dynAttr >> 32);
			int val = (int)(dynAttr);
			DynSet((EShareAttr)key, val);
		}
		
		// buff
		if(msg.BuffArrList.Count > 0)
		{
			BuffOper.Clear();
			foreach(UInt64 buff64 in msg.BuffArrList)
			{
				uint nBuffId = (uint)(buff64 >> 32);
				byte btBuffLevel = (byte)(buff64);
				BuffOper.AddBuff(nBuffId, btBuffLevel, 0, true, 0);
			}
		}
		
		// product
		if(msg.HasProduct) 
			StartProduct(msg.Product);
	}
	
	// 这个函数用来校准其他玩家的真实位置和客户端的显示位置
	private void correctPosition(Vector3 pos)
	{
		// 如果误差大于1米, 需要矫正一下
		if(XUtil.CalcDistanceXZ(Position, pos) > 1.0f)
		{
			Position = pos;
		}
	}

	public override void OnEnterSit()
	{

	}

	public override void OnExitSit()
	{

	}

	public void StartSit()
	{
		m_BehaviourSM.OnEvent((int)EStateEvent.esSit);
	}
	
	public void EndSit()
	{
		m_BehaviourSM.OnEvent((int)EStateEvent.esCancelSit);
	}
	
	public void On_SC_StartMove(SC_StartMove msg)
    {
        Vector3 fromPos = new Vector3(msg.FromX, 0f, msg.FromZ);
		correctPosition(fromPos);
		switch((EProtoMoveType)msg.MoveType)
		{
		case(EProtoMoveType.eMoveType_Stand):
			break;
			
		case(EProtoMoveType.eMoveType_Segment):
			SegmentMoveTo(new Vector3(msg.ToX, 0.0f, msg.ToZ), Speed, null,EAnimName.Run);
			break;
			
		case(EProtoMoveType.eMoveType_Slide):
			SlideMoveTo(new Vector3(msg.ToX, 0.0f, msg.ToZ), msg.Dir, Speed, null);
			break;
			
		case(EProtoMoveType.eMoveType_Step):
			StepMove(Speed, msg.Dir);
			break;
		}
    }

    public void On_SC_StopMove(SC_StopMove msg)
    {
        Vector3 pos = new Vector3(msg.PosX, 0f, msg.PosZ);
		correctPosition(pos);
		StopMove();
    }

    public void On_SC_Jump(SC_Jump msg)
    {
        Vector3 pos = new Vector3(msg.PosX, 0f, msg.PosZ);
		correctPosition(pos);
		Jump(null);
    }
	
	public void On_SC_JumpOver(SC_JumpOver msg)
	{
		Vector3 pos = new Vector3(msg.PosX, 0f, msg.PosZ);
		correctPosition(pos);
		JumpOver();
	}

    public void On_SC_Sit(SC_Sit msg)
    {
       // Vector3 pos = new Vector3(msg.PosX, msg.PosY, msg.PosZ);
	//	correctPosition(pos);
		StartSit();
    }

	public void On_SC_StopSit(SC_StopSit msg)
	{
		EndSit();
	}
		
	#region 属性
	
	public virtual int Class	// 职业
	{
		get { return DynGet(EShareAttr.esa_Class); }
		set
		{
			if(Class != value)
			{
				DynSet(EShareAttr.esa_Class, value);
			}
		}
	}
	
	public virtual float Grow
	{
		get { return DynGet(EShareAttr.esa_Grow) / Define.CONFIG_RATE_BASE; }
		set
		{
			if(Grow != value)
			{
				DynSet(EShareAttr.esa_Grow, (int)(value * Define.CONFIG_RATE_BASE));
			}
		}
	}

	
	public uint UColor
	{
		get { return m_AttrPlayer.UColor; }
		set 
		{
			if(m_AttrPlayer.UColor != value)
			{
				m_AttrPlayer.UColor = value;
				SendModelEvent(EModelEvent.evtUColor, value);
			}
		}
	}
	
	public uint ClassLevel
	{
		get { return m_AttrPlayer.ClassLevel; }
		set 
		{
			if(m_AttrPlayer.ClassLevel != value)
			{
				m_AttrPlayer.ClassLevel = value;
			}
		}
	}
	
	public ushort MountIndex
	{
		get { return m_AttrPlayer.MountIndex; }
		set
		{
			if(m_AttrPlayer.MountIndex != value)
			{
				m_AttrPlayer.MountIndex = value;
				SendModelEvent(EModelEvent.evtMountIndex, value);
			}
		}
	}
	
	public override uint ArmourItemID
    {
        set
        {
			//--4>: 直接刷新模型, 因为下线再上线时逻辑数据是一致的, 而模型必须重置
            //if (m_AttrCharacter.ArmourItemID != value)
            {
                m_AttrCharacter.ArmourItemID = value;
				
				//临时显示自己的强化显示效果
				//--------------------------------------------------
				byte 	strengthenLevel = 0;
				uint   	colorLevel = 0;		
				 XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(value);
				XItem item = ItemManager.GetItem((uint)EItemBoxType.Equip,(short)EQUIP_POS.EQUIP_POS_ARMOUR);
				if(item != null && cfgItem != null)
				{
					strengthenLevel	= (byte)item.mEquipAttr.mStrengthenLevel;
					if(cfgItem.IsRandom > 0)
						colorLevel		= (uint)item.mEquipAttr.mColor;
					else
						colorLevel		= cfgItem.QualityLevel;
				}
				//----------------------------------------------------

                // 刷新显示模型(铠甲模型或者角色裸模)
               
				int JobClass = DynGet(EShareAttr.esa_Class);
                if (cfgItem != null && 
					cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_ARMOR && 
					cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR &&
					(m_AttrCharacter.ShowFashion == 1 || FashionId == 0) )
                {
					//临时显示自己的强化显示效果
					//--------------------------------------------------
                   	int appearLevel = 0;
					
					XCfgStrengthen cfgStrengthen = XItemManager.GetStrengthenCfg(colorLevel,strengthenLevel);
					
					if(cfgStrengthen == null)
					{
						Log.Write(LogLevel.ERROR,"cant find strengthen cfg");
						return ;
					}
					
					if(cfgStrengthen != null)
						appearLevel	= cfgStrengthen.AppearLevel;
					//--------------------------------------------------					
					uint tempModelID = XItemManager.GetArmourID(cfgItem.ArmourID,appearLevel,Sex,JobClass);
					if(tempModelID != 0)
					{
						Log.Write(LogLevel.DEBUG,"Cur Player Model ID is {0}",tempModelID);
						ModelId	= tempModelID;
					}
                }
				else
                {
					if ( m_AttrCharacter.ShowFashion == 0 && FashionId > 0 )
					{
						FashionId = FashionId;
					}
					else
					{
						uint md = 0;		
						XCfgPlayerBase playerBase = XCfgPlayerBaseMgr.SP.GetConfig((byte)JobClass);
						if(playerBase != null)
						{
	                        if (Sex == (byte)EShareSex.eshSex_Male)
	                        {
	                            md = playerBase.MaleModel;
	                        }
	                        else if (Sex == (byte)EShareSex.eshSex_Female)
	                        {
	                            md = playerBase.FemaleModel;
	                        }
						}
	                    if (md > 0)
	                    {
							Log.Write(LogLevel.DEBUG,"Cur Player Model ID is {0}",md);
	                        ModelId = md;
	                    }
					}
                }
            }
        }
    }
	
	public override uint FashionId
    {
        set
        {
        	m_AttrCharacter.FashionId = value;
				
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(value);
			int JobClass = DynGet(EShareAttr.esa_Class);
            if (cfgItem != null && 
				cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_ARMOR && 
				cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOP_FASHION &&
				m_AttrCharacter.ShowFashion == 0)
            {
                int appearLevel = 1;
			
				uint tempModelID = XItemManager.GetArmourID(cfgItem.ArmourID, appearLevel, Sex, JobClass);
				if(tempModelID != 0)
				{
					Log.Write(LogLevel.DEBUG,"Cur Player Model ID is {0}",tempModelID);
					ModelId	= tempModelID;
				}
			}
			else
			{
				ArmourItemID = ArmourItemID;
			}
		}
    }
	
	public uint ShowFashion
	{
		get { return m_AttrCharacter.ShowFashion;}
		set
		{
			if(m_AttrCharacter.ShowFashion != value)
			{
				m_AttrCharacter.ShowFashion = value;
				if ( 0 == value && FashionId > 0)
				{
					FashionId = FashionId;
				}
				else
				{
					ArmourItemID = ArmourItemID;
				}
			}
		}
	}
	
//	public uint WeaponModelId
//	{
//		get { return m_AttrCharacter.WeaponModelID; }
//		set
//		{
//			if(m_AttrCharacter.WeaponModelID != value)
//			{
//				m_AttrCharacter.WeaponModelID	= value;
//				SendModelEvent(EModelEvent.evtWeaponId, value);
//				XEventManager.SP.SendEvent(EEvent.Attr_WeaponItemID, this, value);
//			}
//			
//		}
//	}
	
	public uint WeaponItemID
	{
		get { return m_AttrCharacter.WeaponItemID; }
		set
		{
			//if(m_AttrCharacter.WeaponItemID != value)
			{
				m_AttrCharacter.WeaponItemID = value;				

				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(m_AttrCharacter.WeaponItemID);
			
				//临时显示自己的强化显示效果
				//--------------------------------------------------
				byte strengthenLevel = 0;
				uint colorLevel = 0;
				XItem item = ItemManager.GetItem((uint)EItemBoxType.Equip,(short)EQUIP_POS.EQUIP_POS_WEAPON);
				if(item != null)
				{
					strengthenLevel	= (byte)item.mEquipAttr.mStrengthenLevel;
					colorLevel		= (uint)item.mEquipAttr.mColor;
				}
				//----------------------------------------------------
				int JobClass = DynGet(EShareAttr.esa_Class);
				if (cfgItem != null && cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_WEAPON && cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOT_WEAPON)
				{
					//临时显示自己的强化显示效果
					//--------------------------------------------------
	               	int appearLevel = 0;
					XCfgStrengthen cfgStrengthen = XItemManager.GetStrengthenCfg(colorLevel,strengthenLevel);
					
					if(cfgStrengthen == null)
					{
						Log.Write(LogLevel.ERROR,"cant find strengthen cfg");
						return ;
					}
					
					if(cfgStrengthen != null)
						appearLevel	= cfgStrengthen.AppearLevel;
					//--------------------------------------------------
					uint tempModelID = XItemManager.GetArmourID(cfgItem.ModelId,appearLevel,Sex,JobClass);
					WeaponModelId = tempModelID;
				}
				else
				{						
					XCfgPlayerBase playerBase = XCfgPlayerBaseMgr.SP.GetConfig((byte)JobClass);
					if(playerBase != null)
					{
						WeaponModelId	= playerBase.DefaultWeapon;
					}                   
				}
			}
			
			
		}
	}
	
	public override void OnMouseDown(int mouseCode, Vector3 clickPoint)
	{
		XEventManager.SP.SendEvent(EEvent.ObjSel_SetData,Name,Level,ID);
		XEventManager.SP.SendEvent(EEvent.UI_Show,EUIPanel.eTargetInfo);
	}
	
	#endregion
}



/*
public class XPlayer : XCharacter
{
    public XPlayer(ulong id) : base(id)
    {
        ObjectType = EObjectType.OtherPlayer;
    }

    protected override void OnCreateModel()
    {
        if(ObjectType == EObjectType.OtherPlayer)
        {
			RefreshMountModel();
            RefreshWeaponModel();
            if (!XLogicWorld.SP.ObjectManager.IsShowOtherPlayer)
            {
                SetVisible(false);
            }
        }
    }
	
	public virtual void RefreshMountModel()
	{
		if(HasModel())
		{
			if(0 == MountIndex)
			{
				m_Model.Dismount();
			}
			else
			{
				XCfgMount mountCfg = XCfgMountMgr.SP.GetConfig(MountIndex);
				if(null != mountCfg)
				{
					m_Model.SetMountModel(this, (EMountType)mountCfg.MountType, mountCfg.ModelId);
				}
			}
		}
	}
	
    public override void OnMouseUpAsButton()
    {
		base.OnMouseUpAsButton();
    }
    #region Packet Process Functions

    public override void SetAppearData(object data, bool isShow)
    {
        base.SetAppearData(data, isShow);

        SC_PlayerAppearData msg = data as SC_PlayerAppearData;
        if (null == msg)
        {
            return;
        }

        //--4>TODO: 考虑同步数据进行分类, 使用公用结构, 给子类调用
        Vector3 pos = GetPosition();
        Vector3 posTarget = GetPosition();
        Version = msg.Version;
        if (msg.HasName) Name = msg.Name;
        if (msg.HasTitle) Title = msg.Title;
        if (msg.HasPosX) pos.x = msg.PosX;
        if (msg.HasPosY) pos.y = msg.PosY;
        if (msg.HasPosZ) pos.z = msg.PosZ;
        if (msg.HasTargetX) posTarget.x = msg.TargetX;
        if (msg.HasTargetY) posTarget.y = msg.TargetY;
        if (msg.HasTargetZ) posTarget.z = msg.TargetZ;
        if (msg.HasDirection) SetDirection(new Vector3(0, msg.Direction, 0));
        if (msg.HasModel) ModelId = msg.Model;
        if (msg.HasColor) UColor = msg.Color;
        if (msg.HasSex) Sex = (byte)msg.Sex;
        if (msg.HasClass) JobClass = (byte)msg.Class;
        if (msg.HasClassLevel) ClassLevel = msg.ClassLevel;
        if (msg.HasLevel) Level = (int)msg.Level;
        if (msg.HasMaxHp)
        {
            MaxHp = msg.MaxHp;
            Hp = MaxHp;
        }
        if (msg.HasRunSpeed) Speed = msg.RunSpeed;
        if (msg.HasMountIndex) MountIndex = (ushort)msg.MountIndex;
        if (msg.HasArmourItemID) ArmourItemID = msg.ArmourItemID;
        if (msg.HasWeaponItemID) WeaponItemID = msg.WeaponItemID;

        SetPosition(pos);

        if (isShow)
        {
            CreateModel();

            // 初始化移动状态等
            int nMotionState = (int)EMotionState.ms_idle;
            if (msg.HasMotionState) nMotionState = msg.MotionState;
            switch (nMotionState)
            {
                case (int)EMotionState.ms_run:
                    StartObjectMove(pos, posTarget);
                    break;
                case (int)EMotionState.ms_sit:
                    StartObjectSit(pos);
                    break;
                default:
                    StopObjectMove(pos);
                    break;
            }
        }
    }

    public void On_SC_StartMove(SC_StartMove msg)
    {
        Vector3 fromPos = new Vector3(msg.FromX, msg.FromY, msg.FromZ);
        Vector3 toPos = new Vector3(msg.ToX, msg.ToY, msg.ToZ);
        StartObjectMove(fromPos, toPos);
    }

    public void On_SC_StopMove(SC_StopMove msg)
    {
        Vector3 pos = new Vector3(msg.PosX, msg.PosY, msg.PosZ);
        StopObjectMove(pos);
    }

    public virtual void ForceSetPosition(Vector3 pos)
    {
        SetPosition(pos);
        ResetObjectMoveInfo();
    }

    public void On_SC_Jump(SC_Jump msg)
    {
        Vector3 pos = new Vector3(msg.PosX, msg.PosY, msg.PosZ);
        StartObjectJump(pos);
    }

    public void On_SC_Sit(SC_Sit msg)
    {
        Vector3 pos = new Vector3(msg.PosX, msg.PosY, msg.PosZ);
        StartObjectSit(pos);
    }

    #endregion

    #region attr set
    private XAttrPlayer m_AttrPlayer = new XAttrPlayer();

    public byte JobClass
    {
        get { return (byte)DynGet(EShareAttr.esa_Class); }
        set
        {
            DynSet(EShareAttr.esa_Class, value);
        }
    }

    public uint UColor
    {
        get { return m_AttrPlayer.UColor; }
        set
        {
            m_AttrPlayer.UColor = value;
        }
    }

    public uint ClassLevel
    {
        get { return m_AttrPlayer.ClassLevel; }
        set
        {
            m_AttrPlayer.ClassLevel = value;
        }
    }
	
	public ushort MountIndex
	{
		get { return m_AttrPlayer.MountIndex; }
		set
		{
			if(m_AttrPlayer.MountIndex != value)
			{
				m_AttrPlayer.MountIndex = value;
				RefreshMountModel();
			}
		}
	}

    #endregion

    public override uint ArmourItemID
    {
        set
        {
			//--4>: 直接刷新模型, 因为下线再上线时逻辑数据是一致的, 而模型必须重置
            //if (m_AttrCharacter.ArmourItemID != value)
            {
                m_AttrCharacter.ArmourItemID = value;
				
				//临时显示自己的强化显示效果
				//--------------------------------------------------
				byte 	strengthenLevel = 0;
				uint   	colorLevel = 0;				
				XItem item = ItemManager.GetItem((uint)EItemBoxType.Equip,(short)EQUIP_POS.EQUIP_POS_ARMOUR);
				if(item != null)
				{
					strengthenLevel	= (byte)item.mEquipAttr.mStrengthenLevel;
					colorLevel		= (uint)item.mEquipAttr.mColor;
				}
				//----------------------------------------------------

                // 刷新显示模型(铠甲模型或者角色裸模)
                XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(value);
                if (cfgItem != null && cfgItem.ItemType == (ushort)EItem_Type.EITEM_TYPE_ARMOR && cfgItem.EquipPos == (int)EQUIP_SLOT_TYPE.EQUIP_SLOT_ARMOUR)
                {
					//临时显示自己的强化显示效果
					//--------------------------------------------------
                   	int appearLevel = 0;
					XCfgStrengthen cfgStrengthen = XCfgStrengthenMgr.SP.GetConfig(colorLevel,strengthenLevel);
					if(cfgStrengthen != null)
						appearLevel	= cfgStrengthen.AppearLevel;
					//--------------------------------------------------
					
					uint tempModelID = XItemManager.GetArmourID(cfgItem.ArmourID,appearLevel,Sex,JobClass);
					if(tempModelID != 0)
						ModelId	= tempModelID;
                }
				else
                {
					uint md = 0;		
					XCfgPlayerBase playerBase = XCfgPlayerBaseMgr.SP.GetConfig(JobClass);
					if(playerBase != null)
					{
                        if (Sex == (byte)EShareSex.eshSex_Male)
                        {
                            md = playerBase.MaleModel;
                        }
                        else if (Sex == (byte)EShareSex.eshSex_Female)
                        {
                            md = playerBase.FemaleModel;
                        }
					}
                    if (md > 0)
                    {
                        ModelId = md;
                    }
                }
            }
        }
    }

    #region Death About
    protected override void ToDead()
	{
		base.ToDead();
		PlayAnimation(EAnimName.Death);	
	}
	
	protected override void ToRevive()
	{
		base.ToDead();
		PlayAnimation(EAnimName.Idle);
	}
	#endregion
}*/
