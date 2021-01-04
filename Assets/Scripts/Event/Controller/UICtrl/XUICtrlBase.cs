using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//按位组合
public enum UI_MODE
{	
	UI_MODE_NO_OP				= 0,		//不做处理的界面,比如冒血，头顶板子，登陆界面，选择，创建角色界面等
	UI_MODE_DYNAMIC				= 1,		//动态界面，在切换模式的时候会被关闭
	UI_MODE_STATIC_NORMAL		= 2,		//静态主界面
	UI_MODE_STATIC_SUB_SCENE	= 3,		//静态副本界面
	UI_MODE_STATIC_BATTLE		= 4,		//静态副本界面
	UI_MODE_STATIC_BATTLE_CAMERA= 5,		//静态录像界面
	UI_MODE_CUTSCENE			= 6,		//过场动画场景界面
	UI_MODE_NORMAL_CUTSCENE		= 7,		//普通过场动画场景界面
	UI_MODE_CHARACTER			= 8,		//任务选择场景界面
	UI_MODE_NUM
}

public abstract class XUICtrlBase
{
	public virtual bool Show()
	{		
		return true;
	}
	
	public virtual bool Hide()
	{		
		return true;
	}
	
	public virtual bool Toggle()
	{
		return true;
	}
	
	public virtual bool ReqOriginal(object arg)
	{
		return true;
	}
	
	public virtual void OnOriginal(object arg)
	{
	}
	
	public virtual void OnCreated(object arg)
	{
	}
	
	public virtual bool IsCanShow()
	{
		return true;
	}

	public virtual void OnShow()
	{
	}

	public virtual void OnHide()
	{
	}
	
	public virtual void Breathe()
	{
	}

	public virtual Bounds GetUIBounds()
	{
		return new Bounds();
	}

	public virtual void SetPos(Vector3 pos) {}

	public virtual Vector3 GetPos(){ return Vector3.zero; }

	public virtual void MovePos(Vector3 orignalPos, Vector3 targetPos){}
	
	public virtual EUIPanel GetPanelType()
	{
		return EUIPanel.eCount;
	}

	public int UIMode { get; set; }

	public virtual bool IsShow()
	{
		return mIsShow;
	}

	public virtual bool IsResourceLoaded()
	{
		return false;
	}

	protected bool mIsShow = false;

	public uint PanelKey { get; set; }

	public bool IsLogicShow = false;
}

public abstract class XUICtrlTemplate<TUI>  : XUICtrlBase where TUI : XUIBaseLogic
{
	protected TUI LogicUI 			{ get; private set; }

	protected TUI OriginalUI 		{ get; private set; }

	private bool IsLoadingResource = false;
	private bool m_bIsOriginaling = false;

	public XUICtrlTemplate ()
	{
		LogicUI = null;
		OriginalUI = null;
		UIMode = 1 << (int)UI_MODE.UI_MODE_NO_OP;
	}
	public override bool IsResourceLoaded()
	{
		return LogicUI != null;
	}

	public override Bounds GetUIBounds()
	{
		if(LogicUI != null)
			return NGUIMath.CalculateRelativeWidgetBounds(LogicUI.transform);

		return new Bounds();
	}

	public override void SetPos(Vector3 pos)
	{
		if(LogicUI == null)
			return ;

		LogicUI.transform.localPosition = pos;

		TweenAlpha uiAlpha = LogicUI.GetComponent<TweenAlpha>();
		if(uiAlpha == null)
			uiAlpha = LogicUI.gameObject.AddComponent<TweenAlpha>();
		uiAlpha.Reset();
		uiAlpha.from	= 0.0f;
		uiAlpha.to	= 1.0f;
		uiAlpha.duration = 0.15f;
		uiAlpha.enabled	= true;
	}

	public override Vector3 GetPos()
	{
		if(LogicUI == null)
			return Vector3.zero;
		return LogicUI.transform.localPosition;
	}

	public override void MovePos(Vector3 orignalPos, Vector3 targetPos) 
	{
		if(LogicUI == null)
			return;
		TweenPosition uiPos = LogicUI.GetComponent<TweenPosition>();
		if(uiPos == null)
			uiPos = LogicUI.gameObject.AddComponent<TweenPosition>();
		uiPos.Reset();
		uiPos.from = orignalPos;
		uiPos.to = targetPos;
		uiPos.duration = 0.2f;

		TweenAlpha uiAlpha = LogicUI.GetComponent<TweenAlpha>();
		if(uiAlpha == null)
			uiAlpha = LogicUI.gameObject.AddComponent<TweenAlpha>();
		uiAlpha.Reset();
		uiAlpha.from	= 0.0f;
		uiAlpha.to	= 1.0f;
		uiAlpha.duration = 0.3f;

		uiPos.enabled	= true;
		uiAlpha.enabled	= true;
	}
	
	public override bool Show()
	{
		IsLogicShow = true;
		if (!IsResourceLoaded ()) {
			IsLoadingResource = true;
			return false;
		}
		
		if (IsLoadingResource)
			return true;
		
		mIsShow = true;
		LogicUI.Show ();
		return true;
	}
	
	public override bool Hide()
	{
		IsLogicShow = false;
		if (null != LogicUI) {
			mIsShow = false;
			base.Hide ();
			LogicUI.Hide ();
		}
		return true;
	}
	
	public override bool Toggle()
	{	
		if (!IsShow ())
			return Show ();
		return Hide ();
	}
	
	public override bool ReqOriginal(object arg)
	{
		if (null != OriginalUI) {
			return true;
		}
		if (!m_bIsOriginaling) {
			m_bIsOriginaling = true;
			return false;
		}
		return true;
	}
	
	public override void OnCreated(object arg)
	{
		base.OnCreated (arg);
		LogicUI = arg as TUI;
		IsLoadingResource = false;
		mIsShow = true;
		LogicUI.Init ();
		LogicUI.Show ();
		
		// 处理滞留事件
		for (int i=0; i<m_CreatedHandlerHold.Count; i++) {
			object[] arr = m_CreatedHandlerHold [i];
			OnEventCreated ((EEvent)(arr [0]), (object[])(arr [1]));
		}
		m_CreatedHandlerHold.Clear ();
		
		if (!IsLogicShow)
			Hide ();
	}
	
	public override void OnOriginal(object arg)
	{
		base.OnOriginal (arg);
		OriginalUI = arg as TUI;
		
		// 处理滞留事件
		for (int i=0; i<m_OriginalHandlerHold.Count; i++) {
			object[] arr = m_OriginalHandlerHold [i];
			OnEventOrignal ((EEvent)(arr [0]), (object[])(arr [1]));
		}
		m_OriginalHandlerHold.Clear ();
	}
	
	private SortedList<EEvent, List<XEventManager.XGlobalEventHandler>> m_CheckCreatedHandler = new SortedList<EEvent, List<XEventManager.XGlobalEventHandler>> ();
	private List<object[]> m_CreatedHandlerHold = new List<object[]> ();
	private SortedList<EEvent, List<XEventManager.XGlobalEventHandler>> m_CheckOriginalHandler = new SortedList<EEvent, List<XEventManager.XGlobalEventHandler>> ();
	private List<object[]> m_OriginalHandlerHold = new List<object[]> ();
	
	// 注册事件: 检测是否创建, 如果否会在创建之后帮你调用一次
	public void RegEventAgent_CheckCreated(EEvent evt, XEventManager.XGlobalEventHandler handler)
	{
		if (!m_CheckCreatedHandler.ContainsKey (evt)) {
			m_CheckCreatedHandler.Add (evt, new List<XEventManager.XGlobalEventHandler> ());
			XEventManager.SP.AddHandler (OnEventCreated, evt);
		}
		m_CheckCreatedHandler [evt].Add (handler);
	}
			
	// 注册事件: 检测是否Original, 如果否会在Original之后帮你调用一次
	public void RegEventAgent_CheckOriginal(EEvent evt, XEventManager.XGlobalEventHandler handler)
	{
		if (!m_CheckOriginalHandler.ContainsKey (evt)) {
			m_CheckOriginalHandler.Add (evt, new List<XEventManager.XGlobalEventHandler> ());
			XEventManager.SP.AddHandler (OnEventOrignal, evt);
		}
		m_CheckOriginalHandler [evt].Add (handler);
	}
	
	private void OnEventCreated(EEvent evt, params object[] args)
	{
		if (!m_CheckCreatedHandler.ContainsKey (evt))
			return;
		
		if (null != LogicUI) {
			foreach (XEventManager.XGlobalEventHandler handle in m_CheckCreatedHandler[evt]) {
				handle (evt, args);
			}
		} else {
			m_CreatedHandlerHold.Add (new object[]{evt, args});
		}
	}
	
	private void OnEventOrignal(EEvent evt, params object[] args)
	{
		if (!m_CheckOriginalHandler.ContainsKey (evt))
			return;

		if (null != OriginalUI) {
			foreach (XEventManager.XGlobalEventHandler handle in m_CheckOriginalHandler[evt]) {
				handle (evt, args);
			}
		} else {
			m_OriginalHandlerHold.Add (new object[]{evt, args});
		}
	}
}
