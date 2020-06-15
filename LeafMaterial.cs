using Godot;
using System;

public class LeafMaterial : MeshInstance
{
    public static OpenSimplexNoise noise = new OpenSimplexNoise();
    public static float NoiseScale = 0.6f;
    public static float NoisePeriod = 1.5f;
    public static float RotateAngle = 2 * Mathf.Pi / 256;

    public static Vector2 CalculateUV(Vector3 position) {
        float u = 0.5f - Mathf.Atan2(-position.z, position.x) / (2.0f * Mathf.Pi);
        float v = 0.5f - Mathf.Asin(position.y) / Mathf.Pi;
        return new Vector2(u, v);
    }

    public static Vector3 CalculateCoordinate(Vector2 uv) {
        float u = uv.x;
        float v = uv.y;
        float y = Mathf.Sin((0.5f - v) * Mathf.Pi);
        float magOther = Mathf.Sqrt(1 - y*y);
        float z = -magOther * Mathf.Sin(u * 2.0f * Mathf.Pi);
        float x = -magOther * Mathf.Cos(u * 2.0f * Mathf.Pi);
        return new Vector3(x, y, z);
    }

    public static Vector3 UpdatePoint(Vector3 pt) {
        return pt + NoiseScale * pt * noise.GetNoise3dv(pt);
    }

    public static Vector3 CalculateNormal(Vector2 uv) {
        Vector3 pt = CalculateCoordinate(uv);
        Vector3 pt1 = UpdatePoint(pt);
        Vector3 pt2, pt3;
        Vector3 axisInto = -pt;
        Vector3 axis1 = axisInto.Cross(Vector3.Up).Normalized();
        if (axisInto.Cross(Vector3.Up).LengthSquared() < 0.001f) {
            axis1 = axisInto.Cross(Vector3.Right).Normalized();
        }
        Vector3 axis2 = axisInto.Cross(axis1);
        pt2 = UpdatePoint(pt.Rotated(axis1, RotateAngle));
        pt3 = UpdatePoint(pt.Rotated(axis2, RotateAngle));
        Vector3 normal = (pt3 - pt1).Cross(pt2 - pt1).Normalized();
        if (normal.Dot(pt) < 0) {
            normal = -normal;
        }
        return normal;
    }

    public static ImageTexture Simplex3D(int dimensions) {
        noise.Period = NoisePeriod;
        Image image = new Image();
        image.Create(dimensions, dimensions, true, Image.Format.Rgb8);
        image.Lock();
        for (int y = 0; y < dimensions; y++) {
            for (int x = 0; x < dimensions; x++) {
                float u = ((float) x) / dimensions;
                float v = ((float) y) / dimensions;
                Vector2 uv = new Vector2(u, v);
                Vector3 value = UpdatePoint(CalculateCoordinate(uv));
                value = (value + Vector3.One * 3) / 6;
                image.SetPixel(x, y, new Color(value.x, value.y, value.z));
            }
        }
        image.SavePng("LeafOffset.png");
        image.Unlock();
        ImageTexture texture = new ImageTexture();
        texture.CreateFromImage(image);
        texture.Flags = (uint)ImageTexture.FlagsEnum.Mipmaps;
        return texture;
    }

    public static ImageTexture SimplexNormals3D(int dimensions) {
        noise.Period = NoisePeriod;
        Image image = new Image();
        image.Create(dimensions, dimensions, true, Image.Format.Rgb8);
        image.Lock();
        for (int y = 0; y < dimensions; y++) {
            for (int x = 0; x < dimensions; x++) {
                float u = ((float) x) / dimensions;
                float v = ((float) y) / dimensions;
                Vector2 uv = new Vector2(u, v);
                Vector3 value = CalculateNormal(uv);
                value = (value + Vector3.One) / 2;
                image.SetPixel(x, y, new Color(value.x, value.y, value.z));
            }
        }
        image.SavePng("LeafNormals.png");
        image.Unlock();
        ImageTexture texture = new ImageTexture();
        texture.CreateFromImage(image);
        texture.Flags = (uint)ImageTexture.FlagsEnum.Mipmaps;
        return texture;
    }
    public override void _Ready() {
        ShaderMaterial mat = GetSurfaceMaterial(0) as ShaderMaterial;
        mat.SetShaderParam("noise", Simplex3D(256));
        mat.SetShaderParam("normals", SimplexNormals3D(256));
    }

}
