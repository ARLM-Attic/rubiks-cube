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
    public class Collision
    {
        public static bool RayTriangleIntersect(Ray r,
                            Vector3 vert0, Vector3 vert1, Vector3 vert2,
                            out float t)
        {
            t = 0;

            Vector3 edge1 = vert1 - vert0;
            Vector3 edge2 = vert2 - vert0;

            Vector3 tvec, pvec, qvec;
            float det, inv_det;

            pvec = Vector3.Cross(r.Direction, edge2);

            det = Vector3.Dot(edge1, pvec);

            if (det > -0.00001f)
                return false;

            inv_det = 1.0f / det;

            tvec = r.Position - vert0;

            float u = Vector3.Dot(tvec, pvec) * inv_det;
            if (u < -0.001f || u > 1.001f)
                return false;

            qvec = Vector3.Cross(tvec, edge1);

            float v = Vector3.Dot(r.Direction, qvec) * inv_det;
            if (v < -0.001f || u + v > 1.001f)
                return false;

            t = Vector3.Dot(edge2, qvec) * inv_det;

            if (t <= 0)
                return false;

            return true;
        }

        public struct VertexPositionNormal
        {
            public Vector3 Position;
            public Vector3 Normal;
        }

        public static bool SelectMesh(Model m, Matrix transform,
                                 Ray clickRay,
                                 Func<ModelMesh, Ray, float?>  Intersect,
                                 out ModelMesh selectedMesh,
                                 out float closestDistance
                                 )
        {

            selectedMesh = null;
            closestDistance = float.MaxValue;

            /*
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);
            */


            //Transform the ray into the model's local space. This will keep us from having to transform every polygon into world space during the collision testing process. Simply take the inverse of the mesh's world transform and apply it to the ray position and direction. Note that the Direction is transformed using Vector3.TransformNormal. This is to prevent the translation and scaling of the matrix from adjusting the ray direction. 


            Matrix mat = Matrix.Invert(transform);
            clickRay.Position = Vector3.Transform(clickRay.Position, mat);
            clickRay.Direction = Vector3.TransformNormal(clickRay.Direction, mat);



            //Now lets loop thru the meshes that comprise this model so we can do the ray intersection testing.

            
            for (int i = 0; i < m.Meshes.Count; i++)
            {
                ModelMesh mesh = m.Meshes[i];

                //Matrix absTransform = transforms[mesh.ParentBone.Index];

                //First we test the ray against the bounding sphere of the mesh. Since the BoundingSphere.Intersects function returns a float? (Nullable float) we need to check the HasValue member variable of the float? to see if it is null or not. If the value is null (HasValue == false) then we know the ray does not intersect the model.
                //BoundingBox box = XNAUtils.CreateBoxFromSphere(mesh.BoundingSphere);
                float? distance = Intersect(mesh, clickRay);// clickRay.Intersects(box);
                if (distance.HasValue == false)
                {
                    continue;
                }

                if (distance < closestDistance)
                {

                    selectedMesh = mesh;
                    closestDistance = (float)distance;
                }
                //Debug.WriteLine(mesh.Name);
                /*

                //The bunny model's VertexDeclaration consists of two elements: a Position and Normal. Lets create a buffer that can store the vertices in the mesh and populate it with the vertex buffer data.
                VertexPositionNormal[] vertices = new VertexPositionNormal[
                    mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride];

                mesh.VertexBuffer.GetData<VertexPositionNormal>(vertices);



                //Since an index buffer can either be of type short or type int, we need to check wich type the models index buffer is and then lock the data, iterate thru the TriangleList and test the Ray against each polygon in the model. If we find a collider then we test to see if it is the closest collider to the ray. If it is then the model index and polygon index are stored in the output parameters.


                if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
                {
                    short[] indices = new short[mesh.IndexBuffer.SizeInBytes / sizeof(short)];

                    mesh.IndexBuffer.GetData<short>(indices);

                    for (int x = 0; x < indices.Length; x += 3)
                    {
                        float fDist;

                        if (RayTriangleIntersect(
                            clickRay,
                            Vector3.Transform(vertices[indices[x + 0]].Position, absTransform),
                            Vector3.Transform(vertices[indices[x + 1]].Position, absTransform),
                            Vector3.Transform(vertices[indices[x + 2]].Position, absTransform),
                            out fDist))
                        {
                            if (fDist < fClosestPoly)
                            {
                                meshIndex = i;
                                polyIndex = x / 3;

                                fClosestPoly = fDist;
                                Debug.WriteLine(string.Format("mesh: {0}, bone: {1}, meshname:{2}", meshIndex, mesh.ParentBone.Index, mesh.Name));

                            }
                        }
                    }
                }
                else if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.ThirtyTwoBits)
                {
                    int[] indices = new int[mesh.IndexBuffer.SizeInBytes / sizeof(int)];

                    mesh.IndexBuffer.GetData<int>(indices);


                    for (int x = 0; x < indices.Length; x += 3)
                    {
                        float fDist;

                        if (RayTriangleIntersect(
                            clickRay,
                            vertices[indices[x + 0]].Position,
                            vertices[indices[x + 1]].Position,
                            vertices[indices[x + 2]].Position,
                            out fDist))
                        {
                            if (fDist < fClosestPoly)
                            {
                                meshIndex = i;
                                polyIndex = x / 3;

                                fClosestPoly = fDist;
                            }
                        }
                    }
                }*/

            }
            return selectedMesh != null;
        }
    }
}
