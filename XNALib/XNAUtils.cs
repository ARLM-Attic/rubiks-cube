using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace XNALib
{
    public static class XNAUtils
    {
        public static BoundingBox TransformBoundingBox(BoundingBox origBox, Matrix matrix)
        {
            Vector3 origCorner1 = origBox.Min;
            Vector3 origCorner2 = origBox.Max;

            Vector3 transCorner1 = Vector3.Transform(origCorner1, matrix);
            Vector3 transCorner2 = Vector3.Transform(origCorner2, matrix);

            return new BoundingBox(transCorner1, transCorner2);
        }

        public static BoundingSphere TransformBoundingSphere(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
        {
            Vector3 trans;
            Vector3 scaling;
            Quaternion rot;
            transformationMatrix.Decompose(out scaling, out rot, out trans);

            float maxScale = scaling.X;
            if (maxScale < scaling.Y)
                maxScale = scaling.Y;
            if (maxScale < scaling.Z)
                maxScale = scaling.Z;

            float transformedSphereRadius = originalBoundingSphere.Radius * maxScale;
            Vector3 transformedSphereCenter = Vector3.Transform(originalBoundingSphere.Center, transformationMatrix);

            BoundingSphere transformedBoundingSphere = new BoundingSphere(transformedSphereCenter, transformedSphereRadius);

            return transformedBoundingSphere;
        }

        public static Model LoadModelWithBoundingSphere(out Matrix[] modelTransforms, string asset, ContentManager content)
        {
            Model newModel = content.Load<Model>(asset);

            modelTransforms = new Matrix[newModel.Bones.Count];
            newModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

            BoundingSphere completeBoundingSphere = new BoundingSphere();
            foreach (ModelMesh mesh in newModel.Meshes)
            {
                BoundingSphere origMeshSphere = mesh.BoundingSphere;
                BoundingSphere transMeshSphere = XNAUtils.TransformBoundingSphere(origMeshSphere, modelTransforms[mesh.ParentBone.Index]);
                completeBoundingSphere = BoundingSphere.CreateMerged(completeBoundingSphere, transMeshSphere);
            }
            newModel.Tag = completeBoundingSphere;

            return newModel;
        }


        public static Matrix[] AutoScale(Model model, float requestedSize)
        {
            BoundingSphere bSphere = (BoundingSphere)model.Tag;
            float originalSize = bSphere.Radius * 2;
            float scalingFactor = requestedSize / originalSize;
            model.Root.Transform = model.Root.Transform * Matrix.CreateScale(scalingFactor);
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            return modelTransforms;
        }

        public static void DrawBoundingBox(BoundingBox bBox, GraphicsDevice device, BasicEffect basicEffect, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Vector3 v1 = bBox.Min;
            Vector3 v2 = bBox.Max;

            VertexPositionColor[] cubeLineVertices = new VertexPositionColor[8];
            cubeLineVertices[0] = new VertexPositionColor(v1, Color.White);
            cubeLineVertices[1] = new VertexPositionColor(new Vector3(v2.X, v1.Y, v1.Z), Color.Red);
            cubeLineVertices[2] = new VertexPositionColor(new Vector3(v2.X, v1.Y, v2.Z), Color.Green);
            cubeLineVertices[3] = new VertexPositionColor(new Vector3(v1.X, v1.Y, v2.Z), Color.Blue);

            cubeLineVertices[4] = new VertexPositionColor(new Vector3(v1.X, v2.Y, v1.Z), Color.White);
            cubeLineVertices[5] = new VertexPositionColor(new Vector3(v2.X, v2.Y, v1.Z), Color.Red);
            cubeLineVertices[6] = new VertexPositionColor(v2, Color.Green);
            cubeLineVertices[7] = new VertexPositionColor(new Vector3(v1.X, v2.Y, v2.Z), Color.Blue);

            short[] cubeLineIndices = { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 };

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            device.RenderState.FillMode = FillMode.Solid;            
            basicEffect.Begin();
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
                device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, cubeLineVertices, 0, 8, cubeLineIndices, 0, 12);
                pass.End();
            }
            basicEffect.End();
        }

        public static void DrawSphereSpikes(BoundingSphere sphere, GraphicsDevice device, BasicEffect basicEffect, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Vector3 up = sphere.Center + sphere.Radius * Vector3.Up;
            Vector3 down = sphere.Center + sphere.Radius * Vector3.Down;
            Vector3 right = sphere.Center + sphere.Radius * Vector3.Right;
            Vector3 left = sphere.Center + sphere.Radius * Vector3.Left;
            Vector3 forward = sphere.Center + sphere.Radius * Vector3.Forward;
            Vector3 back = sphere.Center + sphere.Radius * Vector3.Backward;

            VertexPositionColor[] sphereLineVertices = new VertexPositionColor[6];
            sphereLineVertices[0] = new VertexPositionColor(up, Color.White);
            sphereLineVertices[1] = new VertexPositionColor(down, Color.White);
            sphereLineVertices[2] = new VertexPositionColor(left, Color.White);
            sphereLineVertices[3] = new VertexPositionColor(right, Color.White);
            sphereLineVertices[4] = new VertexPositionColor(forward, Color.White);
            sphereLineVertices[5] = new VertexPositionColor(back, Color.White);

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;            
            basicEffect.Begin();
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, sphereLineVertices, 0, 3);
                pass.End();
            }
            basicEffect.End();
            
        }

        public static VertexPositionColor[] VerticesFromVector3List(List<Vector3> pointList, Color color)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[pointList.Count];

            int i = 0;
            foreach (Vector3 p in pointList)
                vertices[i++] = new VertexPositionColor(p, color);

            return vertices;
        }

        public static BoundingBox CreateBoxFromSphere(BoundingSphere sphere)
        {
            float radius = sphere.Radius * (float)Math.Sqrt(2.0) / 2.0f;
            Vector3 outerPoint = new Vector3(radius, radius, radius);

            Vector3 p1 = sphere.Center - outerPoint;
            Vector3 p2 = sphere.Center + outerPoint;

            return new BoundingBox(p1, p2);
        }

        public static T[] Reshape2Dto1D<T>(T[,] array2D)
        {
            int width = array2D.GetLength(0);
            int height = array2D.GetLength(1);
            T[] array1D = new T[width * height];

            int i = 0;
            for (int z = 0; z < height; z++)
                for (int x = 0; x < width; x++)
                    array1D[i++] = array2D[x, z];

            return array1D;
        }

        public static T[,] Reshape1Dto2D<T>(T[] vertices, int width, int height)
        {
            T[,] vertexArray = new T[width, height];
            int i = 0;
            for (int h = 0; h < height; h++)
                for (int w = 0; w < width; w++)
                    vertexArray[w, h] = vertices[i++];

            return vertexArray;
        }


        public static Ray CreateRayFrom2D(GraphicsDevice graphicsDevice, Vector2 point, Matrix projection, Matrix view, Matrix world)
        {
            Vector3 rayNear = graphicsDevice.Viewport.Unproject(
                                    new Vector3(point.X, point.Y, 0f),
                                    projection, view, Matrix.Identity);

            Vector3 rayFar = graphicsDevice.Viewport.Unproject(
                    new Vector3(point.X, point.Y, 1f),
                    projection, view, world);

            Vector3 direction = rayFar - rayNear;
            direction.Normalize();
            //Debug.WriteLine(string.Format("start:{0}, direction:{1}", rayNear, direction));
            Ray ray = new Ray(rayNear, direction);
            return ray;
        }
    }
}