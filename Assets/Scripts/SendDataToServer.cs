using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 이 스크립트(컴포넌트)는 시나리오 선택부분의 각 시나리오 버튼에 하위 컴포넌트로 붙어있다.
/// </summary>
public class SendDataToServer : MonoBehaviour
{

    [Header("아래 변수를 체크하면 버튼을 눌렀을때 기능이 작동한다")]
    public bool isUse = true;

    public int sceneNum;
    GameObject controlSystem;
    LobbyServerConnect lsConnect;



    private void OnEnable()
    {
        this.GetComponent<UnityEngine.UI.Image>().raycastTarget = isUse;

        //Debug.Log(name + " ::: OnEnable");

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
            if (isUse == false)
            {
                Debug.Log("Is False");
                return;
            }
            else
            {
                    // 트리거 내용을 서술한다//
                    Debug.Log("버튼을 눌렀을 때 시나리오가 선택된다.");
                lsConnect.ChangeScenario(sceneNum);
            }
        });
        //Debug.Log("트리거 생성");
        trigger.triggers.Add(entry);
    }
}

