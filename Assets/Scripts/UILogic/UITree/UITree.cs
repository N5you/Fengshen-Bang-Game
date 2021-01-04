using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UITree : MonoBehaviour {
	
	public GameObject m_rootObj = null;
	
	public GameObject m_parentDemo = null;
	
	public enum ControlButtonType
	{
		ControlButton_OpenChild,
		ControlButton_NotOpenChild,
	}
	
	public ControlButtonType ButtonClickType = ControlButtonType.ControlButton_OpenChild;
	
	public bool ChildOptionCanBeNone = false;
	
	public bool OnVerticalBarResize = false;
	int index = 0;
	private SortedList<int, GameObject> m_allObj2Resize = new SortedList<int, GameObject>();
	private SortedList<int, GameObject> m_allObj2ResizeCollinder = new SortedList<int, GameObject>();
	private SortedList<int, Vector3> m_allResizeScale = new SortedList<int, Vector3>();
	private UIScrollBar m_verticalBar;
	
	public bool testAddChild = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(true==testAddChild)
		{
			GameObject parObj = insertNode("test parent");
			
			insertItem( "test child",parObj );
			
			testAddChild = false;
		}
		
	}
	
	public void repositionNow()
	{
		UITable parentTable = m_rootObj.GetComponent<UITable>();
		if(null==parentTable)
		{
			return;
		}
		parentTable.repositionNow = true;
	}
	
	public GameObject insertItem( string itemName,GameObject parentItem )
	{
		UITreeParentNode parentNode = parentItem.GetComponent<UITreeParentNode>();
		if( null==parentNode )
		{
			return null;
		}
		GameObject obj = parentNode.addChild( itemName );
		UITable parentTable = m_rootObj.GetComponent<UITable>();
		parentTable.Reposition();
		
		m_allObj2ResizeCollinder[index] = obj;
		GameObject objsize = obj.transform.Find("CenterPoint").gameObject;
		m_allObj2Resize[index] = objsize;
		m_allResizeScale[index] = objsize.transform.localScale;
		index++;
		
		return obj;
	}
	
	public GameObject insertNode(string parentName) // insert under the root node
	{
		return insertNode(parentName,m_rootObj);
	}
	
	public GameObject insertNode( string parentName,GameObject parentItem )
	{
		UITable parentTable = parentItem.GetComponent<UITable>();
		if(null==parentTable)
		{
			return null;
		}
		
		GameObject nodeObj = GameObject.Instantiate(m_parentDemo) as GameObject;
		nodeObj.SetActive(true);
		nodeObj.transform.parent = parentItem.transform;
		nodeObj.transform.localPosition = Vector3.zero; //new Vector3(0,0,-10);
		nodeObj.GetComponent<UITreeParentNode>().initParentNode(this.GetComponent<UITree>(),parentName);
		
		if ( ButtonClickType == ControlButtonType.ControlButton_NotOpenChild )
			nodeObj.GetComponent<UITreeParentNode>().SetNeedOpenChildRen(false);
		
		nodeObj.GetComponent<UITreeParentNode>().SetCheckBoxOptionCanBeDone(ChildOptionCanBeNone);
		parentTable.repositionNow = true;
		
		
		//  保存需要扩展的控件
		GameObject obj = nodeObj.GetComponent<UITreeParentNode>().m_controlObj.transform.Find("Sprite (11000015)").gameObject;
		m_allObj2Resize[index] = obj;
		m_allResizeScale[index] = obj.transform.localScale;
		index++;
		GameObject checkObj = nodeObj.GetComponent<UITreeParentNode>().m_controlObj.transform.Find("Control_CheckBox").gameObject;
		m_allObj2ResizeCollinder[index] = checkObj;
		GameObject obj2 = checkObj.transform.Find("Sprite (11000315)").gameObject;
		m_allObj2Resize[index] = obj2;
		m_allResizeScale[index] = obj2.transform.localScale;
		obj2.name = obj2.name + index.ToString();
		index++;
		
		UICheckbox cb = nodeObj.GetComponent<UITreeParentNode>().m_controlObj.GetComponent<UICheckbox>();
		if ( cb != null )
		{
			cb.afterActive += ResizeCollinder;
		}
				
		return nodeObj;
	}
	
	public void clearTree()
	{
		for( int i=0;i<m_rootObj.transform.childCount;i++ )
		{
			GameObject.Destroy( m_rootObj.transform.GetChild(i).gameObject );
		}
	}
	
	public void SetScrollBar(UIScrollBar bar)
	{
		m_verticalBar = bar;
		
		m_verticalBar.onScrollBarShowState += ResizeForms;
	}
	
	private void ResizeForms(bool resize)
	{
		if ( !OnVerticalBarResize )
			return;
		for( int i = 0; i < index; i++ )
		{
		    Vector3 v = m_allResizeScale[i];
			Vector3 tmp = new Vector3(v.x, v.y, v.z);
			if ( resize )
				tmp.x -= 16;
			
			// 更新大小
			GameObject tmpObj = m_allObj2Resize[i];
			tmpObj.transform.localScale = tmp;	
		}
	}
	
	// 重新计算碰撞体
	private void ResizeCollinder(bool resize)
	{
		for( int i = 0; i < index; i++ )
		{
			GameObject tmpObjCollinder;;
			if ( m_allObj2ResizeCollinder.TryGetValue(i, out tmpObjCollinder))
			{
				NGUITools.AddWidgetCollider(tmpObjCollinder);
			}
		}
	}
	
}
	