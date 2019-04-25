using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ClickRoleBtn : MonoBehaviour {

    GameObject controlSystem;

    // 롤 선택창을 지정하기 위해 생성된 변수.//
    GameObject selectRole;
    Text btnText;
	void OnEnable () {

        // SelectRole를 지정한 것에서 가지고 온다.//
        controlSystem = GameObject.Find("ControlSystem");
        // 여기서 롤 선택창을 찾아서 지정하도록 한다.//
        // 롤 선택창은 컨트롤시스템 오브젝트의 컴포넌트인 UI_WindCollect에// 
        // 이미 지정되어 있다.//
        selectRole = controlSystem.GetComponent<UI_WinCollect>().selectRole;


        // 텍스트를 지정한다.//
        int chCount = transform.childCount;
        for (int i = 0; i < chCount; i++)
        {
            btnText = transform.GetChild(i).GetComponent<Text>();
            if (btnText != null)
            {
                string cName = btnText.name;
                if (cName.ToLower().Contains("text")) break;
            }
        }

        // 이벤트 트리거를 세팅한다.//
        // (이벤트 트리거를 컴포넌트로 붙인다.)//
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger trigger = GetComponent<EventTrigger>();


        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;

        // Debug.Log(name + "트리거 내용 지정");
        // 여기서 지정된 것은 롤(직업)을 선택하는 창을 호출하는 함수이다.//
        entry.callback.AddListener((eventData) =>
        {
            // 롤 선택 창 호출하는 함수 실행.//
            ShowRole();
        });

        //Debug.Log("트리거 생성");
        trigger.triggers.Add(entry);
    }

    void ShowRole()
    {
        Debug.Log("ShowRole 메소드 실행");
        Debug.Log("ChangeRole을 델리게이트에 등록");
        // 롤 선택창을 켜준다.(화면의 맨 앞에서 보여준다.)//
        selectRole.SetActive(true);
        selectRole.GetComponent<ExtraWin>().NextMethod(ChangeRole);
    }

    void ChangeRole(int rNum)
    {
        Debug.Log("체인지 롤을 실행");
        Transform pa = transform.parent;

        pa.GetComponent<UserInfoBar>().uData.role = rNum.ToString();
        pa.GetComponent<UserInfoBar>().SetUserData();
    }

}
