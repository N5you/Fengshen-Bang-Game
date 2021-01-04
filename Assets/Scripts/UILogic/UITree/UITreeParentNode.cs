using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//tree node manager all of the node obj
public class UITreeParentNode: MonoBehaviour 
{	
	//demo obj public to editor
	public GameObject m_ChildrenListDemo = null;
	
	public GameObject m_ChildrenControlDemo = null;
	
	public GameObject m_ChildDemo = null;
	
	public GameObject m_controlObj = null;
	
	public GameObject m_controlCheckBox = null;
	
	public GameObject m_Children = null;
	
	public GameObject m_BaseObj = null;
	
	public bool m_OptionCanBeDone = false;
	
	//public SortedList<uint,GameObject> m_childrenList;
	
	public UITree m_treeManager = null;
	
	private bool m_needOpenChildren = true;
	
	private uint m_uiCurrentID = 0;
	
	private SortedList<uint,GameObject > m_childObjList = new SortedList<uint, GameObject>();
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void initParentNode(UITree tree,string parentName)
	{
		m_treeManager = tree;
		
		addControlButton( parentName );
		addChildrenList();
		
		UITable table = NGUITools.FindInParents<UITable>(gameObject);
		if (table != null) {
			 table.repositionNow = true;
		}
		
	}
	
	public GameObject addChild(string strChild ){
		addChildrenList();
		
		m_uiCurrentID++;
		GameObject child = GameObject.Instantiate( m_ChildDemo ) as GameObject;
		child.name = m_uiCurrentID.ToString();
		m_childObjList.Add(m_uiCurrentID,child );
		
		child.SetActive(true);
		child.transform.parent = m_Children.transform;
		child.transform.localPosition = m_ChildDemo.transform.localPosition;
		child.transform.localScale = m_ChildDemo.transform.localScale;
		
		for(int i=0;i<child.transform.childCount;i++ )
		{
			UILabel label = child.transform.GetChild(i).GetComponent<UILabel>();
			if(null!=label)
			{
				label.text = strChild;
			}
		}
		
		//add to the root group all of this tree child node
		child.GetComponent<UICheckbox>().radioButtonRoot = m_treeManager.m_rootObj.transform;
		
		child.GetComponent<UICheckbox>().optionCanBeNone = m_OptionCanBeDone;
		
		m_Children.GetComponent<UITable>().repositionNow = true;
		m_Children.GetComponent<UITable>().Reposition();
		return child;
	}
	
	public void removeChildByObj(GameObject child )
	{
		bool bRemoveSuc = false;
		foreach(KeyValuePair<uint,GameObject> keyValue in m_childObjList )
		{
			if(keyValue.Value.Equals(child) )
			{
				GameObject.Destroy(child );
				m_childObjList.Remove( keyValue.Key );
				bRemoveSuc = true;
				break; //find it
			}
		}
		
		if(null!=m_Children && 
			bRemoveSuc)
		{
			m_Children.GetComponent<UITable>().repositionNow = true;
			m_Children.GetComponent<UITable>().Reposition();
		}
	}
	
	public void removeChildByID(uint ID)
	{
		m_childObjList.Remove(ID );
		
		if(null!=m_Children )
		{
			m_Children.GetComponent<UITable>().repositionNow = true;
			m_Children.GetComponent<UITable>().Reposition();
		}
	}
	
	public void addChildrenList()
	{
		if(null!=m_Children)
			return;
		
		m_Children = GameObject.Instantiate(m_ChildrenListDemo ) as GameObject;
		m_Children.SetActive(true);
		
		m_Children.transform.parent = this.transform;
		//m_Children.transform.localScale = m_ChildrenListDemo.transform.localScale;
		
		m_Children.transform.localPosition = m_ChildrenListDemo.transform.localPosition;
		
	}
	
	public void addControlButton(string strChild)
	{
		addChildrenList();
		
		if(null!=m_controlObj)
			return;
		
		m_controlObj = GameObject.Instantiate(m_ChildrenControlDemo) as GameObject;
		
		m_controlObj.SetActive(true);
		m_controlObj.transform.parent = this.transform;
		//m_controlObj.transform.localScale = m_ChildrenControlDemo.transform.localScale;
		m_controlObj.transform.localPosition = m_ChildrenControlDemo.transform.localPosition;
		
		m_controlObj.GetComponent<UIButtonTween>().tweenTarget = m_Children; // control button to scale the m_Children
		
		UILabel[] labels = m_controlObj.GetComponentsInChildren<UILabel>();
		if(0<labels.Length)
		{
			labels[0].text = strChild;
		}
		
		for(int i=0;i<m_controlObj.transform.childCount;i++ )
		{
			UILabel label = m_controlObj.transform.GetChild(i).GetComponent<UILabel>();
			if(null!=label)
			{
				label.text = strChild;
			}
		}
		
		m_controlCheckBox = m_controlObj.transform.Find("Control_CheckBox").gameObject;
		m_controlCheckBox.GetComponent<UICheckbox>().onStateChange = addControlButtonStateChange;
		
		UIEventListener.Get(m_controlCheckBox).onDoubleClick += onDBClickControlButton;
	} 
	
	private void onDBClickControlButton(GameObject go)
	{
		if ( null == m_controlCheckBox )
			return;
		m_controlObj.GetComponent<UICheckbox>().isChecked = !m_controlObj.GetComponent<UICheckbox>().isChecked;
	}
	
	public void SetSelected(bool check, bool force)
	{
		if ( m_needOpenChildren || force )
			m_controlObj.GetComponent<UICheckbox>().isChecked = check;
	}
	
	public void SetNeedOpenChildRen(bool need)
	{
		m_needOpenChildren = need;
		if ( !m_needOpenChildren )
		{
			m_controlCheckBox.transform.GetComponent<UICheckbox>().radioButtonRoot = m_treeManager.m_rootObj.transform;
		}
	}
	
	public void SetCheckBoxOptionCanBeDone(bool done)
	{
		m_controlCheckBox.transform.GetComponent<UICheckbox>().optionCanBeNone = done;
		m_OptionCanBeDone = done;
	}
	
	private void addControlButtonStateChange(bool state)
	{
		SetSelected(state, false);
	}
}