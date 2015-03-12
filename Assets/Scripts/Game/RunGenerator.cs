//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************

public class RunGenerator : MonoBehaviour
{
	#region Script Parameters
	public List<string> Runs;
	#endregion

	#region Static
	#endregion

	#region Properties
	#endregion

	#region Fields
	// Const -------------------------------------------------------------------
	private const string RUN_FOLDER = "Runs/";

	// Static -------------------------------------------------------------------
	private static RunGenerator mInstance;
	public static RunGenerator Get { get { return mInstance; } }

	// Private -----------------------------------------------------------------
	private int NumberRuns;

	#endregion

	#region Unity Methods
	void Awake()
	{
		mInstance = this;
	}

	void Start()
	{
		NumberRuns = Runs.Count;
	}
	#endregion

	#region Methods
	public Run GenerateRun(string run)
	{
		Run ret = null;

		ret = GameObject.Instantiate(Resources.Load(RUN_FOLDER + run)) as Run;
		if (ret == null)
		{
			Debug.LogError(run + " doest exist or instantiate failed");
		}
		return ret;
	}

	public Run GenerateRandomRun()
	{
		int rand = Random.Range(0, NumberRuns);

		return GenerateRun(Runs[rand]);
	}
	#endregion

	#region Implementation
	#endregion
}
