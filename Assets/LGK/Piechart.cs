using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class Piechart : MonoBehaviour
{

	[System.Serializable]
	public struct Datum
	{
		public string name;
		public float value;
		public Color color;
	}

	public Datum[] data;
	// Start is called before the first frame update
	public Mesh mesh;



	public float stepsPerRotation = 360;

	Vector2 AngleVector(float angle) => new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

	void BuildMesh()
	{

		var total = data.Sum(x => x.value);

		if (total == 0)
			throw new System.DataMisalignedException("0 total data");


		float startAngle = 0;


		var verts = new Vector3[data.Length * 4];
		var tris = new int[data.Length * 4];
		var colors = new Color32[data.Length * 4];

		var baseIndex = 0;
		foreach (var d in data)
		{
			var pvalue = d.value / total;
			var angleCovered = 2 * Mathf.PI * pvalue;


			var a = AngleVector(startAngle);
			var b = AngleVector(startAngle + angleCovered);


			var c = AngleVector(startAngle + angleCovered/2);

			var closestPoint = ((a + c) / 2).magnitude;

			verts[baseIndex + 0] = Vector3.zero;
			verts[baseIndex + 1] = a/ closestPoint;
			verts[baseIndex + 2] = c/ closestPoint;
			verts[baseIndex + 3] = b/ closestPoint;


			var col = (Color32)d.color;


			for (int i = 0; i < 4; i++)
			{
				colors[baseIndex + i] = col;
				tris[baseIndex + i] = baseIndex + i;
			}

			startAngle += angleCovered;
			baseIndex += 4;
		}
		mesh.vertices = verts;
		mesh.colors32 = colors.ToArray();
		mesh.SetIndices(tris, MeshTopology.Quads, 0);
		mesh.UploadMeshData(false);
	}

	void Start()
	{
		if (!mesh)
		{
			mesh = new Mesh();
			var filter = GetComponent<MeshFilter>();
			if (filter)
				filter.mesh = mesh;
		}
	}

	// Update is called once per frame
	void Update()
	{
		BuildMesh();
	}
}
