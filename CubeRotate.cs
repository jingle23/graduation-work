using UnityEngine;
using System.Collections;

public class CubeRotate : MonoBehaviour {
	public bool isXAxis = false;
	public int CurrentRotateIndex = 0;

	public Vector3 m_XPoint;
	public Vector3 m_YPoint;
	
	public void Rotate(){
		StartCoroutine ("RotateCoroutine");
	}

	IEnumerator RotateCoroutine(){
		yield return new WaitForSeconds(0.5f);

		float fAngle = 0;
		if (isXAxis) {
			while(fAngle<360){
				float frameAngle = Time.deltaTime * 200;
				fAngle += frameAngle;
				transform.RotateAround(m_XPoint,new Vector3(1,0,0), frameAngle);
				yield return null;
			}
		} else {
			while(fAngle<360){
				float frameAngle = Time.deltaTime * 200;
				fAngle += frameAngle;
				transform.RotateAround(m_YPoint,new Vector3(0,1,0), frameAngle);
				yield return null;
			}
		}

		GameObject.Find ("Yaba").GetComponent<RotateManager> ().SetBool (CurrentRotateIndex);
	}
}
