using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using InputEventSystem;
using WindowSystem;

namespace WindowSystemTestbed
{
    public class Tutorial1 : Game
    {
        #region Fields
        private GraphicsDeviceManager graphics;
        private InputEvents input;
        private GUIManager gui;
        private MenuBar menuBar;
        private MenuItem newMenuItem;
        private MenuItem openMenuItem;
        private MenuItem saveMenuItem;
        private MenuItem saveAsMenuItem;
        private MenuItem exitMenuItem;
        private MenuItem undoMenuItem;
        private MenuItem redoMenuItem;
        #endregion

        #region Constructors
        public Tutorial1()
        {
            this.graphics = new GraphicsDeviceManager(this);

            this.input = new InputEvents(this);
            Components.Add(this.input);

            this.gui = new GUIManager(this);
            Components.Add(this.gui);

            // GUI requires variable timing to function correctly
            IsFixedTimeStep = false;
            Window.Title = "XNA Window System Tutorial 1";
        }
        #endregion

        protected override void Initialize()
        {
            // Has to be initialized before child controls can be added
            this.gui.Initialize();

            this.menuBar = new MenuBar(this, gui);
            MenuItem fileMenu = new MenuItem(this, gui);
            fileMenu.Text = "File";
            this.newMenuItem = new MenuItem(this, gui);
            this.newMenuItem.Text = "New...";
            fileMenu.Add(this.newMenuItem);
            this.openMenuItem = new MenuItem(this, gui);
            this.openMenuItem.Text = "Open...";
            fileMenu.Add(this.openMenuItem);
            this.saveMenuItem = new MenuItem(this, gui);
            this.saveMenuItem.Text = "Save";
            this.saveMenuItem.IsEnabled = false;
            fileMenu.Add(this.saveMenuItem);
            this.saveAsMenuItem = new MenuItem(this, gui);
            this.saveAsMenuItem.Text = "Save As...";
            this.saveAsMenuItem.IsEnabled = false;
            fileMenu.Add(this.saveAsMenuItem);
            this.exitMenuItem = new MenuItem(this, gui);
            this.exitMenuItem.Text = "Exit";
            fileMenu.Add(this.exitMenuItem);
            menuBar.Add(fileMenu);
            MenuItem editMenu = new MenuItem(this, gui);
            editMenu.Text = "Edit";
            this.undoMenuItem = new MenuItem(this, gui);
            this.undoMenuItem.Text = "Undo";
            editMenu.Add(this.undoMenuItem);
            this.redoMenuItem = new MenuItem(this, gui);
            this.redoMenuItem.Text = "Redo";
            editMenu.Add(this.redoMenuItem);
            this.menuBar.Add(editMenu);

            // Add menubar to gui
            this.gui.Add(this.menuBar);

            // Add event handlers
            this.newMenuItem.Click += new ClickHandler(OnMenuItemClicked);
            this.openMenuItem.Click += new ClickHandler(OnMenuItemClicked);
            this.saveMenuItem.Click += new ClickHandler(OnMenuItemClicked);
            this.saveAsMenuItem.Click += new ClickHandler(OnMenuItemClicked);
            this.exitMenuItem.Click += new ClickHandler(OnMenuItemClicked);
            this.undoMenuItem.Click += new ClickHandler(OnMenuItemClicked);
            this.redoMenuItem.Click += new ClickHandler(OnMenuItemClicked);

            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        #region Event Handlers
        private void OnMenuItemClicked(UIComponent sender)
        {
            if (sender == this.newMenuItem)
            {
                MessageBox messageBox = new MessageBox(this, gui, "New clicked!", "Tutorial 1", MessageBoxButtons.OK, MessageBoxType.Info);
                messageBox.Show(true);
            }
            else if (sender == this.openMenuItem)
            {
                MessageBox messageBox = new MessageBox(this, gui, "Open clicked!", "Tutorial 1", MessageBoxButtons.OK, MessageBoxType.Info);
                messageBox.Show(true);
            }
            else if (sender == this.undoMenuItem)
            {
                MessageBox messageBox = new MessageBox(this, gui, "Undo clicked!", "Tutorial 1", MessageBoxButtons.OK, MessageBoxType.Info);
                messageBox.Show(true);
            }
            else if (sender == this.redoMenuItem)
            {
                MessageBox messageBox = new MessageBox(this, gui, "Redo clicked!", "Tutorial 1", MessageBoxButtons.OK, MessageBoxType.Info);
                messageBox.Show(true);
            }
            else if (sender == this.exitMenuItem)
                Exit();
        }
        #endregion
    }
}
