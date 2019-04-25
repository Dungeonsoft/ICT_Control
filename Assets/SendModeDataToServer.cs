using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SendModeDataToServer : MonoBehaviour
{

    public int modeNum;
    GameObject controlSystem;
    LobbyServerConnect lsConnect;
     

    private void OnEnable()
    {
        controlSystem = GameObject.Find("ControlSystem");
        lsConnect = controlSystem.GetComponent<LobbyServerConnect>();

        // 이벤트 트리거를 세팅한다.//
        // (이벤트 트리거를 컴포넌트로 붙인다.)//
        if (GetComponent<EventTrigger>() == null)
        {
            gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;

        //Debug.Log(name + "트리거 내용 지정");
        entry.callback.AddListener((eventData) =>
        {
                // 트리거 내용을 서술한다//
                Debug.Log("버튼을 눌렀을 때 시나리오가 선택된다.");
                lsConnect.ChangeMode(modeNum);
        });
        //Debug.Log("트리거 생성");
        trigger.triggers.Add(entry);

    }
}