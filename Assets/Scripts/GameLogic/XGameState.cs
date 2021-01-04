/*using System;

//--4>TODO: 登录, 选角过程需要增加状态
enum EGameState
{
    Unknown,
	LoadingCharScene,
	CharScene,
    LoadingNormalScene,
    NormalScene,
    LoadingClientScene,
    ClientScene,
    LoadingFightScene,
    FightScene,
}

//--4>TODO: 增加状态机
class XGameState
{
    public EGameState State { get; set; }

    public XGameState() 
    {
        State = EGameState.Unknown;
    }

    public void OnLoadScene(ESceneType t)
    {
        switch (t)
        {
            case ESceneType.NormalScene:
                State = EGameState.LoadingNormalScene;
                break;
            case ESceneType.ClientScene:
                State = EGameState.LoadingClientScene;
                break;
            case ESceneType.FightScene:
                State = EGameState.LoadingFightScene;
                break;
			case ESceneType.CharScene:
				State = EGameState.LoadingCharScene;
				break;			
            default:
                //--4>TODO: log error
                break;
        }
    }

    public void OnSceneLoaded()
    {
        switch (State)
        {
            case EGameState.LoadingNormalScene:
                State = EGameState.NormalScene;
                break;
            case EGameState.LoadingClientScene:
                State = EGameState.ClientScene;
                break;
            case EGameState.LoadingFightScene:
                State = EGameState.FightScene;
                break;
			case EGameState.LoadingCharScene:
				State = EGameState.CharScene;
				break;
            default:
                //--4>TODO: log error
                break;
        }
    }

    public bool IsLoading
    {
        get
        {
            return In(EGameState.LoadingCharScene, EGameState.LoadingNormalScene, EGameState.LoadingClientScene, EGameState.LoadingFightScene);
        }
    }

    public bool IsInNormalScene
    {
        get
        {
            return In(EGameState.NormalScene);
        }
    }

    public bool IsInClientScene
    {
        get { return In(EGameState.ClientScene); }
    }

    public bool IsInFightScene
    {
        get { return In(EGameState.FightScene); }
    }

    public bool IsSceneLoaded
    {
        get
        {
            return In(EGameState.CharScene, EGameState.NormalScene, EGameState.ClientScene, EGameState.FightScene);
        }
    }

    public bool IsSubSceneFlow
    {
        get
        {
            return In(EGameState.LoadingClientScene, EGameState.LoadingFightScene, EGameState.ClientScene, EGameState.FightScene);
        }
    }

    public bool IsInControl
    {
        get
        {
            return In(EGameState.NormalScene, EGameState.ClientScene);
        }
    }

    public bool IsInputLocked
    {
        get
        {
            return !IsInControl;
        }
    }

    public bool In(params EGameState[] states)
    {
        foreach (EGameState s in states)
        {
            if (s == State)
            {
                return true;
            }
        }
        return false;
    }
}
*/
