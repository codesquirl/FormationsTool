using System.Windows.Media.Media3D;

namespace FormationsTool.Mesh
{
    public static class VectorExtensions
    {
        public static Vector3D Scale(this Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return vector * scale;
        }
    }
}
