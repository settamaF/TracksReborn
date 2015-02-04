//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************

public class RailGenerator : MonoBehaviour
{
	#region Script Parameters
		public List<GameObject>	LstPoint;
		public float			Distance = 1f;
		public float			Height = 0f;
		public float			MarginError = 0.5f;
		public bool				Manual = false;
		public GameObject		Rail;
		public bool				DestroyRail = true;
		public float			Margin = 0.5f; //Value between 0 and 1, 0.5 = center rail
	#endregion

	#region Static

	#endregion

	#region Properties

	#endregion

	#region Fields
		private bool	mError = false;

	#endregion

	#region Unity Methods
		void Start () 
		{
			RaycastHit hit;

			if (Margin < 0 || Margin > 1)
			{
				DebugError("Error value margin, value must be between 0 and 1");
				mError = true;
				return;
			}

			this.LstPoint = new List<GameObject>();
			if (Height == 0)
			{
				if (Physics.Raycast(this.transform.position, -this.transform.up, out hit))
					Height = hit.distance;
			}
			SetHeight();
			SetMiddle();
			Rail = new GameObject("Rail" + Margin);
			if (!DestroyRail)
				Rail.hideFlags = HideFlags.DontSave;
			this.CreatePoint(this.transform);
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				Manual = !Manual;
			if (!mError)
			{
				if (!Manual || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					if (this.NextPoint())
						this.CreatePoint(this.transform);
					else
						this.mError = true;
				}
			}
		}

	#endregion

	#region Methods

	#endregion

	#region Implementation

		private void CreatePoint(Transform tmp)
		{
			GameObject point = new GameObject("Point"+this.LstPoint.Count);

			if (!DestroyRail)
				point.hideFlags = HideFlags.DontSave;
			point.transform.position = tmp.position;
			point.transform.rotation = tmp.rotation;
			this.LstPoint.Add(point);
			point.transform.parent = Rail.transform;
		}

		private bool NextPoint()
		{
			this.transform.Translate(Vector3.forward * this.GetDistance());


			if (!this.SetHeight())
				return DebugError("No Ground");
			if (!this.SetMiddle())
				return DebugError("No collider on left or right");
			SetForward();
			return true;
		}

		private bool SetHeight()
		{
			RaycastHit	hit;
			float		deltaDistance;

			if (Physics.Raycast(this.transform.position, -Vector3.up, out hit))
				this.transform.up = hit.normal;
			if (Physics.Raycast(this.transform.position, -this.transform.up, out hit))
			{
				deltaDistance = Height - hit.distance;
				this.transform.Translate(Vector3.up * deltaDistance);
				return true;
			}
			return false;
		}

		private bool SetMiddle()
		{
			RaycastHit	hitLeft;
			RaycastHit	hitRight;
			float		lenght;

			if (LstPoint.Count > 0)
				this.transform.forward = this.LstPoint[this.LstPoint.Count - 1].transform.forward;
			if (!Physics.Raycast(this.transform.position, this.transform.right, out hitRight))
				return false;
			if (!Physics.Raycast(this.transform.position, -this.transform.right, out hitLeft))
				return false;
			lenght = hitLeft.distance + hitRight.distance;
			lenght = lenght * Margin;
			lenght = lenght - hitLeft.distance;
			this.transform.Translate(Vector3.right * lenght);
			return true;
		}

		private void SetForward()
		{
			Transform	lastPoint;
			Vector3		forward;

			if (this.LstPoint.Count <= 0)
				return;
			lastPoint = this.LstPoint[this.LstPoint.Count - 1].transform;
			forward = this.transform.position - lastPoint.position;
			Debug.DrawLine(this.transform.position, lastPoint.position, Color.cyan, 100);
			this.transform.forward = forward;
		}

		private float GetDistance()
		{
			RaycastHit hit;

			if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, this.Distance))
			{
				return Distance - hit.distance - MarginError;
			}
			return Distance;
		}

		private bool DebugError(string msg)
		{
			Debug.LogError("Error generation Rail : " + msg);
			return false;
		}
	#endregion
}
