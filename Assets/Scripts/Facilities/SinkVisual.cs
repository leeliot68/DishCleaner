using UnityEngine;

public class SinkVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sink sink;

    [SerializeField] private Sprite emptySinkSprite;
    [SerializeField] private Sprite cleaningSinkSprite;
    [SerializeField] private Sprite cleanedSinkSprite;

    // Update is called once per frame
    void Update()
    {
        switch (sink.getState())
        {
            case SinkState.Empty:
                spriteRenderer.sprite = emptySinkSprite;
                break;
            case SinkState.Washing:
                spriteRenderer.sprite = cleaningSinkSprite;
                break;
            case SinkState.Ready:
                spriteRenderer.sprite = cleanedSinkSprite;
                break;
        }
    }
}
