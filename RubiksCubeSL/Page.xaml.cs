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
using Knoics;

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

        private PollingTimer _timer = new PollingTimer(1000);

        private PerspectiveCamera _camera;


        Matrix3D _cameraRotate = Matrix3D.Identity;
        Matrix3D _viewMatrix = Matrix3D.Identity;

        int _cameraMove = 0;
        int _cameraFar = 120;
        int _cameraNear = 60;
        int _cameraZ = 60;

        bool _isLeftButtonPressed = false;
        bool _isDrag = false;
        private Vector3D _previousPosition3D = new Vector3D(0, 0, 1);
        private Point _previousPosition2D;

        private void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _timer.Reset();
        }

        private void TextBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _timer.Reset();
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
            
            
            _rubikscube = RubiksCube.CreateRubiksCube(new Vector3D(), 3, 10, RubiksCube_OneOpDone, new Factory());
            ModelVisual3D model = ((CubeModel)_rubikscube.Model).ModelVisual;
            viewport.Children.Add(model);

            Model3DGroup group = (Model3DGroup)model.Content;
            foreach (IMesh mesh in _rubikscube.Meshes)
            {
                group.Children.Add(((CubieMesh)mesh).Geometry);
            }
            _rubikscube.Random();
            _model = model;
            Kit3D.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }


        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (_timer.OnInterval())
                CheckCommand();
            
            _rubikscube.Animator.Start(null);

            _rubikscube.Animator.Update();

            //camera movement
            
            if (_cameraMove != 0)
            {
                _cameraZ += _cameraMove;
                Vector3D p = new Vector3D(0, 0, _cameraZ);//120); //60;
                p = Ext3D.Transform(p, _cameraRotate); _camera.Position = new Point3D(p.X, p.Y, p.Z);
                //Debug.WriteLine(_camera.Position);
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
            

            _rubikscube.ViewMatrix = _viewMatrix;
            _rubikscube.InverseProjectionMatrix = _viewport.ScreenToViewTransform;
            _rubikscube.ViewPoint = _camera.Position;
        }



        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftButtonPressed = true;
            _isDrag = false;
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                _isDrag = true;
                _previousPosition2D = e.GetPosition(_viewport);
                _previousPosition3D = _previousPosition2D.ProjectToTrackball(
                    _viewport.ActualWidth,
                    _viewport.ActualHeight
                    );
            }
            _rubikscube.StartTrack(e.GetPosition(_viewport));
        }


        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isLeftButtonPressed = false;
            _rubikscube.EndTrack();
        }


        static int debug = 0;
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
                    //do twist
                    _rubikscube.Track(e.GetPosition(_viewport));
            }
            else
            {
                if (_rubikscube.InTrack)
                {
                    _rubikscube.EndTrack();
                }
            }

            bool hit = _rubikscube.HitTestOnly(e.GetPosition(_viewport), false);
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

            Matrix3D vm = Matrix3D.Identity;
            _cameraRotate =  _camera.RotateCamera(new Vector3D(0, 1, 0), Math.PI / 6).RotateMatrix;
            Vector3D axis = new Vector3D(1, 0, 0); axis = Ext3D.Transform(axis, _cameraRotate);
            
           Methods.RotateCameraResult rcr = _camera.RotateCamera(axis, -Math.PI / 6);
            _cameraRotate *=  rcr.RotateMatrix;
            _viewMatrix = rcr.ViewMatrix;

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

        private void Track(Point currentPosition)
        {
            Vector3D currentPosition3D = currentPosition.ProjectToTrackball(
                _viewport.ActualWidth, _viewport.ActualHeight);
            Vector3D axis = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D);
            double angle = Ext3D.AngleBetween(_previousPosition3D, currentPosition3D);
            axis = Ext3D.Transform(axis, _cameraRotate);
            Methods.RotateCameraResult rcr = _camera.RotateCamera(axis, -angle);
            _cameraRotate *= rcr.RotateMatrix;
            _viewMatrix = rcr.ViewMatrix;
            OnLayoutUpdated();
            _previousPosition3D = currentPosition3D;
        }



        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            _rubikscube.Undo();
        }

    }
}
