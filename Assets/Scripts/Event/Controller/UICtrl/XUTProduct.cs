using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

class XUTProduct : XUICtrlTemplate<XProductUI>
{
	public XUTProduct()
	{
		RegEventAgent_CheckCreated(EEvent.product_LearnCareer, OnLearnCareer);
		RegEventAgent_CheckCreated(EEvent.product_UpgradeCareer, OnUpgradeCareer);
		RegEventAgent_CheckCreated(EEvent.product_ForgetCareer, OnForgetCareer);
		RegEventAgent_CheckCreated(EEvent.product_Strength, OnSetStrength);
		RegEventAgent_CheckCreated(EEvent.product_Exp, OnSetExp);
		RegEventAgent_CheckCreated(EEvent.product_AddGatherRec, OnAddGatherRec);
		RegEventAgent_CheckCreated(EEvent.product_AddFormula, OnAddFormula);
		RegEventAgent_CheckCreated(EEvent.Attr_Dynamic, OnLevelUp);
		XEventManager.SP.AddHandler(OnItemUpdate, EEvent.Bag_ItemNumChanged);
	}
	
	public void OnLearnCareer(EEvent evt, params object[] args)
	{
		LogicUI.OnLearnCareer((byte)(args[0]), (XCareerInfo)(args[1]));
	}
	
	public void OnUpgradeCareer(EEvent evt, params object[] args)
	{
		LogicUI.OnUpgradeCareer((byte)(args[0]), (XCareerInfo)(args[1]));
	}
	
	public void OnForgetCareer(EEvent evt, params object[] args)
	{
		LogicUI.OnForgetCareer((byte)(args[0]));
	}
	
	public void OnSetStrength(EEvent evt, params object[] args)
	{
		LogicUI.OnSetStrength((uint)(args[0]), (uint)((int)(args[1])));
	}
	
	public void OnSetExp(EEvent evt, params object[] args)
	{
		LogicUI.OnSetExp((byte)(args[0]), (XCareerInfo)(args[1]));
	}
	
	public void OnAddGatherRec(EEvent evt, params object[] args)
	{
		LogicUI.OnAddGatherRec((uint)(args[0]));
	}
	
	public void OnAddFormula(EEvent evt, params object[] args)
	{
		LogicUI.OnAddFormula((uint)(args[0]));
	}
	
	public void OnItemUpdate(EEvent evt, params object[] args)
	{
		if(null == LogicUI) return;
		uint itemId = (uint)(args[0]);
		LogicUI.OnItemUpdate(itemId);
	}
	
	public void OnLevelUp(EEvent evt, params object[] args)
	{
		EShareAttr attr = (EShareAttr)(args[1]);
		if(attr != EShareAttr.esa_Level) return;
		XCharacter ch = (XCharacter)(args[0]);
		if(ch != XLogicWorld.SP.MainPlayer) return;
		LogicUI.OnLevelUp();
	}
}

