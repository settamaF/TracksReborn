//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************
public class Run : MonoBehaviour 
{
#region Script Parameters
#endregion

#region Static
#endregion

#region Properties
	public List<List<Vector3>> Rails { get { return mRails;}}
	public Transform Next { get; set; }
#endregion

#region Fields
	// Private -----------------------------------------------------------------
	private List<List<Vector3>> mRails;

#endregion

#region Unity Methods
	void Awake()
	{
		Next = transform.FindChild("Next");
		if (!Next)
		{
			Debug.LogError(this.name + " Doesn't contain Next object");
			return;
		}
		InitRails();
	}
#endregion

#region Methods
	public List<Vector3>	GetRail(int index)
	{
		if (index < 0 ||index > mRails.Count)
			return null;
		return mRails[index];
	}
#endregion

#region Implementation
	private void InitRails()
	{
		mRails = new List<List<Vector3>>();

		foreach(Transform child in transform)
		{
			if (child.name.Contains("Rail") && child.childCount > 0)
			{
				List<Vector3> rail = new List<Vector3>();
				foreach(Transform points in child)
				{
					rail.Add(points.position);
				}
				mRails.Add(rail);
			}
		}
	}
#endregion
}
