using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SkiaSharp;

namespace shapes
{
    internal class Camera
    {
        public Vector3 CameraVector;
        public Vector3 CameraOrigin;
        public Vector3 Axis1, Axis2;
        public double Elevation;

        public Camera()
        {
            CameraVector = Vector3.Zero;
            CameraOrigin = Vector3.Zero;
            Azimuth = 0;
            Elevation = 90;

            ModifyOrigin(new(0, 0, 0));
        }

        public double Azimuth
        {
            get;
            private set;
        }

        public void SetAzimuth(double _newAngle)
        {
            Azimuth = _newAngle % 360;
            CalculateCameraVector();
        }

        public void AddToAzimuth(double _increment)
        {
            Azimuth = (Azimuth + _increment) % 360;
            CalculateCameraVector();
        }

        private double AzimuthRadian
        {
            get { return Azimuth / 180d * Math.PI; }
        }

        private double ElevationRadian
        {
            get { return Elevation / 180d * Math.PI; }
        }

        /// <summary>
        /// Calculate three vectors: camera and two perpendicular, later used when casting 3D space to 2D plane.
        /// </summary>
        private void CalculateCameraVector()
        {
            CameraVector = new Vector3(
                (float)Math.Round(Math.Cos(AzimuthRadian) * Math.Sin(ElevationRadian), 15), 
                (float)Math.Round(Math.Sin(AzimuthRadian) * Math.Sin(ElevationRadian), 15),
                (float)Math.Round(Math.Cos(ElevationRadian), 15)
            ); CameraVector = Vector3.Normalize(CameraVector);

            Axis1 = new Vector3(
                (float)Math.Round(-Math.Sin(AzimuthRadian), 15), 
                (float)Math.Round(Math.Cos(AzimuthRadian), 15),
                0
            ); Axis1 = Vector3.Normalize(Axis1);

            Axis2 = new Vector3(
                (float)Math.Round(Math.Cos(ElevationRadian) * Math.Cos(AzimuthRadian), 5),
                (float)Math.Round(Math.Cos(ElevationRadian) * Math.Sin(AzimuthRadian), 5),
                (float)Math.Round(-Math.Sin(ElevationRadian), 5)
            ); Axis2 = Vector3.Normalize(Axis2);
        }

        public Vector3 GetCameraVector()
        {
            CalculateCameraVector();

            return CameraVector;
        }

        /// <summary>
        /// Cast 3D point to local 2D plane representing the screen.
        /// </summary>
        /// <param name="pos">Position in 3D space</param>
        /// <returns>Position in 2D space</returns>
        public Vector2 CastToLocal(Vector3 pos)
        {
            var s = Vector3.Dot(CameraVector, pos - CameraOrigin);
            var t_1 = Vector3.Dot(Axis1, pos - CameraOrigin);
            var t_2 = Vector3.Dot(Axis2, pos - CameraOrigin);

            return new(t_1, t_2);
        }

        /// <summary>
        /// Cast an array of 3D points using <c>CastToLocal</c>
        /// </summary>
        /// <param name="toCast">Array of points in 3D space</param>
        /// <param name="offset">A 2D offset applied to cast points</param>
        /// <returns>Array of points in 2D space</returns>
        public SKPoint[] CastArrayToLocal(Vector3[] toCast, Vector2 offset)
        {
            var res = new SKPoint[toCast.Length];

            for (int i=0; i<toCast.Length; i++)
            {
                var local = CastToLocal(toCast[i]) - offset;

                res[i] = new SKPoint(local.X, local.Y);
            }

            return res;
        }

        public SKPoint[] CastArrayToLocal(Vector3[] toCast)
        {
            return CastArrayToLocal(toCast, Vector2.Zero);
        }

        /// <summary>
        /// Modify the camera origin by <c>change</c>
        /// </summary>
        /// <param name="change">Offset to be added to <c>Origin</c></param>
        public void ModifyOrigin(Vector3 change)
        {
            CameraOrigin += change;
        }

        /// <summary>
        /// Modify the elevation by <c>change</c>
        /// </summary>
        /// <param name="change">Offset to be added to <c>Elevation</c></param>
        public void ModifyElevation(double change)
        {
            if (Elevation + change >= 0 && Elevation + change <= 180)
            {
                Elevation += change;
            }
        }

        /// <summary>
        /// Set <c>Elevation</c> to <c>newElevation</c>
        /// </summary>
        public void SetElevation(int newElevation)
        {
            if (newElevation >= 0 && newElevation <= 180)
            {
                Elevation = newElevation;
                CalculateCameraVector();
            }
        }
    }
}
