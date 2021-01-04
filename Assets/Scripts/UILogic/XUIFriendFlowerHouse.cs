using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
[AddComponentMenu("UILogic/XUIFriendFlowerHouse")]
public class XUIFriendFlowerHouse : XDefaultFrame
{
	public static readonly uint PAGE_RECIVEINFO_MAX_NUM = 15;
	public UIImageButton IB_PreviousPage;
	public UIImageButton IB_NextPage;
	public UILabel PageNo;
	public UILabel NoDataTips;
	public UILabel[] Flowers;
	public UILabel[] GetText;
	public UILabel[] ReciveTimes;
	public UILabel[] SendPlayer;
	private uint cureentPage = 1;

	public override bool Init()
	{
		base.Init ();
		initUIData ();
		
		return true;
	}
	
	public override void Show()
	{
		base.Show ();
		cureentPage = 1;
		UpdateInfo ();
	}
	
	public void UpdateInfo()
	{
		int listCount = XFriendManager.SP.GetHouseRecordList ().Count;
		if(listCount <= 0)
			return;
		int totalPage = (int)Mathf.Ceil ((float)listCount / (float)PAGE_RECIVEINFO_MAX_NUM);
		PageNo.text = string.Format ("{0}/{1}", (cureentPage).ToString (), totalPage.ToString ());
		
		for (int cnt = (int)((cureentPage - 1) * PAGE_RECIVEINFO_MAX_NUM), index = 0; cnt!= (int)((cureentPage - 1) * PAGE_RECIVEINFO_MAX_NUM + PAGE_RECIVEINFO_MAX_NUM); ++cnt) {	
			try {
				this.Flowers [index].text = string.Format (XStringManager.SP.GetString (118), XFriendManager.SP.GetHouseRecordList () [cnt].Flowers.ToString ());
				this.GetText [index].text = string.Format (XStringManager.SP.GetString (123));
				this.SendPlayer [index].text = XFriendManager.SP.GetHouseRecordList () [cnt].PresentName.ToString ();
				
				ulong reciveTime = XFriendManager.SP.GetHouseRecordList () [cnt].ReciveTime;
				TimeSpan ts = DateTime.Now - new DateTime (1970, 1, 1, 8, 0, 0);
				uint timeSecond = (uint)(ts.TotalMilliseconds / 1000);
				uint timeoffset = timeSecond - (uint)reciveTime;
				if (timeoffset < 60) {
					this.ReciveTimes [index].text = string.Format (XStringManager.SP.GetString (119), timeoffset.ToString ());
				}
				else if (timeoffset >= 60 && timeoffset < 3600) {
						this.ReciveTimes [index].text = string.Format (XStringManager.SP.GetString (120), ((int)Mathf.Floor (timeoffset / 60)).ToString ());
					}
					else if (timeoffset >= 3600 && timeoffset < 3600 * 24) {
							this.ReciveTimes [index].text = string.Format (XStringManager.SP.GetString (121), ((int)Mathf.Floor (timeoffset / 3600)).ToString ());
						}
						else {
							this.ReciveTimes [index].text = string.Format (XStringManager.SP.GetString (122), ((int)Mathf.Floor (timeoffset / (3600 * 24))).ToString ());
						}
			}
			catch {
				this.Flowers [index].text = "";
				this.GetText [index].text = "";
				this.SendPlayer [index].text = "";
				this.ReciveTimes [index].text = "";
			}
			++index;
		}
	}
	
	private bool initUIData()
	{
		if (!NoDataTips.gameObject) {
			Log.Write (LogLevel.ERROR, "XUIFriendFlowerHouse, NoDataTips is not Set");
			return false;
		}

		if (!IB_NextPage.gameObject) {
			Log.Write (LogLevel.ERROR, "XUIFriendFlowerHouse, IB_NextPage is not Set");
			return false;
		}

		if (!IB_PreviousPage.gameObject) {
			Log.Write (LogLevel.ERROR, " XUIFriendFlowerHouse, IB_PreviousPage is not Set");
			return false;
		}

		if (!PageNo.gameObject) {
			Log.Write (LogLevel.ERROR, "XUIFriendFlowerHouse, PageNo is not Set");
			return false;
		}
		UIEventListener previousPageListener = UIEventListener.Get (IB_PreviousPage.gameObject);
		previousPageListener.onClick += OnPreviousPageBtn;

		UIEventListener nextPageListener = UIEventListener.Get (IB_NextPage.gameObject);
		nextPageListener.onClick += OnNextPageBtn;
		
		if (XFriendManager.SP.GetHouseRecordList ().Count <= 0) {
			PageNo.text = string.Format ("0/0");
		}
		else {
			NoDataTips.gameObject.SetActive (false);
			UpdateInfo ();
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		
		return true;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFriendFlowerHouse);
	}
	
	private void OnPreviousPageBtn(GameObject go)
	{
		if (cureentPage <= 1) {
			return;
		}
		--cureentPage;
		UpdateInfo ();
	}

	private void OnNextPageBtn(GameObject go)
	{
		int listCount = XFriendManager.SP.GetHouseRecordList ().Count;
		if (cureentPage >= (int)Mathf.Ceil ((float)listCount / (float)PAGE_RECIVEINFO_MAX_NUM)) {
			return;
		}
		++cureentPage;
		UpdateInfo ();
	}
}
