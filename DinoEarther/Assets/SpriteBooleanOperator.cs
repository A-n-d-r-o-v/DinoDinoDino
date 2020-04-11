using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ClipperLib;

public class SpriteBooleanOperator : MonoBehaviour {

	[SerializeField]
	[Tooltip("The object which is used to modify the collider. This is automatically destroyed upon creating the hole.")]
	private GameObject otherObject;

	[SerializeField]
	[Tooltip("The sprite mask containing the shape of the hole")]
	private SpriteMask spriteMask;

	void Start () {
		GetComponent<SpriteRenderer> ().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
	}
	
	void Update () {
		// Check for other object. If it exists, remove its geometry from this object (boolean difference), and remove it from the game.
		if (otherObject != null) {
			PolygonCollider2D otherCollider = otherObject.GetComponent<PolygonCollider2D> ();
			if (otherCollider == null) {
				otherObject.AddComponent<PolygonCollider2D>();
				otherCollider = otherObject.GetComponent<PolygonCollider2D> ();
			}
			RemoveGeometryFromCollider (otherCollider);
			Instantiate (spriteMask, otherObject.transform.position, otherObject.transform.rotation);
			spriteMask.gameObject.name = "Dino hole";

			Destroy (otherObject);
		}
	}

	// Removes the geometry of otherCollider from the collider of the object this script is attached to by setting the points of this objects
	// PolygonCollider2D to the resulting path of the boolean difference of (this - other), where other is an arbitrary PolygonCollider.
	private void RemoveGeometryFromCollider(PolygonCollider2D otherCollider) {
		// Vec2 array of this gameobjects poly collider (geometry to modify)
		Vector2[] thisColliderPointArr = GetComponent<PolygonCollider2D> ().points;
		Vector2 positionOffset = otherCollider.gameObject.transform.position - transform.position;
		Vector2[] otherColliderPointArr = otherCollider.points;//sum(otherCollider.points, positionOffset);

		// Vec2 array of the other gameobjects poly collider (hole geometry)
		Vector2[] thisScreenSpaceArr = ToGlobalScreenSpace (thisColliderPointArr, GetComponent<PolygonCollider2D> ());
		Vector2[] otherScreenSpaceArr = ToGlobalScreenSpace (otherColliderPointArr, otherCollider);

		// Convert Vector2 to IntPoint
		List<IntPoint> thisList = ToIntPointList (thisScreenSpaceArr);
		List<IntPoint> otherList = ToIntPointList (otherScreenSpaceArr);

		List<List<IntPoint>> solution = new List<List<IntPoint>> (); // Solution contains lists of regions. Each region consists of an IntPoint list
		Clipper clipper = new Clipper ();
		clipper.AddPath (thisList, PolyType.ptSubject, true);
		clipper.AddPath (otherList, PolyType.ptClip, true);
		clipper.Execute (ClipType.ctDifference, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd); // Perform boolean difference and store the resulting clip in solution

		Vector2[] solutionVec2Arr = ToVec2Arr (solution[0]);
		GetComponent<PolygonCollider2D> ().points = ToLocalWorldSpace(solutionVec2Arr, GetComponent<PolygonCollider2D> ());
	}

	// Note: rounds values to nearest int
	private List<IntPoint> ToIntPointList(Vector2[] arr) {
		List<IntPoint> newList = new List<IntPoint>();
		foreach (Vector2 point in arr) {
			newList.Add (new IntPoint((int)point.x, (int)point.y));
		}
		return newList;
	}

	private Vector2[] ToVec2Arr(List<IntPoint> list) {
		Vector2[] newArr = new Vector2[list.Count];
		for (int i = 0; i < list.Count; i++) {
			newArr [i] = new Vector2(list[i].X, list[i].Y);
		}
		return newArr;
	}

	private Vector2[] ToGlobalScreenSpace(Vector2[] arr, PolygonCollider2D col) {
		Vector2[] newArr = new Vector2[arr.Length];
		for (int i = 0; i < arr.Length; i++) {
			newArr [i] = Camera.main.WorldToScreenPoint (col.transform.TransformPoint(arr [i]));
		}
		return newArr;
	}

	private Vector2[] ToLocalWorldSpace(Vector2[] arr, PolygonCollider2D col) {
		Vector2[] newArr = new Vector2[arr.Length];
		for (int i = 0; i < arr.Length; i++) {
			newArr [i] = col.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(arr [i]));
		}
		return newArr;
	}
		
}
