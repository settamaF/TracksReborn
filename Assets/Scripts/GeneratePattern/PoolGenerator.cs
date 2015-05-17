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
	RUN,
	DECOR,
	OBSTACLE,
	BONUS,
	NOTHING
}

public class PoolGenerator : MonoBehaviour 
{

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
	private const int	MAX_DUPLICATION = 50;
	private const int	OFFSETX = -1000;
	private const int	OFFSETY = -1000;

	// Private -----------------------------------------------------------------
	private Dictionary<ObjectType, Dictionary<string, Stack<GameObject>>> mPool;
#endregion

#region Unity Methods
	void Awake()
	{
		if (mInstance != null && mInstance != this)
		{
			Debug.LogWarning("PoolGenerator - we were instantiating a second copy of PoolGenerator, so destroying this instance", this);
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
		Dictionary<string, Stack<GameObject>> stacks;

		if (!mPool.TryGetValue(type, out stacks))
			return null;
		range = stacks.Count();
		if (range <= 0)
			return null;
		rand = Random.Range(0, range);
		var keyValue = stacks.ElementAt(rand);
		return GetObject(type, keyValue.Key);
	}

	public GameObject GetObject(ObjectType type, string name)
	{
		Dictionary<string, Stack<GameObject>> stacks;

		if (!mPool.TryGetValue(type, out stacks))
			return null;
		Stack<GameObject> stack;
		if (!stacks.TryGetValue(name, out stack))
			return null;
		GameObject ret;
		if (stack.Count > 0)
		{
			if (stack.Count == 1)
				ret = InstantiatePrefab(type, name);
			else
				ret = stack.Pop();
			if (ret)
			{
				ret.SetActive(true);
				return ret;
			}
		}
		return null;
	}

	public void AddToStack(ObjectType type, GameObject gameObject)
	{
		Dictionary<string, Stack<GameObject>> stacks;

		if (!mPool.TryGetValue(type, out stacks))
			return;
		Stack<GameObject> stack;
		if (!stacks.TryGetValue(gameObject.name, out stack))
		{
			Destroy(gameObject);
			return;
		}
		if (stack.Count >= MAX_DUPLICATION)
		{
			Destroy(gameObject);
			return;
		}
		stack.Push(gameObject);
		gameObject.transform.parent = stack.FirstOrDefault().transform.parent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.SetActive(false);
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
		mPool = new Dictionary<ObjectType,Dictionary<string,Stack<GameObject>>>();

		foreach (Transform childType in transform)
		{
			ObjectType objType = IsObjectType(childType.name);
			if (objType == ObjectType.NOTHING)
				continue;
			var stacks = new Dictionary<string, Stack<GameObject>>();
			foreach (Transform child in childType)
			{
				if (stacks.ContainsKey(child.name))
					break;
				var stack = GeneratePool(child.gameObject);
				stacks.Add(child.name, stack);
			}
			mPool.Add(objType, stacks);
		}
	}

	private Stack<GameObject> GeneratePool(GameObject prefab)
	{
		Stack<GameObject> ret = new Stack<GameObject>();

		prefab.transform.localPosition = Vector3.zero;
		prefab.SetActive(false);
		ret.Push(prefab);
		for (int i = 1; i < DEFAULT_DUPLICATION; i++)
		{
			GameObject obj = InstantiatePrefab(prefab);

			if (obj)
			{
				ret.Push(obj);
			}
			else
			{
				Debug.LogError("Error instantiate prefab: " + prefab.name, this);
				break;
			}
		}
		return ret;
	}

	private GameObject InstantiatePrefab(ObjectType type, string name)
	{
		Dictionary<string, Stack<GameObject>> objs;

		if (!mPool.TryGetValue(type, out objs))
			return null;
		Stack<GameObject> prefabs;
		if (!objs.TryGetValue(name, out prefabs))
			return null;
		return InstantiatePrefab(prefabs.FirstOrDefault());
	}

	private GameObject InstantiatePrefab(GameObject prefab)
	{
		GameObject ret;

		if (!prefab)
		{
			Debug.LogError("Error instantiate prefab " + prefab, this);
			return null;
		}
		ret = Instantiate(prefab) as GameObject;
		ret.transform.parent = prefab.transform.parent;
		ret.transform.localPosition = Vector3.zero;
		ret.name = prefab.name;
		ret.SetActive(false);
		return ret;
	}
	
	private ObjectType IsObjectType(string name)
	{
		if (name == ObjectType.BONUS.ToString("g"))
			return ObjectType.BONUS;
		else if (name == ObjectType.DECOR.ToString("g"))
			return ObjectType.DECOR;
		else if (name == ObjectType.RUN.ToString("g"))
			return ObjectType.RUN;
		else if (name == ObjectType.OBSTACLE.ToString("g"))
			return ObjectType.OBSTACLE;
		return ObjectType.NOTHING;
	}

#endregion
}
