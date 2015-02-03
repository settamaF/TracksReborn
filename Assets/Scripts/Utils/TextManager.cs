//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml; 
using System.Xml.Serialization;
using System.IO;

//******************************************************************************
public class TextManager : MonoBehaviour 
{
#region Script Parameters	
	public bool     TestCoverage        = false;
#endregion 
	
#region Static
	private static TextManager      mInstance   = null;
	public static TextManager 		Get	{ get {	return mInstance; }	}

	private static System.Random    mRandom     = new System.Random();
#endregion

#region Fields
	/// <summary>
	/// Random text item, stores when it has been used.  
	/// Changed from a Dictionary<string, bool>> as we need the Used value to be modifiable without recreating the list each time, and also need to have duplicate items.
	/// </summary>
	private class UsedTextItem
	{
		public UsedTextItem(string text)
		{
			Text    = text;
			Used    = false;
		}
		
		public bool     Used    { get; set; }
		public string   Text    { get; private set; }
	}
	
	private Dictionary<string, List<UsedTextItem>>  mTextManagerItems   = new Dictionary<string, List<UsedTextItem>>();
	private TextItems                               mTextItems;	
#endregion	

#region Unity Methods	
	/// <summary>
	/// Called on object instantiation. This is considered to be a replacement for the constructor.
	/// </summary>
	protected void Awake() 
	{   
	   	if(mInstance != null && mInstance != this) 
		{
			UnityEngine.Debug.Log("TextManager - we were instantiating a second copy of TextManager, so destroying this instance");
			DestroyImmediate(this.gameObject, true);
			return;
		}
	    
	    // Keep this object alive for the duration of the game
		DontDestroyOnLoad(this);
		
	    mInstance = this;
	    
	    LoadText( Application.systemLanguage );
	}
#endregion	

#region Methods	
	public string RequestText(string textId)
	{
		//DebugUtils.LogMessage(MessageFilter.System, "TextManager.RequestText - id {0}", textId);
		
		if(string.IsNullOrEmpty(textId))
		{
			Debug.LogError("TextManager.RequestText - textId null or empty");
			return "ERROR - no text id!";
		}

		List<UsedTextItem> textList;
	
		if(!mTextManagerItems.TryGetValue(textId, out textList))
		{
			Debug.LogError("TextManager.RequestText - no text with id "+textId);
			return "ERROR - " + textId;
		}
		
		if(textList.Count == 0)
		{
			Debug.LogError("TextManager.RequestText - id "+textId+" text list is empty");
			return "ERROR - " + textId;
		}
				
		if(textList.Count == 1)
		{
			return textList[0].Text;	
		}
				
		// We want each text randomly displayed once before one gets displayed again
		List<UsedTextItem> unused = new List<UsedTextItem>();
		foreach(UsedTextItem entry in textList)
		{
			// Get the strings that haven't been displayed yet
			if(!entry.Used)
			{
				unused.Add(entry);
			}
		}
		
		// If all have already been displayed, let's start over
		if(unused.Count == 0)
		{
			foreach(UsedTextItem entry in textList)
			{
				entry.Used = false;
			}
			
			unused = textList;
		}
						
		UsedTextItem randomItem = unused[mRandom.Next(unused.Count)];
		randomItem.Used = true;
		return randomItem.Text;	
	}	

    public void Flush()
    {
        mTextManagerItems.Clear();
    }

    public void LoadText( SystemLanguage language )
    {  
        TextAsset textAsset = (TextAsset)Resources.Load(GetResourcePath( language ), typeof(TextAsset));      
        
        mTextItems = Serialization.DeserialiseFromTextAsset<TextItems>(textAsset);
        
        // Add items to dictionary
        foreach(TextItem item in mTextItems.Items)
        {
			if(mTextManagerItems.ContainsKey(item.Id))
			{
            	Debug.LogError("TextManager.LoadText - we already have an item with Id "+item.Id);
			}
            mTextManagerItems[item.Id] = new List<UsedTextItem>();//new Dictionary<string, bool>();
            
            if(TestCoverage == true)
            {
                // text coverage mode.
                mTextManagerItems[item.Id].Add(new UsedTextItem("xxxxxxxxxxxxxx"));
            }
            else
            {
                // normal case, just as it used to be
                foreach( string text in item.GetList() )
                {
                    mTextManagerItems[item.Id].Add(new UsedTextItem(text));
                }
            }
        }
    }
#endregion	

#region Implementation	
	private string GetResourcePath( SystemLanguage language )
	{		
        switch( language )
		{
		default:
			return "xml/" + "LocalisedText_fr";
		}
	}	
#endregion
}	
	 
//******************************************************************************
public class TextItems
{
#region Properties
	public int              Version         { get { return mVersion; }      set { mVersion = value; } }
	public List<TextItem>   Items           { get { return mTextItems; }    set { mTextItems = value; } }
#endregion

#region Fields
	private List<TextItem>  mTextItems  = new List<TextItem>();
	private int             mVersion;
#endregion
	
#region Methods
	public TextItem GetTextItem(string textId)
	{
		Dictionary<string,  TextItem > dTextManagerItems = new Dictionary<string, TextItem>();

		// Add items to dictionary
		foreach(TextItem item in  Items)
		{
			dTextManagerItems.Add(item.Id,item);
		}
		if(dTextManagerItems.ContainsKey(textId))
		{
		  return dTextManagerItems[textId];
		}
        else
		{
			return null;
		}
	}
#endregion 
}

//******************************************************************************
[XmlInclude(typeof(RandomisedTextItem))]
public class TextItem
{
	public string Id        { get; set; }
	public string Text      { get; set; }
	public string EnHash    { get; set; }
	
	public virtual List<string> GetList()
	{
		List<string> textList = new List<string>();
		textList.Add( Text );
		return textList;
	}
}

//******************************************************************************
public class RandomisedTextItem : TextItem
{
	[XmlArray(ElementName = "TextList")]
	[XmlArrayItem(ElementName = "Text")]
	public List<string>     TextList    { get; set; }

	public override List<string> GetList()
	{
		return TextList;
	}
}