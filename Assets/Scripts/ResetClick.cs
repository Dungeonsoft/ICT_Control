 using UnityEngine;
using UnityEngine.EventSystems;

public class ResetClick : MonoBehaviour {

    GameObject controlSystem;
    GameObject checkWin;
    UI_WinCollect wCol;

    public string checkWinTitle;
    public string checkWinText;


    void Awake () {
        controlSystem = GameObject.Find("ControlSystem");
        wCol = controlSystem.GetComponent<UI_WinCollect>();
        checkWin = wCol.checkWin;

        // 이벤트 트리거를 세팅한다.//
        // (이벤트 트리거를 컴포넌트로 붙인다.)//
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger trigger = GetComponent<EventTrigger>();


        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;

        //Debug.Log(name + "트리거 내용 지정");
        entry.callback.AddListener((eventData) =>
        {
            // 트리거 내용을 서술한다//
            // 함수를 지정한다//
            Debug.Log("리셋 트리거 지정"+ eventData);
            checkWin.SetActive(true);
            wCol.checkWinTitle.text = checkWinTitle;
            wCol.checkWinText.text = checkWinText;
            checkWin.GetComponent<ExtraWin>().NextMethod(ResetUI);
        });
        //Debug.Log("트리거 생성");


        trigger.triggers.Add(entry);
    }

    // 창 실행후 컨펌을 누르면 실행되어야 할 메소드 설정//
    /// <summary>
    /// 인자 a를 불린 대신 사용하여 0은 false, 1은 true로 정의한다//
    /// </summary>
    /// <param name="a"></param>
    public void ResetUI(int a)
    {
        Debug.Log("ResetUI");
        if (a == 1)
        {
            for (int i = 0; i < wCol.resetBtnList.Length; i++)
                wCol.resetBtnList[i].GetComponent<BtnColorChange>().ActivationButton();
            wCol.timeText.text = "Morning";

            controlSystem.GetComponent<LobbyServerConnect>().ResetUserList();
        }
        checkWin.SetActive(false);
    }
	
}
