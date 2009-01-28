#region Description
//-----------------------------------------------------------------------------
// File:        CubeModel.cs
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
using WindowSystem;
using XNALib;


namespace RubiksCube
{

    enum Axis
    {
        X,
        Y,
        Z
    }
    

    delegate void OneOpDone (string op);

    class CubeModel
    {
        Model _model;
        Matrix[] _cubeTransforms;
        Matrix[] _cubeOriginalTransforms;
        Matrix _worldMatrix = Matrix.Identity;// Matrix.CreateScale(0.01f);// Matrix.Identity;
        Animator _animator;
        private CubeConfiguration _cubeConfiguration;
        public IEnumerable<Cubie> SelectedCubies { get; set; }
        public string SelectedBasicOp { get; set; }
        public OneOpDone OneOpDone;
        public bool IsSolved { get; set; }
        public CubeModel(Animator animator)
        {
            _animator = animator;

            _animator.BeginUpdator = (obj) =>
            {
                
                BeforeTransform(obj);

                return true;
            };

            _animator.IntervalUpdator = (obj, change) =>
            {
                Transform transform = (Transform)obj;
                DoTransform(transform, change);
                
                return true;
            };
            
            _animator.EndUpdator = (obj, change) =>
                {
                    Transform transform = (Transform)obj;
                    AfterTransform(transform, change);
                    return true;
                };
            
            
        }

        public Model LoadModel(ContentManager content, string asset)
        {
            _model = XNAUtils.LoadModelWithBoundingSphere(out _cubeTransforms, asset, content);
            _cubeTransforms = XNAUtils.AutoScale(_model, 15.0f);

            Debug.WriteLine(string.Format("start transforms[18]: {0}", _cubeTransforms[18]));

            _cubeOriginalTransforms = new Matrix[_model.Bones.Count];
            _model.CopyBoneTransformsTo(_cubeOriginalTransforms);
            _cubeConfiguration = new CubeConfiguration();
            _cubeConfiguration.Init(_model);
            SelectedCubies = null;
            return _model;
        }

        public bool SelectCubicle(
                                 Ray ray,
                                 out Cubicle selectedCubicle,
                                 out float distance)
        {
            string selectedBasicOp;
            return _cubeConfiguration.SelectCubicle(ray, out selectedCubicle, out selectedBasicOp, out distance);
        }

        public void SelectCubies(Ray ray)
        {
            Cubicle selectedCubicle;
            float distance;
            string basicOp;
            if (_cubeConfiguration.SelectCubicle(ray, out selectedCubicle, out basicOp, out distance))
            {
                if (!string.IsNullOrEmpty(basicOp))
                {
                    SelectedBasicOp = basicOp;
                    SelectedCubies = CubeOperations.BasicOps[SelectedBasicOp].GetCubicles().Select(b => _cubeConfiguration[b].Cubie);
                }
            }
            
        }



        public void Random()
        {
            string[] ops = CubeOperations.GetRandomOps(10);
            foreach (string op in ops)
            {
                Rotate(op);
            }
        }


        public void Rotate(string action)
        {
            Rotate(action, 0f, true);
        }

        public void Rotate(string action, float duration, bool silent)
        {
            if (string.IsNullOrEmpty(action)) return;
            action = action.ToUpper();
            if (!CubeOperations.IsValidOp(action))
                return;

            AnimContext animContext = new AnimContext()
            {
                Op = action,
                Duration = duration,
                Silent = silent
                //Change  = rotation
                //Transform = transform
            };

            //run once
            /*
            Transform transform = BeforeTransform(animContext);
            DoTransform(transform, rotation);
            UpdateCubiePosition(transform);
             */
            //Animation
            _animator.Start(animContext);
        }

        private Transform BeforeTransform(AnimContext animContext)
        {
            BasicOp basicOp;
            bool isReverse;
            CubeOperations.GetBasicOp(animContext.Op, out basicOp, out isReverse);

            IEnumerable<Cubicle> cubicles = basicOp.GetCubicles().Select(c=> _cubeConfiguration[c]);
            Transform transform = new Transform()
            {
                Op = animContext.Op,
                IsReversedBasicOp = isReverse,
                BasicOp = basicOp,
                AffectedCubicles = cubicles,
                Silent = animContext.Silent
            };
            animContext.Change = isReverse? -basicOp.Rotation : basicOp.Rotation;
            animContext.Transform = transform;

            return transform;
        }

        private void DoTransform(Transform transform, float delta)
        {
            //Debug.WriteLine(string.Format("transform bones: {0}", bones.Count()));
            foreach (Cubicle cubicle in transform.AffectedCubicles)
            {
                cubicle.Cubie.Rotate(transform.BasicOp.Axis, delta);
                //Debug.WriteLine(string.Format("{0}", bone.Name));
            }

        }

        private void AfterTransform(Transform  transform, float rotation)
        {
            foreach (Cubicle cubicle in transform.AffectedCubicles)
            {
                Cubie cubie = cubicle.Cubie;
                cubie.RotateUnit(transform.BasicOp.Axis, rotation);
                //Debug.WriteLine(string.Format("{0}", bone.Name));
            }

            foreach (string[] change in transform.BasicOp.CubicleGroupCycles)
            {
                Group.Cycle<string, Cubie>(transform.IsReversedBasicOp ? change.Reverse().ToArray() : change, k => _cubeConfiguration[k].Cubie, (k, v) => { _cubeConfiguration[k].Cubie = v; return true; });
            }

            IsSolved = _cubeConfiguration.IsSolved();
            if (!transform.Silent && OneOpDone != null)
                OneOpDone(transform.Op);
        }

        public void Draw(Matrix view, Matrix projection, GameTime gameTime)
        {
            _cubeConfiguration.DrawModel(_model, SelectedCubies,  _worldMatrix, _cubeTransforms, view, projection);
        }

        public void Reset()
        {
            SelectedCubies = null;
            _cubeConfiguration.Reset();
        }

    }
}
