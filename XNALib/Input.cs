#region Description
//-----------------------------------------------------------------------------
// File:        Input.cs
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

namespace XNALib
{

    public class Input : IInput
    {
        private MouseState _originalMouseState;
        float _leftrightRot;

        public float Pan {get;set;}
        public float Tilt {get;set;}
        public float Roll { get; set; }


        public float Forward { get; set; }
        public float Right { get; set; }
        public float Up { get; set; }



        public Vector2 Selected { get; set; }
        public Vector2 DragFrom { get; set; }
        public Vector2 DragTo { get; set; }

        public Input()
        {
            _originalMouseState = Mouse.GetState();
            Reset();
        }

        public void Reset()
        {
            Pan = 0f;
            Tilt = 0f;
            Roll = 0f;

            Forward = 0f;
            Right = 0f;
            Up = 0f;
        }

        public InputMode Update(Game game, Matrix projection, Matrix view)
        {
            MouseState currentMouseState = Mouse.GetState();
            //float moveSpeed = 0.01f;
            float rotationSpeed = 0.001f;
            InputMode mode = InputMode.NoInput;
            if (currentMouseState != _originalMouseState)
            {

                if (currentMouseState.RightButton == ButtonState.Pressed && _originalMouseState.RightButton == ButtonState.Pressed)
                {
                    float xDifference = currentMouseState.X - _originalMouseState.X;
                    float yDifference = currentMouseState.Y - _originalMouseState.Y;

                    Pan = rotationSpeed * xDifference;
                    Tilt = rotationSpeed * yDifference;
                }


                if (currentMouseState.LeftButton == ButtonState.Pressed && _originalMouseState.LeftButton == ButtonState.Released)
                {
                    Selected = new Vector2(currentMouseState.X, currentMouseState.Y);
                    DragFrom = Selected;
                    mode = InputMode.Select;
                }

                if (currentMouseState.LeftButton == ButtonState.Released && _originalMouseState.LeftButton == ButtonState.Pressed)
                {
                    DragTo =new Vector2(currentMouseState.X, currentMouseState.Y);
                    mode = InputMode.Drag;
                }

                    /*
                else if (currentMouseState.MiddleButton == ButtonState.Pressed && _originalMouseState.MiddleButton == ButtonState.Pressed)
                {
                    float xDifference = currentMouseState.X - _originalMouseState.X;
                    float yDifference = currentMouseState.Y - _originalMouseState.Y;
                    Right = moveSpeed * xDifference;
                    Up = moveSpeed * yDifference;
                }
                */
                _originalMouseState = currentMouseState;
                //Mouse.SetPosition(clientBounds.Width / 2,             clientBounds.Height / 2);
                
            }
            return mode;
        }
    }
}
