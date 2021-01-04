using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class XUTNpcDialog : XUICtrlTemplate<XNpcDialog>
{

	private XNpc m_NpcObj = null;
	
	public XUTNpcDialog()
	{
		XEventManager.SP.AddHandler(checkNpcDialog, EEvent.NpcDialog_CheckSignal);
		
		XEventManager.SP.AddHandler(bindNpc, EEvent.NpcDialog_BindNpc);
	}
	
	public override void OnHide()
	{
		base.OnHide();
		
		m_NpcObj = null;
		
		
	}
	
	private void bindNpc(EEvent evt, params object[] args)
	{
	 	m_NpcObj = (args[0]) as XNpc;
	}
	
	private void checkNpcDialog(EEvent evt, params object[] args)
	{
		if( null != m_NpcObj )
		{
			float fDistance = Vector3.Distance( XLogicWorld.SP.MainPlayer.Position,m_NpcObj.Position );
			
			if( fDistance<=Define.GOOD_NPC_CLICK_DISTANCE && !IsShow() )
			{
				//XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eNpcDialog );
			}else if( fDistance>=Define.MAX_NPC_CLICK_DISTANCE && IsShow() )
			{
				XEventManager.SP.SendEvent(EEvent.NpcDialog_FarDistance,null );
			}
		}
		
	}
	
	
}
