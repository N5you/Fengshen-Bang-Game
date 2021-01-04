using UnityEngine;
using System.Collections;

public class XUTFaBao : XUICtrlTemplate<XFaBaoUI> 
{
	public XUTFaBao()
	{
		XEventManager.SP.AddHandler(On_SetMountBaoJiValue, EEvent.Mount_SetBaoJiValue);
		XEventManager.SP.AddHandler(On_SetMountNormalValue, EEvent.Mount_SetNormalValue);
		XEventManager.SP.AddHandler(On_SetMountExp, EEvent.Mount_SetExp);
				
		RegEventAgent_CheckCreated(EEvent.Mount_SetLevel, On_Update_Mount_Level);
		RegEventAgent_CheckCreated(EEvent.Mount_SetPeiYangCount, On_Update_Mount_PeiYangCount);
		RegEventAgent_CheckCreated(EEvent.Mount_ShowJiaChengInfo, On_ShowJiaChengInfo);
		RegEventAgent_CheckCreated(EEvent.Mount_ShowZuoQiInfo, On_ShowZuoQiInfo);
		RegEventAgent_CheckCreated(EEvent.Mount_ShowZuoQiSelect, On_ShowZuoQiSelect);
	}
	
	public override void OnShow()
	{
		if ( XMountManager.SP.m_nMountLevel == 0 )
			return;
		
		setMountLevel(XMountManager.SP.m_nMountLevel);
		setMountpeiYangCount(XMountManager.SP.MountPeiYangCount);
		XCfgMountAttr nextAttr = XCfgMountAttrMgr.SP.GetConfig((ushort)XMountManager.SP.m_nMountLevel);
		if ( nextAttr != null )
			setMountExp((uint)XMountManager.SP.m_nCurrExp, (uint)nextAttr.NeedExp);
		showJiaChengInfo((uint)XMountManager.SP.m_nMountLevel);
		showZuoQiInfo();
	}
	
	private void On_Update_Mount_Level(EEvent evt, params object[] args)
    {
        if ( null == LogicUI || args.Length < 1 || !LogicUI.ZuoQiPanel.activeSelf )
            return;
		
		int level = (int)args[0];
		
		setMountLevel(level);
    }
	
	private void setMountLevel(int level)
	{
		LogicUI.Label_Mount_Level.text = level.ToString();
		LogicUI.LevelPanel.SetActive(true);
		
		LogicUI.HuanHua.SetActive(true);
		if ( XMountManager.SP.m_nEquipMount >0 )
			LogicUI.JieCheng.SetActive(true);
		else
			LogicUI.QiCheng.SetActive(true);
		LogicUI.Obj_NormalMountPeiYang.SetActive(true);
		LogicUI.Obj_SuperMountPeiYang.SetActive(true);
		LogicUI.ZuoQiObj.SetActive(true);
		LogicUI.MountExpPanel.SetActive(true);
	}
	
	private void On_Update_Mount_PeiYangCount(EEvent evt, params object[] args)
    {
        if ( null == LogicUI || args.Length < 1 || !LogicUI.ZuoQiPanel.activeSelf )
            return;
		
        setMountpeiYangCount((int)args[0]);
    }
	
	private void setMountpeiYangCount(int count)
	{
		LogicUI.Label_Mount_PeiYangCount.text = string.Format(XStringManager.SP.GetString(1076u), count);
	}
	
	private void On_SetMountBaoJiValue(EEvent evt, params object[] args)
	{
		if ( null == LogicUI || args.Length < 1 || !LogicUI.ZuoQiPanel.activeSelf )
            return;
		
		int exp = (int)args[0];
		LogicUI.ShowExpAnima(1, exp);
	}
	
	private void On_SetMountNormalValue(EEvent evt, params object[] args)
	{
		if ( null == LogicUI || args.Length < 1 || !LogicUI.ZuoQiPanel.activeSelf )
            return;
		
		int exp = (int)args[0];
		LogicUI.ShowExpAnima(0, exp);
	}
	
	private void On_SetMountExp(EEvent evt, params object[] args)
	{
		if ( null == LogicUI || args.Length < 2 || !LogicUI.ZuoQiPanel.activeSelf )
            return;
		
		setMountExp((uint)args[0], (uint)args[1]);
	}
	
	private void setMountExp(uint exp, uint needExp)
	{
		LogicUI.MountExpPanel.SetActive(true);
		LogicUI.MountExpSlider.sliderValue = (float)((float)exp/(float)needExp);
		LogicUI.MountExpLabel.text = exp.ToString() + "/" + needExp.ToString();
	}
	
	private void On_ShowJiaChengInfo(EEvent evt, params object[] args)
	{
		if ( null == LogicUI || args.Length < 1 || !LogicUI.ZuoQiPanel.activeSelf )
            return;
		
		showJiaChengInfo((uint)args[0]);
	}
	
	private void showJiaChengInfo(uint level)
	{
		XCfgMountAttr currAttr = XCfgMountAttrMgr.SP.GetConfig((ushort)level);
		XCfgMountAttr nextAttr = XCfgMountAttrMgr.SP.GetConfig((ushort)(level + 1));
		
		if ( null != currAttr )
		{
			LogicUI.Label_WuLi[0].text = currAttr.WuLi.ToString();
			LogicUI.Label_LingQiao[0].text = currAttr.LingQiao.ToString();
			LogicUI.Label_TiZhi[0].text = currAttr.TiZhi.ToString();
			LogicUI.Label_ShuFa[0].text = currAttr.ShuFa.ToString();
			int speed = (int)(currAttr.Speed * 100);
			LogicUI.Label_SuDu[0].text = speed.ToString() + "%";
			
			LogicUI.Obj_NormalMountPeiYang.GetComponent<UIImageButton>().CommonTips =
				string.Format(XStringManager.SP.GetString(1084), currAttr.CostMoney);
			LogicUI.Obj_SuperMountPeiYang.GetComponent<UIImageButton>().CommonTips = 
				string.Format(XStringManager.SP.GetString(1085), currAttr.CostIngot);
		}
		else
		{
			LogicUI.Label_WuLi[0].text = "0";
			LogicUI.Label_LingQiao[0].text = "0";
			LogicUI.Label_TiZhi[0].text = "0";
			LogicUI.Label_ShuFa[0].text = "0";
			LogicUI.Label_TongLing[0].text = "0";
			LogicUI.Label_SuDu[0].text = "0";
		}
		
		if ( null != nextAttr )
		{
			LogicUI.Label_WuLi[1].text = nextAttr.WuLi.ToString();
			LogicUI.Label_LingQiao[1].text = nextAttr.LingQiao.ToString();
			LogicUI.Label_TiZhi[1].text = nextAttr.TiZhi.ToString();
			LogicUI.Label_ShuFa[1].text = nextAttr.ShuFa.ToString();
			LogicUI.Label_TongLing[1].text = nextAttr.TongLing.ToString();
			int speed = (int)(nextAttr.Speed * 100);
			LogicUI.Label_SuDu[1].text = speed.ToString() + "%";
		}
		else
		{
			LogicUI.Label_WuLi[1].text = "0";
			LogicUI.Label_LingQiao[1].text = "0";
			LogicUI.Label_TiZhi[1].text = "0";
			LogicUI.Label_ShuFa[1].text = "0";
			LogicUI.Label_TongLing[1].text = "0";
			LogicUI.Label_SuDu[1].text = "0";
		}
		
		LogicUI.JiaChengPanbel.SetActive(true);
	}
	
	private void On_ShowZuoQiSelect(EEvent evt, params object[] args)
	{
		if ( null == LogicUI )
            return;
		
		LogicUI.ZuoQiGroup.SetSelect( XMountManager.SP.m_nEquipMount - (XMountManager.SP.m_nCurrentPage * 6) - 1, true);
		XFaBaoUI.ZuoQiSelectItem.CurrentSelPos = XMountManager.SP.m_nEquipMount;
		//LogicUI.ShowMountModel(XFaBaoUI.ZuoQiSelectItem.CurrentSelPos);
	}
	
	private void On_ShowZuoQiInfo(EEvent evt, params object[] args)
	{
		if ( null == LogicUI || !LogicUI.ZuoQiPanel.activeSelf )
            return;
		
		showZuoQiInfo();
	}
	
	private void showZuoQiInfo()
	{
		for(int i = 0; i < 6; i++)
		{
			LogicUI.zqsprite[i].gameObject.SetActive(false);
			LogicUI.bk[i].gameObject.SetActive(false);
		}
		
		int currZuoQiPos = XMountManager.SP.m_nEquipMount;
		int currCount = XMountManager.SP.m_Mounts.Count;
		int startPos = XMountManager.SP.m_nCurrentPage * 6;
		for( int i = 0; i < 6; i++ )
		{
			int pos = i + startPos + 1;
			XMount mount = XMountManager.SP.GetMount(pos);
			if ( null == mount )
				break;
			XCfgMount mountinfo = XCfgMountMgr.SP.GetConfig(mount.MountIndex);
			LogicUI.bk[i].CommonTips = mountinfo.MountTips; 
			LogicUI.zqsprite[i].gameObject.SetActive(true);
			LogicUI.bk[i].gameObject.SetActive(true);
			LogicUI.ZuoQiItems[i].SetPos(pos);
			XUIDynamicAtlas.SP.SetSprite(LogicUI.zqsprite[i], mountinfo.MountAtlas, mountinfo.MountIcon, true, null);
		}
	}
}
