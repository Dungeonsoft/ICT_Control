using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoleFix : MonoBehaviour
{

    ExtraWin srWin;
    public int roleNum;
    private void Awake()
    {
        srWin = transform.parent.parent.GetComponent<ExtraWin>();

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
            srWin.RunDel(roleNum);
            Debug.Log("셀렉트 윈 창 끔!:: " + roleNum);
            transform.parent.parent.gameObject.SetActive(false);
        });

        //Debug.Log("트리거 생성");
        trigger.triggers.Add(entry);
    }

}
