using UnityEngine;
using System.Collections;

public class UtilityFunctions : MonoBehaviour {
	public static bool isApproximate(float a, float b, float percision){
		float diff = Mathf.Abs (a - b);
		
		return (diff <= percision);
	}
}

