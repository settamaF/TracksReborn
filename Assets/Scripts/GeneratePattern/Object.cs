//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************
public class Object : MonoBehaviour 
{
	public class ObjectChild
	{
		public ObjectType		Type;
		public int				ChildCount;
		public List<Transform>	Childs;
	}

	[System.Serializable]
	public class RateObject
	{
		public ObjectType		Type;
		public int				Rate;
	}

#region Script Parameters
	public ObjectType			Type;
	public bool					Static;
	public List<RateObject>		ListRate;
#endregion

#region Static
#endregion

#region Properties
	private Transform mNext;
	public Transform Next { get { return mNext; } }

#endregion	

#region Fields
	// Const -------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private List<ObjectChild> mObjectChild;
#endregion

#region Unity Methods
	void Awake()
	{
		if (Type == ObjectType.GROUND)
		{
			mNext = this.transform.FindChild("Next");
			if (!mNext)
			{
				Debug.LogError("Error no child Next in " + this.name);
				return;
			}
			Initialize();
		}
	}

	void OnDrawGizmos()
	{
		foreach (Transform child in transform)
		{
			Gizmos.color = GetDebugColor(child.name.ToUpper());
			if (Gizmos.color != Color.black)
				Gizmos.DrawSphere(child.position, 0.5f);
		}
	}
#endregion

#region Methods
	public void Initialize()
	{
		mObjectChild = new List<ObjectChild>();
		for (int i = 0; i <= PoolGenerator.mMaxEnumType; i++)
		{
			ObjectChild objectChild = new ObjectChild();

			objectChild.Type = (ObjectType)i;
			objectChild.Childs = Initialize((ObjectType)i);
			objectChild.ChildCount = objectChild.Childs.Count;
			mObjectChild.Add(objectChild);
		}
	}

	public void Use(ref bool eventActive, ref int eventDuration, ref bool setEventEnd)
	{
		foreach (var objectChild in mObjectChild)
		{
			foreach (Transform child in objectChild.Childs)
			{
				if (child.childCount > 0 || !RateGenerate(objectChild.Type))
					continue;
				if (objectChild.Type == ObjectType.EVENT && eventActive)
				{
					if (eventDuration <= 0)
					{
						eventActive = false;
						setEventEnd = true;
					}
					else
					{
						eventDuration--;
						continue;
					}
				}
				GameObject gameObject;
				if (objectChild.Type == ObjectType.EVENT && setEventEnd)
				{
					setEventEnd = false;
					gameObject = PoolGenerator.Get.GetObject(ObjectType.EVENT, "End Event");
					if (gameObject)
					{
						gameObject.GetComponent<EventTrigger>().Reset();
						gameObject.transform.parent = child;
						gameObject.transform.localPosition = Vector3.zero;
					}
				}
				else
				{
					gameObject = PoolGenerator.Get.GetRandomObject(objectChild.Type);
					if (gameObject)
					{
						gameObject.transform.parent = child;
						gameObject.transform.localPosition = Vector3.zero;
						if (objectChild.Type == ObjectType.EVENT)
						{
							eventActive = true;
							gameObject.GetComponent<EventTrigger>().Reset();
							eventDuration = gameObject.GetComponent<EventTrigger>().Duration;
						}
					}
				}
			}
		}
	}

	public void Reset()
	{
		foreach (var objectChild in mObjectChild)
		{
			foreach (Transform child in objectChild.Childs)
			{
				if (child.childCount > 0)
				{
					for (int i = 0; i < child.childCount; i++)
					{
						if (!child.GetChild(i).GetComponent<Object>().Static)
							PoolGenerator.Get.AddToStack(child.GetChild(i).gameObject, objectChild.Type);
					}
				}
			}
		}
		PoolGenerator.Get.AddToStack(this.gameObject, Type);
	}
#endregion

#region Implementation

	private List<Transform> Initialize(ObjectType type)
	{
		List<Transform> ret = new List<Transform>();

		foreach (Transform child in this.transform)
		{
			if (child.name.ToUpper() == type.ToString())
			{
				ret.Add(child);
			}
		}
		return ret;
	}

	private Color GetDebugColor(string type)
	{
		switch (type)
		{
			case "BLIND":
				return Color.red;
			case "EVENT":
				return Color.green;
			case "GROUND":
				return Color.blue;
			case "OBSTACLE":
				return Color.white;
			default:
				break;
		}
		return Color.black;
	}

	private bool RateGenerate(ObjectType type)
	{
		int rand = Random.Range(0, 101);
		int rate = 0;

		if (ListRate.Count <= 0)
			return true;
		foreach (var rates in ListRate)
		{
			if (rates.Type == type)
			{
				rate = rates.Rate;
				break;
			}
		}

		if (rand >= rate)
			return true;
		return false;
	}
#endregion
}
