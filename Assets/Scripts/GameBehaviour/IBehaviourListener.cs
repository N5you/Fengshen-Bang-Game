using System;
using UnityEngine;

public interface IBehaviourListener
{
	void OnMouseDown(int mouseCode, Vector3 clickPoint);
	void OnMouseUp(int mouseCode);
    void OnMouseUpAsButton(int mouseCode);
    void OnMouseEnter();
	void OnMouseExit();
    void OnControllerColliderHit(ControllerColliderHit hit);
	void OnCancelSelect();
}
