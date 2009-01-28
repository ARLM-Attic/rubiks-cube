#region Using Statements
using System;
using Microsoft.Xna.Framework;
using WindowSystem;
#endregion

namespace WindowSystemTestbed
{
    public class UserDetailsDialog : Dialog
    {
        #region Fields
        private const int LargeSeperation = 10;
        private const int SmallSeperation = 5;
        private TextButton OKButton;
        private TextButton cancelButton;
        private TextBox nameTextBox;
        #endregion

        #region Properties
        public string Name
        {
            get { return nameTextBox.Text; }
        }
        #endregion

        #region Constructors
        public UserDetailsDialog(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            // Name label
            Label nameLabel = new Label(game, guiManager);
            Add(nameLabel);
            nameLabel.Text = "Name:";
            nameLabel.X = LargeSeperation;
            nameLabel.Y = LargeSeperation;
            nameLabel.Width = 75;
            nameLabel.Height = nameLabel.TextHeight;

            // Name textbox
            this.nameTextBox = new TextBox(game, guiManager);
            Add(this.nameTextBox);
            this.nameTextBox.Initialize();
            this.nameTextBox.X = nameLabel.X;
            this.nameTextBox.Y = nameLabel.Y
                + nameLabel.Height
                + SmallSeperation;
            
            // Set the window width to the default textbox width
            ClientWidth = nameTextBox.Width + (2 * LargeSeperation);

            // Cancel button
            this.cancelButton = new TextButton(game, guiManager);
            Add(this.cancelButton);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.X = ClientWidth - this.cancelButton.Width - LargeSeperation;
            this.cancelButton.Y = this.nameTextBox.Y + this.nameTextBox.Height + LargeSeperation;
            this.cancelButton.Click += new ClickHandler(OnButtonClicked);

            // OK button
            this.OKButton = new TextButton(game, guiManager);
            Add(this.OKButton);
            this.OKButton.Text = "OK";
            this.OKButton.X = this.cancelButton.X - SmallSeperation - this.OKButton.Width;
            this.OKButton.Y = this.cancelButton.Y;
            this.OKButton.Click += new ClickHandler(OnButtonClicked);

            // Set the window height to the amount needed to show all controls
            ClientHeight = this.OKButton.Y + this.OKButton.Height + LargeSeperation;

            // Set the window title
            TitleText = "User Details";

            // This dialog does not need to be resized by the user
            Resizable = false;

            CenterWindow();
        }
        #endregion

        #region Event Handlers
        protected void OnButtonClicked(UIComponent sender)
        {
            if (sender == this.OKButton)
                SetDialogResult(DialogResult.OK);

            CloseWindow();
        }
        #endregion
    }
}