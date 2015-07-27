//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************
public class InputManager : MonoBehaviour 
{
#region Script Parameters
#endregion

#region Properties
#endregion

#region Fields
	// Const -------------------------------------------------------------------
	private static int				MIN_SWIPE = 50;
	private static int				MAX_STACK = 10;
	//static
	private static InputManager		mInstance;
	public static InputManager		Get { get { return mInstance; } }
	public static Vector2			Left = new Vector2(-1, 0);
	public static Vector2			Right = new Vector2(1, 0);
	public static Vector2			Up = new Vector2(0, 1);
	public static Vector2			Down = new Vector2(0, -1);
	public static Vector2[]			Directions = new Vector2[] { Left, Right, Up, Down };

	// Private -----------------------------------------------------------------
	private Vector3					mClickedPos;
	private Queue<Vector2>			mInput = new Queue<Vector2>();

#endregion

#region Unity Methods
	void Start() 
	{
		mInstance = this;
	}
	
	void Update()
	{
		if (mInput.Count < MAX_STACK)
		{
			Vector2 direction = GetDirection();
			if (direction.Equals(Vector2.zero))
			{
				return;
			}
			mInput.Enqueue(direction);
		}
	}

#endregion

#region Methods
	public Vector2 GetPositionInput(Vector2 position)
	{
		Vector2 direction = Vector2.zero;

		if (mInput.Count == 0)
		{
			return direction;
		}
		direction = mInput.Dequeue();
		for (int i = 0; i < Directions.Length; ++i)
		{
			if (direction == Directions[i])
			{
				return position + direction;
			}
		}
		if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
		{
			Vector2 dir = direction.x > 0 ? Right : Left;
			position += dir;
		}
		else
		{
			Vector2 dir = direction.y > 0 ? Up : Down;
			position += dir;
		}
		return position;
	}
	
	public Vector2 GetDirectionInput()
	{
		Vector2 direction = Vector2.zero;

		if (mInput.Count == 0)
		{
			return direction;
		}
		direction = mInput.Dequeue();
		return direction;
	}

	public Vector2 GetDirectionNormalizedInput()
	{
		Vector2 direction = GetDirectionInput();

		if (!direction.Equals(Vector2.zero))
		{
			if (Mathf.Abs(direction.x) > MIN_SWIPE && Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
				direction = direction.x > 0 ? Right : Left;
			else if (Mathf.Abs(direction.y) > MIN_SWIPE)
				direction = direction.y > 0 ? Up : Down;
		}
		return direction;
	}
#endregion

#region Implementation
	Vector2 GetDirection()
	{
		Vector2 ret;

		ret = GetSwipe();
		/*if (ret.Equals(Vector2.zero))
		{
			ret = GetClickInput();
		}*/
		if (ret.Equals(Vector2.zero))
		{
			ret = GetDirectionKeyboardInput();
		}
		return ret;
	}

	Vector2 GetSwipe()
	{
		Vector2 ret = Vector2.zero;

		if (Input.GetMouseButtonDown(0))
		{
			mClickedPos = Input.mousePosition;
			return ret;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			Vector3 swipe = Input.mousePosition - mClickedPos;
			if (Mathf.Abs(swipe.x) < MIN_SWIPE)
				swipe.x = 0;
			if (Mathf.Abs(swipe.y) < MIN_SWIPE)
				swipe.y = 0;
			ret.x = swipe.x;
			ret.y = swipe.y;
		}
		return ret;
	}

	Vector2 GetClickInput()
	{
		Vector2 pos = Vector2.zero;

		if (Input.GetMouseButtonUp(0))
		{
			Vector3 wp = Utils.GetWorldPositionOnPlane(Input.mousePosition);
			pos = new Vector2(wp.x, wp.z);
			pos.x = Mathf.Floor(pos.x + .5f);
			pos.y = Mathf.Floor(pos.y + .5f);
		}
		return pos;
	}

	Vector2 GetDirectionKeyboardInput()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			return Left;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			return Right;
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			return Down;
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			return Up;
		}
		return Vector2.zero;
	}
#endregion
}
