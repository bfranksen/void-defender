using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [SerializeField] float backgroundScrollSpeed = 0.15f;
    Material material;
    Vector2 offset;

    // Start is called before the first frame update
    private void Start() {
        material = GetComponent<Renderer>().material;
        offset = new Vector2(0f, backgroundScrollSpeed);
    }

    // Update is called once per frame
    private void Update() {
        material.mainTextureOffset += offset * Time.deltaTime;
    }
}
