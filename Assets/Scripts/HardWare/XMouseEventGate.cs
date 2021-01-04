using UnityEngine;
using System.Collections;
using XGame.Client.Base.Pattern;

/*	
 * 鼠标事件功能实现
 */
public class XMouseEventGate : XSingleton<XMouseEventGate>
{
	private XBehaviour leftBehaviour = null;		// 左键单击对象记录
	private XBehaviour rightBehaviour = null;		// 右键单击对象记录
	private XBehaviour moveBehaviour = null;		// 鼠标移动对象记录
	private XBehaviour lastLeftBehaviour = null;	// 上一次左键点击记录
	
	private bool m_bIsMainCameraRotate = false;
	
	public bool Init()
	{
		XEventManager.SP.AddHandler(OnMainPlayerEnterGame, EEvent.MainPlayer_EnterGame);
		return true;
	}	
	
	private void OnMainPlayerEnterGame(EEvent evt, params object[] args)
	{
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_MouseDown, 0, OnMouse0Down);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_MouseUp, 0, OnMouse0Up);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_MouseDown, 1, OnMouse1Down);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_MouseUp, 1, OnMouse1Up);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_MouseMove, 0, OnMouseMove);
		XHardWareGate.SP.RegEvent(EHWEventType.e_HW_MouseScroll, 0, OnMouseScroll);
	}
	
	// 获取所有UI层的撞击collider
	private RaycastHit[] GetUIHit()
	{
		Camera camera2D = LogicApp.SP.UICamera;
		Ray rayUI = camera2D.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hitUI = Physics.RaycastAll(rayUI, Mathf.Infinity, 1 << GlobalU3dDefine.Layer_UI_2D);
		return hitUI;
	}
	
	// 根据对象拣选优先级, 获取3D层的Behaviour
	private XBehaviour GetXBehaviour(out Vector3 clickPoint)
	{
		clickPoint = Vector3.zero;
		Camera mainCamera = LogicApp.SP.MainCamera;
		Ray rayX = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hitsX = Physics.RaycastAll(rayX, Mathf.Infinity, 
			(1 << GlobalU3dDefine.Layer_GameObject | 1 << GlobalU3dDefine.Layer_Decal | 1<< GlobalU3dDefine.Layer_TerrainObject ));
		
		XBehaviour behaviour = null;
		int k = -1;
		for(int i=0; i<hitsX.Length; i++)
		{
			XBehaviour behaTmp = hitsX[i].transform.GetComponent<XBehaviour>();
			if(null == behaTmp) continue;
			if(null == behaviour)
			{
				k = i;
				behaviour = behaTmp;
				clickPoint = hitsX[i].point;
			}
			else if((int)behaTmp.BehaType < (int)behaviour.BehaType)
			{
				k = i;
				behaviour = behaTmp;
				clickPoint = hitsX[i].point;
			}
			else if((int)behaTmp.BehaType == (int)behaviour.BehaType)
			{
				if(hitsX[i].distance < hitsX[k].distance)
				{
					k = i;
					behaviour = behaTmp;
					clickPoint = hitsX[i].point;
				}
			}
		}
		return behaviour;
	}
	
	public void OnMouse0Down()
	{
		leftBehaviour = null;
		RaycastHit[] hitUI = GetUIHit();
		if(0 == hitUI.Length)
		{
			Vector3 clickPoint = Vector3.zero;
			leftBehaviour = GetXBehaviour(out clickPoint);
			if(null != leftBehaviour) 
			{
				if(lastLeftBehaviour != null)
					lastLeftBehaviour.OnCancelSelect();
				leftBehaviour.WeOnMouseDown(0, clickPoint);
				lastLeftBehaviour	= leftBehaviour;
			}
		}
		else
		{
			XDefaultFrame tgtFrame = null;
			for(int i=0; i<hitUI.Length; i++)
			{
				XTabFrame tabFrame = hitUI[i].collider.gameObject.GetComponent<XTabFrame>();
				if(null != tabFrame)
				{
					if(null == tgtFrame) tgtFrame = tabFrame.TgtFrame;
					else if(tabFrame.TgtFrame.transform.position.z < tgtFrame.transform.position.z) tgtFrame = tabFrame.TgtFrame;
				}
			}
			if(null != tgtFrame) tgtFrame.Top();
		}
	}
	
	public void OnMouse0Up()
	{
		RaycastHit[] hitUI = GetUIHit();
		if(0 == hitUI.Length)
		{
			Vector3 clickPoint = Vector3.zero;
			XBehaviour behaviour = GetXBehaviour(out clickPoint);
			if(null != behaviour)
			{
				behaviour.WeOnMouseUp(0);
				if(behaviour == leftBehaviour)
					leftBehaviour.WeOnMouseUpAsButton(0);
			}
		}
		leftBehaviour = null;
	}
	
	public void OnMouse1Down()
	{
		RaycastHit[] hitUI = GetUIHit();
		if(0 == hitUI.Length)
		{
			m_bIsMainCameraRotate = true;
			Cursor.visible = false;
			Vector3 clickPoint = Vector3.zero;
			rightBehaviour = GetXBehaviour(out clickPoint);
			if(null != rightBehaviour)
			{
				rightBehaviour.WeOnMouseDown(1, clickPoint);
			}
		}
	}
	
	public void OnMouse1Up()
	{
		if(m_bIsMainCameraRotate)
		{
			m_bIsMainCameraRotate = false;
			Cursor.visible = true;
		}
		RaycastHit[] hitUI = GetUIHit();
		if(0 == hitUI.Length)
		{
			Vector3 clickPoint = Vector3.zero;
			XBehaviour behaviour = GetXBehaviour(out clickPoint);
			if(null != behaviour)
			{
				behaviour.WeOnMouseUp(1);
				if(behaviour == rightBehaviour)
					rightBehaviour.WeOnMouseUpAsButton(1);
			}
		}
		rightBehaviour = null;
	}
	
	public void OnMouseMove()
	{
		RaycastHit[] hitUI = GetUIHit();
		XBehaviour behaviour = null;
		if(0 == hitUI.Length)
		{
			Vector3 clickPoint = Vector3.zero;
			behaviour = GetXBehaviour(out clickPoint);
		}
		
		if(null != moveBehaviour)
		{
			if(null == behaviour)
			{
				moveBehaviour.WeOnMouseExit();
				moveBehaviour	= null;
			}
			else if(behaviour != moveBehaviour)
			{
				moveBehaviour.WeOnMouseExit();
				behaviour.WeOnMouseEnter();
				moveBehaviour	= behaviour;
			}
		}
		else
		{
			if(null != behaviour)
			{
				behaviour.WeOnMouseEnter();
				moveBehaviour	= behaviour;
			}
		}
		leftBehaviour = behaviour;
		
		if(m_bIsMainCameraRotate)
		{
			XCameraLogic.SP.Rotate(XHardWareGate.SP.MouseMoveX);
		}
	}
	
	public void OnMouseScroll()
	{
		XCameraLogic.SP.Scroll(XHardWareGate.SP.MouseScroll * LogicApp.SP.UserDefine.MainCameraScrollSpeed, false);
	}
}
