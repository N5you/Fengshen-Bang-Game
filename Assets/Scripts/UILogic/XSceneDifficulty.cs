using UnityEngine;
using System.Collections;

[AddComponentMenu("UILogic/XSceneDifficulty")]
public class XSceneDifficulty : XUIBaseLogic
{
/*	public enum DifficultUnit
	{
		easy = 0,
		normal,
		hard,
		hell,
	}

	internal class Difficulty
	{
		public UICheckbox Check = null;
		public UISlicedSprite Lock = null;
		public UISlicedSprite Passed = null;
		public DifficultUnit Diff;

		public Difficulty(GameObject _go)
		{
			this.Check = _go.GetComponent<UICheckbox>();
			this.Lock = this.Check.transform.FindChild("Lock").GetComponent<UISlicedSprite>();
			this.Passed = this.Check.transform.FindChild("Passed").GetComponent<UISlicedSprite>();

			UIEventListener listener = UIEventListener.Get(_go);
			listener.onClick = Click;
		}

		public Difficulty(UICheckbox _uic)
		{
			this.Check = _uic;
			this.Lock = this.Check.transform.FindChild("Lock").GetComponent<UISlicedSprite>();
			this.Passed = this.Check.transform.FindChild("Passed").GetComponent<UISlicedSprite>();

			UIEventListener listener = UIEventListener.Get(_uic.gameObject);
			listener.onClick = Click;
		}

		public void SetDiff(int _diff)
		{
			this.Diff = (DifficultUnit)_diff;
		}

		private void Click(GameObject _go)
		{
			XSceneDifficulty.SP.changeDifficult(this);
		}
	}

	public UILabel SceneName = null;
	public UISlicedSprite SceneIcon = null;
	public UILabel SceneInfor = null;
	public UIButton ChallengeBtn = null;
	public UIButton CloseBtn = null;
	public UISlicedSprite[] Stars = null;
	public UICheckbox[] SceneDifficulty = null;

	private Difficulty[] m_difficulty;

	public static XSceneDifficulty SP
	{
		get { return XWeUIManager.SP.WeUIRoot.SceneDifficulty; }
	}

	public override bool Init()
	{
		base.Init();

		UIEventListener challengeBtn = UIEventListener.Get(ChallengeBtn.gameObject);
		challengeBtn.onClick = challengeScene;
		UIEventListener closeBtn = UIEventListener.Get(CloseBtn.gameObject);
		closeBtn.onClick = Close;

		InitDifficulty();

		return true;
	}

	public override void Hide()
	{
		base.Hide();
	}

	public void SetName(string _name)
	{
		SceneName.text = _name;
	}

	private DifficultUnit changeDifficult(Difficulty _d)
	{
		return _d.Diff;
	}

	public void SetIcon(UIAtlas _atlas, string _spriteName)
	{
		SceneIcon.atlas = _atlas;
		SceneIcon.spriteName = _spriteName;
	}

	public void SetInfor(string _content)
	{
		SceneInfor.text = _content;
	}

	 public void challengeScene(GameObject _go)
	{

	}

	private void Close(GameObject _go)
	{
		Hide();
	}

	public void SetStars(int _num)
	{
		//float x = Stars[0].transform.parent.localPosition.x;
		for (int i = 0; i < 5; i++)
		{
			if (i < _num)
			{
				//set position
				//Vector3 pos = Stars[i].transform.localPosition;
				//Stars[i].transform.localPosition = new Vector3(x + i * 50, pos.y, pos.z);
				//
				Stars[i].enabled = true;
				continue;
			}
			Stars[i].enabled = false;
		}
	}

	public void SetDifficult(bool[] _lock, bool[] _passed)
	{
		if (_lock.Length != 4 || _passed.Length != 4) return;
		for (int i = 0; i < m_difficulty.Length; i++)
		{
			m_difficulty[i].Lock.enabled = _lock[i];
			m_difficulty[i].Passed.enabled = _passed[i];
		}
	}

	private void InitDifficulty()
	{
		m_difficulty = new Difficulty[SceneDifficulty.Length];
		for (int i = 0; i < SceneDifficulty.Length; i++)
		{
			m_difficulty[i] = new Difficulty(SceneDifficulty[i]);
			m_difficulty[i].SetDiff(i);
		}
	}*/
}
