using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[UnityEditor.AssetImporters.ScriptedImporter(0, "wire")]
public class WireMeshImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    static char[] separators = new char[] { ' ' };

    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        string[] lines = File.ReadAllLines(ctx.assetPath);

        if (lines.Length > 0 && lines[0] == "WIREMESH")
        {
            Mesh m = new Mesh();

            if (lines.Length > 1)
            {
                List<Vector3> vertices = new List<Vector3>();
                int[] indices = new int[(lines.Length - 1) * 2];
                

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(separators);

                    if (values.Length != 6)
                        throw new System.IO.InvalidDataException("Invalid wire data length");

                    float[] linedata = new float[6];
                    for (int j = 0; j < 6; j++)
                        linedata[j] = float.Parse(values[j], System.Globalization.NumberStyles.Float ,System.Globalization.CultureInfo.InvariantCulture);

                    vertices.Add(new Vector3(linedata[0], linedata[1], linedata[2]));
                    vertices.Add(new Vector3(linedata[3], linedata[4], linedata[5]));

                    indices[(i - 1) * 2] = (i - 1) * 2;
                    indices[(i - 1) * 2 + 1] = (i - 1) * 2 + 1;
                }

                m.SetVertices(vertices);
                m.SetIndices(indices, MeshTopology.Lines, 0, true);
            }
            else
                Debug.LogWarning("Empty Mesh");

            m.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
            ctx.AddObjectToAsset("Mesh", m);
            ctx.SetMainObject(m);
        }
        else throw new System.IO.InvalidDataException("Invalid File!");

    }


}
