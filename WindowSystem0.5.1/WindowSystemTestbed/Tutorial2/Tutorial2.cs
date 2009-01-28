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
    public class Tutorial2 : Game
    {
        #region Fields
        private GraphicsDeviceManager graphics;
        private InputEvents input;
        private GUIManager gui;
        private UserDetailsDialog dialog;
        #endregion

        #region Constructors
        public Tutorial2()
        {
            this.graphics = new GraphicsDeviceManager(this);

            this.input = new InputEvents(this);
            Components.Add(this.input);

            // Add a key down event handler
            this.input.KeyDown += new KeyDownHandler(OnKeyDown);

            this.gui = new GUIManager(this);
            Components.Add(this.gui);

            // GUI requires variable timing to function correctly
            IsFixedTimeStep = false;
            Window.Title = "XNA Window System Tutorial 2";
        }
        #endregion

        protected override void Initialize()
        {
            // Has to be initialized before child controls can be added
            this.gui.Initialize();

            // Show a message box informing the user how to use the program
            MessageBox info = new MessageBox(
                this,
                this.gui,
                "Press enter to bring up the dialog.",
                "Info",
                MessageBoxButtons.OK,
                MessageBoxType.Info
                );
            info.Show(true);

            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        #region Event Handlers
        private void OnKeyDown(KeyEventArgs args)
        {
            if (args.Key == Keys.Enter)
            {
                // Only create a new dialog if one isn't show
                if (this.gui.GetModal() == null)
                {
                    // Create and show dialog
                    this.dialog = new UserDetailsDialog(this, this.gui);
                    this.dialog.Close += new CloseHandler(OnDialogClosed);
                    this.dialog.Show(true);
                }
            }
        }

        private void OnDialogClosed(UIComponent sender)
        {
            if (sender == this.dialog)
            {
                if (this.dialog.DialogResult == DialogResult.OK)
                {
                    MessageBox message = new MessageBox(
                        this,
                        this.gui,
                        "Name: " + this.dialog.Name,
                        "User Name",
                        MessageBoxButtons.OK,
                        MessageBoxType.Info
                        );
                    message.Show(true);
                }

                // Remove event handler so garbage collector will pick it up
                this.dialog.Close -= OnDialogClosed;
                this.dialog = null;
            }
        }
        #endregion
    }
}