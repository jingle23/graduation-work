using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainGameFollowPathManager : MainRoomPropertyController
{
	//방향
	enum Direction
	{
		Left,
		Right,
		Up,
		Down
	}
	
	List<Direction> ShuffleList = new List<Direction>();    //방향 리스트
	List<Vector2> PlayerList = new List<Vector2>();         //방향에 따른 좌표 리스트
	
	GameObject[,] BlockObject = new GameObject[4, 4];       //4x4 블록
	
	public GameObject BlockPrefab;
	GameObject Effect;

	public int PathNum = 0;
	int CurrentPlayerIndex = 0;
	bool isGameStarted = false;
	void Awake()
	{
		Effect = transform.GetChild(0).gameObject;          //맞았을때 O표 이펙트
		
		// 처음만 블록 생성 및 초기화
		for (int i = 0; i < 4; ++i)
			for (int j = 0; j < 4; ++j)
		{
			BlockObject[i, j] = Instantiate(BlockPrefab) as GameObject;
			BlockObject[i, j].transform.parent = gameObject.transform;
			BlockObject[i, j].transform.name = i.ToString() + "," + j.ToString();
			BlockObject[i, j].transform.localPosition = new Vector3(-1.65f + i * 1.1f, -3.7f, -1.65f + j * 1.1f);

			//BlockObject[i, j].transform.localPosition = new Vector3(-2.5f + i * 1.5f, -3.7f, -2.5f + j * 1.5f);
		}
		
		//EnterRoom();
	}
	
	public override void EnterRoom()
	{
		Debug.Log("메인게임 길찾기 방에 들어왔습니다.");
		Initialize();
		StartCoroutine("FindPathGame");
	}
	
	public override void Update()
	{
		base.Update();
	}
	
	public override void LeaveRoom()
	{
		Debug.Log("메인게임 길찾기 방에서 나갔습니다.");
	}
	
	public override void Clear()
	{
		StopCoroutine("FindPathGame");

		StartCoroutine("SlowClear");

		Debug.Log("메인게임 길찾기 클리어 했습니다.");
	}
	
	// 초기화
	void Initialize()
	{
		CurrentPlayerIndex = 0;
		
		for (int i = 0; i < 4; ++i)
			for (int j = 0; j < 4; ++j)
		{
			// 색깔 설정
			BlockObject[i, j].GetComponent<Renderer>().material.color = Color.white;
			BlockObject[i, j].tag = "FindMineBlock";
			BlockObject[i, j].SetActive(true);
		}
		isGameStarted = false;
		ShuffleList.Clear();
		PlayerList.Clear();
		
	}
	
	// 판정
	public void Judge(GameObject obj)
	{
		// 플레이어가 밟은곳을 검사해서 현재 플레이어가 밟아야 할 순서와 맞지 않는다면 게임 오버 처리, 제대로 밟았다면 이펙트 출력 및 진행
		if (isGameStarted)
		{
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
			{
				if (BlockObject[i, j].name == obj.name)
				{
					// 위치비교
					if (i == (int)PlayerList[CurrentPlayerIndex].x && j == (int)PlayerList[CurrentPlayerIndex].y)
					{
						Effect.transform.position = BlockObject[i, j].transform.position;
						StartCoroutine("CollisionJudgeRoutine");	// 맞은 표시 이펙트 호출

						PathNum = PathNum + 1;
						
						if( PathNum == 10 )
							_isClearConditionCompleted = true;

						return;
					}


				}
			}
			StartCoroutine("GameOver"); 
		}
	}

	// 서서히 없어지게 보이도록
	IEnumerator SlowClear()
	{
		
		yield return new WaitForSeconds (2.0f);
		
		float fAlpha = 1;
		
		// 1. 회전하면서 야바위가 하얗게 없어지는 것처럼 보이기 위해 알파값을 점점 줄여준다. 
		while (fAlpha >= 0) {
			fAlpha -= 0.01f;
			
			for (int i = 0; i < 4; ++i) {
				for (int j = 0; j < 4; ++j) {
					BlockObject [i, j].GetComponent<Renderer> ().material.SetColor ("_Color", new Color (1, 1, 1, fAlpha));

				}
			}
			yield return null;
		}
		
		for (int i = 0; i < 4; ++i) {
			for (int j = 0; j < 4; ++j) {
				BlockObject [i, j].SetActive (false);
				
			}
		}

	}

	IEnumerator GameOver()  //게임오버의 예
	{
		float time = 0;
		while (time < 3)
		{
			HPManager.instance.GetComponent<Collider>().enabled = false;
			HPManager.instance.gameObject.transform.Translate(0, -1 * Time.deltaTime, 0);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
	
	IEnumerator CollisionJudgeRoutine() // 이펙트 켜고 인덱스 ++
	{
		
		Effect.SetActive(true);
		CurrentPlayerIndex++;
		yield return new WaitForSeconds(0.5f);
		Effect.SetActive(false);
	}
	
	IEnumerator FindPathGame()
	{
		// 세로만 생각하면, 네가지 경우가 있음 Up3번은 무조건 있어야하고, 나머지는 DownUp 셋트가 몇번이있느냐 ( 마지막 블록에 도착하기 위해서는 ) 
		// 0번일경우 Up 3번에 X축 이동 6번을 랜덤돌리면 됨
		// 1번일경우 Up 3번 + Down Up + X축 4번 랜덤
		// 2번일경우 Up 3번 + Down Up 2세트 + X축 2번 랜덤
		// 3번일경우 Up 3번 + Down Up 3세트 랜덤
		for (int i = 0; i < 3;++i)
			ShuffleList.Add(Direction.Up);
		switch (Random.Range(0, 4)) 
		{
		case 0:
			for (int i = 0; i < 6; ++i)
			{
				if (Random.Range(0, 2) == 1)
					ShuffleList.Add(Direction.Right);
				else
					ShuffleList.Add(Direction.Left);
			}
			break;
		case 1:
			ShuffleList.Add(Direction.Up);
			ShuffleList.Add(Direction.Down);
			for (int i = 0; i < 4; ++i)
			{
				if (Random.Range(0, 2) == 1)
					ShuffleList.Add(Direction.Right);
				else
					ShuffleList.Add(Direction.Left);
			}
			break;
		case 2:
			for (int i = 0; i < 2; ++i)
			{
				ShuffleList.Add(Direction.Up);
				ShuffleList.Add(Direction.Down);
			}
			for (int i = 0; i < 2; ++i)
			{
				if (Random.Range(0, 2) == 1)
					ShuffleList.Add(Direction.Right);
				else
					ShuffleList.Add(Direction.Left);
			}
			break;
		case 3:
			for (int i = 0; i < 3; ++i)
			{
				ShuffleList.Add(Direction.Up);
				ShuffleList.Add(Direction.Down);
			}
			break;
		}
		
		System.Random rnd = new System.Random();    //리스트 랜덤으로 섞어줌
		ShuffleList = ShuffleList.OrderBy(x => rnd.Next()).ToList();
		
		int randX = Random.Range(0, 4); //시작지점
		int randY = 0;
		
		BlockObject[randX, randY].GetComponent<Renderer>().material.color = Color.red;
		yield return new WaitForSeconds(1);
		BlockObject[randX, randY].GetComponent<Renderer>().material.color = Color.cyan;
		
		PlayerList.Add(new Vector2(randX, randY));
		
		for(int i=0;i<ShuffleList.Count;++i){
			switch (ShuffleList[i])
			{
			case Direction.Down:
				if(randY == 0)
				{
					for (int j = i; j < ShuffleList.Count; ++j)
					{
						if(ShuffleList[j] == Direction.Up)
						{
							ShuffleList[i] = Direction.Up;
							ShuffleList[j] = Direction.Down;
							break;
						}
					}
					randY++;
				}
				else
					randY--;
				break;
			case Direction.Up:
				if (randY == 3)
				{
					for (int j = i; j < ShuffleList.Count; ++j)
					{
						if (ShuffleList[j] == Direction.Down)
						{
							ShuffleList[i] = Direction.Down;
							ShuffleList[j] = Direction.Up;
							break;
						}
					}
					randY--;
				}
				else
					randY++;
				break;
			case Direction.Right:
				if (randX == 3)
				{
					ShuffleList[i] = Direction.Left;
					randX--;
				}
				else
					randX++;
				break;
			case Direction.Left:
				if (randX == 0)
				{
					ShuffleList[i] = Direction.Right;
					randX++;
				}
				else
					randX--;
				break;
			}
			BlockObject[randX, randY].GetComponent<Renderer>().material.color = Color.red;
			yield return new WaitForSeconds(1);
			BlockObject[randX, randY].GetComponent<Renderer>().material.color = Color.cyan;
			PlayerList.Add(new Vector2(randX, randY));
		}
		isGameStarted = true;
		// 여기서부터 조작 시작
		
		
		
	}
}
