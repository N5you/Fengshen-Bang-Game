//方便检测是否一定时间后可以执行
//时间单位为妙
public class TimeCalc
{
	public void CleanUp()
	{
		mTerm		= 0;
		mLeftTime	= 0;
		mIsStart	= false;
		mEndDel		= false;
		mIsInit		= false;
	}
	
	public bool IsStart()
	{
		if(mIsInit)
		{
			if(mEndDel)
				return false;
			
			return true;
		}
		else
			return mIsStart;
	}
	
	public void BeginTimeCalc(float term,bool isDel)
	{
		mIsStart = true;
		mTerm	 = term;
		mLeftTime= term;
		mEndDel	 = isDel;
		mIsInit	 = true;
	}
	
	public bool CountTime(float time)
	{
		if(!mIsStart)
			return false;

		mLeftTime	-= time;
		if(mLeftTime < 0)
		{
			if(mEndDel)
				mIsStart	= false;
			else
				mLeftTime	= mTerm;
			return true;
		}
		
		return false;
	}
	
	public float 	GetLeftTime()
	{
		return mLeftTime;
	}
	
	private float 	mTerm;
	private float	mLeftTime;
	private bool 	mIsStart;
	private bool 	mEndDel;
	private bool 	mIsInit;
}
