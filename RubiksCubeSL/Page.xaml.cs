//-----------------------------------------------------------------------------
// File:        Page.xaml.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Knoics.Math;
using Knoics.RubiksCube;
using Knoics.Interactive;
using CMatrix = Knoics.Math.Matrix;
using Kit3D.Windows.Controls;
using Kit3D.Windows.Media.Media3D;
using Kit3D.Windows.Media;
using Quaternion = Kit3D.Windows.Media.Media3D.Quaternion;
using Kit3D.Math;
using System.Diagnostics;

namespace RubiksCubeSL
{

    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        int _steps;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbCubeOps.Text = "";
            txtInput.Focus();
            _steps = 0;
            Init();
        }


        private RubiksCube _rubikscube;
        private CubeInteraction _interaction;
        private Animator _animator;
        private ModelVisual3D _model;
        private Viewport3D _viewport;
        private const int CommandCheckInterval = 1000; //in ms
        private int _elapsedTime = 0; //in ms
        private DateTime _lastTime = DateTime.Now;
        private Point _cubeCenter = new Point();
        private double _cubeSize = 0;
        private PerspectiveCamera _camera;
        private CMatrix _viewMatrix;
        private Dictionary<MeshGeometry3D, CubieMesh> _meshes = new Dictionary<MeshGeometry3D,CubieMesh>();
        private void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _elapsedTime = 0;
        }

        private void TextBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _elapsedTime = 0;
        }


        void CheckCommand()
        {
            txtInput.Text = _rubikscube.DoCommand(txtInput.Text);
            txtInput.SelectionStart = txtInput.Text.Length;
        }


        void RubiksCube_OneOpDone(string op, bool isSolved)
        {
            _steps++;
            tbCubeOps.Text = tbCubeOps.Text + op;
            tbSteps.Text = _steps.ToString();

            /*
            if (isSolved)
            {
                MessageBox.Show("solved.");
            }*/
        }


        private void Init()
        {

            Viewport3D viewport = new Viewport3D();
            _viewport = viewport;
            //cubePanel.Children.Add(viewport);
            //this.LayoutRoot.Children.Add(viewport);
            this.cubePanel.Children.Add(viewport);
            PerspectiveCamera camera = new PerspectiveCamera();
            _camera = camera;
            camera.Position = new Point3D(0, 0, 60);
            camera.LookDirection = new Vector3D(0, 0, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 75;

            
            Vector3 v = new Vector3(-1, 1, 0); v.Normalize();
            Knoics.Math.Quaternion q = Knoics.Math.Quaternion.CreateFromAxisAngle(v, Math.PI/4);
            CMatrix r = CMatrix.CreateFromQuaternion(q);
            Vector3 p = new Vector3(0, 0, 60);
            p = Vector3.Transform(p, r); _camera.Position = new Point3D(p.X, p.Y, p.Z);

            Vector3 d = new Vector3(0, 0, -1);
            d = Vector3.Transform(d, r); _camera.LookDirection = new Vector3D(d.X, d.Y, d.Z);


            _viewMatrix = Knoics.Interactive.Camera.GetViewMatrix(MathConverter.ToPoint3(camera.Position), MathConverter.ToVector3(camera.LookDirection), MathConverter.ToVector3(camera.UpDirection));

            viewport.Camera = camera;
            viewport.ShowModelBoundingBoxes = true;
            viewport.HorizontalAlignment = HorizontalAlignment.Stretch;
            viewport.VerticalAlignment = VerticalAlignment.Stretch;
            
            
            _animator = new Animator(30f); //interval: 30ms 

            CubeConfiguration.Factory = new Factory();
            _rubikscube = RubiksCube.CreateRubiksCube(new Point3(), 10, _animator);
            _interaction = new CubeInteraction(_rubikscube);
            ModelVisual3D model = ((CubeModel)_rubikscube.Model).ModelVisual;
            viewport.Children.Add(model);

            Model3DGroup group = (Model3DGroup)model.Content;
            foreach (IMesh mesh in _rubikscube.Meshes)
            {
                MeshGeometry3D geometry = ((CubieMesh)mesh).Mesh;
                group.Children.Add(((CubieMesh)mesh).Geometry);
                _meshes.Add(geometry, (CubieMesh)mesh);
            }
            _rubikscube.OneOpDone = RubiksCube_OneOpDone;
            _rubikscube.Random();


            _model = model;


            Kit3D.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }


        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - _lastTime;
            _lastTime = now;
            _elapsedTime += elapsed.Milliseconds;
            if (_elapsedTime >= CommandCheckInterval)
            {
                CheckCommand();
                _elapsedTime = 0;
            }

            if (_animator.ReadyInQueue)
            {
                _animator.Start(null);
            }
            _animator.Update();            
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
            GeneralTransform transform = cubePanel.TransformToVisual(Application.Current.RootVisual as UIElement);
            _cubeCenter = transform.Transform(new Point(cubePanel.Width/2, cubePanel.Height/2));
            _cubeSize = System.Math.Min(cubePanel.Width, cubePanel.Height);

            _rubikscube.ViewMatrix = MathConverter.ToMatrix(ViewMatrix);
            _rubikscube.InverseProjectionMatrix = MathConverter.ToMatrix(_viewport.ScreenToViewTransform);
            _rubikscube.ViewPoint = MathConverter.ToPoint3(_camera.Position);
        }


        bool _isLeftButtonPressed = false;
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftButtonPressed = true;
            _interaction.StartTrack(MathConverter.ToPoint2(e.GetPosition(_viewport)));
        }


        Matrix3D InverseViewMatrix
        {
            get { Matrix3D m = ViewMatrix; m.Invert();  return m;}
        }

        Matrix3D ViewMatrix
        {
            get
            {
                Vector3D cameraZAxis = -_camera.LookDirection;
                cameraZAxis.Normalize();

                Vector3D cameraXAxis = Vector3D.CrossProduct(_camera.UpDirection, cameraZAxis);
                cameraXAxis.Normalize();

                Vector3D cameraYAxis = Vector3D.CrossProduct(cameraZAxis, cameraXAxis);

                Vector3D cameraPosition = (Vector3D)_camera.Position;
                double offsetX = -Vector3D.DotProduct(cameraXAxis, cameraPosition);
                double offsetY = -Vector3D.DotProduct(cameraYAxis, cameraPosition);
                double offsetZ = -Vector3D.DotProduct(cameraZAxis, cameraPosition);

                return new Matrix3D(cameraXAxis.X, cameraYAxis.X, cameraZAxis.X, 0,
                                    cameraXAxis.Y, cameraYAxis.Y, cameraZAxis.Y, 0,
                                    cameraXAxis.Z, cameraYAxis.Z, cameraZAxis.Z, 0,
                                    offsetX, offsetY, offsetZ, 1);
            }
        }



        private void Rotate(Axis axis, double angle)
        {
            Quaternion delta = new Quaternion();
            if (axis == Axis.X)
                delta = new Quaternion(new Vector3D(1, 0, 0), angle);
            else if (axis == Axis.Y)
                delta = new Quaternion(new Vector3D(0, 1, 0), angle);
            else if (axis == Axis.Z)
                delta = new Quaternion(new Vector3D(0, 0, 1), angle);
            else
                System.Diagnostics.Debug.Assert(false);
            RotateTransform3D rt = (RotateTransform3D)_model.Transform;
            AxisAngleRotation3D r = (AxisAngleRotation3D)rt.Rotation;
            Quaternion q = new Quaternion(r.Axis, r.Angle);
            q *= delta;
            r.Axis = q.Axis;
            r.Angle = q.Angle;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isLeftButtonPressed = false;
            _interaction.EndTrack();
        }


        Matrix3D ConvertTo3D(CMatrix m)
        {
            Matrix3D m3d = new Matrix3D();
            m3d.M11 = m.M11; m3d.M12 = m.M12; m3d.M13 = m.M13; m3d.M14 = m.M14;
            m3d.M21 = m.M21; m3d.M22 = m.M22; m3d.M23 = m.M23; m3d.M24 = m.M24;
            m3d.M31 = m.M31; m3d.M32 = m.M32; m3d.M33 = m.M33; m3d.M34 = m.M34;
            m3d.OffsetX = m.M41; m3d.OffsetY = m.M42; m3d.OffsetZ = m.M43; m3d.M44 = m.M44;
            return m3d;
        }

        
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isLeftButtonPressed )
            {
                _interaction.Track(MathConverter.ToPoint2(e.GetPosition(_viewport)));
            }
            
            HitResult result;
            bool hit = _rubikscube.HitTest(MathConverter.ToPoint2(e.GetPosition(_viewport)), false, out result);
            if (hit)
            {
                if (this.Cursor != Cursors.Hand)
                    this.Cursor = Cursors.Hand;
            }
            else
            {
                if (this.Cursor != Cursors.Arrow)
                    this.Cursor = Cursors.Arrow;
            }
        }

        private void btnSolved_Click(object sender, RoutedEventArgs e)
        {
            _steps = 0;
            _rubikscube.Reset();
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            _steps = 0;
            _rubikscube.Random();
        }

    }
}
