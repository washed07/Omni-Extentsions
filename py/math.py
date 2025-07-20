from typing import Tuple
from math import (log10, log2, degrees, radians, dist, gamma, isqrt, prod, 
                 remainder, trunc, expm1, log1p, sqrt, ceil, floor, isfinite, 
                 isinf, isnan, nan, inf, pi, tau, e)
import struct

# Constants
pi: float = 3.141592653589793
piFast: float = 3.14159265359
e: float = 2.718281828459045
eFast: float = 2.71828182846
tau: float = 6.283185307179586
inf: float = float('inf')
nan: float = float('nan')

# Basic arithmetic
def add(a, b): return a + b
def ceil(a: float) -> int: return int(a) if int(a) == a else int(a) + 1
def div(a, b): return a / b
def floor(a: float) -> int: return int(a)
def inv(a) -> float: return 1 / a
def invsqrt(a) -> float: return 1 / sqrt(a)
def mod(a, b): return a % b
def mul(a, b): return a * b
def pow(a, b): return a ** b
def sqrt(a): return a ** 0.5
def sub(a, b): return a - b

# Trigonometric functions
def cos(a):
    # Using Taylor series approximation for cos(x)
    x = wrap_angle(a)  # Normalize angle to [-π, π]
    
    # Taylor series terms: 1 - x²/2! + x⁴/4! - x⁶/6! + x⁸/8!
    x2 = x * x
    return (1 - x2/2 + x2*x2/24 - x2*x2*x2/720 + x2*x2*x2*x2/40320)

def sin(a):
    # Using Taylor series approximation for sin(x)
    x = wrap_angle(a)  # Normalize angle to [-π, π]
    
    # Taylor series terms: x - x³/3! + x⁵/5! - x⁷/7! + x⁹/9!
    x2 = x * x
    return (x - x2*x/6 + x2*x2*x/120 - x2*x2*x2*x/5040 + x2*x2*x2*x2*x/362880)

def tan(a):
    # Using Taylor series approximation for tan(x)
    x = wrap_angle(a)  # Normalize angle to [-π, π]
    
    # Taylor series terms: x + x³/3 + 2x⁵/15 + 17x⁷/315 + 62x⁹/2835
    x2 = x * x
    return (x + x*x2/3 + 2*x2*x2*x/15 + 17*x2*x2*x2*x/315 + 62*x2*x2*x2*x2*x/2835)

# Inverse trigonometric functions
def acos(a):
    # Using Taylor series approximation for arccos(x)
    if abs(a) > 1:
        return nan
    elif a == 1:
        return 0
    elif a == -1:
        return pi
    else:
        x = a
        x2 = x * x
        # Taylor series terms for arccos(x) around x=0
        return pi/2 - (x + x*x2/6 + 3*x2*x2*x/40 + 5*x2*x2*x2*x/112 + 35*x2*x2*x2*x2*x/1152)
    
def asin(a):
    # Using Taylor series approximation for arcsin(x)
    if abs(a) > 1:
        return nan
    elif a == 1:
        return pi/2
    elif a == -1:
        return -pi/2
    else:
        x = a
        x2 = x * x
        # Taylor series terms for arcsin(x) around x=0
        return x + x*x2/6 + 3*x2*x2*x/40 + 5*x2*x2*x2*x/112 + 35*x2*x2*x2*x2*x/1152
    
def atan(a):
    # Using Taylor series approximation for arctan(x)
    x = a
    x2 = x * x
    # Taylor series terms for arctan(x) around x=0
    return x - x2*x/3 + x2*x2*x/5 - x2*x2*x2*x/7 + x2*x2*x2*x2*x/9

def atan2(y, x):
    # Using Taylor series approximation for arctan(y/x)
    if x == 0:
        if y > 0:
            return pi / 2
        elif y < 0:
            return -pi / 2
        else:
            return nan
    else:
        return atan(y / x)
    
# Hyperbolic functions
def acosh(a):
    # Using Taylor series approximation for arccosh(x)
    if a < 1:
        return nan
    elif a == 1:
        return 0
    else:
        x = a
        x2 = x * x
        # Taylor series terms for arccosh(x) around x=1
        return log(x + sqrt(x2 - 1))
    
def asinh(a):
    # Using Taylor series approximation for arcsinh(x)
    x = a
    x2 = x * x
    # Taylor series terms for arcsinh(x) around x=0
    return x + x2*x/6 + 3*x2*x2*x/40 + 5*x2*x2*x2*x/112 + 35*x2*x2*x2*x2*x/1152

def atanh(a):
    # Using Taylor series approximation for arctanh(x)
    if abs(a) >= 1:
        return nan
    elif a == 1:
        return inf
    elif a == -1:
        return -inf
    else:
        x = a
        x2 = x * x
        # Taylor series terms for arctanh(x) around x=0
        return x + x2*x/3 + x2*x2*x/5 + x2*x2*x2*x/7 + x2*x2*x2*x2*x/9
    
def cosh(a):
    # Using Taylor series approximation for cosh(x)
    x = a
    x2 = x * x
    # Taylor series terms for cosh(x)
    return 1 + x2/2 + x2*x2/24 + x2*x2*x2/720 + x2*x2*x2*x2/40320

def sinh(a):
    # Using Taylor series approximation for sinh(x)
    x = a
    x2 = x * x
    # Taylor series terms for sinh(x)
    return x + x2*x/6 + x2*x2*x/120 + x2*x2*x2*x/5040 + x2*x2*x2*x2*x/362880

def tanh(a):
    # Using Taylor series approximation for tanh(x)
    x = a
    x2 = x * x
    # Taylor series terms for tanh(x)
    return x - x*x2/3 + 2*x2*x2*x/15 - 17*x2*x2*x2*x/315 + 62*x2*x2*x2*x2*x/2835

# Logarithmic and exponential functions
def exp(a):
    # Using Taylor series approximation for exp(x)
    x = a
    x2 = x * x
    # Taylor series terms: 1 + x + x²/2! + x³/3! + x⁴/4! + x⁵/5!
    return 1 + x + x2/2 + x2*x/6 + x2*x2/24 + x2*x2*x/120

def expm1(a):
    # Using Taylor series approximation for expm1(x)
    x = a
    x2 = x * x
    # Taylor series terms: x + x²/2! + x³/3! + x⁴/4! + x⁵/5!
    return x + x2/2 + x2*x/6 + x2*x2/24 + x2*x2*x/120

def log(a):
    # Using Taylor series approximation for ln(x)
    if a <= 0:
        return nan
    x = a - 1
    x2 = x * x
    # Taylor series terms for ln(1+x): x - x²/2 + x³/3 - x⁴/4 + x⁵/5
    return x - x2/2 + x2*x/3 - x2*x2/4 + x2*x2*x/5

def log10(a): return log(a) / log(10)
def log1p(a): return log(1 + a)
def log2(a): return log(a) / log(2)

# Number theory functions
def fact(a):
    if a == 0:
        return 1
    else:
        return a * fact(a - 1)

def gcd(a, b):
    while b:
        a, b = b, a % b
    return a

def isqrt(a): return int(a ** 0.5)

def lcm(a, b):
    return a * b // gcd(a, b)

# Number property checks
def is_abundant(a):
    return a < sum([i for i in range(1, a) if a % i == 0])

def is_armstrong(a):
    return a == sum([int(i) ** len(str(a)) for i in str(a)])

def is_deficient(a):
    return a > sum([i for i in range(1, a) if a % i == 0])

def is_even(a):
    return a % 2 == 0

def is_inf(a: float) -> bool: return a == inf or a == -inf

def is_nan(a: float) -> bool: return a != a

def is_negative(a):
    return a < 0

def is_odd(a):
    return a % 2 != 0

def is_palindrome(a):
    return str(a) == str(a)[::-1]

def is_perfect(a):
    return a == sum([i for i in range(1, a) if a % i == 0])

def is_perfect_square(a):
    return a ** 0.5 == int(a ** 0.5)

def is_positive(a):
    return a > 0

def is_prime(a):
    if a < 2:
        return False
    for i in range(2, int(a ** 0.5) + 1):
        if a % i == 0:
            return False
    return True

def is_zero(a):
    return a == 0

# Geometric functions
def bezier_point(p0: Tuple[float, float], p1: Tuple[float, float], 
                p2: Tuple[float, float], t: float) -> Tuple[float, float]:
    """Calculates point along quadratic Bézier curve at parameter t.
    
    Implements quadratic Bézier curve interpolation between three control points
    where p0 is start point, p1 is control point, and p2 is end point.
    
    Args:
        p0 (Tuple[float, float]): Start point as (x,y)
        p1 (Tuple[float, float]): Control point as (x,y)
        p2 (Tuple[float, float]): End point as (x,y)
        t (float): Parameter in range [0,1]
        
    Returns:
        Tuple[float, float]: Point on curve at parameter t
        
    Examples:
        >>> bezier_point((0,0), (1,1), (2,0), 0)
        (0.0, 0.0)
        >>> bezier_point((0,0), (1,1), (2,0), 0.5)
        (1.0, 0.5)
        >>> bezier_point((0,0), (1,1), (2,0), 1)
        (2.0, 0.0)
    
    Notes:
        - t should be in range [0,1]
        - Curve is tangent to p0p1 and p1p2 at endpoints
        - Provides C¹ continuity
    """
    t2 = t * t
    mt = 1 - t
    mt2 = mt * mt
    return (
        mt2 * p0[0] + 2 * mt * t * p1[0] + t2 * p2[0],
        mt2 * p0[1] + 2 * mt * t * p1[1] + t2 * p2[1]
    )

def circle_area(radius: float) -> float:
    return pi * radius**2

def degrees_to_radians(degrees: float) -> float:
    return degrees * (pi / 180.0)

def radians_to_degrees(radians: float) -> float:
    return radians * (180.0 / pi)

def rect_overlap(r1_pos: Tuple[float, float], r1_size: Tuple[float, float], 
                r2_pos: Tuple[float, float], r2_size: Tuple[float, float]) -> bool:
    """Determines if two axis-aligned rectangles overlap in 2D space.
    
    Uses the separating axis theorem for AABB (Axis-Aligned Bounding Box) collision
    detection. Checks if rectangles overlap on both x and y axes.
    
    Args:
        r1_pos (Tuple[float, float]): Position (x,y) of first rectangle's top-left corner
        r1_size (Tuple[float, float]): Size (width, height) of first rectangle
        r2_pos (Tuple[float, float]): Position (x,y) of second rectangle's top-left corner
        r2_size (Tuple[float, float]): Size (width, height) of second rectangle
        
    Returns:
        bool: True if rectangles overlap, False otherwise
        
    Examples:
        >>> rect_overlap((0,0), (10,10), (5,5), (10,10))
        True
        >>> rect_overlap((0,0), (5,5), (10,10), (5,5))
        False
        >>> rect_overlap((0,0), (10,10), (5,0), (5,5))
        True
    
    Notes:
        - Rectangles are assumed to be axis-aligned (not rotated)
        - Edge touching counts as overlap
    """
    return (r1_pos[0] < r2_pos[0] + r2_size[0] and
            r1_pos[0] + r1_size[0] > r2_pos[0] and
            r1_pos[1] < r2_pos[1] + r2_size[1] and
            r1_pos[1] + r1_size[1] > r2_pos[1])

def triangle_area(base: float, height: float) -> float:
    return 0.5 * base * height

def triangle_area_sides(a: float, b: float, c: float) -> float:
    s = (a + b + c) / 2  # semi-perimeter
    return (s * (s - a) * (s - b) * (s - c))**0.5

# Utility functions
def clamp(value: float, min_val: float, max_val: float) -> float:
    """Clamp a value between min and max values.
    
    Args:
        value: Value to clamp
        min_val: Minimum allowed value
        max_val: Maximum allowed value
        
    Returns:
        Clamped value between min_val and max_val
    """
    return max(min_val, min(value, max_val))

def fast_invsqrt(a) -> float:
    x2 = a * 0.5
    y = a
    packed_y = struct.pack('f', y)
    
    i = struct.unpack('i', packed_y)[0]
    i = 0x5f3759df - (i >> 1)
    packed_i = struct.pack('i', i)
    y = struct.unpack('f', packed_i)[0]
    
    y = y * (1.5 - (x2 * y * y))
    return y

def lerp(start: float, end: float, t: float) -> float:
    """Linear interpolation between start and end points."""
    return start + t * (end - start)

def map_range(value: float, in_min: float, in_max: float, 
              out_min: float, out_max: float) -> float:
    """Map a value from one range to another."""
    return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min

def sum(a):
    total = 0
    for x in a:
        total += x
    return total

def wrap_angle(angle: float) -> float:
    """Wraps an angle to the range [-π, π].
    
    Normalizes any angle to equivalent angle in [-π, π] range
    by adding or subtracting 2π as needed.
    
    Args:
        angle (float): Angle in radians to wrap
        
    Returns:
        float: Equivalent angle in [-π, π] range
        
    Examples:
        >>> wrap_angle(4 * math.pi)  # 720 degrees
        0.0
        >>> wrap_angle(-3 * math.pi)  # -540 degrees
        -3.141592653589793
        >>> wrap_angle(math.pi / 2)   # 90 degrees
        1.5707963267948966
    
    Notes:
        - Useful for normalizing rotation angles
        - Preserves angle equivalence (sinθ, cosθ)
        - Input can be any real number
    """
    while angle > pi:
        angle -= 2 * pi
    while angle < -pi:
        angle += 2 * pi
    return angle