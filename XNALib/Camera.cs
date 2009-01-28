#region Description
//-----------------------------------------------------------------------------
// File:        Camera.cs
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

namespace XNALib
{
    public class Camera 
    {
        /*
        private Vector3 _position;
        private Vector3 _lookAt;
        private Vector3 _up;
        private Matrix _viewMatrix;
        */
        private Vector3 _position;
        private Vector3 _lookAt;
        private Vector3 _up;
        //Vector3 cameraPosition;
        Matrix viewMatrix;
        MouseState originalMouseState;

        public Camera(Vector3 position, Vector3 lookAt, Vector3 up) 
        {
            
            _position = position;
            _lookAt = lookAt;
            _up = up;
            
            //cameraPosition = new Vector3(1,1,10);
            Rotate(0f, 0f, 0f);

//Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            originalMouseState = Mouse.GetState();
            //Update();
        }
        public Matrix ViewMatrix
        {
            get { return this.viewMatrix; }
        }
       
        public void Rotate(float pan, float tile, float roll)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(-tile) * 
            Matrix.CreateRotationY(-pan);
            _position = Vector3.Transform(_position, cameraRotation);
            viewMatrix = Matrix.CreateLookAt(_position, 
            _lookAt, _up);
        }

        public void Translate(float right, float up, float forward)
        {
        }
        /*
        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            float moveSpeed = 0.5f;
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * 
            Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            _position += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }
         */
    }
}
