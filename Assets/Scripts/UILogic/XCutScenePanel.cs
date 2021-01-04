using UnityEngine;
using System.Collections;

public class XCutScenePanel : XUIBaseLogic {
	
	public GameObject m_curName;
	public GameObject m_curWord;
	public GameObject m_leftView;
	public GameObject m_rightView;
	public GameObject m_leftRoot;
	public GameObject m_rightRoot;
	
	public GameObject m_topWord;
	
	public GameObject m_nextBtn;
	
	public GameObject m_BackGroundBlack;
	
	public GameObject m_BackGroundBlackText;
	
	public GameObject m_BackGroundBlackRoot;
	
	public GameObject m_otherRoot;
	
	public GameObject m_GameCutScneneRoot;
	
	private UILabel m_currentLabel = null;
	
	private float m_fSayWaitTime = -1.0f;
	
	public static bool m_activeTalk = false;
	
	public override bool Init()
	{
		bool b = base.Init();
		
		XEventManager.SP.AddHandler( playLeftSay,EEvent.CutScene_LeftSay );
		XEventManager.SP.AddHandler( playRightSay,EEvent.CutScene_RightSay );
		XEventManager.SP.AddHandler( playTopSay,EEvent.CutScene_TopSay );
		XEventManager.SP.AddHandler( playBlackWord,EEvent.CutScene_BackWord );
		XEventManager.SP.AddHandler( playBlackAlpha,EEvent.CutScene_BackAlpha );
		
		UIEventListener.Get(m_nextBtn).onClick += clickNextCallBack;
		
		return b;
	}
	
	public override void Show()
	{
		base.Show();
		clearPlayer();
		
		m_GameCutScneneRoot.SetActive(m_activeTalk );
	}
	
	public override void Hide()
	{
		base.Hide();
	}
	
	private void updateSayWaitTime()
	{
		if( 0.0f < m_fSayWaitTime && null != m_currentLabel )
		{
			TypewriterEffect effect = m_currentLabel.gameObject.GetComponent<TypewriterEffect >();
			if(null!=effect )  //only word show over need time work
				return;
			
			m_fSayWaitTime-= Time.deltaTime;
			
			if( 0.0f < m_fSayWaitTime ) //time is over
				return;
			
			XEventManager.SP.SendEvent(EEvent.CutScene_NextTalk,null );
		}
		
	}
	
	private void clearPlayer()
	{
		m_leftRoot.SetActive(false);
		m_rightRoot.SetActive(false);
		m_topWord.SetActive(false);
		m_otherRoot.SetActive(false);
		
		setBlackGroundAlpha(0.0f);
		
		m_fSayWaitTime = -1.0f;
	}
	
	private void clickNextCallBack(GameObject _go)
	{
		#if RES_DEBUG
			//need jump to end
			if( !m_activeTalk )//need finish in debug open init cutscene 
			{
				XCutSceneMgr.SP.finishCutScene();
			}
		#endif
		
		if( null != m_currentLabel )
		{
			TypewriterEffect effect = m_currentLabel.gameObject.GetComponent<TypewriterEffect >();
			if( null== effect )
			{
				XEventManager.SP.SendEvent(EEvent.CutScene_NextTalk,null );
			}else
			{
				effect.overDelay();
				Destroy(effect);
			}
		}		
	}
	
	private void playLeftSay(EEvent evt, params object[] args )
	{
		m_leftRoot.SetActive(true);
		m_rightRoot.SetActive(false);
		m_topWord.SetActive(false);
		m_otherRoot.SetActive(true);
		
		XModelRTTMgr.SP.AddSingleModel((uint)args[0],m_leftView.GetComponent<UITexture>() );
		
		m_curName.GetComponent<UILabel>().text = (string)args[1];
		m_curWord.GetComponent<UILabel>().text = (string)args[2];
		
		m_curWord.AddComponent<TypewriterEffect>();
		
		m_currentLabel = m_curWord.GetComponent<UILabel>();
		
		m_fSayWaitTime = 2.0f;
	}
	
	private void playRightSay(EEvent evt, params object[] args )
	{
		m_leftRoot.SetActive(false);
		m_rightRoot.SetActive(true);
		m_topWord.SetActive(false);
		m_otherRoot.SetActive(true);
		
		XModelRTTMgr.SP.AddSingleModel((uint)args[0],m_rightView.GetComponent<UITexture>() );
		
		m_curName.GetComponent<UILabel>().text = (string)args[1];
		m_curWord.GetComponent<UILabel>().text = (string)args[2];
		
		m_curWord.AddComponent<TypewriterEffect>();
		
		m_currentLabel = m_curWord.GetComponent<UILabel>();
		
		m_fSayWaitTime = 2.0f;
	}
	
	private void playTopSay(EEvent e,params object[] args)
	{
		m_leftRoot.SetActive(false);
		m_rightRoot.SetActive(false);
		m_topWord.SetActive(true);
		m_otherRoot.SetActive(false);
		
		UILabel lable = m_topWord.GetComponent<UILabel>();
		
		lable.text = (string)args[0];
		
		m_currentLabel = lable;
		
		m_fSayWaitTime = 2.0f;
	}
	
	private void playBlackWord(EEvent e,params object[] args){
		m_BackGroundBlackText.GetComponent<UILabel>().text = (string)args[0];
		setBlackGroundAlpha(0.0f);
	}
	
	private void playBlackAlpha(EEvent e,params object[] args )
	{
		setBlackGroundAlpha((float)args[0] );
	}
	
	private void setBlackGroundAlpha(float alpha)
	{
		m_BackGroundBlack.GetComponent<UISprite>().alpha = alpha;
		m_BackGroundBlackText.GetComponent<UILabel>().alpha = alpha;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		updateSayWaitTime();
	}
}
