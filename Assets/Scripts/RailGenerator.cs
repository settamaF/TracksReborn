using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RailGenerator : MonoBehaviour
{
	public List<GameObject>	LstPoint;
	public float			Distance = 1f;
	public int				NbPoint = 0;
	public float			Height = 0f;
	public float			MarginError = 0.5f;
	private bool			Error = false;

	public bool				Manual = false;
	public GameObject		Rail;
	// Use this for initialization
	void Start () 
	{
		RaycastHit hit;

		this.LstPoint = new List<GameObject>();
		if (Physics.Raycast(this.transform.position, -this.transform.up, out hit))
			this.Height = hit.distance;
		this.SetMiddle();
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
		if (!Error)
		{
			if (!Manual || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				if (this.NextPoint())
					this.CreatePoint(this.transform);
				else
				{
					Debug.Log("error");
					this.Error = true;
				}
			}
		}
		this.DebugDrawRay();
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
		float tmp = 0.1f;
		Vector3 tmpPosition = this.transform.position;
		Quaternion tmpRotation = this.transform.rotation;

		this.transform.Translate(Vector3.forward * this.CastForward());
		while (!this.SetHeight() && tmp < MarginError)
		{
			this.transform.Translate(Vector3.forward * tmp);
			tmp += 0.1f;
		}
		if (tmp >= MarginError)
		{
			this.transform.position = tmpPosition;
			this.transform.rotation = tmpRotation;
			return false;
		}
		tmp = 0.1f;
		while (!this.SetMiddle() && tmp < MarginError)
		{
			this.transform.Translate(Vector3.forward * tmp);
			tmp += 0.1f;
		}
		if (tmp >= MarginError)
		{
			this.transform.position = tmpPosition;
			this.transform.rotation = tmpRotation;
			return false;
		}
		if (this.AlignToGround())
		{
			this.SetForward();
			return true;
		}
		return false;
	}

	private bool SetHeight()
	{
		RaycastHit	hit;
		float		deltaDistance;

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

		if (!Physics.Raycast(this.transform.position, this.transform.right, out hitRight) ||
			!Physics.Raycast(this.transform.position, -this.transform.right, out hitLeft))
		{
			Debug.Log("No collider to left or right");
			return false; ;
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

		lastPoint = this.LstPoint[this.LstPoint.Count - 1].transform;
		forward = this.transform.position - lastPoint.position;
		this.transform.forward = forward;
	}

	private bool AlignToGround()
	{
		RaycastHit hit;

		if (Physics.Raycast(this.transform.position, -this.transform.up, out hit))
			this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		else
		{
			Debug.Log("No ground");
			return false;
		}
		return true;
	}

	private float CastForward()
	{
		RaycastHit hit;

		if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, this.Distance))
			return Distance - hit.distance - MarginError;
		return Distance;
	}

	private void DebugDrawRay()
	{
		Debug.DrawRay(this.transform.position, this.transform.forward * 10f, Color.yellow);
		Debug.DrawRay(this.transform.position, this.transform.right * 10f, Color.green);
	}
}
