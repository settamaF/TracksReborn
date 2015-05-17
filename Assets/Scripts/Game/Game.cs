//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using UnityEngine;
using System.Collections.Generic;

//******************************************************************************

public class Game : MonoBehaviour
{
#region Script Parameters

#endregion

#region Properties
	public List<Rail>		Rails { get { return mRails; } }
	public List<Run>		Runs { get { return mRuns; } }
#endregion

#region Fields
	// const
	public const int		MAX_SIZE_RUNS = 4;
	private const int		MIN_LANE = -1;
	private const int		MAX_LANE = 1;
	private const string	START_RUN = "Runs/StartRun";

	// static
	private static Game		mInstance;
	public static Game		Get { get { return mInstance; } }

	//private
	private List<Run>		mRuns;
	private List<Rail>		mRails;
	private bool			mDeleteLastRun = false;
	private Run				mActualRun;
#endregion

#region Unity Methods
	void Awake()
	{
		mInstance = this;
	}
	void Start()
	{
		mRuns = new List<Run>();
		mRails = new List<Rail>();
		CreateRuns();
	}
#endregion

#region Methods
	public void DestroyLastRun()
	{
		Run delete;

		delete = mRuns[0];
		mRuns.RemoveAt(0);
		foreach (var rail in delete.Rails)
		{
			foreach (var globalRail in mRails)
			{
				if (globalRail.Index == rail.Key)
					globalRail.Remove(rail.Value.Count);
			}
		}
		PoolGenerator.Get.AddToStack(ObjectType.RUN, delete.gameObject);
	}

	public bool AddRuns()
	{
		Run run = null;
		Run lastRun;

		var gameObject = PoolGenerator.Get.GetRandomObject(ObjectType.RUN);
		if (gameObject)
			run = gameObject.GetComponent<Run>();
		if (!run)
		{
			Debug.LogError("Error add a run", this);
			return false;
		}
		lastRun = mRuns[mRuns.Count - 1];
		run.SetTransform(lastRun.Next);
		mRuns.Add(run);
		foreach (var rail in run.Rails)
		{
			bool find = false;
			foreach (var globalRail in mRails)
			{
				if (globalRail.Index == rail.Key)
				{
					globalRail.AddRail(rail.Value);
					find = true;
					break;
				}
			}
			if (!find)
			{
				Rail newRail = new Rail();
				newRail.Index = rail.Key;
				newRail.AddRail(rail.Value);
				mRails.Add(newRail);
			}
		}
		return true;
	}

	public void UpdateRuns(Transform trans)
	{
		RaycastHit hit;
		Run hitTransform;

		if (Physics.Raycast(trans.position, Vector3.down, out hit))
		{
			hitTransform = hit.transform.GetComponentInParent<Run>();
			if (hitTransform.gameObject != mActualRun.gameObject)
			{
				if (mDeleteLastRun)
				{
					DestroyLastRun();
				}
				else
					mDeleteLastRun = true;
				mActualRun = hitTransform.GetComponentInParent<Run>();
			}
		}
		else
			Debug.LogError("Error no ground below the controller");
		if (mRuns.Count < MAX_SIZE_RUNS)
		{
			AddRuns();
		}
	}
#endregion

#region Implementation

	void CreateFirstRun()
	{
		Run firstRun;

		firstRun = GameObject.Instantiate(Resources.Load(START_RUN, typeof(Run))) as Run;
		if (firstRun == null)
		{
			Debug.LogError(START_RUN + " doest exist or instantiate failed");
			Destroy(this.gameObject);
		}
		mRuns.Add(firstRun);
		mActualRun = firstRun;
		foreach(var rail in firstRun.Rails)
		{
			Rail newRail = new Rail();

			newRail.Index = rail.Key;
			newRail.AddRail(rail.Value);
			mRails.Add(newRail);
		}
	}

	void CreateRuns()
	{
		CreateFirstRun();
		for (int i = 1; i < MAX_SIZE_RUNS; i++)
		{
			if (!AddRuns())
				return;
		}
	}

#endregion
}
