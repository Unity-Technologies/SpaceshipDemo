using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshMerger
{
    [MenuItem("GameObject/Merge Selection to single Mesh", priority = 2000)]
    static void MergeSelection()
    {
        List<CombineInstance> combine = new List<CombineInstance>();

        if (Selection.gameObjects.Length > 0)
        {
            foreach (var go in Selection.gameObjects)
            {
                var local = go.transform.worldToLocalMatrix;

                MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>();
                foreach (var f in filters)
                {
                    CombineInstance c = new CombineInstance();
                    c.mesh = f.sharedMesh;
                    c.transform = local * f.gameObject.transform.localToWorldMatrix ;
                    combine.Add(c);
                }
            }
        }

        Mesh m = new Mesh();
        m.CombineMeshes(combine.ToArray(), true);
        m.RecalculateBounds();

        string filename = EditorUtility.SaveFilePanelInProject("Save Mesh", "MergedMesh", "mesh", "Save Mesh");

        if (filename != string.Empty)
        {
            AssetDatabase.CreateAsset(m, filename);
            AssetDatabase.ImportAsset(filename);
        }

    }

}
