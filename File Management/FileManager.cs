using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Revision 1.00 //
/// Author: <see href="https://github.com/tari-cat/UnityStuff"/>
/// 
/// <para><seealso cref="FileManager"/> is my solution to saving player data, and various things. All files are encrypted in my own format, Invisible Ink (.iink).</para>
/// <para>Persistent files are saved in the <seealso cref="Application.persistentDataPath"/> directory, and only need a file name.</para>
/// <para>Note that the extension is not automatically included in the file name, and must be provided.</para>
/// </summary>
public class FileManager
{
    /// <summary>
    /// Check if a file in the <seealso cref="Application.persistentDataPath"/> directory exists.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    public static bool PersistentFileExists(string fileName)
    {
        return File.Exists(Application.persistentDataPath + "\\" + fileName.Replace('/', '\\'));
    }

    /// <summary>
    /// Check if a file exists.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    public static bool FileExists(string fileName)
    {
        return File.Exists(fileName.Replace('/', '\\'));
    }

    /// <summary>
    /// Save a class to the <seealso cref="Application.persistentDataPath"/> directory with a file name. Files stored here are saved across runs.
    /// </summary>
    public static bool SaveAsPersistentFile(string fileName, object obj)
    {
        return SaveToFile(Application.persistentDataPath + "\\" + fileName, obj);
    }

    /// <summary>
    /// Save a class to the specified directory.
    /// <para>Don't use this for player save data. Use <seealso cref="SaveAsPersistentFile(string, object)"/> instead.</para>
    /// </summary>
    public static bool SaveToFile(string fileName, object obj)
    {
        string json = JsonUtility.ToJson(obj);
        Debug.Log("Saving json to file " + fileName + ": " + json);
        return WriteToFile(fileName, json);
    }

    /// <summary>
    /// Read a class object from the <seealso cref="Application.persistentDataPath"/> directory with a file name and object type.
    /// </summary>
    public static object ReadAsPersistentFile(string fileName, Type type)
    {
        return ReadFromFile(Application.persistentDataPath + "\\" + fileName, type);
    }

    /// <summary>
    /// Read a class object from the <seealso cref="Application.persistentDataPath"/> directory with a file name. Give the class type as a generic type parameter.
    /// Files stored here are persistent across runs.
    /// </summary>
    public static T ReadAsPersistentFile<T>(string fileName)
    {
        return ReadFromFile<T>(Application.persistentDataPath + "\\" + fileName);
    }

    /// <summary>
    /// Read a class object from the specified file name and object type.
    /// </summary>
    public static object ReadFromFile(string fileName, Type type)
    {
        string json = ReadFromFile(fileName);
        Debug.Log("Loading json from file " + fileName + ": " + json);
        return JsonUtility.FromJson(json, type);
    }

    /// <summary>
    /// Read a class object from the specified file name given a type parameter.
    /// </summary>
    public static T ReadFromFile<T>(string fileName)
    {
        string json = ReadFromFile(fileName);
        Debug.Log("Loading json from file " + fileName + ": " + json);
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// Delete a persistent file given a file name and path.
    /// </summary>
    /// <param name="fileName">Persistent file name and path.</param>
    public static void DeletePersistentFile(string fileName)
    {
        if (PersistentFileExists(fileName))
        {
            File.Delete(Application.persistentDataPath + "\\" + fileName.Replace('/', '\\'));
        }
    }

    /// <summary>
    /// Delete a file given a file name and path.
    /// </summary>
    /// <param name="fileName">File name and path.</param>
    public static void DeleteFile(string fileName)
    {
        if (FileExists(fileName))
        {
            File.Delete(fileName.Replace('/', '\\'));
        }
    }

    /// <summary>
    /// Overwrite an object's data with given file's data. Useful for components.
    /// </summary>
    public static void Overwrite(string fileName, object obj)
    {
        string json = ReadFromFile(fileName);
        Debug.Log("Overwriting object with json from file " + fileName + ": " + json);
        JsonUtility.FromJsonOverwrite(json, obj);
    }

    /// <summary>
    /// Overwrite an object's data with given file's data. Useful for components.
    /// </summary>
    public static void OverwriteFromPersistentFile(string fileName, object obj)
    {
        string json = ReadFromFile(Application.persistentDataPath + "\\" + fileName);
        Debug.Log("Overwriting object with json from file " + fileName + ": " + json);
        JsonUtility.FromJsonOverwrite(json, obj);
    }

    // The code below this line is used to encrypt and decrypt InvisibleInk (.iink, my save file format.)

    //! Base64 decode method. Uses System Convert and encoding UTF8.
    private static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    //! Base64 encode method. Uses System Convert and encoding UTF8.
    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    //! Write to file internal. Replaces '/' with '\\'. Also creates any directories that may not exist.
    private static bool WriteToFile(string writeTo, string encode)
    {
        try
        {
            writeTo = writeTo.Replace('/', '\\');
            Directory.CreateDirectory(writeTo.Substring(0, writeTo.LastIndexOf('\\')));
            StreamWriter file = new StreamWriter(writeTo);
            file.Write(Encode(encode));
            file.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("FileManager.cs cannot write to " + writeTo + ". \n" + e.ToString());
            return false;
        }
    }

    //! Read from file internal. Replaces '/' with '\\'.
    private static string ReadFromFile(string readFrom)
    {
        try
        {
            readFrom = readFrom.Replace('/', '\\');
            StreamReader file = new StreamReader(readFrom);
            string decoded = Decode(file.ReadToEnd());
            file.Close();

            return decoded;
        }
        catch (Exception e)
        {
            Debug.LogError("FileManager.cs cannot read from " + readFrom + ". \n" + e.ToString());
            return null;
        }
    }

    //! Shortcut for encrypt.
    private static string Encode(string toEncode)
    {
        return Encrypt(toEncode);
    }

    //! Encrypts a given (preferrably .json) string. The string is encoded in base64, then we replace every character with its zero-width equivalent.
    private static string Encrypt(string encrypt)
    {
        encrypt = Base64Encode(encrypt);
        string encoded = "";
        foreach (char i in encrypt)
            encoded += zeroWidth[i];
        return encoded;
    }

    //! Decrypts a given .iink string. The string is encoded in base64, on top of a layer of characters with an offset of 56320. We ignore the delimiter unicode char.
    private static string Decode(string toDecode)
    {
        string decoded = "";
        for (int i = 0; i < toDecode.Length; i += 2)
            decoded += (char)(toDecode[i + 1] - 56320);
        return Base64Decode(decoded);
    }

    //! Somehow, making an array of chars for this was hard because of how unicode wants to work. So we just make an array of all of them. Probably a better way to do this. Hacky.
    private static readonly string[] zeroWidth =
    {
            "\udb40\udc00",
            "\udb40\udc01",
            "\udb40\udc02",
            "\udb40\udc03",
            "\udb40\udc04",
            "\udb40\udc05",
            "\udb40\udc06",
            "\udb40\udc07",
            "\udb40\udc08",
            "\udb40\udc09",
            "\udb40\udc0a",
            "\udb40\udc0b",
            "\udb40\udc0c",
            "\udb40\udc0d",
            "\udb40\udc0e",
            "\udb40\udc0f",
            "\udb40\udc10",
            "\udb40\udc11",
            "\udb40\udc12",
            "\udb40\udc13",
            "\udb40\udc14",
            "\udb40\udc15",
            "\udb40\udc16",
            "\udb40\udc17",
            "\udb40\udc18",
            "\udb40\udc19",
            "\udb40\udc1a",
            "\udb40\udc1b",
            "\udb40\udc1c",
            "\udb40\udc1d",
            "\udb40\udc1e",
            "\udb40\udc1f",
            "\udb40\udc20",
            "\udb40\udc21",
            "\udb40\udc22",
            "\udb40\udc23",
            "\udb40\udc24",
            "\udb40\udc25",
            "\udb40\udc26",
            "\udb40\udc27",
            "\udb40\udc28",
            "\udb40\udc29",
            "\udb40\udc2a",
            "\udb40\udc2b",
            "\udb40\udc2c",
            "\udb40\udc2d",
            "\udb40\udc2e",
            "\udb40\udc2f",
            "\udb40\udc30",
            "\udb40\udc31",
            "\udb40\udc32",
            "\udb40\udc33",
            "\udb40\udc34",
            "\udb40\udc35",
            "\udb40\udc36",
            "\udb40\udc37",
            "\udb40\udc38",
            "\udb40\udc39",
            "\udb40\udc3a",
            "\udb40\udc3b",
            "\udb40\udc3c",
            "\udb40\udc3d",
            "\udb40\udc3e",
            "\udb40\udc3f",
            "\udb40\udc40",
            "\udb40\udc41",
            "\udb40\udc42",
            "\udb40\udc43",
            "\udb40\udc44",
            "\udb40\udc45",
            "\udb40\udc46",
            "\udb40\udc47",
            "\udb40\udc48",
            "\udb40\udc49",
            "\udb40\udc4a",
            "\udb40\udc4b",
            "\udb40\udc4c",
            "\udb40\udc4d",
            "\udb40\udc4e",
            "\udb40\udc4f",
            "\udb40\udc50",
            "\udb40\udc51",
            "\udb40\udc52",
            "\udb40\udc53",
            "\udb40\udc54",
            "\udb40\udc55",
            "\udb40\udc56",
            "\udb40\udc57",
            "\udb40\udc58",
            "\udb40\udc59",
            "\udb40\udc5a",
            "\udb40\udc5b",
            "\udb40\udc5c",
            "\udb40\udc5d",
            "\udb40\udc5e",
            "\udb40\udc5f",
            "\udb40\udc60",
            "\udb40\udc61",
            "\udb40\udc62",
            "\udb40\udc63",
            "\udb40\udc64",
            "\udb40\udc65",
            "\udb40\udc66",
            "\udb40\udc67",
            "\udb40\udc68",
            "\udb40\udc69",
            "\udb40\udc6a",
            "\udb40\udc6b",
            "\udb40\udc6c",
            "\udb40\udc6d",
            "\udb40\udc6e",
            "\udb40\udc6f",
            "\udb40\udc70",
            "\udb40\udc71",
            "\udb40\udc72",
            "\udb40\udc73",
            "\udb40\udc74",
            "\udb40\udc75",
            "\udb40\udc76",
            "\udb40\udc77",
            "\udb40\udc78",
            "\udb40\udc79",
            "\udb40\udc7a",
            "\udb40\udc7b",
            "\udb40\udc7c",
            "\udb40\udc7d",
            "\udb40\udc7e",
            "\udb40\udc7f",
            "\udb40\udc80",
            "\udb40\udc81",
            "\udb40\udc82",
            "\udb40\udc83",
            "\udb40\udc84",
            "\udb40\udc85",
            "\udb40\udc86",
            "\udb40\udc87",
            "\udb40\udc88",
            "\udb40\udc89",
            "\udb40\udc8a",
            "\udb40\udc8b",
            "\udb40\udc8c",
            "\udb40\udc8d",
            "\udb40\udc8e",
            "\udb40\udc8f",
            "\udb40\udc90",
            "\udb40\udc91",
            "\udb40\udc92",
            "\udb40\udc93",
            "\udb40\udc94",
            "\udb40\udc95",
            "\udb40\udc96",
            "\udb40\udc97",
            "\udb40\udc98",
            "\udb40\udc99",
            "\udb40\udc9a",
            "\udb40\udc9b",
            "\udb40\udc9c",
            "\udb40\udc9d",
            "\udb40\udc9e",
            "\udb40\udc9f",
            "\udb40\udca0",
            "\udb40\udca1",
            "\udb40\udca2",
            "\udb40\udca3",
            "\udb40\udca4",
            "\udb40\udca5",
            "\udb40\udca6",
            "\udb40\udca7",
            "\udb40\udca8",
            "\udb40\udca9",
            "\udb40\udcaa",
            "\udb40\udcab",
            "\udb40\udcac",
            "\udb40\udcad",
            "\udb40\udcae",
            "\udb40\udcaf",
            "\udb40\udcb0",
            "\udb40\udcb1",
            "\udb40\udcb2",
            "\udb40\udcb3",
            "\udb40\udcb4",
            "\udb40\udcb5",
            "\udb40\udcb6",
            "\udb40\udcb7",
            "\udb40\udcb8",
            "\udb40\udcb9",
            "\udb40\udcba",
            "\udb40\udcbb",
            "\udb40\udcbc",
            "\udb40\udcbd",
            "\udb40\udcbe",
            "\udb40\udcbf",
            "\udb40\udcc0",
            "\udb40\udcc1",
            "\udb40\udcc2",
            "\udb40\udcc3",
            "\udb40\udcc4",
            "\udb40\udcc5",
            "\udb40\udcc6",
            "\udb40\udcc7",
            "\udb40\udcc8",
            "\udb40\udcc9",
            "\udb40\udcca",
            "\udb40\udccb",
            "\udb40\udccc",
            "\udb40\udccd",
            "\udb40\udcce",
            "\udb40\udcd0",
            "\udb40\udcd1",
            "\udb40\udcd2",
            "\udb40\udcd3",
            "\udb40\udcd4",
            "\udb40\udcd5",
            "\udb40\udcd6",
            "\udb40\udcd7",
            "\udb40\udcd8",
            "\udb40\udcd9",
            "\udb40\udcda",
            "\udb40\udcdb",
            "\udb40\udcdc",
            "\udb40\udcdd",
            "\udb40\udcde",
            "\udb40\udcdf",
            "\udb40\udcdf",
            "\udb40\udce0",
            "\udb40\udce1",
            "\udb40\udce2",
            "\udb40\udce3",
            "\udb40\udce4",
            "\udb40\udce5",
            "\udb40\udce6",
            "\udb40\udce7",
            "\udb40\udce8",
            "\udb40\udce9",
            "\udb40\udcea",
            "\udb40\udceb",
            "\udb40\udcec",
            "\udb40\udced",
            "\udb40\udcee",
            "\udb40\udcef",
            "\udb40\udcef",
            "\udb40\udcf0",
            "\udb40\udcf1",
            "\udb40\udcf2",
            "\udb40\udcf3",
            "\udb40\udcf4",
            "\udb40\udcf5",
            "\udb40\udcf6",
            "\udb40\udcf7",
            "\udb40\udcf8",
            "\udb40\udcf9",
            "\udb40\udcfa",
            "\udb40\udcfb",
            "\udb40\udcfc",
            "\udb40\udcfd",
            "\udb40\udcfe",
            "\udb40\udcff",
            "\udb40\udcff",
        };
}