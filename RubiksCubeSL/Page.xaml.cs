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

using Kit3D.Windows.Controls;
using Kit3D.Windows.Media.Media3D;
using Kit3D.Windows.Media;

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbCubeOps.Text = "";
            txtInput.Focus();
            Init();
        }


        private RubiksCube _rubikscube;
        
        private ModelVisual3D _model;
        private Viewport3D _viewport;
        private const int CommandCheckInterval = 1000; //in ms
        private int _elapsedTime = 0; //in ms
        private DateTime _lastTime = DateTime.Now;
        private Point _cubeCenter = new Point();
        private double _cubeSize = 0;
        private PerspectiveCamera _camera;
        //private CMatrix _viewMatrix;
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


        void RubiksCube_OneOpDone(string op, string seq, int steps,  bool isSolved)
        {
            tbSteps.Text = steps.ToString();
            tbCubeOps.Text = seq;

            /*
            if (isSolved)
            {
                MessageBox.Show("solved.");
            }*/
            
        }

        private Matrix3D RotateCamera(Vector3D axis, double angle)
        {
            axis.Normalize();
            Quaternion3D q = Quaternion3D.CreateFromAxisAngle(axis, angle);
            Matrix3D m  = Ext3D.CreateFromQuaternion(q);

            
            
            Vector3D p = new Vector3D(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);//120); //60;
            p = Ext3D.Transform(p, m); _camera.Position = new Point3D(p.X, p.Y, p.Z);

            
            Vector3D d = new Vector3D(_camera.LookDirection.X, _camera.LookDirection.Y, _camera.LookDirection.Z);
            d = Ext3D.Transform(d, m); _camera.LookDirection = new Vector3D(d.X, d.Y, d.Z);

            
            Vector3D up = new Vector3D(_camera.UpDirection.X, _camera.UpDirection.Y, _camera.UpDirection.Z);
            up = Ext3D.Transform(up, m); _camera.UpDirection = new Vector3D(up.X, up.Y, up.Z);
            
            return m;
        }

        Matrix3D _cameraRotate = Matrix3D.Identity;
        int _cameraMove = 0;
        int _cameraFar = 120;
        int _cameraNear = 60;
        int _cameraZ = 60;
        private void Init()
        {

            Viewport3D viewport = new Viewport3D();
            _viewport = viewport;
            this.cubePanel.Children.Add(viewport);
            PerspectiveCamera camera = new PerspectiveCamera();
            _camera = camera;


            ResetCamera();
            viewport.Camera = camera;
            viewport.ShowModelBoundingBoxes = true;
            viewport.HorizontalAlignment = HorizontalAlignment.Stretch;
            viewport.VerticalAlignment = VerticalAlignment.Stretch;
            
            
            CubeConfiguration.Factory = new Factory();
            _rubikscube = RubiksCube.CreateRubiksCube(new Vector3D(), 3, 10);
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
            //_rubikscube.Random();


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

            _rubikscube.Animator.Start(null);

            _rubikscube.Animator.Update();

            //camera movement
            
            if (_cameraMove != 0)
            {
                _cameraZ += _cameraMove;
                Vector3D p = new Vector3D(0, 0, _cameraZ);//120); //60;
                p = Ext3D.Transform(p, _cameraRotate); _camera.Position = new Point3D(p.X, p.Y, p.Z);
                Debug.WriteLine(_camera.Position);
                if (_cameraZ <= _cameraNear || _cameraZ >= _cameraFar)
                {
                    _cameraMove = 0;
                    OnLayoutUpdated();
                }
            }
            
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
            OnLayoutUpdated();
        }

        private void OnLayoutUpdated()
        {
            //Debug.WriteLine("OnLayoutUpdated");
            GeneralTransform transform = cubePanel.TransformToVisual(Application.Current.RootVisual as UIElement);
            _cubeCenter = transform.Transform(new Point(cubePanel.Width / 2, cubePanel.Height / 2));
            _cubeSize = System.Math.Min(cubePanel.Width, cubePanel.Height);

            _rubikscube.ViewMatrix = ViewMatrix;
            _rubikscube.InverseProjectionMatrix = _viewport.ScreenToViewTransform;
            _rubikscube.ViewPoint = _camera.Position;
        }


        bool _isLeftButtonPressed = false;
        bool _isDrag = false;
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftButtonPressed = true;
            _isDrag = false;
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                _isDrag = true;
                _previousPosition2D = e.GetPosition(_viewport);
                _previousPosition3D = ProjectToTrackball(
                    _viewport.ActualWidth,
                    _viewport.ActualHeight,
                    _previousPosition2D);
            }
            _rubikscube.Interaction.StartTrack(e.GetPosition(_viewport));
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
            _rubikscube.Interaction.EndTrack();
        }


        
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isLeftButtonPressed)
            {
                if (_isDrag && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    //drag
                    Point currentPosition = e.GetPosition(_viewport);
                    Track(currentPosition);
                    _previousPosition2D = currentPosition;
                }
                else
                    _rubikscube.Interaction.Track(e.GetPosition(_viewport));
            }
            else
            {
                if (_rubikscube.Interaction.InTrack)
                {
                    _rubikscube.Interaction.EndTrack();
                }
            }
            
            HitResult result;
            bool hit = _rubikscube.HitTest(e.GetPosition(_viewport), false, out result);
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

        void ResetCamera()
        {
            _cameraZ = _cameraNear; _cameraMove = 0;
            _camera.Position = new Point3D(0, 0, _cameraNear); //60
            _camera.LookDirection = new Vector3D(0, 0, -1);
            _camera.UpDirection = new Vector3D(0, 1, 0);
            _camera.FieldOfView = 80;

            _cameraRotate = RotateCamera(new Vector3D(0, 1, 0), Math.PI / 6);
            Vector3D axis = new Vector3D(1, 0, 0); axis = Ext3D.Transform(axis, _cameraRotate);
            _cameraRotate *= RotateCamera(axis, -Math.PI / 6);

        }
        void ResetUI()
        {
            tbSteps.Text = "0";
            tbCubeOps.Text = string.Empty;

            ResetCamera();

            btnUnfold.Content = "Unfold";
            //_rubikscube.ViewPoint = MathConverter.ToPoint3(_camera.Position);
            _viewport.UpdateLayout();
        }

        private void btnSolved_Click(object sender, RoutedEventArgs e)
        {
            _rubikscube.Reset();
            ResetUI();
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            _rubikscube.Random();
            if(!_rubikscube.Unfolded)
                ResetUI();
        }

        private void btnUnfold_Click(object sender, RoutedEventArgs e)
        {
            _rubikscube.Unfold(30);
            
            if (_rubikscube.Unfolded)
            {
                _cameraMove = 2;
                btnUnfold.Content = "Fold";
            }
            else
            {
                _cameraMove = -2;
                btnUnfold.Content = "Unfold";
            }
        }



        private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();
        private Vector3D _previousPosition3D = new Vector3D(0, 0, 1);
        private Point _previousPosition2D;
        private double AngleBetween(Vector3D vector1, Vector3D vector2)
        {
            double num;
            vector1.Normalize();
            vector2.Normalize();
            if (Vector3D.DotProduct(vector1, vector2) < 0.0)
            {
                Vector3D vectord2 = -vector1 - vector2;
                num = 3.1415926535897931 - (2.0 * Math.Asin(vectord2.Length / 2.0));
            }
            else
            {
                Vector3D vectord = vector1 - vector2;
                num = 2.0 * Math.Asin(vectord.Length / 2.0);
            }
            return num;// *57.295779513082323;
        }




        private void Track(Point currentPosition)
        {
            Vector3D currentPosition3D = ProjectToTrackball(
                _viewport.ActualWidth, _viewport.ActualHeight, currentPosition);
            Vector3D axis3d = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D);
            double angle = AngleBetween(_previousPosition3D, currentPosition3D);
            Vector3D axis = new Vector3D(axis3d.X, axis3d.Y, axis3d.Z);
            axis = Ext3D.Transform(axis, _cameraRotate);
            Matrix3D m = RotateCamera(axis, -angle);
            //_cameraRotate = m * _cameraRotate;
            _cameraRotate *= m;
            OnLayoutUpdated();
            _previousPosition3D = currentPosition3D;
        }


        private Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double x = point.X / (width / 2);    // Scale so bounds map to [0,0] - [2,2]
            double y = point.Y / (height / 2);

            x = x - 1;                           // Translate 0,0 to the center
            y = 1 - y;                           // Flip so +Y is up instead of down

            double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3D(x, y, z);
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            _rubikscube.Undo();
        }

    }
}
