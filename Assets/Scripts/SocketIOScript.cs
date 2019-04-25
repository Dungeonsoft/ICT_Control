using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Quobject.SocketIoClientDotNet.Client;
using socket.io;
using Newtonsoft.Json;

public class ChatData {
	public string id;
	public string msg;
};

public class SocketIOScript : MonoBehaviour
{
    public string serverURL = "http://localhost:3000";

    public InputField uiInput = null;
    public Button uiSend = null;
    public Text uiChatLog = null;

    protected Socket socket = null;
    protected List<string> chatLog = new List<string>();

    void Destroy()
    {
        DoClose();
    }

    // Use this for initialization
    void Start()
    {
        DoOpen();

        uiSend.onClick.AddListener(() =>
        {
            SendChat();
        });
    }

    // Update is called once per frame
    void Update()
    {
        lock (chatLog)
        {
            if (chatLog.Count > 0)
            {
                string str = uiChatLog.text;
                foreach (var s in chatLog)
                {
                    str = str + "\n" + s;
                }
                uiChatLog.text = str;
                chatLog.Clear();
            }
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            SendChat();
        }
    }

    void SendChat()
    {
        if (string.IsNullOrEmpty(uiInput.text)) return;
        SendChat(uiInput.text);
        uiInput.text = "";
        uiInput.ActivateInputField();
    }

    void DoOpen()
    {
        if (socket == null)
        {
            //socket = IO.Socket (serverURL);
            socket = Socket.Connect(serverURL);

            //socket.On(Socket.EVENT_CONNECT, () => {
            socket.On(SystemEvents.connect, () =>
            {

                lock (chatLog)
                {
                    // Access to Unity UI is not allowed in a background thread, so let's put into a shared variable
                    chatLog.Add("Socket.IO connected.");
                }
            });
            socket.On("chat", (string data) =>
            {
                string str = data.ToString();

                ChatData chat = JsonConvert.DeserializeObject<ChatData>(str);
                string strChatLog = "user#" + chat.id + ": " + chat.msg;

                // Access to Unity UI is not allowed in a background thread, so let's put into a shared variable
                lock (chatLog)
                {
                    chatLog.Add(strChatLog);
                }
            });
        }
    }

    void DoClose()
    {
        if (socket != null)
        {
            socket.On(SystemEvents.disconnect, () =>
            {
                Debug.Log("¾È³ç~ Àß°¡!");
            });
            socket = null;
        }
    }

    void SendChat(string str)
    {
        if (socket != null)
        {
            socket.Emit("chat", str);
        }
    }
}
