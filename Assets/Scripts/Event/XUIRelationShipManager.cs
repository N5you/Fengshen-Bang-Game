using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XUIRelationShipManager
{
	static readonly int MAX_OPEN_PANEL = 4;

	public class XUIRelastionShip
	{
		public EUIPanel Panel;
		public bool IsReady;
		public ushort SortValue;

		public XUIRelastionShip()
		{
			Panel = EUIPanel.eCount;
			IsReady = false;
		}

		public XUIRelastionShip(EUIPanel _panel, bool _isReady, ushort _sortValue)
		{
			Panel = _panel;
			IsReady = _isReady;
			SortValue = _sortValue;
		}
	}

	public EUIPanel curMainPanel;

	public SortedList<EUIPanel, XUIRelastionShip> mUIRelationList;

	bool isAlready = false;

	public bool IsHasValue
	{
		get
		{
			if (mUIRelationList.Count != 0)
				return true;
			return false;
		}
	}

	public XUIRelationShipManager()
	{
		mUIRelationList = new SortedList<EUIPanel, XUIRelastionShip>();
	}

	public bool IsHasOpenPanel(EUIPanel _panel)
	{
		if (EUIAnchor.eCenter != XUIManager.SP.GetUIAnchor(_panel))
			return false;

		XCfgUIRelationShip cfg = XCfgUIRelationShipMgr.SP.GetConfig((ushort)_panel);
		if (cfg == null)
			return false;

		//如果List不为空，侧清除
		if (mUIRelationList.Count != 0 && isAlready)
		{
			foreach (XUIRelastionShip relation in mUIRelationList.Values)
			{
				XUICtrlBase curPanelCtrl = XUIManager.SP.GetUIControl(relation.Panel);
				if (curPanelCtrl != null)
					curPanelCtrl.Hide();
				
			}
			mUIRelationList.Clear();
			isAlready = false;
		}

		string[] strArr = cfg.OpenPanelID.Split(',');

		if (strArr.Length > MAX_OPEN_PANEL)
		{
			Log.Write(LogLevel.ERROR, "OpenPanelID Length Wrong!!!, OpenPanel Need: < " + MAX_OPEN_PANEL.ToString() + ", Error Cfg Key Panel:" + _panel.ToString());
			return false;
		}

		if (strArr[0] == "0" || strArr[0] == "")
		{
			XUICtrlBase ctrl = XUIManager.SP.GetUIControl(_panel);
			ctrl.SetPos(new Vector3(0f, 0f, ctrl.GetPos().z));
			return false;
		}
		else
		{
			if (curMainPanel != _panel)
			{
				mUIRelationList.Clear();
				curMainPanel = _panel;
				isAlready = false;
			}

			//开启UI加入到关系管理中
			if (!mUIRelationList.ContainsKey(_panel))
			{
				XUICtrlBase mainCtrl = XUIManager.SP.GetUIControl(_panel);
				bool isReady = mainCtrl.IsResourceLoaded();
				ushort sortvalue = XUIManager.SP.GetUISortValue(_panel);
				mUIRelationList.Add(_panel, new XUIRelationShipManager.XUIRelastionShip(_panel, isReady, sortvalue));
			}

			//开启UI所需要打开的UI加入到关系管理中
			for (int cnt = 0; cnt != strArr.Length; ++cnt)
			{
				EUIPanel newPanel = (EUIPanel)int.Parse(strArr[cnt]);
				XUICtrlBase ctrl = XUIManager.SP.GetUIControl(newPanel);
				ushort sortvalue = XUIManager.SP.GetUISortValue(newPanel);
				if (!ctrl.IsResourceLoaded())
				{
					if (!mUIRelationList.ContainsKey(newPanel))
					{
						mUIRelationList.Add(newPanel, new XUIRelationShipManager.XUIRelastionShip(newPanel, false, sortvalue));
					}
				}
				else
				{
					if (!mUIRelationList.ContainsKey(newPanel))
					{
						mUIRelationList.Add(newPanel, new XUIRelationShipManager.XUIRelastionShip(newPanel, true, sortvalue));
					}
				}
				if(!ctrl.IsLogicShow)
				{
					XEventManager.SP.SendEvent(EEvent.UI_Show, newPanel);
				}
			}
			SortPanel();
			return true;
		}
	}

	public void SortPanel()
	{
		foreach (KeyValuePair<EUIPanel, XUIRelastionShip> relation in mUIRelationList)
		{
			if (relation.Value.IsReady)
			{
				isAlready = true;
			}
			else
			{
				isAlready = false;
				break;
			}
		}

		if (isAlready)
		{
			//根据UI面板计算总宽和最大高度UI
			if (this.mUIRelationList.Count == 0)
				return;
			int width = 0;
			int height = 0;
			foreach (XUIRelastionShip relationItem in mUIRelationList.Values)
			{
				XUICtrlBase ctrl = XUIManager.SP.GetUIControl(relationItem.Panel);
				Bounds bound = ctrl.GetUIBounds();
				width += (int)bound.extents.x * 2;

				int tempHeight = (int)bound.extents.y * 2;
				if (tempHeight > height)
					height = tempHeight;
			}

			int sortListLenth  = mUIRelationList.Count;
			Transform centerTF = LogicApp.SP.UIRoot.UIAnchors[(int)EUIAnchor.eCenter];

			XUICtrlBase ctrls  = XUIManager.SP.GetUIControl(mUIRelationList.Values[0].Panel);
			XUICtrlBase ctrls2 = null;
			XUICtrlBase ctrls3 = null;
			XUICtrlBase ctrls4 = null;

			Vector3 FirstCenterPos = new Vector3((int)(centerTF.position.x - width / 2 + ctrls.GetUIBounds().extents.x), 0, ctrls.GetPos().z);
			Vector3 SecondCenterPos = Vector3.zero;
			Vector3 ThirdCenterPos = Vector3.zero;
			Vector3 ForthCenterPos = Vector3.zero;

			Vector3 orignalPos2 = Vector3.zero;
			Vector3 orignalPos3 = Vector3.zero;
			Vector3 orignalPos4 = Vector3.zero;

			Vector3 orignalPos = new Vector3(0f, 0f, ctrls.GetPos().z);
			ctrls.MovePos(orignalPos, FirstCenterPos);

			if (sortListLenth >= 2)
			{ 
				ctrls2 = XUIManager.SP.GetUIControl(mUIRelationList.Values[1].Panel);
				if (ctrls2 == null)
					Log.Write(LogLevel.ERROR, "ctrls2 is null");
				orignalPos2     = new Vector3(0f, 0f, ctrls2.GetPos().z);
				SecondCenterPos = new Vector3((int)(FirstCenterPos.x + ctrls.GetUIBounds().extents.x + 2 + ctrls2.GetUIBounds().extents.x), 0, ctrls2.GetPos().z);
				ctrls2.MovePos(orignalPos2, SecondCenterPos);
			}

			if(sortListLenth >=3)
			{
				ctrls3 = XUIManager.SP.GetUIControl(mUIRelationList.Values[2].Panel);
				if (ctrls3 == null)
					Log.Write(LogLevel.ERROR, "ctrls3 is null");
				orignalPos3     = new Vector3(0f, 0f, ctrls3.GetPos().z);
				ThirdCenterPos = new Vector3((int)(SecondCenterPos.x + ctrls2.GetUIBounds().extents.x + 2 + ctrls3.GetUIBounds().extents.x), 0, ctrls3.GetPos().z);
				ctrls3.MovePos(orignalPos3, ThirdCenterPos);
			}

			if (sortListLenth >= 4)
			{
				ctrls4 = XUIManager.SP.GetUIControl(mUIRelationList.Values[3].Panel);
				if (ctrls4 == null)
					Log.Write(LogLevel.ERROR, "ctrls4 is null");
				orignalPos4    = new Vector3(0f, 0f, ctrls4.GetPos().z);
				ForthCenterPos = new Vector3((int)(ThirdCenterPos.x + ctrls3.GetUIBounds().extents.x + 2 + ctrls4.GetUIBounds().extents.x), 0, ctrls4.GetPos().z);
				ctrls4.MovePos(orignalPos4, ForthCenterPos);
			}
			mUIRelationList.Clear();
		}
	}

	//处理面板已生成
	public void PanelCreated(EUIPanel _panel)
	{
		if (mUIRelationList.ContainsKey(_panel))
		{
			mUIRelationList[_panel].IsReady = true;
			//尝试的进行排序
			SortPanel();
		}
	}

	/// <summary>
	/// 对非共享UI的面板进行隐藏
	/// </summary>
	/// <param name="_panel">开启面板UI</param>
	public void CheckPanel(EUIPanel _panel)
	{
		if (EUIAnchor.eCenter != XUIManager.SP.GetUIAnchor(_panel))
			return;

		ushort uid = (ushort)_panel;
		XCfgUIRelationShip UIRelationConfig = XCfgUIRelationShipMgr.SP.GetConfig(uid);
		if (UIRelationConfig == null)
			return;

		string[] sharePanelID = UIRelationConfig.SharePanelID.Split(',');

		if (int.Parse(sharePanelID[0]) == 0)
		{
			int count = (int)EUIPanel.eCount;
			for (int cnt = 1; cnt != (int)count; ++cnt)
			{
				EUIPanel curPanel = (EUIPanel)cnt;
				XUICtrlBase ctrl = XUIManager.SP.GetUIControl(curPanel);
				if (ctrl == null)
					continue;

				if (EUIAnchor.eCenter != XUIManager.SP.GetUIAnchor(curPanel) || curPanel == _panel)
					continue;

				XEventManager.SP.SendEvent(EEvent.UI_Hide, curPanel);
			}
		}
		else
		{
			for (int cnt = 1; cnt != (int)EUIPanel.eCount; ++cnt)
			{
				EUIPanel curPanel = (EUIPanel)cnt;
				XUICtrlBase ctrl = XUIManager.SP.GetUIControl(curPanel);
				if (ctrl == null)
					continue;

				if (EUIAnchor.eCenter != XUIManager.SP.GetUIAnchor(curPanel) || curPanel == _panel)
					continue;

				//配置共享ID不隐藏
				for (int xnt = 0; xnt != sharePanelID.Length; ++xnt)
				{
					if (curPanel == (EUIPanel)int.Parse(sharePanelID[xnt]) || curPanel == _panel)
						continue;
				}
				XEventManager.SP.SendEvent(EEvent.UI_Hide, curPanel);
			}
		}
	}

	/// <summary>
	/// 隐藏子面板
	/// </summary>
	/// <param name="_panel">需要执行隐藏子面板的父UI</param>
	public void HideSubPanel(EUIPanel _panel)
	{
		ushort uid = (ushort)_panel;
		XCfgUIRelationShip UIRelationConfig = XCfgUIRelationShipMgr.SP.GetConfig(uid);
		if (UIRelationConfig == null)
			return;

		XUICtrlBase mainPanelCtrl = XUIManager.SP.GetUIControl(_panel);
		if (mainPanelCtrl.UIMode != 1 << (int)UI_MODE.UI_MODE_DYNAMIC)
			return;

		if (EUIAnchor.eCenter != XUIManager.SP.GetUIAnchor(_panel))
			return;

		string[] strArr = UIRelationConfig.SubPanelID.Split(',');
		for (int cnt = 0; cnt != strArr.Length; ++cnt)
		{
			int panelID = int.Parse(strArr[cnt]);
			if (panelID == 0)
				return;
			XUICtrlBase subPanelCtrl = XUIManager.SP.GetUIControl((EUIPanel)panelID);
			if (subPanelCtrl == null)
			{ 
				Debug.LogWarning("UIManager, HideSubPanel GetUIControl is null");
			}
			subPanelCtrl.Hide();
		}
	}
}