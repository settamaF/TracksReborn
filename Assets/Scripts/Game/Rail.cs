//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************
public class Rail : MonoBehaviour 
{
#region Script Parameters
	public List<Vector3> Points;
#endregion

#region Static
#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Private -----------------------------------------------------------------
#endregion

#region Unity Methods

#endregion

#region Methods
	public void AddRail(List<Vector3> newRail)
	{
		Points.AddRange(newRail);
	}

	public void Remove(int countPoint)
	{
		Points.RemoveRange(0, countPoint);
	}

#endregion

#region Implementation

#endregion
}
