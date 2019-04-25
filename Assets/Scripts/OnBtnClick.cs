using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnBtnClick : MonoBehaviour
{
    Image btnLine;
    Text btnText;
    Image btnIcon;

    [Header("Element0: False ::: Element1: True")]

    Color[] lineColor = new Color[2];
    Color[] textColor = new Color[2];
    Color[] iconColor = new Color[2];
    Color onTextColorOrange = new Color();

    [Header("텍스트 온 컬러 오렌지 사용시 아래 체크")]
    public bool isTextColorOrange;

    public string getColorObjName = "ControlSystem";

    // Use this for initialization
    void Awake()
    {

        // 컬레 데이터를 자기 있는 오브젝트를 찾는다//
        UI_BaseColorSetting cSet = GameObject
            .Find(getColorObjName)
            .GetComponent<UI_BaseColorSetting>();

        lineColor = cSet.GetLineColor02();
        textColor = cSet.GetTextColor();
        iconColor = cSet.GetIconColor();
        onTextColorOrange = iconColor[1];

        int chCount = transform.childCount;

        // 외곽라인이 되는 이미지를 지정한다.//
        for (int i = 0; i < chCount; i++)
        {
            btnLine = transform.GetChild(i).GetComponent<Image>();
            if (btnLine != null)
            {
                string cName = btnLine.name;
                if (cName.ToLower().Contains("outline")) break;
            }
        }

        // 텍스트를 지정한다.//
        for (int i = 0; i < chCount; i++)
        {
            btnText = transform.GetChild(i).GetComponent<Text>();
            if (btnText != null)
            {
                string cName = btnText.name;
                if (cName.ToLower().Contains("text")) break;
            }
        }

        // 아이콘을 지정한다.//
        for (int i = 0; i < chCount; i++)
        {
            btnIcon = transform.GetChild(i).GetComponent<Image>();
            if (btnIcon != null)
            {
                string cName = btnIcon.name;
                if (cName.ToLower().Contains("icon")) break;
            }
        }



        // 이벤트 트리거를 세팅한다.//
        // (이벤트 트리거를 컴포넌트로 붙인다.)//
        if (GetComponent<EventTrigger>() == null) gameObject.AddComponent<EventTrigger>();
        EventTrigger trigger = GetComponent<EventTrigger>();


        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;

        //Debug.Log(name + "누를때 트리거 내용 지정");
        entryDown.callback.AddListener((eventData) =>
        {
            ChangeColor(1);
        });

        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;

        //Debug.Log(name + "뗄때 트리거 내용 지정");
        entryUp.callback.AddListener((eventData) =>
        {
            ChangeColor(0);
        });

        //Debug.Log("트리거 생성");
        trigger.triggers.Add(entryDown);
        trigger.triggers.Add(entryUp);

    }

    private void Start()
    {
        // 초기 컬러를 지정한다.//
        ChangeColor(0);
    }

    public void ChangeColor(int num)
    {
        //Debug.Log(this.name + ": Deactivation");
        if (btnLine != null)
            btnLine.color = lineColor[num];
        if (btnText != null)
        {
            if (num ==1 && isTextColorOrange)
                btnText.color = onTextColorOrange;
            else
                btnText.color = textColor[num];
        }
        //Debug.Log("아이콘 컬러 바꿈!");
        if(btnIcon != null)
            btnIcon.color = iconColor[num];
    }
}
