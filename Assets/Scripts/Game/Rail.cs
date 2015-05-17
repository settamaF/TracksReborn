//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************
public class Rail
{
#region Parameters
	public List<Transform>	Points = new List<Transform>();
	public string			Index;
#endregion

#region Static
#endregion

#region Properties
	
#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Private -----------------------------------------------------------------
#endregion

#region Methods
	public void AddRail(List<Transform> newRail)
	{
		Points.AddRange(newRail);
	}

	public void Remove(int countPoint)
	{
		if (countPoint < Points.Count)
			Points.RemoveRange(0, countPoint);
	}

	public void DebugShowRail()
	{
		for (int i = 0; i < Points.Count - 1; i++)
		{
			Debug.DrawLine(Points[i].position, Points[i + 1].position, Color.cyan);
		}
	}
#endregion

#region Implementation

#endregion
}
