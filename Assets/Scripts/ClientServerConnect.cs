using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using socket.io;
using Newtonsoft.Json;

public class Idpw
{
    public string uid;
    public string pw;
}

public class FromServer
{
    public string data;
}

public class ClientServerConnect : MonoBehaviour
{
    Socket socket;

    public string serverURL = "http://localhost:3000";
    public InputField uid;
    public InputField pw;
    public GameObject CsBtn;
    public GameObject InsertBtn;

    public GameObject readyExam;
    public GameObject addCount;

    private void Start()
    {
        readyExam.SetActive(false);
        if (socket == null)
        {
            socket = Socket.Connect(serverURL);
            socket.On("connect", () =>
            {
                Debug.Log("Connected");
            });

            socket.On("insertdata", (string data) =>
            {
                Debug.Log(data);

                // 스트링 데이터 클린업.
                data = CleanupString(data);

                InsertBtn.transform.Find("Text").GetComponent<Text>().text = "data";
            });

            // InnoServer에서 socket.emit('ReadyExam','Ready')가 작동하면 이곳이 작동한다.
            socket.On("ReadyExam", (string readyCnt) =>
            {
                Debug.Log("ReadyExam Show!");
                Text txt = addCount.transform.GetChild(0).GetComponent<Text>();
                txt.text = "ADDED COUNT: " + readyCnt;
                readyExam.SetActive(true);
            });

            // InnoServer에서 socket.emit('AddCnt',readyCnt)가 작동하면 이곳이 작동한다.
            socket.On("AddCnt", (string readyCnt) =>
            {
                Text txt = addCount.transform.GetChild(0).GetComponent<Text>();
                txt.text = "ADDED COUNT: " + readyCnt;
                readyExam.SetActive(false);
            });

        }
    }

    public void ConnectServer()
    {
        Debug.Log("로그인버튼 누름");
        if (string.IsNullOrEmpty(uid.text) ||
            string.IsNullOrEmpty(pw.text))
        {
            return;
        }

        // 접속 완료시 해야될 내용들을 명시한다.
        Idpw idpw = new Idpw();
        idpw.uid = uid.text;
        idpw.pw = pw.text;

        string jsonData = JsonUtility.ToJson(idpw);

        socket.EmitJson("login", jsonData, (string data) =>
        {
            uid.transform.parent.gameObject.SetActive(false);
            pw.transform.parent.gameObject.SetActive(false);
            Debug.Log("ackData: " + data);

            data = CleanupString(data);

            CsBtn.transform.Find("Text").GetComponent<Text>().text = data;
            CsBtn.GetComponent<Button>().enabled = false;
        });

    }

    public void InsertData()
    {
        socket.Emit("insertdata");
    }

    public void AddCount()
    {
        socket.Emit("ReadyExam");
    }

    // 데이터가 배열 형태로 들어오니 배열형을 제거하고 문자열 형태만 남겨놓는다.
    string CleanupString(string data)
    {
        string[] ridCha = new string[] { "[", "]", "\"","\'" };

        for (int i = 0; i < ridCha.Length; i++)
            data = data.Replace(ridCha[i], "");

        return data;
    }

}










/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using socket.io;
using Newtonsoft.Json;

public class Idpw
{
    public string uid;
    public string pw;
}

public class FromServer
{
    public string data;
}

public class ClientServerConnect : MonoBehaviour
{

    public string serverURL = "http://localhost:3000";
    public InputField uid;
    public InputField pw;
    public GameObject CsBtn;
    public GameObject InsertBtn;

    Socket socket;


    public void ConnectServer()
    {
        Debug.Log("버튼 누름");
        if (string.IsNullOrEmpty(uid.text) ||
            string.IsNullOrEmpty(pw.text))
        {
            return;
        }
        // 아직 클라이언트가 서버와 연결이 되어있지 않다면
        if (socket == null)
        {
            // 지정된 주소의 서버에 클라이언트(자신)을 연결준비한다.
            socket = Socket.Connect(serverURL);
            Idpw idpw = new Idpw();
            idpw.uid = uid.text;
            idpw.pw = pw.text;

            // 접속 완료시 해야될 내용들을 명시한다.
            socket.On("connect", () =>
            {
                string jsonData = JsonUtility.ToJson(idpw);

                //socket.Emit("login", jsonIdpw);
                //Debug.Log("Connected3");

                socket.EmitJson("login", jsonData, (string ackData)=> {
                    uid.transform.parent.gameObject.SetActive(false);
                    pw.transform.parent.gameObject.SetActive(false);
                    Debug.Log("ackData: " + ackData);

                    // 데이터가 배열 형태로 들어오니 배열형을 제거하고 문자열 형태만 남겨놓는다.
                    string[] ridCha = new string[] {"[","]","\"" };
                    for(int i = 0; i< ridCha.Length; i++)
                        ackData = ackData.Replace(ridCha[i],"");

                    CsBtn.transform.Find("Text").GetComponent<Text>().text = ackData;
                    CsBtn.GetComponent<Button>().enabled = false;
                });
            });

        }
    }

    public void InsertData()
    {
        socket.Emit("insertdata");
        socket.On("successupload", () =>
        {
            InsertBtn.transform.Find("Text").GetComponent<Text>().text = "Success Upload";
        });
    }

    private void Start()
    {
    }
}
 */
