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
using CubeLib;
using CubeLib.Common;

namespace RubiksCubeWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CubeWindow : Window
    {
        private DispatcherTimer _commandTimer;
        private DispatcherTimer _updateTimer;
        private RubiksCube _rubikscube;
        private Animator _animator;

        public CubeWindow()
        {
            InitializeComponent();

            _animator = new Animator(30f); //interval: 30ms 

            RubiksCube.MeshFactory = new MeshFactory();
            _rubikscube = new RubiksCube(new Position(), 10, _animator);
            _rubikscube.IterateFaces(delegate(object mesh)
            {
                viewport.Children.Add((CubieMesh)mesh);
            });

            _rubikscube.OneOpDone = RubiksCube_OneOpDone;
            //_rubikscube.Random();


            _commandTimer = new DispatcherTimer();
            _commandTimer.Interval = TimeSpan.FromMilliseconds(1000);
            _commandTimer.Tick += new EventHandler(_timer_Tick);
            _commandTimer.Start();

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(30);
            _updateTimer.Tick += new EventHandler(_updateTimer_Tick);
            _updateTimer.Start();

        }

        void _updateTimer_Tick(object sender, EventArgs e)
        {
            if (_animator.ReadyInQueue)
            {
                _animator.Start(null);
            }
            _animator.Update();            
        }

        void RubiksCube_OneOpDone(string op, bool isSolved)
        {
            txtCubeOps.Text = txtCubeOps.Text + op;
            if (isSolved)
            {
                MessageBox.Show("solved.");
            }
        }


        void _timer_Tick(object sender, EventArgs e)
        {
            txtInput.Text = Interpreter.DoCommand(_rubikscube, txtInput.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCubeOps.Text = "";
            txtInput.Focus();
        }

        private void txtInput_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _commandTimer.Stop();
        }

        private void txtInput_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _commandTimer.Start();
        }
    }
}
