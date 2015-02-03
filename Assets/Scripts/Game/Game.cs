//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using System.Collections.Generic;
using UnityEngine;

//******************************************************************************

public class Game : MonoBehaviour
{

#region Script Parameters

#endregion
	
#region Properties

#endregion
	
#region Fields
	// const


	// static
	private static Game		mInstance;
	public static Game		Get { get{ return mInstance; } }

	//private
	

	//Character value

#endregion
	
#region Unity Methods
	void Awake()
	{
		mInstance = this;
	}

	void Start()
	{

	}
	void Update()
	{

	}
#endregion
	
#region Implementation
	
#endregion
}
