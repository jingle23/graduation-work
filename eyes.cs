using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Aubergine;

public class eyes : MonoBehaviour {
	public RawImage m_image;
	PP_LensCircle m_lens;
	PP_BlurH m_blur;
	public Animator m_standing;
	

	// Use this for initialization
	void Start () {
		m_lens = GetComponent<PP_LensCircle> ();
		m_blur = GetComponent<PP_BlurH> ();

		//m_lens.radiusX
		//m_image.color.a
		//m_blur.blurStrength
		StartCoroutine ("gamgam");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator gamgam() {

		float a = 1;

		while( m_image.color.a > 0 )
		{
			m_image.color = new Color(0,0,0,a);
			a -= 0.4f * Time.deltaTime;
			yield return null;
		}

		while (m_blur.blurStrength > 0) 
		{
			m_blur.blurStrength -= 0.4f * Time.deltaTime;
			yield return null;
		}

		while (m_lens.radiusX < 2) 
		{
			m_lens.radiusX += 0.5f * Time.deltaTime;
			yield return null;
		}

		m_standing.GetComponent<Animator> ().SetBool ("isstanding", true);
		//m_standing.GetComponent<Animator> ().gameObject.transform.eulerAngles = new Vector3 (0, 0, 0);


	}

}

