using UnityEngine;
using System.Collections;

public class Cube   // 야바위 큐브 클래스
{
    public GameObject m_Object;	// 야바위 큐브를 넣을 게임오브젝트
    public Vector3 m_Position;	// 각 큐브마다의 x,y축

    // 생성자
    public Cube(GameObject _Object, Vector3 _Position)
    {
        this.m_Object = _Object;
        this.m_Position = _Position;
    }
};

// 야바위 회전시키는 클래스
public class YabawiManager : MainRoomPropertyController{
    Cube[] m_Cube = new Cube[16];		// 16개 큐브를 동적할당
    bool RotateOrder = false;

    public GameObject BlockObject;		// 야바위가될 큐브
    public Material m_GoldMaterial;		// 황금야바위 재질
    public Material m_StoneMaterial;	// 일반야바위 재질
	//public Rigidbody m_rigid;

	public GameObject m_Effects;	// 틀릴 때 불꽃 튀기는 효과
	public GameObject m_Effects2;	// 맞을 때 동그라미 효과

    int RandomGoldCube1;	// 랜덤한 위치에 생성될 야바위
    int RandomGoldCube2;

	public int YabaNum = 2;

    // 회전축
    Vector3 m_YPoint;	// y축으로 회전할 큐브의 중심축
    Vector3 m_XPoint;	// x축으로 회전할 큐브의 중심축
    Vector3 m_DiagonalPoint; // 대각선
    
    void Awake()
    {
		//m_rigid = GetComponent<Rigidbody>();
        CreateCube();
    }

    public override void EnterRoom()
    {
        Debug.Log("메인게임 야바위 방에 들어왔습니다.");
        Initialize();
        RayEvent.OnLeftClick += OnLeftClick;
        RayEvent.OnHover += OnHover;
        RayEvent.OnHoverEnd += OnHoverEnd;

    }

    public override void Update()
    {
        base.Update();

		// 스페이스를 누르면 섞이기 시작
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			StartCoroutine("RotateObjects");
			StartCoroutine("ChangeMaterial");
		}

    }

    public override void LeaveRoom()
    {
        Debug.Log("메인게임 야바위 방에서 나갔습니다.");
    }

    public override void Clear()
    {
        StopCoroutine("FindPathGame");

		StartCoroutine ("SlowClear");


        Debug.Log("메인게임 야바위 클리어 했습니다.");
    }

    void Initialize()
    {
        // 첫 야바위 큐브 m_cube[0] 생성위치 (0,0,0)을 기준으로 값을 더함
        m_YPoint = transform.localPosition + new Vector3(0, -3, 0.7f);
        m_XPoint = transform.localPosition + new Vector3(-0.7f, -2.5f, 0.7f);
        m_DiagonalPoint = transform.localPosition + new Vector3(0, -2.5f, 0.7f);
        // y는 2단이기 때문에 0.5

        RotateOrder = false;


        NotReduplicateRandom();
        
        for (int i = 0; i < 16; ++i)
        {
            if (i == RandomGoldCube1 || i == RandomGoldCube2)
            {
                m_Cube[i].m_Object.GetComponent<MeshRenderer>().material = m_GoldMaterial;
                m_Cube[i].m_Object.name = "Gold";
            }
            else
            {
                m_Cube[i].m_Object.GetComponent<MeshRenderer>().material = m_StoneMaterial;
                m_Cube[i].m_Object.name = "Cube";
            }
        }
    }

    void NotReduplicateRandom()
    {
        RandomGoldCube1 = Random.Range(0, 8);
        RandomGoldCube2 = Random.Range(8, 16);
        if (RandomGoldCube1 == RandomGoldCube2)
            NotReduplicateRandom();
    }

    void CreateCube()
	{
        int i = 0;
        for (int x = 0; x < 3; ++x)
        {
            for (int y = 0; y < 2; ++y)
            {
                for (int z = 0; z < 3; ++z)
                {
                    if (x == 1 && z == 1)
                    {// 중심에 없는 두 개의 큐브
                        // x와 z의 값은 1이지만 y값만 다름
                        continue;
                    }

                    GameObject c = Instantiate(BlockObject) as GameObject;

                    c.transform.parent = gameObject.transform;
                    c.transform.localPosition = new Vector3(-0.7f + x * 0.7f,-3 + y * 1.0f, z * 0.7f);
                    m_Cube[i] = new Cube(c, new Vector3(x, y, z));

                    i++;
                }
            }
        }
    }

    public void SetBool()
    {
        RotateOrder = true;
    }

    //실제로 돌려주는 루틴
    void RotateJudge(int j)
    {
        for (int i = 0; i < 16; ++i)
        {
            bool RotateSwitch = false;
            switch (j)
            {
                case 0:
                    if (m_Cube[i].m_Position.y == 1)
                    {
                        RotateSwitch = true;
                    }
                    break;
                case 1:
                    if (m_Cube[i].m_Position.y == 0)
                    {
                        RotateSwitch = true;
                    }
                    break;
                case 2:
                    if (m_Cube[i].m_Position.x == 0)
                    {
                        RotateSwitch = true;
                    }
                    break;
                case 3:
                    if (m_Cube[i].m_Position.x == 1)
                    {
                        RotateSwitch = true;
                    }
                    break;
                case 4:
                    if (m_Cube[i].m_Position.x == 2)
                    {
                        RotateSwitch = true;
                    }
                    break;
                case 5:
                    if (m_Cube[i].m_Position == new Vector3(2, 1, 0) || m_Cube[i].m_Position == new Vector3(0, 1, 2)
                        || m_Cube[i].m_Position == new Vector3(2, 0, 0) || m_Cube[i].m_Position == new Vector3(0, 0, 2))
                    {
                        RotateSwitch = true;
                    }
                    break;
                case 6:
                    if (m_Cube[i].m_Position == new Vector3(0, 0, 0) || m_Cube[i].m_Position == new Vector3(0, 1, 0)
                        || m_Cube[i].m_Position == new Vector3(2, 1, 2) || m_Cube[i].m_Position == new Vector3(2, 0, 2))
                    {
                        RotateSwitch = true;
                    }
                    break;
            }

            if (RotateSwitch)
            {
                m_Cube[i].m_Object.GetComponent<CubeRotate>().CurrentRotateIndex = j;
                switch (j)
                {
                    case 0:
                        m_Cube[i].m_Object.GetComponent<CubeRotate>().Rotate(m_YPoint, new Vector3(0, 1, 0));
                        break;
                    case 1:
                        m_Cube[i].m_Object.GetComponent<CubeRotate>().Rotate(m_YPoint, new Vector3(0, 1, 0));
                        break;
                    case 2:
                        m_Cube[i].m_Object.GetComponent<CubeRotate>().Rotate(m_XPoint, new Vector3(1, 0, 0));
                        break;
                    case 3:
                        m_Cube[i].m_Object.GetComponent<CubeRotate>().Rotate(m_XPoint, new Vector3(1, 0, 0));
                        break;
                    case 4:
                        m_Cube[i].m_Object.GetComponent<CubeRotate>().Rotate(m_XPoint, new Vector3(1, 0, 0));
                        break;
                    case 5:
                        m_Cube[i].m_Object.GetComponent<CubeRotate>().Rotate(m_DiagonalPoint, new Vector3(0.5f, 0, 0.5f));
                        break;
                    case 6:
                        m_Cube[i].m_Object.GetComponent<CubeRotate>().Rotate(m_DiagonalPoint, new Vector3(-0.5f, 0, 0.5f));
                        break;
                }
            }
        }
    }

    IEnumerator RotateObjects()
    {
        // 세번 돌리는데 어느쪽으로 돌릴지는 랜덤
        for (int j = 0; j < 6; ++j)		// 회전 수 조절
        {
            int rand = Random.Range(0, 7);

            RotateJudge(rand);
            while (!RotateOrder)
            {
                yield return null;
            }
            RotateOrder = false;
        }

    }


	// 서서히 없어지게 보이도록
	IEnumerator SlowClear()
	{
		float fAlpha = 1;

		// 클리어시 야바위 알파값 줄여서 천천히 없어지게 보이도록 
		while (fAlpha >= 0) 
		{
			fAlpha -= 0.01f;
					

			for (int i = 0; i < 16; ++i) 
			{
				m_Cube[i].m_Object.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1, 1, 1, fAlpha));
				m_Cube[i].m_Object.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, fAlpha));
				m_Cube[i].m_Object.AddComponent<Rigidbody>();
			}

			yield return null;
		

		}

		// 알파값 줄인다음 야바위 없애고 리지드바디 삭제
		for (int i = 0; i < 16; ++i) 
		{
			m_Cube [i].m_Object.SetActive (false);
			Destroy( m_Cube[i].m_Object.AddComponent<Rigidbody>() );
		}


	}
	
	// 야바위의 알파값 조절
    IEnumerator ChangeMaterial()
    {
        float fAlpha = 1;

		// 1. 회전하면서 야바위가 하얗게 없어지는 것처럼 보이기 위해 알파값을 점점 줄여준다. 
        while (fAlpha >= 0)
        {
            fAlpha -= 0.01f;
            for (int i = 0; i < 16; ++i)
            {
                //if (i == RandomGoldCube1 || i == RandomGoldCube2)
                    m_Cube[i].m_Object.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1, 1, 1, fAlpha));
            }
            yield return null;
        }

		// 2. 알파값을 모두 줄여 0이 되면 모두 안보이게 되고 그런 다음
		//    황금야바위의 메테리얼도 검은야바위로 바꾼다.
        for (int i = 0; i < 16; ++i)
        {
            if (i == RandomGoldCube1 || i == RandomGoldCube2)
            {
                m_Cube[i].m_Object.GetComponent<Renderer>().material = m_StoneMaterial;
                m_Cube[i].m_Object.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
            }
        }

		// 3. 모두 검은야바위로 되었으니 다시 알파값을 높여준다.
        while (fAlpha <= 1)
        {
            fAlpha += 0.01f;
            for (int i = 0; i < 16; ++i)
            {
                if (i == RandomGoldCube1 || i == RandomGoldCube2)
                    m_Cube[i].m_Object.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, fAlpha));
            }
            yield return null;
        }
    }

    void OnLeftClick(GameObject obj)
    {
        if(obj.CompareTag("Cube"))
        {
            if (obj.transform.name == "Gold")
            {
				obj.transform.GetComponent<Renderer>().material = m_GoldMaterial;
				Instantiate ( m_Effects2, obj.transform.position, m_Effects2.transform.rotation );

				YabaNum = YabaNum - 1;

				if(YabaNum == 0){
					_isClearConditionCompleted = true;
				}



            }
            else
            {
                //HPManager.instance.GetDamage(30);
				Instantiate(m_Effects, obj.transform.position, m_Effects.transform.rotation);

            }
        }
    }

    void OnHover(GameObject obj)
    {
        if (obj.CompareTag("Cube"))
        {
            obj.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void OnHoverEnd(GameObject obj)
    {
        if (obj.CompareTag("Cube"))
        {
            obj.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
