import pygame
from typing import Tuple

class Button:
    """Interactive button for menu navigation.

    Handles rendering, hover effects, and click detection for menu buttons.

    Attributes:
        text (str): Button label text
        pos (Tuple[int, int]): Center position of button
        font (pygame.font.Font): Font for rendering text
        text_surface (pygame.Surface): Rendered text surface
        rect (pygame.Rect): Button collision/position rectangle
    """

    # Add new color constants
    DEFAULT_COLOR = (255, 255, 255)  # White
    HOVER_COLOR = (255, 0, 0)  # Red

    def __init__(self, text: str, pos: Tuple[int, int]):
        """Initialize button with text and position.

        Args:
            text: String to display on button
            pos: (x, y) coordinates for button center
        """
        self.text = text
        self.pos = pos
        self.font = pygame.font.Font(None, 50)
        self.current_color = self.DEFAULT_COLOR
        self.render_text()

    def render_text(self, color: Tuple[int, int, int] = None) -> None:
        """Render button text with given color.

        Args:
            color: RGB color tuple for text (default: WHITE)
        """
        color = color or self.current_color
        self.text_surface = self.font.render(self.text, True, color)
        self.rect = self.text_surface.get_rect(center=self.pos)

    def draw(self, surface: pygame.Surface, mouse_pos: Tuple[int, int]) -> None:
        """Draw the button on the given surface.

        Args:
            surface: Pygame surface to draw the button on
            mouse_pos: Current mouse position (x, y)
        """
        if self.rect.collidepoint(mouse_pos):
            self.current_color = self.HOVER_COLOR
        else:
            self.current_color = self.DEFAULT_COLOR
        self.render_text()
        surface.blit(self.text_surface, self.rect.topleft)
