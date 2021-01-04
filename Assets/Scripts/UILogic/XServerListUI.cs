using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XServerListUI")]
public class XServerListUI : XUIBaseLogic
{
	[System.Serializable]
	public class ServerLabelUnit
	{
		public int ServerID = 0;
		public UILabel ServerLabel = null;
		public void Init()
		{
			NGUITools.AddWidgetCollider(ServerLabel.gameObject);
			ServerLabel.color = Color.gray;
			UIEventListener listen = UIEventListener.Get(ServerLabel.gameObject);
			listen.onHover += OnMouseOver;
			listen.onClick += OnClick;
		}
		public void OnClick(GameObject go)
		{
			XEventManager.SP.SendEvent(EEvent.ServerList_SelectServer, ServerID);
		}
		public void OnMouseOver(GameObject go, bool isOver)
		{
			if(isOver) ServerLabel.color = Color.red;
			else ServerLabel.color = Color.gray;
		}
	}
	
	public UIGrid  GridLabels = null;
	public ServerLabelUnit Sample = new ServerLabelUnit();
	
	public void OnAddServerInfo(ServerInfo server)
	{
		if(null == Sample)
		{
			Log.Write(LogLevel.ERROR, "XServerListUI, 未设置ServerLabelSample");
			return;
		}
		if(0 == Sample.ServerID)
		{
			Sample.ServerID = server.ID;
			Sample.ServerLabel.text = "" + server.ID + "   " + server.Name;
			Sample.Init();
		}
		else
		{
			ServerLabelUnit unit = new ServerLabelUnit();
			unit.ServerID = server.ID;
			GameObject go = Instantiate(Sample.ServerLabel.gameObject) as GameObject;
			go.transform.parent = GridLabels.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Sample.ServerLabel.transform.localScale;
			GridLabels.Reposition();
			unit.ServerLabel = go.GetComponent<UILabel>();
			unit.ServerLabel.text = "" + server.ID + "   " + server.Name;
			unit.Init();
		}
	}
}