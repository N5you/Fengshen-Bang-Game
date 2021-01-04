using UnityEngine;
using System.Collections;
using XGame.Client.Base.Pattern;

// 主摄像机逻辑
public class XCameraLogic : XSingleton<XCameraLogic>
{
	public Camera mainCamera { get; private set; }
	private Transform m_MotherTransform = null;
	private Vector3 m_MotherPos = Vector3.zero;
	
	// 主摄像机在一个矩形平面内运动, 以下保存着摄像机相对矩形信息
	private float m_LocalY = 0f;			// Attach的时候的Y轴相对坐标
	private float m_LocalDis = 0f;			// Attach的时候的X,Z平面相对距离
	private Vector3 m_relaPosition = Vector3.zero;
	private float m_fWheelDelta = 0f;
	private float m_fWaitWD = 0f;
	private GameObject m_CamDemo = null;
	
	private Vector3 m_v3CamPosTarget = Vector3.zero;
	public bool	IsEnableCameraCollide = false;
	
	public XCameraLogic()
	{
		mainCamera = Camera.main;
		m_CamDemo = new GameObject("camera temp");
		m_CamDemo.transform.parent = mainCamera.transform.parent;
		m_CamDemo.transform.position = mainCamera.transform.position;
		m_CamDemo.transform.rotation = mainCamera.transform.rotation;
		
		Object.DontDestroyOnLoad(mainCamera);
		Object.DontDestroyOnLoad(m_CamDemo);
	}
	
	private Vector3 relaPosition{
		set{ m_relaPosition = value;
		}
	}
	
	public float WheelDelta{
		get{ return m_fWheelDelta; }
		set{ m_fWheelDelta = value; }
	}
	
	public void Breathe()
	{
		if(0f != m_fWaitWD)
		{
			bool w = m_fWaitWD > 0;
			float fd = Time.deltaTime * LogicApp.SP.UserDefine.MainCameraCurveSpeed;
			if(!w) fd *= -1;
			m_fWaitWD -= fd;
			if(w != (m_fWaitWD > 0) || !scroll(fd))
				m_fWaitWD = 0f;
		}
	}
	
	private void printCamPos(string preStr)
	{
		//Log.Write("Frame"+Time.frameCount.ToString()+" " + preStr+" parentName="+mainCamera.transform.parent.name+"  pos=" + mainCamera.transform.position.ToString() +"  CamTargetPos="+m_v3CamPosTarget.ToString() );
		
	}
	
	public void LateBreathe()
	{
		printCamPos("LateBreathe ");
		if(XHardWareGate.SP.LockMouse )
			return;
		//all the input is over
		cameraCollider();
	}
	
	public void OnRenderObject()
	{
		if(XHardWareGate.SP.LockMouse && !IsEnableCameraCollide)
			return;
		
		//the render scene is over
//		printCamPos("OnRenderObject UP ");
//		mainCamera.transform.position = m_v3CamPosTarget;
//		printCamPos("OnRenderObject DOWN ");
	}
	
	public void AttachTo(Transform tran, Vector3 localPosition)
	{
		m_MotherPos = Vector3.zero;
		m_MotherTransform = tran;
		mainCamera.transform.parent = tran;
		mainCamera.transform.localPosition = localPosition;
		mainCamera.transform.LookAt(tran);
		m_LocalY = localPosition.y;
		m_LocalDis = Mathf.Sqrt(localPosition.x * localPosition.x + localPosition.z * localPosition.z);
		relaPosition = mainCamera.transform.position - m_MotherTransform.transform.position;
		m_fWheelDelta = 0f;
	}
	
	// 根据当前的摄像机矩形信息, 置摄像机到另一个点上
	public void AttachTo(Transform tran)
	{
		if(null == tran) 
			return;
		m_MotherTransform = tran;
		mainCamera.transform.position = tran.position + m_relaPosition;
		mainCamera.transform.parent = tran;
		mainCamera.transform.LookAt(tran);
	}
	
	public void AdjustCamera(Vector3 localPos,Vector3 rot)
	{
		mainCamera.transform.localPosition 	= localPos;
		mainCamera.transform.localRotation	= Quaternion.Euler(rot);
	}
	
	public void AttachTo(Vector3 pos, Vector3 localPosition)
	{
		m_MotherTransform = null;
		m_MotherPos = pos;
		mainCamera.transform.parent = LogicApp.SP.transform;
		mainCamera.transform.position = pos + localPosition;
		mainCamera.transform.LookAt(pos);
		m_LocalY = localPosition.y;
		m_LocalDis = Mathf.Sqrt(localPosition.x * localPosition.x + localPosition.z * localPosition.z);
		m_fWheelDelta = 0f;	
		relaPosition = mainCamera.transform.position - pos;
	}
	
	public void SetCamera(Vector3 pos,Vector3 rotation)
	{
		m_MotherTransform = null;
		m_MotherPos = new Vector3(0,0,0);
		mainCamera.transform.parent 	= LogicApp.SP.transform;
		mainCamera.transform.position	= pos;
		mainCamera.transform.rotation	= Quaternion.Euler(rotation);
	}
	
	private void cameraCollider()
	{
//		if(!IsEnableCameraCollide)
//			return ;
//		
//		m_v3CamPosTarget = mainCamera.transform.position;
//				
//		RaycastHit hitInfo;
//		
//		Vector3 startPos = XLogicWorld.SP.MainPlayer.Position;
//		startPos.y	+= 1.5f;
//		
//		Vector3 dir = mainCamera.transform.position - startPos;
//		
//		Ray dirRay = new Ray(startPos,dir);	//post ray
//		
//		Physics.SphereCast(dirRay,			//
//			0.5f,
//			out hitInfo,
//			dir.magnitude,
//			1<<GlobalU3dDefine.Layer_TerrainObject
//			);
//		
//		if(null == hitInfo.collider )
//			return;
//		
//		//hitInfo.point is not on the dir vector,may be other bit offset from the dir point
//		Vector3 zoomNewPos = dirRay.GetPoint( hitInfo.distance-1.0f );
//		
//		mainCamera.transform.position = zoomNewPos;
	}
	
	public void Detach()	// 保留矩形信息
	{
		m_MotherPos = Vector3.zero;
		m_MotherTransform = null;
		mainCamera.transform.parent = LogicApp.SP.transform;
	}
	
	//  水平圆圈转动
	public void Rotate(float fMouseMoveX)
	{
		if(null == m_MotherTransform && Vector3.zero == m_MotherPos)
			return;
			
		Vector3 vec = m_MotherPos;
		if(null != m_MotherTransform) 
			vec = m_MotherTransform.position;
		float fRotateDelta = fMouseMoveX * LogicApp.SP.UserDefine.MainCameraRotateSpeed * Time.deltaTime;
		mainCamera.transform.RotateAround(vec, Vector3.up, fRotateDelta);
		Vector3 pos;
		
		relaPosition = mainCamera.transform.position - vec;
	}
	
	// 按轨迹朝着观察对象移动摄像机
	public void Scroll(float f, bool bRate)
	{
		if(bRate)
		{
			scroll(f);
		}
		else
		{
			if(m_fWaitWD > 0 != f > 0)
				m_fWaitWD = f;
			else
				m_fWaitWD += f;
		}
	}
	
	private bool scroll(float fd)
	{
		if(0f == fd || null == m_MotherTransform && Vector3.zero == m_MotherPos)
			return false;
		
		float nwd = m_fWheelDelta + fd;
		if(nwd < 0f) nwd = 0f;
		else if(nwd > 0.6f) nwd = 0.6f;
		if(nwd == m_fWheelDelta) 
			return false;
		m_fWheelDelta = nwd;

		AnimationCurve curve = LogicApp.SP.UserDefine.MainCameraYCurve;
		float fv = curve.Evaluate(m_fWheelDelta);
		float fy = m_LocalY * fv;
		Vector3 mPos = m_MotherPos;
		if(null != m_MotherTransform)
			mPos = m_MotherTransform.position;
		Vector3 vec = mainCamera.transform.position - mPos;
		float fs = Mathf.Sqrt(vec.x * vec.x + vec.z * vec.z);
		float fx = vec.x * (m_LocalDis * (1f - m_fWheelDelta) / fs);
		float fz = vec.z * (m_LocalDis * (1f - m_fWheelDelta) / fs);
		mainCamera.transform.position = mPos + new Vector3(fx, fy, fz);
		mainCamera.transform.LookAt(mPos);
		relaPosition = mainCamera.transform.position - mPos;
		return true;
	}
	
	public void ChangeFOV(float fov)
	{
		if(mainCamera == null)
			return ;
		
		mainCamera.fieldOfView	= fov;
	}
}