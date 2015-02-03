//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using UnityEngine;

//******************************************************************************
/// <summary>
/// Utility class for help with serialization.
/// </summary>
public abstract class Serialization 
{
#region Fields
	// Const -------------------------------------------------------------------	
	private const string    			PASSWORD    		= "aelrkasodifjawi";      // Not very secure but good enough for our purposes
	
	private static readonly byte[]		ENCRYPTION_SALT		= new byte[] 
	{
		0x53, 0x6f, 0x64, 0x19, 0x75, 0x6d, 0x20,             
		0x4a, 0x68, 0x6c, 0x6f, 0x7a ,0x69, 0x64, 0x67
	};
	
	#if UNITY_EDITOR
	// We deliberately do not encrypt files when running in the Unity Editor
	private static bool					SHOULD_ENCRYPT		= false;
	#else
	private static bool					SHOULD_ENCRYPT		= true;
	#endif
#endregion
	
#region Static
	public static void ToEncryptedFile<T>( T objectToSerialize, string pathAndFileName )
	{
		ToFile<T>( objectToSerialize, pathAndFileName, SHOULD_ENCRYPT );
	}
	
	public static void ToFile<T>( T objectToSerialize, string fileName, bool encrypt )
	{	
		string plainText 		= ObjectToString<T>( objectToSerialize );
		
		if( !encrypt )
		{
			ToFile<T>(objectToSerialize, fileName);
			return;
		}
		
		if(File.Exists(fileName))
		{
			File.Delete(fileName);
		}
		
		string encryptedText	= EncryptString( plainText, PASSWORD );
		File.WriteAllText( fileName, encryptedText, Encoding.UTF8 );
	}
	
	// Reads the file and return the content as string - Yas
	public static string ReadFile( string path, bool encrypt )
	{
		string encryptedText = File.ReadAllText( path );
		if( !encrypt )
		{
			return encryptedText;
		}
		else
		{
			string plainText = DecryptString( encryptedText, PASSWORD );
			return plainText;
		}
	}
	
	public static T FromEncryptedFile<T>( string pathAndFileName )
	{
		return FromFile<T>( pathAndFileName, SHOULD_ENCRYPT );
	}
	
	public static T FromFile<T>( string path, bool encrypt )
	{
		if(!encrypt)
		{
			return FromFile<T>(path);
		}
		
		string encryptedText	=  File.ReadAllText(path);
		string plainText 		= DecryptString(encryptedText, PASSWORD);
		
		return FromString<T>(plainText);
	}
	
	public static void ToFile<T>(object objectToSerialize, string fileName)
	{
		// Delete any existing file
		if(File.Exists(fileName))
		{
			File.Delete(fileName);
		}
		
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		
		// Specify null namespace to remove default
		XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
		ns.Add("","");
		
		using(FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
		{
			using(StreamWriter sr = new StreamWriter(fs, Encoding.UTF8))
			{
				serializer.Serialize(sr, objectToSerialize, ns);
			}
		}
	}
	
	public static T FromFile<T>(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		
		using(FileStream fileStream = new FileStream(path, System.IO.FileMode.Open))
		{
			return (T)serializer.Deserialize(fileStream);
		}
	}
	
	public static T FromString<T>(string text)
	{
		try
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			
			using (TextReader reader = new StringReader(text))
			{
				return (T)serializer.Deserialize(reader);
			}
		}
		catch(Exception e)
		{
			Debug.LogError("Serialization.FromString - exception "+ e.ToString());
			return default(T);
		}
	}
	
	public static StringWriter ObjectToStringWriter<T>(T obj)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.OmitXmlDeclaration = true;
		
		// Specify null namespace to remove default
		XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
		ns.Add("","");
		
		StringWriter sw = new StringWriter();
		
		using(XmlWriter xmlWriter = XmlWriter.Create(sw, settings))
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(xmlWriter, obj, ns);
		}
		
		return sw;
	}
	
	public static string ObjectToString<T>(T obj)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.OmitXmlDeclaration = true;
		
		// Specify null namespace to remove default
		XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
		ns.Add("","");
		
		StringWriter sw = new StringWriter();
		
		using(XmlWriter xmlWriter = XmlWriter.Create(sw, settings))
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(xmlWriter, obj, ns);
		}
		
		return sw.ToString();
	}
	
	public static T DeserialiseFromTextAsset<T>(TextAsset textAsset)
	{
		T newObject = FromString<T>(textAsset.text);
		return newObject;
	}
	
	public static Byte[] ToByteArray<T>(T obj)
	{	
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memStream = new MemoryStream();
		binaryFormatter.Serialize(memStream, obj);
		return memStream.ToArray();
	}
	
	public static T FromByteArray<T>(byte[] byteArray)
	{	
		MemoryStream memStream = new MemoryStream(byteArray);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		
		// Set memory stream position to starting point
		memStream.Position = 0;
		
		if(memStream.Length == 0)
		{
			// vjs - not sure about this...
			return default(T);
		}
		
		return (T)binaryFormatter.Deserialize(memStream);	
	}
	
	public static string PrettyPrintXml( string xmlText )
	{
		XmlDocument doc = new XmlDocument();
		doc.LoadXml( xmlText );
		
		var stringBuilder 			= new StringBuilder();
		XmlWriterSettings settings 	= new XmlWriterSettings();
		settings.Indent 			= true;
		settings.IndentChars 		= "\t";
		settings.NewLineChars 		= "\r\n";
		settings.NewLineHandling 	= NewLineHandling.Replace;
		
		using( XmlWriter writer = XmlWriter.Create( stringBuilder, settings ) ) 
		{
			doc.Save( writer );
		}
		
		return stringBuilder.ToString();
	}	
	
	public static string EncryptedFileName( string fileName )
	{
		if( SHOULD_ENCRYPT )
		{
			return Utils.Encrypt( fileName );
		}
		
		return fileName;
	}
#endregion		
	
#region Implementation
	public static string EncryptString( string clearText, string password ) 
	{
		SymmetricAlgorithm algorithm	= GetAlgorithm( password );
		byte[] clearBytes				= Encoding.Unicode.GetBytes( clearText );
		var memoryStream				= new MemoryStream();
		var cryptoStream				= new CryptoStream( memoryStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write );
		cryptoStream.Write( clearBytes, 0, clearBytes.Length );
		cryptoStream.Close();
		
		return Convert.ToBase64String( memoryStream.ToArray() );
	}
	
	private static string DecryptString( string cipherText, string password ) 
	{
		SymmetricAlgorithm algorithm     = GetAlgorithm(password);
		byte[] cipherBytes               = Convert.FromBase64String(cipherText);
		
		using(MemoryStream memoryStream = new MemoryStream())
		{
			CryptoStream cryptoStream = new CryptoStream( memoryStream, algorithm.CreateDecryptor(), CryptoStreamMode.Write );
			cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
			cryptoStream.Close(); 
			
			return Encoding.Unicode.GetString( memoryStream.ToArray() );
		}
	}	
	
	private static SymmetricAlgorithm GetAlgorithm( string password ) 
	{
		SymmetricAlgorithm algorithm 	= Rijndael.Create();
		Rfc2898DeriveBytes deriveBytes	= new Rfc2898DeriveBytes( password, ENCRYPTION_SALT );
		algorithm.Padding   			= PaddingMode.ISO10126;
		algorithm.Key       			= deriveBytes.GetBytes( 32 );
		algorithm.IV        			= deriveBytes.GetBytes( 16 );
		
		return algorithm;
	}
#endregion
}