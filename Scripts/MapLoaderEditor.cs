using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MapLoader))]
public class MapLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapLoader script = (MapLoader)target;
        if (GUILayout.Button("Générer les points dans la scène"))
        {
            script.BakePoints(true); 
        }
    }
}
#endif