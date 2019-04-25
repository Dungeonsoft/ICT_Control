using Newtonsoft.Json;
using socket.io;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LobbyServerConnect : MonoBehaviour
{
    Socket socket;

    public string idGotten;
    public string dateFromServer;

    public string serverURL = "http://localhost";

    public GameObject traineeTabPrefab;

    public GameObject[] traineeTab;
    public GameObject[] traineeTabDeact;

    public int[] liveRoleExtinguish;
    //public List<int> deactiveRole;
    //public List<int> deleteRole;
    public Dictionary<int, string> comRole = new Dictionary<int, string>();

    public int loadSceneNum = 1;

    public GameObject selectRoleWin;

    private void Start()
    {
        Debug.Log("길이: " + traineeTab.Length);

        //csv 파일에 있는 롤 정보를 로드한다.
        LoadOriginalData();

        //// 초기 활성될 캐릭터 4개를 세팅하는 부분.
        //for (int i = 0; i < traineeTab.Length; i++)
        //{
        //    //GameObject.Find("DebugWin").GetComponent<Text>().text += "반복: " + i+ "\br";
        //    Debug.Log("반복: " + i);
        //    UserInfoBar uibar = traineeTab[i].GetComponent<UserInfoBar>();
        //    uibar.isUsing = false;
        //    uibar.uData.role = liveRoleExtinguish[i].ToString();
        //    uibar.uData.name = "Trainee" + i.ToString("00");
        //    uibar.uData.status = "None";
        //    uibar.uData.result = "Pass";
        //    uibar.SetUserData(false);
        //}

        if (socket == null)
        {
            // 소켓은 주어진 주소를 기반으로 연결한다.//
            // Socket.Connect(serverURL) //
            // 위코드는 지정된 주소의 서버에 접속하라는 코드이다.//
            // 이렇게 코드를 작성해서 연결을 하면//
            // 실제 서버에서는//
            // socketio.on('connection', (socket)//
            // 위와 같이 되어있는 부분을 찾아가서 연결이 된다.//
            socket = Socket.Connect(serverURL);

            // 연결되면 이곳이 실행이 된다.//
            socket.On("connect", () =>
            {
                Debug.Log("-커넥트 되었다.-");
                Debug.Log("-처리할 내용을 이곳에 구현한다-");
                Debug.Log("Connected");
            });

            //여기서 지정된 서버에서 지정된 아이디.//
            socket.On("GetIdMade", (string idAndDate) =>
            {
                Debug.Log("Id Gotten Data:::: " + idAndDate);
                IdDate idDate = JsonConvert.DeserializeObject<IdDate>(idAndDate);
                idGotten = idDate.id;
                dateFromServer = idDate.date;

                Debug.Log("ID Gotten using to(id): " + idGotten);
                ConnectServer();
            });

            socket.On("loggedUser", (string data) =>
            {
                Debug.Log("-유저가 로그인을 하면 유저정보를 이곳으로 보낸다.-");
                Debug.Log(data);

            });

            // InnoServer에서 socket.emit('ReadyExam','Ready')가 작동하면 이곳이 작동한다.
            socket.On("ReadyExam", (string readyCnt) =>
            {
                Debug.Log("ReadyExam Show!");
            });

            // InnoServer에서 socket.emit('AddCnt',readyCnt)가 작동하면 이곳이 작동한다.
            socket.On("AddCnt", (string readyCnt) =>
            {
            });
        }
        // 서버에 클라이언트가 접속에 성공하면 정보를 받아온다.
        socket.On("createTraineeTab", (string traineeInfo) =>
        {
            //GameObject.Find("DebugWin").GetComponent<Text>().text += "트레이너 정보를 입력한다::: " + traineeInfo+ "\br";

            Debug.Log("트레이너 정보를 입력한다::: " + traineeInfo);

            #region 안쓰는 코드.
            //Transform setTransform = GameObject.Find("ScrollBase").transform;
            //int beforeCnt = setTransform.childCount;
            //GameObject GO = Instantiate(traineeTabPrefab) as GameObject;
            //GO.transform.parent = setTransform;

            // 유저 탭이 하나씩 추가될때 마다 이것이 작동하여 차례대로 정렬되게 보여준다.
            // 유저 바의 높이를 받아온다.
            //float h1 = GO.GetComponent<UserInfoBar>().getHeight;
            // 탭의 Y 위치를 계산한다.(로컬좌표기준)
            //float h2 = (-1.0f * beforeCnt * (h1 + 10));
            //GO.GetComponent<RectTransform>().localPosition = new Vector3(0, h2, 0);
            #endregion 

            //GameObject GO = new GameObject();
            for (int i = 0; i < traineeTab.Length; i++)
            {

                if (traineeTab[i].GetComponent<UserInfoBar>().isUsing == false)
                {
                    //GameObject.Find("DebugWin").GetComponent<Text>().text += "유저탭에 데이터를 넣는다\br";

                    UserInfoBar uiBar = traineeTab[i].GetComponent<UserInfoBar>();
                    //유저탭에 데이터를 넣는다//
                    GetUserBaseData guData = JsonConvert.DeserializeObject<GetUserBaseData>(traineeInfo);
                    uiBar.uData = guData;
                    uiBar.SetUserData();
                    uiBar.isUsing = true;
                    break;
                }
            }

            #region 안쓰는 코드.
            //유저탭에 데이터를 넣는다//
            //GetUserBaseData guData = JsonConvert.DeserializeObject<GetUserBaseData>(traineeInfo);
            //GO.GetComponent<UserInfoBar>().uData = guData;
            //GO.GetComponent<UserInfoBar>().SetUserData();
            #endregion
        });

        //서버에서 유저 완료 정보를 가지고 와서 유저 탭의 정보를 변경한다.
        socket.On("alarmclear", (string userData) =>
         {
             Debug.Log("유저탭에 종료정보를 넣어준다.");
             for (int i = 0; i < traineeTab.Length; i++)
             {
                 if (traineeTab[i].GetComponent<UserInfoBar>().isUsing == true)
                 {
                     SendEnd guData = JsonConvert.DeserializeObject<SendEnd>(userData);
                     UserInfoBar uiBar = traineeTab[i].GetComponent<UserInfoBar>();

                    //유저 아이디가 같은 탭을 찾아서 내용을 컴플리트(완료)로 변경하여 준다.
                    if (guData.uid == uiBar.uData.uid)
                     {
                         uiBar.userResult.transform.GetChild(0).GetComponent<Text>().text = "Pass";
                         uiBar.userStatus.transform.GetChild(0).GetComponent<Text>().text = "Finished";
                         break;
                     }
                 }
             }
         });
    }

    public GameObject severConnectingWin;
    public GameObject serverConnectedText;
    public Text severConnectingWinText;
    //public DataArrange dArrange;
    public Text dateText;

    public void ConnectServer()
    {
        Debug.Log("로그인 준비 시작");
        Debug.Log("ID Gotten using to(id)2: " + idGotten);

        // 여기서 처음 서버와 에 통제툴이 로그인하도록 만든다.//
        // 처음에 보낼 데이터는 없다.//

        // "loginControl", idGotten 여기서 첫번째는 함수 이름//
        // 정확히는 람다함수 loginControl(함수이름)을 호출한다//
        // idGotten은 보내는 서버에서 생성되어 받았던 ID(문자)이다.//
        // 뒤의 (string data) 이부분은 서버쪽에서 데이터가 처리되고//
        // 다시 이부분이 자동호출되는데 그때 들어오는 데이터의 형식이다.//
        // 데이터의 형식은 모두 string을 사용한다.//
        // on과 emit를 앞부분에 붙여 사용하는데 emit는 최초 보내는쪽//
        // on은 밭는쪽//
        // string data를 바로 보낼때는 Emit을 쓰지만//
        // string 변수로 만들어서 보낼때는  EmitJason을 사용한다.//
        // 위의 두줄은 확실하지 않음//
        // 됐다 안되었다 함//
        // 그때 그때 맞는 쪽으로 테스트//
        socket.Emit("loginControl", idGotten, (string data) =>
        {
            data = CleanupString(data);
            Debug.Log("통제툴 이제 5초 있다가 시작됨: " + data);

                        //처음에는 다 막고 있다가 화면을 보여줌
                        StartCoroutine(StartControlCenter());
        });
        Debug.Log("여기옴");
    }

    // 추가할 데이터를 서버에 보낸다//
    public void InsertData(string data)
    {
        socket.Emit("insertdata", data);
    }

    // 데이터가 배열 형태로 들어오니 배열형을 제거하고 문자열 형태만 남겨놓는다.
    string CleanupString(string data)
    {
        string[] ridCha = new string[] { "[", "]", "\"", "\'" };

        for (int i = 0; i < ridCha.Length; i++)
            data = data.Replace(ridCha[i], "");

        return data;
    }



    IEnumerator StartControlCenter()
    {
        for (int i = 0; i < 3; i++)
        {
            severConnectingWinText.text += ".";
            yield return new WaitForSeconds(1f);
        }
        severConnectingWin.SetActive(false);
        serverConnectedText.SetActive(true);

        // 서버에서 넘어온 시간 데이터를 재정리함//
        int idx = dateFromServer.IndexOf("T");
        dateFromServer = dateFromServer.Substring(0, idx);

        dateText.text = dateFromServer.ToString();
        //StartCoroutine(dArrange.ChildrenArrangeCo());
    }

    /// <summary>
    /// Stop 버튼을 누르면 작동하는 메소드 ///
    /// </summary>
    public void StopClients()
    {
        Debug.Log("통제툴에서 클라이언트를 중지 시킨다.");
        // 서버의 stopClients를 작동시킨다//
        socket.Emit("stopClients");
    }

    /// <summary>
    /// Start 버트을 누르면 작도하는 메소드///
    /// </summary>
    public void StartClients()
    {
        Transform setTransform = GameObject.Find("ScrollBase").transform;
        int arrayCnt = setTransform.childCount;
        Debug.Log("arrayCnt :: " + arrayCnt);

        string roleData = "";
        for (int i = 0; i < arrayCnt; i++)
        {
            UserInfoBar uibar = setTransform.GetChild(i).GetComponent<UserInfoBar>();
            if (uibar.isUsing == true)
            {
                string rval = uibar.uData.role.ToString();
                Debug.Log("r val :: " + rval);
                roleData += rval;
                if (i + 1 < arrayCnt) roleData += ",";
            }
        }

        Debug.Log("roleData :: " + roleData);
        // 서버의 startClents를 작동시킨다//
        socket.Emit("startClents", roleData);
    }

    // SendDataToServer 에서 호출된다.
    // 호출 후 서버의 changeScenario 를 호출한다.
    // 거기서는 다시 각각의 클라이언트에 시나리오 넘버를 보낸다.
    // 클라이언트에서는 실행될 시나리오 넘버를 변경한다.
    public void ChangeScenario(int sNum)
    {
        Debug.Log("change scene ::: " + sNum);
        loadSceneNum = sNum;
        comRole = new Dictionary<int, string>();
        SetLiveRole(loadSceneNum);

        socket.Emit("changeScenario", sNum.ToString());

    }
    public void ChangeMode(int mNum)
    {
        Debug.Log("change mode ::: " + mNum);
        socket.Emit("changeMode", mNum.ToString());
    }

    public void SendChangedUserData(GetUserBaseData uData)
    {
        Debug.Log("서버에 유저 롤 변경 데이터를 보낼 준비를 한다.");
        // 데이터를 json으로 변환하고.
        string uDataJson = JsonConvert.SerializeObject(uData);
        Debug.Log(uDataJson);
        // 서버로 보낸다.
        socket.EmitJson("sendChangedUserData", @uDataJson);
    }
    
    public void SendChangeWeather(string wString)
    {
        GetWeatherData wData = new GetWeatherData();
        wData.weatherIs = wString;
        Debug.Log("서버에 날씨 변경 데이터를 보낸다.");
        string wDataJson = JsonConvert.SerializeObject(wData);
        Debug.Log(wDataJson);
        socket.EmitJson("sendChangeWeather", wDataJson);
        // 이 곳에서 서버로 날씨를 변경하는 내용을 정리하여 보낸다.
        // 데이터는 Json으로 변경한다.
    }


    // 이부분은 아직 정립이 안되었다.
    // 튜토리얼 또는 시험모드를 선택했을때 그 정보를 서버에 보내기 위해.
    // 만들어 놓은 함수이다.
    /// <summary>
    /// mode는 서버로 보낼 모드 상태 정보이다.
    /// </summary>
    /// <param name="mode"></param>
    public void SendChangedMode(string mode)
    {
        Debug.Log("Selected Mode: " + mode);
        socket.Emit("sendChangedMode", mode);
    }

    // 여기에서 접속한 유저를 해제하는 기능을 한다.
    public void ResetUserList()
    {
        Debug.Log("Rest UserList");

        // 서버에 먼저 데이터를 초기화하라는 신호를 보낸다.
        socket.Emit("resetUserList");

        // 사용되고 있는 탭들을 모두 초기화하여 준다.
        for (int i = 0; i < traineeTab.Length; i++)
        {
            UserInfoBar uibar = traineeTab[i].GetComponent<UserInfoBar>();
            uibar.isUsing = false;
            uibar.uData.role = liveRoleExtinguish[i].ToString();
            // uibar.uData.name = "Trainee" + i.ToString("00");
            uibar.uData.name = "Trainee" + "00";
            uibar.uData.status = "None";
            uibar.uData.result = "None";
            uibar.SetUserData(false);
        }

        // 유저 리스트를 초기화하기전 그 정보를 담고 활용하고 있던 딕셔너리변수를 초기화한다.
        comRole = new Dictionary<int, string>();
        // 리셋을 하면 시나리오 1번으로 초기화 한다.(소화-Extinguishment)
        // 유저 리스트도 역시 초기화한다.
        SetLiveRole(1);

        socket.Emit("stopClients");
    }


    #region 로컬에 들어있는 롤역할을 정의한 csv파일 불러와서 사용가능하도록 가공한다.
    // 로컬에 들어있는 롤역할을 정의한 파일 불러와서 사용가능하도록 가공한다.

    // 로컬의 csv 파일을 불러올 수 있는 그릇의 역할을 하는 리스트 변수를 선언한다.
    public List<Dictionary<string, object>> sceneRoleCsvData;
    public string activeRoleInfoPath;
    public void LoadOriginalData()
    {
        Debug.Log("CSV 파일 로드");
        sceneRoleCsvData = CSVReader.Read(activeRoleInfoPath);

        //Debug.Log("Loaded Data: "+sceneRoleCsvData);
        //Debug.Log("Data Length: "+ sceneRoleCsvData.Count);
        //Debug.Log("Data Scene01 [0] ::  " + sceneRoleCsvData[0]["Scene01"].ToString());

        // 시나리오별 액티브 정보를 정리하여 리스트에 입력한다.
        SetLiveRole(loadSceneNum);
    }

    public void SetLiveRole(int scNum)
    {
        string sName = "Scene" + scNum.ToString("00");

        // liveRoleExtinguish number
        int lreNum = 0;
        int roleCnt = sceneRoleCsvData.Count;
        for (int i = 0; i < roleCnt; i++)
        {
            string roleStatus = sceneRoleCsvData[i][sName].ToString();
            if (roleStatus == "active")
            {
                // liveRoleExtinguish 배열변수에 정보를 입력한다.
                // 롤은 1부터 시작.
                liveRoleExtinguish[lreNum] = i + 1;
                lreNum++;
            }
            //else if (roleStatus == "delete")
            //{
            //    // 롤탭을 하이드 한다.
            //    deleteRole.Add(i + 1);
            //}
            //else if (roleStatus == "deactive")
            //{
            //    // 디액티브인 롤탭정보를 갱신한다.
            //    deactiveRole.Add(i + 1);
            //}
            else
            {
                //Debug.Log("roleStatus::: "+ (i+1)+"   :::   "+roleStatus);
                comRole.Add(i + 1, roleStatus);
            }
        }

        // 먼저 액티브롤세팅을 먼저한다.
        SetActiveRole();
        // 안나오는 롤과 디액티브롤을 세팅한다. 
        SetComRole();

        //SelectRole 윈도우 세팅을 변경한다.
        ChangeSelRoleWin();

    }

    /// <summary>
    /// 액티브 롤 세팅
    /// </summary>
    public void SetActiveRole()
    {
        // 초기 활성될 캐릭터 4개를 세팅하는 부분.
        for (int i = 0; i < traineeTab.Length; i++)
        {
            //GameObject.Find("DebugWin").GetComponent<Text>().text += "반복: " + i+ "\br";
            Debug.Log("반복: " + i);
            UserInfoBar uibar = traineeTab[i].GetComponent<UserInfoBar>();
            uibar.isUsing = false;
            uibar.uData.role = liveRoleExtinguish[i].ToString();
            uibar.uData.name = "Trainee" + "00";
            uibar.uData.status = "None";
            uibar.uData.result = "Pass";
            uibar.SetUserData(false);
        }
    }

    public void SetComRole()
    {
        int rNum = 1;
        for (int i = 0; i < traineeTabDeact.Length; i++)
        {
            //Debug.Log("시작 R값::: "+ rNum);
            UserInfoBar npcBar = traineeTabDeact[i].GetComponent<UserInfoBar>();

            while (comRole.ContainsKey(rNum) == false)
            {
                rNum++;
            }

            //Debug.Log("Role["+ rNum + "] ::: "+ comRole[rNum]);
            npcBar.uData.role = rNum.ToString();
            if (comRole[rNum] == "deactive")
            {
                traineeTabDeact[i].SetActive(true);
                npcBar.SetComRole();
            }
            else if (comRole[rNum] == "delete")
            {
                traineeTabDeact[i].SetActive(false);
            }
            //다음것을 검사할 수 있도록 값을 1증가시킨다.
            rNum++;
        }
    }

    public void ChangeSelRoleWin()
    {
        Transform roles = selectRoleWin.transform.Find("Roles");
        Debug.Log("roles Name ::: " + roles.name);

        int lreNum = 0;
        for (int i = 0; i < roles.childCount; i++)
        {
            Debug.Log("roles.childCount Value ::" + roles.childCount);
            Debug.Log("i Value ::" + i);
            Debug.Log("lreNum Value ::" + lreNum);
            if (i == liveRoleExtinguish[lreNum] - 1)
            {
                Image img = roles.GetChild(i).GetComponent<Image>();
                img.raycastTarget = true;
                img.color = new Color(1f, 1f, 1f, 1f);
                img.GetComponent<Button>().enabled = true;
                img.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.1f);
                img.transform.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.5f);

                if (lreNum < liveRoleExtinguish.Length - 1)
                {
                    lreNum++;
                }
            }
            else
            {
                Debug.Log("roles.GetChild(" + i + ")" + roles.GetChild(i).name);
                Image img = roles.GetChild(i).GetComponent<Image>();
                img.raycastTarget = false;
                img.color = new Color(1f, 1f, 1f, 0.02f);
                img.GetComponent<Button>().enabled = false;
                img.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.1f);
                img.transform.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.02f);
            }
        }
    }
    #endregion
}

public class GetWeatherData
{
    public string weatherIs;
}


// 이클래스를 기반으로 서버에서 기본 데이터를 받아온다.
[System.Serializable]
public class GetUserBaseData
{
    public string uid;
    public string name;
    public string role;
    public string status;
    public string result;
    public string _id;
}

//최초 아이디와 날짜를 받아올 때 사용하기 위해 만들어 놓은 클래스.
public class IdDate
{
    public string id;
    public string date;
}

public class SendEnd
{
    //여기서는 액션 클리어 트루 폴스로 값을 넣어준다.
    public string roleNum;
    public string uid;
    public string actionClear;
}


// 제이슨으로 데이터를 클래스로 변환해서 받을때 참고 //
// GetUserBaseData guData = JsonConvert.DeserializeObject<GetUserBaseData>(traineeInfo);


/*
  {
      "uid":"noHtw06-idRUK1igAAAJ",
      "name":"trainee4",
      "role":"4",
      "status":"Ready",
      "result":"Pass",
      "_id":"5b0d3602d28a3b637886badc"
  }
 */
 