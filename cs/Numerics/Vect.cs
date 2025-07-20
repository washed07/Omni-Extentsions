using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace OX.Numerics
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Represents a 4-dimensional vector with optimized mathematical operations.
    /// This structure provides comprehensive vector mathematics including 2D, 3D, and 4D operations,
    /// geometric calculations, interpolation methods, and efficient comparison operators.
    /// The vector uses a cached squared magnitude for performance optimization.
    /// </summary>
    /// <remarks>
    /// The Vect structure is designed for high-performance mathematical computations with:
    /// - Aggressive inlining for critical operations
    /// - Cached squared magnitude to avoid redundant calculations
    /// - Support for both 2D and 3D geometric operations
    /// - Comprehensive interpolation and transformation methods
    /// - Implicit conversions to/from System.Numerics types
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vect : IEquatable<Vect>, IComparable<Vect>
    {
        #region Core Fields

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public readonly Num x;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public readonly Num y;

        /// <summary>
        /// The Z component of the vector (defaults to 0 for 2D operations).
        /// </summary>
        public readonly Num z;

        /// <summary>
        /// The W component of the vector (defaults to 0, used for 4D operations or homogeneous coordinates).
        /// </summary>
        public readonly Num w;

        /// <summary>
        /// Cached squared magnitude to optimize repeated magnitude calculations.
        /// Computed once during construction to avoid redundant square root operations.
        /// </summary>
        private readonly Num? cachedSqrMagnitude;

        #endregion

        #region Static Constants

        /// <summary>Vector with all components set to zero (0, 0, 0, 0).</summary>
        public static readonly Vect Zero;

        /// <summary>Vector with all components set to one (1, 1, 1, 1).</summary>
        public static readonly Vect One;

        /// <summary>Unit vector pointing up in 2D space (0, -1, 0, 0).</summary>
        public static readonly Vect Up;

        /// <summary>Unit vector pointing down in 2D space (0, 1, 0, 0).</summary>
        public static readonly Vect Down;

        /// <summary>Unit vector pointing left in 2D space (-1, 0, 0, 0).</summary>
        public static readonly Vect Left;

        /// <summary>Unit vector pointing right in 2D space (1, 0, 0, 0).</summary>
        public static readonly Vect Right;

        /// <summary>Unit vector pointing forward in 3D space (0, -1, 0, 0).</summary>
        public static readonly Vect Forward;

        /// <summary>Unit vector pointing backward in 3D space (0, 1, 0, 0).</summary>
        public static readonly Vect Back;

        /// <summary>Unit vector along the X-axis (1, 0, 0, 0).</summary>
        public static readonly Vect UnitX;

        /// <summary>Unit vector along the Y-axis (0, 1, 0, 0).</summary>
        public static readonly Vect UnitY;

        /// <summary>Unit vector along the Z-axis (0, 0, 1, 0).</summary>
        public static readonly Vect UnitZ;

        /// <summary>Unit vector along the W-axis (0, 0, 0, 1).</summary>
        public static readonly Vect UnitW;

        /// <summary>Vector with all components set to positive infinity.</summary>
        public static readonly Vect PositiveInfinity;

        /// <summary>Vector with all components set to negative infinity.</summary>
        public static readonly Vect NegativeInfinity;

        #endregion

        #region Static Initializer

        /// <summary>
        /// Static constructor that initializes all predefined vector constants.
        /// This ensures consistent vector definitions across the application.
        /// </summary>
        static Vect()
        {
            // Initialize common vectors
            Zero = new(0, 0);
            One = new(1, 1);
            Up = new(0, -1);
            Down = new(0, 1);
            Left = new(-1, 0);
            Right = new(1, 0);
            Forward = new(0, -1);
            Back = new(0, 1);
            UnitX = new(1, 0);
            UnitY = new(0, 1);
            UnitZ = new(0, 0, 1);
            UnitW = new(0, 0, 0, 1);
            PositiveInfinity = new(float.PositiveInfinity, float.PositiveInfinity);
            NegativeInfinity = new(float.NegativeInfinity, float.NegativeInfinity);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new vector with the specified components.
        /// The squared magnitude is pre-calculated and cached for performance optimization.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector (optional, defaults to 0).</param>
        /// <param name="w">The W component of the vector (optional, defaults to 0).</param>
        /// <remarks>
        /// This constructor caches the squared magnitude to optimize repeated magnitude calculations.
        /// For vectors that will have their magnitude checked frequently, this provides significant performance benefits.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vect(Num x, Num y, Num z = default, Num w = default)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            cachedSqrMagnitude = (x * x) + (y * y) + (z * z) + (w * w);
        }

        #endregion

        #region Basic Properties

        /// <summary>
        /// Gets the squared magnitude (length squared) of the vector.
        /// This is more efficient than Magnitude when you only need to compare lengths
        /// since it avoids the expensive square root calculation.
        /// </summary>
        /// <value>The squared magnitude of the vector.</value>
        /// <remarks>
        /// Use this property instead of Magnitude when:
        /// - Comparing vector lengths (since sqrt is monotonic)
        /// - Checking if a vector is within a certain distance (compare against distance²)
        /// - Performing distance-based sorting operations
        /// </remarks>
        public readonly Num SqrMagnitude => cachedSqrMagnitude ?? ((x * x) + (y * y) + (z * z) + (w * w));

        /// <summary>
        /// Gets the magnitude (length) of the vector.
        /// This involves a square root calculation, so use SqrMagnitude when possible for better performance.
        /// </summary>
        /// <value>The magnitude of the vector.</value>
        public readonly Num Magnitude => Num.Sqrt(SqrMagnitude);

        /// <summary>
        /// Gets a normalized version of this vector (unit vector in the same direction).
        /// If the vector has zero magnitude, returns the zero vector to avoid division by zero.
        /// </summary>
        /// <value>A unit vector in the same direction, or zero vector if magnitude is zero.</value>
        public readonly Vect Normalized => Magnitude > Num.Epsilon ? this / Magnitude : Zero;

        /// <summary>
        /// Gets a vector perpendicular to this vector in 2D space.
        /// This rotates the vector 90 degrees counter-clockwise.
        /// </summary>
        /// <value>A vector perpendicular to this vector.</value>
        /// <remarks>
        /// For a 2D vector (x, y), the perpendicular vector is (-y, x).
        /// This is useful for calculating surface normals, collision detection, and geometric algorithms.
        /// </remarks>
        public readonly Vect Perpendicular => new(-y, x, z, w);

        /// <summary>
        /// Determines whether this vector is normalized (has a magnitude of approximately 1).
        /// Uses epsilon comparison to account for floating-point precision errors.
        /// </summary>
        /// <value>True if the vector is normalized, false otherwise.</value>
        public readonly bool IsNormalized => Math.Abs(SqrMagnitude - 1) < Num.Epsilon;

        /// <summary>
        /// Gets the polar coordinates (radius, angle) representation of this vector in 2D space.
        /// The angle is measured in radians from the positive X-axis.
        /// </summary>
        /// <value>A tuple containing the radius (magnitude) and angle in radians.</value>
        /// <remarks>
        /// The angle ranges from -π to π, following the standard mathematical convention.
        /// This is useful for rotational calculations and converting between coordinate systems.
        /// </remarks>
        public readonly (Num radius, Num angle) Polar => (Magnitude, Num.Atan2(y, x));

        #endregion

        #region Vector Component Swizzling

        /// <summary>Gets a 2D vector with Y and X components swapped.</summary>
        public Vect yx => new(y, x);

        /// <summary>Gets a 2D vector with Z and X components.</summary>
        public Vect zx => new(z, x);

        /// <summary>Gets a 2D vector with Z and Y components.</summary>
        public Vect zy => new(z, y);

        /// <summary>Gets a 2D vector with X and Y components.</summary>
        public Vect xy => new(x, y);

        /// <summary>Gets a 2D vector with X and Z components.</summary>
        public Vect xz => new(x, z);

        /// <summary>Gets a 2D vector with Y and Z components.</summary>
        public Vect yz => new(y, z);

        #endregion

        #region Basic Vector Operations

        /// <summary>
        /// Gets a vector with each component rounded down to the nearest integer.
        /// </summary>
        /// <value>A vector with floor-applied components.</value>
        public readonly Vect Floor => new(Math.Floor((double)x), Math.Floor((double)y), Math.Floor((double)z), Math.Floor((double)w));

        /// <summary>
        /// Gets a vector with each component rounded up to the nearest integer.
        /// </summary>
        /// <value>A vector with ceiling-applied components.</value>
        public readonly Vect Ceiling => new((Num)Math.Ceiling((double)x), (Num)Math.Ceiling((double)y), (Num)Math.Ceiling((double)z), (Num)Math.Ceiling((double)w));

        /// <summary>
        /// Gets a vector with the absolute value of each component.
        /// </summary>
        /// <value>A vector with all positive components.</value>
        public readonly Vect Abs => new(Num.Abs(x), Num.Abs(y), Num.Abs(z), Num.Abs(w));

        #endregion

        #region Geometric Operations

        /// <summary>
        /// Determines if this vector is parallel to another vector within floating-point precision.
        /// Two vectors are parallel if their cross product is approximately zero.
        /// </summary>
        /// <param name="other">The vector to compare against.</param>
        /// <returns>True if the vectors are parallel, false otherwise.</returns>
        /// <remarks>
        /// Parallel vectors point in the same or opposite directions.
        /// This method accounts for floating-point precision errors using epsilon comparison.
        /// </remarks>
        public readonly bool IsParallelTo(Vect other)
        {
            return Math.Abs(Math.Abs(Dot(this, other)) - (Magnitude * other.Magnitude)) < Num.Epsilon;
        }

        /// <summary>
        /// Determines if this vector is perpendicular to another vector within floating-point precision.
        /// Two vectors are perpendicular if their dot product is approximately zero.
        /// </summary>
        /// <param name="other">The vector to compare against.</param>
        /// <returns>True if the vectors are perpendicular, false otherwise.</returns>
        /// <remarks>
        /// Perpendicular vectors meet at a 90-degree angle.
        /// This method accounts for floating-point precision errors using epsilon comparison.
        /// </remarks>
        public readonly bool IsPerpendicularTo(Vect other)
        {
            return Math.Abs(Dot(this, other)) < Num.Epsilon;
        }

        /// <summary>
        /// Calculates the angle between this vector and another vector in radians.
        /// </summary>
        /// <param name="other">The vector to measure the angle to.</param>
        /// <returns>The angle between the vectors in radians (0 to π).</returns>
        /// <remarks>
        /// The returned angle is always positive and ranges from 0 to π radians.
        /// For signed angles in 2D, use SignedAngle method instead.
        /// </remarks>
        public readonly Num AngleTo(Vect other)
        {
            return Num.Acos(Dot(this, other) / (Magnitude * other.Magnitude));
        }

        /// <summary>
        /// Calculates the Euclidean distance between this vector and another vector.
        /// </summary>
        /// <param name="other">The target vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        public readonly Num DistanceTo(Vect other)
        {
            return (this - other).Magnitude;
        }

        /// <summary>
        /// Calculates the signed distance from this point to a plane defined by a point and normal.
        /// </summary>
        /// <param name="point">A point on the plane.</param>
        /// <param name="normal">The normal vector of the plane.</param>
        /// <returns>The signed distance to the plane (positive if on the normal side, negative otherwise).</returns>
        /// <remarks>
        /// The sign of the result indicates which side of the plane this point is on.
        /// Positive values indicate the point is on the side the normal vector points toward.
        /// </remarks>
        public readonly Num DistanceToPlane(Vect point, Vect normal)
        {
            return Dot(this - point, normal);
        }

        /// <summary>
        /// Projects this vector onto another vector.
        /// </summary>
        /// <param name="other">The vector to project onto.</param>
        /// <returns>The projection of this vector onto the other vector.</returns>
        /// <remarks>
        /// Vector projection finds the component of this vector in the direction of the other vector.
        /// The result is a vector that lies along the direction of the 'other' vector.
        /// </remarks>
        public readonly Vect ProjectOn(Vect other)
        {
            return Dot(this, other) / other.SqrMagnitude * other;
        }

        /// <summary>
        /// Reflects this vector off a surface with the given normal.
        /// </summary>
        /// <param name="normal">The normal vector of the surface to reflect off.</param>
        /// <returns>The reflected vector.</returns>
        /// <remarks>
        /// This calculates the reflection as if this vector were a light ray bouncing off a surface.
        /// The normal vector should be normalized for accurate results.
        /// </remarks>
        public readonly Vect ReflectOn(Vect normal)
        {
            return this - (2 * ProjectOn(normal));
        }

        #endregion

        #region Static Geometric Operations

        /// <summary>
        /// Calculates the angle bisector between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A normalized vector that bisects the angle between the two input vectors.</returns>
        /// <remarks>
        /// The angle bisector is the normalized sum of the two normalized input vectors.
        /// This is useful in graphics for calculating reflection directions and lighting calculations.
        /// </remarks>
        public static Vect Bisector(Vect a, Vect b)
        {
            return (a.Normalized + b.Normalized).Normalized;
        }

        /// <summary>
        /// Calculates the midpoint between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The vector that lies exactly halfway between the two input vectors.</returns>
        public static Vect Midpoint(Vect a, Vect b)
        {
            return (a + b) * 0.5f;
        }

        /// <summary>
        /// Calculates the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Num Distance(Vect a, Vect b) { return (a - b).Magnitude; }

        /// <summary>
        /// Calculates the squared distance between two vectors.
        /// This is more efficient than Distance when you only need to compare distances.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        /// <remarks>
        /// Use this instead of Distance when comparing distances, as it avoids the expensive square root calculation.
        /// Since square root is monotonic, comparing squared distances gives the same ordering as comparing distances.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Num DistanceSqr(Vect a, Vect b)
        {
            Num dx = a.x - b.x, dy = a.y - b.y, dz = a.z - b.z, dw = a.w - b.w;
            return (dx * dx) + (dy * dy) + (dz * dz) + (dw * dw);
        }

        #endregion

        #region Interpolation Methods

        /// <summary>
        /// Performs quadratic Bézier interpolation between three control points.
        /// </summary>
        /// <param name="a">The start point of the curve.</param>
        /// <param name="b">The control point that influences the curve's shape.</param>
        /// <param name="c">The end point of the curve.</param>
        /// <param name="t">The interpolation parameter (0 to 1).</param>
        /// <returns>A point on the quadratic Bézier curve at parameter t.</returns>
        /// <remarks>
        /// Quadratic Bézier curves provide smooth interpolation with one control point.
        /// t=0 returns point a, t=1 returns point c, and t=0.5 gives a point influenced by control point b.
        /// </remarks>
        public static Vect QuadraticBezier(Vect a, Vect b, Vect c, Num t)
        {
            Num u = 1 - t;
            return (u * u * a) + (2 * u * t * b) + (t * t * c);
        }

        /// <summary>
        /// Performs cubic Bézier interpolation between four control points.
        /// </summary>
        /// <param name="a">The start point of the curve.</param>
        /// <param name="b">The first control point.</param>
        /// <param name="c">The second control point.</param>
        /// <param name="d">The end point of the curve.</param>
        /// <param name="t">The interpolation parameter (0 to 1).</param>
        /// <returns>A point on the cubic Bézier curve at parameter t.</returns>
        /// <remarks>
        /// Cubic Bézier curves provide very smooth interpolation with two control points.
        /// This is the standard curve type used in vector graphics and animation systems.
        /// </remarks>
        public static Vect CubicBezier(Vect a, Vect b, Vect c, Vect d, Num t)
        {
            Num u = 1 - t;
            Num u2 = u * u;
            Num t2 = t * t;
            return (u2 * u * a) + (3 * u2 * t * b) + (3 * u * t2 * c) + (t2 * t * d);
        }

        /// <summary>
        /// Performs smooth step interpolation between two vectors using a cubic Hermite curve.
        /// </summary>
        /// <param name="a">The start vector.</param>
        /// <param name="b">The end vector.</param>
        /// <param name="t">The interpolation parameter (0 to 1).</param>
        /// <returns>A smoothly interpolated vector between a and b.</returns>
        /// <remarks>
        /// Smooth step provides a smooth acceleration and deceleration, unlike linear interpolation.
        /// The derivatives at t=0 and t=1 are zero, creating smooth transitions.
        /// </remarks>
        public static Vect SmoothStep(Vect a, Vect b, Num t)
        {
            t = Num.Clamp((t - 0) / (1 - 0), 0, 1);
            t = t * t * (3 - (2 * t));
            return Lerp(a, b, t);
        }

        /// <summary>
        /// Linearly interpolates from this vector toward a target vector.
        /// </summary>
        /// <param name="target">The target vector to interpolate toward.</param>
        /// <param name="t">The interpolation parameter (0 to 1).</param>
        /// <returns>A vector interpolated between this vector and the target.</returns>
        /// <remarks>
        /// Linear interpolation provides the shortest path between two points.
        /// t=0 returns this vector, t=1 returns the target vector.
        /// </remarks>
        public readonly Vect Lerp(Vect target, Num t)
        {
            return this + ((target - this) * t);
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="a">The start vector.</param>
        /// <param name="b">The end vector.</param>
        /// <param name="t">The interpolation parameter (0 to 1).</param>
        /// <returns>A vector linearly interpolated between a and b.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect Lerp(Vect a, Vect b, Num t) { return a + ((b - a) * t); }

        /// <summary>
        /// Performs Catmull-Rom spline interpolation between four control points.
        /// </summary>
        /// <param name="p0">The point before the start of the curve segment.</param>
        /// <param name="p1">The start point of the curve segment.</param>
        /// <param name="p2">The end point of the curve segment.</param>
        /// <param name="p3">The point after the end of the curve segment.</param>
        /// <param name="t">The interpolation parameter (0 to 1).</param>
        /// <returns>A point on the Catmull-Rom spline at parameter t.</returns>
        /// <remarks>
        /// Catmull-Rom splines pass through all control points and provide smooth curves
        /// that are particularly useful for creating smooth paths through a series of points.
        /// The curve segment goes from p1 to p2, with p0 and p3 influencing the tangents.
        /// </remarks>
        public static Vect CatmullRom(Vect p0, Vect p1, Vect p2, Vect p3, Num t)
        {
            Num t2 = t * t;
            Num t3 = t2 * t;

            return 0.5f * (
                (2 * p1) +
                ((-p0 + p2) * t) +
                (((2 * p0) - (5 * p1) + (4 * p2) - p3) * t2) +
                ((-p0 + (3 * p1) - (3 * p2) + p3) * t3)
            );
        }

        #endregion

        #region Conversion Methods

        /// <summary>
        /// Creates a vector from polar coordinates.
        /// </summary>
        /// <param name="radius">The distance from the origin.</param>
        /// <param name="angle">The angle in radians from the positive X-axis.</param>
        /// <returns>A vector representing the polar coordinates in Cartesian form.</returns>
        /// <remarks>
        /// Converts polar coordinates (r, θ) to Cartesian coordinates (x, y).
        /// The angle is measured counter-clockwise from the positive X-axis.
        /// </remarks>
        public static Vect FromPolar(Num radius, Num angle)
        {
            return new(radius * Num.Cos(angle), radius * Num.Sin(angle));
        }

        /// <summary>
        /// Creates a unit vector from an angle in radians.
        /// </summary>
        /// <param name="angle">The angle in radians from the positive X-axis.</param>
        /// <returns>A unit vector pointing in the specified direction.</returns>
        /// <remarks>
        /// This is equivalent to FromPolar(1, angle).
        /// Useful for creating direction vectors from angles.
        /// </remarks>
        public static Vect FromAngle(Num angle)
        {
            return new Vect(Num.Cos(angle), Num.Sin(angle));
        }

        /// <summary>
        /// Creates a unit vector from an angle in degrees.
        /// </summary>
        /// <param name="angleDegrees">The angle in degrees from the positive X-axis.</param>
        /// <returns>A unit vector pointing in the specified direction.</returns>
        /// <remarks>
        /// Converts the angle from degrees to radians and creates a unit vector.
        /// Useful when working with degree-based angle systems.
        /// </remarks>
        public static Vect FromAngleDegrees(Num angleDegrees)
        {
            Num angleRadians = angleDegrees * Num.PI / 180;
            return FromAngle(angleRadians);
        }

        #endregion

        #region Vector Comparison Helper

        /// <summary>
        /// Custom equality comparer for vectors that uses approximate equality comparison.
        /// This is useful for hash sets and dictionaries where floating-point precision matters.
        /// </summary>
        private class VectComparer : IEqualityComparer<Vect>
        {
            /// <summary>
            /// Determines whether two vectors are approximately equal.
            /// </summary>
            /// <param name="x">The first vector to compare.</param>
            /// <param name="y">The second vector to compare.</param>
            /// <returns>True if the vectors are approximately equal, false otherwise.</returns>
            public bool Equals(Vect x, Vect y)
            {
                return x.ApproximatelyEquals(y);
            }

            /// <summary>
            /// Returns a hash code for the specified vector.
            /// </summary>
            /// <param name="obj">The vector for which a hash code is to be returned.</param>
            /// <returns>A hash code for the specified vector.</returns>
            public int GetHashCode(Vect obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion

        #region Vertex Manipulation and Geometric Analysis

        /// <summary>
        /// Calculates the perpendicular axes for a set of vertices, typically used in collision detection.
        /// </summary>
        /// <param name="vectors">An array of vertices forming a polygon.</param>
        /// <returns>An array of unique perpendicular vectors (axes) for the polygon edges.</returns>
        /// <remarks>
        /// This method is used in the Separating Axis Theorem (SAT) for collision detection.
        /// Each edge of the polygon generates a perpendicular axis that can be used for projection testing.
        /// Duplicate axes are automatically removed using approximate equality comparison.
        /// </remarks>
        public static Vect[] GetAxes(Vect[] vectors)
        {
            HashSet<Vect> axes = new(new VectComparer());

            for (int i = 0; i < vectors.Length; i++)
            {
                Vect edge = vectors[(i + 1) % vectors.Length] - vectors[i];
                axes.Add(edge.Perpendicular);
            }

            return [.. axes];
        }

        /// <summary>
        /// Calculates normalized perpendicular axes for a set of vertices.
        /// </summary>
        /// <param name="vectors">An array of vertices forming a polygon.</param>
        /// <returns>An array of unique normalized perpendicular vectors for the polygon edges.</returns>
        /// <remarks>
        /// Similar to GetAxes, but returns normalized vectors suitable for projection calculations.
        /// Normalized axes ensure consistent projection magnitudes regardless of edge lengths.
        /// </remarks>
        public static Vect[] GetNormalAxes(Vect[] vectors)
        {
            Vect[] axes = GetAxes(vectors);
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i] = axes[i].Normalized;
            }
            return [.. axes];
        }

        /// <summary>
        /// Calculates the edge vectors for a polygon defined by vertices.
        /// </summary>
        /// <param name="vectors">An array of vertices forming a polygon.</param>
        /// <returns>An array of edge vectors connecting consecutive vertices.</returns>
        /// <remarks>
        /// Each edge vector points from one vertex to the next, with the last edge connecting
        /// the final vertex back to the first vertex to close the polygon.
        /// </remarks>
        public static Vect[] GetEdges(Vect[] vectors)
        {
            Vect[] edges = new Vect[vectors.Length];
            for (int i = 0; i < vectors.Length; i++)
            {
                edges[i] = vectors[(i + 1) % vectors.Length] - vectors[i];
            }
            return edges;
        }

        /// <summary>
        /// Determines if two 1D projection ranges overlap.
        /// </summary>
        /// <param name="p1">The first projection range (min, max).</param>
        /// <param name="p2">The second projection range (min, max).</param>
        /// <returns>True if the projections overlap, false otherwise.</returns>
        /// <remarks>
        /// Used in collision detection algorithms like SAT to determine if projected shapes overlap on an axis.
        /// Two ranges overlap if the maximum of one is greater than or equal to the minimum of the other.
        /// </remarks>
        public static bool ProjectionsOverlap((Num min, Num max) p1, (Num min, Num max) p2)
        {
            return p1.max >= p2.min && p2.max >= p1.min;
        }

        /// <summary>
        /// Calculates the perimeter of a polygon defined by vertices.
        /// </summary>
        /// <param name="vectors">An array of vertices forming a polygon.</param>
        /// <returns>The total perimeter length of the polygon.</returns>
        /// <remarks>
        /// Sums the distances between consecutive vertices, including the distance from the last vertex back to the first.
        /// </remarks>
        public static Num Perimeter(Vect[] vectors)
        {
            Num perimeter = 0;
            for (Num i = 0; i < vectors.Length; i++)
            {
                Num nextIndex = (i + 1) % vectors.Length;
                perimeter += Vect.Distance(vectors[i], vectors[nextIndex]);
            }

            return perimeter;
        }

        /// <summary>
        /// Transforms an array of vertices by applying rotation and translation.
        /// </summary>
        /// <param name="vectors">The vertices to transform.</param>
        /// <param name="position">The translation offset to apply.</param>
        /// <param name="rotation">The rotation angle in radians to apply.</param>
        /// <returns>A new array of transformed vertices.</returns>
        /// <remarks>
        /// Applies rotation first, then translation. This is the standard order for 2D transformations.
        /// All vertices are rotated around the origin before being translated.
        /// </remarks>
        public static Vect[] Transform(Vect[] vectors, Vect position, Num rotation)
        {
            Vect[] transformed = new Vect[vectors.Length];

            for (int i = 0; i < vectors.Length; i++)
            {
                Vect rotated = Rotate(vectors[i], rotation);
                transformed[i] = rotated + position;
            }

            return transformed;
        }

        /// <summary>
        /// Finds the vertex with the greatest magnitude (distance from origin).
        /// </summary>
        /// <param name="vectors">The array of vertices to search.</param>
        /// <returns>The vertex with the greatest magnitude.</returns>
        /// <remarks>
        /// Uses squared magnitude comparison for efficiency, avoiding unnecessary square root calculations.
        /// </remarks>
        public static Vect FurthestPoint(Vect[] vectors)
        {
            return vectors.MaxBy(v => v.SqrMagnitude);
        }

        /// <summary>
        /// Finds the vertex that extends furthest in a given direction.
        /// </summary>
        /// <param name="vectors">The array of vertices to search.</param>
        /// <param name="direction">The direction vector to project onto.</param>
        /// <returns>The vertex with the maximum projection in the given direction.</returns>
        /// <remarks>
        /// This is used in support function implementations for collision detection algorithms like GJK.
        /// The direction vector does not need to be normalized.
        /// </remarks>
        public static Vect FurthestPoint(Vect[] vectors, Vect direction)
        {
            return vectors.MaxBy(v => Dot(v, direction));
        }

        /// <summary>
        /// Finds the closest point on a polygon boundary to a given point.
        /// </summary>
        /// <param name="vectors">The vertices of the polygon.</param>
        /// <param name="point">The point to find the closest approach to.</param>
        /// <returns>A tuple containing the closest point and the distance to it.</returns>
        /// <remarks>
        /// Checks all edges of the polygon and returns the closest point on any edge.
        /// Uses point-to-line-segment distance calculations for accuracy.
        /// </remarks>
        public static (Vect point, Num distance) ClosestPoint(Vect[] vectors, Vect point)
        {
            Num minDistSq = float.MaxValue;
            Vect closest = default;

            for (int i = 0; i < vectors.Length; i++)
            {
                Vect v1 = vectors[i];
                Vect v2 = vectors[(i + 1) % vectors.Length];
                Vect lineVec = v2 - v1;
                Vect pointVec = point - v1;

                Num lineLengthSq = lineVec.SqrMagnitude;
                Num t = Num.Clamp(Vect.Dot(pointVec, lineVec) / lineLengthSq, 0, 1);
                Vect projection = v1 + (lineVec * t);
                Num distSq = Vect.DistanceSqr(point, projection);

                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    closest = projection;
                }
            }

            return (closest, Num.Sqrt(minDistSq));
        }

        /// <summary>
        /// Calculates the centroid (center of mass) of a set of vertices.
        /// </summary>
        /// <param name="vectors">The array of vertices.</param>
        /// <returns>The centroid vector representing the average position of all vertices.</returns>
        /// <exception cref="ArgumentException">Thrown when the vector array is null or empty.</exception>
        /// <remarks>
        /// The centroid is calculated as the arithmetic mean of all vertex positions.
        /// This assumes uniform mass distribution across all vertices.
        /// </remarks>
        public static Vect Centroid(Vect[] vectors)
        {
            if (vectors == null || vectors.Length == 0)
            {
                throw new ArgumentException("Vector array must have at least 1 point");
            }

            Num x = 0, y = 0, z = 0, w = 0;
            foreach (Vect v in vectors)
            {
                x += v.x;
                y += v.y;
                z += v.z;
                w += v.w;
            }

            Num count = vectors.Length;
            return new Vect(x / count, y / count, z / count, w / count);
        }

        #endregion

        #region 2D Cross Product and Projection

        /// <summary>
        /// Calculates the 2D cross product (determinant) of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The scalar result of the 2D cross product.</returns>
        /// <remarks>
        /// In 2D, the cross product returns a scalar representing the Z-component of the 3D cross product.
        /// This value indicates the orientation: positive for counter-clockwise, negative for clockwise.
        /// The magnitude represents twice the area of the triangle formed by the vectors and origin.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Num Cross2D(Vect a, Vect b)
        {
            return (a.x * b.y) - (a.y * b.x);
        }

        /// <summary>
        /// Projects an array of vectors onto an axis and returns the projection range.
        /// </summary>
        /// <param name="vectors">The vectors to project.</param>
        /// <param name="axis">The axis to project onto.</param>
        /// <returns>A tuple containing the minimum and maximum projection values.</returns>
        /// <exception cref="ArgumentException">Thrown when the vector array is null or empty.</exception>
        /// <remarks>
        /// Used in collision detection algorithms like SAT to determine if shapes overlap on a given axis.
        /// The axis vector does not need to be normalized, but normalization provides more intuitive results.
        /// </remarks>
        public static (float min, float max) Project(Vect[] vectors, Vect axis)
        {
            if (vectors == null || vectors.Length == 0)
            {
                throw new ArgumentException("Vector array cannot be null or empty");
            }

            float min = Dot(vectors[0], axis);
            float max = min;

            for (int i = 1; i < vectors.Length; i++)
            {
                float projection = Dot(vectors[i], axis);
                min = Math.Min(min, projection);
                max = Math.Max(max, projection);
            }

            return (min, max);
        }

        #endregion

        #region Arithmetic Operators

        /// <summary>
        /// Negates all components of a vector (unary minus operator).
        /// </summary>
        /// <param name="a">The vector to negate.</param>
        /// <returns>A vector with all components negated.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator -(Vect a)
        {
            return new(-a.x, -a.y, -a.z, -a.w);
        }

        /// <summary>
        /// Clamps a vector's components between corresponding components of min and max vectors.
        /// </summary>
        /// <param name="value">The vector to clamp.</param>
        /// <param name="min">The minimum values for each component.</param>
        /// <param name="max">The maximum values for each component.</param>
        /// <returns>A vector with components clamped between min and max.</returns>
        public static Vect Clamp(Vect value, Vect min, Vect max) { return Min(Max(value, min), max); }

        /// <summary>
        /// Adds two vectors component-wise.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The sum of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator +(Vect a, Vect b) { return new Vect(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w); }

        /// <summary>
        /// Subtracts the second vector from the first vector component-wise.
        /// </summary>
        /// <param name="a">The vector to subtract from.</param>
        /// <param name="b">The vector to subtract.</param>
        /// <returns>The difference between the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator -(Vect a, Vect b) { return new Vect(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w); }

        /// <summary>
        /// Multiplies a vector by a scalar value.
        /// </summary>
        /// <param name="a">The vector to multiply.</param>
        /// <param name="b">The scalar multiplier.</param>
        /// <returns>The vector scaled by the scalar value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator *(Vect a, Num b) { return new Vect(a.x * b, a.y * b, a.z * b, a.w * b); }

        /// <summary>
        /// Adds a scalar value to all components of a vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <param name="b">The scalar value to add.</param>
        /// <returns>A vector with the scalar added to all components.</returns>
        public static Vect operator +(Vect a, Num b) { return new Vect(a.x + b, a.y + b, a.z + b, a.w + b); }

        /// <summary>
        /// Adds a vector to a scalar value (commutative addition).
        /// </summary>
        /// <param name="a">The scalar value.</param>
        /// <param name="b">The vector.</param>
        /// <returns>A vector with the scalar added to all components.</returns>
        public static Vect operator +(Num a, Vect b) { return new Vect(a + b.x, a + b.y, a + b.z, a + b.w); }

        /// <summary>
        /// Multiplies two vectors component-wise (Hadamard product).
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector with components multiplied component-wise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator *(Vect a, Vect b) { return new Vect(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w); }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="a">The vector to divide.</param>
        /// <param name="b">The scalar divisor.</param>
        /// <returns>The vector divided by the scalar value.</returns>
        /// <remarks>
        /// Uses multiplication by reciprocal for better performance.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator /(Vect a, Num b)
        {
            Num invB = 1 / b;
            return new Vect(a.x * invB, a.y * invB, a.z * invB, a.w * invB);
        }

        /// <summary>
        /// Divides two vectors component-wise.
        /// </summary>
        /// <param name="a">The dividend vector.</param>
        /// <param name="b">The divisor vector.</param>
        /// <returns>A vector with components divided component-wise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator /(Vect a, Vect b) { return new Vect(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w); }

        /// <summary>
        /// Multiplies a scalar by a vector (commutative multiplication).
        /// </summary>
        /// <param name="a">The scalar multiplier.</param>
        /// <param name="b">The vector to multiply.</param>
        /// <returns>The vector scaled by the scalar value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator *(Num a, Vect b) { return b * a; }

        /// <summary>
        /// Divides a scalar by each component of a vector.
        /// </summary>
        /// <param name="a">The scalar dividend.</param>
        /// <param name="b">The vector divisor.</param>
        /// <returns>A vector with the scalar divided by each component.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator /(Num a, Vect b) { return new Vect(a / b.x, a / b.y, a / b.z, a / b.w); }

        /// <summary>
        /// Subtracts a scalar value from all components of a vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <param name="b">The scalar value to subtract.</param>
        /// <returns>A vector with the scalar subtracted from all components.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect operator -(Vect a, Num b) { return new Vect(a.x - b, a.y - b, a.z - b, a.w - b); }

        #endregion

        #region Comparison Operators

        /// <summary>
        /// Determines if the first vector has a greater magnitude than the second vector.
        /// Uses squared magnitude comparison for efficiency.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>True if the first vector has greater magnitude, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Vect a, Vect b) { return a.SqrMagnitude > b.SqrMagnitude; }

        /// <summary>
        /// Determines if the first vector has a smaller magnitude than the second vector.
        /// Uses squared magnitude comparison for efficiency.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>True if the first vector has smaller magnitude, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Vect a, Vect b) { return a.SqrMagnitude < b.SqrMagnitude; }

        #endregion

        #region Type Conversions

        /// <summary>
        /// Implicitly converts a 3D tuple to a vector.
        /// </summary>
        /// <param name="tuple">The tuple containing (x, y, z) components.</param>
        /// <returns>A vector with the specified components and w=0.</returns>
        public static implicit operator Vect((Num x, Num y, Num z) tuple) { return new Vect(tuple.x, tuple.y, tuple.z); }

        /// <summary>
        /// Implicitly converts a 2D tuple to a vector.
        /// </summary>
        /// <param name="tuple">The tuple containing (x, y) components.</param>
        /// <returns>A vector with the specified components and z=w=0.</returns>
        public static implicit operator Vect((Num x, Num y) tuple) { return new Vect(tuple.x, tuple.y); }

        /// <summary>
        /// Implicitly converts a System.Numerics.Vector2 to a Vect.
        /// </summary>
        /// <param name="v">The Vector2 to convert.</param>
        /// <returns>A Vect with x and y components from the Vector2.</returns>
        public static implicit operator Vect(Vector2 v) { return new Vect(v.X, v.Y); }

        /// <summary>
        /// Implicitly converts a System.Numerics.Vector3 to a Vect.
        /// </summary>
        /// <param name="v">The Vector3 to convert.</param>
        /// <returns>A Vect with x, y, and z components from the Vector3.</returns>
        public static implicit operator Vect(Vector3 v) { return new Vect(v.X, v.Y, v.Z); }

        /// <summary>
        /// Implicitly converts a Vect to a System.Numerics.Vector2.
        /// </summary>
        /// <param name="v">The Vect to convert.</param>
        /// <returns>A Vector2 with x and y components from the Vect.</returns>
        public static implicit operator Vector2(Vect v) { return new Vector2(v.x, v.y); }

        /// <summary>
        /// Implicitly converts a Vect to a System.Numerics.Vector3.
        /// </summary>
        /// <param name="v">The Vect to convert.</param>
        /// <returns>A Vector3 with x, y, and z components from the Vect.</returns>
        public static implicit operator Vector3(Vect v) { return new Vector3(v.x, v.y, v.z); }

        #endregion

        #region Equality and Comparison

        /// <summary>
        /// Determines whether the specified object is equal to this vector.
        /// </summary>
        /// <param name="obj">The object to compare with this vector.</param>
        /// <returns>True if the object is a Vect with equal components, false otherwise.</returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is Vect vect && vect == this;
        }

        /// <summary>
        /// Determines whether another vector is equal to this vector.
        /// </summary>
        /// <param name="other">The vector to compare with this vector.</param>
        /// <returns>True if all components are exactly equal, false otherwise.</returns>
        public readonly bool Equals(Vect other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w) && Nullable.Equals
                (cachedSqrMagnitude, other.cachedSqrMagnitude);
        }

        /// <summary>
        /// Compares this vector to another vector based on magnitude.
        /// </summary>
        /// <param name="other">The vector to compare to.</param>
        /// <returns>A value indicating the relative magnitude order of the vectors.</returns>
        public readonly int CompareTo(Vect other) { return Magnitude.CompareTo(other.Magnitude); }

        /// <summary>
        /// Returns a hash code for this vector.
        /// </summary>
        /// <returns>A hash code suitable for use in hash-based collections.</returns>
        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + x.GetHashCode();
                hash = (hash * 31) + y.GetHashCode();
                hash = (hash * 31) + z.GetHashCode();
                return (hash * 31) + w.GetHashCode();
            }
        }

        /// <summary>
        /// Tests two vectors for exact equality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>True if all components are exactly equal, false otherwise.</returns>
        public static bool operator ==(Vect a, Vect b) { return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w; }

        /// <summary>
        /// Tests two vectors for inequality.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>True if any components are not equal, false otherwise.</returns>
        public static bool operator !=(Vect a, Vect b) { return !(a == b); }

        #endregion

        #region String Formatting and Conversion

        /// <summary>
        /// Default format string for vector string representation.
        /// </summary>
        private static readonly string Format = "({0:F6}, {1:F6}, {2:F6}, {3:F6})";

        /// <summary>
        /// Returns a string representation of this vector using the default format.
        /// </summary>
        /// <returns>A string representation showing all four components with 6 decimal places.</returns>
        public override readonly string ToString() { return string.Format(Format, x, y, z, w); }

        /// <summary>
        /// Returns a string representation of this vector using the specified format.
        /// </summary>
        /// <param name="format">The format string to apply to each component.</param>
        /// <returns>A formatted string representation of the vector.</returns>
        public readonly string ToString(string format)
        {
            return $"({x.ToString(format, System.Globalization.CultureInfo.InvariantCulture)}, {y.ToString(format, System.Globalization.CultureInfo.InvariantCulture)}, {z.ToString(format, System.Globalization.CultureInfo.InvariantCulture)}, {w.ToString(format, System.Globalization.CultureInfo.InvariantCulture)})";
        }

        /// <summary>
        /// Returns a string representation of this vector using the specified format provider.
        /// </summary>
        /// <param name="formatProvider">The format provider to use for number formatting.</param>
        /// <returns>A formatted string representation of the vector.</returns>
        public readonly string ToString(IFormatProvider formatProvider)
        {
            return $"({x.ToString(null, formatProvider)}, {y.ToString(null, formatProvider)}, {z.ToString(null, formatProvider)}, {w.ToString(null, formatProvider)})";
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Determines if this vector is approximately equal to another vector within a specified tolerance.
        /// </summary>
        /// <param name="other">The vector to compare against.</param>
        /// <param name="tolerance">The maximum allowed difference (default: 1e-5).</param>
        /// <returns>True if the vectors are approximately equal, false otherwise.</returns>
        /// <remarks>
        /// Uses squared distance comparison for efficiency. This method is useful when dealing with
        /// floating-point precision issues that can cause exact equality comparisons to fail.
        /// </remarks>
        public readonly bool ApproximatelyEquals(Vect other, float tolerance = 1e-5f)
        {
            return (this - other).SqrMagnitude <= tolerance * tolerance;
        }

        /// <summary>
        /// Rounds each component of the vector to the specified number of decimal places.
        /// </summary>
        /// <param name="decimals">The number of decimal places to round to (default: 0).</param>
        /// <returns>A vector with rounded components.</returns>
        public readonly Vect Round(int decimals = 0)
        {
            return new(Math.Round((double)x, decimals), Math.Round((double)y, decimals),
                Math.Round((double)z, decimals), Math.Round((double)w, decimals));
        }

        /// <summary>
        /// Clamps each component of this vector between the corresponding components of min and max vectors.
        /// </summary>
        /// <param name="min">The vector containing minimum values for each component.</param>
        /// <param name="max">The vector containing maximum values for each component.</param>
        /// <returns>A vector with each component clamped between the corresponding min and max values.</returns>
        public readonly Vect Clamp(Vect min, Vect max)
        {
            return new(Num.Clamp(x, min.x, max.x), Num.Clamp(y, min.y, max.y), Num.Clamp(z, min.z, max.z), Num.Clamp(w, min.w, max.w));
        }

        /// <summary>
        /// Applies a selector function to this vector and returns the result as a single-element array.
        /// </summary>
        /// <param name="selector">The function to apply to this vector.</param>
        /// <returns>An array containing the result of applying the selector to this vector.</returns>
        /// <remarks>
        /// This method provides LINQ-like functionality for vector operations.
        /// </remarks>
        public readonly Vect[] Select(Func<Vect, Vect> selector)
        {
            return [selector(this)];
        }

        #endregion

        #region Deconstructors

        /// <summary>
        /// Deconstructs this vector into x and y components.
        /// </summary>
        /// <param name="x">The x component of the vector.</param>
        /// <param name="y">The y component of the vector.</param>
        public void Deconstruct(out Num x, out Num y)
        {
            x = this.x;
            y = this.y;
        }

        /// <summary>
        /// Deconstructs this vector into x, y, and z components.
        /// </summary>
        /// <param name="x">The x component of the vector.</param>
        /// <param name="y">The y component of the vector.</param>
        /// <param name="z">The z component of the vector.</param>
        public void Deconstruct(out Num x, out Num y, out Num z)
        {
            x = this.x;
            y = this.y;
            z = this.z;
        }

        /// <summary>
        /// Deconstructs this vector into x, y, z, and w components.
        /// </summary>
        /// <param name="x">The x component of the vector.</param>
        /// <param name="y">The y component of the vector.</param>
        /// <param name="z">The z component of the vector.</param>
        /// <param name="w">The w component of the vector.</param>
        public void Deconstruct(out Num x, out Num y, out Num z, out Num w)
        {
            x = this.x;
            y = this.y;
            z = this.z;
            w = this.w;
        }

        #endregion

        #region Triangulation Methods

        /// <summary>
        /// Triangulates a polygon defined by an array of vertices using the ear clipping algorithm.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon in counter-clockwise order.</param>
        /// <returns>An array of triangle indices, where each triplet represents a triangle.</returns>
        /// <exception cref="ArgumentException">Thrown when the vertex array has fewer than 3 vertices.</exception>
        /// <remarks>
        /// This method uses the ear clipping algorithm to decompose a simple polygon into triangles.
        /// The input vertices should form a simple polygon (no self-intersections) and be in counter-clockwise order.
        /// Returns indices into the original vertex array, where every 3 consecutive indices form a triangle.
        /// </remarks>
        public static int[] Triangulate(Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                throw new ArgumentException("Polygon must have at least 3 vertices");
            }

            if (vertices.Length == 3)
            {
                return [0, 1, 2];
            }

            List<int> indices = [];
            List<int> vertexList = Enumerable.Range(0, vertices.Length).ToList();

            // Ensure counter-clockwise winding
            if (IsClockwise(vertices))
            {
                vertexList.Reverse();
            }

            int iterations = 0;
            int maxIterations = vertices.Length * vertices.Length; // Prevent infinite loops

            while (vertexList.Count > 3 && iterations < maxIterations)
            {
                bool earFound = false;

                for (int i = 0; i < vertexList.Count; i++)
                {
                    int prev = vertexList[(i - 1 + vertexList.Count) % vertexList.Count];
                    int curr = vertexList[i];
                    int next = vertexList[(i + 1) % vertexList.Count];

                    if (IsEar(vertices, vertexList, prev, curr, next))
                    {
                        // Add triangle
                        indices.Add(prev);
                        indices.Add(curr);
                        indices.Add(next);

                        // Remove the ear vertex
                        vertexList.RemoveAt(i);
                        earFound = true;
                        break;
                    }
                }

                if (!earFound)
                {
                    // Fallback: force remove a vertex to prevent infinite loop
                    if (vertexList.Count > 3)
                    {
                        int prev = vertexList[vertexList.Count - 1];
                        int curr = vertexList[0];
                        int next = vertexList[1];

                        indices.Add(prev);
                        indices.Add(curr);
                        indices.Add(next);

                        vertexList.RemoveAt(0);
                    }
                }

                iterations++;
            }

            // Add the final triangle
            if (vertexList.Count == 3)
            {
                indices.Add(vertexList[0]);
                indices.Add(vertexList[1]);
                indices.Add(vertexList[2]);
            }

            return [.. indices];
        }

        /// <summary>
        /// Determines if a polygon is wound in clockwise order.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>True if the polygon is clockwise, false if counter-clockwise.</returns>
        /// <remarks>
        /// Uses the shoelace formula to calculate the signed area of the polygon.
        /// Positive area indicates counter-clockwise winding, negative indicates clockwise.
        /// </remarks>
        private static bool IsClockwise(Vect[] vertices)
        {
            Num sum = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                int next = (i + 1) % vertices.Length;
                sum += (vertices[next].x - vertices[i].x) * (vertices[next].y + vertices[i].y);
            }
            return sum > 0;
        }

        /// <summary>
        /// Determines if a vertex forms an "ear" that can be safely removed during triangulation.
        /// </summary>
        /// <param name="vertices">The complete vertex array.</param>
        /// <param name="vertexList">The current list of remaining vertex indices.</param>
        /// <param name="prev">Index of the previous vertex.</param>
        /// <param name="curr">Index of the current vertex.</param>
        /// <param name="next">Index of the next vertex.</param>
        /// <returns>True if the vertex forms an ear, false otherwise.</returns>
        /// <remarks>
        /// An ear is a triangle formed by three consecutive vertices where:
        /// 1. The triangle is convex (internal angle less than 180 degrees)
        /// 2. No other vertices lie inside the triangle
        /// </remarks>
        private static bool IsEar(Vect[] vertices, List<int> vertexList, int prev, int curr, int next)
        {
            Vect a = vertices[prev];
            Vect b = vertices[curr];
            Vect c = vertices[next];

            // Check if the angle is convex (less than 180 degrees)
            if (Cross2D(b - a, c - b) <= 0)
            {
                return false;
            }

            // Check if any other vertex is inside the triangle
            for (int i = 0; i < vertexList.Count; i++)
            {
                int idx = vertexList[i];
                if (idx == prev || idx == curr || idx == next)
                    continue;

                if (IsPointInTriangle(vertices[idx], a, b, c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a point lies inside a triangle using barycentric coordinates.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="a">First vertex of the triangle.</param>
        /// <param name="b">Second vertex of the triangle.</param>
        /// <param name="c">Third vertex of the triangle.</param>
        /// <returns>True if the point is inside the triangle, false otherwise.</returns>
        /// <remarks>
        /// Uses the barycentric coordinate method to determine point-in-triangle inclusion.
        /// This method is robust and handles edge cases well.
        /// </remarks>
        private static bool IsPointInTriangle(Vect point, Vect a, Vect b, Vect c)
        {
            Vect v0 = c - a;
            Vect v1 = b - a;
            Vect v2 = point - a;

            Num dot00 = Dot(v0, v0);
            Num dot01 = Dot(v0, v1);
            Num dot02 = Dot(v0, v2);
            Num dot11 = Dot(v1, v1);
            Num dot12 = Dot(v1, v2);

            Num invDenom = 1 / ((dot00 * dot11) - (dot01 * dot01));
            Num u = ((dot11 * dot02) - (dot01 * dot12)) * invDenom;
            Num v = ((dot00 * dot12) - (dot01 * dot02)) * invDenom;

            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }

        /// <summary>
        /// Creates a triangulated mesh from a polygon and returns both vertices and triangle indices.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>A tuple containing the vertex array and triangle indices.</returns>
        /// <remarks>
        /// This is a convenience method that returns both the original vertices and the triangulation indices.
        /// Useful when you need both the geometry data and connectivity information.
        /// </remarks>
        public static (Vect[] vertices, int[] triangles) CreateTriangulatedMesh(Vect[] vertices)
        {
            int[] triangles = Triangulate(vertices);
            return (vertices, triangles);
        }

        /// <summary>
        /// Triangulates a polygon with holes using the ear clipping algorithm with hole bridging.
        /// </summary>
        /// <param name="outerVertices">The vertices of the outer boundary polygon.</param>
        /// <param name="holes">An array of hole polygons, each represented as an array of vertices.</param>
        /// <returns>An array of triangle indices for the triangulated polygon with holes.</returns>
        /// <exception cref="ArgumentException">Thrown when the outer polygon has fewer than 3 vertices.</exception>
        /// <remarks>
        /// This method handles complex polygons with holes by first bridging holes to the outer boundary,
        /// then applying the ear clipping algorithm to the resulting simple polygon.
        /// All polygons should be in counter-clockwise order for outer boundaries and clockwise for holes.
        /// </remarks>
        public static int[] TriangulateWithHoles(Vect[] outerVertices, Vect[][] holes)
        {
            if (outerVertices == null || outerVertices.Length < 3)
            {
                throw new ArgumentException("Outer polygon must have at least 3 vertices");
            }

            if (holes == null || holes.Length == 0)
            {
                return Triangulate(outerVertices);
            }

            // Combine outer polygon with holes by creating bridges
            List<Vect> combinedVertices = new(outerVertices);
            
            foreach (var hole in holes)
            {
                if (hole.Length >= 3)
                {
                    // Find the rightmost point of the hole
                    int rightmostHoleIndex = 0;
                    for (int i = 1; i < hole.Length; i++)
                    {
                        if (hole[i].x > hole[rightmostHoleIndex].x)
                        {
                            rightmostHoleIndex = i;
                        }
                    }

                    // Find the closest visible vertex on the outer boundary
                    int bridgeIndex = FindClosestVisibleVertex(combinedVertices, hole[rightmostHoleIndex]);
                    
                    // Insert the hole vertices with bridge connections
                    List<Vect> holeVertices = new();
                    
                    // Add bridge vertex
                    holeVertices.Add(combinedVertices[bridgeIndex]);
                    
                    // Add hole vertices starting from rightmost point
                    for (int i = 0; i < hole.Length; i++)
                    {
                        int idx = (rightmostHoleIndex + i) % hole.Length;
                        holeVertices.Add(hole[idx]);
                    }
                    
                    // Close the bridge
                    holeVertices.Add(hole[rightmostHoleIndex]);
                    holeVertices.Add(combinedVertices[bridgeIndex]);
                    
                    // Insert into combined vertices
                    combinedVertices.InsertRange(bridgeIndex + 1, holeVertices);
                }
            }

            return Triangulate(combinedVertices.ToArray());
        }

        /// <summary>
        /// Validates if a polygon is simple (no self-intersections).
        /// </summary>
        /// <param name="vertices">The vertices of the polygon to validate.</param>
        /// <returns>True if the polygon is simple, false if it has self-intersections.</returns>
        /// <remarks>
        /// A simple polygon has no self-intersecting edges. This method checks all edge pairs
        /// to ensure no two non-adjacent edges intersect.
        /// </remarks>
        public static bool IsSimplePolygon(Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                return false;
            }

            int n = vertices.Length;
            
            for (int i = 0; i < n; i++)
            {
                Vect p1 = vertices[i];
                Vect q1 = vertices[(i + 1) % n];
                
                for (int j = i + 2; j < n; j++)
                {
                    // Skip adjacent edges and the closing edge
                    if (j == (i + n - 1) % n) continue;
                    
                    Vect p2 = vertices[j];
                    Vect q2 = vertices[(j + 1) % n];
                    
                    if (LineSegmentsIntersect(p1, q1, p2, q2))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        /// <summary>
        /// Calculates the signed area of a polygon using the shoelace formula.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The signed area (positive for counter-clockwise, negative for clockwise).</returns>
        /// <remarks>
        /// The shoelace formula efficiently calculates polygon area. The sign indicates winding direction:
        /// positive for counter-clockwise, negative for clockwise orientation.
        /// </remarks>
        public static Num SignedArea(Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                return 0;
            }

            Num area = 0;
            int n = vertices.Length;
            
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += vertices[i].x * vertices[j].y;
                area -= vertices[j].x * vertices[i].y;
            }
            
            return area * 0.5f;
        }

        /// <summary>
        /// Calculates the area of a polygon (always positive).
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The area of the polygon.</returns>
        public static Num Area(Vect[] vertices)
        {
            return Num.Abs(SignedArea(vertices));
        }

        /// <summary>
        /// Determines if a point is inside a polygon using the ray casting algorithm.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>True if the point is inside the polygon, false otherwise.</returns>
        /// <remarks>
        /// Uses the ray casting algorithm (point-in-polygon test) which counts intersections
        /// of a ray from the point to infinity with polygon edges. Odd count means inside.
        /// </remarks>
        public static bool IsPointInPolygon(Vect point, Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                return false;
            }

            bool inside = false;
            int n = vertices.Length;
            
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                Vect vi = vertices[i];
                Vect vj = vertices[j];
                
                if (((vi.y > point.y) != (vj.y > point.y)) &&
                    (point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x))
                {
                    inside = !inside;
                }
            }
            
            return inside;
        }

        /// <summary>
        /// Simplifies a polygon by removing vertices that don't significantly change the shape.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon to simplify.</param>
        /// <param name="tolerance">The maximum allowed deviation from the original shape.</param>
        /// <returns>A simplified polygon with fewer vertices.</returns>
        /// <remarks>
        /// Uses the Douglas-Peucker algorithm to reduce vertex count while preserving shape.
        /// Higher tolerance values result in more aggressive simplification.
        /// </remarks>
        public static Vect[] SimplifyPolygon(Vect[] vertices, Num tolerance = default)
        {
            if (tolerance == default)
            {
                tolerance = 0.1f;
            }
            
            if (vertices == null || vertices.Length <= 3)
            {
                return vertices ?? new Vect[0];
            }

            // Use Douglas-Peucker algorithm
            bool[] keep = new bool[vertices.Length];
            keep[0] = true;
            keep[vertices.Length - 1] = true;
            
            SimplifyRecursive(vertices, 0, vertices.Length - 1, tolerance, keep);
            
            List<Vect> simplified = new();
            for (int i = 0; i < vertices.Length; i++)
            {
                if (keep[i])
                {
                    simplified.Add(vertices[i]);
                }
            }
            
            return simplified.ToArray();
        }

        /// <summary>
        /// Decomposes a complex polygon into convex polygons.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon to decompose.</param>
        /// <returns>An array of convex polygons, each represented as an array of vertices.</returns>
        /// <remarks>
        /// Breaks down a concave polygon into multiple convex polygons using a recursive approach.
        /// This is useful for collision detection and physics simulations that work better with convex shapes.
        /// </remarks>
        public static Vect[][] DecomposeToConvex(Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                return new Vect[0][];
            }

            if (IsConvex(vertices))
            {
                return new Vect[][] { vertices };
            }

            List<Vect[]> result = new();
            DecomposeRecursive(vertices.ToList(), result);
            return result.ToArray();
        }

        /// <summary>
        /// Determines if a polygon is convex.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>True if the polygon is convex, false otherwise.</returns>
        /// <remarks>
        /// A convex polygon has all interior angles less than 180 degrees.
        /// This method checks the cross product sign consistency around the polygon.
        /// </remarks>
        public static bool IsConvex(Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                return false;
            }

            bool? isPositive = null;
            int n = vertices.Length;
            
            for (int i = 0; i < n; i++)
            {
                Vect p1 = vertices[i];
                Vect p2 = vertices[(i + 1) % n];
                Vect p3 = vertices[(i + 2) % n];
                
                Num cross = Cross2D(p2 - p1, p3 - p2);
                
                if (Math.Abs(cross) > 1e-6f) // Ignore nearly collinear points
                {
                    bool currentIsPositive = cross > 0;
                    
                    if (isPositive == null)
                    {
                        isPositive = currentIsPositive;
                    }
                    else if (isPositive != currentIsPositive)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        /// <summary>
        /// Calculates the convex hull of a set of points using Graham's scan algorithm.
        /// </summary>
        /// <param name="points">The input points.</param>
        /// <returns>The vertices of the convex hull in counter-clockwise order.</returns>
        /// <remarks>
        /// The convex hull is the smallest convex polygon that contains all input points.
        /// Uses Graham's scan algorithm for O(n log n) time complexity.
        /// </remarks>
        public static Vect[] ConvexHull(Vect[] points)
        {
            if (points == null || points.Length < 3)
            {
                return points ?? new Vect[0];
            }

            // Find the bottom-most point (and leftmost in case of tie)
            int bottomIndex = 0;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].y < points[bottomIndex].y ||
                    (points[i].y == points[bottomIndex].y && points[i].x < points[bottomIndex].x))
                {
                    bottomIndex = i;
                }
            }

            // Swap bottom point to first position
            (points[0], points[bottomIndex]) = (points[bottomIndex], points[0]);

            // Sort points by polar angle with respect to bottom point
            Vect pivot = points[0];
            Array.Sort(points, 1, points.Length - 1, new PolarAngleComparer(pivot));

            // Build convex hull using Graham's scan
            Stack<Vect> hull = new();
            hull.Push(points[0]);
            hull.Push(points[1]);

            for (int i = 2; i < points.Length; i++)
            {
                while (hull.Count > 1)
                {
                    Vect top = hull.Pop();
                    Vect second = hull.Peek();
                    hull.Push(top);

                    if (Cross2D(top - second, points[i] - top) > 0)
                    {
                        break;
                    }
                    hull.Pop();
                }
                hull.Push(points[i]);
            }

            return hull.Reverse().ToArray();
        }

        /// <summary>
        /// Finds the minimum bounding rectangle (oriented bounding box) of a polygon.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The four corners of the minimum bounding rectangle.</returns>
        /// <remarks>
        /// Calculates the oriented bounding box that minimizes area. This is more efficient
        /// than an axis-aligned bounding box for elongated shapes.
        /// </remarks>
        public static Vect[] MinimumBoundingRectangle(Vect[] vertices)
        {
            if (vertices == null || vertices.Length < 3)
            {
                return new Vect[0];
            }

            // First get the convex hull
            Vect[] hull = ConvexHull(vertices);
            
            if (hull.Length < 3)
            {
                return hull;
            }

            Num minArea = float.MaxValue;
            Vect[] bestRectangle = new Vect[4];

            // Try each edge of the convex hull as a potential side of the rectangle
            for (int i = 0; i < hull.Length; i++)
            {
                Vect edge = hull[(i + 1) % hull.Length] - hull[i];
                Num angle = Num.Atan2(edge.y, edge.x);

                // Rotate all points so this edge is horizontal
                Vect[] rotated = new Vect[hull.Length];
                Num cos = Num.Cos(-angle);
                Num sin = Num.Sin(-angle);
                
                for (int j = 0; j < hull.Length; j++)
                {
                    Vect p = hull[j];
                    rotated[j] = new Vect(
                        p.x * cos - p.y * sin,
                        p.x * sin + p.y * cos
                    );
                }

                // Find axis-aligned bounding box of rotated points
                Num minX = rotated.Min(p => p.x);
                Num maxX = rotated.Max(p => p.x);
                Num minY = rotated.Min(p => p.y);
                Num maxY = rotated.Max(p => p.y);

                Num area = (maxX - minX) * (maxY - minY);
                
                if (area < minArea)
                {
                    minArea = area;
                    
                    // Create rectangle corners and rotate back
                    Vect[] rect = {
                        new Vect(minX, minY),
                        new Vect(maxX, minY),
                        new Vect(maxX, maxY),
                        new Vect(minX, maxY)
                    };

                    cos = Num.Cos(angle);
                    sin = Num.Sin(angle);
                    
                    for (int j = 0; j < 4; j++)
                    {
                        Vect p = rect[j];
                        bestRectangle[j] = new Vect(
                            p.x * cos - p.y * sin,
                            p.x * sin + p.y * cos
                        );
                    }
                }
            }

            return bestRectangle;
        }

        #region Private Helper Methods

        /// <summary>
        /// Finds the closest visible vertex on the boundary for hole bridging.
        /// </summary>
        private static int FindClosestVisibleVertex(List<Vect> boundary, Vect holeVertex)
        {
            Num minDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < boundary.Count; i++)
            {
                Num distance = DistanceSqr(boundary[i], holeVertex);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        /// <summary>
        /// Checks if two line segments intersect.
        /// </summary>
        private static bool LineSegmentsIntersect(Vect p1, Vect q1, Vect p2, Vect q2)
        {
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special cases (collinear points)
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false;
        }

        /// <summary>
        /// Calculates the orientation of three points.
        /// </summary>
        private static int Orientation(Vect p, Vect q, Vect r)
        {
            Num val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
            if (Math.Abs(val) < 1e-6f) return 0; // Collinear
            return (val > 0) ? 1 : 2; // Clockwise or Counterclockwise
        }

        /// <summary>
        /// Checks if point q lies on segment pr.
        /// </summary>
        private static bool OnSegment(Vect p, Vect q, Vect r)
        {
            return q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                   q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y);
        }

        /// <summary>
        /// Recursive helper for Douglas-Peucker simplification.
        /// </summary>
        private static void SimplifyRecursive(Vect[] vertices, int start, int end, Num tolerance, bool[] keep)
        {
            if (end <= start + 1) return;

            Num maxDistance = 0;
            int maxIndex = start;

            // Find the point with maximum distance from line segment
            for (int i = start + 1; i < end; i++)
            {
                Num distance = PointToLineDistance(vertices[i], vertices[start], vertices[end]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }

            // If max distance is greater than tolerance, recursively simplify
            if (maxDistance > tolerance)
            {
                keep[maxIndex] = true;
                SimplifyRecursive(vertices, start, maxIndex, tolerance, keep);
                SimplifyRecursive(vertices, maxIndex, end, tolerance, keep);
            }
        }

        /// <summary>
        /// Calculates the distance from a point to a line segment.
        /// </summary>
        private static Num PointToLineDistance(Vect point, Vect lineStart, Vect lineEnd)
        {
            Vect line = lineEnd - lineStart;
            Num lineLengthSq = line.SqrMagnitude;
            
            if (lineLengthSq < 1e-6f)
            {
                return Distance(point, lineStart);
            }

            Num t = Num.Clamp(Dot(point - lineStart, line) / lineLengthSq, 0, 1);
            Vect projection = lineStart + (line * t);
            return Distance(point, projection);
        }

        /// <summary>
        /// Recursive helper for convex decomposition.
        /// </summary>
        private static void DecomposeRecursive(List<Vect> vertices, List<Vect[]> result)
        {
            if (vertices.Count < 3) return;

            if (IsConvex(vertices.ToArray()))
            {
                result.Add(vertices.ToArray());
                return;
            }

            // Find a reflex vertex and create a cut
            for (int i = 0; i < vertices.Count; i++)
            {
                int prev = (i - 1 + vertices.Count) % vertices.Count;
                int next = (i + 1) % vertices.Count;

                if (Cross2D(vertices[next] - vertices[i], vertices[prev] - vertices[i]) < 0)
                {
                    // Found reflex vertex, try to make a cut
                    for (int j = 0; j < vertices.Count; j++)
                    {
                        if (j == prev || j == i || j == next) continue;

                        // Check if we can make a valid diagonal
                        if (IsValidDiagonal(vertices, i, j))
                        {
                            // Split polygon along diagonal
                            List<Vect> poly1 = new();
                            List<Vect> poly2 = new();

                            int start = Math.Min(i, j);
                            int end = Math.Max(i, j);

                            for (int k = start; k <= end; k++)
                            {
                                poly1.Add(vertices[k]);
                            }

                            for (int k = end; k != start; k = (k + 1) % vertices.Count)
                            {
                                poly2.Add(vertices[k]);
                            }
                            poly2.Add(vertices[start]);

                            DecomposeRecursive(poly1, result);
                            DecomposeRecursive(poly2, result);
                            return;
                        }
                    }
                }
            }

            // Fallback: add as is if no valid cut found
            result.Add(vertices.ToArray());
        }

        /// <summary>
        /// Checks if a diagonal between two vertices is valid for polygon decomposition.
        /// </summary>
        private static bool IsValidDiagonal(List<Vect> vertices, int i, int j)
        {
            // Check if diagonal is inside polygon and doesn't intersect edges
            Vect p1 = vertices[i];
            Vect p2 = vertices[j];

            // Check if diagonal intersects any edge
            for (int k = 0; k < vertices.Count; k++)
            {
                int next = (k + 1) % vertices.Count;
                if (k == i || k == j || next == i || next == j) continue;

                if (LineSegmentsIntersect(p1, p2, vertices[k], vertices[next]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Comparer for sorting points by polar angle relative to a pivot point.
        /// </summary>
        private class PolarAngleComparer : IComparer<Vect>
        {
            private readonly Vect pivot;

            public PolarAngleComparer(Vect pivot)
            {
                this.pivot = pivot;
            }

            public int Compare(Vect a, Vect b)
            {
                Num cross = Cross2D(a - pivot, b - pivot);
                if (Math.Abs(cross) < 1e-6f)
                {
                    // Collinear points - sort by distance
                    return DistanceSqr(pivot, a).CompareTo(DistanceSqr(pivot, b));
                }
                return cross > 0 ? -1 : 1;
            }
        }

        #endregion

        #endregion

        #region Math Methods

        /// <summary>
        /// Raises each component of this vector to the specified power.
        /// </summary>
        /// <param name="power">The exponent to apply to each component.</param>
        /// <returns>A vector with each component raised to the specified power.</returns>
        public readonly Vect Pow(Num power)
        {
            return new(Math.Pow(x, power), Math.Pow(y, power), Math.Pow(z, power), Math.Pow(w, power));
        }

        /// <summary>
        /// Calculates the square root of each component of a vector.
        /// </summary>
        /// <param name="v">The vector to apply square root to.</param>
        /// <returns>A vector with the square root of each component.</returns>
        public static Vect Sqrt(Vect v)
        {
            return new(Math.Sqrt(v.x), Math.Sqrt(v.y), Math.Sqrt(v.z), Math.Sqrt(v.w));
        }

        /// <summary>
        /// Returns a normalized version of the specified vector.
        /// If the vector has zero magnitude, returns the original vector to avoid division by zero.
        /// </summary>
        /// <param name="vect">The vector to normalize.</param>
        /// <returns>A unit vector in the same direction, or the original vector if magnitude is zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect Normalize(Vect vect)
        {
            Num mag = vect.Magnitude;
            return mag == 0 ? vect : vect / mag;
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        /// <remarks>
        /// The dot product measures the similarity between two vectors.
        /// Returns positive for vectors pointing in similar directions, negative for opposite directions, and zero for perpendicular vectors.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Num Dot(Vect a, Vect b) { return (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w); }

        /// <summary>
        /// Calculates the cross product of two 3D vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector perpendicular to both input vectors, following the right-hand rule.</returns>
        /// <remarks>
        /// The cross product creates a vector perpendicular to both input vectors.
        /// The magnitude equals the area of the parallelogram formed by the two vectors.
        /// Only uses the x, y, and z components; w component is set to 0.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect Cross(Vect a, Vect b)
        {
            return new Vect((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x));
        }

        #endregion

        #region Manipulation/Transformation Methods

        /// <summary>
        /// Performs component-wise multiplication of two vectors (Hadamard product).
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector with components multiplied component-wise.</returns>
        public static Vect Scale(Vect a, Vect b)
        {
            return new Vect(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        /// <summary>
        /// Scales this vector by another vector component-wise.
        /// </summary>
        /// <param name="scale">The scaling vector.</param>
        /// <returns>A vector with components scaled by the corresponding components of the scale vector.</returns>
        public readonly Vect Scale(Vect scale)
        {
            return new Vect(x * scale.x, y * scale.y, z * scale.z, w * scale.w);
        }

        /// <summary>
        /// Projects a vector onto another vector (onto a normal).
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="onNormal">The vector to project onto.</param>
        /// <returns>The projection of vector onto onNormal.</returns>
        /// <remarks>
        /// Vector projection finds the component of one vector in the direction of another.
        /// If onNormal has very small magnitude, returns zero vector to avoid division by zero.
        /// </remarks>
        public static Vect Project(Vect vector, Vect onNormal)
        {
            Num sqrMag = onNormal.SqrMagnitude;
            if (sqrMag < 1e-15f)
            {
                return Zero;
            }

            Num dot = Dot(vector, onNormal);
            return onNormal * dot / sqrMag;
        }

        /// <summary>
        /// Reflects a vector off a surface with the given normal.
        /// </summary>
        /// <param name="inDirection">The incoming direction vector.</param>
        /// <param name="inNormal">The normal vector of the reflecting surface.</param>
        /// <returns>The reflected direction vector.</returns>
        /// <remarks>
        /// Calculates reflection as if the incoming direction were a light ray bouncing off a surface.
        /// The normal vector should be normalized for accurate results.
        /// </remarks>
        public static Vect Reflect(Vect inDirection, Vect inNormal)
        {
            Num factor = -2 * Dot(inNormal, inDirection);
            return inDirection + (factor * inNormal);
        }

        /// <summary>
        /// Calculates the inverse linear interpolation parameter for a value between two vectors.
        /// </summary>
        /// <param name="a">The start vector.</param>
        /// <param name="b">The end vector.</param>
        /// <param name="value">The value to find the interpolation parameter for.</param>
        /// <returns>A vector containing the interpolation parameters for each component.</returns>
        /// <remarks>
        /// Given vectors a and b and a value, finds t such that Lerp(a, b, t) = value.
        /// If a == b, returns zero vector to avoid division by zero.
        /// </remarks>
        public static Vect InverseLerp(Vect a, Vect b, Vect value)
        {
            return a != b
                ? new Vect(
                    (value.x - a.x) / (b.x - a.x),
                    (value.y - a.y) / (b.y - a.y),
                    (value.z - a.z) / (b.z - a.z),
                    (value.w - a.w) / (b.w - a.w)
                )
                : Zero;
        }

        /// <summary>
        /// Clamps the magnitude of a vector to a maximum length.
        /// </summary>
        /// <param name="vector">The vector to clamp.</param>
        /// <param name="maxLength">The maximum allowed magnitude.</param>
        /// <returns>A vector with magnitude clamped to maxLength, preserving direction.</returns>
        /// <remarks>
        /// If the vector's magnitude is less than or equal to maxLength, returns the original vector.
        /// Otherwise, returns a vector in the same direction with magnitude equal to maxLength.
        /// </remarks>
        public static Vect ClampMagnitude(Vect vector, Num maxLength)
        {
            Num sqrMagnitude = vector.SqrMagnitude;
            if (sqrMagnitude > maxLength * maxLength)
            {
                Num mag = Num.Sqrt(sqrMagnitude);
                Num normalizedX = vector.x / mag;
                Num normalizedY = vector.y / mag;
                Num normalizedZ = vector.z / mag;
                Num normalizedW = vector.w / mag;
                return new Vect(
                    normalizedX * maxLength,
                    normalizedY * maxLength,
                    normalizedZ * maxLength,
                    normalizedW * maxLength);
            }
            return vector;
        }

        /// <summary>
        /// Alias for ClampMagnitude. Clamps the length of a vector to a maximum value.
        /// </summary>
        /// <param name="vector">The vector to clamp.</param>
        /// <param name="maxLength">The maximum allowed length.</param>
        /// <returns>A vector with length clamped to maxLength, preserving direction.</returns>
        public static Vect ClampLength(Vect vector, Num maxLength)
        {
            return ClampMagnitude(vector, maxLength);
        }

        /// <summary>
        /// Sets the magnitude of this vector to a specific value.
        /// </summary>
        /// <param name="newMagnitude">The desired magnitude.</param>
        /// <returns>A vector in the same direction with the specified magnitude.</returns>
        /// <remarks>
        /// If this vector has zero magnitude, the result may be unpredictable.
        /// Consider checking IsNormalized or Magnitude before calling this method.
        /// </remarks>
        public Vect SetMagnitude(Num newMagnitude)
        {
            return Normalized * newMagnitude;
        }

        /// <summary>
        /// Alias for SetMagnitude. Sets the length of this vector to a specific value.
        /// </summary>
        /// <param name="newLength">The desired length.</param>
        /// <returns>A vector in the same direction with the specified length.</returns>
        public Vect SetLength(Num newLength)
        {
            return SetMagnitude(newLength);
        }

        /// <summary>
        /// Returns a vector with the specified magnitude in the same direction as this vector.
        /// </summary>
        /// <param name="magnitude">The desired magnitude.</param>
        /// <returns>A vector with the specified magnitude, or this vector if magnitude is zero.</returns>
        /// <remarks>
        /// This method is more robust than SetMagnitude as it handles zero-magnitude vectors gracefully.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vect WithMagnitude(Num magnitude)
        {
            Num currentMag = Magnitude;
            if (currentMag == 0)
            { return this; }

            Num scale = magnitude / currentMag;
            return this * scale;
        }

        /// <summary>
        /// Alias for WithMagnitude. Returns a vector with the specified length in the same direction.
        /// </summary>
        /// <param name="length">The desired length.</param>
        /// <returns>A vector with the specified length, or this vector if magnitude is zero.</returns>
        public readonly Vect WithLength(Num length)
        {
            return WithMagnitude(length);
        }

        #endregion

        #region Angle/Rotation

        /// <summary>
        /// Gets the angle of this vector in degrees.
        /// </summary>
        /// <returns>The angle in degrees from the positive X-axis.</returns>
        /// <remarks>
        /// The angle is measured counter-clockwise from the positive X-axis.
        /// Range: -180 to 180 degrees.
        /// </remarks>
        public readonly Num AngleDeg()
        {
            return Num.Atan2(y, x) * 180 / Num.PI;
        }

        /// <summary>
        /// Gets the angle of this vector in radians.
        /// </summary>
        /// <returns>The angle in radians from the positive X-axis.</returns>
        /// <remarks>
        /// The angle is measured counter-clockwise from the positive X-axis.
        /// Range: -π to π radians.
        /// </remarks>
        public readonly Num AngleRad()
        {
            return Num.Atan2(y, x);
        }

        /// <summary>
        /// Calculates the unsigned angle between two vectors in radians.
        /// </summary>
        /// <param name="from">The vector to measure from.</param>
        /// <param name="to">The vector to measure to.</param>
        /// <returns>The angle between the vectors in radians (0 to π).</returns>
        /// <remarks>
        /// Always returns a positive angle. For signed angles in 2D, use SignedAngle instead.
        /// </remarks>
        public static Num Angle(Vect from, Vect to)
        {
            Num denominator = Num.Sqrt(from.SqrMagnitude * to.SqrMagnitude);
            if (denominator < 1e-15f)
            {
                return 0;
            }

            Num dot = Num.Clamp(Dot(from, to) / denominator, -1, 1);
            return Num.Acos(dot);
        }

        /// <summary>
        /// Calculates the signed angle between two vectors in 2D space.
        /// </summary>
        /// <param name="from">The vector to measure from.</param>
        /// <param name="to">The vector to measure to.</param>
        /// <returns>The signed angle in radians (positive for counter-clockwise, negative for clockwise).</returns>
        /// <remarks>
        /// Uses the 2D cross product to determine the sign of the angle.
        /// Positive values indicate counter-clockwise rotation, negative values indicate clockwise rotation.
        /// </remarks>
        public static Num SignedAngle(Vect from, Vect to)
        {
            Num unsigned = Angle(from, to);
            Num sign = Num.Sign((from.x * to.y) - (from.y * to.x));
            return unsigned * sign;
        }

        /// <summary>
        /// Rotates a vector by the specified angle in radians.
        /// </summary>
        /// <param name="v">The vector to rotate.</param>
        /// <param name="angle">The rotation angle in radians.</param>
        /// <returns>The rotated vector.</returns>
        /// <remarks>
        /// Performs 2D rotation around the origin. Only affects x and y components.
        /// Positive angles rotate counter-clockwise, negative angles rotate clockwise.
        /// </remarks>
        public static Vect Rotate(Vect v, Num angle)
        {
            Num cos = Num.Cos(angle);
            Num sin = Num.Sin(angle);
            return new Vect((v.x * cos) - (v.y * sin), (v.x * sin) + (v.y * cos), v.z, v.w);
        }

        #endregion

        #region Min/Max

        /// <summary>
        /// Gets the maximum component value of this vector.
        /// </summary>
        /// <returns>The largest component value among x, y, z, and w.</returns>
        public readonly Num Max()
        {
            return Num.Max(Num.Max(Num.Max(x, y), z), w);
        }

        /// <summary>
        /// Gets the minimum component value of this vector.
        /// </summary>
        /// <returns>The smallest component value among x, y, z, and w.</returns>
        public readonly Num Min()
        {
            return Num.Min(Num.Min(Num.Min(x, y), z), w);
        }

        /// <summary>
        /// Returns a vector containing the minimum components from two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector with the minimum x, y, z, and w components from both input vectors.</returns>
        private static Vect Min(Vect a, Vect b)
        {
            return new Vect(Num.Min(a.x, b.x), Num.Min(a.y, b.y), Num.Min(a.z, b.z), Num.Min(a.w, b.w));
        }

        /// <summary>
        /// Returns a vector containing the maximum components from two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector with the maximum x, y, z, and w components from both input vectors.</returns>
        private static Vect Max(Vect a, Vect b)
        {
            return new Vect(Num.Max(a.x, b.x), Num.Max(a.y, b.y), Num.Max(a.z, b.z), Num.Max(a.w, b.w));
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serializes this vector to a binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to serialize to.</param>
        /// <remarks>
        /// Writes the x, y, z, and w components in order to the binary stream.
        /// Use this for efficient binary serialization of vector data.
        /// </remarks>
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
            writer.Write(w);
        }

        #endregion
    }
}