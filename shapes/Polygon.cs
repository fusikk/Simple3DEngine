using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace shapes
{
    public class Polygon
    {
        public List<Vector3> Vertices;
        public Vector3 SurfaceVector;

        /// <param name="group">Array of vertices that define the polygon</param>
        /// <param name="origin">Origin of the cuboid. Used to calculate the surface vector of the polygon.</param>
        /// <exception cref="ArgumentException">Thrown when <c>group</c> has an incorrect amount of vertices.</exception>
        public Polygon(Vector3[] group, Vector3 origin)
        {
            if (group.Length != 3)
            {
                throw new ArgumentException("Invalid number of vertices (must be equal to 3)");
            }

            Vertices = new(group);
            CalculateSurfaceVector(origin);
        }

        /// <summary>
        /// Calculate the surface vector - perpendicular to plane of the polygon.
        /// </summary>
        public void CalculateSurfaceVector(Vector3 origin)
        {
            Vector3 directional = Vector3.Cross(Vertices[1] - Vertices[0], Vertices[2] - Vertices[0]);
            directional = Vector3.Normalize(directional);

            var fromOrigin = Vertices[0] - origin;
            var dot1 = Vector3.Dot(fromOrigin, directional);

            if (dot1 < 0)
                directional *= -1;

            SurfaceVector = directional;
        }
    }
}
