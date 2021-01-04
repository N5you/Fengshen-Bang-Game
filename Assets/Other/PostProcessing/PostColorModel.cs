using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Post Color Model")]
public class PostColorModel : ImageEffectBase {
	
	public float effectValue = 1.0f;
	
	public Color effectColor = Color.red;
	
	public Color modelColor = Color.black;
	
	public int m_effectLayer = GlobalU3dDefine.Layer_BattleObject;
	
	private RenderTexture m_preTexture = null;
	
	private Camera m_colorModelCamera = null;
	
	private GameObject m_colorObj;
	
	void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if(null == material )
			return;
		
		material.SetTexture("_ModelTex",m_preTexture);
		
		Graphics.Blit(src,dest,material );
    }
	
	// Use this for initialization
	void Start () {
		if(null == material )
			return;
		
		if(null!=m_colorObj)
		{
			GameObject.DestroyImmediate(m_colorObj);
		}
		m_preTexture = new RenderTexture(Screen.width,Screen.height,16);
		m_preTexture.hideFlags = HideFlags.HideAndDontSave;
		
		m_colorObj = new GameObject();
		
		//color camera
		m_colorModelCamera = m_colorObj.AddComponent<Camera>();
		
		m_colorObj.transform.parent = transform;
		
		m_colorObj.transform.localPosition = Vector3.zero;
		m_colorObj.transform.localRotation = Quaternion.identity;
		
		m_colorModelCamera.fieldOfView = GetComponent<Camera>().fieldOfView;
		m_colorModelCamera.depth = GetComponent<Camera>().depth - 0.1f;			//pre render
		m_colorModelCamera.targetTexture = m_preTexture;
		m_colorModelCamera.cullingMask = 1<<m_effectLayer;
		
		m_colorModelCamera.backgroundColor = new Color(-1f,-1f,-1f,-1f);
		m_colorModelCamera.clearFlags = CameraClearFlags.SolidColor;
		
		//mainCamera's material
		material.SetTexture("_ModelTex",m_preTexture);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(null == material)
			return;
		
		material.SetFloat("_Modulus",effectValue);
		
		material.SetColor("_ModelBackColor",effectColor);
		
		material.SetColor("_ModelColor",modelColor);
		
		m_colorModelCamera.backgroundColor = effectColor;
	}

}
