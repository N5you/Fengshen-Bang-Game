using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XGame.Client.Packets;
[AddComponentMenu("UILogic/XUIFriendCharmRank")]
public class XUIFriendCharmRank : XDefaultFrame
{
	public static readonly int MAX_TOP_RANK_NUM = 15;
	public UIImageButton BottomCloseBTN;
	public UILabel NoDataTips;
	public UILabel[] RankNum;
	public UILabel[] PlayerName;
	public UILabel[] Flowers;

	public override bool Init()
	{
		base.Init ();
		initUIData ();
		return true;
	}

	private void initUIData()
	{
		UIEventListener closeListener = UIEventListener.Get (BottomCloseBTN.gameObject);
		closeListener.onClick += hideUI;
		uint dataCount = XCharmRankManager.SP.GetDataCount ();
		if (dataCount > 0) {
			NoDataTips.gameObject.SetActive (false);
		}
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFriendCharmRank);
	}
	
	public void UpdateInfo()
	{
		
		for (int cnt = 0; cnt != XCharmRankManager.SP.GetDataCount(); ++cnt) {
			if(cnt > MAX_TOP_RANK_NUM)
				break;
			try{
				this.RankNum [cnt].text = XCharmRankManager.SP.GetData (cnt).Rank.ToString();
				this.PlayerName [cnt].text = XCharmRankManager.SP.GetData (cnt).PlayerName;
				this.Flowers [cnt].text = XCharmRankManager.SP.GetData (cnt).Flowers.ToString ();
			}
			catch {
				this.RankNum [cnt].text = "";
				this.PlayerName [cnt].text = "";
				this.Flowers [cnt].text = "";
			}
		}
	}
	
	public override void Show()
	{
		base.Show ();
		uint dataCount = XCharmRankManager.SP.GetDataCount ();
		if (dataCount == 0) {
			return;
		}
		UpdateInfo ();
	}

	private void hideUI(GameObject go)
	{
		Hide ();
	}
}