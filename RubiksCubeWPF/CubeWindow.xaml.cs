﻿//-----------------------------------------------------------------------------
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


        private DispatcherTimer _commandTimer;
        private DispatcherTimer _updateTimer;
        private RubiksCube _rubikscube;
        private Animator _animator;
        private Viewport3D _viewport;

        private void txtInput_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _commandTimer.Stop();
        }

        private void txtInput_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _commandTimer.Start();
        }

        void _updateTimer_Tick(object sender, EventArgs e)
        {
            if (_animator.ReadyInQueue)
            {
                _animator.Start(null);
            }
            _animator.Update();            
        }

        void _commandTimer_Tick(object sender, EventArgs e)
        {
            txtInput.Text = Interpreter.DoCommand(_rubikscube, txtInput.Text);
        }


        void RubiksCube_OneOpDone(string op, bool isSolved)
        {
            txtCubeOps.Text = txtCubeOps.Text + op;
            /*
            if (isSolved)
            {
                MessageBox.Show("solved.");
            }
             */
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
            viewport.Camera = camera;
            camera.Position = new Point3D(30, 30, 30);
            camera.LookDirection = new Vector3D(-1, -1, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 90;


            _animator = new Animator(30f); //interval: 30ms 

            RubiksCube.MeshFactory = new MeshFactory();
            _rubikscube = new RubiksCube(new Position(), 10, _animator);
            _rubikscube.IterateFaces(delegate(object mesh)
            {
                viewport.Children.Add(((CubieMesh)mesh).ModelVisual3D);
            });

            _rubikscube.OneOpDone = RubiksCube_OneOpDone;
            //_rubikscube.Random();


            _commandTimer = new DispatcherTimer();
            _commandTimer.Interval = TimeSpan.FromMilliseconds(1000);
            _commandTimer.Tick += new EventHandler(_commandTimer_Tick);
            _commandTimer.Start();

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(30);
            _updateTimer.Tick += new EventHandler(_updateTimer_Tick);
            _updateTimer.Start();
        }
    }
}
