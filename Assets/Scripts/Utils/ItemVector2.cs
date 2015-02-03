//******************************************************************************
// Author: Frédéric SETTAMA
//******************************************************************************

using UnityEngine;

public class ItemVector2
{
#region Fields
	private Vector2 mPos;
#endregion

#region Methods
	public ItemVector2(Vector2 pos)
	{
		mPos = pos;
	}
	
	public ItemVector2(float x, float y)
	{
		mPos = new Vector2(x,y);
	}
	
	public static implicit operator ItemVector2(Vector2 pos)
	{
		return new ItemVector2(pos);
	}
	
	public override int GetHashCode()
	{
		unchecked // Overflow is fine, just wrap
		{
			int hash = 17;
			hash = hash * 23 + Mathf.RoundToInt(mPos.x).GetHashCode();
			hash = hash * 23 + Mathf.RoundToInt(mPos.y).GetHashCode();
			return hash;
		}
	}
	
	public override bool Equals(object obj)
	{
		ItemVector2 other = obj as ItemVector2;
		
		return Mathf.RoundToInt(other.mPos.x) == Mathf.RoundToInt(mPos.x)
			&& Mathf.RoundToInt(other.mPos.y) == Mathf.RoundToInt(mPos.y);
	}
#endregion
}
