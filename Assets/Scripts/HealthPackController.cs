using System.Collections;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    public GameObject cube;
    public GameObject effect;
    public bool Active => cube.activeSelf;

    // Start is called before the first frame update
    void Start()
    {

    }

    IEnumerator CoroutineSetAvailableAfter(float seconds) {
        this.cube.SetActive(false);
        this.effect.SetActive(false);
        yield return new WaitForSeconds(seconds);
        this.cube.SetActive(true);
        this.effect.SetActive(true);
    }

    public void SetAvailableAfter(float seconds)
    {
        StartCoroutine(this.CoroutineSetAvailableAfter(seconds));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
