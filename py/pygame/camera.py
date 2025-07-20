from typing import Tuple
from pygame import Vector2, Rect

class Camera:
    """Camera system for smooth player tracking.

    This class handles the camera movement to follow a target (e.g., player) smoothly
    within the constraints of the level boundaries. It uses linear interpolation to
    achieve smooth transitions.

    Attributes:
        offset (Vector2): Current offset of the camera.
        target_offset (Vector2): Target offset the camera is moving towards.
        speed (float): Speed of the camera movement.
        window_size (Tuple[int, int]): Size of the game window.
        bounds (pygame.Rect): Maximum scroll bounds of the camera.
    """

    def __init__(self, window_size: Tuple[int, int], level_size: Tuple[int, int]) -> None:
        """Initialize camera with window and level dimensions.

        Args:
            window_size (Tuple[int, int]): Size of the game window (width, height).
            level_size (Tuple[int, int]): Size of the level (width, height).
        """
        self.offset = Vector2(0, 0)
        self.target_offset = Vector2(0, 0)
        self.speed = 0.1
        self.window_size = window_size

        # Calculate maximum scroll bounds
        self.bounds = Rect(
            0,
            0,
            max(0, level_size[0] - window_size[0]),  # Prevent negative bounds
            max(0, level_size[1] - window_size[1])
        )

    def follow(self, target_pos: Vector2, delta: float) -> None:
        """Update camera position to follow target.

        Args:
            target_pos (Vector2): Position of the target to follow.
            delta (float): Time delta for frame-independent movement.
        """
        self.target_offset.x = -target_pos[0] + (self.window_size[0] / 2)
        self.target_offset.y = -target_pos[1] + (self.window_size[1] / 2)

        # Constrain camera to level bounds
        if self.bounds.width > 0:
            self.target_offset.x = max(-self.bounds.width, min(0, self.target_offset.x))
        if self.bounds.height > 0:
            self.target_offset.y = max(-self.bounds.height, min(0, self.target_offset.y))

        # Smooth camera movement using linear interpolation
        self.offset.x = self.lerp(self.offset.x, self.target_offset.x, self.speed)
        self.offset.y = self.lerp(self.offset.y, self.target_offset.y, self.speed)

    def apply(self, rect: Rect) -> Tuple[int, int]:
        """Apply camera offset to a given rectangle.

        Args:
            rect (pygame.Rect): Rectangle to apply the camera offset to.

        Returns:
            Tuple[int, int]: New position of the rectangle with the camera offset applied.
        """
        return (
            int(rect.x + self.offset.x),
            int(rect.y + self.offset.y)
        )

    @staticmethod
    def lerp(start: float, end: float, t: float) -> float:
        """Linearly interpolate between start and end by t.

        Args:
            start (float): Starting value.
            end (float): Ending value.
            t (float): Interpolation factor (0.0 to 1.0).

        Returns:
            float: Interpolated value.
        """
        return start + t * (end - start)