#region Description
//-----------------------------------------------------------------------------
// File:        RubiksCubeGame.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;

using Knoics.RubiksCube;
using XNALib;
using WindowSystem;
using InputEventSystem;
using CMatrix = Knoics.Math.Matrix;

namespace RubiksCubeWindows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class RubiksCubeGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;
        private InputEvents input;
        private GUIManager gui;
        private TextBox _textBox;
        private Label _label;
        private MessageBox _messageBox;
        private RubiksCube _rubikscube;
        
        Matrix _worldMatrix = Matrix.Identity;// Matrix.CreateScale(0.01f);// Matrix.Identity;

        private ICamera _camera;
        private IInput _input;
        private Animator _animator;
        private Timer _commandLineCheckTimer;

        private MenuBar menuBar;
        private MenuItem startMenuItem;
        private MenuItem solvedMenuItem;
        private MenuItem exitMenuItem;

        public RubiksCubeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            RubiksCube.MeshFactory = new MeshFactory();

            this.input = new InputEvents(this);
            this.Components.Add(input);
            this.gui = new GUIManager(this);
            this.Components.Add(gui);
            //this.Components.Add(new TextLabel(this));

            this.Components.Add(new Fps(this, false));


            
            _commandLineCheckTimer = new Timer(this);
            this.Components.Add(_commandLineCheckTimer);

            _commandLineCheckTimer.OnTime = (t) =>
            {
                _textBox.Text = Interpreter.DoCommand(_rubikscube, _textBox.Text);
                return true;
            };
            this.IsFixedTimeStep = false;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gui.Initialize();
            _textBox = new TextBox(this, gui);
            _label = new Label(this, gui);
            _textBox.X = 0;
            _textBox.Y = this.Window.ClientBounds.Height - _textBox.Height;
            _textBox.KeyDown += new KeyDownHandler(_textBox_KeyDown);
            

            _messageBox = new MessageBox(this, gui, "Congratulation! Your rubik's cube is solved.", "RubiksCube", MessageBoxButtons.OK, MessageBoxType.Info);
            this.gui.Add(_textBox);
            this.gui.Add(_label);

            this.menuBar = new MenuBar(this, gui);
            MenuItem fileMenu = new MenuItem(this, gui);
            fileMenu.Text = "File";
            this.startMenuItem = new MenuItem(this, gui);
            this.startMenuItem.Text = "Start Game";
            fileMenu.Add(this.startMenuItem);

            this.solvedMenuItem = new MenuItem(this, gui);
            this.solvedMenuItem.Text = "Show Solved";
            fileMenu.Add(this.solvedMenuItem);

            this.exitMenuItem = new MenuItem(this, gui);
            this.exitMenuItem.Text = "Exit";
            fileMenu.Add(this.exitMenuItem);

            menuBar.Add(fileMenu);
            this.gui.Add(menuBar);

            _label.Y = menuBar.Height;
            _label.X = 0;
            _label.Text = string.Empty;
            
            startMenuItem.Click += new ClickHandler(startMenuItem_Click);
            exitMenuItem.Click += new ClickHandler(exitMenuItem_Click);
            solvedMenuItem.Click += new ClickHandler(solvedMenuItem_Click);

            _input = new Input();


            ResetCamera();
            base.Initialize();
        }

        void _textBox_KeyDown(KeyEventArgs args)
        {
            _commandLineCheckTimer.Reset();
        }


        void RubiksCube_OneOpDone(string op, bool isSolved)
        {
            _label.Text = _label.Text + op;
            _label.FitToText();
            if (isSolved)
            {
                _messageBox.Show(false);
            }
        }

        void solvedMenuItem_Click(UIComponent sender)
        {
            Init();
        }

        void exitMenuItem_Click(UIComponent sender)
        {
            this.Exit();
        }

        void startMenuItem_Click(UIComponent sender)
        {
            Init();

            _rubikscube.Random();
        }

        void ResetCamera()
        {
            _camera = new QuaternionCamera(new Vector3(40, 40, 40), Vector3.Zero, Vector3.Up, this.GraphicsDevice.Viewport.AspectRatio);
        }
        void Init()
        {
            _label.Text = string.Empty;
            _rubikscube.Reset();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            _animator = new Animator(30f); //interval: 30ms 
            RubiksCube.GraphicsDevice = graphics.GraphicsDevice;


            _rubikscube = new RubiksCube(new Position() { X = 0, Y = 0, Z = 0 }, 10, _animator);
            /*
            Matrix[] modelTransforms;
            _rubikscube.InitModel(XNAUtils.LoadModelWithBoundingSphere(out modelTransforms, "rubik_cube-XNA", Content));
            */
            _rubikscube.OneOpDone = RubiksCube_OneOpDone;
            _rubikscube.Random();


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (_animator.ReadyInQueue)
            {
                _animator.Start(null);
            }
            InputMode mode = _input.Update(this, _camera.Projection, _camera.View);
            /*
            if (mode == InputMode.Select)
            {
                Ray ray = XNAUtils.CreateRayFrom2D(GraphicsDevice, _input.Selected, _camera.Projection, _camera.View, Matrix.Identity);
                _rubikscube.SelectCubicle(ray);
                
            }
            else if (mode == InputMode.Drag)
            {
                Vector2 d = _input.DragTo - _input.DragFrom;
                if (Math.Abs(d.X) > 5 || Math.Abs(d.Y) > 5)
                {
                    Ray from = XNAUtils.CreateRayFrom2D(GraphicsDevice, _input.DragFrom, _camera.Projection, _camera.View, Matrix.Identity);
                    Cubicle fromCubicle; float fromDistance;
                    bool fromIntersected = _rubikscube.SelectCubicle(from, out fromCubicle, out fromDistance);
                    Ray to = XNAUtils.CreateRayFrom2D(GraphicsDevice, _input.DragTo, _camera.Projection, _camera.View, Matrix.Identity);
                    Cubicle toCubicle; float toDistance;
                    bool toIntersected = _rubikscube.SelectCubicle(to, out toCubicle, out toDistance);

                    if (fromIntersected && toIntersected)
                    {
                        Vector3 fromPoint = from.Position + from.Direction * fromDistance;
                        Vector3 toPoint = to.Position + to.Direction * toDistance;
                        Position fromPos = new Position(fromPoint.X, fromPoint.Y, fromPoint.Z);
                        Position toPos = new Position(toPoint.X, toPoint.Y, toPoint.Z);
                        string op = CubeOperation.GetOp(_rubikscube.SelectedBasicOp, fromPos, toPos);
                        if (!string.IsNullOrEmpty(op))
                            _rubikscube.Rotate(op, 500, false);
                    }
                }
            }
            
            */
            _camera.Rotate(-_input.Pan, -_input.Tilt, -_input.Pan);
            _input.Reset();
            _animator.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            //_model.CopyAbsoluteBoneTransformsTo(_modelTransforms);
            //_rubikscube.DrawModel(_modelTransforms, _worldMatrix, _camera.View, _camera.Projection);

            _rubikscube.Draw(MathConverter.FromXNAMatrix(_worldMatrix), MathConverter.FromXNAMatrix(_camera.View), MathConverter.FromXNAMatrix(_camera.Projection));
            

            base.Draw(gameTime);
        }
    }
}
