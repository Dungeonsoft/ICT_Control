using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };
	

    /// <summary>
    /// 전체 시나리오 파일을 읽어서 프로그램에서 사용가능하도록 만든다.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
	public static List<Dictionary<string, object>> Read(string file)
	{
        //Debug.Log("file Path :: " + file);
		var list = new List<Dictionary<string, object>>();
		TextAsset data = Resources.Load (file) as TextAsset;
        //Debug.Log("Data: "+ data);
		var lines = Regex.Split (data.text, LINE_SPLIT_RE);
		
        // 렝쓰가 1과 같거나 작으면 바로 리턴하는 것은 처리할 값이 없어서이다.
        // 첫번째 줄은 단순히 데이터가 어떤 것인지 정의하기 위해 있는 것이라.
        // 실제 데이터와는 상관이 없다.
		if(lines.Length <= 1) return list;
		
		var header = Regex.Split(lines[0], SPLIT_RE);

        // 아래 for 문에서 i 값이 1부터 시작하는 것은 실제 csv파일(엑셀 파일)을 확인하면.
        // 첫번째줄(0번줄)은 데이터 값이 아니라 각 칼럼의 값이 어떤 것인지 데이터의 정의.
		for(var i=1; i < lines.Length; i++) {
			
			var values = Regex.Split(lines[i], SPLIT_RE);
			if(values.Length == 0 ||values[0] == "") continue;
			
			var entry = new Dictionary<string, object>();
			for(var j=0; j < header.Length && j < values.Length; j++ ) {
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				object finalvalue = value;
				int n;
				float f;
				if(int.TryParse(value, out n)) {
					finalvalue = n;
				} else if (float.TryParse(value, out f)) {
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add (entry);
		}
		return list;
	}
}
