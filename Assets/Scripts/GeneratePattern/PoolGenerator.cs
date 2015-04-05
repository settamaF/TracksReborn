//******************************************************************************
// Authors: Frederic SETTAMA
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

//******************************************************************************

public enum ObjectType
{
	OBSTACLE = 0,
	BLIND,
	GROUND,
	EVENT
}

[System.Serializable]
public class Stacks
{
	public ObjectType Type;
	public List<ObjectStack> Stack;

	public Stacks(ObjectType type, List<ObjectStack> stack)
	{
		Type = type;
		Stack = stack;
	}
}

[System.Serializable]
public class ObjectStack
{
	public string				Name;
	public Stack<GameObject>	Objects;
}

[System.Serializable]
public class ObjectPrefab
{
	public ObjectType Type;
	public List<GameObject> Prefabs;
}

public class PoolGenerator : MonoBehaviour 
{
#region Script Parameters
	public List<ObjectPrefab> ObjectPrefabs;
#endregion

#region Static
	public static int mMaxEnumType;
#endregion

#region Properties
	private static PoolGenerator mInstance;
	public static PoolGenerator Get {get {return mInstance;}}
#endregion

#region Fields
	// Const -------------------------------------------------------------------
	private const int	DEFAULT_DUPLICATION = 10;
	private const int	OFFSETX = -1000;
	private const int	OFFSETY = -1000;

	// Private -----------------------------------------------------------------
	private List<Stacks> mPool;
#endregion

#region Unity Methods
	void Awake()
	{
		if (mInstance != null && mInstance != this)
		{
			Debug.LogWarning("PoolGenerator - we were instantiating a second copy of PoolGenerator, so destroying this instance");
			DestroyImmediate(this.gameObject, true);
			return;
		}
		mInstance = this;
		DontDestroyOnLoad(this);
		Initialization();
	}
#endregion

#region Methods
	public GameObject GetRandomObject(ObjectType type)
	{
		int range = 0;
		int rand;

		foreach(var stack in mPool)
		{
			if (stack.Type == type)
			{
				range = stack.Stack.Count;
				if (range <= 0)
					return null;
				rand = Random.Range(0, range);
				return GetObject(type, stack.Stack[rand].Name);
			}
		}
		return null;
	}

	public GameObject GetObject(ObjectType type, string name)
	{
		int index = GetIndexStack(type);

		if (index == -1)
		{
			Debug.LogWarning("Warning no object with " + type.ToString());
			return null;
		}
		foreach (var stack in mPool[index].Stack)
		{
			if (stack.Name.ToLower() == name.ToLower())
			{
				GameObject ret;
				if (stack.Objects.Count > 0)
				{
					ret = stack.Objects.Pop();
					if (ret)
						return ret;
				}
				ret = InstantiatePrefab(type, name);
				return ret;
			}
		}
		return null;
	}

	public void AddToStack(GameObject gameObject, ObjectType type)
	{
		foreach (var stacks in mPool)
		{
			if (stacks.Type == type)
			{
				foreach (var stack in stacks.Stack)
				{
					if (stack.Name == gameObject.name)
					{
						stack.Objects.Push(gameObject);
						gameObject.transform.parent = this.transform;
						gameObject.transform.localPosition = Vector3.zero;
						break;
					}
				}
			}
		}
	}
#endregion

#region Implementation

	private void Initialization()
	{
		mMaxEnumType = (int)System.Enum.GetValues(typeof(ObjectType)).Cast<ObjectType>().Last();
		this.transform.position = new Vector3(OFFSETX, OFFSETY, 0);
		InitPoolGenerator();
	}

	private void InitPoolGenerator()
	{
		mPool = new List<Stacks>();
		for (int i = 0; i <= mMaxEnumType; i++)
		{
			int index = GetIndexPrefab((ObjectType)i);
			if (index >= 0)
			{
				List<ObjectStack> stack = new List<ObjectStack>();
				foreach (var prefab in ObjectPrefabs[index].Prefabs)
				{
					ObjectStack obj = new ObjectStack();
					obj.Name = prefab.name;
					obj.Objects = GeneratePool(prefab);
					stack.Add(obj);
				}
				mPool.Add(new Stacks((ObjectType)i, stack));
			}
		}
	}

	private int GetIndexPrefab(ObjectType type)
	{
		for (int i = 0; i < ObjectPrefabs.Count; i++)
		{
			if (ObjectPrefabs[i].Type == type)
				return i;
		}
		return -1;
	}

	private int GetIndexStack(ObjectType type)
	{
		for (int i = 0; i < mPool.Count; i++)
		{
			if (mPool[i].Type == type)
				return i;
		}
		return -1;
	}

	private Stack<GameObject> GeneratePool(GameObject prefab)
	{
		Stack<GameObject> ret = new Stack<GameObject>();

		for (int i = 0; i < DEFAULT_DUPLICATION; i++)
		{
			GameObject obj = InstantiatePrefab(prefab);

			if (obj)
			{
				ret.Push(obj);
			}
			else
			{
				Debug.LogError("Error instantiate prefab: " + prefab.name);
				break;
			}
		}
		return ret;
	}

	private GameObject InstantiatePrefab(ObjectType type, string name)
	{
		int index = GetIndexPrefab(type);

		if (index < 0)
			return null;
		foreach(var prefab in ObjectPrefabs[index].Prefabs)
		{
			if (prefab.name == name)
				return InstantiatePrefab(prefab);
		}
		return null;
	}

	private GameObject InstantiatePrefab(GameObject prefab)
	{
		GameObject ret;

		ret = Instantiate(prefab) as GameObject;
		ret.transform.parent = this.transform;
		ret.transform.localPosition = Vector3.zero;
		ret.name = prefab.name;
		return ret;
	}
#endregion
}
