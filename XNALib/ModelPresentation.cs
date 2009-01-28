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
    public class ModelPresentation
    {
        
        public static void DrawModel(Model model, IEnumerable<ModelMesh> selectedMesh, Matrix worldMatrix, Matrix[] modelTransforms, Matrix viewMatrix, Matrix projectionMatrix)
        {
            int meshIndex = 0;
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                
                Matrix transform = modelTransforms[mesh.ParentBone.Index] * worldMatrix;
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (selectedMesh!=null&&selectedMesh.Contains(mesh))
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
                mesh.Draw();
                meshIndex++;
            }
        }


    }
}
