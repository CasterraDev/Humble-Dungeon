using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building Database", menuName = "Building System/Database")]
public class BuildableObjectDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public BuildableObject[] buildableObjects;
    public Dictionary<int, BuildableObject> GetBuildableObject = new Dictionary<int, BuildableObject>();

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < buildableObjects.Length; i++)
        {
            buildableObjects[i].Id = i;
            GetBuildableObject.Add(i, buildableObjects[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        GetBuildableObject = new Dictionary<int, BuildableObject>();
    }
}
