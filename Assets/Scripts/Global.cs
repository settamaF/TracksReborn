//******************************************************************************
// Author: Frédéric SETTAMA
//******************************************************************************

using UnityEngine;

//******************************************************************************
public class Global : MonoBehaviour
{
#region Static
		private static 	Global    		mInstance = null;	

		public static 	Global 			Get	{ get { return mInstance; } }
#endregion

#region Properties

#endregion

#region Unity Methods
	void Awake ()
	{
		if(mInstance != null && mInstance != this)
		{
			DestroyImmediate (this.gameObject, true);
			return;
		}

		DontDestroyOnLoad (this);
		mInstance = this;
	}
#endregion

#region Methods

#endregion
}
