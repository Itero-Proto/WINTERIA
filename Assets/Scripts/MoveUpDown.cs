using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    public float minY = -2f;
    public float maxY = 2f;

    public float fallSpeed = 1f;      // скорость плавного опускания
    public float riseSpeed = 10f;     // скорость резкого подъёма

    private float startX;
    private float startZ;
    private float currentY;

    private bool goingDown = true;

    void Start()
    {
        startX = transform.position.x;
        startZ = transform.position.z;
        currentY = maxY; // начинаем сверху
    }

    void Update()
    {
        if (goingDown)
        {
            currentY -= fallSpeed * Time.deltaTime;

            if (currentY <= minY)
            {
                currentY = minY;
                goingDown = false;
            }
        }
        else
        {
            currentY += riseSpeed * Time.deltaTime;

            if (currentY >= maxY)
            {
                currentY = maxY;
                goingDown = true;
            }
        }

        transform.position = new Vector3(startX, currentY, startZ);
    }
}