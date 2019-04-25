using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UserInfoBar : MonoBehaviour {

    public GameObject userRole;
    public GameObject userName;
    public GameObject userStatus;
    public GameObject userResult;
    public GameObject userSelect;

    [SerializeField]
    public GetUserBaseData uData;

    public bool isUsing = false;

    public float getHeight
    {
        get
        {
            float height = 0;
            height = userRole.GetComponent<RectTransform>().rect.height;
            return height;
        }
    }

    public void SetDeactiveRole()
    {

    }

    public void SetUserData(bool isSend = true)
    {
        string roleName = SetRoleToString(int.Parse(uData.role));
        // Debug.Log("Role Name::: " + roleName);
        userRole.transform.Find("Text").GetComponent<Text>().text = roleName;
        userName.transform.Find("Text").GetComponent<Text>().text = uData.name;
        userStatus.transform.Find("Text").GetComponent<Text>().text = uData.status;
        userResult.transform.Find("Text").GetComponent<Text>().text = uData.result;
        //isUsing = true;
        // 역할이 변경되었으니 그 정보를 서버에 넘겨준다.
        if (isSend == true) SendData();
    }

    public void SetNpcData()
    {
        userStatus.transform.Find("Text").GetComponent<Text>().text = uData.status;
    }

    public void SetComRole()
    {
        //Debug.Log("SetComRole");
        string roleName = SetRoleToString(int.Parse(uData.role));
        userRole.transform.Find("Text").GetComponent<Text>().text = roleName;
    }

    public string SetRoleToString(int r)
    {
        string rName = "";
        switch(r)
        {
            case 1:
                rName = "Captain";
                break;
            case 2:
                rName = "Officer 1st";
                break;
            case 3:
                rName = "Officer 2nd";
                break;
            case 4:
                rName = "Officer 3rd";
                break;
            case 5:
                rName = "Engineer Chief";
                break;
            case 6:
                rName = "Engineer 1st";
                break;
            case 7:
                rName = "Engineer 2nd";
                break;
            case 8:
                rName = "Engineer 3rd";
                break;
            case 9:
                rName = "Bosun";
                break;
            case 10:
                rName = "Crew A";
                break;
            case 11:
                rName = "Crew B";
                break;
            case 12:
                rName = "Crew C";
                break;
            case 13:
                rName = "Oiler 1st";
                break;
            case 14:
                rName = "Oiler A";
                break;
            case 15:
                rName = "Oiler B";
                break;
            case 16:
                rName = "Oiler C";
                break;
            case 17:
                rName = "Cook Chief";
                break;
        }
        return rName;
    }

    public void SendData()
    {
        Debug.Log("SendData 실행");
        GameObject cSystem = GameObject.Find("ControlSystem");
        Debug.Log("cSystem name::: "+ cSystem.name);
        cSystem.GetComponent<LobbyServerConnect>().SendChangedUserData(uData);
    }
}
