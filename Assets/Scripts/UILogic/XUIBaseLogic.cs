using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XUIBaseLogic")]
public class XUIBaseLogic : MonoBehaviour
{
	public uint PanelKey  {get; set;}
	public virtual bool Init()
	{	
       	this.Reset();
		return true;
	}
	
	public virtual bool Breathe()
	{ 
		return true; 
	}
		
	public virtual void Show()
	{
		gameObject.SetActive(true);
		XEventManager.SP.SendEvent(EEvent.UI_OnShow, this);
	}
	
	public virtual void Hide()
	{
		gameObject.SetActive(false);
		XEventManager.SP.SendEvent(EEvent.UI_OnHide, this);
	}

    public virtual void Toggle()
    {
		if (gameObject.activeSelf)
        {
            this.Hide();
        }
        else
        {
            this.Show();
        }
    }
	
    // 重置界面
    public virtual void Reset()
    {
    }
	
	public virtual void ShowData(params object[] args)
	{
	}
}
