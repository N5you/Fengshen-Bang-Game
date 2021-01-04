
using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XFriend")]
public class XFriend : XDefaultFrame
{
    //	public GameObject ButtonConfirm = null;
    //	public UISprite ResultFont;
    //
    //	public override bool Init()
    //	{
    //		base.Init();
    //		UIEventListener listen = UIEventListener.Get(ButtonConfirm.gameObject);
    //		listen.onClick += OnClickConfirm;
    //		return true;
    //	}
    //
    //	public void OnClickConfirm(GameObject go)
    //	{
    //		XLogicWorld.SP.SubSceneManager.LeaveFightScene(false);
    //		Hide();
    //	}	

    public UIImageButton[] RightContainerBtn = new UIImageButton[8];
    public GameObject List;
    public GameObject FuncGather;
    public GameObject AddFriendsBtn;
    public GameObject CharmRankBtn;
    public GameObject FlowreHouseBtn;
    public Vector3 ListPos;
    public Vector3 FuncGatherPos;

    public override bool Init()
    {
        base.Init();

        InitUI();

        return true;
    }

    //  界面初始化
    public void InitUI()
    {
        UIEventListener listen1 = UIEventListener.Get(RightContainerBtn[0].gameObject);
        listen1.onClick += OnClickConfirm;
        UIEventListener listen2 = UIEventListener.Get(RightContainerBtn[1].gameObject);
        listen2.onClick += OnClickConfirm2;
        UIEventListener listen3 = UIEventListener.Get(RightContainerBtn[2].gameObject);
        listen3.onClick += OnClickConfirm3;
        UIEventListener listenCharmRankBtn = UIEventListener.Get(CharmRankBtn);
        listenCharmRankBtn.onClick += OnCharmRankBtn;
        UIEventListener listenFlowerHouseBtn = UIEventListener.Get(FlowreHouseBtn);
        listenFlowerHouseBtn.onClick += OnFlowerHouseBtn;

        FuncGatherPos = FuncGather.transform.localPosition;
        ListPos = List.transform.localPosition;
        ShowFuncGather(false);
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
    }
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFriend);
	}

    //  显示功能按钮
    public void ShowFuncGather(bool _isShow)
    {
        if (_isShow)
        {
            List.transform.localPosition = ListPos;
            FuncGather.SetActive(true);
            return;
        }
        //List.transform.localPosition = FuncGatherPos;
        FuncGather.SetActive(false);
    }


    #region UIEvent

    //添加好友
    private void OnClickConfirm(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.Friend_AddFriend);
    }

    //删除好友
    private void OnClickConfirm2(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.Friend_DelFriend);
    }

    // 移动至黑名单
    private void OnClickConfirm3(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.Friend_MoveToBlackList);
    }

    //鲜花屋
    private void OnFlowerHouseBtn(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eFriendFlowerHouse);
    }

    //魅力榜
    private void OnCharmRankBtn(GameObject go)
    {
        XEventManager.SP.SendEvent(EEvent.UI_Toggle, EUIPanel.eFriendCharmRank);
    }
    #endregion
}
