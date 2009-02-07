﻿//-----------------------------------------------------------------------------
// File:        MeshFactory.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
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

using CMatrix = Knoics.Math.Matrix;
using Knoics.RubiksCube;

namespace RubiksCubeSL
{
    class MeshFactory : IMeshFactory
    {
        #region IMeshFactory Members
        public IMesh CreateMesh(Position[] vertexes, MeshColor color)//(Axis axis, Position center, MeshSize size, double edgeWidth, MeshColor color)
        {
            //return new CubieMesh(axis, center, size, edgeWidth, color);
            return new CubieMesh(vertexes, color);
        }

        public IMesh CreateMesh(object meshObject)
        {
            throw new NotImplementedException();
        }
        #endregion

    }

    class CubieMesh : IMesh
    {
        private readonly GeometryModel3D _geometry;
        private readonly MeshGeometry3D _mesh;
        private readonly ModelVisual3D _model;
        private MeshSize Size { get; set; }
        private Color Color { get; set; }
        private Point3D Center { get; set; }
        private Axis Axis { get; set; }
        private double EdgeWidth { get; set; }

        public ModelVisual3D ModelVisual3D { get { return _model; } }

        private Color MapToColor (MeshColor meshColor){
            Color color = new Color();
            color.A = meshColor.A;
            color.B = meshColor.B;
            color.G = meshColor.G;
            color.R = meshColor.R;

            return color;
        }

        private Point3DCollection PositionsToPoint3DCollection(Position[] positions)
        {
            Point3DCollection points = new Point3DCollection(positions.Count());
            foreach(Position pos in positions){
                Point3D vertex = new Point3D();
                vertex.X = pos.X;
                vertex.Y = pos.Y;
                vertex.Z = pos.Z;
                points.Add(vertex);
            }
            return points;
        }

        public CubieMesh(Position[] vertexes, MeshColor color)
        {
            _model = new ModelVisual3D();
            _mesh = new MeshGeometry3D();
            _geometry = new GeometryModel3D();
            _geometry.Geometry = _mesh;
            SetColor(MapToColor(color));
            _model.Content = _geometry;


            Point3DCollection positions = PositionsToPoint3DCollection(vertexes);
            //positions.Freeze();
            _mesh.Positions = positions;

            Int32Collection indices = new Int32Collection
            {
                3,1,0,
                3,2,1
            };

            _mesh.TriangleIndices = indices;
            _geometry.SeamSmoothing = 1;
        }

        private void SetColor(Color color)
        {
            Material colorMaterial = new DiffuseMaterial(new Kit3DBrush(new SolidColorBrush(color)));
            _geometry.Material = colorMaterial;
            _geometry.BackMaterial = colorMaterial;
             
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
            
        }

        public void Transform(CMatrix matrix)
        {
            Matrix3D m = MathConverter.ToMatrix3D(matrix);
            _geometry.Transform = new MatrixTransform3D(Matrix3D.Multiply(m, _geometry.Transform.Value));// m * _model.Transform;
            
        }

        #endregion

        #region IMesh Members


        public object MeshObject
        {
            get { return this; }
        }

        #endregion

        #region IMesh Members

        public void Draw(CMatrix world, CMatrix view, CMatrix projection)
        {
            
        }

        #endregion
    }


    class MathConverter
    {
        public static Matrix3D ToMatrix3D(CMatrix matrix)
        {
            Matrix3D m = new Matrix3D();
            m.M11 = matrix.M11; m.M12 = matrix.M12; m.M13 = matrix.M13; m.M14 = matrix.M14;
            m.M21 = matrix.M21; m.M22 = matrix.M22; m.M23 = matrix.M23; m.M24 = matrix.M24;
            m.M31 = matrix.M31; m.M32 = matrix.M32; m.M33 = matrix.M33; m.M34 = matrix.M34;
            m.OffsetX = matrix.M41; m.OffsetY = matrix.M42; m.OffsetZ = matrix.M43; m.M44 = matrix.M44;
            return m;
        }
        public static CMatrix ToMatrix(Matrix3D matrix)
        {
            CMatrix m = new CMatrix();
            m.M11 = (float)matrix.M11; m.M12 = (float)matrix.M12; m.M13 = (float)matrix.M13; m.M14 = (float)matrix.M14;
            m.M21 = (float)matrix.M21; m.M22 = (float)matrix.M22; m.M23 = (float)matrix.M23; m.M24 = (float)matrix.M24;
            m.M31 = (float)matrix.M31; m.M32 = (float)matrix.M32; m.M33 = (float)matrix.M33; m.M34 = (float)matrix.M34;
            m.M41 = (float)matrix.OffsetX; m.M42 = (float)matrix.OffsetY; m.M43 = (float)matrix.OffsetZ; m.M44 = (float)matrix.M44;
            return m;
        }
    }

}
