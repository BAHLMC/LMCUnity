using UnityEngine;
using System.Collections;

public class TranslateObject : MonoBehaviour {

    public GameObject target;
    public Vector2 startLocation;
    public Vector2 endLocation;
    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        target.transform.localPosition = Vector3.Lerp(new Vector3(startLocation.x, startLocation.y, target.transform.position.z), new Vector3(endLocation.x, endLocation.y, target.transform.position.z), Time.time);
	}
}
