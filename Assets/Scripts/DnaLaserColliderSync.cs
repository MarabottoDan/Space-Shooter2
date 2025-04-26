using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DnaLaserColliderSync : MonoBehaviour
{
    private BoxCollider2D _collider;
    private float _baseHeight;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _baseHeight = _collider.size.y;
    }

    void Update()
    {
        float currentYScale = transform.localScale.y;

        _collider.size = new Vector2(_collider.size.x, _baseHeight * currentYScale);
        _collider.offset = new Vector2(0, (_baseHeight * currentYScale) / 2f);
    }
}
