//-----------------------------------------------------------------------------
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

using CMatrix = Knoics.Math.Matrix;
using CColor = Knoics.RubiksCube.Color;
using CSize = Knoics.Math.Size;
using Color = System.Windows.Media.Color;
using Size = System.Windows.Size;

using Knoics.Math;
using Knoics.RubiksCube;

namespace RubiksCubeSL
{
    class Factory : IFactory
    {
        #region IMeshFactory Members
        public IMesh CreateMesh(CubieFace face, Point3[] vertexes, CColor color)
        {
            return new CubieMesh(face, vertexes, color);
        }

        public IModel CreateModel()
        {
            return new CubeModel();
        }
        #endregion
    }

    class CubeModel : IModel
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

        public CMatrix GetTransform()
        {
            return MathConverter.ToMatrix(_visualModel.Transform.Value);
        }

        public void Transform(CMatrix matrix)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class CubieMesh : IMesh
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
        private Color MapToColor (CColor meshColor){
            Color color = new Color();
            color.A = meshColor.A;
            color.B = meshColor.B;
            color.G = meshColor.G;
            color.R = meshColor.R;

            return color;
        }

        private Point3DCollection PositionsToPoint3DCollection(Point3[] positions)
        {
            Point3DCollection points = new Point3DCollection(positions.Count());
            foreach(Point3 pos in positions){
                Point3D vertex = new Point3D();
                vertex.X = pos.X;
                vertex.Y = pos.Y;
                vertex.Z = pos.Z;
                points.Add(vertex);
            }
            return points;
        }

        static readonly string[] PositiveFaces = new string[] { "U", "F", "R" };
        public CubieMesh(CubieFace face, Point3[] vertexes, CColor color)
        {
            _face = face;
            //_visual = new ModelVisual3D();
            _mesh = new MeshGeometry3D();
            _geometry = new GeometryModel3D();
            _geometry.Geometry = _mesh;
            SetColor(MapToColor(color));
            //_visual.Content = _geometry;
            

            Point3DCollection positions = PositionsToPoint3DCollection(vertexes);
            //positions.Freeze();
            _mesh.Positions = positions;

            Int32Collection indices;
            
            if (!string.IsNullOrEmpty(PositiveFaces.FirstOrDefault(f => f == face.Name))) //X,Y,Z direction
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

        private CubieFace _face;
        public CubieFace Face { get { return _face; } }
        private Matrix3D _savedTransform;
        public void Save()
        {
            _savedTransform = _geometry.Transform.Value;
        }
        public void Restore()
        {
            _geometry.Transform = new MatrixTransform3D(_savedTransform);
        }
        public void Transform(CMatrix matrix, bool isFromSaved)
        {
            Matrix3D m = MathConverter.ToMatrix3D(matrix);
            if(isFromSaved)
                _geometry.Transform = new MatrixTransform3D(Matrix3D.Multiply(m, _savedTransform));// m * _model.Transform;
            else
                _geometry.Transform = new MatrixTransform3D(Matrix3D.Multiply(m, _geometry.Transform.Value));// m * _model.Transform;
            
        }

        public CMatrix GetTransform()
        {
            return MathConverter.ToMatrix(_geometry.Transform.Value);
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

        public static Point3 ToPoint3(Point3D point)
        {
            return new Point3(point.X, point.Y, point.Z);
        }
        public static Point2 ToPoint2(Point point)
        {
            return new Point2(point.X, point.Y);
        }

        public static Vector3 ToVector3(Vector3D v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }

}
