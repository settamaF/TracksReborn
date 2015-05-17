//******************************************************************************
// Authors: Frederic SETTAMA
//******************************************************************************

using UnityEngine;
using System.Collections;

//******************************************************************************

public class DebugFct : MonoBehaviour
{
#region Script Parameters
	public bool		SimulateInfiniteRun = false;
	public float	TimeDebug = 2;
	public float	Count = 0;
	public float	SpeedCamera = 1;
#endregion

#region Fields
	// Private -----------------------------------------------------------------
	private Rail		mDefaultRail = null;
	private bool		mCameraDebugInit = false;
	private Game		mGame;
	private Transform	mActualIndexPoint;
	private float		mLerpT = 0;
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
		else
			MoveCamera();
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

#endregion

#region Implementation
	private void CameraFollowRailInit()
	{
		if (!mCameraDebugInit)
		{
			mCameraDebugInit = true;
			foreach (var rail in mGame.Rails)
			{
				if (rail.Index == "0.5")
				{
					mDefaultRail = rail;
					break;
				}

			}
			if (mDefaultRail == null)
				return;
			Camera.main.transform.position = mDefaultRail.Points[0].position;
			Camera.main.transform.LookAt(mDefaultRail.Points[1]);
			mActualIndexPoint = mDefaultRail.Points[0];
		}
	}

	private void MoveCamera()
	{
		Vector3 position;
		int index = 0;

		CameraFollowRailInit();
		if (mDefaultRail == null)
			return;
		mLerpT += Time.deltaTime * SpeedCamera;
		mLerpT = Mathf.Clamp(mLerpT, 0, 1);
		for (int i = 0; i < mDefaultRail.Points.Count; i++ )
		{
			if (mDefaultRail.Points[i] == mActualIndexPoint)
			{
				index = i;
				break;
			}
		}
		position = Vector3.Lerp(mDefaultRail.Points[index].position, mDefaultRail.Points[index + 1].position, mLerpT);
		if (mLerpT >= 1)
		{
			mLerpT = 0;
			index++;
			mActualIndexPoint = mDefaultRail.Points[index];
		}
		Camera.main.transform.position = position;
		Camera.main.transform.LookAt(mDefaultRail.Points[index + 1]);
		mGame.UpdateRuns(Camera.main.transform);
	}
#endregion
}
