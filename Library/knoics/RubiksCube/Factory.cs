﻿//-----------------------------------------------------------------------------
// File:        Factory.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Media;

using Kit3D.Windows.Controls;
using Kit3D.Windows.Media.Media3D;
using Kit3D.Windows.Media;

using Knoics.Math;

namespace Knoics.RubiksCube
{
    public class Factory : IFactory
    {
        #region IMeshFactory Members
        public IMesh CreateMesh(string faceName, Vector3D[] vertexes, Color color)
        {
            return new CubieMesh(faceName, vertexes, color);
        }

        public IModel CreateModel()
        {
            return new CubeModel();
        }
        #endregion

    }

    public class CubeModel : IModel
    {
        #region IModel Members
        private readonly ModelVisual3D _visualModel;
        public ModelVisual3D ModelVisual { get { return _visualModel; } }
        public CubeModel()
        {
            _visualModel = new ModelVisual3D();

            Model3DGroup group = new Model3DGroup();
            _visualModel.Content = group;
            RotateTransform3D rotation = new RotateTransform3D();
            _visualModel.Transform = rotation;
            rotation.Rotation = new AxisAngleRotation3D();
        }

        public Matrix3D Transform
        {
            get
            {
                return _visualModel.Transform.Value;
            }
        }

        public void DoTransform(Matrix3D matrix, bool isFromSaved)
        {
            //Matrix3D m = MathConverter.ToMatrix3D(matrix);
            if (isFromSaved)
                _visualModel.Transform = new MatrixTransform3D(Matrix3D.Multiply(matrix, _savedTransform));// m * _model.Transform;
            else
                _visualModel.Transform = new MatrixTransform3D(Matrix3D.Multiply(matrix, _visualModel.Transform.Value));// m * _model.Transform;

        }

        public void Reset()
        {
            _visualModel.Transform = new MatrixTransform3D(Matrix3D.Identity);
        }

        private Matrix3D _savedTransform;
        public void Save()
        {
            _savedTransform = _visualModel.Transform.Value;
        }
        public void Restore()
        {
            _visualModel.Transform = new MatrixTransform3D(_savedTransform);
        }

        #endregion
    }

    public class CubieMesh : IMesh
    {
        private readonly GeometryModel3D _geometry;
        private readonly MeshGeometry3D _mesh;
        private Size Size { get; set; }
        private Color Color { get; set; }
        private Point3D Center { get; set; }
        private Axis Axis { get; set; }
        private double EdgeWidth { get; set; }

        public GeometryModel3D Geometry { get { return _geometry; } }
        public MeshGeometry3D Mesh { get { return _mesh; } }


        private Point3DCollection PositionsToPoint3DCollection(Vector3D[] positions)
        {
            Point3DCollection points = new Point3DCollection(positions.Count());
            foreach (Point3D pos in positions)
            {
                Point3D vertex = new Point3D();
                vertex.X = pos.X;
                vertex.Y = pos.Y;
                vertex.Z = pos.Z;
                points.Add(vertex);
            }
            return points;
        }


        static readonly string[] PositiveFaces = new string[] { "U", "F", "R" };
        public CubieMesh(string faceName, Vector3D[] vertexes, Color color)
        {

            //_visual = new ModelVisual3D();
            _mesh = new MeshGeometry3D();
            _geometry = new GeometryModel3D();
            _geometry.Geometry = _mesh;
            SetColor(color);
            //_visual.Content = _geometry;


            Point3DCollection positions = PositionsToPoint3DCollection(vertexes);
            //positions.Freeze();
            _mesh.Positions = positions;

            Int32Collection indices;

            if (!string.IsNullOrEmpty(PositiveFaces.FirstOrDefault(f => f == faceName))) //X,Y,Z direction
            {
                indices = new Int32Collection
                {
                    0,1,3,
                    1,2,3
                };
            }
            else
            {
                indices = new Int32Collection
                {
                    3,1,0,
                    3,2,1
                };
            }

            _mesh.TriangleIndices = indices;
            _geometry.SeamSmoothing = 1;
        }

        private void SetColor(Color color)
        {
            Material colorMaterial = new DiffuseMaterial(new Kit3DBrush(new SolidColorBrush(color)));
            Material backcolorMaterial = new DiffuseMaterial(new Kit3DBrush(new SolidColorBrush(Colors.Black)));
            _geometry.Material = colorMaterial;
            _geometry.BackMaterial = backcolorMaterial;

            /*WPF Version
            MaterialGroup unlitMaterial = new MaterialGroup();
            unlitMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.Black)));
            unlitMaterial.Children.Add(new EmissiveMaterial(new SolidColorBrush(color)));
            //unlitMaterial.Freeze();
            _geometry.Material = unlitMaterial;
            _geometry.BackMaterial = unlitMaterial;
             */
        }

        /*
        #region Size
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(
                "Size",
                typeof(Size),
                typeof(CubeFace),
                new PropertyMetadata(
                    new Size(10,10),
                    OnSizeChanged));

        private static void OnSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((CubeFace)sender).SetSize((Size)args.NewValue);
        }

        public Size Size
        {
            get { return (Size)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private void SetSize(Size size)
        {
            ConstructGeometry();
        }
        #endregion

        #region Axis
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register(
                "Axis",
                typeof(string),
                typeof(CubeFace),
                new PropertyMetadata(
                    "Z",
                    OnAxisChanged));

        private static void OnAxisChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((CubeFace)sender).SetAxis((string)args.NewValue);
        }

        public string Axis
        {
            get { return (string)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        private void SetAxis(string axis)
        {
            ConstructGeometry();
        }
        #endregion

        #region Origin
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register(
                "Origin",
                typeof(Point3D),
                typeof(CubeFace),
                new PropertyMetadata(
                    new Point3D(0,0,0),
                    OnOriginChanged));

        private static void OnOriginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((CubeFace)sender).SetOrigin((Point3D)args.NewValue);
        }

        public Point3D Origin
        {
            get { return (Point3D)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        private void SetOrigin(Point3D origin)
        {
            ConstructGeometry();
        }
        #endregion

        #region Color
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                "Color",
                typeof(Color),
                typeof(CubeFace),
                new PropertyMetadata(
                    Colors.Red,
                    OnColorChanged));

        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((CubeFace)sender).SetColor((Color)args.NewValue);
        }


        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion 
        */

        #region IMesh Members

        public void Reset()
        {
            _geometry.Transform = new MatrixTransform3D(Matrix3D.Identity);
        }

        private Matrix3D _savedTransform;
        public void Save()
        {
            _savedTransform = _geometry.Transform.Value;
        }
        public void Restore()
        {
            _geometry.Transform = new MatrixTransform3D(_savedTransform);
        }
        public void DoTransform(Matrix3D matrix, bool isFromSaved)
        {
            //Matrix3D m = MathConverter.ToMatrix3D(matrix);
            if (isFromSaved)
                _geometry.Transform = new MatrixTransform3D(Matrix3D.Multiply(matrix, _savedTransform));// m * _model.Transform;
            else
                _geometry.Transform = new MatrixTransform3D(Matrix3D.Multiply(matrix, _geometry.Transform.Value));// m * _model.Transform;

        }

        public Matrix3D Transform
        {
            get
            {
                return _geometry.Transform.Value;
            }
        }

        #endregion
    }

}
