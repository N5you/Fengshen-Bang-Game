using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

[AddComponentMenu("UILogic/XCharacterOperationUI")]
public class XCharacterOperationUI : XUIBaseLogic
{
	public static  Vector3 SelCharCameraPos = new Vector3(-6.243665f,-3.0f,-32f);
	public static  Vector3 ModelPos = new Vector3(-6.111599f,-4.15f,-25.89092f);
	private Vector3 ModelDir = new Vector3(0,0.0f,0);
	public static float SelCharCameraFOV = 30;
	public UILabel Label_CareerDescription = null;
	public UIRadioButton Radio_Sex = null;
	public UIRadioButton Radio_Career = null;
	public UIInput Input_Name = null;
	public GameObject RollObject = null;
	public GameObject StartGame = null;
	
	public GameObject m_ModelTrans;
	private uint[] m_arrModelIds = new uint[6];
	private XU3dModel[] m_arrModels = new XU3dModel[6];
	private XU3dModel m_curShowModel = null;
	
	public UICheckbox	CheckBoxMJ;
	public UICheckbox	CheckBoxQQ;
	public UICheckbox	CheckBoxFL;
	
	public UISprite		DutySprite;
	
	private string[]	DutySpriteName = new string[4] {"","11000852","11000851","11000850"};
	
	public override bool Init()
	{
		base.Init();
		
		UIEventListener listen = UIEventListener.Get(RollObject);
		listen.onClick += OnClickRoll;
		
		listen = UIEventListener.Get(StartGame);
		listen.onClick += OnClickStartGame;
		
		for(int i=0; i<3; i++)
		{
			XCfgPlayerBase cfgPlayerBase = XCfgPlayerBaseMgr.SP.GetConfig((byte)(i+1));
			m_arrModelIds[i * 2] = cfgPlayerBase.SelMaleModel;
			m_arrModelIds[i * 2 + 1] = cfgPlayerBase.SelFemaleModel;
		}
		return true;
	}
	
	public override void Show()
	{
		base.Show();
//		for(int i=0; i<m_arrModelIds.Length; i++)
//		{
//			XU3dModel u3dModel = new XU3dModel("xuanjue_" + i, EModelType.ePlayer, m_arrModelIds[i]);
//			u3dModel.Visible = false;
//			m_arrModels[i] = u3dModel;
//			u3dModel.Parent		= LogicApp.SP.transform;
//			u3dModel.Position 	= ModelPos;
//			u3dModel.Direction =  ModelDir;
//			u3dModel.PlayAnimation(EAnimName.Idle, 1.0f, false);
//		}
		
		Radio_Sex.onRadioChanged += OnSexChanged;
		Radio_Career.onRadioChanged += OnCareerChanged;		
		ChangeShowModel();
		Label_CareerDescription.text = XStringManager.SP.GetString((uint)(19 + Radio_Career.CurrentSelect));
		XEventManager.SP.SendEvent(EEvent.CharOper_RandomName, Radio_Sex.CurrentSelect + 1);
		Radio_Sex.CurrentSelect	= 1;
		
		
	}
	
	public override void Hide()
	{
		base.Hide();
		//XCameraLogic.SP.Detach();
		for(int i=0; i<m_arrModels.Length; i++)
		{
			if(null != m_arrModels[i])
			{
				m_arrModels[i].Destroy();
				m_arrModels[i] = null;
			}
		}
		m_curShowModel = null;
	}
	
	private void OnClickRoll(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.CharOper_RandomName, Radio_Sex.CurrentSelect + 1);
	}
	
	private void OnClickStartGame(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.CharOper_CreatePlayer, Input_Name.text, (uint)(Radio_Sex.CurrentSelect + 1), (uint)(Radio_Career.CurrentSelect + 1));
	}
	
	private void OnCareerChanged(int nIndex)
	{
		XEventManager.SP.SendEvent(EEvent.CharOper_SelectClassSex, Radio_Sex.CurrentSelect + 1, Radio_Career.CurrentSelect + 1);
		Label_CareerDescription.text = XStringManager.SP.GetString((uint)(19 + Radio_Career.CurrentSelect));
		DutySprite.spriteName = DutySpriteName[Radio_Career.CurrentSelect + 1];
		ChangeShowModel();
	}
	
	private void OnSexChanged(int nIndex)
	{
		XEventManager.SP.SendEvent(EEvent.CharOper_SelectClassSex, Radio_Sex.CurrentSelect + 1, Radio_Career.CurrentSelect + 1);
		if(Radio_Sex.CurrentSelect == 1)
		{
			//woman
			CheckBoxMJ.BackGround.spriteName	= "11000806";
			CheckBoxMJ.checkSprite.spriteName	= "11000803";
			
			CheckBoxQQ.BackGround.spriteName	= "11000809";
			CheckBoxQQ.checkSprite.spriteName	= "11000812";
			
			CheckBoxFL.BackGround.spriteName	= "11000818";
			CheckBoxFL.checkSprite.spriteName	= "11000815";
		}
		else
		{
			//man
			CheckBoxMJ.BackGround.spriteName	= "11000808";
			CheckBoxMJ.checkSprite.spriteName	= "11000810";
			
			CheckBoxQQ.BackGround.spriteName	= "11000811";
			CheckBoxQQ.checkSprite.spriteName	= "11000813";
			
			CheckBoxFL.BackGround.spriteName	= "11000814";
			CheckBoxFL.checkSprite.spriteName	= "11000816";
		}
		
		ChangeShowModel();
	}
	
	private void ChangeShowModel()
	{
		int n = Radio_Career.CurrentSelect * 2 + Radio_Sex.CurrentSelect;
		XU3dModel model = m_arrModels[n];
		if(model == null)
		{
			XU3dModel u3dModel = new XU3dModel("xuanjue_" + n, m_arrModelIds[n]);
			u3dModel.Visible 	= false;
			m_arrModels[n] 		= u3dModel;
			u3dModel.Parent		= LogicApp.SP.transform;
			u3dModel.Position 	= ModelPos;
			u3dModel.Direction 	=  ModelDir;
			u3dModel.PlayAnimation(EAnimName.Idle, 1.0f, false);
			model				= u3dModel;
		}
		
		if(model == m_curShowModel)
			return ;
		
		if(null != m_curShowModel) 
		{
			m_curShowModel.Visible = false;
		}
		
		m_curShowModel = model;
		if(null != m_curShowModel)
		{
			m_curShowModel.Visible = true;
			m_curShowModel.PlayAnimation(EAnimName.Idle, 1.0f, false);
		}
	}
	
	public void SetPlayerName(string strName)
	{
		Input_Name.text = strName;
	}
}

