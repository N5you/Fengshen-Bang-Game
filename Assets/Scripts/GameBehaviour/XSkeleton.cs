using UnityEngine;
using System.Collections;

public enum ESkeleton
{
	eMainObject = 0,	// 原点
	eCapsuleBottom = 0,	// 包围盒底, 不转
	eHeadCenter,		// 头部
	eChest,				// 胸部
	eWaist,				// 腰部
	eLeftHand,			// 左手
	eRightHand,			// 右手
	eLeftFoot,			// 左脚
	eRightFoot,			// 右脚
	eLeftForwardFoot,	// 左前足
	eRightForwardFoot,	// 右前足
	eCapsuleTop,		// 包围盒顶, 不转
	eCapsuleHalf,		// 包围盒中, 不转
	eMountZuo,			// 坐骑挂接点_坐姿
	eMountFei,			// 坐骑挂接点_飞行
	eMountZhan,			// 坐骑挂接点_站立
	eWeapon,			// 武器挂接点
	eMount,				// 坐骑挂接点_角色
	eCameraBind,		// 摄像机绑点
	eCount,
}

public class XSkeleton
{
	private Transform m_mainTran = null;
	private Transform[] m_trans;
	
	public XSkeleton(GameObject gameObject, CharacterController charCtrl)
	{
		GameObject go = null;
		m_trans = new Transform[(int)ESkeleton.eCount];
		Transform[] trans = gameObject.GetComponentsInChildren<Transform>(true);
		for(int i=0; i<trans.Length; i++)
		{
			if(trans[i].name == "Bip01 Head")
			{
				m_trans[(int)ESkeleton.eHeadCenter] = trans[i];
			}
			else if(trans[i].name == "Bip01 Spine2")
			{
				m_trans[(int)ESkeleton.eChest] = trans[i];
			
				/*go = new GameObject("CameraBind");
				go.transform.parent = gameObject.transform;
				go.transform.localScale = Vector3.one;
				go.transform.position = trans[i].position;
				go.transform.localRotation = Quaternion.Euler(Vector3.zero);
				m_trans[(int)ESkeleton.eCameraBind] = go.transform;*/
			}
			else if(trans[i].name == "Bip01 Spine1")
			{
				if(null == m_trans[(int)ESkeleton.eChest])
				{
					m_trans[(int)ESkeleton.eChest] = trans[i];
				}
			}
			else if(trans[i].name == "Bip01 Spine")
			{
				m_trans[(int)ESkeleton.eWaist] = trans[i];
			}
			else if(trans[i].name == "Bip01 L Hand")
			{
				m_trans[(int)ESkeleton.eLeftHand] = trans[i];
			}
			else if(trans[i].name == "Bip01 R Hand")
			{
				m_trans[(int)ESkeleton.eRightHand] = trans[i];
			}
			else if(trans[i].name == "Bip01 L Foot")
			{
				m_trans[(int)ESkeleton.eLeftFoot] = trans[i];
			}
			else if(trans[i].name == "Bip01 R Foot")
			{
				m_trans[(int)ESkeleton.eRightFoot] = trans[i];
			}
			else if(trans[i].name == "Bip01 L Finger0")
			{
				m_trans[(int)ESkeleton.eLeftForwardFoot] = trans[i];
			}
			else if(trans[i].name == "Bip01 R Finger0")
			{
				m_trans[(int)ESkeleton.eRightForwardFoot] = trans[i];
			}
			else if(trans[i].name == "Bone_Zuoqi")
			{
				go = new GameObject("Bond_Zuoqi");
				go.transform.parent = gameObject.transform;
				go.transform.position = trans[i].transform.position;
				go.transform.rotation = trans[i].transform.rotation;
				m_trans[(int)ESkeleton.eMountZuo] = go.transform;
			}
			else if(trans[i].name == "Bone_Chibang")
			{
				go = new GameObject("Bone_Chibang");
				go.transform.parent = gameObject.transform;
				go.transform.position = trans[i].transform.position;
				go.transform.rotation = trans[i].transform.rotation;
				m_trans[(int)ESkeleton.eMountFei] = go.transform;
			}
			else if(trans[i].name == "Bone_Yun")
			{
				go = new GameObject("Bone_Yun");
				go.transform.parent = gameObject.transform;
				go.transform.position = trans[i].transform.position;
				go.transform.rotation = trans[i].transform.rotation;
				m_trans[(int)ESkeleton.eMountZhan] = go.transform;
			}
			else if(trans[i].name == "Bone_Staff" || trans[i].name == "Bone_Bow" || trans[i].name == "Bone_Sword")
			{
				m_trans[(int)ESkeleton.eWeapon] = trans[i];
			}
			else if(trans[i].name == "Bone_Mount")
			{
				m_trans[(int)ESkeleton.eMount] = trans[i];
			}
		}
		m_mainTran = gameObject.transform;
		
		// 摄像机固定绑点
		go = new GameObject("CameraBind");
		go.transform.parent = gameObject.transform;
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = new Vector3(0f, 1.2f, 0f);
		go.transform.localRotation = Quaternion.Euler(Vector3.zero);
		m_trans[(int)ESkeleton.eCameraBind] = go.transform;
		
		go = new GameObject("CapsuleBottom");
		go.transform.parent = gameObject.transform;
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.Euler(Vector3.zero);
		m_trans[(int)ESkeleton.eCapsuleBottom] = go.transform;

		if(null != charCtrl)
		{
			go = new GameObject("CapsuleHalf");
			go.transform.parent = gameObject.transform;
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = new Vector3(0f, charCtrl.center.y, 0f);
			go.transform.localRotation = Quaternion.Euler(Vector3.zero);
			m_trans[(int)ESkeleton.eCapsuleHalf] = go.transform;

			go = new GameObject("CapsuleTop");
			go.transform.parent = gameObject.transform;
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = new Vector3(0f, charCtrl.height / 2f + charCtrl.center.y, 0f);
			go.transform.localRotation = Quaternion.Euler(Vector3.zero);
			m_trans[(int)ESkeleton.eCapsuleTop] = go.transform;			
		}
	}
	
	public Transform GetPoint(ESkeleton s)
	{
		if(0 > (int)s || (int)ESkeleton.eCount < (int)s)
			return null;
		
		Transform tran = m_trans[(int)s];
		if(null == tran) return m_mainTran;
		return tran;
	}
	
	public void AddPoint(ESkeleton s, Transform trans)
	{
		if(0 > (int)s || (int)ESkeleton.eCount < (int)s || null == trans)
			return;
		
		if(null == m_trans[(int)s])
		{
			GameObject go = new GameObject(s.ToString());
			go.transform.parent = m_trans[(int)ESkeleton.eMainObject];
			m_trans[(int)s] = go.transform;
		}
		m_trans[(int)s].transform.position = trans.position;
		m_trans[(int)s].transform.rotation = trans.rotation;
	}
}

