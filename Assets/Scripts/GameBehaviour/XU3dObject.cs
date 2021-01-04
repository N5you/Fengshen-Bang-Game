using UnityEngine;
using System.Collections.Generic;

public class XU3dObject
{
	public GameObject m_gameObject { get; private set; }
	private Vector3 m_position;			// 绝对坐标
	protected Vector3 m_rotation;		// 绝对旋转
	private Vector3 m_scale;			// 相对缩放
	private Transform m_parent;			// 父节点
	private int m_layer;				// 层
	private bool m_bVisible;
	private List<GameObject> m_children; // 子对象
	
	public virtual bool Visible
	{
		get 
		{ 
			if(null != m_gameObject)
				m_bVisible = m_gameObject.activeSelf;
			return m_bVisible; 
		}
		set 
		{
			m_bVisible = value;
			if(null != m_gameObject && m_gameObject.activeSelf != m_bVisible)
			{
				m_gameObject.SetActive(m_bVisible);
			}
		}
	}
	
	public Vector3 Position
	{
		get 
		{ 
			if(null != m_gameObject)
				m_position = m_gameObject.transform.position;			
			return m_position; 
		}
		set
		{
			m_position = value;
			if(null != m_gameObject)
			{
				m_gameObject.transform.position = m_position;
			}
		}
	}
	
	public Vector3 LocalPosition
	{
		get
		{
			if(null == Parent) 
				return Position;
			return Parent.InverseTransformPoint(Position);
		}
		set
		{
			if(null == Parent)
				Position = value;
			else
				Position = Parent.TransformPoint(value);
		}
	}
		
	public Vector3 Direction
	{
		get 
		{ 
			if(null != m_gameObject)
				m_rotation = m_gameObject.transform.eulerAngles;
			return m_rotation; 
		}
		set
		{
			m_rotation = value;
			if(null != m_gameObject)
			{
				m_gameObject.transform.rotation = Quaternion.Euler(m_rotation);
			}
		}
	}
	
	public Vector3 LocalDirection
	{
		get 
		{
			if(null == Parent)
				return Direction;
			return Direction - Parent.transform.eulerAngles;
		}
		set
		{
			if(null == Parent)
				Direction = value;
			else
				Direction = Parent.transform.eulerAngles + value;
		}
	}
	
	public Transform Parent
	{
		get 
		{ 
			if(null != m_gameObject)
				m_parent = m_gameObject.transform.parent;
			return m_parent; 
		}
		set 
		{
			m_parent = value;
			if(null != m_gameObject)
			{
				m_gameObject.transform.parent = m_parent;
			}
		}
	}
	
	public virtual Vector3 Scale
	{
		get 
		{
			if(null != m_gameObject)
				m_scale = m_gameObject.transform.localScale;
			return m_scale; 
		}
		set 
		{
			m_scale = value;
			if(null != m_gameObject)
			{
				m_gameObject.transform.localScale = m_scale;
			}
		}
	}
	
	public int Layer
	{
		get 
		{ 
			if(null != m_gameObject)
				m_layer = m_gameObject.layer;
			return m_layer; 
		}
		set 
		{
			m_layer = value;
			if(null != m_gameObject)
			{
				XUtil.SetLayer(m_gameObject, m_layer);
			}
		}
	}
	
	public XU3dObject()
	{
		m_gameObject = null;
		m_bVisible = true;
		m_position = Vector3.zero;
		m_rotation = Vector3.zero;
		m_scale = Vector3.one;
		m_parent = null;
		m_children = new List<GameObject>();
	}
	
	public void NewGameObject(string name)
	{
		restoreInfo();
		detachAll();
		if(null != m_gameObject)
			GameObject.Destroy(m_gameObject);
		m_gameObject = new GameObject(name);
		resetGameObject();
		reAttachAll();
	}
	
	public void CopyGameObject(GameObject go)
	{
		restoreInfo();
		if(null == go) return;
		detachAll();
		if(null != m_gameObject) 
			GameObject.Destroy(m_gameObject);
		m_gameObject = GameObject.Instantiate(go) as GameObject;
		resetGameObject();
		reAttachAll();
	}
	
	public void AttachGameObject(GameObject go)
	{
		AttachGameObject(go, Vector3.zero, Vector3.zero);
	}
	
	public void AttachGameObject(GameObject go, Vector3 localPosition, Vector3 localRotation)
	{
		if(null == go || null == m_gameObject) return;
		go.transform.parent = m_gameObject.transform;
		go.transform.localPosition = localPosition;
		go.transform.localRotation = Quaternion.Euler(localRotation);
		m_children.Add(go);
	}
	
	public void AttachU3dObject(XU3dObject u3dObject)
	{
		AttachU3dObject(u3dObject, Vector3.zero, Vector3.zero);
	}
	
	public void AttachU3dObject(XU3dObject u3dObject, Vector3 localPosition, Vector3 localRotaion)
	{
		if(null == u3dObject) return;
		AttachGameObject(u3dObject.m_gameObject, localPosition, localRotaion);
	}
	
	private void restoreInfo()
	{
#pragma warning disable
		bool b = Visible;
		Vector3 vec = Position;
		vec = Direction;
		Transform t = Parent;
		vec = Scale;
		int n = Layer;
#pragma warning restore
	}
	
	private void detachAll()
	{
		for(int i=0; i<m_children.Count;)
		{
			GameObject go = m_children[i];
			if(null != go && go.transform.parent == m_gameObject.transform)
			{
				go.transform.parent = null;
				i++;
				continue;
			}
			m_children.RemoveAt(i);
		}
	}
	
	private void reAttachAll()
	{
		for(int i=0; i<m_children.Count;)
		{
			GameObject go = m_children[i];
			if(null != go)
			{
				go.transform.parent = m_gameObject.transform;
				i++;
				continue;
			}
			m_children.RemoveAt(i);
		}
	}
	
	// 重新设置 position / rotation / parent / layer / children
	private void resetGameObject()
	{
		if(null == m_gameObject) return;
		m_gameObject.transform.parent = m_parent;
		m_gameObject.transform.position = m_position;
		m_gameObject.transform.rotation = Quaternion.Euler(m_rotation);
		m_gameObject.transform.localScale = m_scale;
		XUtil.SetLayer(m_gameObject, m_layer);
		for(int i=0; i<m_children.Count;)
		{
			GameObject go = m_children[i];
			if(null == go)
			{
				m_children.RemoveAt(i);
				continue;
			}
			go.transform.parent = m_gameObject.transform;
			i++;
		}
		if(!m_bVisible) m_gameObject.SetActive(false);
		Object.DontDestroyOnLoad(m_gameObject);
	}
	
	public void Destroy()
	{
		Destroy(false);
	}
	
	public virtual void Destroy(bool bDestroyAll)
	{
		if(null == m_gameObject) 
			return;
		
		if(!bDestroyAll)
		{
			for(int i=0; i<m_children.Count; i++)
			{
				GameObject go = m_children[i];
				if(null != go && go.transform.parent == m_gameObject.transform)
				{
					go.transform.parent = null;
				}
			}
		}
		m_children.Clear();
		GameObject.Destroy(m_gameObject);
	}
}

