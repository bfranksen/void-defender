using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour {

    [SerializeField] float explosionSpeed = 1f;
    [SerializeField] float explosionTime = 1f;

    private IEnumerator Start() {
        yield return new WaitForSeconds(explosionTime);
        Destroy(gameObject);
    }

    private void Update() {
        float explosionExpansionRate = explosionSpeed * Time.deltaTime;
        transform.localScale += new Vector3(explosionExpansionRate, explosionExpansionRate, 0);
    }
}
