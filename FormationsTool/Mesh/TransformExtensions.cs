using System.Windows.Media.Media3D;

namespace FormationsTool.Mesh
{
    public static class TransformExtensions
    {
        public static void Transform(this Transform3D transform, Triangle triangle)
        {
            transform.Transform(triangle.Points);
        }
    }
}
