using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textMesh;
    public float timeTillDisappear = 1f;
    public float popUpMaxJumpHeight = 0.4f;
    private float popUpJumpHeight;
    [Range(0f, 1f)]
    public float targetXRandom;
    [Range(0f, 1f)]
    public float targetYRandom;
    private float disappearTimer;
    private Color textColor;

    protected float parabolaAnimation;
    protected Vector3 targetPosition;

    private void Awake()
    {
        textMesh = gameObject.GetComponentInChildren<TextMeshPro>();
        disappearTimer = timeTillDisappear;
    }

    public void SetUp(int damage, Vector3 direction)
    {
        textMesh.SetText(damage.ToString());
        textMesh.fontSize = 8;

        textColor = textMesh.color;

        targetPosition = transform.position + direction;
        float randomX = Random.Range(-targetXRandom, targetXRandom);
        float randomY = Random.Range(targetYRandom, targetYRandom);
        targetPosition += new Vector3(randomX, randomY);

        popUpJumpHeight = Random.Range(0f, popUpMaxJumpHeight);
    }

    private void Update()
    {
        disappearTimer -= Time.deltaTime;
        parabolaAnimation += Time.deltaTime;

        transform.position = MathParabola.Parabola(transform.position, targetPosition, popUpJumpHeight, parabolaAnimation /2f);

        if (disappearTimer < 0)
        {
            // Start disappear
            float disappearSpeed = 1.1f;
            textColor.a /= disappearSpeed;
            textMesh.color = textColor;
            if (textColor.a < 0.1)
            {
                Destroy(gameObject);
            }
        }
    }
}
