#region Description
//-----------------------------------------------------------------------------
// File:        RubiksCube.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Knoics.Math;
//using Knoics.Interactive;
using System.Diagnostics;
using System.Windows;
using Kit3D.Windows.Media.Media3D;



namespace Knoics.RubiksCube
{

    public enum SelectMode
    {
        Cubies,
        Cube
    }

    public class CubeSelection
    {
        public IEnumerable<Cubie> SelectedCubies { get; set; }
        public SelectMode SelectMode { get; set; }
    }

    public delegate void OneOpDone(string op, string seq, int steps, bool isSolved);
    public class HitResult
    {
        public Point3D HitPoint { get; set; }
        public Cubicle HitCubicle { get; set; }
        public double Distance { get; set; }
    }

    public class RubiksCube : RotatableObject
    {

        public BoundingBox3D BoundingBox { get; private set; }
        public double CubieSize { get; set; }
        public CubeSize CubeSize { get; set; }
        private Dictionary<string, Cubicle> _cubicles;
        public Dictionary<string, Cubicle> Cubicles { get { return _cubicles; } }
        
        private void AfterOp(string op)
        {
            bool isSolved = IsSolved();

            string text = _sequence;// tbCubeOps.Text;
            string action = CubeOperation.GetOp(ref text, true, false);
            if (CubeOperation.IsReverse(action, op))
            {
                //cancel
                _steps--;
                _sequence = text;
            }
            else
            {
                _steps++;
                _sequence = _sequence + op;
            }
            if (OneOpDone != null)
            {
                OneOpDone(op, _sequence, _steps, isSolved);
            }
        }

        private IEnumerable<CubicleFace> GetFaces(string faceName)
        {
            foreach (Cubicle cubicle in Cubicles.Values)
            {
                if (cubicle.Name.IndexOf(faceName) >= 0)
                {
                    yield return cubicle.Faces[faceName];
                }
            }
        }

        private bool BeforeOp(AnimContext animContext)
        {
            string op = animContext.Op;
            if (op == CubeOperation.UndoOp)
            { //undo
                if (string.IsNullOrEmpty(_sequence)) return  false;

                op = CubeOperation.GetOp(ref _sequence, true, true); 
                op = CubeOperation.GetReverseOp(op);
                if (!string.IsNullOrEmpty(op))
                    animContext.Op = op;
                
            }

            CubeSize size = CubeSize;
            double edgeX = CubieSize * size.Width / 2;
            double edgeY = CubieSize * size.Height / 2;
            double edgeZ = CubieSize * size.Depth / 2;

            if (op == CubeOperation.FoldOp || op == CubeOperation.UnFoldOp)
            {
                bool unfold = (op == CubeOperation.UnFoldOp);
                FaceTransform f = new FaceTransform("F", Axis.X, false, new Vector3D(0, edgeY, edgeZ), new Vector3D(), face => GetFaces(face))
                {
                    Silent = true,
                    ChangeAngle = unfold ? -CubeOperation.PiOver2 : CubeOperation.PiOver2,
                    Begin = 0,
                    /*
                    Face = "F",
                    AxisTranslationFromOrigin = new Vector3D(0, edgeY, edgeZ),
                    Axis = Axis.X,
                    IsAxisMoving = false
                    */
                };
                FaceTransform b = new FaceTransform("B", Axis.X, false, new Vector3D(0, edgeY, -edgeZ), new Vector3D(), face => GetFaces(face))
                {
                    Silent = true,
                    ChangeAngle = unfold ? CubeOperation.PiOver2 : -CubeOperation.PiOver2,
                    Begin = 0
                    /*
                    Face = "B",
                    AxisTranslationFromOrigin = new Vector3D(0, edgeY, -edgeZ),
                    Axis = Axis.X,
                    IsAxisMoving = false
                     */
                };
                FaceTransform l = new FaceTransform("L", Axis.Z, false,new Vector3D(-edgeX, edgeY, 0),new Vector3D(), face => GetFaces(face))
                {
                    Silent = true,
                    ChangeAngle = unfold ? -CubeOperation.PiOver2 : CubeOperation.PiOver2,
                    Begin = 0,
                    /*
                    Face = "L",
                    AxisTranslationFromOrigin = new Vector3D(-edgeX, edgeY, 0),
                    Axis = Axis.Z,
                    IsAxisMoving = false
                     */
                };
                FaceTransform r = new FaceTransform("R", Axis.Z, false, new Vector3D(edgeX, edgeY, 0), new Vector3D(), face => GetFaces(face))
                {
                    Silent = true,
                    ChangeAngle = unfold ? CubeOperation.PiOver2 : -CubeOperation.PiOver2,
                    Begin = 0
                    /*
                    Face = "R",
                    AxisTranslationFromOrigin = new Vector3D(edgeX, edgeY, 0),
                    Axis = Axis.Z,
                    IsAxisMoving = false
                    */
                };


                FaceTransform d;
                //if (unfold)
                {
                    d = new FaceTransform("D", Axis.Z, true,new Vector3D(edgeX, -edgeY, 0),new Vector3D(edgeX, edgeY, 0), face => GetFaces(face))
                    {
                        Silent = true,
                        ChangeAngle = unfold ? CubeOperation.PiOver2 : -CubeOperation.PiOver2,
                        Begin = 0
                        /*
                        Face = "D",
                        AxisTranslationFromOrigin = new Vector3D(edgeX, -edgeY, 0),
                        Axis2TranslationFromOrigin = new Vector3D(edgeX, edgeY, 0),
                        Axis = Axis.Z,
                        IsAxisMoving = true
                        */
                    };
                }
                animContext.TransformParams.Add(f);
                animContext.TransformParams.Add(b);
                animContext.TransformParams.Add(l);
                animContext.TransformParams.Add(r);
                animContext.TransformParams.Add(d);
            }
            else
            {
                if (!CubeOperation.IsValidOp(op))
                    return false;

                BasicOp basicOp;
                bool isReverse;
                CubeOperation.GetBasicOp(op, out basicOp, out isReverse);
                CubieTransform transform = new CubieTransform(op, isReverse, basicOp, cn=>Cubicles[cn])
                {
                    Silent = animContext.Silent
                };
                transform.ChangeAngle = transform.RotateAngle - animContext.RotatedAngle;
                animContext.TransformParams.Add(transform);
            }


            return true;
        }

        


        private RubiksCube()
        {
            _cubicles = new Dictionary<string, Cubicle>();
            #region Animation Part
            {
                _animator = new Animator(BeforeOp, AfterOp );
                /*
                _animator.EndUpdator = (obj, op) =>
                {
                    bool isSolved = IsSolved();
                    if (OneOpDone != null)
                        OneOpDone(op, _sequence, _steps, isSolved);
                    return true;
                };
                 */
            }
            #endregion
        }

        public static RubiksCube CreateRubiksCube(Vector3D origin, int cubieNum, double cubieSize)
        {
            //if(cubieNum!=3) throw new ArgumentException("Invalid CubieNum");
            RubiksCube rubiksCube = new RubiksCube();
            rubiksCube.CubieSize = cubieSize;
            rubiksCube._model = CubeConfiguration.Factory.CreateModel();
            
            CubeSize size = new CubeSize(cubieNum, cubieNum, cubieNum );
            rubiksCube.CubeSize = size;
            Vector3D start = origin;
            start.X -= (size.Width - 1) * cubieSize / 2;
            start.Y -= (size.Height - 1) * cubieSize / 2;
            start.Z -= (size.Depth - 1) * cubieSize / 2;
            Vector3D min = origin; min.X -= size.Width * cubieSize / 2; min.Y -= size.Height * cubieSize / 2; min.Z -= size.Depth * cubieSize / 2;
            Vector3D max = origin; max.X += size.Width * cubieSize / 2; max.Y += size.Height * cubieSize / 2; max.Z += size.Depth * cubieSize / 2;
            rubiksCube.BoundingBox = new BoundingBox3D(min, max);

            for (int i = 0; i < size.Width; i++)
            {
                for (int j = 0; j < size.Height; j++)
                {
                    for (int k = 0; k < size.Depth; k++)
                    {
                        //if (k == 0 || j == 0) //for debug
                        {
                            Vector3D cubieOri = start;
                            cubieOri.X += i * cubieSize; cubieOri.Y += j * cubieSize; cubieOri.Z += k * cubieSize;
                            string cubicleName = CubeConfiguration.GetCubicleName(size, i, j, k);
                            //Debug.WriteLine(string.Format("({0},{1},{2}): {3}", i,j,k, cubicleName));
                            if (!string.IsNullOrEmpty(cubicleName))
                            {
                                string cubieName = cubicleName; //solved configuration
                                Cubicle cubicle = Cubicle.CreateCubicle(cubicleName, cubieName, cubieOri, cubieSize);
                                rubiksCube._cubicles.Add(cubicleName, cubicle);
                            }
                        }
                    }
                }
            }
            return rubiksCube;
        }

        public Cubicle this[string key]
        {
            get
            {
                return _cubicles[key];
            }
        }

        public void Undo()
        {
            if(_sequence.Length>0)
                Op(CubeOperation.UndoOp, 30);
        }

        public void Op(string op, int frames)
        {
            if (!string.IsNullOrEmpty(op))
            {
                bool unfolded = Unfolded;
                int unfoldFrames = frames <= 3 ? 1 : frames / 3;
                if (unfolded)
                {
                    Unfold(unfoldFrames);
                }
                Rotate(op, unfolded ? unfoldFrames : frames, false, 0, false);
                if (unfolded)
                {
                    Unfold(unfoldFrames);
                }

            }
        }

        public string DoCommand(string commandString)
        {
            string text = commandString.Trim().ToUpper();
            if (string.IsNullOrEmpty(text)) return text;
            string action = CubeOperation.GetOp(ref text, false, false);
            Op(action, 30);
            return text;
        }

        private IModel _model;
        public IModel Model { get { return _model; } }

        public IEnumerable<IMesh> Meshes
        {
            get
            {
                foreach (Cubicle cubicle in _cubicles.Values)
                {
                    foreach (CubieFace face in cubicle.Cubie.Faces.Values)
                    {
                        foreach (IMesh mesh in face.Meshes)
                            yield return mesh;
                    }
                }
            }
        }

        

        public override void Save()
        {
            base.Save();
            _model.Save();
        }

        public override void Restore()
        {
            base.Restore();
            _model.Restore();
        }

        public override void Reset()
        {
            base.Reset();
            _steps = 0;
            _sequence = string.Empty;
            SelectedCubies = null;
            _unfolded = false;
            Cubie[] cubies = _cubicles.Values.Select(c => c.Cubie).ToArray();
            foreach (Cubie cubie in cubies)
            {
                cubie.Reset();
                _cubicles[cubie.Name].SetCubie(cubie);
                string seq = string.Concat(cubie.Faces.Keys.ToArray());
                _cubicles[cubie.Name].SetCubieFaces(cubie.Faces, seq, seq);
            }
        }

        public override Matrix3D Rotate(Axis axis, double deltaAngle, bool isFromSaved)
        {
            Matrix3D matrix = base.Rotate(axis, deltaAngle, isFromSaved);
            _model.DoTransform(matrix, isFromSaved);
            return matrix;
        }

        public bool IsFaceSoved(string face)
        {
            IEnumerable<Cubicle> cubicles = _cubicles.Where(p => p.Key.IndexOf(face) >= 0).Select(kv => kv.Value);
            string faceToMatch = _cubicles[face].Cubie.Name;
            return (cubicles.FirstOrDefault(c => c.Cubie.Name.IndexOf(faceToMatch) < 0) == null); //no matched
        }

        public bool IsSolved()
        {
            foreach (string face in CubeConfiguration.Faces.Keys)
            {
                if (!IsFaceSoved(face)) return false;
            }
            return true;
        }


        private Matrix3D _viewMatrix = Matrix3D.Identity;
        private Matrix3D _inverseViewMatrix = Matrix3D.Identity;
        public Matrix3D ViewMatrix
        {
            get { return _viewMatrix; }
            set { 
                _viewMatrix = value;
                _inverseViewMatrix = _viewMatrix;// Matrix.Invert(_viewMatrix); 
                _inverseViewMatrix.Invert();
            }
        }

        private Matrix3D _projectionMatrix = Matrix3D.Identity;
        private Matrix3D _inverseProjectionMatrix = Matrix3D.Identity;
        public Matrix3D ProjectionMatrix
        {
            get { return _projectionMatrix; }
            set { 
                _projectionMatrix = value;
                _inverseProjectionMatrix = _projectionMatrix;// Matrix.Invert(_projectionMatrix);
                _inverseProjectionMatrix.Invert();
            }
        }

        public Matrix3D InverseProjectionMatrix
        {
            set
            {
                _inverseProjectionMatrix = value;
                _projectionMatrix = _inverseProjectionMatrix;// Matrix.Invert(_inverseProjectionMatrix);
                _projectionMatrix.Invert();
            }
        }

        private Point3D _viewpoint = new Point3D();
        public Point3D ViewPoint { get { return _viewpoint; } set { _viewpoint = value; } }

        /// <summary>
        /// Only if deep is true, then HitCubieResult could have some value
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="deep"></param>
        /// <param name="hitResult"></param>
        /// <returns></returns>
        public bool HitTest(Point pt, bool deep, out HitResult hitResult)
        {
            hitResult = null;
            Ray3D ray = Ext3D.Unproject(pt, _viewpoint, _model.Transform, _inverseViewMatrix, _inverseProjectionMatrix);
            //Debug.WriteLine(string.Format("ray: {0}", ray));
            double? d = this.BoundingBox.Intersects(ray);
            if (!deep)
            {
                if (d!=null)
                {
                    //Debug.WriteLine(string.Format("first ray: {0}, distance:{1}", ray, (double)d));
                    hitResult = new HitResult() { Distance = d.Value, HitCubicle = null, HitPoint = ray.Origin + d.Value * ray.Direction };
                }
            }
            else
            {
                List<HitResult> results = new List<HitResult>();
                if (d!=null)
                {
                    //double? d1;
                    foreach (Cubicle cubicle in _cubicles.Values)
                    {
                        Matrix3D localToWorld = _model.Transform; localToWorld.Invert();// *cubicle.Cubie.Transform;
                        ray = Ext3D.Unproject(pt, _viewpoint, localToWorld, _inverseViewMatrix, _inverseProjectionMatrix);
                        //d1 = cubicle.Cubie.BoundingBox.Intersects(ray);
                        double? d1 = cubicle.BoundingBox.Intersects(ray);
                        if (d1!=null)
                        {
                            HitResult result = new HitResult() { Distance = d1.Value, HitCubicle = cubicle, HitPoint = ray.Origin + d1.Value * ray.Direction };
                            results.Add(result);
                        }
                    }
                    results.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                    if (results.Count > 0)
                        hitResult = results[0];
                }
            }
            return d!=null;
        }

        public double? TestAngle(Point pt, HitResult prevHit, out Axis axis)
        {
            HitResult hitResult;
            bool hit = HitTest(pt, true, out hitResult);
            double? angle = null;
            axis = Axis.X;
            if (hit)
            {
                Vector3D from, to;
                angle = 0;
                
                axis = Axis.Z;
                Point3D oxy = new Point3D(0, 0, prevHit.HitPoint.Z); from = prevHit.HitPoint - oxy; to = new Point3D(hitResult.HitPoint.X, hitResult.HitPoint.Y, prevHit.HitPoint.Z) - oxy;
                double xyAngle = Ext3D.AngleBetween(from, to);
                 
                angle = xyAngle;
                if (Vector3D.DotProduct(Ext3D.UnitZ, Vector3D.CrossProduct(from, to)) < 0)
                    angle = -angle;
                

                //plane yz, where x = prevHit.X, Axis.X
                Point3D oyz = new Point3D(prevHit.HitPoint.X, 0, 0); from = prevHit.HitPoint - oyz; to = new Point3D(prevHit.HitPoint.X, hitResult.HitPoint.Y, hitResult.HitPoint.Z) - oyz;
                double yzAngle = Ext3D.AngleBetween(from, to);
                if (System.Math.Abs(yzAngle) > System.Math.Abs((double)angle))
                {
                    angle = yzAngle;
                    axis = Axis.X;
                    if (Vector3D.DotProduct(Ext3D.UnitX, Vector3D.CrossProduct(from, to)) < 0)
                        angle = -angle;
                }

                //plane zx, where y = prevHit.Y, Axis.Y
                Point3D ozx = new Point3D(0, prevHit.HitPoint.Y, 0); from = prevHit.HitPoint - ozx; to = new Point3D(hitResult.HitPoint.X, prevHit.HitPoint.Y, hitResult.HitPoint.Z) - ozx;
                double zxAngle = Ext3D.AngleBetween(from, to);
                if (System.Math.Abs(zxAngle) > System.Math.Abs((double)angle))
                {
                    angle = zxAngle;
                    axis = Axis.Y;
                    if (Vector3D.DotProduct(Ext3D.UnitY, Vector3D.CrossProduct(from, to)) < 0)
                        angle = -angle;
                }

                double threshold = Angle.DegreesToRadians(1.0);

                if (angle != null) 
                {
                    //Debug.WriteLine(string.Format("angle: {0}, threshold:{1}", angle, threshold));
                    if (System.Math.Abs((double)angle) < threshold)
                    {
                        //Debug.WriteLine("non null to null");
                        angle = null;
                    }

                }
                //else
                    //Debug.WriteLine("null angle");

            }
            return angle;
        }
        public bool NoOp
        {
            get {
                return _animator.NoOp && !_unfolded;
            }
        }
        public double TestAngle(Point pt, HitResult prevHit, Axis axis)
        {
            Ray3D ray = Ext3D.Unproject(pt, _viewpoint, _model.Transform, _inverseViewMatrix, _inverseProjectionMatrix);
            //Plane yz = new Plane(-1, 0, 0, prevHit.HitPoint.X);
            //Point3 pyz; double? d = yz.Intersects(ray); Debug.WriteLine(string.Format("second ray: {0}: distance: {1}", ray, (double)d));
            
            //Debug.WriteLine(string.Format("second ray: {0}", ray));
            double angle = 0;
            switch (axis)
            {
                case Axis.X:
                    Vector3D v = new Vector3D(-1, 0, 0);
                    Plane3D yz = new Plane3D(v, prevHit.HitPoint.X); Point3D oyz = new Point3D(prevHit.HitPoint.X, 0, 0);
                    Point3D pyz;
                    yz.Intersect(ray, out pyz); Vector3D from = prevHit.HitPoint - oyz; Vector3D to = pyz - oyz;
                    angle =  Ext3D.AngleBetween(from, to);
                    if (Vector3D.DotProduct(Ext3D.UnitX, Vector3D.CrossProduct(from, to)) < 0)
                        angle = -angle;

                    break;
                case Axis.Z:
                    v = new Vector3D(0, 0, -1);
                    Plane3D xy = new Plane3D(v, prevHit.HitPoint.Z); Point3D oxy = new Point3D(0, 0, prevHit.HitPoint.Z);
                    Point3D pxy;
                    xy.Intersect(ray, out pxy); from = prevHit.HitPoint - oxy; to = pxy - oxy;
                    angle = Ext3D.AngleBetween(from, to);
                    if (Vector3D.DotProduct(Ext3D.UnitZ, Vector3D.CrossProduct(from, to)) < 0)
                        angle = -angle;

                    break;
                case Axis.Y:
                    v = new Vector3D(0, -1, 0);
                    Plane3D zx = new Plane3D(v, prevHit.HitPoint.Y); Point3D ozx = new Point3D(0, prevHit.HitPoint.Y, 0);
                    Point3D pzx;
                    zx.Intersect(ray, out pzx); from = prevHit.HitPoint - ozx; to = pzx - ozx;
                    angle = Ext3D.AngleBetween(from, to);
                    if (Vector3D.DotProduct(Ext3D.UnitY, Vector3D.CrossProduct(from, to)) < 0)
                        angle = -angle;

                    break;
            }
            /*
            if (angle < 2)
                angle = null;
            else
            {
                Debug.WriteLine(string.Format("axis: {0}, angle:{1}", axis, angle));
            }
            */
             return angle;
             
        }

        public CubeSelection Select(Cubicle cubicle, Axis axis, out string basicOp)
        {
            CubeSelection cs = new CubeSelection();
            basicOp = string.Empty;
            //if (cubicle.Name.Length > 1)
            {//not center piece, select cubies
                cs.SelectMode = SelectMode.Cubies;
                basicOp = CubeOperation.GetBasicOp(cubicle.Name, axis).Op;
                cs.SelectedCubies = CubeOperation.BasicOps[basicOp].CubicleGroup.Select(c => _cubicles[c].Cubie);
            }
            /*
            else //center piece
            {
                cs.SelectMode = SelectMode.Cube;
            }
             */
            return cs;
        }



        #region CubeAnimation
        private Animator _animator;
        public IEnumerable<Cubie> SelectedCubies { get; set; }
        public string SelectedBasicOp { get; set; }
        public OneOpDone OneOpDone;

        public Animator Animator { get { return _animator; } }
        private int _steps;
        private string _sequence = string.Empty;
        public void Random()
        {
            if (!Unfolded)
            {
                _steps = 0;
                _sequence = string.Empty;
                string[] ops = CubeOperation.GetRandomOps(10);
                foreach (string op in ops)
                {
                    Rotate(op);
                }
            }
        }


        public void Rotate(string op)
        {
            Rotate(op, 0f, true, 0, false);
        }


        public bool Unfolded { get { return _unfolded; } }
        private bool _unfolded = false;

        public void Unfold(double frames)
        {
            bool unfold = !_unfolded;
            _animator.Start(new AnimContext(0.0, frames, unfold ? CubeOperation.UnFoldOp : CubeOperation.FoldOp, true, new List<Transform>()));
            //_animator.Start(new AnimContext(0.0, frames, unfold ? CubeOperation.UnFoldOp : CubeOperation.FoldOp, true, new List<Transform>()));

            _unfolded = unfold;
        }

        /// <summary>
        /// startPos: 
        /// </summary>
        /// <param name="op"></param>
        /// <param name="duration"></param>
        /// <param name="silent"></param>
        /// <param name="startPos"></param>
        internal void Rotate(string op, double frames, bool silent, double rotatedAngle, bool fromInteraction) 
        {
            if (string.IsNullOrEmpty(op)) return;
            if (!fromInteraction && InTrack) return;
            _animator.Start(new AnimContext(rotatedAngle, frames, op.ToUpper(), silent, new List<Transform>()));
            //_animator.Start(new AnimContext(rotatedAngle, frames, op.ToUpper(), silent, new List<Transform>()));
        }
        #endregion


        #region Interaction
        private HitResult _prevHit;
        private double? _angle = 0;
        private string _basicOp;
        private Axis _axis = Axis.X;
        private CubeSelection _selected;

        public bool InTrack { get { return _prevHit != null; } }
        public void StartTrack(Point pt)
        {
            try
            {
                if (_prevHit != null)
                {
                    EndTrack();
                }

                if (NoOp && _prevHit == null)
                {
                    HitResult result;
                    bool hit = HitTest(pt, true, out result);
                    if (hit && result != null)
                    {
                        _prevHit = result;
                        //Debug.WriteLine(string.Format("start:{0},pt:{1}", result.HitCubicle, result.HitPoint));
                        _angle = null;
                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("StartTrack Error: " + e.ToString());
            }

        }

        public void Track(Point p)
        {
            try
            {
                if (_prevHit != null && NoOp)
                {
                    if (_angle == null)
                    {

                        _angle = TestAngle(p, _prevHit, out _axis);
                        _selected = Select(_prevHit.HitCubicle, _axis, out _basicOp);
                        if (_selected.SelectMode == SelectMode.Cubies)
                            foreach (Cubie cubie in _selected.SelectedCubies) cubie.Save();
                        else
                            Save();
                        //Debug.WriteLine(string.Format("selected angle: {0}, axis:{1}", _angle, _axis));
                    }

                    if (_angle != null && (_selected.SelectedCubies != null || _selected.SelectMode == SelectMode.Cube))
                    {
                        double angle = TestAngle(p, _prevHit, _axis);
                        if (System.Math.Abs(angle) < Angle.DegreesToRadians(90))
                        {
                            if (_selected.SelectMode == SelectMode.Cubies)
                                foreach (Cubie cubie in _selected.SelectedCubies)
                                    cubie.Rotate(_axis, angle, true);
                            else
                            {
                                Rotate(_axis, angle, true);
                                //Debug.WriteLine(string.Format("move angle: {0}", _angle));
                            }
                            _angle = angle;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Track Error: " + e.ToString());
            }
        }

        public void EndTrack()
        {
            try
            {
                if (NoOp && _prevHit != null)
                {
                    if (_angle != null)
                    {
                        double angle = (double)_angle;
                        if (System.Math.Abs(angle) > Angle.DegreesToRadians(10) && System.Math.Abs(angle) < Angle.DegreesToRadians(90))
                        {
                            if (_selected.SelectMode == SelectMode.Cubies)
                            {
                                //finish OP
                                RotationDirection dir = (angle > 0) ? RotationDirection.CounterClockWise : RotationDirection.Clockwise;
                                string op = CubeOperation.GetOp(_basicOp, dir);
                                if (!string.IsNullOrEmpty(op))
                                {
                                    Rotate(op, 20, false, angle, true);
                                    //Debug.WriteLine(string.Format("op:{0}, angle:{1}", op, angle));
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            //Debug.WriteLine(string.Format("endtrack: {0}", angle));
                            //cancel OP
                            if (_selected.SelectMode == SelectMode.Cubies)
                                foreach (Cubie cubie in _selected.SelectedCubies) cubie.Restore();
                            else
                                Restore();
                        }
                    }
                    _prevHit = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("EndTrack Error: " + e.ToString());
            }
        }
        #endregion
    }

}
