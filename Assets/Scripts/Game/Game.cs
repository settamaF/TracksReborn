//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using UnityEngine;
using System.Collections.Generic;

//******************************************************************************

public class Game : MonoBehaviour
{
#region Script Parameters
	public GameObject		Player;
	public float			MoveSpeed = 1;
	public float			RotateSpeed = 1;
	public float			SwitchSpeed = 1;
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
	private Rail			mActualRail;
	private int				mActualIndexRail;
	private Transform		mActualIndexPoint;
	private Rail			mNextRail = null;
	private float			mLerpT = 0;
	private Vector2			mCurrentInput = Vector2.zero;
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
		if (InitPlayerPosition())
			this.enabled = true;
		else
			this.enabled = false;
	}

	void Update()
	{
		Vector2 input = InputManager.Get.GetDirectionNormalizedInput();
		if (mCurrentInput.Equals(Vector2.zero))
		{
			mCurrentInput = input;
			ExecuteInput();
		}
		MovePlayer();
		UpdateRuns(Player.transform);
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

	bool InitPlayerPosition()
	{
		int index = 0;

		foreach (var rail in mRails)
		{
			if (rail.Index == "0.5")
			{
				mActualRail = rail;
				break;
			}
			index++;
		}
		if (mActualRail == null)
			return false;
		mActualIndexRail = index;
		mNextRail = mActualRail;
		if (!Player)
		{
			Player = Camera.main.gameObject;
		}
		Player.transform.position = mActualRail.Points[0].position;
		Player.transform.LookAt(mActualRail.Points[1]);
		mActualIndexPoint = mActualRail.Points[0];
		return true;
	}

	void MovePlayer()
	{
		Vector3 position;
		int index = 0;

		mLerpT += Time.deltaTime * MoveSpeed;
		mLerpT = Mathf.Clamp(mLerpT, 0, 1);
		for (int i = 0; i < mActualRail.Points.Count; i++)
		{
			if (mActualRail.Points[i] == mActualIndexPoint)
			{
				index = i;
				break;
			}
		}
		position = Vector3.Lerp(mActualRail.Points[index].position, mNextRail.Points[index + 1].position, mLerpT);
		//if jump update heigt
		if (mLerpT >= 1)
		{
			mLerpT = 0;
			index++;
			if (mNextRail != mActualRail)
			{
				mActualRail = mNextRail;
				if (mCurrentInput.Equals(InputManager.Left))
					mActualIndexRail--;
				else
					mActualIndexRail++;
				mCurrentInput = Vector2.zero;
			}
			mActualIndexPoint = mActualRail.Points[index];
		}
		Player.transform.position = position;
		var smoothRotation = Quaternion.LookRotation(mActualRail.Points[index + 1].position - Player.transform.position);
		Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, smoothRotation, RotateSpeed * Time.deltaTime);
	}

	void ExecuteInput()
	{
		if (mCurrentInput.Equals(InputManager.Left) || mCurrentInput.Equals(InputManager.Right))
		{
			SwitchLane();
		}
		else if (mCurrentInput.Equals(InputManager.Up))
		{
			Jump();
		}
		else
		{
			Crouch();
		}

	}
	void SwitchLane()
	{
		int indexNextLane = mActualIndexRail;

		if (mCurrentInput.Equals(InputManager.Left))
			indexNextLane--;
		else
			indexNextLane++;
		if (indexNextLane < 0 || indexNextLane >= mRails.Count)
		{
			//GameOver
			mCurrentInput = Vector2.zero;
		}
		else
		{
			mNextRail = mRails[indexNextLane];
		}
	}

	void Jump()
	{
		mCurrentInput = Vector2.zero;
	}
	
	void Crouch()
	{
		mCurrentInput = Vector2.zero;
	}
#endregion
}
