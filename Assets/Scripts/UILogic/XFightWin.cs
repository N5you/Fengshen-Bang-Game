using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XFightWin")]
public class XFightWin : XUIBaseLogic
{	
	public static int MAX_ITEM_ICON_NUM = 8;
	public UISprite WinSprite;
	public GameObject ButtonConfirm = null;
	public UILabel	ExpLabel;
	public UILabel 	RepLabel;
	public UILabel  MoneyLabel;
	
	public static int MAX_STARS_COUNT = 3;
	public int StarShowCount = 0;
	public UISprite[] Stars;
	private bool showEffect = false;
	
	public XActionIcon[]	UIAcionIcon	= new XActionIcon[MAX_ITEM_ICON_NUM]; 
	public XBaseActionIcon[] LogicIcon = new XBaseActionIcon[MAX_ITEM_ICON_NUM];

	public override bool Init()
	{
		base.Init();
		UIEventListener listen = UIEventListener.Get(ButtonConfirm);
		listen.onClick += OnClickConfirm;
		
		for(int i = 0; i < MAX_ITEM_ICON_NUM; i++)
		{
			LogicIcon[i] = new XBaseActionIcon();
			LogicIcon[i].SetUIIcon(UIAcionIcon[i]);
			LogicIcon[i].IsCanDrag		= false;
			LogicIcon[i].IsCanDrop		= false;
			LogicIcon[i].IsCanPopMenu	= false;
		}
		return true;
	}

	public void OnClickConfirm(GameObject go)
	{
		XBattleManager.SP.LeaveFightScenePVE(true);
		Hide();
	}
	
	public override void Show()
	{
		base.Show();
		
		TweenPosition posEffect = WinSprite.gameObject.GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
		
		TweenScale scaleEffect = WinSprite.gameObject.GetComponent<TweenScale>();
		if(scaleEffect != null)
		{
			scaleEffect.Reset();
			scaleEffect.enabled	= true;
		}
		
		showEffect = true;
	}
	
	void Update ()
	{
		if ( showEffect )
		{
			float startTime= 0.3f;
			for(int i = 0; i < StarShowCount; i++ )
			{
				CoroutineManager.StartCoroutine(showStars(i, startTime));
				startTime = startTime + 0.3f;
			}
			float timeDisapper = StarShowCount * 0.3f + 0.7f;
			Invoke("showFinish", timeDisapper);
			StarShowCount = 0;
			showEffect = false;
		}
	}
	
	void showFinish()
	{
		OnClickConfirm(null);
	}
	
	private IEnumerator showStars(int index, float time)
	{
      	yield return new WaitForSeconds(time);
		Stars[index].gameObject.SetActive(true);
	}
	
	public void ResetUIData()
	{
		for(int i = 0; i < MAX_ITEM_ICON_NUM; i++)
		{
			LogicIcon[i].ResetUIAndLogic();
		}
	}
}
