#region Description
//-----------------------------------------------------------------------------
// File:        CubeInteraction.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        02/21/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Net;
using System.Collections.Generic;
using Knoics.Math;
using System.Diagnostics;
using System.Windows;

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

    public class CubeInteraction
    {
        private RubiksCube _rubikscube;
        public CubeInteraction(RubiksCube cube)
        {
            _rubikscube = cube;
            
        }
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

                if (_rubikscube.NoOp && _prevHit == null)
                {
                    HitResult result;
                    bool hit = _rubikscube.HitTest(pt, true, out result);
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
                if (_prevHit != null && _rubikscube.NoOp )
                {
                    if (_angle == null)
                    {

                        _angle = _rubikscube.TestAngle(p, _prevHit, out _axis);
                        _selected = _rubikscube.Select(_prevHit.HitCubicle, _axis, out _basicOp);
                        if (_selected.SelectMode == SelectMode.Cubies)
                            foreach (Cubie cubie in _selected.SelectedCubies) cubie.Save();
                        else
                            _rubikscube.Save();
                        //Debug.WriteLine(string.Format("selected angle: {0}, axis:{1}", _angle, _axis));
                    }

                    if (_angle != null && (_selected.SelectedCubies!= null ||_selected.SelectMode == SelectMode.Cube))
                    {
                        double angle = _rubikscube.TestAngle(p, _prevHit, _axis);
                        if (System.Math.Abs(angle) < Angle.DegreesToRadians(90))
                        {
                            if (_selected.SelectMode == SelectMode.Cubies)
                                foreach (Cubie cubie in _selected.SelectedCubies) cubie.Rotate(_axis, angle, true);
                            else
                            {
                                _rubikscube.Rotate(_axis, angle, true);
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
                if (_rubikscube.NoOp&&_prevHit != null)
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
                                    _rubikscube.Rotate(op, 20, false, angle, true);
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
                                _rubikscube.Restore();
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

    }
}
