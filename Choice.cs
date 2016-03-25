using UnityEngine;
using System.Collections;

public class Choice : MonoBehaviour {

	public Camera m_Camera;
	GameObject m_SelectedObject;

	public GameObject m_Effects;	// 틀릴 때 불꽃 튀기는 효과

	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Raycast - 레이캐스팅(광선캐스팅) 쏘는데 맞은애를 가져올 수 있음
		RaycastHit rayhit;	// 맞은 애 정보
		Ray ray = m_Camera.ScreenPointToRay (new Vector2(Screen.width/2,Screen.height/2)); // 카메라에서 레이를 쏜다 뭐 이런 뜻.
		if (Physics.Raycast (ray, out rayhit, 1000, 1 << LayerMask.NameToLayer ("Cube"))) {

			// 물리적인 광선 발사, 쏴서 맞은 놈과의 충돌체크.
			// rayhit 구조체의 정보를 채워줌

			if (rayhit.transform.tag == "Cube") { // 맞은 광선의 태그가 큐브 일때

				if (Input.GetMouseButtonDown (0) ) { // 마우스 클릭을 한다면

					if(rayhit.transform.name == "Gold")	// 클릭한 큐브가 황금큐브 일때	
					{
						rayhit.transform.GetComponent<Renderer>().material = GameObject.Find("Yaba").GetComponent<RotateManager>().m_GoldMaterial;
						// "Yaba"오브젝트를 찾아서 RotateManager의 황금큐브 메테리얼 정보를 가져온 후 대입한다  
					}
					else // 클릭한 큐브가 황금큐브가 아닐 경우
					{
						Instantiate(m_Effects,transform.position + new Vector3(0,0.4f,0.4f),m_Effects.transform.rotation);
						// 스파크 이팩트를 new Vector3() 위치에 생성한다.
					}


					// 수정해야할 부분
					// 1. 광선에 맞은 큐브가 골드큐브이면( i%4 == 0이면 ) 큐브 색깔이 다시 황금색으로 변하도록 코드작성
					// if(Input.GetMouseButtonDown (0) && (i%4 == 0) )
					//		알파값을 다시 -시켜준다음
					// 		m_Cube [i].m_Object.GetComponent<Renderer> ().material = m_GoldMaterial; 메테리얼을 골드큐브로 바꿔침
					//
					// 2. 광선에 맞은 큐브가 골드큐브가 아니면( i%4 != 0) 스파크가 튀도록 
					// else if( Input.GetMouseButtonDown(0) && (i%4 != 0) )
					//		Instantiate(m_Effects,transform.position + new Vector3(0,0.4f,0.4f),m_Effects.transform.rotation);
					//		( 틀렸기 때문에 불꽃스파크 이펙트가 튀도록 )

				}

				if(m_SelectedObject == null){
					m_SelectedObject = rayhit.transform.gameObject;
					m_SelectedObject.GetComponent<Renderer>().material.color = new Color(1,0,0,1);
				}

				if(m_SelectedObject != rayhit.transform.gameObject)
				{
					m_SelectedObject.GetComponent<Renderer>().material.color = new Color(1,1,1,1);
					m_SelectedObject = rayhit.transform.gameObject;
					m_SelectedObject.GetComponent<Renderer>().material.color = new Color(1,0,0,1);	
				}



			}

		}
		else if (m_SelectedObject != null){
			m_SelectedObject.GetComponent<Renderer>().material.color = new Color(1,1,1,1);
		}


		/*
		 * // 색깔 참고
		        public static Color red = new Color(1,0,0);
			    public static Color green = new Color(0,1,0);
			    public static Color blue = new Color(0,0,1);
			    public static Color purple = new Color(1,0,1);
			    public static Color yellow = new Color(1,1,0);
			    public static Color cyan = new Color(0,1,1);
			    public static Color orange = new Color(1,0.6f,0);
			    public static Color darkred = new Color(0.5f,0,0);
			    public static Color darkyellow = new Color(0.5f,0.5f,0);
			    public static Color darkblue = new Color(0,0,0.5f);
			    public static Color darkcyan = new Color(0,0.5f,0.5f);
			    public static Color darkpurple = new Color(0.5f,0,0.5f);
			    public static Color white = new Color(1,1,1);
			    public static Color black = new Color(0,0,0);
			    public static Color silver = new Color(0.76f,0.76f,0.76f);
			    public static Color gray = new Color(0.5f,0.5f,0.5f);

		 */

	}
}
