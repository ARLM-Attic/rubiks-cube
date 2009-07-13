#region Description
//-----------------------------------------------------------------------------
// File:        CubieFace.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using System.Windows;
using Knoics.Math;
using Kit3D.Windows.Media.Media3D;


namespace Knoics.RubiksCube
{
    public class CubieFace : ITransform
    {
        //public IMesh       MainMesh { get; set; }

        private IMesh _faceMesh;
        private List<IMesh> _meshes;
        public List<IMesh> Meshes { get { return _meshes; } }
        public IMesh FaceMesh { get { return _faceMesh; } }
        public CubicleFace CubicleFace { get; set; }

        private Matrix3D _transform = Matrix3D.Identity;
        private Matrix3D _savedTransform = Matrix3D.Identity;
        private Matrix3D _unfoldTransform = Matrix3D.Identity;
        public Matrix3D Transform{
            get { return _transform; }
        }
        private string _name;
        private Vector3D _center;
        private Cubie _cubie;
        private double _size;
        public Cubie Cubie { get { return _cubie; } }
        public string Name { get { return _name; } }
        private CubieFace(Cubie cubie, string name, Vector3D center, double size)
        {
            _meshes = new List<IMesh>();
            _center = center;
            _name = name;
            _size = size;
            _cubie = cubie;
        }

        public static CubieFace ConstructCubieFace(Cubie cubie, string name, Vector3D center, double size)
        {
            CubieFace face = new CubieFace(cubie, name, center, size);
            double edgeWidth = size * 0.05;
            Size faceSize = new Size(size, size);

            IMesh u = CubeConfiguration.Factory.CreateMesh(face, ConstructVertexes(CubeConfiguration.Faces[name].Normal, center, faceSize, edgeWidth), CubeConfiguration.Faces[name].Color);
            face._faceMesh = u; face._meshes.Add(u);

            Vector3D[] decorCenters = new Vector3D[4];
            Size[] sizes = new Size[4];
            if (name == "U" || name == "D")
            {
                //decorator:
                Vector3D decorCenter;
                decorCenter = center; decorCenter.Z += size / 2 - edgeWidth / 2; decorCenters[0] = decorCenter; sizes[0] = new Size(edgeWidth, size);
                decorCenter = center; decorCenter.Z -= size / 2 - edgeWidth / 2; decorCenters[1] = decorCenter; sizes[1] = new Size(edgeWidth, size);
                decorCenter = center; decorCenter.X += size / 2 - edgeWidth / 2; decorCenters[2] = decorCenter; sizes[2] = new Size(size,edgeWidth);
                decorCenter = center; decorCenter.X -= size / 2 - edgeWidth / 2; decorCenters[3] = decorCenter; sizes[3] = new Size(size, edgeWidth);
            }
            else if (name == "F" || name == "B")
            {
                //decorator:
                Vector3D decorCenter;
                decorCenter = center; decorCenter.X += size / 2 - edgeWidth / 2; decorCenters[0] = decorCenter; sizes[0] = new Size(edgeWidth, size);
                decorCenter = center; decorCenter.X -= size / 2 - edgeWidth / 2; decorCenters[1] = decorCenter; sizes[1] = new Size(edgeWidth, size);
                decorCenter = center; decorCenter.Y += size / 2 - edgeWidth / 2; decorCenters[2] = decorCenter; sizes[2] = new Size(size, edgeWidth);
                decorCenter = center; decorCenter.Y -= size / 2 - edgeWidth / 2; decorCenters[3] = decorCenter; sizes[3] = new Size(size, edgeWidth);
            }
            else if (name == "L" || name == "R")
            {
                //decorator:
                Vector3D decorCenter;
                decorCenter = center; decorCenter.Y += size / 2 - edgeWidth / 2; decorCenters[0] = decorCenter; sizes[0] = new Size(edgeWidth, size);
                decorCenter = center; decorCenter.Y -= size / 2 - edgeWidth / 2; decorCenters[1] = decorCenter; sizes[1] = new Size(edgeWidth, size);
                decorCenter = center; decorCenter.Z += size / 2 - edgeWidth / 2; decorCenters[2] = decorCenter; sizes[2] = new Size(size, edgeWidth);
                decorCenter = center; decorCenter.Z -= size / 2 - edgeWidth / 2; decorCenters[3] = decorCenter; sizes[3] = new Size(size, edgeWidth);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }

            
            for(int i = 0; i<decorCenters.Length; i++)
            {
                face._meshes.Add(CubeConfiguration.Factory.CreateMesh(face, ConstructVertexes(CubeConfiguration.Faces[name].Normal, decorCenters[i], sizes[i], 0), Colors.Black));
            }
            return face;
        }

        public void Save()
        {
            _savedTransform = _transform;
            foreach (IMesh mesh in Meshes) mesh.Save();
        }

        public void Restore()
        {
            _transform = _savedTransform;
            foreach (IMesh mesh in Meshes) mesh.Restore();
        }

        public void DoTransform(Matrix3D matrix, bool isFromSaved)
        {
            if (isFromSaved)
                _transform = matrix * _savedTransform;
            else
                _transform = matrix * _transform;

            foreach (IMesh mesh in Meshes) mesh.DoTransform(matrix, isFromSaved);
        }

        public void Reset()
        {
            _transform = Matrix3D.Identity;
            _savedTransform = Matrix3D.Identity;
            _unfoldTransform = Matrix3D.Identity;
            foreach (IMesh mesh in Meshes)
            {
                mesh.Reset();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", _cubie, _name);
            
        }

        private static Vector3D[] ConstructVertexes(Axis axis, Vector3D center, Size size, double edgeWidth)
        {

            Vector3D[] positions = new Vector3D[4];
            Vector3D vertex = center;
            size.Width -= edgeWidth * 2; size.Height -= edgeWidth * 2;


            switch (axis)
            {
                case Axis.X:
                    //right-bottom
                    vertex.Y -= size.Width / 2; vertex.Z -= size.Height / 2; positions[0] = vertex;// Add(vertex);
                    //right-top
                    vertex.Y += size.Width; positions[1] = vertex;// .Add(vertex);
                    //left-top
                    vertex.Z += size.Height; positions[2] = vertex;//.Add(vertex);
                    //left-bottom
                    vertex.Y -= size.Width; positions[3] = vertex;//.Add(vertex);
                    break;
                case Axis.Y:

                    vertex.Z -= size.Width / 2; vertex.X -= size.Height / 2; positions[0] = vertex;//.Add(vertex);
                    vertex.Z += size.Width; positions[1] = vertex;// .Add(vertex);
                    vertex.X += size.Height; positions[2] = vertex;//.Add(vertex);
                    vertex.Z -= size.Width; positions[3] = vertex;//.Add(vertex);
                    break;
                case Axis.Z:
                    vertex.X -= size.Width / 2; vertex.Y -= size.Height / 2; positions[0] = vertex;//.Add(vertex);
                    vertex.X += size.Width; positions[1] = vertex;// .Add(vertex);
                    vertex.Y += size.Height; positions[2] = vertex;// .Add(vertex);
                    vertex.X -= size.Width; positions[3] = vertex;// .Add(vertex);
                    break;
            }
            return positions;
        }
    }
}
