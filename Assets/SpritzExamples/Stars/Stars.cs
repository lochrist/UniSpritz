using UniMini;
using UnityEngine;

public class Stars : SpritzGame
{
    Vector3[] stars;
    public int count = 1000;
    public float minZ = 1f;
    public float maxZ = 50f;
    public float xSize = 512f;
    public float ySize = 512f;
    public float speed = 50f;

    public override void InitializeSpritz()
    {
        stars = new Vector3[count];
        for (int i = 0; i < stars.Length; i++)
            stars[i] = new Vector3(Random.Range(-xSize, xSize), Random.Range(-ySize, ySize), (float)(stars.Length - i) * maxZ / stars.Length + minZ);
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        for (int i = 0; i < stars.Length; i++)
        {
            int x = (int)(stars[i].x / stars[i].z) + resolution.x / 2;
            int y = (int)(stars[i].y / stars[i].z) + resolution.y / 2;
            byte b = (byte)(255f / stars[i].z);
            var c = b / 255f;

            if (x < resolution.x)
                stars[i] += new Vector3(Time.deltaTime * speed, 0f, 0f);
            else
                stars[i] = new Vector3(-xSize, stars[i].y, stars[i].z);

            if (x >= 0 && x < resolution.x && y >= 0 && y < resolution.y)
                Spritz.DrawPixel(x, y, new Color(c, c, c));
        }
    }
}
