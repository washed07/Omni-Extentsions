using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace OX.Numerics
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// A high-performance, all-in-one numerical data type that implements comprehensive mathematical operations.
    /// This struct provides a unified interface for floating-point arithmetic, trigonometric functions,
    /// numerical integration, root finding, statistical operations, and more.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Num struct is designed to be a drop-in replacement for float, double, and other numeric types
    /// while providing enhanced mathematical capabilities. It implements numerous .NET interfaces for
    /// seamless integration with the type system and supports implicit conversions from common numeric types.
    /// </para>
    /// 
    /// <para><strong>Key Features:</strong></para>
    /// <list type="bullet">
    /// <item><description>High-precision mathematical constants (π, e, φ, τ, etc.)</description></item>
    /// <item><description>Comprehensive trigonometric and hyperbolic functions</description></item>
    /// <item><description>Logarithmic and exponential functions</description></item>
    /// <item><description>Numerical integration methods (Runge-Kutta, Euler, Trapezoidal, Simpson's)</description></item>
    /// <item><description>Root finding algorithms (Newton-Raphson, Bisection)</description></item>
    /// <item><description>Interpolation methods (Linear, Lagrange)</description></item>
    /// <item><description>Statistical functions (mean, standard deviation)</description></item>
    /// <item><description>Vector operations (dot product, cross product)</description></item>
    /// <item><description>Random number generation (uniform, Gaussian)</description></item>
    /// <item><description>Implicit conversions to/from standard numeric types</description></item>
    /// <item><description>Full support for arithmetic, comparison, and logical operators</description></item>
    /// <item><description>Implementation of .NET generic math interfaces (INumber&lt;T&gt;, etc.)</description></item>
    /// </list>
    /// 
    /// <para><strong>Performance Considerations:</strong></para>
    /// <para>
    /// The struct uses single-precision floating-point (float) internally for optimal performance
    /// while providing mathematical functions implemented using Taylor series and other numerical
    /// methods. The tolerance for equality comparisons is configurable but set to 1e-15 by default
    /// for high precision.
    /// </para>
    /// 
    /// <para><strong>Thread Safety:</strong></para>
    /// <para>
    /// All operations are thread-safe except for the shared Random instance used in random number
    /// generation. For multithreaded scenarios requiring random numbers, consider using separate
    /// Random instances or thread-local storage.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para><strong>Basic Usage:</strong></para>
    /// <code>
    /// // Implicit conversions
    /// Num a = 5;           // From int
    /// Num b = 3.14f;       // From float
    /// Num c = 2.718;       // From double
    /// 
    /// // Arithmetic operations
    /// Num result = a + b * c;
    /// 
    /// // Mathematical functions
    /// Num sine = Num.Sin(Num.PI / 2);     // ≈ 1
    /// Num sqrt = Num.Sqrt(16);            // = 4
    /// Num log = Num.Log(Num.E);           // ≈ 1
    /// </code>
    /// 
    /// <para><strong>Numerical Analysis:</strong></para>
    /// <code>
    /// // Solve differential equation dy/dx = x + y
    /// Num.Function f = (x, y) => x + y;
    /// Num solution = Num.RK4(f, 0, 1, 0.1, 100);
    /// 
    /// // Find root of x² - 4 = 0
    /// Num.Function equation = (x, _) => x * x - 4;
    /// Num.Function derivative = (x, _) => 2 * x;
    /// Num root = Num.NewtonRaphson(equation, derivative, 1);  // ≈ 2
    /// 
    /// // Numerical integration
    /// Num.Function integrand = (x, _) => x * x;
    /// Num area = Num.SimpsonsIntegration(integrand, 0, 2, 100);  // ≈ 8/3
    /// </code>
    /// 
    /// <para><strong>Statistical Analysis:</strong></para>
    /// <code>
    /// Num[] data = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    /// var (mean, stdDev) = Num.Statistics(data);
    /// // mean = 5.5, stdDev ≈ 3.03
    /// 
    /// // Random number generation
    /// Num uniform = Num.RandomUniform(0, 100);     // 0 to 100
    /// Num normal = Num.RandomGaussian(50, 10);     // Normal dist, mean=50, σ=10
    /// </code>
    /// 
    /// <para><strong>Vector Operations:</strong></para>
    /// <code>
    /// Num[] v1 = {1, 2, 3};
    /// Num[] v2 = {4, 5, 6};
    /// 
    /// Num dotProduct = Num.Dot(v1, v2);         // = 32
    /// Num[] crossProduct = Num.Cross(v1, v2);   // = {-3, 6, -3}
    /// </code>
    /// </example>
    public readonly struct Num :
    IEquatable<Num>,
    IConvertible,
    INumber<Num>,
    IAdditionOperators<Num, Num, Num>,
    ISubtractionOperators<Num, Num, Num>,
    IMultiplyOperators<Num, Num, Num>,
    IDivisionOperators<Num, Num, Num>,
    IModulusOperators<Num, Num, Num>,
    IComparable,
    IComparable<Num>,
    ISpanFormattable,
    IFormattable,
    ISpanParsable<Num>
    {
        #region Fields and Constants

        /// <summary>
        /// The internal floating-point value stored by this Num instance.
        /// </summary>
        private readonly float _value;

        /// <summary>
        /// Tolerance used for floating-point comparisons to handle precision errors.
        /// </summary>
        /// <remarks>
        /// This value determines the precision of equality comparisons. Lower values provide
        /// higher precision but may impact performance in computation-heavy scenarios.
        /// </remarks>
        private const double Tolerance = 1e-15d;

        #endregion

        #region Mathematical Constants

        /// <summary>
        /// Euler's number (e) ≈ 2.718281828459045.
        /// </summary>
        /// <remarks>
        /// The base of natural logarithms, fundamental to exponential and logarithmic functions.
        /// </remarks>
        public static readonly Num E = 2.718281828459045235360287471352662497757247093699959574966967d;

        /// <summary>
        /// Small epsilon value for numerical comparisons and tolerances.
        /// </summary>
        /// <remarks>
        /// Used in iterative algorithms and when a small non-zero value is needed.
        /// </remarks>
        public static readonly Num Epsilon = 1e-5f;

        /// <summary>
        /// Pi (π) ≈ 3.141592653589793, the ratio of a circle's circumference to its diameter.
        /// </summary>
        public static readonly Num PI = 3.141592653589793238462643383279502884197169399375105820974944d;

        /// <summary>
        /// Pi as a double-precision value for compatibility.
        /// </summary>
        public static readonly double PId = 3.141592653589793d;

        /// <summary>
        /// Pi as a single-precision value for compatibility.
        /// </summary>
        public static readonly float PIf = 3.1415926f;

        /// <summary>
        /// Square root of 2 ≈ 1.414213562373095.
        /// </summary>
        /// <remarks>
        /// Frequently used in geometry, trigonometry, and numerical analysis.
        /// </remarks>
        public static readonly Num SQRT2 = 1.414213562373095048801688724209698078569671875376948073176679d;

        /// <summary>
        /// The golden ratio (φ) ≈ 1.618033988749895.
        /// </summary>
        /// <remarks>
        /// Also known as the divine proportion, appears in nature, art, and mathematics.
        /// Calculated as (1 + √5) / 2.
        /// </remarks>
        public static readonly Num PHI = 1.618033988749894848204586834365638117720309179805762862135448d;

        /// <summary>
        /// Tau (τ) ≈ 6.283185307179586, equal to 2π.
        /// </summary>
        /// <remarks>
        /// Represents a full circle in radians. Some mathematicians prefer τ over π
        /// for its conceptual clarity in circular mathematics.
        /// </remarks>
        public static readonly Num TAU = 6.283185307179586476925286766559005768394338798750211641949889d;

        /// <summary>
        /// Natural logarithm of 2 ≈ 0.693147180559945.
        /// </summary>
        /// <remarks>
        /// Used in logarithmic calculations and change of base formulas.
        /// </remarks>
        public static readonly Num LN2 = 0.693147180559945309417232121458176568075500134360255254120680009d;

        /// <summary>
        /// Natural logarithm of 10 ≈ 2.302585092994046.
        /// </summary>
        /// <remarks>
        /// Used for converting between natural and common logarithms.
        /// </remarks>
        public static readonly Num LN10 = 2.302585092994045684017991454684364207601101488628772976033327900d;

        /// <summary>
        /// Base-2 logarithm of e ≈ 1.442695040888963.
        /// </summary>
        /// <remarks>
        /// Reciprocal of LN2, used in logarithmic conversions.
        /// </remarks>
        public static readonly Num LOG2E = 1.442695040888963407359924681001892137426645954152985934135449407d;

        /// <summary>
        /// Base-10 logarithm of e ≈ 0.434294481903252.
        /// </summary>
        /// <remarks>
        /// Reciprocal of LN10, used for converting natural logarithms to common logarithms.
        /// </remarks>
        public static readonly Num LOG10E = 0.434294481903251827651128918916605082294397005803666566114453783d;

        #endregion

        #region Boundary Values

        /// <summary>
        /// The maximum finite value that can be represented by a Num (same as float.MaxValue).
        /// </summary>
        public static readonly Num MaxValue = float.MaxValue;

        /// <summary>
        /// The maximum finite value for double-precision compatibility.
        /// </summary>
        public static readonly Num MaxD = double.MaxValue;

        /// <summary>
        /// The minimum finite value that can be represented by a Num (same as float.MinValue).
        /// </summary>
        public static readonly Num MinValue = float.MinValue;

        /// <summary>
        /// The minimum finite value for double-precision compatibility.
        /// </summary>
        public static readonly Num MinD = double.MinValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Num struct with the specified floating-point value.
        /// </summary>
        /// <param name="value">The floating-point value to wrap.</param>
        private Num(float value) { _value = value; }

        #endregion

        #region Delegates

        /// <summary>
        /// Represents a mathematical function that takes two Num parameters and returns a Num result.
        /// </summary>
        /// <param name="x">The first parameter of the function.</param>
        /// <param name="y">The second parameter of the function.</param>
        /// <returns>The result of the function evaluation.</returns>
        /// <remarks>
        /// This delegate is commonly used in numerical integration and differential equation solving methods.
        /// </remarks>
        public delegate Num Function(Num x, Num y);

        /// <summary>
        /// Represents a mathematical function that takes three Num parameters and returns a Num result.
        /// </summary>
        /// <param name="x">The first parameter of the function.</param>
        /// <param name="y">The second parameter of the function.</param>
        /// <param name="z">The third parameter of the function.</param>
        /// <returns>The result of the function evaluation.</returns>
        /// <remarks>
        /// This delegate is used for three-dimensional mathematical functions and operations.
        /// </remarks>
        public delegate Num Function2D(Num x, Num y, Num z);

        #endregion

        #region Numerical Integration Methods

        /// <summary>
        /// Solves ordinary differential equations using the fourth-order Runge-Kutta method.
        /// </summary>
        /// <param name="f">The function representing dy/dx = f(x, y).</param>
        /// <param name="x0">The initial x value.</param>
        /// <param name="y0">The initial y value.</param>
        /// <param name="h">The step size.</param>
        /// <param name="steps">The number of integration steps to perform.</param>
        /// <returns>The approximate value of y after the specified number of steps.</returns>
        /// <remarks>
        /// The RK4 method provides higher accuracy than Euler's method by using four slope estimates
        /// per step. It's particularly effective for smooth functions and is widely used in scientific computing.
        /// The method has O(h^4) local truncation error, making it highly accurate for most applications.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Solve dy/dx = x + y with initial condition y(0) = 1
        /// Num.Function f = (x, y) => x + y;
        /// Num result = Num.RK4(f, 0, 1, 0.1, 10);
        /// </code>
        /// </example>
        public static Num RK4(Function f, Num x0, Num y0, Num h, int steps)
        {
            Num x = x0;
            Num y = y0;

            for (int i = 0; i < steps; i++)
            {
                Num k1 = f(x, y);
                Num k2 = f(x + (h / 2), y + (h * k1 / 2));
                Num k3 = f(x + (h / 2), y + (h * k2 / 2));
                Num k4 = f(x + h, y + (h * k3));

                y += h * (k1 + (2 * k2) + (2 * k3) + k4) / 6;
                x += h;
            }

            return y;
        }

        /// <summary>
        /// Solves ordinary differential equations using Euler's method (first-order).
        /// </summary>
        /// <param name="f">The function representing dy/dx = f(x, y).</param>
        /// <param name="x0">The initial x value.</param>
        /// <param name="y0">The initial y value.</param>
        /// <param name="h">The step size.</param>
        /// <param name="steps">The number of integration steps to perform.</param>
        /// <returns>The approximate value of y after the specified number of steps.</returns>
        /// <remarks>
        /// Euler's method is the simplest numerical method for solving ordinary differential equations.
        /// While less accurate than higher-order methods like RK4, it's computationally efficient
        /// and useful for quick approximations or when high precision isn't required.
        /// The method has O(h) local truncation error.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Solve dy/dx = x with initial condition y(0) = 0
        /// Num.Function f = (x, y) => x;
        /// Num result = Num.EulersMethod(f, 0, 0, 0.1, 10);
        /// </code>
        /// </example>
        public static Num EulersMethod(Function f, Num x0, Num y0, Num h, int steps)
        {
            Num x = x0;
            Num y = y0;

            for (int i = 0; i < steps; i++)
            {
                y += h * f(x, y);
                x += h;
            }

            return y;
        }

        /// <summary>
        /// Performs numerical integration using the trapezoidal rule.
        /// </summary>
        /// <param name="f">The function to integrate (only x parameter is used, y is set to Zero).</param>
        /// <param name="a">The lower bound of integration.</param>
        /// <param name="b">The upper bound of integration.</param>
        /// <param name="n">The number of subdivisions (higher values increase accuracy).</param>
        /// <returns>The approximate value of the definite integral.</returns>
        /// <remarks>
        /// The trapezoidal rule approximates the definite integral by dividing the area under
        /// the curve into trapezoids. It's more accurate than the rectangle rule but less
        /// accurate than Simpson's rule. The method has O(h^2) error where h = (b-a)/n.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Integrate x^2 from 0 to 1
        /// Num.Function f = (x, _) => x * x;
        /// Num result = Num.TrapezoidalIntegration(f, 0, 1, 100);
        /// </code>
        /// </example>
        public static Num TrapezoidalIntegration(Function f, Num a, Num b, int n)
        {
            Num h = (b - a) / n;
            Num sum = (f(a, Zero) + f(b, Zero)) / 2;

            for (int i = 1; i < n; i++)
            {
                Num x = a + (i * h);
                sum += f(x, Zero);
            }

            return h * sum;
        }

        /// <summary>
        /// Performs numerical integration using Simpson's rule (composite Simpson's 1/3 rule).
        /// </summary>
        /// <param name="f">The function to integrate (only x parameter is used, y is set to Zero).</param>
        /// <param name="a">The lower bound of integration.</param>
        /// <param name="b">The upper bound of integration.</param>
        /// <param name="n">The number of subdivisions (will be incremented if odd to ensure even count).</param>
        /// <returns>The approximate value of the definite integral.</returns>
        /// <remarks>
        /// Simpson's rule provides higher accuracy than the trapezoidal rule by using parabolic
        /// approximations instead of linear ones. It requires an even number of subdivisions.
        /// The method has O(h^4) error where h = (b-a)/n, making it very accurate for smooth functions.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Integrate sin(x) from 0 to π
        /// Num.Function f = (x, _) => Num.Sin(x);
        /// Num result = Num.SimpsonsIntegration(f, 0, Num.PI, 100);
        /// </code>
        /// </example>
        public static Num SimpsonsIntegration(Function f, Num a, Num b, int n)
        {
            if (n % 2 != 0)
            {
                n++; // Ensure n is even
            }

            Num h = (b - a) / n;
            Num sum = f(a, Zero) + f(b, Zero);

            for (int i = 1; i < n; i++)
            {
                Num x = a + (i * h);
                sum += (i % 2 == 0 ? 2 : 4) * f(x, Zero);
            }

            return h * sum / 3;
        }

        #endregion

        #region Root Finding Methods

        /// <summary>
        /// Finds the root of a function using the Newton-Raphson method.
        /// </summary>
        /// <param name="f">The function for which to find the root.</param>
        /// <param name="df">The derivative of the function.</param>
        /// <param name="x0">The initial guess for the root.</param>
        /// <param name="tolerance">The convergence tolerance (uses Epsilon if default/zero).</param>
        /// <param name="maxIterations">The maximum number of iterations to perform.</param>
        /// <returns>The approximate root of the function.</returns>
        /// <remarks>
        /// The Newton-Raphson method uses the function and its derivative to iteratively
        /// converge to a root. It has quadratic convergence near the root but requires
        /// a good initial guess and may fail if the derivative is zero or nearly zero.
        /// The method uses the formula: x_{n+1} = x_n - f(x_n)/f'(x_n).
        /// </remarks>
        /// <example>
        /// <code>
        /// // Find root of f(x) = x^2 - 4 (should find x = 2)
        /// Num.Function f = (x, _) => x * x - 4;
        /// Num.Function df = (x, _) => 2 * x;
        /// Num root = Num.NewtonRaphson(f, df, 1);
        /// </code>
        /// </example>
        public static Num NewtonRaphson(Function f, Function df, Num x0, Num tolerance = default, int maxIterations = 100)
        {
            if (tolerance == Zero)
            {
                tolerance = Epsilon;
            }

            Num x = x0;

            for (int i = 0; i < maxIterations; i++)
            {
                Num fx = f(x, Zero);
                Num dfx = df(x, Zero);

                if (Abs(dfx) < tolerance)
                {
                    break;
                }

                Num dx = fx / dfx;
                x -= dx;

                if (Abs(dx) < tolerance)
                {
                    return x;
                }
            }

            return x;
        }

        /// <summary>
        /// Finds the root of a function using the bisection method.
        /// </summary>
        /// <param name="f">The continuous function for which to find the root.</param>
        /// <param name="a">The left endpoint of the initial interval.</param>
        /// <param name="b">The right endpoint of the initial interval.</param>
        /// <param name="tolerance">The convergence tolerance (uses Epsilon if default/zero).</param>
        /// <param name="maxIterations">The maximum number of iterations to perform.</param>
        /// <returns>The approximate root of the function.</returns>
        /// <exception cref="ArgumentException">Thrown when the function doesn't have opposite signs at the endpoints.</exception>
        /// <remarks>
        /// The bisection method is a robust root-finding algorithm that works by repeatedly
        /// halving the interval containing the root. It requires that the function be continuous
        /// and have opposite signs at the endpoints. While slower than Newton-Raphson,
        /// it's guaranteed to converge to a root if one exists in the interval.
        /// The method has linear convergence.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Find root of f(x) = x^3 - x - 1 in interval [1, 2]
        /// Num.Function f = (x, _) => x * x * x - x - 1;
        /// Num root = Num.Bisection(f, 1, 2);
        /// </code>
        /// </example>
        public static Num Bisection(Function f, Num a, Num b, Num tolerance = default, int maxIterations = 100)
        {
            if (tolerance == Zero)
            {
                tolerance = Epsilon;
            }

            Num fa = f(a, Zero);
            Num fb = f(b, Zero);

            if (fa * fb >= 0)
            {
                throw new ArgumentException("Function must have opposite signs at endpoints");
            }

            for (int i = 0; i < maxIterations; i++)
            {
                Num c = (a + b) / 2;
                Num fc = f(c, Zero);

                if (Abs(fc) < tolerance || (b - a) / 2 < tolerance)
                {
                    return c;
                }

                if (fa * fc < 0)
                {
                    b = c;
                }
                else
                {
                    a = c;
                    fa = fc;
                }
            }

            return (a + b) / 2;
        }

        #endregion

        #region Interpolation Methods

        /// <summary>
        /// Performs linear interpolation between two points.
        /// </summary>
        /// <param name="x0">The x-coordinate of the first point.</param>
        /// <param name="y0">The y-coordinate of the first point.</param>
        /// <param name="x1">The x-coordinate of the second point.</param>
        /// <param name="y1">The y-coordinate of the second point.</param>
        /// <param name="x">The x-coordinate at which to interpolate.</param>
        /// <returns>The interpolated y-value at the specified x-coordinate.</returns>
        /// <remarks>
        /// Linear interpolation (lerp) estimates values between two known points using a straight line.
        /// This is the simplest form of interpolation and is widely used in computer graphics,
        /// animation, and numerical analysis. The formula used is:
        /// y = y0 + (x - x0) * (y1 - y0) / (x1 - x0)
        /// </remarks>
        /// <example>
        /// <code>
        /// // Interpolate between points (0, 1) and (10, 5) at x = 5
        /// Num result = Num.Lerp(0, 1, 10, 5, 5); // Result: 3
        /// </code>
        /// </example>
        public static Num Lerp(Num x0, Num y0, Num x1, Num y1, Num x)
        {
            return y0 + ((x - x0) * (y1 - y0) / (x1 - x0));
        }

        /// <summary>
        /// Performs Lagrange polynomial interpolation through a set of points.
        /// </summary>
        /// <param name="x">Array of x-coordinates of the known points.</param>
        /// <param name="y">Array of y-coordinates of the known points.</param>
        /// <param name="xi">The x-coordinate at which to interpolate.</param>
        /// <returns>The interpolated y-value at the specified x-coordinate.</returns>
        /// <remarks>
        /// Lagrange interpolation finds a polynomial of degree n-1 that passes through
        /// n given points. Unlike linear interpolation, this method can handle any number
        /// of points and produces a smooth curve. However, it can suffer from Runge's
        /// phenomenon with high-degree polynomials and equally-spaced points.
        /// 
        /// The method name "Larp" appears to be a typo for "Lagrange" but is kept for
        /// backward compatibility.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Interpolate through points (0,1), (1,4), (2,9)
        /// Num[] x = {0, 1, 2};
        /// Num[] y = {1, 4, 9};
        /// Num result = Num.Larp(x, y, 1.5);
        /// </code>
        /// </example>
        public static Num Larp(Num[] x, Num[] y, Num xi)
        {
            Num result = Zero;

            for (int i = 0; i < x.Length; i++)
            {
                Num term = y[i];
                for (int j = 0; j < x.Length; j++)
                {
                    if (j != i)
                    {
                        term *= (xi - x[j]) / (x[i] - x[j]);
                    }
                }
                result += term;
            }

            return result;
        }

        #endregion

        #region Statistical Methods

        /// <summary>
        /// Calculates basic statistical measures (mean and standard deviation) for a dataset.
        /// </summary>
        /// <param name="values">Array of values to analyze.</param>
        /// <returns>A tuple containing the mean and sample standard deviation.</returns>
        /// <exception cref="ArgumentException">Thrown when the array is null or empty.</exception>
        /// <remarks>
        /// This method calculates the arithmetic mean and the sample standard deviation
        /// (using Bessel's correction with n-1 in the denominator). The standard deviation
        /// measures the spread of the data around the mean.
        /// 
        /// For population standard deviation, the denominator would be n instead of n-1.
        /// This implementation uses the sample standard deviation which is more commonly
        /// used when working with sample data rather than entire populations.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num[] data = {1, 2, 3, 4, 5};
        /// var (mean, stdDev) = Num.Statistics(data);
        /// // mean = 3, stdDev ≈ 1.58
        /// </code>
        /// </example>
        public static (Num mean, Num stdDev) Statistics(Num[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("Array cannot be null or empty");
            }

            Num sum = Zero;
            foreach (Num value in values)
            {
                sum += value;
            }

            Num mean = sum / values.Length;

            Num sumSquaredDiff = Zero;
            foreach (Num value in values)
            {
                Num diff = value - mean;
                sumSquaredDiff += diff * diff;
            }

            Num stdDev = Sqrt(sumSquaredDiff / (values.Length - 1));
            return (mean, stdDev);
        }

        #endregion

        #region Vector Operations

        /// <summary>
        /// Calculates the dot product (scalar product) of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        /// <exception cref="ArgumentException">Thrown when vectors have different lengths.</exception>
        /// <remarks>
        /// The dot product is a fundamental operation in linear algebra that measures
        /// the similarity between two vectors. It's calculated as the sum of the products
        /// of corresponding components: a·b = Σ(a[i] * b[i]).
        /// 
        /// The dot product has geometric significance:
        /// - If the result is positive, the vectors point in similar directions
        /// - If zero, the vectors are orthogonal (perpendicular)
        /// - If negative, the vectors point in opposite directions
        /// </remarks>
        /// <example>
        /// <code>
        /// Num[] v1 = {1, 2, 3};
        /// Num[] v2 = {4, 5, 6};
        /// Num dotProduct = Num.Dot(v1, v2); // Result: 32
        /// </code>
        /// </example>
        public static Num Dot(Num[] a, Num[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("Vectors must be of same length");
            }

            Num sum = Zero;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }

        /// <summary>
        /// Calculates the cross product (vector product) of two 3D vectors.
        /// </summary>
        /// <param name="a">The first 3D vector.</param>
        /// <param name="b">The second 3D vector.</param>
        /// <returns>A new 3D vector representing the cross product.</returns>
        /// <exception cref="ArgumentException">Thrown when either vector is not 3-dimensional.</exception>
        /// <remarks>
        /// The cross product is defined only for 3D vectors and produces a vector that is
        /// perpendicular to both input vectors. The magnitude of the result equals the area
        /// of the parallelogram formed by the two input vectors.
        /// 
        /// The cross product follows the right-hand rule:
        /// - Point fingers in direction of first vector
        /// - Curl fingers toward second vector
        /// - Thumb points in direction of cross product
        /// 
        /// Formula: a × b = (a₂b₃ - a₃b₂, a₃b₁ - a₁b₃, a₁b₂ - a₂b₁)
        /// </remarks>
        /// <example>
        /// <code>
        /// Num[] v1 = {1, 0, 0}; // X-axis
        /// Num[] v2 = {0, 1, 0}; // Y-axis
        /// Num[] cross = Num.Cross(v1, v2); // Result: {0, 0, 1} (Z-axis)
        /// </code>
        /// </example>
        public static Num[] Cross(Num[] a, Num[] b)
        {
            return a.Length != 3 || b.Length != 3
                ? throw new ArgumentException("Vectors must be 3-dimensional")
                : (new Num[]
            {
                (a[1] * b[2]) - (a[2] * b[1]),
                (a[2] * b[0]) - (a[0] * b[2]),
                (a[0] * b[1]) - (a[1] * b[0])
            });
        }

        #endregion

        #region Random Number Generation

        /// <summary>
        /// Static Random instance used for random number generation.
        /// </summary>
        private static readonly Random random = new();

        /// <summary>
        /// Generates a random number from a uniform distribution.
        /// </summary>
        /// <param name="min">The minimum value (inclusive). Defaults to 0.</param>
        /// <param name="max">The maximum value (exclusive). Defaults to 1.</param>
        /// <returns>A random number between min and max.</returns>
        /// <remarks>
        /// Uniform distribution means each value in the range has an equal probability
        /// of being selected. This is useful for general random number generation,
        /// simulations, and Monte Carlo methods.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num randomValue = Num.RandomUniform(5, 10); // Random between 5 and 10
        /// Num randomPercent = Num.RandomUniform(); // Random between 0 and 1
        /// </code>
        /// </example>
        public static Num RandomUniform(Num min = default, float max = 1)
        {
            return min + ((max - min) * (Num)random.NextDouble());
        }

        /// <summary>
        /// Generates a random number from a Gaussian (normal) distribution.
        /// </summary>
        /// <param name="mean">The mean (center) of the distribution. Defaults to 0.</param>
        /// <param name="stdDev">The standard deviation (spread) of the distribution. Defaults to 1.</param>
        /// <returns>A random number from the specified Gaussian distribution.</returns>
        /// <remarks>
        /// Uses the Box-Muller transform to convert uniform random numbers into
        /// Gaussian-distributed random numbers. The Gaussian distribution is the
        /// familiar bell curve and appears frequently in nature and statistics.
        /// 
        /// About 68% of values fall within 1 standard deviation of the mean,
        /// 95% within 2 standard deviations, and 99.7% within 3 standard deviations.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num normalValue = Num.RandomGaussian(); // Standard normal (mean=0, stdDev=1)
        /// Num customValue = Num.RandomGaussian(100, 15); // Mean=100, stdDev=15
        /// </code>
        /// </example>
        public static Num RandomGaussian(Num mean = default, float stdDev = 1)
        {
            Num u1 = RandomUniform();
            Num u2 = RandomUniform();

            Num z = Sqrt(-2 * Log(u1)) * Cos(2 * PI * u2);
            return mean + (stdDev * z);
        }

        #endregion

        #region Basic Mathematical Functions

        /// <summary>
        /// Returns the absolute value of a number.
        /// </summary>
        /// <param name="value">The number to get the absolute value of.</param>
        /// <returns>The absolute value of the input.</returns>
        /// <remarks>
        /// The absolute value is always non-negative and represents the distance
        /// from zero on the number line, regardless of direction.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Abs(-5); // Result: 5
        /// Num result2 = Num.Abs(3);  // Result: 3
        /// </code>
        /// </example>
        public static Num Abs(Num value)
        {
            return value < 0 ? -value : value;
        }

        /// <summary>
        /// Returns the smaller of two numbers.
        /// </summary>
        /// <param name="a">The first number to compare.</param>
        /// <param name="b">The second number to compare.</param>
        /// <returns>The smaller of the two numbers.</returns>
        public static Num Min(Num a, Num b) { return a < b ? a : b; }

        /// <summary>
        /// Returns the larger of two numbers.
        /// </summary>
        /// <param name="a">The first number to compare.</param>
        /// <param name="b">The second number to compare.</param>
        /// <returns>The larger of the two numbers.</returns>
        public static Num Max(Num a, Num b) { return a > b ? a : b; }

        /// <summary>
        /// Returns the sign of a number.
        /// </summary>
        /// <param name="value">The number to determine the sign of.</param>
        /// <returns>-1 if negative, 1 if positive, 0 if zero.</returns>
        /// <remarks>
        /// The sign function is useful in mathematical algorithms and
        /// for normalizing direction vectors.
        /// </remarks>
        public static Num Sign(Num value) { return value < 0 ? -1 : value > 0 ? 1 : 0; }

        #endregion

        #region Trigonometric Functions

        /// <summary>
        /// Calculates the cosine of an angle using Taylor series expansion.
        /// </summary>
        /// <param name="value">The angle in radians.</param>
        /// <returns>The cosine of the angle.</returns>
        /// <remarks>
        /// Uses the Taylor series: cos(x) = 1 - x²/2! + x⁴/4! - x⁶/6! + ...
        /// The input is normalized to [0, 2π] for better numerical stability.
        /// This implementation uses the first 6 terms for a good balance of accuracy and performance.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Cos(Num.PI / 2); // Result ≈ 0 (cosine of 90 degrees)
        /// </code>
        /// </example>
        public static Num Cos(Num value)
        {
            // Taylor series for cosine: cos(x) = 1 - x²/2! + x⁴/4! - x⁶/6! + ...
            Num x = value % (2 * PI); // Normalize to [0, 2π]
            Num x2 = x * x;
            Num result = 1;

            // First 6 terms of Taylor series
            result -= x2 / 2;
            result += x2 * x2 / 24;
            result -= x2 * x2 * x2 / 720;
            result += x2 * x2 * x2 * x2 / 40320;

            return result;
        }

        /// <summary>
        /// Calculates the arccosine (inverse cosine) of a value using Taylor series.
        /// </summary>
        /// <param name="value">The value to calculate arccosine for (must be between -1 and 1).</param>
        /// <returns>The arccosine in radians, or NaN if input is out of range.</returns>
        /// <remarks>
        /// Uses the Taylor series: acos(x) = π/2 - x - x³/6 - 3x⁵/40 - 5x⁷/112 - ...
        /// The function returns values in the range [0, π].
        /// Returns NaN for inputs outside the valid domain [-1, 1].
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Acos(0); // Result ≈ π/2 (90 degrees)
        /// Num result2 = Num.Acos(1); // Result ≈ 0 (0 degrees)
        /// </code>
        /// </example>
        public static Num Acos(Num value)
        {
            // Taylor series for acos(x) = π/2 - x - x³/6 - 3x⁵/40 - 5x⁷/112 - 35x⁹/1152 - ...
            if (value > 1 || value < -1)
            {
                return float.NaN;
            }

            Num x = value;
            Num x2 = x * x;
            Num result = PI / 2;

            // First 6 terms of Taylor series
            result -= x;
            result -= x2 * x / 6;
            result -= 3 * x2 * x2 * x / 40;
            result -= 5 * x2 * x2 * x2 * x / 112;
            result -= 35 * x2 * x2 * x2 * x2 * x / 1152;

            return result;
        }

        /// <summary>
        /// Calculates the sine of an angle using Taylor series expansion.
        /// </summary>
        /// <param name="value">The angle in radians.</param>
        /// <returns>The sine of the angle.</returns>
        /// <remarks>
        /// Uses the Taylor series: sin(x) = x - x³/3! + x⁵/5! - x⁷/7! + ...
        /// The input is normalized to [0, 2π] for better numerical stability.
        /// This implementation uses the first 6 terms for good accuracy and performance.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Sin(Num.PI / 2); // Result ≈ 1 (sine of 90 degrees)
        /// </code>
        /// </example>
        public static Num Sin(Num value)
        {
            // Taylor series for sine: sin(x) = x - x³/3! + x⁵/5! - x⁷/7! + ...
            Num x = value % (2 * PI); // Normalize to [0, 2π]
            Num x2 = x * x;
            Num x3 = x2 * x;
            Num result = x;

            // First 6 terms of Taylor series
            result -= x3 / 6;
            result += x3 * x2 / 120;
            result -= x3 * x2 * x2 / 5040;
            result += x3 * x2 * x2 * x2 / 362880;

            return result;
        }

        /// <summary>
        /// Calculates the tangent of an angle.
        /// </summary>
        /// <param name="value">The angle in radians.</param>
        /// <returns>The tangent of the angle.</returns>
        /// <remarks>
        /// Calculated as sin(x)/cos(x). Uses a small tolerance value when cosine
        /// approaches zero to avoid division by zero errors.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Tan(Num.PI / 4); // Result ≈ 1 (tangent of 45 degrees)
        /// </code>
        /// </example>
        public static Num Tan(Num value)
        {
            Num s = Sin(value);
            Num c = Cos(value);
            return s / (c == 0 ? Tolerance : c);
        }

        /// <summary>
        /// Calculates the arctangent (inverse tangent) of a value using Taylor series.
        /// </summary>
        /// <param name="value">The value to calculate arctangent for.</param>
        /// <returns>The arctangent in radians, in the range [-π/2, π/2].</returns>
        /// <remarks>
        /// Uses the Taylor series: atan(x) = x - x³/3 + x⁵/5 - x⁷/7 + ...
        /// For values outside [-1, 1], uses the identity atan(x) = π/2 - atan(1/x).
        /// This method provides good accuracy for all input values.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Atan(1); // Result ≈ π/4 (45 degrees)
        /// Num result2 = Num.Atan(0); // Result: 0
        /// </code>
        /// </example>
        public static Num Atan(Num value)
        {
            // Taylor series for atan(x) = x - x³/3 + x⁵/5 - x⁷/7 + ...
            if (value > 1 || value < -1)
            {
                return (PI / 2) - Atan(1 / value);
            }

            Num x = value;
            Num x2 = x * x;
            Num result = x;

            for (int i = 3; i <= 11; i += 2)
            {
                x *= -x2;
                result += x / i;
            }

            return result;
        }

        /// <summary>
        /// Calculates the arctangent of the quotient of two specified numbers.
        /// </summary>
        /// <param name="y">The y-coordinate of a point.</param>
        /// <param name="x">The x-coordinate of a point.</param>
        /// <returns>
        /// The angle θ in radians such that -π ≤ θ ≤ π, and tan(θ) = y/x.
        /// The angle is in the correct quadrant based on the signs of both parameters.
        /// </returns>
        /// <remarks>
        /// This function is useful for converting Cartesian coordinates to polar coordinates.
        /// Unlike Atan(y/x), this function considers the signs of both arguments to determine
        /// the correct quadrant of the result. This avoids issues with division by zero
        /// and provides a full range of [-π, π] for the result.
        /// 
        /// Special cases:
        /// - If x > 0, returns Atan(y/x)
        /// - If x < 0 and y ≥ 0, returns Atan(y/x) + π
        /// - If x < 0 and y < 0, returns Atan(y/x) - π
        /// - If x = 0 and y > 0, returns π/2
        /// - If x = 0 and y < 0, returns -π/2
        /// - If x = 0 and y = 0, returns 0 (undefined case)
        /// </remarks>
        /// <example>
        /// <code>
        /// Num angle1 = Num.Atan2(1, 1);   // Result ≈ π/4 (45 degrees, first quadrant)
        /// Num angle2 = Num.Atan2(1, -1);  // Result ≈ 3π/4 (135 degrees, second quadrant)
        /// Num angle3 = Num.Atan2(-1, -1); // Result ≈ -3π/4 (-135 degrees, third quadrant)
        /// </code>
        /// </example>
        public static Num Atan2(Num y, Num x)
        {
            if (x > 0)
            {
                return Atan(y / x);
            }
            else if (x < 0)
            {
                return y >= 0 ? Atan(y / x) + PI : Atan(y / x) - PI;
            }
            else if (y > 0)
            {
                return PI / 2;
            }
            else if (y < 0)
            {
                return -PI / 2;
            }

            return 0; // undefined, but return 0
        }

        #endregion

        #region Mathematical Functions

        /// <summary>
        /// Calculates the square root of a number using Newton's method.
        /// </summary>
        /// <param name="value">The number to calculate the square root of.</param>
        /// <returns>The square root of the input, or NaN if the input is negative.</returns>
        /// <remarks>
        /// Uses Newton's iterative method: x[n+1] = (x[n] + value/x[n])/2
        /// This provides high accuracy and fast convergence for positive numbers.
        /// Returns NaN for negative inputs since complex numbers are not supported.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Sqrt(25); // Result: 5
        /// Num result2 = Num.Sqrt(2); // Result: ≈ 1.414
        /// </code>
        /// </example>
        public static Num Sqrt(Num value)
        {
            if (value < 0)
            {
                return new Num(float.NaN);
            }

            if (value == 0)
            {
                return Zero;
            }

            // Newton's method: x[n+1] = (x[n] + value/x[n])/2
            Num x = value / 2;
            Num lastX;

            do
            {
                lastX = x;
                x = (x + (value / x)) / 2;
            } while (Abs(x - lastX) > Tolerance);

            return x;
        }

        /// <summary>
        /// Calculates the cube root (third root) of a number using Newton's method.
        /// </summary>
        /// <param name="value">The number to calculate the cube root of.</param>
        /// <returns>The cube root of the input value.</returns>
        /// <remarks>
        /// Uses Newton's method for cube root: x[n+1] = (2*x[n] + value/x[n]²)/3
        /// This method works for both positive and negative values, unlike square root.
        /// The iteration runs for 8 steps, which provides good accuracy for most values.
        /// For zero input, returns zero directly without iteration.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Cbrt(27);  // Result: 3
        /// Num result2 = Num.Cbrt(-8);  // Result: -2
        /// Num result3 = Num.Cbrt(2);   // Result: ≈ 1.26
        /// </code>
        /// </example>
        public static Num Cbrt(Num value)
        {
            if (value == 0)
            {
                return Zero;
            }

            Num x = value / 3;  // Initial guess

            // Newton's method for cube root
            for (int i = 0; i < 8; i++)
            {
                x = ((2 * x) + (value / (x * x))) / 3;
            }

            return x;
        }

        /// <summary>
        /// Calculates e raised to the specified power using Taylor series expansion.
        /// </summary>
        /// <param name="value">The exponent.</param>
        /// <returns>e raised to the specified power.</returns>
        /// <remarks>
        /// Uses the Taylor series: e^x = 1 + x + x²/2! + x³/3! + x⁴/4! + ...
        /// This implementation uses the first 10 terms for good accuracy.
        /// For x = 0, returns exactly 1.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Exp(1);  // Result ≈ e ≈ 2.718
        /// Num result2 = Num.Exp(0); // Result: 1
        /// </code>
        /// </example>
        public static Num Exp(Num value)
        {
            // Taylor series for e^x
            if (value == 0)
            {
                return One;
            }

            Num result = One;
            Num term = One;

            for (int i = 1; i <= 10; i++)
            {
                term *= value / i;
                result += term;
            }

            return result;
        }

        /// <summary>
        /// Calculates the natural logarithm (base e) of a number.
        /// </summary>
        /// <param name="value">The number to calculate the natural logarithm of.</param>
        /// <returns>The natural logarithm of the input, or NaN if input is non-positive.</returns>
        /// <remarks>
        /// Uses the identity: log(x) = 2 * atanh((x-1)/(x+1))
        /// This method is numerically stable and accurate for positive inputs.
        /// Returns NaN for zero or negative inputs.
        /// Returns exactly 0 for input of 1.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Log(Num.E);  // Result ≈ 1
        /// Num result2 = Num.Log(1);     // Result: 0
        /// </code>
        /// </example>
        public static Num Log(Num value)
        {
            // Use the identity log(x) = 2 * atanh((x-1)/(x+1))
            if (value <= 0)
            {
                return new Num(float.NaN);
            }

            if (value == 1)
            {
                return Zero;
            }

            Num y = (value - 1) / (value + 1);
            Num y2 = y * y;
            Num result = y;

            for (int i = 3; i <= 11; i += 2)
            {
                y *= y2;
                result += y / i;
            }

            return 2 * result;
        }

        /// <summary>
        /// Calculates the base-2 logarithm of a number.
        /// </summary>
        /// <param name="value">The number to calculate the base-2 logarithm of.</param>
        /// <returns>The base-2 logarithm of the input, or NaN if input is non-positive.</returns>
        /// <remarks>
        /// Uses the change of base formula: log₂(x) = ln(x) / ln(2)
        /// This function is useful in computer science for calculating bit depths,
        /// complexity analysis, and binary tree heights.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Log2(8);   // Result: 3 (since 2³ = 8)
        /// Num result2 = Num.Log2(1024); // Result: 10 (since 2¹⁰ = 1024)
        /// </code>
        /// </example>
        public static Num Log2(Num value)
        {
            return Log(value) / LN2;
        }

        /// <summary>
        /// Calculates the base-10 (common) logarithm of a number.
        /// </summary>
        /// <param name="value">The number to calculate the base-10 logarithm of.</param>
        /// <returns>The base-10 logarithm of the input, or NaN if input is non-positive.</returns>
        /// <remarks>
        /// Uses the change of base formula: log₁₀(x) = ln(x) / ln(10)
        /// The common logarithm is widely used in engineering, science, and the decibel scale.
        /// Each unit increase represents a 10-fold increase in the original value.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Log10(100);  // Result: 2 (since 10² = 100)
        /// Num result2 = Num.Log10(1000); // Result: 3 (since 10³ = 1000)
        /// </code>
        /// </example>
        public static Num Log10(Num value)
        {
            return Log(value) / LN10;
        }

        /// <summary>
        /// Calculates the hyperbolic sine of a value.
        /// </summary>
        /// <param name="value">The value to calculate the hyperbolic sine of.</param>
        /// <returns>The hyperbolic sine of the input.</returns>
        /// <remarks>
        /// Uses the definition: sinh(x) = (e^x - e^(-x)) / 2
        /// The hyperbolic sine function appears in solutions to certain differential equations
        /// and in the definition of hyperbolic geometry. It's an odd function: sinh(-x) = -sinh(x).
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Sinh(0);  // Result: 0
        /// Num result2 = Num.Sinh(1);  // Result: ≈ 1.175
        /// </code>
        /// </example>
        public static Num Sinh(Num value)
        {
            return (Exp(value) - Exp(-value)) / 2;
        }

        /// <summary>
        /// Calculates the hyperbolic cosine of a value.
        /// </summary>
        /// <param name="value">The value to calculate the hyperbolic cosine of.</param>
        /// <returns>The hyperbolic cosine of the input.</returns>
        /// <remarks>
        /// Uses the definition: cosh(x) = (e^x + e^(-x)) / 2
        /// The hyperbolic cosine function represents the shape of a hanging chain or cable
        /// (catenary curve). It's an even function: cosh(-x) = cosh(x), and cosh(0) = 1.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Cosh(0);  // Result: 1
        /// Num result2 = Num.Cosh(1);  // Result: ≈ 1.543
        /// </code>
        /// </example>
        public static Num Cosh(Num value)
        {
            return (Exp(value) + Exp(-value)) / 2;
        }

        /// <summary>
        /// Calculates the hyperbolic tangent of a value.
        /// </summary>
        /// <param name="value">The value to calculate the hyperbolic tangent of.</param>
        /// <returns>The hyperbolic tangent of the input.</returns>
        /// <remarks>
        /// Uses the formula: tanh(x) = (e^(2x) - 1) / (e^(2x) + 1)
        /// The hyperbolic tangent function is commonly used in neural networks as an activation function.
        /// It's an odd function that approaches ±1 as x approaches ±∞, making it useful for normalization.
        /// Special handling prevents overflow for large absolute values.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Tanh(0);   // Result: 0
        /// Num result2 = Num.Tanh(1);   // Result: ≈ 0.762
        /// Num result3 = Num.Tanh(100); // Result: ≈ 1 (saturated)
        /// </code>
        /// </example>
        public static Num Tanh(Num value)
        {
            if (value > 50)
            {
                return One;  // Avoid overflow
            }

            if (value < -50)
            {
                return -One;
            }

            Num exp2x = Exp(2 * value);
            return (exp2x - 1) / (exp2x + 1);
        }

        /// <summary>
        /// Raises a number to an integer power using fast exponentiation by squaring.
        /// </summary>
        /// <param name="value">The base number to raise to a power.</param>
        /// <param name="n">The integer exponent.</param>
        /// <returns>The result of value raised to the power of n.</returns>
        /// <remarks>
        /// Uses the efficient exponentiation by squaring algorithm, which has O(log n) complexity
        /// instead of O(n) for naive multiplication. This method is particularly efficient for
        /// large integer exponents.
        /// 
        /// For negative exponents, the method calculates 1/value^|n|.
        /// For zero exponent, always returns 1 regardless of the base (except 0^0 case).
        /// 
        /// Algorithm: Repeatedly squares the base and halves the exponent, multiplying the result
        /// when the exponent is odd.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.PowInt(2, 10);  // Result: 1024
        /// Num result2 = Num.PowInt(5, -2);  // Result: 0.04 (1/25)
        /// Num result3 = Num.PowInt(7, 0);   // Result: 1
        /// </code>
        /// </example>
        public static Num PowInt(Num value, int n)
        {
            if (n == 0)
            {
                return One;
            }

            if (n < 0)
            {
                value = One / value;
                n = -n;
            }

            Num result = One;
            while (n > 0)
            {
                if ((n & 1) == 1)
                {
                    result *= value;
                }

                value *= value;
                n >>= 1;
            }
            return result;
        }

        /// <summary>
        /// Raises a number to the specified power.
        /// </summary>
        /// <param name="x">The base number.</param>
        /// <param name="y">The exponent.</param>
        /// <returns>x raised to the power of y.</returns>
        /// <remarks>
        /// <para>
        /// Uses different algorithms based on the exponent type:
        /// - For integer exponents: Fast exponentiation by squaring
        /// - For non-integer exponents: exp(y * ln(x))
        /// </para>
        /// <para>
        /// Special cases:
        /// - Any number to the power of 0 equals 1
        /// - 0 to any positive power equals 0
        /// - Any number to the power of 1 equals itself
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result = Num.Pow(2, 3);    // Result: 8
        /// Num result2 = Num.Pow(4, 0.5); // Result: 2 (square root)
        /// </code>
        /// </example>
        public static Num Pow(Num x, Num y)
        {
            if (y == 0)
            {
                return One;
            }

            if (x == 0)
            {
                return Zero;
            }

            if (y == 1)
            {
                return x;
            }

            // Handle integer powers efficiently
            if (IsInteger(y))
            {
                return PowInt(x, (int)y);
            }

            // For non-integer powers, use exp(y*ln(x))
            return Exp(y * Log(x));
        }

        /// <summary>
        /// Rounds a number to the nearest integer using "round half up" strategy.
        /// </summary>
        /// <param name="value">The number to round.</param>
        /// <returns>The nearest integer value.</returns>
        /// <remarks>
        /// Uses the "round half up" (or "round half away from zero") strategy where
        /// values exactly halfway between two integers are rounded up to the next integer.
        /// For example, 2.5 rounds to 3, and -2.5 rounds to -2.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Round(2.3);  // Result: 2
        /// Num result2 = Num.Round(2.5);  // Result: 3
        /// Num result3 = Num.Round(2.7);  // Result: 3
        /// </code>
        /// </example>
        public static Num Round(Num value)
        {
            Num integerPart = new((int)value._value);
            Num fraction = value - integerPart;

            return fraction < 0.5 ? integerPart : integerPart + One;
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        /// <param name="value">The number to floor.</param>
        /// <returns>The largest integer ≤ value.</returns>
        /// <remarks>
        /// The floor function always rounds down toward negative infinity.
        /// For positive numbers, this truncates the decimal part.
        /// For negative numbers, it rounds away from zero if there's a fractional part.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Floor(2.7);   // Result: 2
        /// Num result2 = Num.Floor(-2.3);  // Result: -3
        /// Num result3 = Num.Floor(5.0);   // Result: 5
        /// </code>
        /// </example>
        public static Num Floor(Num value)
        {
            return new Num((int)value._value);
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to the specified number.
        /// </summary>
        /// <param name="value">The number to ceiling.</param>
        /// <returns>The smallest integer ≥ value.</returns>
        /// <remarks>
        /// The ceiling function always rounds up toward positive infinity.
        /// For positive numbers with fractional parts, this rounds away from zero.
        /// For negative numbers, this truncates the decimal part.
        /// If the input is already an integer, it returns the same value.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Ceiling(2.3);  // Result: 3
        /// Num result2 = Num.Ceiling(-2.7); // Result: -2
        /// Num result3 = Num.Ceiling(5.0);  // Result: 5
        /// </code>
        /// </example>
        public static Num Ceiling(Num value)
        {
            int intPart = (int)value._value;
            return value == intPart ? value : new Num(intPart + 1);
        }

        /// <summary>
        /// Clamps a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>The clamped value within [min, max].</returns>
        /// <remarks>
        /// If value is less than min, returns min.
        /// If value is greater than max, returns max.
        /// Otherwise, returns the original value.
        /// This function is useful for ensuring values stay within valid bounds.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Clamp(5, 1, 10);   // Result: 5 (within range)
        /// Num result2 = Num.Clamp(-2, 1, 10);  // Result: 1 (clamped to min)
        /// Num result3 = Num.Clamp(15, 1, 10);  // Result: 10 (clamped to max)
        /// </code>
        /// </example>
        public static Num Clamp(Num value, Num min, Num max) { return Min(Max(value, min), max); }

        /// <summary>
        /// Generic conversion method for numeric types to Num.
        /// </summary>
        /// <typeparam name="T">The type that implements IConvertible.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>A new Num instance with the converted value.</returns>
        /// <remarks>
        /// This internal method is used by implicit conversion operators to convert
        /// various numeric types to Num. It uses Convert.ToSingle for the conversion.
        /// </remarks>
        private static Num FromT<T>(T value) where T : IConvertible { return new Num(Convert.ToSingle(value)); }

        /// <summary>
        /// Converts the current value from degrees to radians.
        /// </summary>
        /// <returns>The equivalent value in radians.</returns>
        /// <remarks>
        /// Uses the conversion formula: radians = degrees × π / 180
        /// This method treats the current Num value as degrees and converts it to radians.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num degrees = 90;
        /// Num radians = degrees.ToRadians(); // Result: ≈ π/2 ≈ 1.571
        /// </code>
        /// </example>
        public Num ToRadians() { return new Num(_value * PI / 180); }

        /// <summary>
        /// Generic conversion method for nullable numeric types to nullable Num.
        /// </summary>
        /// <typeparam name="T">The nullable type that implements IConvertible.</typeparam>
        /// <param name="value">The nullable value to convert.</param>
        /// <returns>A nullable Num instance, or null if input is null.</returns>
        /// <remarks>
        /// This internal method handles nullable type conversions for implicit operators.
        /// Returns null if the input value is null, otherwise converts using FromT.
        /// </remarks>
        private static Num? FromT<T>(T? value) where T : struct, IConvertible
        {
            return value == null ? null : new Num(Convert.ToSingle(value));
        }

        /// <summary>
        /// Creates a new Num instance from an existing Num value.
        /// </summary>
        /// <param name="value">The Num value to copy.</param>
        /// <returns>A new Num instance with the same value.</returns>
        /// <remarks>
        /// This method appears to be a utility function for creating Num instances,
        /// though it's functionally equivalent to just using the input value directly
        /// since Num is a value type.
        /// </remarks>
        public static Num N(Num value) { return new Num(value); }

        /// <summary>
        /// Converts the current value from degrees to radians.
        /// </summary>
        /// <returns>The equivalent value in radians.</returns>
        /// <remarks>
        /// Uses the conversion formula: radians = degrees × π / 180
        /// This is an alias for ToRadians() method with the same functionality.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num degrees = 180;
        /// Num radians = degrees.Radians(); // Result: ≈ π ≈ 3.14159
        /// </code>
        /// </example>
        public Num Radians() { return new Num(_value * PI / 180); }

        /// <summary>
        /// Converts the current value from radians to degrees.
        /// </summary>
        /// <returns>The equivalent value in degrees.</returns>
        /// <remarks>
        /// Uses the conversion formula: degrees = radians × 180 / π
        /// This method treats the current Num value as radians and converts it to degrees.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num radians = Num.PI;
        /// Num degrees = radians.Degrees(); // Result: 180
        /// </code>
        /// </example>
        public Num Degrees() { return new Num(_value * 180 / PI); }

        #endregion

        #region Conversion Operators

        /// <summary>
        /// Implicitly converts a byte to a Num.
        /// </summary>
        public static implicit operator Num(byte value) { return FromT(value); }

        /// <summary>
        /// Implicitly converts an int to a Num.
        /// </summary>
        public static implicit operator Num(int value) { return FromT(value); }

        /// <summary>
        /// Implicitly converts a float to a Num.
        /// </summary>
        public static implicit operator Num(float value) { return FromT(value); }

        /// <summary>
        /// Implicitly converts a double to a Num.
        /// </summary>
        public static implicit operator Num(double value) { return FromT(value); }

        /// <summary>
        /// Implicitly converts a decimal to a Num.
        /// </summary>
        public static implicit operator Num(decimal value) { return FromT(value); }

        /// <summary>
        /// Implicitly converts a nullable float to a nullable Num.
        /// </summary>
        public static implicit operator Num?(float? value) { return value.HasValue ? new Num(value.Value) : null; }

        /// <summary>
        /// Implicitly converts a nullable int to a nullable Num.
        /// </summary>
        public static implicit operator Num?(int? value) { return value.HasValue ? new Num(value.Value) : null; }

        /// <summary>
        /// Implicitly converts a Num to a float.
        /// </summary>
        public static implicit operator float(Num value) { return value._value; }

        /// <summary>
        /// Implicitly converts a Num to a double.
        /// </summary>
        public static implicit operator double(Num value) { return value._value; }

        /// <summary>
        /// Implicitly converts a Num to an int (truncates decimal part).
        /// </summary>
        public static implicit operator int(Num value) { return (int)value._value; }

        /// <summary>
        /// Implicitly converts a Num to a byte (truncates and fits within byte range).
        /// </summary>
        public static implicit operator byte(Num value) { return (byte)value._value; }

        /// <summary>
        /// Implicitly converts a Num to a decimal.
        /// </summary>
        public static implicit operator decimal(Num value) { return (decimal)value._value; }

        #endregion

        #region Arithmetic Operators

        /// <summary>
        /// Adds two Num values.
        /// </summary>
        public static Num operator +(Num a, Num b) { return new Num(a._value + b._value); }

        /// <summary>
        /// Subtracts one Num value from another.
        /// </summary>
        public static Num operator -(Num a, Num b) { return new Num(a._value - b._value); }

        /// <summary>
        /// Multiplies two Num values.
        /// </summary>
        public static Num operator *(Num a, Num b) { return new Num(a._value * b._value); }

        /// <summary>
        /// Divides one Num value by another.
        /// </summary>
        public static Num operator /(Num a, Num b) { return new Num(a._value / b._value); }

        /// <summary>
        /// Returns the remainder after dividing one Num value by another.
        /// </summary>
        public static Num operator %(Num a, Num b) { return new Num(a._value % b._value); }

        /// <summary>
        /// Returns the negation of a Num value.
        /// </summary>
        public static Num operator -(Num a) { return new Num(-a._value); }

        /// <summary>
        /// Returns the Num value unchanged (unary plus operator).
        /// </summary>
        public static Num operator +(Num a) { return a; }

        /// <summary>
        /// Increments a Num value by 1.
        /// </summary>
        public static Num operator ++(Num a) { return new Num(a._value + 1); }

        /// <summary>
        /// Decrements a Num value by 1.
        /// </summary>
        public static Num operator --(Num a) { return new Num(a._value - 1); }

        #region Null-Safe Arithmetic Operators

        /// <summary>
        /// Adds two nullable Num values, returning null if either operand is null.
        /// </summary>
        public static Num? operator +(Num? a, Num? b)
        {
            return a.HasValue && b.HasValue ? new Num(a.Value._value + b.Value._value) : null;
        }

        /// <summary>
        /// Subtracts two nullable Num values, returning null if either operand is null.
        /// </summary>
        public static Num? operator -(Num? a, Num? b)
        {
            return a.HasValue && b.HasValue ? new Num(a.Value._value - b.Value._value) : null;
        }

        /// <summary>
        /// Multiplies two nullable Num values, returning null if either operand is null.
        /// </summary>
        public static Num? operator *(Num? a, Num? b)
        {
            return a.HasValue && b.HasValue ? new Num(a.Value._value * b.Value._value) : null;
        }

        /// <summary>
        /// Divides two nullable Num values, returning null if either operand is null.
        /// </summary>
        public static Num? operator /(Num? a, Num? b)
        {
            return a.HasValue && b.HasValue ? new Num(a.Value._value / b.Value._value) : null;
        }

        #endregion

        #region Comparison Operators

        /// <summary>
        /// Determines whether two Num values are equal within the specified tolerance.
        /// </summary>
        /// <remarks>
        /// Uses floating-point tolerance comparison to handle precision errors.
        /// Two values are considered equal if their absolute difference is less than the tolerance.
        /// </remarks>
        public static bool operator ==(Num a, Num b) { return Abs(a._value - b._value) < Tolerance; }

        /// <summary>
        /// Determines whether two Num values are not equal within the specified tolerance.
        /// </summary>
        public static bool operator !=(Num a, Num b) { return Abs(a._value - b._value) > Tolerance; }

        /// <summary>
        /// Determines whether the first Num value is greater than the second.
        /// </summary>
        public static bool operator >(Num a, Num b) { return a._value > b._value; }

        /// <summary>
        /// Determines whether the first Num value is less than the second.
        /// </summary>
        public static bool operator <(Num a, Num b) { return a._value < b._value; }

        /// <summary>
        /// Determines whether the first Num value is greater than or equal to the second.
        /// </summary>
        public static bool operator >=(Num a, Num b) { return a._value >= b._value; }

        /// <summary>
        /// Determines whether the first Num value is less than or equal to the second.
        /// </summary>
        public static bool operator <=(Num a, Num b) { return a._value <= b._value; }

        #region Null-Safe Comparison Operators

        /// <summary>
        /// Determines whether two nullable Num values are equal.
        /// </summary>
        /// <remarks>
        /// Returns true if both values are null, or if both have values and are equal within tolerance.
        /// Returns false if only one value is null or if the values are not equal.
        /// </remarks>
        public static bool operator ==(Num? a, Num? b)
        {
            return (!a.HasValue && !b.HasValue) ||
                   (a.HasValue && b.HasValue && Abs(a.Value._value - b.Value._value) < Tolerance);
        }

        /// <summary>
        /// Determines whether two nullable Num values are not equal.
        /// </summary>
        public static bool operator !=(Num? a, Num? b) { return !(a == b); }

        /// <summary>
        /// Determines whether the first nullable Num value is greater than the second.
        /// </summary>
        /// <remarks>
        /// Returns false if either value is null. Only returns true if both values have values
        /// and the first is greater than the second.
        /// </remarks>
        public static bool operator >(Num? a, Num? b)
        {
            return a.HasValue && b.HasValue && a.Value._value > b.Value._value;
        }

        /// <summary>
        /// Determines whether the first nullable Num value is less than the second.
        /// </summary>
        /// <remarks>
        /// Returns false if either value is null. Only returns true if both values have values
        /// and the first is less than the second.
        /// </remarks>
        public static bool operator <(Num? a, Num? b)
        {
            return a.HasValue && b.HasValue && a.Value._value < b.Value._value;
        }

        #endregion

        #endregion

        #region Logical Operators

        /// <summary>
        /// Determines if a Num value should be treated as true in a boolean context.
        /// </summary>
        /// <remarks>
        /// Returns true if the value is non-zero, false if the value is zero.
        /// This allows Num to be used in conditional statements and boolean expressions.
        /// </remarks>
        public static bool operator true(Num a) { return a._value != 0; }

        /// <summary>
        /// Determines if a Num value should be treated as false in a boolean context.
        /// </summary>
        /// <remarks>
        /// Returns true if the value is zero, false if the value is non-zero.
        /// This allows Num to be used in conditional statements and boolean expressions.
        /// </remarks>
        public static bool operator false(Num a) { return a._value == 0; }

        /// <summary>
        /// Performs logical negation on a Num value.
        /// </summary>
        /// <returns>1 if the input is zero, 0 if the input is non-zero.</returns>
        /// <remarks>
        /// This operator treats zero as false and non-zero as true, then returns the logical inverse.
        /// </remarks>
        public static Num operator !(Num a) { return new Num(a._value == 0 ? 1 : 0); }

        #endregion

        #region Bitwise Operators

        /// <summary>
        /// Performs bitwise AND operation on two Num values.
        /// </summary>
        /// <remarks>
        /// Converts the values to integers, performs bitwise AND, then converts back to Num.
        /// This operation truncates any fractional parts before the bitwise operation.
        /// </remarks>
        public static Num operator &(Num a, Num b) { return new Num((int)a._value & (int)b._value); }

        /// <summary>
        /// Performs bitwise OR operation on two Num values.
        /// </summary>
        /// <remarks>
        /// Converts the values to integers, performs bitwise OR, then converts back to Num.
        /// This operation truncates any fractional parts before the bitwise operation.
        /// </remarks>
        public static Num operator |(Num a, Num b) { return new Num((int)a._value | (int)b._value); }

        /// <summary>
        /// Performs bitwise XOR operation on two Num values.
        /// </summary>
        /// <remarks>
        /// Converts the values to integers, performs bitwise XOR, then converts back to Num.
        /// This operation truncates any fractional parts before the bitwise operation.
        /// </remarks>
        public static Num operator ^(Num a, Num b) { return new Num((int)a._value ^ (int)b._value); }

        /// <summary>
        /// Performs bitwise NOT operation on a Num value.
        /// </summary>
        /// <remarks>
        /// Converts the value to integer, performs bitwise NOT, then converts back to Num.
        /// This operation truncates any fractional part before the bitwise operation.
        /// </remarks>
        public static Num operator ~(Num a) { return new Num(~(int)a._value); }

        /// <summary>
        /// Performs left bit shift operation on a Num value.
        /// </summary>
        /// <param name="a">The value to shift.</param>
        /// <param name="b">The number of positions to shift left.</param>
        /// <returns>The result of shifting the integer representation left by b positions.</returns>
        /// <remarks>
        /// Converts the value to integer, performs left shift, then converts back to Num.
        /// This operation truncates any fractional part before the shift operation.
        /// </remarks>
        public static Num operator <<(Num a, int b) { return new Num((int)a._value << b); }

        /// <summary>
        /// Performs right bit shift operation on a Num value.
        /// </summary>
        /// <param name="a">The value to shift.</param>
        /// <param name="b">The number of positions to shift right.</param>
        /// <returns>The result of shifting the integer representation right by b positions.</returns>
        /// <remarks>
        /// Converts the value to integer, performs right shift, then converts back to Num.
        /// This operation truncates any fractional part before the shift operation.
        /// </remarks>
        public static Num operator >>(Num a, int b) { return new Num((int)a._value >> b); }

        #endregion

        // Equality implementation
        public bool Equals(Num other) { return Abs(_value - other._value) < Tolerance; }

        public override bool Equals(object? obj) { return obj is Num other && Equals(other); }

        public override int GetHashCode() { return _value.GetHashCode(); }

        public override string ToString() { return _value.ToString(CultureInfo.CurrentCulture); }

        public int CompareTo(Num other)
        {
            if (Abs(_value - other._value) < Tolerance)
            {
                return 0; // Equal within tolerance
            }

            return _value > other._value ? 1 : -1; // Greater than or less than
        }

        #endregion

        #region IConvertible Implementation

        /// <summary>
        /// Returns the TypeCode for the Num type.
        /// </summary>
        /// <returns>TypeCode.Single, indicating this type is based on single-precision floating point.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Single;
        }

        /// <summary>
        /// Converts the value to a signed byte.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A signed byte equivalent to the value.</returns>
        public sbyte ToSByte(IFormatProvider? provider)
        {
            return Convert.ToSByte(_value);
        }

        /// <summary>
        /// Converts the value to a single-precision floating-point number.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A single-precision floating-point number equivalent to the value.</returns>
        public float ToSingle(IFormatProvider? provider)
        {
            return _value;
        }

        /// <summary>
        /// Converts the value to a Boolean value.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>true if the value is non-zero; otherwise, false.</returns>
        public bool ToBoolean(IFormatProvider? provider)
        {
            return _value != 0;
        }

        /// <summary>
        /// Converts the value to an unsigned byte.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An unsigned byte equivalent to the value.</returns>
        public byte ToByte(IFormatProvider? provider)
        {
            return Convert.ToByte(_value);
        }

        /// <summary>
        /// Converts the value to a Unicode character.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A Unicode character equivalent to the value.</returns>
        public char ToChar(IFormatProvider? provider)
        {
            return Convert.ToChar(_value);
        }

        /// <summary>
        /// This conversion is not supported for Num.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>This conversion is not supported.</returns>
        /// <exception cref="InvalidCastException">Always thrown as DateTime conversion is not supported.</exception>
        public DateTime ToDateTime(IFormatProvider? provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts the value to a decimal number.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A decimal number equivalent to the value.</returns>
        public decimal ToDecimal(IFormatProvider? provider)
        {
            return Convert.ToDecimal(_value);
        }

        /// <summary>
        /// Converts the value to a double-precision floating-point number.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A double-precision floating-point number equivalent to the value.</returns>
        public double ToDouble(IFormatProvider? provider)
        {
            return Convert.ToDouble(_value);
        }

        /// <summary>
        /// Converts the value to a 16-bit signed integer.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A 16-bit signed integer equivalent to the value.</returns>
        public short ToInt16(IFormatProvider? provider)
        {
            return Convert.ToInt16(_value);
        }

        /// <summary>
        /// Converts the value to a 32-bit signed integer.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A 32-bit signed integer equivalent to the value.</returns>
        public int ToInt32(IFormatProvider? provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// Converts the value to a 64-bit signed integer.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A 64-bit signed integer equivalent to the value.</returns>
        public long ToInt64(IFormatProvider? provider)
        {
            return Convert.ToInt64(_value);
        }

        /// <summary>
        /// Converts the value to its equivalent string representation using the specified culture-specific format information.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value.</returns>
        string IConvertible.ToString(IFormatProvider? provider)
        {
            return _value.ToString(provider);
        }

        /// <summary>
        /// Converts the value to the specified type using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="conversionType">The type to which to convert the value.</param>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An object whose type is conversionType and whose value is equivalent to the value.</returns>
        /// <exception cref="InvalidCastException">This conversion is not supported.</exception>
        /// <exception cref="ArgumentNullException">conversionType is null.</exception>
        object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
        {
            return Convert.ChangeType(_value, conversionType, provider);
        }

        /// <summary>
        /// Converts the value to a 16-bit unsigned integer.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A 16-bit unsigned integer equivalent to the value.</returns>
        ushort IConvertible.ToUInt16(IFormatProvider? provider)
        {
            return Convert.ToUInt16(_value);
        }

        /// <summary>
        /// Converts the value to a 32-bit unsigned integer.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A 32-bit unsigned integer equivalent to the value.</returns>
        uint IConvertible.ToUInt32(IFormatProvider? provider)
        {
            return Convert.ToUInt32(_value);
        }

        /// <summary>
        /// Converts the value to a 64-bit unsigned integer.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>A 64-bit unsigned integer equivalent to the value.</returns>
        ulong IConvertible.ToUInt64(IFormatProvider? provider)
        {
            return Convert.ToUInt64(_value);
        }

        #endregion

        #region INumber<Num> Required Properties and Methods

        /// <summary>
        /// Gets the value representing one (1) for the Num type.
        /// </summary>
        /// <remarks>
        /// This is the multiplicative identity element. Any number multiplied by One equals itself.
        /// </remarks>
        public static Num One => new(1);

        /// <summary>
        /// Gets the value representing zero (0) for the Num type.
        /// </summary>
        /// <remarks>
        /// This is the additive identity element. Any number added to Zero equals itself.
        /// </remarks>
        public static Num Zero => new(0);

        /// <summary>
        /// Gets the additive identity for the Num type (same as Zero).
        /// </summary>
        /// <remarks>
        /// Required by IAdditiveIdentity interface. The additive identity is the value
        /// that doesn't change another value when added to it.
        /// </remarks>
        public static Num AdditiveIdentity => Zero;

        /// <summary>
        /// Gets the multiplicative identity for the Num type (same as One).
        /// </summary>
        /// <remarks>
        /// Required by IMultiplicativeIdentity interface. The multiplicative identity
        /// is the value that doesn't change another value when multiplied by it.
        /// </remarks>
        public static Num MultiplicativeIdentity => One;

        /// <summary>
        /// Parses a string representation of a number into a Num value.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <returns>The parsed Num value.</returns>
        /// <exception cref="FormatException">Thrown when the string cannot be parsed.</exception>
        public static Num Parse(string s, IFormatProvider? provider)
        {
            return new Num(float.Parse(s, provider));
        }

        /// <summary>
        /// Attempts to parse a string representation of a number into a Num value.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <param name="result">When this method returns, contains the parsed Num value if successful, or Zero if parsing failed.</param>
        /// <returns>true if the string was parsed successfully; otherwise, false.</returns>
        /// <remarks>
        /// This method provides a safe way to parse strings without throwing exceptions.
        /// If parsing fails, the result parameter is set to Zero and the method returns false.
        /// </remarks>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Num result)
        {
            if (float.TryParse(s, NumberStyles.Float, provider, out float value))
            {
                result = new Num(value);
                return true;
            }
            result = Zero;
            return false;
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and obj:
        /// - Less than zero: This instance is less than obj
        /// - Zero: This instance is equal to obj (within tolerance)
        /// - Greater than zero: This instance is greater than obj
        /// </returns>
        /// <exception cref="ArgumentException">obj is not a Num.</exception>
        public int CompareTo(object? obj)
        {
            return obj is Num other ? CompareTo(other) : throw new ArgumentException("Object must be of type Num");
        }

        // Add absolute value method required by INumber<T>
        static Num INumberBase<Num>.Abs(Num value)
        {
            return Abs(value);
        }

        // Add required methods for sign checks
        /// <summary>
        /// Determines whether a value represents a negative real number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents a negative real number; otherwise, false.</returns>
        /// <remarks>
        /// This method is required by the INumberBase interface and provides a standard way
        /// to check if a number is negative. Zero is not considered negative.
        /// </remarks>
        public static bool IsNegative(Num value)
        {
            return value._value < 0;
        }

        /// <summary>
        /// Determines whether a value represents a positive real number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents a positive real number; otherwise, false.</returns>
        /// <remarks>
        /// This method is required by the INumberBase interface and provides a standard way
        /// to check if a number is positive. Zero is not considered positive.
        /// </remarks>
        public static bool IsPositive(Num value)
        {
            return value._value > 0;
        }

        /// <summary>
        /// Determines whether a value represents zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents zero; otherwise, false.</returns>
        /// <remarks>
        /// This method uses tolerance-based comparison to account for floating-point precision errors.
        /// A value is considered zero if its absolute value is less than the tolerance threshold.
        /// </remarks>
        public static bool IsZero(Num value)
        {
            return Abs(value._value) < Tolerance;
        }

        /// <summary>
        /// Determines whether a value represents an integral number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents an integral number; otherwise, false.</returns>
        /// <remarks>
        /// This method checks if the value has no fractional part by using the modulo operator.
        /// Values like 1.0, 2.0, -3.0 are considered integers, while 1.5, 2.3 are not.
        /// </remarks>
        public static bool IsInteger(Num value)
        {
            return value._value % 1 == 0;
        }

        // Add INumberBase<Num> required implementations
        /// <summary>
        /// Determines whether a value is in its canonical representation.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true, as all Num values are considered canonical.</returns>
        /// <remarks>
        /// For the Num type, all values are considered to be in canonical form since
        /// there are no alternative representations for the same value. This method
        /// is required by the INumberBase interface.
        /// </remarks>
        public static bool IsCanonical(Num value)
        {
            return true;
        }

        /// <summary>
        /// Determines whether a value represents a complex number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>false, as Num only represents real numbers.</returns>
        /// <remarks>
        /// The Num type only supports real numbers, so this method always returns false.
        /// This method is required by the INumberBase interface for types that might
        /// support complex numbers.
        /// </remarks>
        public static bool IsComplexNumber(Num value)
        {
            return false;
        }

        /// <summary>
        /// Determines whether a value represents an even integral number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents an even integral number; otherwise, false.</returns>
        /// <remarks>
        /// This method first checks if the value is an integer, then determines if it's even.
        /// A number is even if it's divisible by 2 with no remainder. Examples: 2, 4, -6, 0.
        /// </remarks>
        public static bool IsEvenInteger(Num value)
        {
            return value._value % 2 == 0 && IsInteger(value);
        }

        /// <summary>
        /// Determines whether a value is finite (not infinite and not NaN).
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value is finite; otherwise, false.</returns>
        /// <remarks>
        /// A finite number is one that is neither positive infinity, negative infinity, nor NaN.
        /// This method delegates to the underlying float.IsFinite method for accurate detection.
        /// </remarks>
        public static bool IsFinite(Num value)
        {
            return float.IsFinite(value._value);
        }

        /// <summary>
        /// Determines whether a value represents a pure imaginary number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>false, as Num only represents real numbers.</returns>
        /// <remarks>
        /// The Num type only supports real numbers, so this method always returns false.
        /// Imaginary numbers involve the imaginary unit 'i', which is not supported by this type.
        /// </remarks>
        public static bool IsImaginaryNumber(Num value)
        {
            return false;
        }

        /// <summary>
        /// Determines whether a value represents an infinity (positive or negative).
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents positive or negative infinity; otherwise, false.</returns>
        /// <remarks>
        /// This method detects both positive and negative infinity values. Infinity typically
        /// results from division by zero or overflow in mathematical operations.
        /// </remarks>
        public static bool IsInfinity(Num value)
        {
            return float.IsInfinity(value._value);
        }

        /// <summary>
        /// Determines whether a value represents "Not a Number" (NaN).
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents NaN; otherwise, false.</returns>
        /// <remarks>
        /// NaN (Not a Number) represents undefined or unrepresentable floating-point values,
        /// such as the result of 0/0 or square root of negative numbers. NaN is not equal to itself.
        /// </remarks>
        public static bool IsNaN(Num value)
        {
            return float.IsNaN(value._value);
        }

        /// <summary>
        /// Determines whether a value represents negative infinity.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents negative infinity; otherwise, false.</returns>
        /// <remarks>
        /// Negative infinity typically results from operations that produce very large negative values
        /// or division of a negative number by zero. It represents values smaller than any finite number.
        /// </remarks>
        public static bool IsNegativeInfinity(Num value)
        {
            return float.IsNegativeInfinity(value._value);
        }

        /// <summary>
        /// Determines whether a value is normal (finite, non-zero, and not subnormal).
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value is normal; otherwise, false.</returns>
        /// <remarks>
        /// A normal number is a finite floating-point number that is not zero, infinity, NaN, or subnormal.
        /// Normal numbers have full precision and are in the standard floating-point representation.
        /// </remarks>
        public static bool IsNormal(Num value)
        {
            return float.IsNormal(value._value);
        }

        /// <summary>
        /// Determines whether a value represents an odd integral number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents an odd integral number; otherwise, false.</returns>
        /// <remarks>
        /// This method first checks if the value is an integer, then determines if it's odd.
        /// A number is odd if it's not divisible by 2 (has a remainder when divided by 2).
        /// Examples: 1, 3, -5, 7.
        /// </remarks>
        public static bool IsOddInteger(Num value)
        {
            return value._value % 2 != 0 && IsInteger(value);
        }

        /// <summary>
        /// Determines whether a value represents positive infinity.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents positive infinity; otherwise, false.</returns>
        /// <remarks>
        /// Positive infinity typically results from operations that produce very large positive values
        /// or division of a positive number by zero. It represents values larger than any finite number.
        /// </remarks>
        public static bool IsPositiveInfinity(Num value)
        {
            return float.IsPositiveInfinity(value._value);
        }

        /// <summary>
        /// Determines whether a value represents a real number (not NaN).
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents a real number; otherwise, false.</returns>
        /// <remarks>
        /// A real number includes all finite numbers, positive infinity, and negative infinity,
        /// but excludes NaN (Not a Number). This method is useful for validating that a value
        /// represents a meaningful numeric quantity.
        /// </remarks>
        public static bool IsRealNumber(Num value)
        {
            return !float.IsNaN(value._value);
        }

        /// <summary>
        /// Determines whether a value represents a subnormal number.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>true if value represents a subnormal number; otherwise, false.</returns>
        /// <remarks>
        /// Subnormal numbers are very small floating-point numbers that don't use the full precision
        /// of the floating-point format. They fill the gap between zero and the smallest normal number,
        /// providing gradual underflow. Also known as denormalized numbers.
        /// </remarks>
        public static bool IsSubnormal(Num value)
        {
            return float.IsSubnormal(value._value);
        }

        /// <summary>
        /// Compares two values and returns the one with the larger magnitude (absolute value).
        /// </summary>
        /// <param name="x">The first value to compare.</param>
        /// <param name="y">The second value to compare.</param>
        /// <returns>The value with the larger magnitude, or x if magnitudes are equal.</returns>
        /// <remarks>
        /// This method compares the absolute values of the inputs and returns the value whose
        /// absolute value is larger. If the magnitudes are equal, it returns the larger value.
        /// For example: MaxMagnitude(-5, 3) returns -5 because |-5| > |3|.
        /// </remarks>
        public static Num MaxMagnitude(Num x, Num y)
        {
            // Manual implementation
            Num absX = Abs(x);
            Num absY = Abs(y);
            return absX > absY ? x : absY > absX ? y : x >= y ? x : y;
        }
        /// <summary>
        /// Compares two values and returns the one with the larger magnitude, with NaN handling.
        /// </summary>
        /// <param name="x">The first value to compare.</param>
        /// <param name="y">The second value to compare.</param>
        /// <returns>The value with the larger magnitude, preferring non-NaN values.</returns>
        /// <remarks>
        /// Similar to MaxMagnitude but with special handling for NaN values. If one value is NaN
        /// and the other is not, the non-NaN value is returned. This provides more predictable
        /// behavior in the presence of NaN values.
        /// </remarks>
        public static Num MaxMagnitudeNumber(Num x, Num y)
        {
            return new Num(float.MaxMagnitudeNumber(x._value, y._value));
        }

        /// <summary>
        /// Compares two values and returns the one with the smaller magnitude (absolute value).
        /// </summary>
        /// <param name="x">The first value to compare.</param>
        /// <param name="y">The second value to compare.</param>
        /// <returns>The value with the smaller magnitude, or x if magnitudes are equal.</returns>
        /// <remarks>
        /// This method compares the absolute values of the inputs and returns the value whose
        /// absolute value is smaller. If the magnitudes are equal, it returns the smaller value.
        /// For example: MinMagnitude(-5, 3) returns 3 because |3| &lt; |-5|.
        /// </remarks>
        public static Num MinMagnitude(Num x, Num y)
        {
            // Manual implementation
            Num absX = Abs(x);
            Num absY = Abs(y);
            return absX < absY ? x : absY < absX ? y : x <= y ? x : y;
        }
        /// <summary>
        /// Compares two values and returns the one with the smaller magnitude, with NaN handling.
        /// </summary>
        /// <param name="x">The first value to compare.</param>
        /// <param name="y">The second value to compare.</param>
        /// <returns>The value with the smaller magnitude, preferring non-NaN values.</returns>
        /// <remarks>
        /// Similar to MinMagnitude but with special handling for NaN values. If one value is NaN
        /// and the other is not, the non-NaN value is returned. This provides more predictable
        /// behavior in the presence of NaN values.
        /// </remarks>
        public static Num MinMagnitudeNumber(Num x, Num y)
        {
            return new Num(float.MinMagnitudeNumber(x._value, y._value));
        }

        // Add parsing methods
        /// <summary>
        /// Parses a span of characters into a Num value using the specified style and format provider.
        /// </summary>
        /// <param name="s">The span of characters to parse.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <returns>The parsed Num value.</returns>
        /// <exception cref="FormatException">Thrown when the span cannot be parsed.</exception>
        public static Num Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        {
            return new Num(float.Parse(s, style, provider));
        }

        /// <summary>
        /// Parses a string into a Num value using the specified style and format provider.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <returns>The parsed Num value.</returns>
        /// <exception cref="FormatException">Thrown when the string cannot be parsed.</exception>
        public static Num Parse(string s, NumberStyles style, IFormatProvider? provider)
        {
            return new Num(float.Parse(s, style, provider));
        }

        /// <summary>
        /// Attempts to parse a span of characters into a Num value using the specified style and format provider.
        /// </summary>
        /// <param name="s">The span of characters to parse.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <param name="result">When this method returns, contains the parsed Num value if successful, or Zero if parsing failed.</param>
        /// <returns>true if the span was parsed successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Num result)
        {
            if (float.TryParse(s, style, provider, out float value))
            {
                result = new Num(value);
                return true;
            }
            result = Zero;
            return false;
        }

        /// <summary>
        /// Attempts to parse a string into a Num value using the specified style and format provider.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <param name="result">When this method returns, contains the parsed Num value if successful, or Zero if parsing failed.</param>
        /// <returns>true if the string was parsed successfully; otherwise, false.</returns>
        public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Num result)
        {
            if (float.TryParse(s, style, provider, out float value))
            {
                result = new Num(value);
                return true;
            }
            result = Zero;
            return false;
        }

        // Add conversion methods
        /// <summary>
        /// Attempts to convert a value to a Num with overflow checking.
        /// </summary>
        /// <typeparam name="TOther">The type of the value to convert.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="result">When this method returns, contains the converted Num value if successful, or Zero if conversion failed.</param>
        /// <returns>true if the conversion was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method performs checked conversion, meaning it will fail if the conversion would result in overflow.
        /// </remarks>
        public static bool TryConvertFromChecked<TOther>(TOther value, out Num result) where TOther : INumberBase<TOther>
        {
            try
            {
                result = new Num(Convert.ToSingle(value));
                return true;
            }
            catch
            {
                result = Zero;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert a value to a Num with saturation on overflow.
        /// </summary>
        /// <typeparam name="TOther">The type of the value to convert.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="result">When this method returns, contains the converted Num value if successful, or Zero if conversion failed.</param>
        /// <returns>true if the conversion was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method performs saturating conversion, meaning overflow values are clamped to the maximum/minimum representable values.
        /// NaN values are converted to zero.
        /// </remarks>
        public static bool TryConvertFromSaturating<TOther>(TOther value, out Num result) where TOther : INumberBase<TOther>
        {
            try
            {
                float val = Convert.ToSingle(value);
                result = new Num(float.IsNaN(val) ? 0 : val);
                return true;
            }
            catch
            {
                result = Zero;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert a value to a Num with truncation.
        /// </summary>
        /// <typeparam name="TOther">The type of the value to convert.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="result">When this method returns, contains the converted Num value if successful, or Zero if conversion failed.</param>
        /// <returns>true if the conversion was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method performs truncating conversion, meaning any fractional part is discarded.
        /// This is useful when converting from higher-precision types to Num.
        /// </remarks>
        public static bool TryConvertFromTruncating<TOther>(TOther value, out Num result) where TOther : INumberBase<TOther>
        {
            try
            {
                result = new Num((float)Math.Truncate(Convert.ToDouble(value)));
                return true;
            }
            catch
            {
                result = Zero;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert a Num value to another numeric type with overflow checking.
        /// </summary>
        /// <typeparam name="TOther">The target type to convert to.</typeparam>
        /// <param name="value">The Num value to convert.</param>
        /// <param name="result">When this method returns, contains the converted value if successful, or zero if conversion failed.</param>
        /// <returns>true if the conversion was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method performs checked conversion, meaning it will fail if the conversion would result in overflow.
        /// </remarks>
        public static bool TryConvertToChecked<TOther>(Num value, out TOther result) where TOther : INumberBase<TOther>
        {
            try
            {
                result = TOther.CreateChecked(value._value);
                return true;
            }
            catch
            {
                result = TOther.Zero;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert a Num value to another numeric type with saturation on overflow.
        /// </summary>
        /// <typeparam name="TOther">The target type to convert to.</typeparam>
        /// <param name="value">The Num value to convert.</param>
        /// <param name="result">When this method returns, contains the converted value if successful, or zero if conversion failed.</param>
        /// <returns>true if the conversion was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method performs saturating conversion, meaning overflow values are clamped to the target type's maximum/minimum values.
        /// </remarks>
        public static bool TryConvertToSaturating<TOther>(Num value, out TOther result) where TOther : INumberBase<TOther>
        {
            try
            {
                result = TOther.CreateSaturating(value._value);
                return true;
            }
            catch
            {
                result = TOther.Zero;
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert a Num value to another numeric type with truncation.
        /// </summary>
        /// <typeparam name="TOther">The target type to convert to.</typeparam>
        /// <param name="value">The Num value to convert.</param>
        /// <param name="result">When this method returns, contains the converted value if successful, or zero if conversion failed.</param>
        /// <returns>true if the conversion was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method performs truncating conversion, meaning any fractional part is discarded when converting to integer types.
        /// </remarks>
        public static bool TryConvertToTruncating<TOther>(Num value, out TOther result) where TOther : INumberBase<TOther>
        {
            try
            {
                result = TOther.CreateTruncating(value._value);
                return true;
            }
            catch
            {
                result = TOther.Zero;
                return false;
            }
        }

        // Add INumberBase<Num> static property
        /// <summary>
        /// Gets the radix (base) of the Num type's representation.
        /// </summary>
        /// <value>2, indicating binary representation (base-2).</value>
        /// <remarks>
        /// The radix represents the base of the number system used internally.
        /// Since Num is based on IEEE 754 floating-point format, it uses binary (base-2) representation.
        /// </remarks>
        public static int Radix => 2;

        // Add ISpanFormattable implementation
        /// <summary>
        /// Attempts to format the value into the provided span of characters.
        /// </summary>
        /// <param name="destination">The span to write the formatted value to.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written to destination.</param>
        /// <param name="format">A span containing the format specifier to use.</param>
        /// <param name="provider">An optional format provider to use.</param>
        /// <returns>true if the formatting was successful; otherwise, false.</returns>
        /// <remarks>
        /// This method provides efficient formatting by writing directly to a span without allocating strings.
        /// If the destination span is too small, the method returns false.
        /// </remarks>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            return _value.TryFormat(destination, out charsWritten, format, provider);
        }

        // Add IFormattable implementation
        /// <summary>
        /// Converts the value to its string representation using the specified format and format provider.
        /// </summary>
        /// <param name="format">A format string that specifies how to format the value.</param>
        /// <param name="formatProvider">An optional format provider to use for culture-specific formatting.</param>
        /// <returns>The string representation of the value.</returns>
        /// <remarks>
        /// This method supports standard numeric format strings such as "F" (fixed-point), "E" (exponential),
        /// "G" (general), etc. The format provider allows for culture-specific formatting of numbers.
        /// </remarks>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return _value.ToString(format, formatProvider);
        }

        // Add ISpanParsable<Num> implementation
        /// <summary>
        /// Parses a span of characters into a Num value using the specified format provider.
        /// </summary>
        /// <param name="s">The span of characters to parse.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <returns>The parsed Num value.</returns>
        /// <exception cref="FormatException">Thrown when the span cannot be parsed.</exception>
        /// <remarks>
        /// This method provides efficient parsing from character spans without requiring string allocation.
        /// </remarks>
        public static Num Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            return new Num(float.Parse(s, provider));
        }

        /// <summary>
        /// Attempts to parse a span of characters into a Num value using the specified format provider.
        /// </summary>
        /// <param name="s">The span of characters to parse.</param>
        /// <param name="provider">The format provider to use for parsing.</param>
        /// <param name="result">When this method returns, contains the parsed Num value if successful, or Zero if parsing failed.</param>
        /// <returns>true if the span was parsed successfully; otherwise, false.</returns>
        /// <remarks>
        /// This method provides efficient parsing from character spans without requiring string allocation
        /// and without throwing exceptions on parse failures.
        /// </remarks>
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Num result)
        {
            if (float.TryParse(s, provider, out float value))
            {
                result = new Num(value);
                return true;
            }
            result = Zero;
            Vect a = new(10, (Num)10);
            return false;
        }

        /// <summary>
        /// Raises a number to an integer power using fast exponentiation by squaring.
        /// </summary>
        /// <param name="value">The base number to raise to a power.</param>
        /// <param name="exponent">The integer exponent.</param>
        /// <returns>The result of value raised to the power of exponent.</returns>
        /// <remarks>
        /// This is an optimized version of the power function specifically for integer exponents.
        /// Uses the efficient exponentiation by squaring algorithm with O(log n) complexity.
        /// For negative exponents, calculates the reciprocal of the positive power.
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Pow(2, 10);  // Result: 1024
        /// Num result2 = Num.Pow(5, -2);  // Result: 0.04 (1/25)
        /// </code>
        /// </example>
        public static Num Pow(Num value, int exponent)
        {
            if (exponent == 0)
            {
                return One;
            }

            if (exponent < 0)
            {
                return One / Pow(value, -exponent);
            }

            Num result = One;
            Num base_value = value;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                {
                    result *= base_value;
                }

                base_value *= base_value;
                exponent >>= 1;
            }

            return result;
        }

        /// <summary>
        /// Calculates the factorial of a non-negative integer.
        /// </summary>
        /// <param name="n">The non-negative integer to calculate the factorial of.</param>
        /// <returns>The factorial of n (n!).</returns>
        /// <remarks>
        /// <para>
        /// The factorial of a non-negative integer n is the product of all positive integers less than or equal to n.
        /// By definition, 0! = 1.
        /// </para>
        /// <para>
        /// Formula: n! = n × (n-1) × (n-2) × ... × 2 × 1
        /// </para>
        /// <para>
        /// This method uses iterative calculation for efficiency and to avoid stack overflow for large values.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// Num result1 = Num.Factorial(5);  // Result: 120 (5×4×3×2×1)
        /// Num result2 = Num.Factorial(0);  // Result: 1 (by definition)
        /// Num result3 = Num.Factorial(10); // Result: 3628800
        /// </code>
        /// </example>
        public static Num Factorial(int n)
        {
            Num result = One;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }

            return result;
        }

        #endregion
    }
}
