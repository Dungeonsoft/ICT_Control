using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateList : MonoBehaviour {

    public List<BtnColorChange> deactList = new List<BtnColorChange>();

    public void DeactivateBtn()
    {
        foreach (var ch in deactList)
        {
            ch.DeactivationButton();
        }
    }

    private void Awake()
    {
        AddList();
    }

    // List에 자신을 제외한 나머지 버튼들을 추가해준다.//
    void AddList()
    {
        Transform pr = transform.parent;
        int chCnt = pr.childCount;
        for (int i = 0; i< chCnt; i++)
        {
           Transform go = pr.GetChild(i);
            if(go.name != name)
            {
                deactList.Add(go.GetComponent<BtnColorChange>());
            }
        }
    }
}
