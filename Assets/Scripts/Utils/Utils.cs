//******************************************************************************
// Author: Frédéric SETTAMA
//******************************************************************************

using UnityEngine;
using System.Security.Cryptography;
using System.Text;

//******************************************************************************
public class Utils : MonoBehaviour 
{
	public static Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float y=0)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		Plane xz = new Plane(Vector3.up, new Vector3(0, y, 0));
		float distance;
		xz.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}
	
	// Encrypt the specified plainText using MD5 hash.
	public static string Encrypt( string plainText )
	{
		// Calculate MD5 hash from input
		MD5.Create();
		MD5 md5 			= MD5.Create();
		byte[] inputBytes 	= Encoding.ASCII.GetBytes( plainText );
		byte[] hash 		= md5.ComputeHash( inputBytes );
		
		// Convert byte array to hex string
		var stringBuilder = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
		{
			stringBuilder.Append(hash[i].ToString("X2"));
		}
		
		return stringBuilder.ToString();
	}
}
