using OX.Numerics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace OX.Geometry
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class Collision // TODO: add more utility functions. :)
    {
        /*
            * The Separating Axis Theorem (SAT) is a method used to determine if two convex shapes are colliding.
            * The theorem states that if there exists an axis along which the projections of the two shapes do not overlap, the shapes are not colliding.
            * If all axes overlap, the shapes are colliding.
            * The Minimum Translation Vector (MTV) is the smallest vector required to separate the two shapes.

            * The SAT is used in the 'CheckSat' method to determine if two shapes are colliding.
            * The 'GetMtv' method is used to get the MTV of two shapes.

            * All collision detection code is free use and can be modified to suit your needs, credits are not required. NO HELP WILL BE PROVIDED.
        */
        public class Mtv // Minimum Translation Vector
            (Vect axis, float overlap)
        {
            public Vect Axis { get; set; } = axis;
            public float Depth { get; set; } = overlap;
        }

        public static Mtv? GetSATMtv(Vect[] verts1, Vect[] verts2) // Separating Axis Theorem, returns the MTV (Minimum Translation Vector)
        { // NOTE: The vertices passed through the method should be in world space, not local space. Transform the vertices before passing them through the method.
          // More information on the Separating Axis Theorem found in the Method 'CheckSat'.


            Vect direction = Vect.Centroid(verts2) - Vect.Centroid(verts1); // Get the direction vector between the two shapes

            Vect[] axes = [.. Vect.GetAxes(verts1), .. Vect.GetAxes(verts2)];

            float minOverlap = float.MaxValue; // Set the min overlap to the highest possible value to ensure it is overwritten.
            Vect minAxis = Vect.Zero; // Set the min axis to the zero vector to ensure it is overwritten.

            foreach (Vect axis in axes)
            {
                (float min1, float max1) = Vect.Project(verts1, axis);
                (float min2, float max2) = Vect.Project(verts2, axis);

                if (min1 > max2 || min2 > max1)
                {
                    return null;
                }

                float overlap = Math.Min(max1 - min2, max2 - min1); // Get the overlap of the projections
                if (overlap < minOverlap) // If the overlap is less than the current min overlap, set the min overlap and axis to the current values
                {
                    minOverlap = overlap;
                    minAxis = axis;
                    if (Vect.Dot(direction, axis) < 0) // If the direction is opposite to the axis, negate the axis. ensures the axis points towards the other shape.
                    {
                        minAxis = -axis;
                    }
                }
            }

            //DebugSAT(verts1, verts2, GetCenter(verts1), GetCenter(verts2)); // Uncomment to debug the SAT method

            return new Mtv(minAxis, minOverlap); // Return the MTV (Minimum Translation Vector)
        }

        public static bool CheckSAT(Vect[] vertsA, Vect[] vertsB) // Separating Axis Theorem, does not return the MTV (Minimum Translation Vector)
        { // NOTE: The vertices passed through the method should be in world space, not local space. Transform the vertices before passing them through the method.
            /*
                * The Separating Axis Theorem (SAT) is a method used to determine if two convex shapes are colliding.
                * The theorem states that if there exists an axis along which the projections of the two shapes do not overlap, the shapes are not colliding.
                * If all axes overlap, the shapes are colliding.
                * The Minimum Translation Vector (MTV) is the smallest vector required to separate the two shapes.
            */

            // Get all axes to test
            Vect[] axes = [.. Vect.GetAxes(vertsA), .. Vect.GetAxes(vertsB)];

            foreach (Vect axis in axes)
            {
                (float min1, float max1) = Vect.Project(vertsA, axis); // Project the vertices of the shapes onto the axis
                (float min2, float max2) = Vect.Project(vertsA, axis);

                if (min1 > max2 || min2 > max1) // If the projections do not overlap, the shapes are not colliding
                {
                    return false;
                }
            }

            return true; // Return true if all axes overlap
        }

        public struct CcdResult
        {
            public bool HasCollision { get; set; }
            public float TimeOfImpact { get; set; }
            public Vect CollisionPoint { get; set; }
            public Vect CollisionNormal { get; set; }
        }

        private struct SimplexVertex
        {
            public Vect Point;
            public Vect Support1;
            public Vect Support2;

            public SimplexVertex(Vect point, Vect s1, Vect s2)
            {
                Point = point;
                Support1 = s1;
                Support2 = s2;
            }
        }

        // Minkowski-based continuous collision detection
        public static CcdResult CheckMinkowski(Vect[] verts1, Vect[] verts2, Vect vel1, Vect vel2, float deltaTime)
        {
            Vect relativeVel = vel1 - vel2;
            if (relativeVel.SqrMagnitude < Num.Epsilon)
            {
                return new CcdResult { HasCollision = false };
            }

            // Calculate Minkowski difference for swept shapes
            Vect[] minkowskiDiff = GetMinkowskiDifference(verts1, verts2);

            // Ray from origin through relative velocity
            Vect ray = relativeVel.Normalized;
            float maxDist = relativeVel.Magnitude * deltaTime;

            // Find closest point on Minkowski difference to ray
            (Vect closestPoint, float t) = MinkowskiDiffToRay(minkowskiDiff, ray);

            return t >= 0 && t <= maxDist && closestPoint.Magnitude <= Num.Epsilon
                ? new CcdResult
                {
                    HasCollision = true,
                    TimeOfImpact = t / relativeVel.Magnitude,
                    CollisionPoint = verts1[0] + (vel1 * (t / relativeVel.Magnitude)),
                    CollisionNormal = closestPoint.Normalized
                }
                : new CcdResult { HasCollision = false };
        }

        // GJK-based continuous collision detection
        public static CcdResult CheckGjk(Vect[] verts1, Vect[] verts2, Vect vel1, Vect vel2, float deltaTime)
        {
            Vect relativeVel = vel1 - vel2;
            if (relativeVel.SqrMagnitude < Num.Epsilon)
            {
                return new CcdResult { HasCollision = false };
            }

            // Initial search direction
            Vect direction = relativeVel.Normalized;

            // Initialize simplex
            List<SimplexVertex> simplex = new();
            Vect firstPoint = Support(verts1, verts2, direction) - Support(verts1, verts2, -direction);
            simplex.Add(new SimplexVertex(firstPoint, Support(verts1, verts2, direction), Support(verts1, verts2, -direction)));

            direction = -firstPoint;

            // GJK iteration limit
            const int MaxIterations = 20;
            int iterations = 0;

            while (iterations++ < MaxIterations)
            {
                Vect support = Support(verts1, verts2, direction);

                // Check if we've passed the origin
                if (Vect.Dot(support, direction) < 0)
                {
                    return new CcdResult { HasCollision = false };
                }

                simplex.Add(new SimplexVertex(firstPoint, Support(verts1, verts2, direction), Support(verts1, verts2, -direction)));

                if (UpdateSimplexAndDirection(simplex, ref direction))
                {
                    // Found collision, calculate time of impact
                    float toi = CalculateTimeOfImpact(simplex, relativeVel, deltaTime);
                    if (toi >= 0 && toi <= deltaTime)
                    {
                        return new CcdResult
                        {
                            HasCollision = true,
                            TimeOfImpact = toi,
                            CollisionPoint = CalculateCollisionPoint(simplex, toi, vel1, vel2),
                            CollisionNormal = CalculateCollisionNormal(simplex)
                        };
                    }
                }
            }

            return new CcdResult { HasCollision = false };
        }

        // Raycast-based continuous collision detection
        public static CcdResult CheckRaycast(Vect[] verts1, Vect[] verts2, Vect vel1, Vect vel2, float deltaTime)
        {
            Vect relativeVel = vel1 - vel2;
            if (relativeVel.SqrMagnitude < Num.Epsilon)
            {
                return new CcdResult { HasCollision = false };
            }

            // Convert vertices to edges for raycast
            (Vect start, Vect end, Vect normal)[] edges1 = GetEdges(verts1);
            (Vect start, Vect end, Vect normal)[] edges2 = GetEdges(verts2);

            float earliestTime = float.MaxValue;
            Vect collisionNormal = Vect.Zero;
            Vect collisionPoint = Vect.Zero;

            // Check edges of shape1 against vertices of shape2
            foreach (Vect vertex in verts2)
            {
                foreach ((Vect start, Vect end, Vect normal) in edges1)
                {
                    (bool hasIntersection, float t) = RayEdgeIntersection(vertex, -relativeVel, start, end, deltaTime);
                    if (hasIntersection && t < earliestTime)
                    {
                        earliestTime = t;
                        collisionNormal = normal;
                        collisionPoint = vertex + (-relativeVel * t);
                    }
                }
            }

            // Check edges of shape2 against vertices of shape1
            foreach (Vect vertex in verts1)
            {
                foreach ((Vect start, Vect end, Vect normal) in edges2)
                {
                    (bool hasIntersection, float t) = RayEdgeIntersection(vertex, relativeVel, start, end, deltaTime);
                    if (hasIntersection && t < earliestTime)
                    {
                        earliestTime = t;
                        collisionNormal = -normal;
                        collisionPoint = vertex + (relativeVel * t);
                    }
                }
            }

            return earliestTime != float.MaxValue
                ? new CcdResult
                {
                    HasCollision = true,
                    TimeOfImpact = earliestTime,
                    CollisionPoint = collisionPoint,
                    CollisionNormal = collisionNormal
                }
                : new CcdResult { HasCollision = false };
        }

        // Helper methods
        public static Vect[] GetMinkowskiDifference(Vect[] verts1, Vect[] verts2)
        {
            List<Vect> result = new();
            foreach (Vect v1 in verts1)
            {
                foreach (Vect v2 in verts2)
                {
                    result.Add(v1 - v2);
                }
            }
            return [.. result];
        }

        public static (Vect point, float t) MinkowskiDiffToRay(Vect[] minkowskiDiff, Vect rayDir)
        {
            float minDist = float.MaxValue;
            Vect closestPoint = Vect.Zero;
            float closestT = 0;

            foreach (Vect point in minkowskiDiff)
            {
                float t = Vect.Dot(point, rayDir);
                Vect projection = rayDir * t;
                float dist = (point - projection).SqrMagnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    closestPoint = point;
                    closestT = t;
                }
            }

            return (closestPoint, closestT);
        }

        public static Vect Support(Vect[] verts1, Vect[] verts2, Vect direction)
        {
            return Vect.FurthestPoint(verts1, direction) - Vect.FurthestPoint(verts2, -direction);
        }

        private static bool UpdateSimplexAndDirection(List<SimplexVertex> simplex, ref Vect direction)
        {
            return simplex.Count == 2 ? DoLine(simplex, ref direction) : DoTriangle(simplex, ref direction);
        }

        private static bool DoLine(List<SimplexVertex> simplex, ref Vect direction)
        {
            Vect b = simplex[1].Point;
            Vect a = simplex[0].Point;
            Vect ab = b - a;
            Vect ao = -a;

            if (SameDirection(ab, ao))
            {
                direction = ab.Perpendicular.Normalized;
                return false;
            }

            simplex.RemoveAt(0);
            direction = ao;
            return false;
        }

        private static bool DoTriangle(List<SimplexVertex> simplex, ref Vect direction)
        {
            Vect c = simplex[2].Point;
            Vect b = simplex[1].Point;
            Vect a = simplex[0].Point;

            Vect ab = b - a;
            Vect ac = c - a;
            Vect ao = -a;

            Vect abc = ab.Perpendicular;

            if (SameDirection(abc, ao))
            {
                if (SameDirection(ab, ao))
                {
                    simplex.RemoveAt(2);
                    direction = ab.Perpendicular;
                    return false;
                }

                simplex.RemoveAt(1);
                direction = ac.Perpendicular;
                return false;
            }

            Vect acb = -abc;
            if (SameDirection(acb, ao))
            {
                if (SameDirection(ac, ao))
                {
                    simplex.RemoveAt(1);
                    direction = ac.Perpendicular;
                    return false;
                }

                simplex.RemoveAt(2);
                direction = ab.Perpendicular;
                return false;
            }

            return true;
        }

        private static bool SameDirection(Vect direction, Vect ao)
        {
            return Vect.Dot(direction, ao) > 0;
        }

        private static float CalculateTimeOfImpact(List<SimplexVertex> simplex, Vect relativeVel, float deltaTime)
        {
            Vect closestPoint = CalculateClosestPoint(simplex);
            return closestPoint.Magnitude / relativeVel.Magnitude;
        }

        private static Vect CalculateClosestPoint(List<SimplexVertex> simplex)
        {
            if (simplex.Count == 1)
            {
                return simplex[0].Point;
            }

            if (simplex.Count == 2)
            {
                Vect a = simplex[0].Point;
                Vect b = simplex[1].Point;
                Vect ab = b - a;
                Num t = -Vect.Dot(a, ab) / ab.SqrMagnitude;
                t = Num.Clamp(t, 0, 1);
                return a + (ab * t);
            }

            return Vect.Zero; // Origin is inside triangle
        }

        private static Vect CalculateCollisionPoint(List<SimplexVertex> simplex, float toi, Vect vel1, Vect vel2)
        {
            Vect point1 = simplex[0].Support1 + (vel1 * toi);
            Vect point2 = simplex[0].Support2 + (vel2 * toi);
            return (point1 + point2) * 0.5f;
        }

        private static Vect CalculateCollisionNormal(List<SimplexVertex> simplex)
        {
            Vect closestPoint = CalculateClosestPoint(simplex);
            return closestPoint.Normalized;
        }

        private static (Vect start, Vect end, Vect normal)[] GetEdges(Vect[] vertices) // TODO: Move to Vect.cs
        {
            (Vect start, Vect end, Vect normal)[] edges = new (Vect start, Vect end, Vect normal)[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vect start = vertices[i];
                Vect end = vertices[(i + 1) % vertices.Length];
                Vect edge = end - start;
                Vect normal = edge.Perpendicular.Normalized;
                edges[i] = (start, end, normal);
            }
            return edges;
        }

        public static (bool hasIntersection, float t) RayEdgeIntersection(
            Vect rayOrigin, Vect rayDir, Vect edgeStart, Vect edgeEnd, float maxTime)
        {
            Vect edge = edgeEnd - edgeStart;
            Vect normal = edge.Perpendicular.Normalized;

            float denom = Vect.Dot(rayDir, normal);
            if (Math.Abs(denom) < Num.Epsilon)
            {
                return (false, 0);
            }

            float t = Vect.Dot(edgeStart - rayOrigin, normal) / denom;
            if (t < 0 || t > maxTime)
            {
                return (false, 0);
            }

            Vect intersection = rayOrigin + (rayDir * t);
            Num edgeLength = edge.Magnitude;
            Vect toIntersection = intersection - edgeStart;
            float along = Vect.Dot(toIntersection, edge) / edgeLength;

            return along >= 0 && along <= edgeLength ? ((bool hasIntersection, float t))(true, t) : ((bool hasIntersection, float t))(false, 0);
        }

        public static void DebugCollision(Mtv collision)
        {
            System.Diagnostics.Debug.WriteLine("\n=== COLLISION DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"Time: {DateTime.Now:HH:mm:ss.fff}");
            System.Diagnostics.Debug.WriteLine("\nCollision Details:");
            System.Diagnostics.Debug.WriteLine($"  Normal: {collision.Axis}");
            System.Diagnostics.Debug.WriteLine($"  Depth: {collision.Depth}");

            System.Diagnostics.Debug.WriteLine("===================\n");
        }

        public static void DebugSAT(Vect[] verts1, Vect[] verts2, Vect position1, Vect position2)
        {
            System.Diagnostics.Debug.WriteLine("\n=== SAT DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"Time: {DateTime.Now:HH:mm:ss.fff}");

            // Object positions
            System.Diagnostics.Debug.WriteLine("\nPositions:");
            System.Diagnostics.Debug.WriteLine($"  Object 1: {position1}");
            System.Diagnostics.Debug.WriteLine($"  Object 2: {position2}");

            // Vertices
            System.Diagnostics.Debug.WriteLine("\nVertices:");
            System.Diagnostics.Debug.WriteLine("  Object 1:");
            foreach (Vect v in verts1)
            {
                System.Diagnostics.Debug.WriteLine($"    {v}");
            }

            System.Diagnostics.Debug.WriteLine("  Object 2:");
            foreach (Vect v in verts2)
            {
                System.Diagnostics.Debug.WriteLine($"    {v}");
            }

            // Axes and Projections
            System.Diagnostics.Debug.WriteLine("\nAxes and Projections:");
            Vect[] axes = [.. Vect.GetAxes(verts1), .. Vect.GetAxes(verts2)];
            foreach (Vect axis in axes)
            {
                if (axis.SqrMagnitude == 0)
                {
                    continue;
                }

                Vect normalizedAxis = axis.Normalized;
                (float min1, float max1) = Vect.Project(verts1, normalizedAxis);
                (float min2, float max2) = Vect.Project(verts2, normalizedAxis);

                System.Diagnostics.Debug.WriteLine($"  Axis: {normalizedAxis}");
                System.Diagnostics.Debug.WriteLine($"    Shape1 Projection: [{min1}, {max1}]");
                System.Diagnostics.Debug.WriteLine($"    Shape2 Projection: [{min2}, {max2}]");
                System.Diagnostics.Debug.WriteLine($"    Overlap: {(min1 > max2 || min2 > max1 ? "None" : Math.Min(max1 - min2, max2 - min1))}");
            }

            System.Diagnostics.Debug.WriteLine("===================\n");
        }

        public static bool CheckAABB(Vect[] verts1, Vect[] verts2)
        {
            (Vect min1, Vect max1) = GetAABBBounds(verts1);
            (Vect min2, Vect max2) = GetAABBBounds(verts2);

            return !(max1.x < min2.x || min1.x > max2.x ||
                    max1.y < min2.y || min1.y > max2.y ||
                    max1.z < min2.z || min1.z > max2.z);
        }

        public static Mtv? GetAABBMtv(Vect[] verts1, Vect[] verts2)
        {
            (Vect min1, Vect max1) = GetAABBBounds(verts1);
            (Vect min2, Vect max2) = GetAABBBounds(verts2);

            if (!CheckAABB(verts1, verts2))
            {
                return null;
            }

            // Calculate overlap on each axis
            float xOverlap = Math.Min(max1.x - min2.x, max2.x - min1.x);
            float yOverlap = Math.Min(max1.y - min2.y, max2.y - min1.y);
            float zOverlap = Math.Min(max1.z - min2.z, max2.z - min1.z);

            // Find minimum overlap and corresponding axis
            Vect axis;
            float depth;

            if (xOverlap <= yOverlap && xOverlap <= zOverlap)
            {
                depth = xOverlap;
                axis = new Vect(1, 0, 0);
                if (max1.x + min1.x < max2.x + min2.x)
                {
                    axis = -axis;
                }
            }
            else if (yOverlap <= zOverlap)
            {
                depth = yOverlap;
                axis = new Vect(0, 1, 0);
                if (max1.y + min1.y < max2.y + min2.y)
                {
                    axis = -axis;
                }
            }
            else
            {
                depth = zOverlap;
                axis = new Vect(0, 0, 1);
                if (max1.z + min1.z < max2.z + min2.z)
                {
                    axis = -axis;
                }
            }

            return new Mtv(axis, depth);
        }

        private static (Vect min, Vect max) GetAABBBounds(Vect[] vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices array cannot be null or empty");
            }

            Vect min = vertices[0];
            Vect max = vertices[0];

            for (int i = 1; i < vertices.Length; i++)
            {
                min = new Vect(
                    Math.Min(min.x, vertices[i].x),
                    Math.Min(min.y, vertices[i].y),
                    Math.Min(min.z, vertices[i].z)
                );
                max = new Vect(
                    Math.Max(max.x, vertices[i].x),
                    Math.Max(max.y, vertices[i].y),
                    Math.Max(max.z, vertices[i].z)
                );
            }

            return (min, max);
        }
    }
}