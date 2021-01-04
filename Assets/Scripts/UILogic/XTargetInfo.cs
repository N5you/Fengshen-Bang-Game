using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XTargetInfo")]
public class XTargetInfo : XUIBaseLogic
{
	public UILabel TargeterName;
	public UILabel TargeterLevel;
	public UILabel CombatPower;
	
	public UIImageButton AddFriend;
	public UIImageButton LookInfo;
	public UIImageButton ChatPrivate;
	public UIImageButton RequestGroup;
	
	public UISprite VIP;
	
	public UITexture RoleHeadTex;
	
	public override bool Init ()
	{
		base.Init ();
		UIEventListener listenerAddFriend = UIEventListener.Get(AddFriend.gameObject);
		listenerAddFriend.onClick += this.OnClickAddFriend;
		
		UIEventListener listenerLookInfo = UIEventListener.Get(LookInfo.gameObject);
		listenerLookInfo.onClick += this.OnClickLookInfo;
		
		UIEventListener listenerChatPrivate = UIEventListener.Get(ChatPrivate.gameObject);
		listenerChatPrivate.onClick += this.OnClickChatPrivate;
		
		UIEventListener listenerRequestGroup = UIEventListener.Get(RequestGroup.gameObject);
		listenerRequestGroup.onClick += this.OnClickRequestGroup;
		return true;
	}
	
	
	
	public void SetName(string name)
	{
		TargeterName.text = name;
	}
	
	public void SetLevel(string level)
	{
		TargeterLevel.text = level;
	}
	
	public void SetCombatPower(string combatPower)
	{
		CombatPower.text = combatPower;
	}
	
	#region clickbuttonevent
	private void OnClickAddFriend(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.TargetInfo_AddFriend);
	}
	
	private void OnClickChatPrivate(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.TargetInfo_ChatPrivate);
	}
	
	private void OnClickRequestGroup(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.MessageBox,null,null,XStringManager.SP.GetString(139));
	}
	
	private void OnClickLookInfo(GameObject go)
	{
		
		XEventManager.SP.SendEvent(EEvent.TargetInfo_LookInfo);
	}
	#endregion
}
