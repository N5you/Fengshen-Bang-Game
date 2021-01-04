using System;
using UnityEngine;
using XGame.Client.Packets;
using Google.ProtocolBuffers;
/* 类名: XGameObject
 * 描述: 游戏中对象
 * 功能:
 * 		1. 维护模型的创建.销毁.visible
 * 		2. 维护GameObject基础属性
 */

public class XGameObject : XObjectModel.IModelListener,IBehaviourListener
{
	public ulong ID { get; protected set; }

	public EObjectType ObjectType { get; protected set; }

	private bool m_bIsVisible;
	public bool IsWaitAppearData;
	private float m_tmpScale = 0.0f;
	public bool IsActive = true;
	
	protected XAttrPlayer m_AttrPlayer = new XAttrPlayer();	
	
	public XGameObject (ulong id)
	{
		ID = id;
		ObjectType = EObjectType.GameObject;
		Version = 0;
		m_bIsVisible = true;
		IsWaitAppearData = false;
		IsEnableHover = true;
	}

	~XGameObject ()
	{
		m_AttrGameObject = null;
	}
	
	public virtual void Init()
	{
		
	}
	
	public virtual void Breathe()
	{
		if (0f != m_tmpScale) 
		{
			bool b = false;
			if (m_tmpScale < Scale) {
				m_tmpScale += 0.08f;
				if (m_tmpScale >= Scale) {
					m_tmpScale = Scale;
					b = true;
				}
			}
			else if (m_tmpScale > Scale) {
					m_tmpScale -= 0.08f;
					if (m_tmpScale <= Scale) {
						m_tmpScale = Scale;
						b = true;
					}
				}
			
			SendModelEvent (EModelEvent.evtScale, m_tmpScale);
			if (b)
				m_tmpScale = 0f;
		}
		
		if(m_ObjectModel != null)
			m_ObjectModel.Breathe();

	}
	
	public virtual void OnBeginLoadLevel(int nLevelId, ESceneType sceneType)
	{
	}	// 场景开始加载之前
	public virtual void OnLevelLoaded(int nLevelId, ESceneType sceneType)
	{
	}		// 场景加载之后(逻辑地图完成, 静态对象已生成)
	
	public bool IsAppear { get { return null != m_ObjectModel; } }

	public float DisAppearRockon { get; private set; }
	
	public virtual EHeadBoard_Type GetHeadBoardType() { return EHeadBoard_Type.EHeadBoard_Type_None; }
	
	// 出现, 需要告诉模型当前的属性信息
	public virtual void Appear()
	{
		if (IsAppear)
			return;
 		m_ObjectModel = new XObjectModel();
		m_ObjectModel.mainModelLoaded += loadMainModelFinish;
		m_ObjectModel.AddModelListener (this);
		// 通知MainPlayer出现了一个GameObject
		XLogicWorld.SP.MainPlayer.OnObjectAppear (ObjectType, this);
		SendModelEvent (EModelEvent.evtVisible, m_bIsVisible);
		SendModelEvent (EModelEvent.evtName, Name);
		SendModelEvent (EModelEvent.evtHeadBoardType, GetHeadBoardType());
		SendModelEvent (EModelEvent.evtModelId, ModelId);
		SendModelEvent (EModelEvent.evtPosition, Position);
		SendModelEvent (EModelEvent.evtDirection, Direction);
		SendModelEvent (EModelEvent.evtScale, Scale);
	}
		
	public void loadMainModelFinish(XObjectModel model)
	{
		SendModelEvent(EModelEvent.evtMountIndex, m_AttrPlayer.MountIndex);
	}
	
	public virtual void GetHeadPosInfo(ref Vector3 pos, ref GameObject parent)
	{
		m_ObjectModel.GetHeadPosInfo (ref pos, ref parent);
		return;
	}
	
	public  virtual void OnModelLoaded()
	{

	}
	
	public virtual void OnWeaponLoaded()
	{
		
	}
	
	// 消失
	public virtual void DisAppear()
	{
		if (null == m_ObjectModel)
			return;
		
//		SendModelEvent (EModelEvent.evtMountIndex, 0u);
		SendModelEvent (EModelEvent.evtDestroy);
		m_ObjectModel = null;
		DisAppearRockon = Time.time;
	}
	
	// 设置属性
	public virtual void SetAppearData(object data)
	{ 
	}
	
	public void SetHudVisible(bool isVisible)
	{
		SendModelEvent (EModelEvent.evtHudVisible, isVisible);
	}
	
	#region 模型
	protected XObjectModel m_ObjectModel = null;
	
	public XObjectModel ObjectModel {
		get{ return m_ObjectModel; }
	}
	
	public void SendModelEvent(EModelEvent evt, params object[] args)
	{
		if (null == m_ObjectModel)
			return;
		m_ObjectModel.HandleEvent (evt, args);
	}
	
	// 设置模型是否可见
	public bool Visible {
		get { return m_bIsVisible; }
		set {
			if (m_bIsVisible != value) {
				m_bIsVisible = value;
				SendModelEvent (EModelEvent.evtVisible, value);
			}
		}
	}
	
	// 获取模型插槽
	public Transform GetSkeleton(ESkeleton ske)
	{
		if (null == m_ObjectModel)
			return null;
		return m_ObjectModel._getSkeleton (ske);
	}
	
	// 取模型的一个动画时长
	public float AnimLength(EAnimName anim)
	{
		if (null == m_ObjectModel)
			return 0f;
		return m_ObjectModel._animLength (anim);
	}
	
	// 取模型半径, 包围盒半径 * scale_x
	public float Radius()
	{
		if (null == m_ObjectModel)
			return 0f;
		return m_ObjectModel._getRadius ();
	}
	
	// 在模型头顶以特定样式飞一行字
	public void FlyString(EFlyStrType ft, string str)
	{
		if (null == m_ObjectModel)
			return ;
		m_ObjectModel.HandleEvent (EModelEvent.evtFlyString, ft, str);
	}
	
	// 在模型的腰部位置以特定样式飞一行字
	public void FlyStringHalf(EObjectHalfHintType ht, string str)
	{
		if (null == m_ObjectModel)
			return ;
		m_ObjectModel.HandleEvent (EModelEvent.evtFlyStringHalf, ht, str);
	}
		
	// 绑定一个GameObject到特定骨骼上
	public void AttachGo(ESkeleton ske, GameObject go)
	{
		AttachGo (ske, go, Vector3.zero, Vector3.zero);
	}
	
	// 绑定一个GameObject到特定骨骼上, 并设定相对位置
	public void AttachGo(ESkeleton ske, GameObject go, Vector3 localPosition, Vector3 localRotation)
	{
		if (null == go)
			return;
		SendModelEvent (EModelEvent.evtAttachGo, ske, go, localPosition, localRotation);
	}	
	
	public  void OnCancelSelect()
	{
		SendModelEvent(EModelEvent.evtSelect,(uint)0,false);
	}
	
	#endregion
	
	
	
    #region 属性
	private XAttrGameObject m_AttrGameObject = new XAttrGameObject ();

	public uint Version { get; protected set; }
	
	public string Name {
		get { return m_AttrGameObject.Name; }
		set {
			if (m_AttrGameObject.Name == value)
				return;			
			m_AttrGameObject.Name = value;
			SendModelEvent (EModelEvent.evtName, value);
			XEventManager.SP.SendEvent (EEvent.Attr_Name, this, value);
		}
	}

	public uint ModelId {
		get { return m_AttrGameObject.ModelId; }
		set {
			if (m_AttrGameObject.ModelId == value)
				return;	
			m_AttrGameObject.ModelId = value;
			SendModelEvent (EModelEvent.evtModelId, value);
			XEventManager.SP.SendEvent (EEvent.Attr_ModelId, this, value);
		}
	}
	
	public Vector3 Position {
		get { return m_AttrGameObject.Position; }
		set {
			if (m_AttrGameObject.Position == value)
				return;
			m_AttrGameObject.Position = value;
			SendModelEvent (EModelEvent.evtPosition, value);
		}
	}
	
	public Vector3 Direction {
		get { return m_AttrGameObject.Direction; }
		set {
			if (m_AttrGameObject.Direction == value)
				return;
			m_AttrGameObject.Direction = value;
			SendModelEvent (EModelEvent.evtDirection, value);
		}
	}
	
	public Color MatColor {
		get { return m_AttrGameObject.MatColor; }
		set {
			if (m_AttrGameObject.MatColor == value)
				return;
			m_AttrGameObject.MatColor = value;
			SendModelEvent (EModelEvent.evtMatColor, value);
		}
	}
	
	// 整体缩放(头顶面板, 坐骑模型, 本体模型等等一起缩放)
	public float Scale {
		get { return m_AttrGameObject.Scale; }
		set {
			if (0f == Scale || m_AttrGameObject.Scale == value)
				return;
			m_tmpScale = Scale;
			m_AttrGameObject.Scale = value;
		}
	}
	
	public bool IsEnableHover { get; set; }
	
    #endregion
	
	
	#region IBehaviourListener
	public virtual float GetClickDistance()
	{
		return 0f;
	}

	public virtual void OnMouseDown(int mouseCode, Vector3 clickPoint)
	{
	}

	public virtual void OnMouseUp(int mouseCode)
	{
	}

	public virtual void OnMouseUpAsButton(int mouseCode)
	{
	}

	public virtual void OnControllerColliderHit(ControllerColliderHit hit)
	{
	}
	
	public virtual void OnMouseEnter()
	{
		if (!IsEnableHover || m_ObjectModel == null)
			return ;
		
		m_ObjectModel.HoverIn ();
			
	}

	public virtual void OnMouseExit()
	{
		if (!IsEnableHover || m_ObjectModel == null)
			return ;
		m_ObjectModel.HoverOut ();
	}
    #endregion
}
