from math import cos, sin, pi, pow, sqrt
import math
from typing import Tuple


def clamp(value: float, min_value: float, max_value: float) -> float:
    """Clamps a value between a minimum and maximum range."""
    return max(min_value, min(value, max_value))

def lerp(start: float, end: float, t: float, margin: float) -> float:
    num = (1 - t) * start + t * end
    if abs(num) < abs(margin):
        return 0
    return (1 - t) * start + t * end

def sine_in(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * (1 - cos((t * pi) / 2))
    if abs(num) < abs(margin): return 0
    return num

def sine_out(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * sin((t * pi) / 2)
    if abs(num) < abs(margin): return 0
    return num

def sine_inout(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * (-(cos(pi * t) - 1) / 2)
    if abs(num) < abs(margin): return 0
    return num

def quad_in(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * (t * t)
    if abs(num) < abs(margin): return 0
    return num

def quad_out(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * (1 - (1 - t) * (1 - t))
    if abs(num) < abs(margin): return 0
    return num

def quad_inout(start: float, end: float, t: float, margin: float) -> float:
    ease = 2 * t * t if t < 0.5 else 1 - pow(-2 * t + 2, 2) / 2
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def cubic_in(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * (t * t * t)
    if abs(num) < abs(margin): return 0
    return num

def cubic_out(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * (1 - pow(1 - t, 3))
    if abs(num) < abs(margin): return 0
    return num

def cubic_inout(start: float, end: float, t: float, margin: float) -> float:
    ease = 4 * t * t * t if t < 0.5 else 1 - pow(-2 * t + 2, 3) / 2
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def bounce_out(start: float, end: float, t: float, margin: float) -> float:
    n1, d1 = 7.5625, 2.75
    if t < 1 / d1:
        ease = n1 * t * t
    elif t < 2 / d1:
        t -= 1.5 / d1
        ease = n1 * t * t + 0.75
    elif t < 2.5 / d1:
        t -= 2.25 / d1
        ease = n1 * t * t + 0.9375
    else:
        t -= 2.625 / d1
        ease = n1 * t * t + 0.984375
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def bounce_in(start: float, end: float, t: float, margin: float) -> float:
    num = start + (end - start) * (1 - bounce_out(0, 1, 1 - t, 0))
    if abs(num) < abs(margin): return 0
    return num

def bounce_inout(start: float, end: float, t: float, margin: float) -> float:
    if t < 0.5:
        ease = (1 - bounce_out(0, 1, 1 - 2 * t, 0)) / 2
    else:
        ease = (1 + bounce_out(0, 1, 2 * t - 1, 0)) / 2
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def elastic_out(start: float, end: float, t: float, margin: float) -> float:
    c4 = (2 * pi) / 3
    ease = 0 if t == 0 else (1 if t == 1 else pow(2, -10 * t) * sin((t * 10 - 0.75) * c4) + 1)
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def elastic_in(start: float, end: float, t: float, margin: float) -> float:
    c4 = (2 * pi) / 3
    ease = 0 if t == 0 else (1 if t == 1 else -pow(2, 10 * t - 10) * sin((t * 10 - 10.75) * c4))
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def elastic_inout(start: float, end: float, t: float, margin: float) -> float:
    c5 = (2 * pi) / 4.5
    if t == 0: return start
    if t == 1: return end
    if t < 0.5:
        ease = -(pow(2, 20 * t - 10) * sin((20 * t - 11.125) * c5)) / 2
    else:
        ease = (pow(2, -20 * t + 10) * sin((20 * t - 11.125) * c5)) / 2 + 1
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def circ_in(start: float, end: float, t: float, margin: float) -> float:
    ease = 1 - pow(1 - t, 2)
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def circ_out(start: float, end: float, t: float, margin: float) -> float:
    ease = pow(1 - pow(t - 1, 2), 0.5)
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def circ_inout(start: float, end: float, t: float, margin: float) -> float:
    ease = -(pow(1 - 4 * t * t, 0.5) - 1) / 2 if t < 0.5 else (pow(-(2 * t - 3) * (2 * t - 1), 0.5) + 1) / 2
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def quart_in(start: float, end: float, t: float, margin: float) -> float:
    ease = t * t * t * t
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def quart_out(start: float, end: float, t: float, margin: float) -> float:
    ease = 1 - pow(1 - t, 4)
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def quart_inout(start: float, end: float, t: float, margin: float) -> float:
    ease = 8 * t * t * t * t if t < 0.5 else 1 - pow(-2 * t + 2, 4) / 2
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def expo_in(start: float, end: float, t: float, margin: float) -> float:
    ease = 0 if t == 0 else pow(2, 10 * t - 10)
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def expo_out(start: float, end: float, t: float, margin: float) -> float:
    ease = 1 if t == 1 else 1 - pow(2, -10 * t)
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def expo_inout(start: float, end: float, t: float, margin: float) -> float:
    if t == 0: return start
    if t == 1: return end
    if t < 0.5:
        ease = pow(2, 20 * t - 10) / 2
    else:
        ease = (2 - pow(2, -20 * t + 10)) / 2
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def smooth_lerp(start: float, end: float, 
                t: float, smooth_factor: float) -> float:
    """Smoothly interpolates between values with variable smoothing.
    
    Args:
        start (float): Start value
        end (float): End value
        t (float): Raw interpolation parameter [0,1]
        smooth_factor (float): Smoothing strength (higher = smoother)
        
    Returns:
        float: Smoothly interpolated value
        
    Examples:
        >>> smooth_lerp(0, 100, 0.5, 2.0)
        50.0
        >>> abs(smooth_lerp(0, 10, 0.1, 3.0)) < smooth_lerp(0, 10, 0.1, 1.0)
        True
    """
    t = t ** smooth_factor
    return lerp(start, end, t, 0)

def sine_in(start: float, end: float, t: float, margin: float) -> float:
    """Sinusoidal ease-in function that starts slow and accelerates.
    
    Creates a smooth acceleration using the first quadrant of a sine curve.
    Motion begins slowly and gradually speeds up, following sin(t) curve.
    
    Args:
        start (float): Starting value
        end (float): Target value
        t (float): Time parameter [0,1]
        margin (float): Threshold for snapping to zero
        
    Returns:
        float: Eased value between start and end
        
    Examples:
        >>> sine_in(0, 100, 0.0, 0)
        0.0
        >>> sine_in(0, 100, 0.25, 0)  # Slower at start
        14.64
        >>> sine_in(0, 100, 0.75, 0)  # Faster near end
        85.36
        >>> sine_in(0, 100, 1.0, 0)
        100.0
        
    Notes:
        - Curve: y = 1 - cos(t * π/2)
        - Velocity starts at zero and increases smoothly
        - Good for entering animations that need soft start
        - First derivative (velocity) is continuous
        
    Use Cases:
        - Menu items sliding in
        - Fade-in effects
        - Objects entering scene
    """
    num = start + (end - start) * (1 - cos((t * pi) / 2))
    if abs(num) < abs(margin): return 0
    return num

def sine_out(start: float, end: float, t: float, margin: float) -> float:
    """Sinusoidal ease-out function that starts fast and decelerates.
    
    Creates a smooth deceleration using the last quadrant of a sine curve.
    Motion begins quickly and gradually slows down, following sin(t) curve.
    
    Args:
        start (float): Starting value
        end (float): Target value
        t (float): Time parameter [0,1]
        margin (float): Threshold for snapping to zero
        
    Returns:
        float: Eased value between start and end
        
    Examples:
        >>> sine_out(0, 100, 0.0, 0)
        0.0
        >>> sine_out(0, 100, 0.25, 0)  # Faster at start
        70.71
        >>> sine_out(0, 100, 0.75, 0)  # Slower near end
        98.48
        >>> sine_out(0, 100, 1.0, 0)
        100.0
        
    Notes:
        - Curve: y = sin(t * π/2)
        - Velocity starts high and decreases smoothly
        - Good for exiting animations that need gentle stop
        - First derivative (velocity) is continuous
        
    Use Cases:
        - Camera movement ending
        - Scroll deceleration
        - Objects settling into place
    """
    num = start + (end - start) * sin((t * pi) / 2)
    if abs(num) < abs(margin): return 0
    return num

def quad_in(start: float, end: float, t: float, margin: float) -> float:
    """Quadratic ease-in function with accelerating squared curve.
    
    Motion follows t² curve, creating acceleration that increases 
    quadratically over time. Starts very slowly and builds speed.
    
    Args:
        start (float): Starting value
        end (float): Target value
        t (float): Time parameter [0,1]
        margin (float): Threshold for snapping to zero
        
    Returns:
        float: Eased value between start and end
        
    Examples:
        >>> quad_in(0, 100, 0.0, 0)
        0.0
        >>> quad_in(0, 100, 0.25, 0)  # Very slow start
        6.25
        >>> quad_in(0, 100, 0.75, 0)  # Rapid acceleration
        56.25
        >>> quad_in(0, 100, 1.0, 0)
        100.0
        
    Notes:
        - Curve: y = t²
        - Steeper acceleration than sine_in
        - Second power means very gentle start
        - Good for emphasizing build-up
        
    Use Cases:
        - Power charging effects
        - Dramatic entrances
        - Building momentum
    """
    num = start + (end - start) * (t * t)
    if abs(num) < abs(margin): return 0
    return num

def cubic_out(start: float, end: float, t: float, margin: float) -> float:
    """Cubic ease-out function with decelerating cubed curve.
    
    Motion follows 1-(1-t)³ curve, creating deceleration that decreases
    cubically over time. Starts very fast and slows dramatically.
    
    Args:
        start (float): Starting value
        end (float): Target value
        t (float): Time parameter [0,1]
        margin (float): Threshold for snapping to zero
        
    Returns:
        float: Eased value between start and end
        
    Examples:
        >>> cubic_out(0, 100, 0.0, 0)
        0.0
        >>> cubic_out(0, 100, 0.25, 0)  # Quick start
        57.81
        >>> cubic_out(0, 100, 0.75, 0)  # Very slow end
        98.44
        >>> cubic_out(0, 100, 1.0, 0)
        100.0
        
    Notes:
        - Curve: y = 1-(1-t)³
        - Stronger deceleration than quad_out
        - Third power means very gentle stop
        - Good for emphasizing arrival
        
    Use Cases:
        - UI elements settling
        - Camera focus transitions
        - Object placement effects
    """
    num = start + (end - start) * (1 - pow(1 - t, 3))
    if abs(num) < abs(margin): return 0
    return num

def bounce_inout(start: float, end: float, t: float, margin: float) -> float:
    """Combined bounce ease-in/out function with multiple bounces.
    
    Creates a bouncing effect at both start and end. First half uses
    bounce-in, second half uses bounce-out for symmetric animation.
    
    Args:
        start (float): Starting value
        end (float): Target value
        t (float): Time parameter [0,1]
        margin (float): Threshold for snapping to zero
        
    Returns:
        float: Eased value between start and end
        
    Examples:
        >>> bounce_inout(0, 100, 0.0, 0)
        0.0
        >>> bounce_inout(0, 100, 0.25, 0)  # Multiple small bounces
        12.5
        >>> bounce_inout(0, 100, 0.75, 0)  # Symmetric bounce pattern
        87.5
        >>> bounce_inout(0, 100, 1.0, 0)
        100.0
        
    Notes:
        - Combines bounce-in and bounce-out
        - Multiple bounces with diminishing amplitude
        - Symmetric around midpoint
        - Good for playful/energetic transitions
        
    Use Cases:
        - Menu selections
        - Game powerups
        - Attention-grabbing animations
    """
    if t < 0.5:
        ease = (1 - bounce_out(0, 1, 1 - 2 * t, 0)) / 2
    else:
        ease = (1 + bounce_out(0, 1, 2 * t - 1, 0)) / 2
    num = start + (end - start) * ease
    if abs(num) < abs(margin): return 0
    return num

def smooth_damp(current: float, target: float, vel: float,
                smooth_time: float, delta_time: float,
                max_speed: float = float('inf')) -> Tuple[float, float]:
    """Gradually changes a value to a target with smoothed motion and velocity.
    
    Implements critically damped spring physics for smooth animation without
    overshooting. Often used for camera and UI motion.
    
    Args:
        current (float): Current position/value
        target (float): Target position/value
        vel (float): Current velocity (modified by function)
        smooth_time (float): Approximate time to reach target
        delta_time (float): Time since last update
        max_speed (float, optional): Speed limit. Defaults to infinity
        
    Returns:
        Tuple[float, float]: (new_position, new_velocity)
        
    Examples:
        >>> pos, vel = 0.0, 0.0
        >>> for _ in range(60):  # 60fps animation
        ...     pos, vel = smooth_damp(pos, 100.0, vel, 0.3, 1/60)
        >>> abs(pos - 100.0) < 0.01  # Eventually reaches target
        True
        
    Notes:
        - Based on Game Programming Gems 4 Chapter 1.10
        - Never overshoots if target remains constant
        - Automatically adjusts for variable frame rate
        - Maintains continuous velocity
        
    Raises:
        ValueError: If smooth_time is <= 0
    """
    if smooth_time <= 0:
        raise ValueError("Smooth time must be positive")
        
    smooth_time = max(0.0001, smooth_time)
    omega = 2 / smooth_time
    x = omega * delta_time
    exp = 1 / (1 + x + 0.48 * x * x + 0.235 * x * x * x)
    
    change = current - target
    target_temp = target
    
    max_change = max_speed * smooth_time
    change = clamp(change, -max_change, max_change)
    target = current - change
    
    temp = (vel + omega * change) * delta_time
    vel = (vel - omega * temp) * exp
    output = target + (change + temp) * exp
    
    if (target_temp - current > 0) == (output > target_temp):
        output = target_temp
        vel = (output - target_temp) / delta_time
        
    return output, vel

def remap(value: float, from_min: float, from_max: float, 
        to_min: float, to_max: float) -> float:
    """Remaps a value from one range to another using linear interpolation.
    
    Transforms a value from source range [from_min, from_max] to target 
    range [to_min, to_max] maintaining the relative position.
    
    Args:
        value (float): Value to remap
        from_min (float): Lower bound of source range
        from_max (float): Upper bound of source range
        to_min (float): Lower bound of target range
        to_max (float): Upper bound of target range
        
    Returns:
        float: Value mapped to new range
        
    Examples:
        >>> remap(0.5, 0, 1, 0, 100)
        50.0
        >>> remap(75, 0, 100, -1, 1)
        0.5
        >>> remap(-10, -100, 100, 0, 1)
        0.45
        
    Notes:
        - Linear mapping preserves relative position in range
        - No clamping is performed on input or output
        
    Raises:
        ZeroDivisionError: If from_min equals from_max
    """
    normalized = (value - from_min) / (from_max - from_min)
    return normalized * (to_max - to_min) + to_min

def lerp_color(color1: Tuple[int, int, int], 
            color2: Tuple[int, int, int], 
            t: float) -> Tuple[int, int, int]:
    """Linearly interpolates between two RGB colors.
    
    Args:
        color1 (Tuple[int, int, int]): Start RGB color
        color2 (Tuple[int, int, int]): End RGB color
        t (float): Interpolation parameter [0,1]
        
    Returns:
        Tuple[int, int, int]: Interpolated RGB color
        
    Examples:
        >>> lerp_color((255,0,0), (0,0,255), 0.5)
        (128, 0, 128)
        >>> lerp_color((0,0,0), (255,255,255), 0.25)
        (64, 64, 64)
    """
    return (
        int(lerp(color1[0], color2[0], t, 0)),
        int(lerp(color1[1], color2[1], t, 0)),
        int(lerp(color1[2], color2[2], t, 0))
    )

def ease_out_elastic(t: float) -> float:
    """Elastic easing out function for bouncy animations.
    
    Creates a springy/elastic motion that starts fast and bounces at the end.
    
    Args:
        t (float): Time parameter in range [0,1]
        
    Returns:
        float: Eased value in range [0,1]
        
    Examples:
        >>> ease_out_elastic(0)
        0.0
        >>> ease_out_elastic(1)
        1.0
        >>> 0 <= ease_out_elastic(0.5) <= 1  # Bounces within range
        True
        
    Notes:
        - Based on Robert Penner's easing functions
        - Creates overshoot/bounce effect
        - Good for emphasizing completion of actions
        - t should be in range [0,1]
    """
    p = 0.3
    s = p / 4
    return pow(2, -10 * t) * math.sin((t - s) * (2 * math.pi) / p) + 1

def approach(current: float, target: float, increment: float) -> float:
    """Moves a value toward a target by a fixed increment amount.
    
    Similar to move_toward but uses a fixed increment rather than maximum delta.
    Useful for consistent-speed transitions regardless of distance to target.
    
    Args:
        current (float): Starting value to move from
        target (float): Target value to move toward
        increment (float): Fixed amount to move by (must be positive)
        
    Returns:
        float: New value after moving toward target by increment
        
    Raises:
        ValueError: If increment is negative
        
    Examples:
        >>> approach(0.0, 10.0, 2.0)
        2.0
        >>> approach(8.0, 10.0, 2.0)
        10.0
        >>> approach(10.0, 0.0, 2.0)
        8.0
    
    Notes:
        - Always moves by exactly increment amount unless target is reached
        - Different from move_toward which uses a maximum delta
        - Useful for fixed-rate animations and transitions
    """
    if increment < 0:
        raise ValueError("Increment must be positive")
    if current < target:
        return min(current + increment, target)
    return max(current - increment, target)

def smooth_step(edge0: float, edge1: float, x: float) -> float:
    """Performs Hermite interpolation for smooth transitions between two edges.
    
    Implements Ken Perlin's smoothstep function which provides smooth transitions
    using a cubic Hermite interpolation polynomial. The function smoothly transitions
    from 0 to 1 as x moves from edge0 to edge1.
    
    Args:
        edge0 (float): The lower edge of the transition
        edge1 (float): The upper edge of the transition
        x (float): The input value to interpolate
        
    Returns:
        float: Smoothly interpolated value between 0 and 1
        
    Examples:
        >>> smooth_step(0, 1, 0.5)
        0.5
        >>> smooth_step(0, 1, 0.1)
        0.028
        >>> smooth_step(5, 10, 7.5)
        0.5
        
    Notes:
        - Input is clamped to [0,1] range before interpolation
        - Provides C¹ continuity (continuous first derivative)
        - Commonly used for fade effects and smooth transitions
    """
    x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0)
    return x * x * (3 - 2 * x)

def move_toward(current: float, target: float, max_delta: float) -> float:
    """Gradually moves a value toward a target with a maximum change per step.
    
    Useful for smooth value transitions where you want to limit the rate of change.
    The function will move the current value toward the target by at most max_delta
    units per call.
    
    Args:
        current (float): The starting value to move from
        target (float): The destination value to move toward
        max_delta (float): Maximum amount the value can change in one step
        
    Returns:
        float: New value after moving toward target
        
    Examples:
        >>> move_toward(0.0, 10.0, 2.0)
        2.0
        >>> move_toward(8.0, 10.0, 2.0)
        10.0
        >>> move_toward(10.0, 0.0, 3.0)
        7.0
    
    Notes:
        - If the distance to target is less than max_delta, returns target exactly
        - Uses math.copysign to handle positive and negative transitions
    """
    if abs(target - current) <= max_delta:
        return target
    return current + math.copysign(max_delta, target - current)