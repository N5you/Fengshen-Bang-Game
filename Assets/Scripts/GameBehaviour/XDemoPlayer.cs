using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class XDemoPlayer : MonoBehaviour
{
	public enum EAnimName
	{
		Idle,
		Walk,
		Run,
		Jump,
		Drop,
		DropRun,
		DropStand,
		Attack1,
		Attack2,
		Attack3,
		Attack4,
		Cast,
		Death,
		Deffence,
		Dodge,
		Fight,
		Shoot,
		Sit,
		Wound,
		e_Count,
		
	}
	
	public 	Animation			Anim = null;
	private	Camera				m_MainCamera = null;
	private	CharacterController	m_CharCtrl = null;
	private GameObject			m_BindCamera = null;
	
	//private float mTotalScrollValue = 1f;
	//private float mCurrentScrollValue = 0;
	
	// 战斗视角
	public bool FightCamera = false;
	private bool m_bFightCamera = false;

	// 摄像机逻辑
	public bool	LockCamera = false;
	private bool m_bLockCamera = false;
	public Vector3 CameraBornPosition = new Vector3(0, 20, -20);
		
	private bool ViewAdjust = false;
	public float CameraMoveSpeed = 500.0f;
	public float CameraRotateXSpeed = 150.0f;
	public float CameraRotateYSpeed = 180.0f;
	
	// Position and Rotation
	private float	m_fAngleYDur = 0f;
	private Vector2 m_vecTgtDir = Vector2.zero;
	public float RotateSpeed = 360f;
	
	// Move
	private bool m_bRunSpeed = true;		// true: Run false: Walk
	public float DefaultRunSpeed = 5.0f;		// 默认跑步速度
	public float RunSpeed = 5.0f;
	public float WalkSpeed = 1.5f;
	
	public float DropSpeed = 2.6f;
	public float JumpRiseTime = 0.5f;
	
	private float m_fJumpRockon = 0;
	private int  m_nJumpState = 0;			// 0: none, 1: 上升, 2: 降落
	
	//Anim
	private float[,] m_AnimCrossTable = new float[(int)EAnimName.e_Count, (int)EAnimName.e_Count];
	
	
	public string Hint1 = "1 -> Attack1";
	public string Hint2 = "2 -> Attack2";
	public string Hint3 = "3 -> Attack3";
	public string Hint4 = "4 -> Attack4";
	public string Hint5 = "5 -> Death";
	public string Hint6 = "6 -> Deffence";
	public string Hint7 = "7 -> Dodge";
	public string Hint8 = "8 -> Fight";
	public string Hint9 = "9 -> Cast";
	public string Hint10 = "+ -> Sit";
	public string Hint11 = "- -> Wound";
	
	public void Start()
	{
		m_MainCamera = Camera.main;
	
		if(null == Anim || null == m_MainCamera) 
			Application.Quit();
		
		GameObject go = Instantiate(Anim.gameObject) as GameObject;
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		Anim = go.GetComponent(typeof(Animation)) as Animation;
		
		m_CharCtrl = Anim.GetComponent(typeof(CharacterController)) as CharacterController;
		if(null == m_CharCtrl)
		{
			Log.Write(LogLevel.WARN, "[WARN] XDemoPlayer: CharCtrl is null");
			Application.Quit();
		}
		
		CharacterController cc = gameObject.AddComponent(typeof(CharacterController)) as CharacterController;
		cc.center = m_CharCtrl.center;
		cc.radius = m_CharCtrl.radius;
		cc.height = m_CharCtrl.height;
		Destroy(m_CharCtrl);
		m_CharCtrl = cc;
		
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
		for(int i=0; i<trans.Length; i++)
		{
			if(trans[i].name == "Bip01 Spine2")
			{
				m_BindCamera = new GameObject("BindCamera");
				m_BindCamera.transform.parent = transform;
				m_BindCamera.transform.position = trans[i].position;
				break;
			}
		}
		if(null == m_BindCamera) 
			m_BindCamera = gameObject;
		
		// 摄像机固定绑点
//		go = new GameObject("BindCamera");
//		go.transform.parent = Anim.gameObject.transform;
//		go.transform.localScale = Vector3.one;
//		go.transform.localPosition = new Vector3(0f, 1.2f, 0f);
//		go.transform.localRotation = Quaternion.Euler(Vector3.zero);
//		m_BindCamera = go;
		
		m_MainCamera.transform.parent = m_BindCamera.transform;
		m_MainCamera.transform.localPosition = CameraBornPosition;
		m_MainCamera.transform.LookAt(m_BindCamera.transform);
		
		
		for (int i = 0; i < (int)EAnimName.e_Count; ++i)
        {
            for (int j = 0; j < (int)EAnimName.e_Count; ++j)
            {
                m_AnimCrossTable[i, j] = 0.25f;
            }
        }
		
        m_AnimCrossTable[(int)EAnimName.Idle, (int)EAnimName.Run] = 0.2f;
        m_AnimCrossTable[(int)EAnimName.Idle, (int)EAnimName.Jump] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.Run, (int)EAnimName.Jump] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.Run, (int)EAnimName.Idle] = 0.2f;
        m_AnimCrossTable[(int)EAnimName.Drop, (int)EAnimName.DropStand] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.Drop, (int)EAnimName.DropRun] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.DropRun, (int)EAnimName.Run] = 0.1f;
		
		if(!m_CharCtrl.isGrounded)
		{
			m_nJumpState = 2;
			PlayAnimation(EAnimName.Drop, 0);
		}
		
#if UNITY_EDITOR
		FightCamera = EditorPrefs.GetBool("XDemoPlayer_FightCamera");
		LockCamera = EditorPrefs.GetBool("XDemoPlayer_LockCamera");
#endif
	}
	
	public void Update()
	{
		// 校准Run和Walk的速度
		Anim["Run"].speed = RunSpeed / DefaultRunSpeed;
		//Anim["Walk"].speed = WalkSpeed / 1.5f;
		float cur	 = Input.GetAxis("Mouse ScrollWheel");
//		if((mCurrentScrollValue + cur) < -mTotalScrollValue)
//			cur	= 0;
//		else if(mCurrentScrollValue + cur > mTotalScrollValue)
//			cur	= 0;
//		else
//		{
//			mCurrentScrollValue	+= cur;
//		}
			
		m_MainCamera.transform.Translate(0, 0, cur * Time.deltaTime * CameraMoveSpeed);
		
		// 战斗视角
		if(m_bFightCamera != FightCamera)
		{
			if(FightCamera)
			{
				gameObject.transform.position = new Vector3(Terrain.activeTerrain.terrainData.size.x / 2, 1000f, Terrain.activeTerrain.terrainData.size.z / 2);
				while(true)
				{
					m_CharCtrl.Move(new Vector3(0, -20, 0) * Time.deltaTime);
					if(m_CharCtrl.isGrounded || gameObject.transform.position.y < -1000f) break;
				}
			}
			m_MainCamera.transform.localPosition = BattleDisplayerMgr.BATTLE_CAMERA_OFFSET;
			Vector3 newLook = gameObject.transform.position;
			newLook.y+=1.5f;
			m_MainCamera.transform.LookAt(newLook );
			m_MainCamera.fieldOfView	= BattleDisplayerMgr.BATTLE_CAMERA_FOV;
			m_bFightCamera = FightCamera;
			PlayAnimation(EAnimName.Idle, 0);
#if UNITY_EDITOR
			EditorPrefs.SetBool("XDemoPlayer_FightCamera", FightCamera);
#endif
		}
		
		if(FightCamera) return;
		
		if(m_bLockCamera != LockCamera)			// 锁定摄像机
		{
			if(LockCamera)
			{
				m_MainCamera.transform.localPosition = CameraBornPosition;
				
				m_MainCamera.transform.LookAt(m_BindCamera.transform);
				//mCurrentScrollValue	= 0;
			}
			m_bLockCamera = LockCamera;
#if UNITY_EDITOR
			EditorPrefs.SetBool("XDemoPlayer_LockCamera", LockCamera);
#endif
		}
		
		
		if(Input.GetMouseButtonDown(1))
		{
			Cursor.visible = false;
			ViewAdjust = true;
		}
		else if(Input.GetMouseButtonUp(1))
		{
			Cursor.visible = true;
			ViewAdjust = false;
		}
		if(ViewAdjust && !LockCamera)
		{
			m_MainCamera.transform.RotateAround(m_BindCamera.transform.position
				, Vector3.up
				, CameraRotateXSpeed * Input.GetAxis("Mouse X") * Time.deltaTime);
			m_MainCamera.transform.RotateAround(m_BindCamera.transform.position
				, m_MainCamera.transform.TransformDirection(Vector3.right)
				, -CameraRotateYSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime);
		}
		
		// Key
		if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) AlterDirection(Vector2.up);
		if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) AlterDirection(-Vector2.right);
		if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyUp(KeyCode.W)) AlterDirection(-Vector2.up);
		if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) AlterDirection(Vector2.right);	
		
		if(0f != m_fAngleYDur)
		{
			float f = Time.deltaTime * RotateSpeed;
			if(f > Mathf.Abs(m_fAngleYDur)) f = Mathf.Abs(m_fAngleYDur);
			if(m_fAngleYDur < 0f) f *= -1;
			Anim.transform.Rotate(new Vector3(0, f, 0), Space.World);
			m_fAngleYDur -= f;
		}
		
		if(Vector2.zero != m_vecTgtDir)
		{
			m_CharCtrl.Move(Anim.transform.forward * Time.deltaTime * Speed);
		}
		
		if(2 == m_nJumpState)
		{
			m_CharCtrl.Move(new Vector3(0, -DropSpeed, 0) * Time.deltaTime);
			if(m_CharCtrl.isGrounded)
			{
				m_nJumpState = 0;
				if(Vector2.zero == m_vecTgtDir)
				{
					PlayAnimation(EAnimName.DropStand, GetCrossTime(EAnimName.Drop, EAnimName.DropRun));
					PushAnimation(EAnimName.Idle, GetCrossTime(EAnimName.DropStand, EAnimName.Idle));

				}
				else if(m_bRunSpeed)
				{
					PlayAnimation(EAnimName.DropRun, GetCrossTime(EAnimName.Drop, EAnimName.DropRun));
					PushAnimation(EAnimName.Run, GetCrossTime(EAnimName.DropRun, EAnimName.Run));
				}
				else
				{
					PlayAnimation(EAnimName.DropStand, GetCrossTime(EAnimName.Drop, EAnimName.DropStand));
					PushAnimation(EAnimName.Walk, GetCrossTime(EAnimName.DropStand, EAnimName.Walk));
				}
			}
		}
		
		if(1 == m_nJumpState)
		{
			if(Time.time - m_fJumpRockon > JumpRiseTime)
			{
				m_nJumpState = 2;
			}
			else
			{
				m_CharCtrl.Move(new Vector3(0, DropSpeed, 0) * Time.deltaTime);	
			}
		}
		
		if(0 == m_nJumpState) m_CharCtrl.Move(new Vector3(0, -20, 0) * Time.deltaTime);
		
		// Key
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(0 == m_nJumpState)
			{
				PlayAnimation(EAnimName.Jump, GetCrossTime(EAnimName.Idle, EAnimName.Jump));
				PushAnimation(EAnimName.Drop, GetCrossTime(EAnimName.Jump, EAnimName.Drop));
				m_nJumpState = 1;
				m_fJumpRockon = Time.time;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			if(0 == m_nJumpState && Vector2.zero != m_vecTgtDir) AlterDirection(-m_vecTgtDir);
		}
		
		
		if(Input.GetKeyDown(KeyCode.Keypad1)) TestAnimation(EAnimName.Attack1);
		if(Input.GetKeyDown(KeyCode.Keypad2)) TestAnimation(EAnimName.Attack2);
		if(Input.GetKeyDown(KeyCode.Keypad3)) TestAnimation(EAnimName.Attack3);
		if(Input.GetKeyDown(KeyCode.Keypad4)) TestAnimation(EAnimName.Attack4);
		if(Input.GetKeyDown(KeyCode.Keypad5)) TestAnimation(EAnimName.Death);
		if(Input.GetKeyDown(KeyCode.Keypad6)) TestAnimation(EAnimName.Deffence);
		if(Input.GetKeyDown(KeyCode.Keypad7)) TestAnimation(EAnimName.Dodge);
		if(Input.GetKeyDown(KeyCode.Keypad8)) TestAnimation(EAnimName.Fight);
		if(Input.GetKeyDown(KeyCode.Keypad9)) TestAnimation(EAnimName.Shoot);
		if(Input.GetKeyDown(KeyCode.KeypadPlus))	 TestAnimation(EAnimName.Sit);
		if(Input.GetKeyDown(KeyCode.KeypadMinus))	 TestAnimation(EAnimName.Wound);
	}
	
	private void AlterDirection(Vector2 vec)
	{
		if(Vector2.zero == m_vecTgtDir && 0 == m_nJumpState)
		{
			if(m_bRunSpeed) PlayAnimation(EAnimName.Run, GetCrossTime(EAnimName.Idle, EAnimName.Run));
			else PlayAnimation(EAnimName.Walk, GetCrossTime(EAnimName.Idle, EAnimName.Walk));
		}
		
		m_vecTgtDir += vec;
		if(Vector2.zero == m_vecTgtDir)
		{
			m_fAngleYDur = 0f;
			if(0 == m_nJumpState)
			{
				if(m_bRunSpeed) PlayAnimation(EAnimName.Idle, GetCrossTime(EAnimName.Run, EAnimName.Idle));
				else PlayAnimation(EAnimName.Idle, GetCrossTime(EAnimName.Walk, EAnimName.Idle));
			}
			return;
		}
		
		float f1 = m_MainCamera.transform.rotation.eulerAngles.y;
		float f2 = Quaternion.FromToRotation(Vector3.forward, new Vector3(m_vecTgtDir.x, 0, m_vecTgtDir.y)).eulerAngles.y;
		float f3 = Anim.transform.rotation.eulerAngles.y;
		m_fAngleYDur = f1 + f2 - f3;
		while(m_fAngleYDur > 180f) m_fAngleYDur -= 360;
		while(m_fAngleYDur < -180f) m_fAngleYDur += 360;
	}
	
	private void PlayAnimation(EAnimName anim, float fTime)
	{
		if(null == Anim) return;
		if(!Anim.isPlaying) Anim.Play(anim.ToString());
		else Anim.CrossFade(anim.ToString(), fTime);
	}
	
	private void PushAnimation(EAnimName anim, float fTime)
	{
		if(null == Anim) return;
		Anim.CrossFadeQueued(anim.ToString(), fTime);
	}
	
	private void TestAnimation(EAnimName anim)
	{
		if(0 != m_nJumpState || Vector2.zero != m_vecTgtDir) return;
		PlayAnimation(anim, GetCrossTime(EAnimName.Idle, anim));
		PushAnimation(EAnimName.Idle, GetCrossTime(anim, EAnimName.Idle));
	}
				
	private float GetCrossTime(EAnimName anim1, EAnimName anim2)
	{
		return m_AnimCrossTable[(int)anim1, (int)anim2];
	}
	
	private float Speed { get { return m_bRunSpeed ? RunSpeed : WalkSpeed; }}
}

