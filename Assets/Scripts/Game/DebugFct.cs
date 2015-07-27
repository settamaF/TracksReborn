//******************************************************************************
// Authors: Frederic SETTAMA
//******************************************************************************

using UnityEngine;
using System.Collections;

//******************************************************************************

public class DebugFct : MonoBehaviour
{
#region Script Parameters
	public bool			SimulateInfiniteRun = false;
	public float		TimeDebug = 2;
	public float		Count = 0;
	public bool			PrintInput = false;
#endregion

#region Fields
	// Private -----------------------------------------------------------------
	private Game		mGame;
#endregion

#region Unity Methods

	void Start () 
	{
		mGame = Game.Get;
		if (!mGame)
		{
			Debug.LogError("No object with script Game on the scene", this);
			Destroy(gameObject);
		}
	}

	void Update()
	{
		if (SimulateInfiniteRun)
			InfiniteGenerationDebug();
		if (PrintInput)
			PrintInputFct();
		foreach (var rail in mGame.Rails)
		{
			rail.DebugShowRail();
		}
	}
#endregion

#region Methods

	private void InfiniteGenerationDebug()
	{
		Count += Time.deltaTime;
		if (Count >= TimeDebug)
		{
			Count = 0;
			mGame.AddRuns();
			if (mGame.Runs.Count > Game.MAX_SIZE_RUNS)
				mGame.DestroyLastRun();
		}
	}

	private void PrintInputFct()
	{
		Vector2 input = InputManager.Get.GetDirectionNormalizedInput();

		if (!input.Equals(Vector2.zero))
		{
			Debug.Log(input);
		}
	}
#endregion
}
