using System;
using UnityEngine;
using System.Collections;

//--4>: 注意, 这个名字一定要和实际动画名字前缀一致(区分大小写)
public enum EAnimName
{
	Idle = 0,
	Walk,
	Run,
	Jump,
	Drop,
	DropRun,
	DropStand,
	Attack1,
	Attack2,
	Attack3,
	Attack4,
	Cast,
	Death,
	Deffence,
	Dodge,
	Fight,
	Shoot,
	Sit,
	Wound,
	RideIdle,
	RideRun,
	FloatIdle,
	FloatRun,
	Pick,
	Fish,
	Dig,
	Specialidle,
    // ==================
    eAnimNameCount,
}

class XAnimationManager
{
    private float[,] m_AnimCrossTable = new float[(int)EAnimName.eAnimNameCount, (int)EAnimName.eAnimNameCount];

    #region Singleton Pattern

    private XAnimationManager()
    {
        for (int i = 0; i < (int)EAnimName.eAnimNameCount; ++i)
        {
            for (int j = 0; j < (int)EAnimName.eAnimNameCount; ++j)
            {
                //m_AnimCrossTable[i, j] = 0.25f;
				m_AnimCrossTable[i, j] = 0.1f;
            }
        }
        m_AnimCrossTable[(int)EAnimName.Idle, (int)EAnimName.Run] = 0.2f;
        m_AnimCrossTable[(int)EAnimName.Idle, (int)EAnimName.Jump] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.Run, (int)EAnimName.Jump] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.Run, (int)EAnimName.Idle] = 0.2f;
        m_AnimCrossTable[(int)EAnimName.Drop, (int)EAnimName.DropStand] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.Drop, (int)EAnimName.DropRun] = 0.0f;
        m_AnimCrossTable[(int)EAnimName.DropRun, (int)EAnimName.Run] = 0.1f;
    }

    public static XAnimationManager SP = new XAnimationManager();

    #endregion

    public void PlayAnimation(Animation u3dAnimation, EAnimName now, EAnimName next, float fSpeed, bool bIsPush)
    {
		string name = "" + next;
        if(u3dAnimation == null || u3dAnimation[name] == null)
            return;
		
        float fadeTime = m_AnimCrossTable[(int)now, (int)next];
		//u3dAnimation[name].speed = fSpeed;
		
		if(bIsPush)
		{
			//u3dAnimation.CrossFadeQueued(name, fadeTime, QueueMode.PlayNow);
			u3dAnimation.CrossFadeQueued(name, fadeTime);
		}			
		else 
			u3dAnimation.CrossFade(name, fadeTime);
    }
}
