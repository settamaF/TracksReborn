//******************************************************************************
// Author: Frederic SETTAMA  
//******************************************************************************

using System.Collections.Generic;
using UnityEngine;

//******************************************************************************

public class Game : MonoBehaviour
{

	#region Script Parameters
	public float Speed = 10;
	#endregion

	#region Properties

	#endregion

	#region Fields
	// const
	private const int MAX_SIZE_RUNS = 4;
	private const string TAG_CHARACTER = "Player";
	private const int MIN_LANE = -1;
	private const int MAX_LANE = 1;

	// static
	private static Game mInstance;
	public static Game Get { get { return mInstance; } }

	//private
	private Transform mController;
	private List<GameObject> mRuns;
	private GameObject mActualRun;
	private bool mDeleteLast = false;


	//Character value
	private int mLanePos = 0;
	private float mDeltaLerp = 0;
	private bool mIsMoving = false;
	#endregion

	#region Unity Methods
	void Awake()
	{
		mInstance = this;
	}
	void Start()
	{
		GameObject tmp;

		tmp = GameObject.FindWithTag(TAG_CHARACTER);
		if (!tmp)
		{
			Debug.LogError("Error : No player set in the scene");
			Application.Quit();
		}
		mController = tmp.transform;
		mRuns = new List<GameObject>();
		CreateRuns();
	}

	void Update()
	{
		Vector2 input;

		UpdateRuns();
		input = InputManager.Get.GetDirectionNormalizedInput();
		if (!input.Equals(Vector2.zero))
		{
			MovePlayer((int)Mathf.Round(input.x));
		}
		UpdatePos();
	}
	#endregion

	#region Implementation
	void UpdatePos()
	{
		Vector3 target;

		if (!mIsMoving)
			return;


		//mController.position = Vector3.Lerp(mController.position, , Speed * Time.deltaTime);
		if (mDeltaLerp == 1)
			mIsMoving = false;
	}

	void MovePlayer(int input)
	{
		Debug.Log(input);
		if (input + mLanePos < MIN_LANE || input + mLanePos > MAX_LANE || mIsMoving)
			return;
		mLanePos += input;
		mDeltaLerp = 0;
		//mIsMoving = true;
		mController.Translate(Vector3.right * input);
	}

	void CreateFirstRun()
	{
		GameObject firstRun;
		Transform runTransform;
		Vector3 tmp;

		firstRun = RunGenerator.Get.GenerateRun("FirstRun");
		if (!firstRun)
		{
			Application.Quit();
		}
		runTransform = firstRun.transform;
		runTransform.position = Vector3.zero;
		runTransform.rotation = Quaternion.identity;
		tmp = runTransform.position;
		tmp.y += 1;
		mController.position = tmp;
		mRuns.Add(firstRun);
		mActualRun = firstRun;
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

	void UpdateRuns()
	{
		RaycastHit hit;
		Transform hitTransform;

		if (Physics.Raycast(mController.position, Vector3.down, out hit))
		{
			hitTransform = hit.transform;
			if (hitTransform.parent)
				hitTransform = hitTransform.parent;
			if (hitTransform.gameObject != mActualRun)
			{
				if (mDeleteLast)
					DestroyLastRun();
				else
					mDeleteLast = true;
				mActualRun = hitTransform.gameObject;
			}
		}
		else
			Debug.LogError("Error no ground below the player");
		if (mRuns.Count < MAX_SIZE_RUNS)
		{
			AddRuns();
		}
	}

	void DestroyLastRun()
	{
		GameObject delete;

		delete = mRuns[0];
		mRuns.RemoveAt(0);
		Destroy(delete);
	}

	bool AddRuns()
	{
		GameObject run;
		Transform runTransform;
		GameObject lastRun;
		Transform nextRun;

		run = RunGenerator.Get.GenerateRandomRun();
		if (!run)
		{
			Debug.LogError("Error add a run");
			return false;
		}
		lastRun = mRuns[mRuns.Count - 1];
		nextRun = lastRun.transform.FindChild("Next");
		if (!nextRun)
		{
			Debug.LogError("Error no child 'next' on the object " + lastRun.name);
			return false;
		}
		runTransform = run.transform;
		runTransform.position = nextRun.position;
		runTransform.rotation = nextRun.rotation;
		mRuns.Add(run);
		return true;
	}
	#endregion
}
