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
		public int				NbPoint = 0;
		public float			Height = 0f;
		public float			MarginError = 0.5f;
		public bool				Manual = false;
		public GameObject		Rail;

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

			this.LstPoint = new List<GameObject>();
			if (Height == 0)
			{
				if (Physics.Raycast(this.transform.position, -this.transform.up, out hit))
					Height = hit.distance;
			}
			//SetHeight();
			SetMiddle();
			SetNormal();
			Rail = new GameObject("Rail");
			this.CreatePoint(this.transform);
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				Manual = !Manual;
			if (this.NbPoint >= 0 && this.LstPoint.Count >= this.NbPoint)
				return;
			if (!mError)
			{
				if (!Manual || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					if (this.NextPoint())
						this.CreatePoint(this.transform);
					else
						this.mError = true;
					DebugDrawRay();
				}
			}
		}

	#endregion

	#region Methods

	#endregion

	#region Implementation
		private void SetNormal()
		{
			RaycastHit hit;

			if (Physics.Raycast(this.transform.position, -this.transform.up, out hit))
			{
				this.transform.up = hit.normal;
			}
		}

		private void CreatePoint(Transform tmp)
		{
			GameObject point = new GameObject("Point"+this.LstPoint.Count);

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
			SetForward();
			if (!this.SetMiddle())
			{
				return DebugError("No collider on left or right");
			}
			//SetNormal();
			return true;
		}

		private bool SetHeight()
		{
			RaycastHit	hit;
			float		deltaDistance;

			SetNormal();
			if (Physics.Raycast(this.transform.position, -this.transform.up, out hit))
			{
				deltaDistance = Height - hit.distance;
				this.transform.Translate(Vector3.up * deltaDistance);
			}
			else
			{
				Debug.Log("No ground");
				return false;
			}
			return true;
		}

		private bool SetMiddle()
		{
			RaycastHit	hitLeft;
			RaycastHit	hitRight;
			float		lenght;

			if (!Physics.Raycast(this.transform.position, this.transform.right, out hitRight))
			{
				Debug.Log("right");
				return false;
			}
			if (!Physics.Raycast(this.transform.position, -this.transform.right, out hitLeft))
			{
				Debug.Log("left");
				return false;
			}
			lenght = hitLeft.distance + hitRight.distance;
			lenght = lenght / 2;
			lenght = lenght - hitLeft.distance;
			this.transform.Translate(Vector3.right * lenght);
			return true;
		}

		private void SetForward()
		{
			Transform	lastPoint;
			Vector3		forward;
			float		angle;

			lastPoint = this.LstPoint[this.LstPoint.Count - 1].transform;
			forward = this.transform.position - lastPoint.position;
			/*angle = Vector3.Angle(this.transform.forward, forward);
			this.transform.Rotate(Vector3.up, angle);
			Debug.DrawLine(this.transform.position, lastPoint.position, Color.cyan, 100);*/
			//Debug.DrawRay(this.transform.position, forward * 10f, Color.blue, 100);
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

		private void DebugDrawRay()
		{
			Debug.DrawRay(this.transform.position, this.transform.forward * 10f, Color.yellow, 10);
			Debug.DrawRay(this.transform.position, this.transform.right * 10f, Color.green, 10);
		}

		private bool DebugError(string msg)
		{
			Debug.LogError("Error generation Rail : " + msg);
			return false;
		}
	#endregion
}
