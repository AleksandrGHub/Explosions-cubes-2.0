using UnityEngine;

[RequireComponent(typeof(Renderer))]

public class Cube : MonoBehaviour
{
    private float _divider = 2;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void ChangeColor()
    {
        _renderer.material.color = Random.ColorHSV();
    }

    public void ChangeScale()
    {
        transform.localScale /= _divider;
    }
}