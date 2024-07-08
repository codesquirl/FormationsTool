using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace FormationsTool.Mesh
{
    public class TextHelper
    {
        public static GeometryModel3D CreateTextLabel3D(
            string text,
            Brush textColor,
            double height)
        {
            var textBlock = new TextBlock(new Run(text));
            textBlock.Foreground = textColor;
            textBlock.FontFamily = new FontFamily("Ariel");
            
            var material = new DiffuseMaterial();
            material.Brush = new VisualBrush(textBlock);

            var width = text.Length * height;

            var over = new Vector3D(1, 0, 0);
            var up = new Vector3D(0, 1, 0);
            var center = new Point3D(0, 0, 0);
            
            var p0 = center - width / 2 * over - height / 2 * up;
            var p1 = p0 + up * 1 * height;
            var p2 = p0 + over * width;
            var p3 = p0 + up * 1 * height + over * width;

            var geometry = new MeshGeometry3D();
            geometry.Positions = new Point3DCollection();
            geometry.Positions.Add(p0); // 0
            geometry.Positions.Add(p1); // 1
            geometry.Positions.Add(p2); // 2
            geometry.Positions.Add(p3); // 3

            geometry.TriangleIndices.Add(0);
            geometry.TriangleIndices.Add(3);
            geometry.TriangleIndices.Add(1);
            geometry.TriangleIndices.Add(0);
            geometry.TriangleIndices.Add(2);
            geometry.TriangleIndices.Add(3);

            geometry.TextureCoordinates.Add(new Point(0, 1));
            geometry.TextureCoordinates.Add(new Point(0, 0));
            geometry.TextureCoordinates.Add(new Point(1, 1));
            geometry.TextureCoordinates.Add(new Point(1, 0));
            
            return new GeometryModel3D(geometry, material);
        }
    }
}