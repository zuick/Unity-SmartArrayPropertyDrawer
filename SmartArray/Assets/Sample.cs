using UnityEngine;
using System.Collections;

public class Sample : MonoBehaviour {

	//[SmartArray]
	public int [] intArray;
	//[SmartArray]
	public Color [] floatArray;
	//[SmartArray]
	//public MyClass [] myclassArray;
}

[System.Serializable]
public class MyClass
{
	[Range(0,1)]
	public float myFloat;
	public string myString;
	public AnimationCurve myCurve;
}
