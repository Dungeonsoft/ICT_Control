using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 델레게이트 모음//
// 여기 올려놓으면 여기저기 다 쓰일 수 있다.//
public delegate void MyDelegate(int roleNum);
// 델레게이트 모음//


public class ExtraWin : MonoBehaviour {


    MyDelegate myDel;

    // 이곳으로 실행해야할 메소드를 넘겨줌//
    public void NextMethod(MyDelegate nMethod)
    {
        myDel = nMethod;
    }

    // 넘겨진 메소드를 상황에 맞게 실행하여줌//
    public void RunDel(int val)
    {
        Debug.Log("RunDel 실행 :: Input Value :: " + val);
        if (myDel != null) myDel(val);
    }
}

//Role
//0: 랜덤 셀렉트//
//1: 선장//
//2: 1항사//
//3: 2항사//
//4: 3항사//
//5: 기관장//
//6: 1기사//
//7: 2기사//
//8: 3기사//
//9: 갑판장//
//10: 갑판수A//
//11: 갑판수B//
//12: 갑판수C//
//13: 조기장//
//14: 조기수A//
//15: 조기수B//
//16: 조기수C//
//17: 조리장//
