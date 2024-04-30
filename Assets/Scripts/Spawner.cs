using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Rigidbody _prefab;
    [SerializeField] private Camera _camera;
    [SerializeField] private Cube _cube;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionForce;

    private const string Fire1 = nameof(Fire1);

    private Ray _ray;
    private Transform _transform;
    private int _maxPercentProbability = 100;

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown(Fire1) & Physics.Raycast(_ray, out hit, Mathf.Infinity))
        {
            _transform = hit.transform;

            if (hit.transform.GetComponent(_cube.GetType()))
            {
                Destroy(hit.transform.gameObject);

                if (TrySatisfy())
                {
                    Spawn();
                    DecreaseProbability();
                }

                Explode();
            }
        }
    }

    private void Spawn()
    {
        Rigidbody clone;
        float positionY = 0;
        float angleDegreesX = 0;
        float angleDegreesZ = 0;
        float radiusInstantiation = 0.3f;
        int minNumberObjects = 2;
        int maxNumberObjects = 6;
        int multiplier = 2;
        int numberObjects = Random.Range(minNumberObjects, maxNumberObjects);

        for (int i = 0; i < numberObjects; i++)
        {
            float angle = i * Mathf.PI * multiplier / numberObjects;
            float positionX = Mathf.Cos(angle) * radiusInstantiation;
            float positionZ = Mathf.Sin(angle) * radiusInstantiation;
            Vector3 position = _transform.position + new Vector3(positionX, positionY, positionZ);
            float angleDegreesY = -angle * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(angleDegreesX, angleDegreesY, angleDegreesZ);
            clone = Instantiate(_prefab, position, rotation);
            clone.transform.localScale = _transform.localScale;
            Cube cube = clone.GetComponent<Cube>();
            cube.ChangeScale();
            cube.ChangeColor();
        }
    }

    private void Explode()
    {
        int multiplier = 100;
        float explosionForce = _explosionForce + _prefab.transform.localScale.x / _transform.localScale.x * multiplier;
        float explosionRadius = _explosionRadius * _prefab.transform.localScale.x / _transform.localScale.x;

        foreach (Rigidbody explodableCube in GetExplodableObjects())
        {
            explodableCube.AddExplosionForce(explosionForce, _transform.position, explosionRadius);
        }
    }

    private bool TrySatisfy()
    {
        int minNumber = 0;
        int maxNumber = 100;
        return Random.Range(minNumber, maxNumber) <= _maxPercentProbability;
    }

    private void DecreaseProbability()
    {
        int reduceNumber = 2;
        _maxPercentProbability /= reduceNumber;
    }

    private List<Rigidbody> GetExplodableObjects()
    {
        Collider[] hits = Physics.OverlapSphere(_transform.position, _explosionRadius);
        List<Rigidbody> cubes = new();

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody != null)
            {
                cubes.Add(hit.attachedRigidbody);
            }
        }

        return cubes;
    }
}