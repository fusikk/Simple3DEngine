using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SkiaSharp;

namespace shapes
{
    public class Cuboid
    {
        public double Rotation;
        private int Phase;
        private readonly static int MaxPhase = 360;
        public Vector3 Origin;
        private double Size;
        public List<Polygon> Polygons;  

        /// <summary>
        /// Groups of 4 vertices that form a wall of the cuboid
        /// </summary>
        private static int[][] VerticeGroups = 
        {
            new int[] {2, 3, 1, 4 },
            new int[] {2, 3, 6, 7 },
            new int[] {2, 1, 6, 5 },
            new int[] {3, 4, 7, 8 },
            new int[] {1, 4, 5, 8 },
            new int[] {6, 7, 5, 8 }
        };

        public Dictionary<int, Vector3> Vertices
        {
            get;
            private set;
        }

        /// <summary>
        /// Figure in 3D space
        /// </summary>
        /// <param name="_origin">Point in 3D space</param>
        /// <param name="_size">Radius of sphere into which we inscribe the cube</param>
        public Cuboid(Vector3 _origin, int _size)
        {
            Vertices = new();
            Rotation = 0;
            Phase = 0;
            Origin = _origin;
            Size = _size;

            RecreateVertices(0);
            InitializePolygons();
        }

        /// <summary>
        /// Recalculate the vertices whilst increment the phase of vertex placement
        /// </summary>
        private void RecreateVertices(int phaseIncrement)
        {
            Phase = (Phase + phaseIncrement) % MaxPhase;

            double phaseShift = Phase / (double)MaxPhase * 2 * Math.PI;
            Vertices.Clear();
            int key = 1;
            for (double i = Math.PI / 4; i < 2 * Math.PI; i += Math.PI / 2)
            {

                Vertices.Add(key, new((float)(
                    Origin.X + Size * Math.Sin(i + phaseShift)), 
                    (float)(Origin.Y + Size * Math.Cos(i + phaseShift)),
                    (float)(Origin.Z + Size * Math.Cos(Math.PI/4))
                ));

                Vertices.Add(key + 4, new((float)(
                    Origin.X + Size * Math.Sin(i + phaseShift)),
                    (float)(Origin.Y + Size * Math.Cos(i + phaseShift)),
                    (float)(Origin.Z + Size * Math.Cos(3*Math.PI / 4))
                ));
                key++;
            }
        }

        public void RecreateVertices()
        {
            RecreateVertices(0);
        }

        /// <summary>
        /// Create from groups of vertices
        /// </summary>
        private void InitializePolygons()
        {
            Polygons = new();

            foreach (var group in VerticeGroups)
            {
                for (int i=0; i<2; i++)
                {
                    Polygons.Add(
                        new(new[] { Vertices[group[0 + i]], Vertices[group[1 + i]], Vertices[group[2 + i]] }, Origin)    
                    );
                }
            }
        }
    }
}
