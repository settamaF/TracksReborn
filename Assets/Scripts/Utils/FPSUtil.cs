//******************************************************************************
// Author: Frédéric SETTAMA
//******************************************************************************

using UnityEngine;

//******************************************************************************
public class FPSUtil : MonoBehaviour
{
	public UILabel Label;

	void Update ()
	{
		Label.text = (1/Time.smoothDeltaTime).ToString("N0")+" fps";
	}
}
