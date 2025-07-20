using OX.Numerics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace OX.Geometry
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class Polygon
    {
        /// <summary>
        /// Gets or sets the vertices of the polygon in local space (relative to position and rotation).
        /// </summary>
        public Vect[] LocalVertices { get; set; }
        
        /// <summary>
        /// Gets the vertices of the polygon transformed to global space using the current position and rotation.
        /// </summary>
        public Vect[] GlobalVertices => Vect.Transform(LocalVertices, Position, Rotation);
        
        /// <summary>
        /// Gets or sets the position of the polygon in world space.
        /// </summary>
        public Vect Position { get; set; }
        
        /// <summary>
        /// Gets or sets the rotation angle of the polygon in radians.
        /// </summary>
        public Num Rotation { get; set; }
        
        /// <summary>
        /// Gets the radius of the polygon (maximum distance from origin to any vertex).
        /// </summary>
        public Num Radius => LocalVertices.Select(v => v.Magnitude).Max();
        
        /// <summary>
        /// Gets the area of the polygon calculated using the shoelace formula.
        /// </summary>
        public Num Area => CalculateArea(LocalVertices);
        
        /// <summary>
        /// Gets the perimeter of the polygon (sum of all edge lengths).
        /// </summary>
        public Num Perimeter => Vect.Perimeter(LocalVertices);
        
        /// <summary>
        /// Gets the centroid (center of mass) of the polygon in global space.
        /// </summary>
        public Vect Centroid => Vect.Centroid(GlobalVertices);
        
        /// <summary>
        /// Gets the edge vectors of the polygon in global space.
        /// </summary>
        public Vect[] Edges => GetEdges(GlobalVertices).ToArray();
        
        /// <summary>
        /// Gets the normal axes of the polygon used for collision detection (SAT algorithm).
        /// </summary>
        public Vect[] Axes => Vect.GetAxes(GlobalVertices);
        
        /// <summary>
        /// Gets the axis-aligned bounding box of the polygon.
        /// </summary>
        public Polygon BoundingBox => GetBoundingBox(GlobalVertices);
        
        /// <summary>
        /// Gets a value indicating whether the polygon is convex.
        /// </summary>
        public bool Convex => IsConvex();
        
        /// <summary>
        /// Initializes a new instance of the Polygon class with the specified vertices, position, and rotation.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon in local space. Must contain at least 3 vertices.</param>
        /// <param name="position">The position of the polygon in world space. Default is (0,0).</param>
        /// <param name="rotation">The rotation angle of the polygon in radians. Default is 0.</param>
        /// <exception cref="ArgumentException">Thrown when vertices is null or contains fewer than 3 vertices.</exception>
        public Polygon(Vect[] vertices, Vect position = default, Num rotation = default)
        {
            if (vertices == null || vertices.Length < 3)
                throw new ArgumentException("Polygon must have at least 3 vertices");

            LocalVertices = vertices;
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Determines whether the polygon contains the specified point using the ray casting algorithm.
        /// </summary>
        /// <param name="point">The point to test for containment.</param>
        /// <returns>True if the point is inside the polygon; otherwise, false.</returns>
        public bool ContainsPoint(Vect point)
        {
            var vertices = GlobalVertices;
            int intersections = 0;
            
            for (int i = 0; i < vertices.Length; i++)
            {
                int j = (i + 1) % vertices.Length;
                
                if (((vertices[i].y > point.y) != (vertices[j].y > point.y)) &&
                    (point.x < (vertices[j].x - vertices[i].x) * (point.y - vertices[i].y) / 
                    (vertices[j].y - vertices[i].y) + vertices[i].x))
                {
                    intersections++;
                }
            }
            
            return (intersections % 2) == 1;
        }

        /// <summary>
        /// Finds the closest point on the polygon to the specified point.
        /// </summary>
        /// <param name="point">The point to find the closest point to.</param>
        /// <returns>A tuple containing the closest point on the polygon and the distance to that point.</returns>
        public (Vect point, Num Distance) ClosestPoint(Vect point)
        {
            return Vect.ClosestPoint(GlobalVertices, point);
        }

        /// <summary>
        /// Creates an axis-aligned bounding box polygon that encompasses all the specified vertices.
        /// </summary>
        /// <param name="vertices">The vertices to create a bounding box for.</param>
        /// <returns>A rectangular polygon representing the axis-aligned bounding box.</returns>
        public static Polygon GetBoundingBox(Vect[] vertices)
        {
            float minX = vertices.Min(v => v.x);
            float maxX = vertices.Max(v => v.x);
            float minY = vertices.Min(v => v.y);
            float maxY = vertices.Max(v => v.y);

            return new Polygon(new Vect[]
            {
                new Vect(minX, minY),
                new Vect(maxX, minY),
                new Vect(maxX, maxY),
                new Vect(minX, maxY)
            });
        }

        /// <summary>
        /// Determines whether this polygon intersects with another polygon using the Separating Axis Theorem (SAT).
        /// </summary>
        /// <param name="other">The other polygon to test intersection with.</param>
        /// <returns>True if the polygons intersect; otherwise, false.</returns>
        public bool Intersects(Polygon other)
        {
            return Collision.CheckSAT(GlobalVertices, other.GlobalVertices);
        }

        /// <summary>
        /// Rotates the polygon by the specified angle and normalizes the rotation to the range [0, 2π).
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        public void Rotate(Num angle)
        {
            Rotation += angle;
            while (Rotation >= 2 * Math.PI)
                Rotation -= (float)(2 * Math.PI);
            while (Rotation < 0)
                Rotation += (float)(2 * Math.PI);
        }

        /// <summary>
        /// Scales all local vertices of the polygon by the specified factor.
        /// </summary>
        /// <param name="factor">The scaling factor to apply to all vertices.</param>
        public void Scale(Num factor)
        {
            for (int i = 0; i < LocalVertices.Length; i++)
            {
                LocalVertices[i] *= factor;
            }
        }

        /// <summary>
        /// Determines whether the polygon is convex by checking if all cross products have the same sign.
        /// </summary>
        /// <returns>True if the polygon is convex; otherwise, false.</returns>
        private bool IsConvex()
        {
            if (LocalVertices.Length < 3) return false;

            bool sign = false;
            bool signSet = false;

            for (int i = 0; i < LocalVertices.Length; i++)
            {
                var v1 = LocalVertices[i];
                var v2 = LocalVertices[(i + 1) % LocalVertices.Length];
                var v3 = LocalVertices[(i + 2) % LocalVertices.Length];

                var cross = (v2.x - v1.x) * (v3.y - v1.y) - (v2.y - v1.y) * (v3.x - v1.x);

                if (!signSet)
                {
                    sign = cross > 0;
                    signSet = true;
                }
                else if (sign != (cross > 0))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the edge vectors of the polygon by calculating the differences between consecutive vertices.
        /// </summary>
        /// <param name="vertices">The vertices to calculate edges for.</param>
        /// <returns>An enumerable collection of edge vectors.</returns>
        public static IEnumerable<Vect> GetEdges(Vect[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                yield return vertices[(i + 1) % vertices.Length] - vertices[i];
            }
        }

        /// <summary>
        /// Determines whether two 1D projections overlap, used in SAT collision detection.
        /// </summary>
        /// <param name="p1">The first projection with min and max values.</param>
        /// <param name="p2">The second projection with min and max values.</param>
        /// <returns>True if the projections overlap; otherwise, false.</returns>
        public static bool ProjectionsOverlap((Num min, Num max) p1, (Num min, Num max) p2)
        {
            return p1.max >= p2.min && p2.max >= p1.min;
        }

        /// <summary>
        /// Calculates the moment of inertia of the polygon for physics simulations.
        /// Uses triangulation to handle both convex and concave polygons.
        /// </summary>
        /// <param name="mass">The total mass of the polygon.</param>
        /// <returns>The moment of inertia about the polygon's centroid.</returns>
        /// <exception cref="ArgumentException">Thrown when the polygon has fewer than 3 vertices.</exception>
        public Num MomentOfInertia(Num mass)
        {
            if (LocalVertices.Length < 3)
            { throw new ArgumentException("Polygon must have at least 3 vertices"); }

            Vect centroid = Centroid;
            Num totalMomentOfInertia = 0;
            Num totalArea = Area;

            // Handle concave polygons by ensuring positive area triangles
            for (Num i = 1; i < LocalVertices.Length - 1; i++)
            {
                Vect[] triangle =
                {
                    LocalVertices[0],
                    LocalVertices[i],
                    LocalVertices[i + 1]
                };

                Num triangleArea = Num.Abs(CalculateTriangleArea(triangle));
                Num triangleMass = mass * (triangleArea / totalArea);

                Vect triangleCentroid = (Vect)GetTriangleCentroid(triangle);
                Num triangleMoment = CalculateTriangleInertia(triangle, triangleMass);

                Num distanceSquared = Vect.DistanceSqr(triangleCentroid, centroid);
                totalMomentOfInertia += (triangleMass * distanceSquared) + triangleMoment;
            }

            return totalMomentOfInertia;
        }

        /// <summary>
        /// Calculates the centroid (geometric center) of a triangle.
        /// </summary>
        /// <param name="triangle">An array of three vertices representing the triangle.</param>
        /// <returns>The centroid point of the triangle.</returns>
        private static Vect GetTriangleCentroid(Vect[] triangle) => new((triangle[0].x + triangle[1].x + triangle[2].x) / 3, (triangle[0].y + triangle[1].y + triangle[2].y) / 3);

        /// <summary>
        /// Calculates the area of a polygon using the shoelace formula (surveyor's formula).
        /// Works for both convex and concave polygons.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon in order.</param>
        /// <returns>The absolute area of the polygon.</returns>
        /// <exception cref="ArgumentException">Thrown when vertices is null or contains fewer than 3 vertices.</exception>
        public static Num CalculateArea(Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                throw new ArgumentException("Invalid polygon: needs at least 3 vertices");
            }

            Num area = 0;
            Num j = vertices.Length - 1;

            // Shoelace formula (aka surveyor's formula)
            for (Num i = 0; i < vertices.Length; j = i++)
            {
                area += (vertices[j].x * vertices[i].y) - (vertices[i].x * vertices[j].y);
            }

            return Num.Abs(area / 2);
        }

        /// <summary>
        /// Calculates the area of a triangle using the cross product method for better numerical stability.
        /// </summary>
        /// <param name="triangle">An array of three vertices representing the triangle.</param>
        /// <returns>The absolute area of the triangle.</returns>
        /// <exception cref="ArgumentException">Thrown when triangle is null or doesn't contain exactly 3 vertices.</exception>
        public static double CalculateTriangleArea(Vect[] triangle)
        {
            if (triangle == null || triangle.Length != 3)
            { throw new ArgumentException("Invalid triangle array"); }

            // Using vector cross product for better numerical stability
            Vect v1 = triangle[1] - triangle[0];
            Vect v2 = triangle[2] - triangle[0];

            return Num.Abs((v1.x * v2.y) - (v1.y * v2.x)) / 2;
        }

        /// <summary>
        /// Calculates the moment of inertia of a triangle about its centroid.
        /// Used as part of polygon moment of inertia calculation through triangulation.
        /// </summary>
        /// <param name="triangle">An array of three vertices representing the triangle.</param>
        /// <param name="triangleMass">The mass of the triangle.</param>
        /// <returns>The moment of inertia of the triangle about its centroid.</returns>
        /// <exception cref="ArgumentException">Thrown when triangle is null or doesn't contain exactly 3 vertices.</exception>
        private static Num CalculateTriangleInertia(Vect[] triangle, Num triangleMass)
        {
            if (triangle == null || triangle.Length != 3)
            { throw new ArgumentException("Invalid triangle array"); }

            if (triangleMass <= 0)
            { return 0; }

            // Calculate triangle sides
            Num a = Vect.Distance(triangle[0], triangle[1]);
            Num b = Vect.Distance(triangle[1], triangle[2]);
            Num c = Vect.Distance(triangle[2], triangle[0]);

            // Calculate area
            Num s = (a + b + c) / 2; // semi-perimeter
            Num area = Num.Sqrt(s * (s - a) * (s - b) * (s - c));

            // Moment of inertia about centroid
            // Formula: (mass * (a² + b² + c²)) / 36
            return triangleMass * ((a * a) + (b * b) + (c * c)) / 36.0;
        }

        /// <summary>
        /// Finds the edge of the polygon that is closest to the specified point.
        /// Useful for collision response and contact point calculations.
        /// </summary>
        /// <param name="point">The point to find the closest edge to.</param>
        /// <returns>The edge vector closest to the specified point.</returns>
        public Vect EdgeAtPoint(Vect point)
        {
            Vect[] vertices = GlobalVertices;
            Num minDistSq = float.MaxValue;
            Vect edge = default;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vect v1 = vertices[i];
                Vect v2 = vertices[(i + 1) % vertices.Length];

                // Calculate point-to-line-segment distance squared
                Vect lineVec = v2 - v1;
                Vect pointVec = point - v1;
                Num lineLengthSq = lineVec.SqrMagnitude;

                // Project point onto line segment
                Num t = Num.Max(0, Num.Min(1, Vect.Dot(pointVec, lineVec) / lineLengthSq));
                Vect projection = v1 + (lineVec * t);
                Num distSq = Vect.DistanceSqr(point, projection);

                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    edge = lineVec;
                }
            }

            return edge;
        }
    }
}