#region Description
//-----------------------------------------------------------------------------
// File:        Cubie.cs
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
using XNALib;

namespace RubiksCube
{
    class Mesh
    {
        public Matrix InitialTransform { get; set; }
        public ModelMesh ModelMesh { get; set; }
        public void Reset()
        {
            ModelMesh.ParentBone.Transform = InitialTransform;
        }

        public void Transform(Matrix matrix)
        {
            ModelMesh.ParentBone.Transform = matrix * ModelMesh.ParentBone.Transform;
        }
    }

    class Cubie
    {
        public string Id { get; set; }
        public List<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }
        //public Matrix Original { get; set; }

        private List<Mesh> _meshes;
        private Matrix _transform = Matrix.Identity;
        private Vector3 _unitX = Vector3.UnitX;
        private Vector3 _unitY = Vector3.UnitY;
        private Vector3 _unitZ = Vector3.UnitZ;

        public Cubie(string id,  ModelMesh mesh)
        {
            _meshes = new List<Mesh>();
            _meshes.Add(new Mesh() { ModelMesh = mesh, InitialTransform = mesh.ParentBone.Transform });
            Id = id;
            //mesh.ParentBone.Transform;
            //Original = Mesh.ParentBone.Transform;
        }

        public void AddMesh(ModelMesh mesh)
        {
            _meshes.Add(new Mesh() { ModelMesh = mesh, InitialTransform = mesh.ParentBone.Transform });
        }

        private Cubie()
        {
        }
        /*
        public Cubie Clone()
        {
            Cubie cubie = new Cubie();
            cubie.Id = Id;
            cubie.
            cubie.Original = Original;
            cubie._transform = _transform;
            cubie._unitX = _unitX;
            cubie._unitY = _unitY;
            cubie._unitZ = _unitZ;
            return cubie;
        }
        */

        public void Reset()
        {
            //Mesh.ParentBone.Transform = Original;
            foreach (Mesh mesh in Meshes) mesh.Reset();
            

            _transform = Matrix.Identity;
            _unitX = Vector3.UnitX;
            _unitY = Vector3.UnitY;
            _unitZ = Vector3.UnitZ;
        }

        public void Rotate(Axis axis, float delta)
        {
            Quaternion rotationQ = Quaternion.Identity;
            switch (axis)
            {
                case Axis.X:
                    rotationQ *= Quaternion.CreateFromAxisAngle(_unitX, delta);
                    break;
                case Axis.Y:
                    rotationQ *= Quaternion.CreateFromAxisAngle(_unitY, delta);
                    break;
                case Axis.Z:
                    rotationQ *= Quaternion.CreateFromAxisAngle(_unitZ, delta);
                    break;

            }
            Matrix matrix = Matrix.CreateFromQuaternion(rotationQ);
            //Mesh.ParentBone.Transform = matrix * Mesh.ParentBone.Transform;
            foreach (Mesh mesh in Meshes) mesh.Transform(matrix);
        }

        public void RotateUnit(Axis axis, float rotation)
        {
            Quaternion rotationQ = Quaternion.Identity;
            Matrix transform = Matrix.Identity;
            switch (axis)
            {
                case Axis.X:
                    transform = Matrix.CreateRotationX(rotation);
                    break;
                case Axis.Y:
                    transform = Matrix.CreateRotationY(rotation);
                    break;
                case Axis.Z:
                    transform = Matrix.CreateRotationZ(rotation);
                    break;

            }
            _transform = _transform * transform;
            transform = Matrix.Invert(_transform);
            _unitX = Vector3.Transform(Vector3.UnitX, transform);
            _unitY = Vector3.Transform(Vector3.UnitY, transform);
            _unitZ = Vector3.Transform(Vector3.UnitZ, transform);
        }

        /*
        public Matrix Transform
        {
            get { return _transform; }
        }

        public Matrix RotateToWorld(Axis axis, float delta)
        {

            switch (axis)
            {
                case Axis.X:
                    _transform = Matrix.CreateRotationX(delta) * _transform;
                    break;
                case Axis.Y:
                    _transform = Matrix.CreateRotationY(delta) * _transform;
                    break;
                case Axis.Z:
                    _transform = Matrix.CreateRotationZ(delta) * _transform;
                    break;

            }
            return _transform;
        }
        */

    }
}
