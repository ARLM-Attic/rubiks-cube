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
using System.IO;

namespace XNALib
{
    public class ModelStructureExport
    {
        public static void WriteModelStructure(Model model)
        {
            StreamWriter writer = new StreamWriter("modelStructure.txt");
            writer.WriteLine("Model Bone Information");
            writer.WriteLine("----------------------");
            ModelBone root = model.Root;
            WriteBone(root, 0, writer);
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("Model Mesh Information");
            writer.WriteLine("----------------------");
            foreach (ModelMesh mesh in model.Meshes)
                WriteModelMesh(model.Meshes.IndexOf(mesh), mesh, writer);
            writer.Close();
        }

        private static void WriteBone(ModelBone bone, int level, StreamWriter writer)
        {
            for (int l = 0; l < level; l++)
                writer.Write("\t");
            writer.Write("- Name : ");
            if ((bone.Name == "") || (bone.Name == "null"))
                writer.WriteLine("null");
            else
                writer.WriteLine(bone.Name);
            for (int l = 0; l < level; l++)
                writer.Write("\t");
            writer.WriteLine(" Index: " + bone.Index);
            foreach (ModelBone childBone in bone.Children)
                WriteBone(childBone, level + 1, writer);
        }

        private static void WriteModelMesh(int ID, ModelMesh mesh, StreamWriter writer)
        {
            writer.WriteLine("- ID : " + ID);
            writer.WriteLine(" Name: " + mesh.Name);
            writer.Write(" Bone: " + mesh.ParentBone.Name);
            writer.WriteLine(" (" + mesh.ParentBone.Index + ")");
        }
    }
}
