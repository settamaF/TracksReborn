//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

public class EditorXML : EditorWindow
{
	public enum EditMode
	{
		ADD,
		MODIFY
	}

#region Fields
	TextAsset		mTxtAsset;
	Vector2			mScroll;
	EditMode		mEditMode;
	TextItems		mTextItems;
	ItemGUI			mAddItemGUI;
	List<ItemGUI>	mItemGUI;
#endregion

#region Unity Methods
	[MenuItem("Spinbot/EditorXML")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		EditorWindow.GetWindow(typeof(EditorXML));
	}

	void OnEnable()
	{
		mTxtAsset = (TextAsset)Resources.Load(GetResourcePath(Application.systemLanguage), typeof(TextAsset));
		mAddItemGUI = new ItemGUI();
		mItemGUI = new List<ItemGUI>();
		ReadTextAsset(mTxtAsset);
	}

	void OnDestroy()
	{

	}

	void OnGUI()
	{
		var newTxtAsset = (TextAsset)EditorGUILayout.ObjectField(mTxtAsset, typeof(TextAsset), true);

		if (newTxtAsset == null)
		{
			return;
		}
		else if (newTxtAsset != mTxtAsset)
		{
			if (!ReadTextAsset(newTxtAsset))
				return;
		}
		if (GUILayout.Button("Apply modifications"))
		{
			SaveModificationItems();
		}
		bool enabled = EditorGUILayout.BeginToggleGroup("Add", mEditMode == EditMode.ADD);
		if (enabled)
		{
			mEditMode = EditMode.ADD;
			mAddItemGUI.TextID = TextField("Text ID", mAddItemGUI.TextID);
			GUILayout.Label("Text");
			mAddItemGUI.Scroll = EditorGUILayout.BeginScrollView(mAddItemGUI.Scroll);
			mAddItemGUI.Text = EditorGUILayout.TextArea(mAddItemGUI.Text, GUILayout.MinHeight(position.height - 30));
			EditorGUILayout.EndScrollView();
			if (GUILayout.Button("Save"))
			{
				AddItemsToList(mAddItemGUI);
			}
		}
		EditorGUILayout.EndToggleGroup();
		enabled = EditorGUILayout.BeginToggleGroup("Modify", mEditMode == EditMode.MODIFY);
		if (enabled)
		{
			mEditMode = EditMode.MODIFY;
			mScroll = EditorGUILayout.BeginScrollView(mScroll);
			int id = 0;
			foreach(TextItem item in mTextItems.Items)
			{
				CreateGUIForeachItem(item, id);
				id++;
			}
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.EndToggleGroup();
		mTxtAsset = newTxtAsset;
	}
#endregion

#region Implementation
	bool ReadTextAsset(TextAsset txt)
	{
		mTextItems = Serialization.DeserialiseFromTextAsset<TextItems>(txt);

		if (mTextItems == null)
		{
			Debug.LogError("Error load file " + txt.name);
			return false;
		}
		RefreshItemsGUI(mTextItems.Items);
		return true;
	}

	void RefreshItemsGUI(List<TextItem> itemList)
	{
		for(int i =0; i < itemList.Count; i++)
		{
			mItemGUI.Add(new ItemGUI());
		}
	}

	string TextField(string label, string text)
	{
		var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
		EditorGUIUtility.labelWidth = textDimensions.x;
		return EditorGUILayout.TextField(label, text);
	}

	void CreateGUIForeachItem(TextItem textItem, int id)
	{
		Vector2	scroll = mItemGUI[id].Scroll;
		String textID = mItemGUI[id].TextID;
		String text = mItemGUI[id].Text;

		textID = TextField("Text ID", textItem.Id);
		GUILayout.Label("Text");
		scroll = EditorGUILayout.BeginScrollView(scroll);
		text = EditorGUILayout.TextArea(textItem.Text, GUILayout.MinHeight(position.height));
		EditorGUILayout.EndScrollView();
	}

	void SaveModificationItems()
	{
		//a sérializer 
	}

	void AddItemsToList(ItemGUI item)
	{
		if (item.TextID != "" && item.Text != "")
		{
			var tmp = mTextItems.GetTextItem(item.TextID);
			if (tmp != null)
			{
				Debug.Log("we already have an item with Id "+item.TextID);
				return;
			}
			TextItem textItem = new TextItem();
			textItem.Id = item.TextID;
			textItem.Text = item.Text;
			mTextItems.Items.Add(textItem);
			item.Clear();
			RefreshItemsGUI(mTextItems.Items);
		}
	}

	string GetResourcePath(SystemLanguage language)
	{
		switch (language)
		{
			default:
				return "xml/" + "LocalisedText_fr";
		}
	}	
#endregion
}

public class ItemGUI
{
#region Script Parameters
	public Vector2	Scroll;
	public String	TextID;
	public String	Text;
#endregion

#region Methods
	public void Clear()
	{
		Scroll = new Vector2(0, 0);
		TextID = null;
		Text = null;
	}
	#endregion
}

#endif
