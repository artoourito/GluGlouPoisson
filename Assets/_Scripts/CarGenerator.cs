using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> carPrefabs = new List<GameObject>();
    [SerializeField] private float cd = 1f;
    [SerializeField] private float destroyCarCd = 1f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnCar());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnCar()
    {
        int randomVehicule = Random.Range(0, carPrefabs.Count);
        GameObject newCar = Instantiate(carPrefabs[randomVehicule],  transform.position, Quaternion.identity);
        Destroy(newCar, destroyCarCd);
        yield return new WaitForSeconds(cd);
        StartCoroutine(SpawnCar());
    }

}
