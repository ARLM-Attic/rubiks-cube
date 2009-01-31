#region Description
//-----------------------------------------------------------------------------
// File:        CubeConfiguration.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using XNALib;
using CubicleKey = System.String;

namespace RubiksCube
{
    class CubeConfiguration
    {
        private Dictionary<CubicleKey, Cubicle> _cubeConfiguration;
        private List<ModelMesh> _nonCubeMeshs;
        private static string[] Faces = new string[] { "F", "B", "U", "D", "L", "R" };

        public Cubicle this[CubicleKey key]{
            get
            {
                return _cubeConfiguration[key];
            }
        }

        public CubeConfiguration()
        {
            
        }

        private string GetKey(string meshName)
        {
            string key = meshName.Substring(5).ToUpper();
            int dot = key.IndexOf('.');
            if (dot >= 0)
                key = key.Substring(0, dot);
            return key; 
        }

        public void Init(Model model)
        {
            string key;
            Dictionary<CubicleKey, Cubicle> cubeConfiguration = new Dictionary<CubicleKey, Cubicle>();
            foreach (ModelMesh mesh in model.Meshes)
            {
                if (!mesh.Name.StartsWith("Cube."))
                {
                    //Debug.Assert(false);
                    if (_nonCubeMeshs == null) _nonCubeMeshs = new List<ModelMesh>();
                    _nonCubeMeshs.Add(mesh);
                }
                else
                {
                    key = GetKey(mesh.Name);
                    AddMesh(cubeConfiguration, key, mesh);
                    //cubeConfiguration.Add(key, new Cubicle(new Cubie(key, mesh)));
                }
            }
            _cubeConfiguration = cubeConfiguration;
        }

        private void AddMesh(Dictionary<CubicleKey, Cubicle> cubeConfiguration, string cubieName, ModelMesh mesh)
        {
            Cubie cubie = null;
            if (!cubeConfiguration.ContainsKey(cubieName))
                cubeConfiguration.Add(cubieName, new Cubicle(new Cubie(cubieName, mesh)));
            else
            {
                cubie = cubeConfiguration[cubieName].Cubie;
                cubie.AddMesh(mesh);
            }
        }

        public void Reset()
        {
            Dictionary<CubicleKey, Cubicle> cubeConfiguration = new Dictionary<CubicleKey, Cubicle>();
            foreach (Cubicle cubicle in _cubeConfiguration.Values)
            {
                Cubie cubie = cubicle.Cubie;//.Clone();
                cubie.Reset();
                cubeConfiguration.Add(cubie.Id, new Cubicle(cubie));
            }
            _cubeConfiguration = cubeConfiguration;
        }

        public bool IsFaceSoved(string face)
        {
            IEnumerable<Cubicle> cubicles = _cubeConfiguration.Where(p => p.Key.IndexOf(face) >= 0).Select(kv => kv.Value);
            string faceToMatch = _cubeConfiguration[face].Cubie.Id;
            return (cubicles.FirstOrDefault(c => c.Cubie.Id.IndexOf(faceToMatch) < 0) == null); //no matched
        }

        public bool IsSolved()
        {
            foreach (string face in Faces)
            {
                if (!IsFaceSoved(face)) return false;
            }
            return true;
        }


        private Axis GetClosestAxis(Vector3 v1, Vector3 v2, Axis axis)
        {
            float dx = Math.Abs(v1.X - v2.X);
            float dy = Math.Abs(v1.Y - v2.Y);
            float dz = Math.Abs(v1.Z - v2.Z);
            switch (axis)
            {
                case Axis.X: //return YZ
                    return (dy < dz) ? Axis.Z : Axis.Y;
                case Axis.Y: //return YZ
                    return (dz < dx) ? Axis.X : Axis.Z;
                case Axis.Z: //return YZ
                    return (dx < dy) ? Axis.Y : Axis.X;
            }
            return Axis.X;
        }

        private Axis GetClosestAxis(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Ray ray)
        {
            //v1, v2, v4 --YZ
            //v2, v3, v4 --ZX
            //v3, v1, v4 --XY
            Axis[] axises = new Axis[]{Axis.X, Axis.Y, Axis.Z};
            Vector3[,] triangles = new Vector3[,]{ { v1, v2, v4}, {v2, v3, v4}, {v3, v1, v4 }};

            Axis axis = Axis.X;
            int min = -1;
            float distance = float.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                
                float t, u, v;
                if (Helper3D.RayTriangleIntersect(ray.Position, ray.Direction, triangles[i,0], triangles[i,1], triangles[i,2], out t, out u, out v, false))
                {
                    if (t < distance)
                    {
                        distance = t;
                        min = i;
                    }
                }
            }

            if (min >= 0)
            {
                axis = GetClosestAxis(ray.Position + ray.Position * distance, v4, axises[min]);
            }
            return axis;
        }

        private Axis GetClosestAxis(Cubicle cubicle, Ray ray)
        {
            //Debug.WriteLine(cubicle.BoundingBox.ToString());
            //UFL-U
            //URF-F
            //UBR-R
            //ULB-B

            Vector3 v1 = Vector3.Zero; 
            Vector3 v2 = Vector3.Zero; 
            Vector3 v3 = Vector3.Zero; 
            Vector3 v4 = Vector3.Zero;
            Vector3 vmin = cubicle.BoundingBox.Min;
            Vector3 vmax = cubicle.BoundingBox.Max;
            switch (cubicle.Id)
            {
                case "UFL":
                    v1 = new Vector3(vmin.X, vmin.Y, vmax.Z);
                    v2 = new Vector3(vmin.X, vmax.Y, vmin.Z);
                    v3 = vmax;
                    v4 = new Vector3(vmin.X, vmax.Y, vmax.Z);
                    break;
                case "URF":
                    v1 = new Vector3(vmax.X, vmin.Y, vmax.Z);
                    v2 = new Vector3(vmax.X, vmax.Y, vmin.Z);
                    v3 = new Vector3(vmin.X, vmax.Y, vmax.Z);
                    v4 = vmax;
                    break;
                case "UBR":
                    v1 = new Vector3(vmax.X, vmin.Y, vmin.Z);
                    v2 = vmax;
                    v3 = new Vector3(vmin.X, vmax.Y, vmin.Z);
                    v4 = new Vector3(vmax.X, vmax.Y, vmin.Z);
                    break;
                case "ULB":
                    v1 = vmin;
                    v2 = new Vector3(vmin.X, vmax.Y, vmax.Z);
                    v3 = new Vector3(vmax.X, vmax.Y, vmin.Z);
                    v4 = new Vector3(vmin.X, vmax.Y, vmin.Z);
                    break;

                //DLF-D
                //DFR-R
                //DRB-B
                //DBL-L
                case "DLF":
                    v1 = vmin;
                    v2 = new Vector3(vmin.X, vmax.Y, vmax.Z);
                    v3 = new Vector3(vmax.X, vmin.Y, vmax.Z);
                    v4 = new Vector3(vmin.X, vmin.Y, vmax.Z);
                    break;
                case "DFR":
                    v1 = new Vector3(vmax.X, vmin.Y, vmin.Z);
                    v2 = vmax;
                    v3 = new Vector3(vmin.X, vmin.Y, vmax.Z);
                    v4 = new Vector3(vmax.X, vmin.Y, vmax.Z);
                    break;
                case "DRB":
                    v1 = new Vector3(vmax.X, vmax.Y, vmin.Z);
                    v2 = new Vector3(vmax.X, vmin.Y, vmax.Z);
                    v3 = vmin;
                    v4 = new Vector3(vmax.X, vmin.Y, vmin.Z);
                    break;
                case "DBL":
                    v1 = new Vector3(vmin.X, vmax.Y, vmin.Z);
                    v2 = new Vector3(vmin.X, vmin.Y, vmax.Z);
                    v3 = new Vector3(vmax.X, vmin.Y, vmin.Z);
                    v4 = vmin;
                    break;
            }
            return GetClosestAxis(v1, v2, v3, v4, ray);
        }

        public bool SelectCubicle(
                                 Ray clickRay,
                                 out Cubicle selectedCubicle,
                                 out string basicOp,
                                 out float closestDistance
                                 )
        {

            selectedCubicle = null;
            closestDistance = float.MaxValue;
            basicOp = string.Empty;
            /*
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);
            */


            //Transform the ray into the model's local space. This will keep us from having to transform every polygon into world space during the collision testing process. Simply take the inverse of the mesh's world transform and apply it to the ray position and direction. Note that the Direction is transformed using Vector3.TransformNormal. This is to prevent the translation and scaling of the matrix from adjusting the ray direction. 

            /*
            Matrix mat = Matrix.Invert(transform);
            clickRay.Position = Vector3.Transform(clickRay.Position, mat);
            clickRay.Direction = Vector3.TransformNormal(clickRay.Direction, mat);
            */


            //for (int i = 0; i < m.Meshes.Count; i++)
            foreach(Cubicle cubicle in _cubeConfiguration.Values)
            {
                float? distance = cubicle.BoundingBox.Intersects(clickRay);// Intersect(cubicle, clickRay);// clickRay.Intersects(box);
                if (distance.HasValue == false)
                {
                    continue;
                }

                if (distance < closestDistance)
                {

                    selectedCubicle = cubicle;
                    closestDistance = (float)distance;
                }
                //Debug.WriteLine(mesh.Name);
            }

            if (selectedCubicle != null)
            {
                string selectName = selectedCubicle.Id;
                if (Cubicle.BasicOpFromSelectedCubicle.ContainsKey(selectName))
                {
                    if(selectName.Length<3)
                        basicOp = Cubicle.BasicOpFromSelectedCubicle[selectName];
                    else {
                        Axis axis = GetClosestAxis(selectedCubicle, clickRay);
                        basicOp = Cubicle.BasicOpFromAxis[axis].First(op => selectName.IndexOf(op) >= 0);
                    }

                }
            }
            return selectedCubicle != null;
        }
        //private int debug = 0;
        public void DrawModel(Model model, IEnumerable<Cubie> selectedCubies, Matrix worldMatrix, Matrix[] modelTransforms, Matrix viewMatrix, Matrix projectionMatrix)
        {
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            foreach (Cubicle cubicle in _cubeConfiguration.Values)
            {
                List<Mesh> meshes = cubicle.Cubie.Meshes;
                foreach (Mesh mesh in meshes)
                {
                    ModelMesh modelMesh = mesh.ModelMesh;
                    Matrix transform = modelTransforms[modelMesh.ParentBone.Index] * worldMatrix;
                    foreach (BasicEffect effect in modelMesh.Effects)
                    {
                        if (selectedCubies != null && selectedCubies.Contains(cubicle.Cubie))
                        //if(meshIndex == selectedMesh[0])
                        {
                            effect.AmbientLightColor = Vector3.One;
                        }
                        else
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;
                        }
                        effect.World = transform;
                        effect.View = viewMatrix;
                        effect.Projection = projectionMatrix;
                    }
                    modelMesh.Draw();
                    
                }
            }

            if (_nonCubeMeshs != null)
            {
                foreach (ModelMesh mesh in _nonCubeMeshs)
                {
                    Matrix transform = modelTransforms[mesh.ParentBone.Index] * worldMatrix;
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = transform;
                        effect.View = viewMatrix;
                        effect.Projection = projectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }

    }
}
