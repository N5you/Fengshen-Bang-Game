class XUISmallMap : XUICtrlTemplate<XSmallMap>
{
	public XUISmallMap()
	{
		RegEventAgent_CheckCreated(EEvent.Mail_Tip, MailTip);
	}
	
	public override void OnCreated(object arg)
    {
        base.OnCreated(arg);
    }
	
	public void MailTip(EEvent evt, params object[] args)
	{
		if ( args.Length <= 0 )
			return;
		
		LogicUI.UpdateMailCount((int)args[0]);
	}

}
