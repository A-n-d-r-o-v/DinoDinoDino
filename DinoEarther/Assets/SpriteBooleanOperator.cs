using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ClipperLib;

public class SpriteBooleanOperator : MonoBehaviour {

	[SerializeField]
	private GameObject otherObject; // Game object which is used to modify the collider. This is destroyed.

	[SerializeField]
	private SpriteMask spriteMask; // Hole shape

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer> ().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
	}
	
	// Update is called once per frame
	void Update () {
		if (otherObject != null) {
			PolygonCollider2D otherCollider = otherObject.GetComponent<PolygonCollider2D> ();
			if (otherCollider == null) {
				otherObject.AddComponent<PolygonCollider2D>();
				otherCollider = otherObject.GetComponent<PolygonCollider2D> ();
			}
			removeGeometryFromCollider (otherCollider);
			Instantiate (spriteMask, otherObject.transform.position, otherObject.transform.rotation);
			spriteMask.gameObject.name = "Dino hole";

			Destroy (otherObject);
		}
	}
		
	private void removeGeometryFromCollider(PolygonCollider2D otherCollider) {
		// Vec2 array of this gameobjects poly collider (geometry to modify)
		Vector2[] thisColliderPointArr = GetComponent<PolygonCollider2D> ().points;
		Vector2 positionOffset = otherCollider.gameObject.transform.position - transform.position;
		Vector2[] otherColliderPointArr = otherCollider.points;//sum(otherCollider.points, positionOffset);

		// Vec2 array of the other gameobjects poly collider (hole geometry)
		Vector2[] thisScreenSpaceArr = toScreenSpace (thisColliderPointArr, GetComponent<PolygonCollider2D> ());
		Vector2[] otherScreenSpaceArr = toScreenSpace (otherColliderPointArr, otherCollider);

		List<IntPoint> thisList = toIntPointList (thisScreenSpaceArr);
		List<IntPoint> otherList = toIntPointList (otherScreenSpaceArr);

		List<List<IntPoint>> solution = new List<List<IntPoint>> ();
		Clipper clipper = new Clipper ();
		clipper.AddPath (thisList, PolyType.ptSubject, true);
		clipper.AddPath (otherList, PolyType.ptClip, true);
		clipper.Execute (ClipType.ctDifference, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

		Vector2[] solutionVec2Arr = toVec2Arr (solution[0]);
		Debug.Log (solution.Count);
		GetComponent<PolygonCollider2D> ().points = toWorldSpace(solutionVec2Arr, GetComponent<PolygonCollider2D> ());
	}

	// TODO overload operator + in extension class
	public static Vector2[] sum(Vector2[] arr, Vector2 value) {
		Vector2[] result = new Vector2[arr.Length];
		for (int i = 0; i < arr.Length; i++) {
			result [i] = arr [i] + value;
		}
		return result;
	}

	// Note: convert to screen space
	private List<IntPoint> toIntPointList(Vector2[] arr) {
		List<IntPoint> newList = new List<IntPoint>();
		foreach (Vector2 point in arr) {
			newList.Add (new IntPoint((int)point.x, (int)point.y));
		}
		return newList;
	}

	private Vector2[] toScreenSpace(Vector2[] arr, PolygonCollider2D col) {
		Vector2[] newArr = new Vector2[arr.Length];
		for (int i = 0; i < arr.Length; i++) {
			if (i % 40 == 0) {
				Debug.Log ("=====================");
				Debug.Log (arr [i]);
				Debug.Log(Camera.main.WorldToScreenPoint (arr [i]));
				Debug.Log (Camera.main.WorldToScreenPoint (col.transform.TransformPoint(arr [i])));
				Debug.Log ("=====================");
			}
			newArr [i] = Camera.main.WorldToScreenPoint (col.transform.TransformPoint(arr [i]));
				//Camera.main.WorldToScreenPoint(arr [i]);
		}
		return newArr;
	}

	private Vector2[] toWorldSpace(Vector2[] arr, PolygonCollider2D col) {
		Vector2[] newArr = new Vector2[arr.Length];
		for (int i = 0; i < arr.Length; i++) {
			newArr [i] = col.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(arr [i]));
		}
		return newArr;
	}

	// Note: convert to world
	private Vector2[] toVec2Arr(List<IntPoint> list) {
		Vector2[] newArr = new Vector2[list.Count];
		for (int i = 0; i < list.Count; i++) {
			newArr [i] = new Vector2(list[i].X, list[i].Y);
		}
		return newArr;
	}
		
}
