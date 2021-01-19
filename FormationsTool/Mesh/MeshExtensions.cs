using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace FormationsTool.Mesh
{
    public static class MeshExtensions
    {
        // Add a normal for a polygon.
        // Assumes the points are coplanar.
        // Assumes the points are outwardly oriented.
        public static void AddPolygonNormal(this MeshGeometry3D mesh,
            double length, double thickness, params Point3D[] points)
        {
            // Find the "center" point.
            double x = 0, y = 0, z = 0;
            foreach (Point3D point in points)
            {
                x += point.X;
                y += point.Y;
                z += point.Z;
            }
            Point3D endpoint1 = new Point3D(
                x / points.Length,
                y / points.Length,
                z / points.Length);

            // Get the polygon's normal.
            Vector3D n = FindTriangleNormal(
                points[0], points[1], points[2]);

            // Set the length.
            n = n.Scale(length);

            // Find the segment's other end point.
            Point3D endpoint2 = endpoint1 + n;

            // Create the segment.
            AddSegment(mesh, endpoint1, endpoint2, thickness);
        }

        // Add a polygon's normal by using the point names A, B, C, etc.
        // This is used to make dodecahedron normals.
        public static void AddPolygonNormal(this MeshGeometry3D mesh,
            string point_names, double length, double thickness, Point3D[] points)
        {
            Point3D[] polygon_points = new Point3D[point_names.Length];
            for (int i = 0; i < point_names.Length; i++)
            {
                polygon_points[i] = points[ToIndex(point_names[i])];
            }
            AddPolygonNormal(mesh, length, thickness, polygon_points);
        }

        // Return a MeshGeometry3D representing this mesh's triangle normals.
        public static MeshGeometry3D ToTriangleNormals(this MeshGeometry3D mesh,
            double length, double thickness)
        {
            // Make a mesh to hold the normals.
            MeshGeometry3D normals = new MeshGeometry3D();

            // Loop through the mesh's triangles.
            for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's vertices.
                Point3D point1 = mesh.Positions[mesh.TriangleIndices[triangle]];
                Point3D point2 = mesh.Positions[mesh.TriangleIndices[triangle + 1]];
                Point3D point3 = mesh.Positions[mesh.TriangleIndices[triangle + 2]];

                // Make the triangle's normal
                AddTriangleNormal(mesh, normals,
                    point1, point2, point3, length, thickness);
            }

            return normals;
        }

        // Add a segment representing the triangle's normal to the normals mesh.
        private static void AddTriangleNormal(MeshGeometry3D mesh,
            MeshGeometry3D normals, Point3D point1, Point3D point2, Point3D point3,
            double length, double thickness)
        {
            // Get the triangle's normal.
            Vector3D n = FindTriangleNormal(point1, point2, point3);

            // Set the length.
            n = n.Scale(length);

            // Find the center of the triangle.
            Point3D endpoint1 = new Point3D(
                (point1.X + point2.X + point3.X) / 3.0,
                (point1.Y + point2.Y + point3.Y) / 3.0,
                (point1.Z + point2.Z + point3.Z) / 3.0);

            // Find the segment's other end point.
            Point3D endpoint2 = endpoint1 + n;

            // Create the segment.
            AddSegment(normals, endpoint1, endpoint2, thickness);
        }

        // Calculate a triangle's normal vector.
        public static Vector3D FindTriangleNormal(Point3D point1, Point3D point2, Point3D point3)
        {
            // Get two edge vectors.
            Vector3D v1 = point2 - point1;
            Vector3D v2 = point3 - point2;

            // Get the cross product.
            Vector3D n = Vector3D.CrossProduct(v1, v2);

            // Normalize.
            n.Normalize();

            return n;
        }

        // Return a MeshGeometry3D representing this mesh's wireframe.
        public static MeshGeometry3D ToWireframe(this MeshGeometry3D mesh, double thickness)
        {
            // Make a mesh to hold the wireframe.
            MeshGeometry3D wireframe = new MeshGeometry3D();

            // Make a HastSet to keep track of edges
            // that have already been added.
            HashSet<Edge> already_added = new HashSet<Edge>();

            // Loop through the mesh's triangles.
            for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's corner indices.
                int index1 = mesh.TriangleIndices[triangle];
                int index2 = mesh.TriangleIndices[triangle + 1];
                int index3 = mesh.TriangleIndices[triangle + 2];
                Point3D point1 = mesh.Positions[index1];
                Point3D point2 = mesh.Positions[index2];
                Point3D point3 = mesh.Positions[index3];

                // Make the triangle's three segments.
                wireframe.AddEdge(already_added, point1, point2, thickness);
                wireframe.AddEdge(already_added, point2, point3, thickness);
                wireframe.AddEdge(already_added, point3, point1, thickness);
            }

            return wireframe;
        }

        // Add an outline wireframe for a polygon.
        public static void AddPolygonWireframe(this MeshGeometry3D mesh,
            double thickness, params Point3D[] points)
        {
            // Make a HastSet to keep track of edges
            // that have already been added.
            HashSet<Edge> already_added = new HashSet<Edge>();

            // Loop through the polygon's vertices.
            for (int i = 0; i < points.Length - 1; i++)
            {
                mesh.AddEdge(already_added, points[i], points[i + 1], thickness);
            }
            mesh.AddEdge(already_added, points[points.Length - 1], points[0], thickness);
        }

        // Add a polygon's wireframe by using the point names A, B, C, etc.
        // This is used to make dodecahedron wireframes.
        public static void AddPolygonWireframe(this MeshGeometry3D mesh,
            string point_names, double thickness, Point3D[] points)
        {
            Point3D[] polygon_points = new Point3D[point_names.Length];
            for (int i = 0; i < point_names.Length; i++)
            {
                polygon_points[i] = points[ToIndex(point_names[i])];
            }
            AddPolygonWireframe(mesh, thickness, polygon_points);
        }

        // Return a MeshGeometry3D representing this mesh's vertices as boxes.
        public static MeshGeometry3D ToVertexBoxes(this MeshGeometry3D mesh, double thickness)
        {
            // Make a mesh to hold the result.
            MeshGeometry3D boxes = new MeshGeometry3D();

            // Make vectors of the desired lengths.
            Vector3D ux = new Vector3D(thickness, 0, 0);
            Vector3D uy = new Vector3D(0, thickness, 0);
            Vector3D uz = new Vector3D(0, 0, thickness);
            Vector3D ux2 = ux / 2;
            Vector3D uy2 = uy / 2;
            Vector3D uz2 = uz / 2;

            // Loop through the mesh's vertices.
            foreach (Point3D vertex in mesh.Positions)
            {
                // Add a box for this vertex.
                boxes.AddBox(vertex - ux2 - uy2 - uz2, ux, uy, uz);
            }
            return boxes;
        }

        // Add an edge between the two points unless it has already been added.
        private static void AddEdge(this MeshGeometry3D mesh,
            HashSet<Edge> already_added, Point3D point1, Point3D point2, double thickness)
        {
            // Make an Edge.
            Edge edge = new Edge(point1, point2);

            // See if the Edge has already been added.
            if (already_added.Contains(edge)) return;

            // Add the edge.
            already_added.Add(edge);
            mesh.AddSegment(point1, point2, thickness);
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        // Assumes the points are outwardly oriented.
        public static void AddTriangle(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        // Make a thin rectangular prism between the two points.
        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        // If up is missing, create a perpendicular vector to use.
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness, bool extend)
        {
            // Find an up vector that is not colinear with the segment.
            // Start with a vector parallel to the Y axis.
            Vector3D up = new Vector3D(0, 1, 0);

            // If the segment and up vector point in more or less the
            // same direction, use an up vector parallel to the X axis.
            Vector3D segment = point2 - point1;
            segment.Normalize();
            if (Math.Abs(Vector3D.DotProduct(up, segment)) > 0.9)
                up = new Vector3D(1, 0, 0);

            // Add the segment.
            AddSegment(mesh, point1, point2, up, thickness, extend);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            double x1, double y1, double z1,
            double x2, double y2, double z2, double thickness)
        {
            AddSegment(mesh, new Point3D(x1, y1, z1),
                new Point3D(x2, y2, z2), thickness, false);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness)
        {
            AddSegment(mesh, point1, point2, thickness, false);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness,
            bool extend)
        {
            // Get the segment's vector.
            Vector3D v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                Vector3D n = v.Scale(thickness / 2.0);
                point1 -= n;
                point2 += n;
                v += 2 * n;
            }

            // Get the scaled up vector.
            Vector3D n1 = up.Scale(thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = n2.Scale(thickness / 2.0);

            // Make a skinny box.
            mesh.AddBox(point1 - n1 - n2, v, 2 * n1, 2 * n2);
        }

        // Make a box.
        // The vectors should be oriented so v1 x v2 = v3.
        // For example, for a unit box parallel to the coordinate axes,
        // the corner could be the one with the minimum X, Y, and Z
        // coordinates and the axes could be unit vectors along the
        // coordinate axes.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D corner, Vector3D v1, Vector3D v2, Vector3D v3)
        {
            AddRectangle(mesh, corner, v2, v1);         // Back
            AddRectangle(mesh, corner + v3, v1, v2);    // Front
            AddRectangle(mesh, corner, v3, v2);         // Left
            AddRectangle(mesh, corner + v1, v2, v3);    // Right
            AddRectangle(mesh, corner + v2, v3, v1);    // Top
            AddRectangle(mesh, corner, v1, v3);         // Bottom
        }

        // Add a rectangle.
        // Vectors v1 and v2 should be outwardly oriented so
        // v1 x v2 points outward.
        public static void AddRectangle(this MeshGeometry3D mesh,
            Point3D corner, Vector3D v1, Vector3D v2, bool setTextCoords = false)
        {
            AddTriangle(mesh, corner, corner + v1, corner + v1 + v2);
            AddTriangle(mesh, corner, corner + v1 + v2, corner + v2);

            if (setTextCoords)
            {
                // Set the points' texture coordinates.
                mesh.TextureCoordinates.Add(new Point(0, 0));
                mesh.TextureCoordinates.Add(new Point(1, 0));
                mesh.TextureCoordinates.Add(new Point(1, 1));

                mesh.TextureCoordinates.Add(new Point(0, 0));
                mesh.TextureCoordinates.Add(new Point(1, 1));
                mesh.TextureCoordinates.Add(new Point(0, 1));
            }
        }

        // Add X, Y, and Z axes.
        public static void AddAxes(this MeshGeometry3D mesh, double xmax, double thickness)
        {
            mesh.AddAxes(xmax, thickness, true, true, 2, 8);
        }
        public static void AddAxes(this MeshGeometry3D mesh,
            double xmax, double thickness, bool start_at_origin,
            bool show_tick_marks, double tick_thickness, double tick_width)
        {
            double xmin, ymin, zmin;
            if (start_at_origin)
            {
                xmin = 0;
                ymin = 0;
                zmin = 0;
            }
            else
            {
                xmin = -xmax;
                ymin = -xmax;
                zmin = -xmax;
            }
            mesh.AddSegment(xmin, 0, 0, xmax, 0, 0, thickness);
            mesh.AddSegment(0, ymin, 0, 0, xmax, 0, thickness);
            mesh.AddSegment(0, 0, zmin, 0, 0, xmax, thickness);

            if (show_tick_marks)
            {
                Vector3D ux = new Vector3D(thickness, 0, 0);
                Vector3D uy = new Vector3D(0, thickness, 0);
                Vector3D uz = new Vector3D(0, 0, thickness);
                for (int x = (int)xmin; x <= (int)xmax; x++)
                {
                    Point3D corner = new Point3D(
                        x - tick_thickness * 0.5 * thickness,
                        -tick_width * 0.5 * thickness,
                        -tick_width * 0.5 * thickness);
                    mesh.AddBox(corner, tick_thickness * ux, tick_width * uy, tick_width * uz);
                }
                for (int y = (int)ymin; y <= (int)xmax; y++)
                {
                    Point3D corner = new Point3D(
                        -tick_width * 0.5 * thickness,
                        y - tick_thickness * 0.5 * thickness,
                        -tick_width * 0.5 * thickness);
                    mesh.AddBox(corner, tick_width * ux, tick_thickness * uy, tick_width * uz);
                }
                for (int z = (int)zmin; z <= (int)xmax; z++)
                {
                    Point3D corner = new Point3D(
                        -tick_width * 0.5 * thickness,
                        -tick_width * 0.5 * thickness,
                        z - tick_thickness * 0.5 * thickness);
                    mesh.AddBox(corner, tick_width * ux, tick_width * uy, tick_thickness * uz);
                }
            }
        }

        // Add triangles from a List<Triangle>.
        public static void AddTriangles(this MeshGeometry3D mesh,
            List<Triangle> triangles)
        {
            foreach (Triangle triangle in triangles)
            {
                mesh.AddTriangle(
                    triangle.Points[0],
                    triangle.Points[1],
                    triangle.Points[2]);
            }
        }

        // Add a polygon to the indicated mesh.
        // Do not reuse old points but reuse these points.
        // Assumes the points are outwardly oriented.
        // Assumes the points are coplanar.
        public static void AddPolygon(this MeshGeometry3D mesh, params Point3D[] points)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            foreach (Point3D point in points)
                mesh.Positions.Add(point);

            // Create the triangles.
            for (int i = 1; i < points.Length - 1; i++)
            {
                mesh.TriangleIndices.Add(index1);
                mesh.TriangleIndices.Add(index1 + i);
                mesh.TriangleIndices.Add(index1 + i + 1);
            }
        }

        // Add a polygon by using the point names A, B, C, etc.
        // This is used to make dodecahedrons.
        public static void AddPolygon(this MeshGeometry3D mesh, string point_names, Point3D[] points)
        {
            Point3D[] polygon_points = new Point3D[point_names.Length];
            for (int i = 0; i < point_names.Length; i++)
            {
                polygon_points[i] = points[ToIndex(point_names[i])];
            }
            AddPolygon(mesh, polygon_points);
        }

        // Find a point's index from its letter.
        private static int ToIndex(char ch)
        {
            return ch - 'A';
        }

        // Give the mesh a solid colored material.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            DiffuseMaterial material = new DiffuseMaterial(brush);
            return new GeometryModel3D(mesh, material);
        }

        // Merge another mesh into this one.
        public static void MergeWith(this MeshGeometry3D mesh, MeshGeometry3D other)
        {
            int offset = mesh.Positions.Count;
            foreach (Point3D point in other.Positions)
                mesh.Positions.Add(point);
            foreach (int index in other.TriangleIndices)
                mesh.TriangleIndices.Add(index + offset);
        }
    }
}
