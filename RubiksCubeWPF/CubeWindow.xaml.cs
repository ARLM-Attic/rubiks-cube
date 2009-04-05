//-----------------------------------------------------------------------------
// File:        Page.xaml.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Controls.Primitives;
using Knoics.RubiksCube;
using Knoics.Math;
using Knoics.Interactive;

namespace RubiksCubeWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CubeWindow : Window
    {

        public CubeWindow()
        {
            InitializeComponent();
        }


        private RubiksCube _rubikscube;
        
        private Viewport3D _viewport;
        private ModelVisual3D _model;
        private const int CommandCheckInterval = 1000; //in ms
        private int _elapsedTime = 0; //in ms
        private DateTime _lastTime = DateTime.Now;
        private PerspectiveCamera _camera;
        private void txtInput_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _elapsedTime = 0;
        }

        private void txtInput_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _elapsedTime = 0;
        }


        void CheckCommand()
        {
            txtInput.Text = _rubikscube.DoCommand(txtInput.Text);
            txtInput.SelectionStart = txtInput.Text.Length;
        }


        void RubiksCube_OneOpDone(string op, string seq, int steps, bool isSolved)
        {
            txtCubeOps.Text = txtCubeOps.Text + op;
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCubeOps.Text = "";
            txtInput.Focus();
            Init();
        }

        private void Init()
        {
            Viewport3D viewport = new Viewport3D();
            _viewport = viewport;
            cubePanel.Children.Add(viewport);
            PerspectiveCamera camera = new PerspectiveCamera();
            _camera = camera;
            viewport.Camera = camera;
            camera.Position = new Point3D(30, 30, 30);
            camera.LookDirection = new Vector3D(-1, -1, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 90;

            

            
            CubeConfiguration.Factory = new Factory();
            _rubikscube = RubiksCube.CreateRubiksCube(new Point3(), 10);
            ModelVisual3D model = ((CubeModel)_rubikscube.Model).ModelVisual;
            Model3DGroup group = (Model3DGroup)model.Content;
            viewport.Children.Add(model);
            foreach (IMesh mesh in _rubikscube.Meshes)
            {
                group.Children.Add(((CubieMesh)mesh).Geometry);
            }

            _rubikscube.OneOpDone = RubiksCube_OneOpDone;
            _rubikscube.Random();

            _model = model;

            
            
            
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            
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

            if (_rubikscube.Animator.ReadyInQueue)
            {
                _rubikscube.Animator.Start(null);
            }
            _rubikscube.Animator.Update();
        }

        private void cubePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _prevLocation = Project(e.GetPosition(cubePanel));
        }
        Vector3 _prevLocation;
        private void cubePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Vector3 p = Project(e.GetPosition(cubePanel));
                //Position2 p1 = new Position2(_prevLocation.X, _prevLocation.Y);
                //Position2 p2 = new Position2(p.X, p.Y);

                /*
                Vector3 axis;
                double theta;
                Knoics.Interactive.Rotation.GetTransform(_prevLocation, p, out axis, out theta);
                System.Windows.Media.Media3D.Quaternion delta = new System.Windows.Media.Media3D.Quaternion(new Vector3D(axis.X, axis.Y, axis.Z), theta);

                
                AxisAngleRotation3D r = ((RotateTransform3D)_rubikscube.VisualModel.Transform).Rotation as AxisAngleRotation3D;// _rotation;
                System.Windows.Media.Media3D.Quaternion q = new System.Windows.Media.Media3D.Quaternion(r.Axis, r.Angle);
                q *= delta;

                r.Axis = q.Axis;
                r.Angle = q.Angle;
                 */
                
                Knoics.Math.Matrix m = Knoics.Interactive.Rotation.GetRotationTransform(_prevLocation, p, true);
                Matrix3D m3d = MathConverter.ToMatrix3D(m);
                _camera.Transform = new MatrixTransform3D(Matrix3D.Multiply(m3d, _camera.Transform.Value));// m * _model.Transform;
                
                _prevLocation = p;
            }
        }

        private Vector3 Project(Point p)
        {
            return Vector3.CreateVector3(new Point2(p.X, p.Y), cubePanel.Height);
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
        }
    }
}
