﻿using FormationsTool.Mesh;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FormationsTool
{
    /// <summary>
    /// Interaction logic for FormationCanvas.xaml
    /// </summary>
    public partial class FormationCanvas : UserControl
    {
        private Formation formation;
        private GeometryModel3D Floor = null;
        private List<GeometryModel3D> Markers { get; set; } = new List<GeometryModel3D>();
        private List<Light> Lights = new List<Light>();
        private Model3DGroup MainModelGroup = new Model3DGroup();
        private MeshGeometry3D MarkerMesh;

        private Point3D CameraPosition;
        private double markerSize = 60.0;

        public bool SnapToGrid { get; set; } = false;
        public int GridSize { get; set; } = 25;
        public double InitialZoom { get; set; } = 1600.0;

        public double ZoomSensitivity { get; set; } = 1.0;

        public double MarkerSize
        {
            get => markerSize;
            set
            {
                markerSize = value;
                ResetScene();
            }
        }

        public Formation Formation
        {
            get => formation;
            set
            {
                formation = value;
                DataContext = formation;
            }
        }

        public FormationCanvas()
        {
            Initialized += FormationCanvas_Initialized;
            InitializeComponent();
        }

        private void FormationCanvas_Initialized(object sender, EventArgs e)
        {
            DataContextChanged += FormationCanvas_DataContextChanged;

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = MainModelGroup;
            Viewport.Children.Add(visual);

            DefineFloor();
            DefineLights();

            ResetScene();
        }

        private void FormationCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Formation old = e.OldValue as Formation;
            if (old != null)
            {
                old.PositionChanged -= Formation_PositionChanged;
            }
            formation.PositionChanged += Formation_PositionChanged;
            UpdateViewport();
        }

        private void Formation_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            UpdateMarker(e.Index, e.Position);
        }

        #region 3D Objects

        private void ResetScene()
        {
            MainModelGroup.Children.Clear();
            MainModelGroup.Children.Add(Floor);
            AddLights();
            DefineMarkers(Formation?.Positions?.Count ?? 8);
            AddMarkers();
            CameraPosition = new Point3D(0, 0, InitialZoom);
            CoordsLabel.Text = $"{CameraPosition.X},{CameraPosition.Y},{CameraPosition.Z}";
            Camera.Position = CameraPosition;
        }

        void UpdateViewport()
        {
            if (Markers.Count != Formation.Positions.Count)
            {
                ResetScene();
            }

            // Update Marker Positions
            for (int i = 0; i < Formation.Positions.Count; i++)
            {
                UpdateMarker(i, Formation.Positions[i]);
            }
        }

        private void UpdateMarker(int i, FVector vector)
        {
            Markers[i].Transform = new TranslateTransform3D(vector.X, vector.Y, vector.Z);
        }

        private void DefineMarkers(int numMarkers)
        {
            if (numMarkers < 1) return;
            MarkerMesh = new MeshGeometry3D();
            MarkerMesh.AddGeodesicSphere(2, MarkerSize, new TranslateTransform3D(0, 0, 0));

            Markers.Clear();
            Markers.Add(MarkerMesh.MakeModel(App.PlayerColor));
            for (int i = 1; i < numMarkers; i++)
            {
                Markers.Add(MarkerMesh.MakeModel(App.PositionColors[(i-1) % App.PositionColors.Count]));
            }
        }
        private void AddMarkers()
        {
            foreach (var marker in Markers)
            {
                MainModelGroup.Children.Add(marker);
            }
        }


        private void DefineLights()
        {
            Lights.Clear();
            Color color64 = Color.FromArgb(255, 128, 128, 128);
            Color color128 = Color.FromArgb(255, 255, 255, 255);
            Lights.Add(new AmbientLight(color64));
            Lights.Add(new DirectionalLight(color128,
                new Vector3D(-1.0, -2.0, -3.0)));
            Lights.Add(new DirectionalLight(color128,
                new Vector3D(1.0, 2.0, 3.0)));
        }
        private void AddLights()
        {
            foreach (Light light in Lights)
            {
                MainModelGroup.Children.Add(light);
            }
        }

        private void DefineFloor()
        {
            double floorHeight = 0.0;
            MeshGeometry3D floorMesh = new MeshGeometry3D();
            floorMesh.AddRectangle(
                new Point3D(-1000, -1000, floorHeight),
                new Vector3D(2000, 0, 0),
                new Vector3D(0, 2000, 0),
                true
            );
            Floor = floorMesh.MakeModel(Color.FromArgb(255, 64, 64, 64));
        }

        #endregion

        #region MouseHandling

        private bool DraggingMarker = false;
        private bool DraggingCamera = false;
        private int SelectedMarkerIndex = -1;
        private GeometryModel3D SelectedMarker = null;
        private Point DragStartPoint;

        private void DragMoveTo(Point3D point)
        {
            if (SnapToGrid)
            {
                double x = Math.Floor(point.X / GridSize);
                int xClosest = ((int)Math.Round(point.X / GridSize) * GridSize);
                int yClosest = ((int)Math.Round(point.Y / GridSize) * GridSize);
                point.X = xClosest;
                point.Y = yClosest;
            }
            Formation.Positions[SelectedMarkerIndex].Set((float)point.X, (float)point.Y);
        }

        private void DockPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Clamp Camera between 800 -> 3000
            CameraPosition.Z = Math.Min(CameraPosition.Z - (ZoomSensitivity * e.Delta), 3000);
            CameraPosition.Z = Math.Max(CameraPosition.Z, 800);
            CoordsLabel.Text = $"{CameraPosition.X},{CameraPosition.Y},{CameraPosition.Z}";
            Camera.Position = CameraPosition;
        }

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !DraggingCamera)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult =
                    VisualTreeHelper.HitTest(Viewport, e.GetPosition(Viewport)) as RayMeshGeometry3DHitTestResult;
                if (rayMeshResult != null)
                {
                    for (int i = 1; i < Markers.Count; i++)
                    {
                        if (Markers[i] == rayMeshResult.ModelHit)
                        {
                            SelectedMarkerIndex = i;
                            SelectedMarker = Markers[i];
                            DraggingMarker = true;
                            DragStartPoint = e.GetPosition(Viewport);
                            e.Handled = true;
                            return;
                        }
                    }
                }
                SelectedMarker = null;
            }
            else if (e.ChangedButton == MouseButton.Right && !DraggingMarker)
            {
                DraggingCamera = true;
                DragStartPoint = e.GetPosition(Viewport);
                e.Handled = true;
                return;
            }
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DraggingMarker = false;
            DraggingCamera = false;
            SelectedMarker = null;
        }

        private void Viewport_MouseLeave(object sender, MouseEventArgs e)
        {
            DraggingCamera = false;
            DraggingMarker = false;
            SelectedMarker = null;
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (DraggingCamera) {
                var newPoint = e.GetPosition(Viewport);
                CameraPosition.Offset((newPoint.X - DragStartPoint.X) * -2, (newPoint.Y - DragStartPoint.Y) * 2, 0);
                Camera.Position = CameraPosition;
                CoordsLabel.Text = $"{CameraPosition.X},{CameraPosition.Y},{CameraPosition.Z}";
                DragStartPoint = newPoint;
            }
            else
            {
                PointHitTestParameters pointParams = new PointHitTestParameters(e.GetPosition(Viewport));
                VisualTreeHelper.HitTest(Viewport, null, MouseMoveResultCallback, pointParams);
            }
        }

        private HitTestResultBehavior MouseMoveResultCallback(HitTestResult result)
        {
            if (result is RayMeshGeometry3DHitTestResult rayResult)
            {
                if (rayResult.ModelHit == Floor)
                {
                    CursorLabel.Text = $"{rayResult.PointHit.X:F0},{rayResult.PointHit.Y:F0},{rayResult.PointHit.Z:F0}";
                    if (DraggingMarker)
                    {
                        DragMoveTo(rayResult.PointHit);
                    }
                    return HitTestResultBehavior.Stop;
                }
            }
            return HitTestResultBehavior.Continue;
        }

        #endregion

        private void SnapToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            SnapToGrid = SnapToggleButton.IsChecked ?? false;
        }

        private void SnapSizeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (int.TryParse((string)((ComboBoxItem) SnapSizeComboBox.SelectedItem).Content, out int snapSize))
            {
                GridSize = snapSize;
            }
        }
    }
}
