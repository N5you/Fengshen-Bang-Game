using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("NGUI/Interaction/UISpriteGroup")]
public class UISpriteGroup : MonoBehaviour
{
	public UIAtlas m_Atlas;
	public string  m_PressSpriteName;
	public delegate void OnSelectModify(int index);
	public OnSelectModify	mModify;
	
	private class SingleSprite
	{
		public delegate void OnSelect(int index);
		public UISprite m_Sprite;
		private UILabel	 m_Label;
		private int 	 m_nIndex;
		private GameObject	m_GO;
		public OnSelect  onSelect;
		private string	 m_PressSpriteName;
		private string	 m_OldSpriteName;
		private Color	m_OrignalColor;
		
		public SingleSprite(GameObject root, int nIndex,string PressSpriteName,OnSelect onClickCallBack)
		{
			m_GO				= root;
			m_Sprite 			= root.GetComponentsInChildren<UISprite>(true)[0];
			UILabel[] labelArray = root.GetComponentsInChildren<UILabel>(true);
			if(labelArray.Length > 0)
			{
				m_Label				= labelArray[0];
				m_OrignalColor		= m_Label.color;
			}
			m_nIndex 			= nIndex;
			m_PressSpriteName	= PressSpriteName;
			m_OldSpriteName		= m_Sprite.spriteName;
			onSelect			+= onClickCallBack;
			UIEventListener listener = UIEventListener.Get(m_Sprite.gameObject);
			listener.onClick += OnClickSprite;			
		}
		
		public void OnClickSprite (GameObject go)
		{
			if(onSelect != null)
				onSelect(m_nIndex);
		}
		
		public void Select()
		{
			m_Sprite.spriteName	= m_PressSpriteName;
			if(m_Label != null)
				m_Label.color	= new Color32(255,243,55,255);
		}
		
		public void UnSelect()
		{
			m_Sprite.spriteName	= m_OldSpriteName;
			if(m_Label != null)
				m_Label.color	= m_OrignalColor;
		}
		
		public void SetVisible(bool isShow)
		{
			m_GO.SetActive(isShow);
		}
		
		public void SetText(string str)
		{
			if(m_Label != null)
				m_Label.text	= str;
		}
	}
	
	public GameObject[] m_GOArray;
	private List<SingleSprite> m_SpriteList;
	private int m_nCurrentSelect = 0;
	
	public int CurrentSelect 
	{ 
		get 
		{ 
			return m_nCurrentSelect; 
		}
		set
		{
			if(0 > value || value >= m_SpriteList.Count)
				return;
			OnSelect(value);
		}
	}
	
	public void ReSet()
	{
		Select(0);
	}
	
	public UISprite GetSprite(int index)
	{
		if(m_SpriteList.Count <= index)
			return null;	
		return m_SpriteList[index].m_Sprite;
	}
	
	public void Select(int index)
	{
		if(index >= m_SpriteList.Count)
			return ;
		for(int i = 0;i < m_SpriteList.Count; i++)
		{
			if(i == index)
			{
				m_SpriteList[i].Select();
			}
			else
				m_SpriteList[i].UnSelect();
		}
	}
	
	public void OnSelect(int index)
	{
		for(int i = 0;i < m_SpriteList.Count; i++)
		{
			if(i == index)
			{
				m_SpriteList[i].Select();
			}
			else
				m_SpriteList[i].UnSelect();
		}
		
		if(mModify != null)
			mModify(index);
	}
	
	void Awake ()
	{
		m_SpriteList = new List<SingleSprite>();
		for(int i=0; i<m_GOArray.Length; i++)
		{
			m_SpriteList.Add(new SingleSprite(m_GOArray[i], i,m_PressSpriteName,OnSelect));
		}
		if ( m_GOArray.Length > 0 )
			m_SpriteList[0].Select();
	}
	
	public void SetVisible(int count)
	{
		int realCount = count >= m_SpriteList.Count ? m_SpriteList.Count : count;
		
		if(count >= m_GOArray.Length)
		{
			for(int i = 0; i < m_SpriteList.Count; i++)
			{
				m_SpriteList[i].SetVisible(true);
			}
		}
		else
		{
			for(int i = 0; i < m_SpriteList.Count; i++)
			{
				if(m_SpriteList[i] == null)
					continue ;
				if(i < count)
					m_SpriteList[i].SetVisible(true);					
				else
					m_SpriteList[i].SetVisible(false);
			}
		}
		
	}
	
	public void SetText(int index,string str)
	{
		m_SpriteList[index].SetText(str);
	}
	
	public void SetSelect(int index, bool sel)
	{
		if ( index < 0 || index > m_SpriteList.Count )
			return;
		
		if ( sel )
			m_SpriteList[index].Select();
		else
			m_SpriteList[index].UnSelect();
	}
}


