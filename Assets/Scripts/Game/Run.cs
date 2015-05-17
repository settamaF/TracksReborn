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
	public Dictionary<string, List<Vector3>> Rails { get { return mRails; } }
	public Transform Next { get; set; }
#endregion

#region Fields
	// Const   -----------------------------------------------------------------
	private const string NAME_RAIL = "Rail";
	private const string NAME_NEXT_POINT = "Next";

	// Private -----------------------------------------------------------------
	private Dictionary<string, List<Vector3>> mRails;
#endregion

#region Unity Methods
	void Awake()
	{
		Next = transform.FindChild(NAME_NEXT_POINT);
		if (!Next)
		{
			Debug.LogError(this.name + " Doesn't contain Next object");
			return;
		}
		InitRails();
	}
#endregion

#region Methods
	public List<Vector3>	GetRail(string index)
	{
		List<Vector3> rail;

		mRails.TryGetValue(index, out rail);
		if (rail == null)
		{
			Debug.LogError("No rail with " + index + " on the object " + name);
			return null;
		}
		return rail;
	}
#endregion

#region Implementation
	private void InitRails()
	{
		mRails = new Dictionary<string, List<Vector3>>();

		foreach(Transform child in transform)
		{
			if (child.name.Contains(NAME_RAIL) && child.childCount > 0)
			{
				List<Vector3> rail = new List<Vector3>();
				foreach(Transform points in child)
				{
					rail.Add(points.position);
				}
				string index = child.name.Substring(NAME_RAIL.Length - 1);
				mRails.Add(index, rail);
			}
		}
	}
#endregion
}
