using UnityEngine;
using System.Collections;

// 야바위 큐브 클래스
public class Cube	
{
	public GameObject m_Object;	// 야바위 큐브를 넣을 게임오브젝트
	public Vector2 m_Position;	// 각 큐브마다의 x,y축
								// y축과 x축으로 한번씩 회전할거여서 x,y축 필요

	// 생성자
	public Cube(GameObject _Object, Vector2 _Position)
	{
		this.m_Object = _Object;
		this.m_Position = _Position;
	}
};

// 큐브 회전시키는 클래스
public class RotateManager : MonoBehaviour {
	Cube[] m_Cube = new Cube[16];		// 16개 큐브를 동적할당
	bool[] RotateOrder = new bool[5];	// 

	public GameObject m_OriginalCube;

	public Material m_GoldMaterial;
	public Material m_StoneMaterial;

	// 회전축
	Vector3 m_YPoint;	// y축으로 회전할 큐브의 중심축
	Vector3 m_XPoint;	// x축으로 회전할 큐브의 중심축

//	int rand1 = Random.Range (0, 4);
//	int rand2 = Random.Range (4, 8);
//	int rand3 = Random.Range (8, 12);
//	int rand4 = Random.Range (12, 16);


	void Start()
	{
		Initialize ();
		CreateCube ();
		StartCoroutine ("RotateObjects");
		StartCoroutine ("ChangeMaterial");
	}


	// 초기화
	void Initialize()
	{
		// 회전축 초기화
		// 첫 야바위 큐브 m_cube[0] 생성위치 (0,0,0)을 기준으로 값을 더함
		m_YPoint = transform.position + new Vector3 (0.7f, 0, 0.7f); 
		m_XPoint = transform.position + new Vector3 (0, 0.5f, 0.7f);
		// y는 2단이기 때문에 0.5

		for (int i=0; i<5; ++i) {
			RotateOrder[i] = false;
		}

	}


	void CreateCube()
	{
		int i = 0;
		for (int x=0; x<3; ++x) 
		{
			for(int y=0; y<2; ++y)
			{
				for(int z=0; z<3; ++z)
				{
					if(x == 1 && z == 1)
					{// 중심에 없는 두 개의 큐브
					 // x와 z의 값은 1이지만 y값만 다름
						continue;
					}
					/*
					GameObject a = Instantiate(aslkdfmasd) as GameObject;
					m_Cube[i] = new Cube(a, new Vector2(x,y));
					같은 문장이다.
					*/

					GameObject c = Instantiate(m_OriginalCube,
					                           transform.position + new Vector3(x*0.7f,y*1.0f,z*0.7f),
					                           m_OriginalCube.transform.rotation) as GameObject;

					m_Cube[i] = new Cube(c, new Vector2(x,y));
		
					//if (i == rand1 || i == rand2 || i == rand3 || i == rand4)
					if(i%4 == 0){
						m_Cube[i].m_Object.GetComponent<MeshRenderer>().material = m_GoldMaterial;
						m_Cube[i].m_Object.name = "Gold";
					}

					/*
					m_Cube[i] = new Cube(Instantiate(m_OriginalCube,transform.position + new Vector3(x*0.7f,y*0.7f,z*0.7f),m_OriginalCube.transform.rotation) as GameObject, new Vector2(x,y));
					if(i%4 == 0)
						m_Cube[i].m_Object.GetComponent<MeshRenderer>().material = m_GoldMaterial;
					*/

					i++;
				}
			}
		}

		InitializeCubeRotatePoint ();
	}

	public void SetBool(int index)
	{
		if (RotateOrder [index] != true) 
		{
			RotateOrder [index] = true;
		}
	}

	void InitializeCubeRotatePoint()
	{
		for (int nCube=0; nCube<16; ++nCube) {
			m_Cube[nCube].m_Object.GetComponent<CubeRotate>().m_XPoint = m_XPoint;
			m_Cube[nCube].m_Object.GetComponent<CubeRotate>().m_YPoint = m_YPoint;
		}
	}

	void RotateJudge(int j)
	{
		for (int i=0; i<16; ++i) {
			bool RotateSwitch = false;
			switch(j){
			case 0:
				if (m_Cube [i].m_Position.y == 1) {
					m_Cube [i].m_Object.GetComponent<CubeRotate> ().isXAxis = false;
					RotateSwitch = true;
				}
				break;
			case 1:
				if (m_Cube [i].m_Position.y == 0) {
					m_Cube [i].m_Object.GetComponent<CubeRotate> ().isXAxis = false;
					RotateSwitch = true;
				}
				break;
			case 2:
				if (m_Cube [i].m_Position.x == 0) {
					m_Cube [i].m_Object.GetComponent<CubeRotate> ().isXAxis = true;
					RotateSwitch = true;
				}
				break;
			case 3:
				if (m_Cube [i].m_Position.x == 1) {
					m_Cube [i].m_Object.GetComponent<CubeRotate> ().isXAxis = true;
					RotateSwitch = true;
				}
				break;
			case 4:
				if (m_Cube [i].m_Position.x == 2) {
					m_Cube [i].m_Object.GetComponent<CubeRotate> ().isXAxis = true;
					RotateSwitch = true;
				}
				break;
			}
			
			if(RotateSwitch){
				m_Cube [i].m_Object.GetComponent<CubeRotate> ().CurrentRotateIndex = j;
				m_Cube [i].m_Object.GetComponent<CubeRotate> ().Rotate ();
			}
		}
	}

	IEnumerator RotateObjects()
	{
		for (int j = 0; j<5; ++j) 
		{
			RotateJudge(j);
			while (!RotateOrder[j])
			{
				yield return null;
			}
		}

	}

	IEnumerator ChangeMaterial(){
		float fAlpha = 1;
		while(fAlpha>=0){
			fAlpha -= 0.01f;
			for (int i=0; i<16; ++i) 
			{
				//if (i == rand1 || i == rand2 || i == rand3 || i == rand4)
				if(i%4==0)
					m_Cube [i].m_Object.GetComponent<Renderer> ().material.SetColor("_TintColor",new Color (1, 1, 1, fAlpha));
			}
			yield return null;
		}

		for (int i=0; i<16; ++i) 
		{
			if(i%4==0)
			//if (i == rand1 || i == rand2 || i == rand3 || i == rand4)
			{
				m_Cube [i].m_Object.GetComponent<Renderer> ().material = m_StoneMaterial;
				m_Cube [i].m_Object.GetComponent<Renderer> ().material.color = new Color(1,1,1,0);
			}
		}

		while(fAlpha<=1){
			fAlpha += 0.01f;
			for (int i=0; i<16; ++i) {
				if(i%4==0)
					m_Cube [i].m_Object.GetComponent<Renderer> ().material.SetColor("_Color",new Color (1, 1, 1, fAlpha));
			}
			yield return null;
		}
	}

}
