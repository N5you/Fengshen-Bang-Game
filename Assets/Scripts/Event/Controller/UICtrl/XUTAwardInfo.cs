using UnityEngine;
using XGame.Client.Packets;

class XUTAwardInfo : XUICtrlTemplate<XAwardInfo>
{
	private	uint AwardIndex;
	
	public XUTAwardInfo()
	{
		XEventManager.SP.AddHandler(AwardDataHandler,EEvent.UI_ShowAwardInfo);
	}
	
	public void AwardDataHandler(EEvent evt, params object[] args)
	{
		AwardIndex = (uint)args[0];
	}
	
	public override void OnShow()
	{
		base.OnShow();
		
		if(!FeatureDataUnLockMgr.SP.mAwardList.ContainsKey(AwardIndex))
			return ;
		FeatureDataUnLockMgr.AwardData data = FeatureDataUnLockMgr.SP.mAwardList[AwardIndex];
		
		XCfgAddAward cfg = XCfgAddAwardMgr.SP.GetConfig((uint)data.Type);
		if(cfg == null)
			return ;
		
		string content = "";
		switch(data.Type)
		{
		case EAwardType.EAwardType_XianDH:
			{
				content	= string.Format(XStringManager.SP.GetString(247),data.Rank);
				content += "\n";
				content	+= string.Format(XStringManager.SP.GetString(248),data.GameMoney,data.Honour);
				content += "\n";				
				//content += string.Format(XStringManager.SP.GetString(249),"chenghao");
				//content += "\n";
			
				XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(data.ItemID);
				if(cfgItem != null)
				{
					string itemName = "";
					itemName	+= XGameColorDefine.Quality_Color[cfgItem.QualityLevel] + cfgItem.Name;
					content += string.Format(XStringManager.SP.GetString(250),itemName);
				}
			}
			break;
		case EAwardType.EAwardType_ZhanYaoLu:
			{
				content = XStringManager.SP.GetString(530);
				content += "\n";
				content += "\n";
				content += "\n";
				content	+= string.Format(XStringManager.SP.GetString(531),data.Honour);
				content += "\n";
			}
			break;
		}
		
		LogicUI.label.text	= content;
		
		FeatureDataUnLockMgr.SP.DelAward(AwardIndex);
		XEventManager.SP.SendEvent(EEvent.UI_FreshAward);
	}
	
}
