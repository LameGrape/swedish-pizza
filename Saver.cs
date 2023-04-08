using UnityEngine;
using BepInEx;
using System.IO;
using System.Collections.Generic;

public class Saver
{
    static Hotbar hotbar = GameObject.FindObjectOfType<Hotbar>();
    public static List<string> names = new List<string>(){
        "P_Conveyor Belt_Short",
        "P_Conveyor Belt_Long",
        "P_Conveyor Belt_Up",
        "P_Conveyor Belt_Down",
        "P_Conveyor Belt_Left",
        "P_Conveyor Belt_Right",
        "P_Pusher",
        "P_Lift",
        "P_Catapult",
        "P_Funnel"
    };

    static void Format(GameObject[] objects, string path)
    {
        File.WriteAllText(path, string.Empty);
        Stream stream = new FileStream(path, FileMode.OpenOrCreate);
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write((byte)1);
        writer.Write(((ushort)objects.Length));
        foreach (GameObject obj in objects)
        {
            writer.Write((byte)names.IndexOf(obj.name.Substring(0, obj.name.Length - 7)));
            writer.Write(obj.transform.position.x);
            writer.Write(obj.transform.position.y);
            writer.Write(obj.transform.position.z);
            writer.Write(obj.transform.eulerAngles.y);
            writer.Write(obj.GetComponent<Ownership>().myItem.itemCost);
        }
        writer.Close();
        writer.Dispose();
    }

    static void Read(string path)
    {
        Stream stream = new FileStream(path, FileMode.Open);
        BinaryReader reader = new BinaryReader(stream);
        byte version = reader.ReadByte();
        ushort objectCount = reader.ReadUInt16();
        Debug.Log($"Version {version}, Object Count: {objectCount}");
        for (int i = 0; i < objectCount; i++)
        {
            byte index = reader.ReadByte();
            GameObject obj = hotbar.GetRandomItem(index).itemObject;
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float rotation = reader.ReadSingle();
            int cost = reader.ReadInt32();
            GameObject newObj = GameObject.Instantiate(obj, new Vector3(x, y, z), Quaternion.Euler(0, rotation, 0));
            Ownership own = newObj.AddComponent<Ownership>();
            Item item = new Item();
            item.itemObject = newObj;
            item.itemCost = cost;
            own.myItem = item;
        }
        stream.Close();
        stream.Dispose();
    }

    public static void Save(string saveName)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
        }
        Ownership[] owned = GameObject.FindObjectsOfType<Ownership>();
        GameObject[] objects = new GameObject[owned.Length];
        for (int i = 0; i < owned.Length; i++)
        {
            objects[i] = owned[i].gameObject;
        }
        string path = $"{Application.persistentDataPath}/Saves/{saveName}.post";
        Format(objects, path);
        Debug.Log($"Saved game to {path}");
    }

    public static void Load(string saveName)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
        }
        string path = $"{Application.persistentDataPath}/Saves/{saveName}.post";
        if (!File.Exists(path)) return;
        Read(path);
        Debug.Log($"Loaded game from {path}");
    }
}