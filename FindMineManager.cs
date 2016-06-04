using UnityEngine;
using System.Collections;

public class FindMineManager : MainRoomPropertyController
{
    public GameObject BlockPrefab;

	public GameObject Mine_Explosion;

	public GameObject One;
	public GameObject Two;
	public GameObject Three;
	public GameObject Flag;
	public GameObject Zero;

    public Material PlaneMaterial;	// 뒤집기 전 색깔
    public Material AfterClickPlaneMaterial;	// 뒤집고 난 후 색깔

    public Material OneMaterial;
    public Material TwoMaterial;
    public Material ThreeMaterial;
    public Material FlagMaterial;
    public Material MineMaterial;

    int[,] MineArray = new int[5, 5];

	GameObject[,] MineObject = new GameObject[5, 5];
	//GameObject[,] MineObject = new GameObject[5, 5];

    void Awake()
    {
        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
            {
                MineArray[i, j] = 0;
                MineObject[i, j] = Instantiate(BlockPrefab) as GameObject;

                MineObject[i, j].transform.parent = gameObject.transform;
				MineObject[i, j].transform.rotation = gameObject.transform.rotation;

                MineObject[i, j].transform.name = i.ToString() + "," + j.ToString();

                //MineObject[i, j].transform.localPosition = new Vector3(-3 + i * 1.5f, -3.7f, -3 + j * 1.5f);
				MineObject[i, j].transform.localPosition = new Vector3(-2 + i * 1.0f, -3.8f, -2 + j * 1.0f);

            }
    }
    // Use this for initialization
    public override void EnterRoom()
    {
        StartCoroutine("FindMineGame");
        RayEvent.OnLeftClick += OnLeftClick;
        RayEvent.OnRightClick += OnRightClick;
        RayEvent.OnHover += OnHover;
        RayEvent.OnHoverEnd += OnHoverEnd;
    }

    public override void Update()
    {
        base.Update();


    }

    public override void Clear()
    {
        StopCoroutine("FindMineGame");

        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
                MineObject[i, j].SetActive(false);
       
    }

    public override void LeaveRoom()
    {
        RayEvent.OnLeftClick -= OnLeftClick;
        RayEvent.OnRightClick -= OnRightClick;
        RayEvent.OnHover -= OnHover;
        RayEvent.OnHoverEnd -= OnHoverEnd;
    }

    void OnLeftClick(GameObject obj)
    {
        if (obj.CompareTag("FindMineBlock"))
        {
            for(int i=0;i<5;++i)
                for(int j=0;j<5;++j)
            {
                if(MineObject[i,j].name == obj.name)
                {
                    CalculateMine(i, j);
                }
            }
        }
    }

    void OnRightClick(GameObject obj)
    {
        if (obj.CompareTag("FindMineBlock"))
        {
            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                {
                    if (MineObject[i, j].name == obj.name)
                    {
					if(MineArray[i,j] != 3){
						MineArray[i,j] = 3;
                        //MineObject[i, j].GetComponent<Renderer>().material = FlagMaterial;
						string name = MineObject[i,j].name;
						MineObject[i,j] = Instantiate( Flag,
					                              	    new Vector3( MineObject[i,j].transform.position.x,
					            									 MineObject[i,j].transform.position.y + 0.3f,
					             									 MineObject[i,j].transform.position.z),
					                              	    Flag.transform.rotation ) as GameObject;
						MineObject[i,j].name = name;
					/*
					MineObject[i,j] = Instantiate( Flag ) as GameObject;
					MineObject[i,j].transform.rotation = Flag.transform.rotation;
					MineObject[i,j].transform.localPosition = new Vector3( MineObject[i,j].transform.localPosition.x,
					                                                      MineObject[i,j].transform.localPosition.y + 0.3f,
					                                                      MineObject[i,j].transform.localPosition.z);
					*/
                    }
					else{
						Debug.Log("클릭한적이 있는놈임!");
					}
					}
					
                }
        }
    }

    void OnHover(GameObject obj)
    {
        if (obj.CompareTag("FindMineBlock"))
        {
            obj.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void OnHoverEnd(GameObject obj)
    {
        if (obj.CompareTag("FindMineBlock"))
        {
            if (obj.GetComponent<Renderer>().material.mainTexture == FlagMaterial.mainTexture)
            {
                obj.GetComponent<Renderer>().material.color = Color.white;
                return;
            }
            else
            {
                obj.GetComponent<Renderer>().material.color = PlaneMaterial.color;
                return;
            }
        }
    }

    IEnumerator FindMineGame()
    {
        Initialize();

        for (int i = 0; i < 3; ++i)
        {
            NotReduplicationRandom(true);   // 지뢰 생성
        }

        for (int i = 0; i < 5; ++i)
        {
            NotReduplicationRandom(false);  // 처음에 보여줄곳 생성
        }

        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
            {
                if (MineArray[i, j] == 2)
                {
                    CalculateMine(i, j);
                }
            }
        return null;
    }

    void Initialize()
    {
        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
            {
                MineArray[i, j] = 0;
                MineObject[i, j].GetComponent<Renderer>().material = PlaneMaterial;
                MineObject[i, j].tag = "FindMineBlock";
                MineObject[i, j].SetActive(true);
            }
    }

    void NotReduplicationRandom(bool isMine)
    {
        int randX = Random.Range(0, 5);
        int randY = Random.Range(0, 5);
        if (MineArray[randX, randY] != 0)
        {
            NotReduplicationRandom(isMine);
            return;
        }
        if (isMine)
            MineArray[randX, randY] = 1;
        else
            MineArray[randX, randY] = 2;
    }

    void CalculateMine(int x, int y)
    {
        switch (MineArray[x, y])
        {
            case 1:
                MineObject[x, y].GetComponent<Renderer>().material = MineMaterial;
                transform.GetChild(0).localPosition = MineObject[x, y].transform.localPosition;
                transform.GetChild(0).gameObject.SetActive(true);
			// transform.getchild(순서)  오브젝트 하위에 자식 오브젝트를 순서대로 접근할때 사용

				//Invoke("DisableExplosion", 2.0f);
                // 게임오버 처리
				
				Instantiate( Mine_Explosion, MineObject[x,y].transform.position, MineObject[x,y].transform.rotation );
                break;
            
			default:
                int mine = 0;
                for (int i = -1; i <= 1; ++i)
                {
                    for (int j = -1; j <= 1; ++j)
                    {
                        int beforeClampIteratorX = x + i;
                        int beforeClampIteratorY = y + j;

                        int afterClampIteratorX = Mathf.Clamp(beforeClampIteratorX, 0, 4);
                        int afterClampIteratorY = Mathf.Clamp(beforeClampIteratorY, 0, 4);

                        if ((beforeClampIteratorX != afterClampIteratorX) || (beforeClampIteratorY != afterClampIteratorY))
                        {
                            continue;
                        }

                        if (MineArray[beforeClampIteratorX, beforeClampIteratorY] == 1)
                        {
                            mine++;
                        }
                    }
                }

                switch (mine)
                {
                    case 0:
						MineObject[x,y] = Instantiate( Zero,
				                             		    new Vector3( MineObject[x,y].transform.position.x,
				           							   				MineObject[x,y].transform.position.y + 0.3f,
				            										MineObject[x,y].transform.position.z),
				                              		    Quaternion.Euler( new Vector3(-90, 0, 0) ) ) as GameObject;
						break;

					case 1:
						MineObject[x,y] = Instantiate( One,
				                              		    new Vector3( MineObject[x,y].transform.position.x,
				            										 MineObject[x,y].transform.position.y + 0.3f,
				            										 MineObject[x,y].transform.position.z),
				                              		    Quaternion.Euler( new Vector3(-90, 0, 0) ) ) as GameObject;     
                        break;
                    
					case 2:
						MineObject[x,y] = Instantiate( Two,
				                              			new Vector3( MineObject[x,y].transform.position.x,
				            										 MineObject[x,y].transform.position.y + 0.3f,
				            										 MineObject[x,y].transform.position.z),
				                              			Quaternion.Euler( new Vector3(-90, 0, 0) ) ) as GameObject;     						
						break;

                    case 3:
						MineObject[x,y] = Instantiate( Three,
				                              			new Vector3( MineObject[x,y].transform.position.x,
				            										 MineObject[x,y].transform.position.y + 0.3f,
				            										 MineObject[x,y].transform.position.z),
				                              			Quaternion.Euler( new Vector3(-90, 0, 0) ) ) as GameObject;     
                        break;
                }
                break;
        }
        MineObject[x, y].tag = "Untagged";

        int numofCurrentBeforeBlock = 0;
        for(int i=0;i<transform.childCount; ++i)
        {
            if(transform.GetChild(i).CompareTag("FindMineBlock"))
            {
                numofCurrentBeforeBlock++;
            }
        }
        if (numofCurrentBeforeBlock <= 3)
            _isClearConditionCompleted = true;
        
    }

    //void DisableExplosion()
    //{
    //    transform.GetChild(0).gameObject.SetActive(false);
    //}
}