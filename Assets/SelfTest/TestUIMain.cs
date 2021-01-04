using UnityEngine;
using System.Collections;

public class TestUIMain : MonoBehaviour {
	
	public XUIBaseLogic UI;
	private GameObject go;
	// Use this for initialization
	void Start () {
		go = XUtil.Instantiate(UI.gameObject,transform);
		go.transform.localScale = new Vector3(1,1,1);
		XUIBaseLogic baseLogic = go.GetComponent<XUIBaseLogic>();
		baseLogic.Init();
		baseLogic.Show();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
