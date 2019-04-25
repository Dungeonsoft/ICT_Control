using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnColorChange : MonoBehaviour
{
    public bool isFirstOn;
    Image btnBase;
    Image btnLine;
    Image btnIcon;

    [Header("Element0: False ::: Element1: True")]

    // 온 오프 컬러를 지정한다.//
    // 0: False 1: True //
    Color[] baseColor = new Color[2];
    Color[] lineColor = new Color[2];
    Color[] iconColor = new Color[2];

    // 컬러 컴포넌트를 가지고 있는 오브젝트 이름//
    public string getColorObjName = "ControlSystem";
    private void Awake()
    {
        // 컬레 데이터를 자기 있는 오브젝트를 찾는다//
        UI_BaseColorSetting cSet = GameObject
            .Find(getColorObjName)
            .GetComponent<UI_BaseColorSetting>();

        // 각각의 컬러를 외부에서 가져와 지정한다.//
        baseColor = cSet.GetBaseColor();
        lineColor = cSet.GetLineColor01();
        iconColor = cSet.GetIconColor();

        // 베이스가 되는 이미지를 지정한다.//
        btnBase = GetComponent<Image>();

        // 베이스의 아웃라인이 되는 이미지를 지정한다.//
        int chCount = transform.childCount;
        for (int i = 0; i < chCount; i++)
        {
            btnLine = transform.GetChild(i).GetComponent<Image>();
            if (btnLine != null)
            {
                string cName = btnLine.name;
                if (cName.ToLower().Contains("outline")) break;
            }
        }

        // 베이스의 아이콘이 되는 이미지를 지정한다.//
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


        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;

        //Debug.Log(name + "트리거 내용 지정");
        entry.callback.AddListener((eventData) =>
        {
            ActivationButton();
        });

        //Debug.Log("트리거 생성");
        trigger.triggers.Add(entry);


    }

    private void Start()
    {
        // 초기 컬러를 지정한다.//
        if (isFirstOn)
            ActivationButton();
        else
            DeactivationButton();
    }


    public void DeactivationButton()
    {
        //Debug.Log(this.name + ": Deactivation");
        btnBase.color = baseColor[0];
        if (btnLine != null) btnLine.color = lineColor[0];
        if (btnIcon != null) btnIcon.color = iconColor[0];
        
    }

    public void ActivationButton()
    {
        Debug.Log(this.name + ": Activation");
        btnBase.color = baseColor[1];
        if (btnLine != null) btnLine.color = lineColor[1];
        if (btnIcon != null) btnIcon.color = iconColor[1];

        if (GetComponent<DeactivateList>() != null)
        {
            GetComponent<DeactivateList>().DeactivateBtn();
        }
    }
}
