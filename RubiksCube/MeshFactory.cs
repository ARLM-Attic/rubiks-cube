using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Knoics.RubiksCube;
using System.Diagnostics;
using CMatrix = Knoics.Math.Matrix;

namespace RubiksCubeWindows
{
    class MeshFactory : IMeshFactory
    {

        #region IMeshFactory Members

        //public IMesh CreateMesh(Axis axis, Position center, MeshSize size, double edgeWidth, MeshColor color)
        public IMesh CreateMesh(Position[] vertexes, MeshColor color)
        {
            return new Mesh(vertexes, color);
        }

        public IMesh CreateMesh(object mesh)
        {
            throw new NotImplementedException();//return new ImportMesh((ModelMesh)mesh);
        }

        #endregion
    }

    class MathConverter
    {
        public static Matrix ToXNAMatrix(CMatrix matrix)
        {
            Matrix m = new Matrix();
            m.M11 = matrix.M11; m.M12 = matrix.M12; m.M13 = matrix.M13; m.M14 = matrix.M14;
            m.M21 = matrix.M21; m.M22 = matrix.M22; m.M23 = matrix.M23; m.M24 = matrix.M24;
            m.M31 = matrix.M31; m.M32 = matrix.M32; m.M33 = matrix.M33; m.M34 = matrix.M34;
            m.M41 = matrix.M41; m.M42 = matrix.M42; m.M43 = matrix.M43; m.M44 = matrix.M44;
            return m;
        }
        public static CMatrix FromXNAMatrix(Matrix matrix)
        {
            CMatrix m = new CMatrix();
            m.M11 = matrix.M11; m.M12 = matrix.M12; m.M13 = matrix.M13; m.M14 = matrix.M14;
            m.M21 = matrix.M21; m.M22 = matrix.M22; m.M23 = matrix.M23; m.M24 = matrix.M24;
            m.M31 = matrix.M31; m.M32 = matrix.M32; m.M33 = matrix.M33; m.M34 = matrix.M34;
            m.M41 = matrix.M41; m.M42 = matrix.M42; m.M43 = matrix.M43; m.M44 = matrix.M44;
            return m;
        }
    }
#if XNA_MODEL
    class ImportMesh : IMesh
    {
        private Matrix InitialTransform { get; set; }
        private ModelMesh ModelMesh { get; set; }

        public ImportMesh(ModelMesh mesh)
        {
            ModelMesh = mesh;
            InitialTransform = mesh.ParentBone.Transform;
        }

        public void Reset()
        {
            ModelMesh.ParentBone.Transform = InitialTransform;
        }

        public void Transform(CMatrix matrix)
        {

            ModelMesh.ParentBone.Transform = matrix * ModelMesh.ParentBone.Transform;
        }

        #region IMesh Members


        public object MeshObject
        {
            get { return ModelMesh; }
        }

        #endregion

        #region IMesh Members

        public void Draw(CMatrix world, CMatrix view, CMatrix projection)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
#endif

    class Mesh : IMesh
    {
        BasicEffect _basicEffect;
        VertexDeclaration _vertexDeclaration;
        VertexBuffer _vertexBuffer;
        VertexPositionColor[] _vertexArray;
        IndexBuffer _indexBuffer;
        GraphicsDevice _gd;
        Matrix _transform = Matrix.Identity;
        public Mesh(Position[] vertexes, MeshColor meshColor)
        {
            GraphicsDevice gd = (GraphicsDevice)RubiksCube.GraphicsDevice;
            Debug.Assert(vertexes.Count() == 4);
            _gd = gd;
            _basicEffect = new BasicEffect(gd, null);
            _basicEffect.Alpha = 1.0F;
            _basicEffect.VertexColorEnabled = true;

            _vertexDeclaration = new VertexDeclaration(gd, VertexPositionColor.VertexElements);
            int vertexCount = vertexes.Count();// 4;// 24;
            _vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionColor), vertexCount, BufferUsage.WriteOnly);// ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);
            _vertexArray = new VertexPositionColor[vertexCount];

            Color color = MapToColor(meshColor);
            for (int i = 0; i < vertexCount; i++)
            {
                _vertexArray[i].Position = new Vector3((float)vertexes[i].X, (float)vertexes[i].Y, (float)vertexes[i].Z);
                _vertexArray[i].Color = color;
            }

            _vertexBuffer.SetData<VertexPositionColor>(_vertexArray);

            short[] vertexIndices = new short[2 * 3 * 2];

            vertexIndices[0] = (short)0;
            vertexIndices[1] = (short)1;
            vertexIndices[2] = (short)2;
            vertexIndices[3] = (short)0;
            vertexIndices[4] = (short)2;
            vertexIndices[5] = (short)3;

            vertexIndices[6] = (short)2;
            vertexIndices[7] = (short)1;
            vertexIndices[8] = (short)0;
            vertexIndices[9] = (short)3;
            vertexIndices[10] = (short)2;
            vertexIndices[11] = (short)0;

            _indexBuffer = new IndexBuffer(gd, sizeof(short) * vertexIndices.Length,
                BufferUsage.None, IndexElementSize.SixteenBits);

            _indexBuffer.SetData<short>(vertexIndices);

        }


        public void Draw(CMatrix world, CMatrix view, CMatrix projection)
        {

            Matrix w = MathConverter.ToXNAMatrix(world);
            Matrix v = MathConverter.ToXNAMatrix(view);
            Matrix p = MathConverter.ToXNAMatrix(projection);
            GraphicsDevice device = _gd;
            device.VertexDeclaration = _vertexDeclaration;
            _basicEffect.Begin(SaveStateMode.SaveState);
            _basicEffect.World = _transform * w;// sceneWorldTransformation;
            _basicEffect.View = v;// viewMatrix;
            _basicEffect.Projection = p;
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                device.Indices = _indexBuffer;
                device.Vertices[0].SetSource(_vertexBuffer, 0, VertexPositionColor.SizeInBytes);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24 / 6 * 2, 0, 12 / 6 * 2);

                pass.End();
            }
            _basicEffect.End();

        }

        private Color MapToColor(MeshColor meshColor)
        {
            Color color = new Color();
            color.A = meshColor.A;
            color.B = meshColor.B;
            color.G = meshColor.G;
            color.R = meshColor.R;

            return color;
        }

        #region IMesh Members
        public void  Reset()
        {
            _transform = Matrix.Identity;
        }

        public object  MeshObject
        {
	        get { throw new NotImplementedException(); }
        }

        public void  Transform(CMatrix matrix)
        {
            Matrix m = MathConverter.ToXNAMatrix(matrix);
            _transform = m * _transform;
        }

        #endregion
}

}
