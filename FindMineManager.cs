using UnityEngine;
using System.Collections;

public class FindMineManager : MainRoomPropertyController
{
    public GameObject BlockPrefab;

    // 메테리얼들 ( 열기전, 연후 비었을때 , 1 , 2, 3, 깃발, 지뢰
    public Material PlaneMaterial;
    public Material AfterClickPlaneMaterial;
    public Material OneMaterial;
    public Material TwoMaterial;
    public Material ThreeMaterial;
    public Material FlagMaterial;
    public Material MineMaterial;

    int[,] MineArray = new int[5, 5];
    GameObject[,] MineObject = new GameObject[5, 5];
    void Awake()
    {
        //초기화, 지뢰찾기 생성
        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
            {
                MineArray[i, j] = 0;
                MineObject[i, j] = Instantiate(BlockPrefab) as GameObject;
                MineObject[i, j].transform.parent = gameObject.transform;
                MineObject[i, j].transform.name = i.ToString() + "," + j.ToString();
                MineObject[i, j].transform.localPosition = new Vector3(-3 + i * 1.5f, -3.7f, -3 + j * 1.5f);
            }
    }
    // Use this for initialization
    public override void EnterRoom()
    {
        // 클릭 이벤트 등록
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

    // 클리어했을때
    public override void Clear()
    {
        StopCoroutine("FindMineGame");

        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
                MineObject[i, j].SetActive(false);
       
    }

    // 클릭 이벤트 등록해제
    public override void LeaveRoom()
    {
        RayEvent.OnLeftClick -= OnLeftClick;
        RayEvent.OnRightClick -= OnRightClick;
        RayEvent.OnHover -= OnHover;
        RayEvent.OnHoverEnd -= OnHoverEnd;
    }

    // 해당 위치를 클릭했을때
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

    // 우클릭 : 깃발
    void OnRightClick(GameObject obj)
    {
        if (obj.CompareTag("FindMineBlock"))
        {
            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                {
                    if (MineObject[i, j].name == obj.name)
                    {
                        MineObject[i, j].GetComponent<Renderer>().material = FlagMaterial;
                    }
                }
        }
    }

    // 하이라이트
    void OnHover(GameObject obj)
    {
        if (obj.CompareTag("FindMineBlock"))
        {
            obj.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    // 하이라이트 끄기
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


    // 메인 로직
    IEnumerator FindMineGame()
    {
        Initialize();   // 시작할때 초기화

        for (int i = 0; i < 3; ++i)
        {
            NotReduplicationRandom(true);   // 지뢰 생성
        }

        for (int i = 0; i < 5; ++i)
        {
            NotReduplicationRandom(false);  // 처음에 보여줄곳 생성
        }


        for (int i = 0; i < 5; ++i) // 처음에 보여줄곳 계산
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

    // 중복되지 않는 랜덤
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
        switch (MineArray[x, y])    // 현재 클릭한 위치가 지뢰인지 아닌지 검사
        {
            case 1:                 // 지뢰라면
                MineObject[x, y].GetComponent<Renderer>().material = MineMaterial;
                transform.GetChild(0).localPosition = MineObject[x, y].transform.localPosition;
                transform.GetChild(0).gameObject.SetActive(true);
                Invoke("DisableExplosion", 2.0f);
                // 게임오버 처리
                break;
            default:                // 아니라면 계산
                int mine = 0;
                for (int i = -1; i <= 1; ++i)   // 좌,우,위,아래,대각선 8방향 검사후 지뢰가 몇개 있는지 계산한다.
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


                switch (mine)   // 지뢰의 갯수에 따라 메테리얼을 바꿔준다
                {
                    case 0:
                        MineObject[x, y].GetComponent<Renderer>().material = AfterClickPlaneMaterial;
                        break;
                    case 1:
                        MineObject[x, y].GetComponent<Renderer>().material = OneMaterial;
                        break;
                    case 2:
                        MineObject[x, y].GetComponent<Renderer>().material = TwoMaterial;
                        break;
                    case 3:
                        MineObject[x, y].GetComponent<Renderer>().material = ThreeMaterial;
                        break;
                }
                break;
        }
        MineObject[x, y].tag = "Untagged";  // 이미 클릭한 곳은 다시 클릭 안되게 변경

        int numofCurrentBeforeBlock = 0;
        for(int i=0;i<transform.childCount; ++i)    // 현재까지 깐 곳을 계산해서 3개 이하면 지뢰만 남았으므로 클리어처리
        {
            if(transform.GetChild(i).CompareTag("FindMineBlock"))
            {
                numofCurrentBeforeBlock++;
            }
        }
        if (numofCurrentBeforeBlock <= 3)
            _isClearConditionCompleted = true;
        
    }

    void DisableExplosion()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}