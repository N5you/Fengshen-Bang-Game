using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XScenePassed")]
public class XScenePassed : XDefaultFrame
{	
	public UISprite	PassSpriteBK1;
	public UISprite PassSpriteBK2;
	
	// 评价
	public UISprite PingJia;
	private bool pingJiaMoveFinished = false;
	
	// 通关星数
	public static int MaxStarNum = 3;
	public UISprite[] StarList = new UISprite[MaxStarNum];
	public UISprite[] StarBKList = new UISprite[MaxStarNum];
	private int StarsCount = 0;
	
	//  经验
	public GameObject JingYanObj;
	private bool jingYanMoveFinished = false;
	private bool jingyangShowFinished = false;
	public GameObject ExtValueObj;
	public UILabel JingYanValueObj;
	public UILabel JingYanExtValueObj;
	private int totalExp = 0;
	private int extEp = 0;
	
	// 抽奖
	public UISprite ChouJiang;
	private bool chouJiangMoveFinish = false;
	private bool itemMoveFinish = false;
	public UILabel MoneyCost;
	public static int money;
	
	public static int LevelResultNum = 3;
	
	private SelItem[] mSelItemArray = new SelItem[LevelResultNum];
	public GameObject[] AwardItemArray = new GameObject[LevelResultNum];
	
	public delegate void ClickCallBack(int index);
	private bool IsSelect;
	public int SelIndex {get;set;}
	
	class SelItem
	{
		private UISprite mSprite;
		private UISprite mSpriteBK;
		private string	 OrignalName;
		private int mIndex;
		private GameObject mGo;
		private ClickCallBack mCallBack;
		XBaseActionIcon mLogicAI	= new XBaseActionIcon();
		private GameObject mAwardItemGo;
		private XScenePassed mParent;
		private bool IsCanClick = true;	
		
		private XU3dEffect effect;
		
		public SelItem(XScenePassed parent, GameObject head,int index,ClickCallBack callBack)
		{
			mGo					= head;
			mIndex				= index;
			mCallBack			= callBack;	
			mParent 			= parent;
			
			Transform spriteBKTF = mGo.transform.Find("SpriteBK");
			if(spriteBKTF != null)
			{
				mSpriteBK	= spriteBKTF.GetComponent<UISprite>();
				OrignalName	= mSpriteBK.spriteName;
			}
			
			Transform AITF 	= mGo.transform.Find("AwardItem");			
			if(AITF != null)
			{
				mAwardItemGo	= AITF.gameObject;
				Transform tempAI = AITF.transform.Find("ActionIcon");
				if(tempAI != null)
				{
					mLogicAI.SetUIIcon(tempAI.GetComponent<XActionIcon>());	
					mLogicAI.IsCanDrag	= false;
					mLogicAI.IsCanDrop	= false;
					mLogicAI.IsCanPopMenu	= false;
				}
				
				NGUITools.SetActiveChildren(mAwardItemGo,false);
			}
			
			UIEventListener SL	= UIEventListener.Get(mSpriteBK.gameObject);
			SL.onClick += ClickSprite;
		}
		
		public void ClickSprite(GameObject go)
		{
			if(!IsCanClick)
				return ;
			
			if ( !checkBagCanPutItem() )
				return;
			
			bool canGet = true;
			switch( mIndex )
			{
			case 1:
				canGet = checkEnoughMoney();
				
				break;
			case 2:
				canGet = checkEnoughRealmoney();
				
				break;
			default:
				break;
			}
			
			if ( !canGet )
				return;
			
			string methName = "OnRotFinishFirst";
			switch ( mIndex )
			{
			case 1:
				methName = "OnRotFinishSecond";
				
				break;
			case 2:
				methName = "OnRotFinishThird";
				
				break;
			default:
				break;
			}
			mParent.Invoke(methName, 0.2f);
			
			TweenRotation rotAnimBK = mSpriteBK.gameObject.GetComponent<TweenRotation>();
			if(rotAnimBK == null)
				return ;
			
			rotAnimBK.Reset();
			rotAnimBK.enabled	= true;	

			if(mCallBack != null)
				mCallBack(mIndex);
		}
		
		public void Hide()
		{			
			mSpriteBK.spriteName	= "11009006";
		}
		
		public void Disable()
		{
			IsCanClick	= false;
		}
		
		public void ShowAwardItem()
		{
			NGUITools.SetActiveChildren(mAwardItemGo,true);
		}
		
		public void DisabelRotation()
		{
			TweenRotation rotAnimBK = mSpriteBK.gameObject.GetComponent<TweenRotation>();
			if(rotAnimBK == null)
				return ;
			
			rotAnimBK.Reset();
			rotAnimBK.enabled = false;	
		}
		
		public void SetItemData(int itemIndex)
		{
			if(mLogicAI == null)
				return ;
			
			StartEffect(900059);
			
			ShowAwardItem();
			
			mLogicAI.ResetUIAndLogic();
			XItem tempItem = XLogicWorld.SP.MainPlayer.ItemManager.GetItem((uint)itemIndex);
			if(tempItem == null || tempItem.IsEmpty())
				return ;
			XCfgItem cfgItem = XCfgItemMgr.SP.GetConfig(tempItem.DataID);
			if(cfgItem == null)
				return ;
			
			EItemBoxType outType;
			ushort outIndex;
			XItemManager.GetContainerType((ushort)itemIndex,out outType,out outIndex);
			
			mLogicAI.SetLogicData((ActionIcon_Type)outType,outIndex);
			mLogicAI.SetSprite(cfgItem.IconAtlasID,cfgItem.IconID,tempItem.Color,tempItem.ItemCount);
		}
		
		private void StartEffect(uint effectid)
		{
			effect = new XU3dEffect(effectid, EffectLoadedHandle);
		}
		
		private void EffectLoadedHandle(XU3dEffect effect)
    	{
			GameObject obj = mLogicAI.mUIIcon.gameObject;
	        effect.Layer = GlobalU3dDefine.Layer_UI_2D;
	        effect.Parent = obj.transform;
	        effect.LocalPosition = new Vector3(0, 0, -50);
	        effect.Scale = new Vector3(1, 1, 1);
    	}
		
		public void DestroyEffect()
		{
			if ( null != effect )
				effect.Destroy();
		}
		
		public void ReSet()
		{
			IsCanClick	= true;			
			mSpriteBK.transform.rotation	= Quaternion.Euler(Vector3.zero);
			mSpriteBK.spriteName	= OrignalName;
			NGUITools.SetActiveChildren(mAwardItemGo,false);
		}
		
		private bool checkBagCanPutItem()
		{
			if ( XLogicWorld.SP.MainPlayer.ItemManager.GetEmptyPosNum(EItemBoxType.Bag) < 1u )
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 23);
				return false;
			}
			return true;
		}
	
		private bool checkEnoughMoney()
		{
			if ( XLogicWorld.SP.MainPlayer.GameMoney < XScenePassed.money )
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 24);
				return false;
			}
			return true;
		}
	
		private bool checkEnoughRealmoney()
		{
			if ( XLogicWorld.SP.MainPlayer.RealMoney < 10 )
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator, 25);
				XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eVip);
				return false;
			}
			return true;
		}
	
	}
	
	public override bool Init()
	{
		for(int i = 0; i < LevelResultNum; i++)
		{
			mSelItemArray[i] = new SelItem(this, AwardItemArray[i],i,SetSpriteDisable);
		}
		
		TweenPosition posEffect = PingJia.gameObject.GetComponent<TweenPosition>();
		if ( posEffect )
		{
			posEffect.onFinished += PingJiaMoveFinished;
		}
		
		posEffect = JingYanObj.GetComponent<TweenPosition>();
		if ( posEffect )
		{
			posEffect.onFinished += JingyanMoveFinished;
		}
		
		posEffect = ChouJiang.gameObject.GetComponent<TweenPosition>();
		if ( posEffect )
		{
			posEffect.onFinished += ChouJiangMoveFinished;
		}
		
//		posEffect = ExtValueObj.GetComponent<TweenPosition>();
//		if ( posEffect )
//		{
//			posEffect.onFinished += ExtExpMoveFinished;
//		}
				
		return base.Init();
	}

	public override void Show()
	{
		base.Show();
		
		TweenRotation RotEffect = PassSpriteBK1.gameObject.GetComponent<TweenRotation>();
		if(RotEffect != null)
		{
			RotEffect.Reset();
			RotEffect.enabled	= true;
		}
		
		TweenRotation RotEffect2 = PassSpriteBK2.gameObject.GetComponent<TweenRotation>();
		if(RotEffect2 != null)
		{
			RotEffect2.Reset();
			RotEffect2.enabled	= true;
		}
		
		for(int i = 0; i < LevelResultNum; i++)
		{
			mSelItemArray[i].ReSet();
		}
		
		pingJiaMoveFinished = false;
		TweenPosition posEffect = PingJia.gameObject.GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
		
		jingyangShowFinished = false;
		StartCoroutine(showExpLabel(0.1f));
		
		JingYanObj.SetActive(false);
		jingYanMoveFinished = false;
		ExtValueObj.gameObject.SetActive(false);
		
		ChouJiang.gameObject.SetActive(false);
		StartCoroutine(showChouJiangLabel(0.2f));
		
		chouJiangMoveFinish = false;
		
		itemMoveFinish = false;
		for ( int i = 0; i < AwardItemArray.Length; i++ )
		{
			AwardItemArray[i].SetActive(false);
		}
		showChouJiangItem(0.3f);
	}
	
	public override void Hide()
	{
		base.Hide();
	}
	
	private void PingJiaMoveFinished(UITweener tween)
	{
		pingJiaMoveFinished = true;
	}
	
	private void JingyanMoveFinished(UITweener tween)
	{
		jingYanMoveFinished = true;
		Invoke("SetRealExpValue", 0.2f);
	}
	
	private void ChouJiangMoveFinished(UITweener tween)
	{
		chouJiangMoveFinish = true;
	}
	
	private void ExtExpMoveFinished(UITweener tween)
	{
		ChouJiang.gameObject.SetActive(true);
		TweenPosition posEffect = ChouJiang.gameObject.GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
	}

	void Update()
	{
		if ( pingJiaMoveFinished )
		{
			pingJiaMoveFinished = false;
			float startTime= 0.1f;
			if ( StarsCount > 0 )
			{
				for( int i = 0; i < StarsCount; i++ )
				{
					CoroutineManager.StartCoroutine(showStars(i, startTime));
					startTime = startTime + 0.1f;
				}
			}
//			else
//			{
//				showExpLabel();
//			}
		}
		
		if ( jingYanMoveFinished && !jingyangShowFinished )
		{
			showRandExp();
		}
		
//		if ( chouJiangMoveFinish && !itemMoveFinish )
//		{
//			float startTime= 0.2f;
//			for ( int i = 0; i < AwardItemArray.Length; i++ )
//			{
//				CoroutineManager.StartCoroutine(showAwardItem(i, startTime));
//				startTime = startTime + 0.2f;
//			}
//			itemMoveFinish = true;
//		}
	}

	public void SetBattleResult(int startCount, int exp, int extExp, int needMoney)
	{
		if ( startCount <= MaxStarNum )
		{
			StarsCount = startCount;
		}
		for (int i = 0; i < MaxStarNum; i++)
		{
			StarList[i].gameObject.SetActive(false);
			StarBKList[i].gameObject.SetActive(false);
		}
		
		totalExp = exp;
		extEp = extEp;
		money = needMoney;
	}
	
	private IEnumerator showStars(int index, float time)
	{
      	yield return new WaitForSeconds(time);
		StarList[index].gameObject.SetActive(true);
		StarBKList[index].gameObject.SetActive(true);
//		if ( index == StarsCount - 1 )
//		{
//			showExpLabel();
//		}
	}
	
	private IEnumerator showAwardItem(int index, float time)
	{
      	yield return new WaitForSeconds(time);
		
		AwardItemArray[index].SetActive(true);
		TweenPosition posEffect = AwardItemArray[index].GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
	}

	private IEnumerator showExpLabel(float delTime)
	{
		yield return new WaitForSeconds(delTime);
		
		JingYanObj.SetActive(true);
		TweenPosition posEffect = JingYanObj.GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
	}
	
	private IEnumerator showChouJiangLabel(float delTime)
	{
		yield return new WaitForSeconds(delTime);
		
		ChouJiang.gameObject.SetActive(true);
		TweenPosition posEffect = ChouJiang.gameObject.GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
	}
	
	private void showChouJiangItem(float delTime)
	{
		float startTime= delTime;
		for ( int i = 0; i < AwardItemArray.Length; i++ )
		{
			CoroutineManager.StartCoroutine(showAwardItem(i, startTime));
			startTime = startTime + 0.2f;
		}
	}
	
	private void showRandExp()
	{
		string expValueStr = totalExp.ToString();
		int length = expValueStr.Length;
		int baseValue = 1;
		int randValue = 0;
		for( int i = 0; i < length; i++ )
		{
			int rand = Random.Range(0, 9);
			randValue += rand * baseValue;
			baseValue = baseValue * 10;
		}
		JingYanValueObj.text = randValue.ToString();
	}
	
	private void SetRealExpValue()
	{
		JingYanValueObj.text = totalExp.ToString();
		JingYanExtValueObj.text = "+" + extEp.ToString();
		ExtValueObj.SetActive(true);
		jingyangShowFinished = true;
		
		TweenPosition posEffect = ExtValueObj.GetComponent<TweenPosition>();
		if(posEffect != null)
		{
			posEffect.Reset();
			posEffect.enabled	= true;
		}
	}
	
	public void ClickLeaveScene()
	{
		if(IsSelect)
		{
			Hide();
			XEventManager.SP.SendEvent(EEvent.PassInfo_RetScene);
		}
		
		for(int i = 0; i < LevelResultNum; i++)
		{
			mSelItemArray[i].DestroyEffect();
		}
	}
	
	public void OnRotFinishFirst()
	{
		mSelItemArray[0].DisabelRotation();
		
		mSelItemArray[0].Hide();
		CS_Int.Builder builder = CS_Int.CreateBuilder();
		builder.SetData(0);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SubSceneAwardSel,builder.Build());
	}
	
	public void OnRotFinishSecond()
	{
		mSelItemArray[1].DisabelRotation();
		
		mSelItemArray[1].Hide();
		CS_Int.Builder builder = CS_Int.CreateBuilder();
		builder.SetData(1);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SubSceneAwardSel,builder.Build());
	}
	
	public void OnRotFinishThird()
	{
		mSelItemArray[2].DisabelRotation();
		
		mSelItemArray[2].Hide();
		CS_Int.Builder builder = CS_Int.CreateBuilder();
		builder.SetData(2);
		XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_SubSceneAwardSel,builder.Build());
	}
	
	public void SetSpriteDisable(int index)
	{
		SelIndex	= index;
		IsSelect	= true;
		for(int i = 0; i < LevelResultNum; i++)
		{
//			if(index != i)
			mSelItemArray[i].Disable();
		}
	}
	
	public void SetItemData(int itemIndex)
	{
		mSelItemArray[SelIndex].SetItemData(itemIndex);
	}
}
