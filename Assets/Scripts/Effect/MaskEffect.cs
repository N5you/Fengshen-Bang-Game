using UnityEngine;
using System.Collections;
using resource;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/MaskEffect")]
[RequireComponent (typeof(Camera))]
public class MaskEffect : MonoBehaviour {
	
	public static uint MaskMaterialID = 4000;
	public static float TotalChangeTime = 1.0f;
	public Color    JustColor = Color.white;
	public float	Range = 1;
	public float    Power = 1;
	private float 	CurTime = 0.0f;
	public Material	UseMaterial;
	public XResourceMaterial ResMaterial;
	
	protected void Start ()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		
		if(UseMaterial == null)
			return ;
		
		UseMaterial.SetColor("_Color",JustColor);
		UseMaterial.SetFloat("_Range",Range);
		UseMaterial.SetFloat("_Power",Power);
	}
	
	public void LoadMaterial()
	{
		UseMaterial	= ResMaterial.ResMaterial;
		UseMaterial.SetColor("_Color",JustColor);
		UseMaterial.SetFloat("_Range",Range);
		UseMaterial.SetFloat("_Power",Power);
		enabled	= true;
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if(UseMaterial == null)
			return ;
		
		CurTime += Time.deltaTime ;
		float Rate = CurTime /TotalChangeTime;
		if(Rate > 1.0f)
			Rate	= 1.0f;
		
		UseMaterial.SetFloat("_Rate",Rate);
		
		Graphics.Blit (source, destination, UseMaterial);
	}
	
	protected void OnDisable() {
		CurTime	= 0.0f;
	}
	
	public void LoadCompleted(DownloadItem item)
	{
		LoadMaterial();
	}
}
