"""Input Management System.

This module provides a robust input handling system for keyboard events in pygame.
It implements a singleton pattern for global input state management and provides
an enum-based key binding system.

Example:
    >>> input_manager = InputManager()
    >>> input_manager.update()
    >>> if input_manager.is_pressed(KeyBinding.JUMP):
    >>>     player.jump()
"""

from enum import Enum
from typing import Dict, List, Optional
import pygame

class KeyBinding(Enum):
    """Available key bindings for game controls.
    
    This enum maps game actions to specific keyboard keys using pygame key constants.
    
    Attributes:
        JUMP: Up/jump action key (W)
        LEFT: Move left key (A)
        RIGHT: Move right key (D)
    """
    JUMP = pygame.K_w
    DOWN = pygame.K_s
    LEFT = pygame.K_a
    RIGHT = pygame.K_d

class InputState(Enum):
    """Possible states for input keys.
    
    Tracks the complete lifecycle of a key press:
    
    Attributes:
        INACTIVE: Key is not being pressed
        PRESSED: Key was just pressed this frame
        HELD: Key is being held down
        RELEASED: Key was just released this frame
        
    State Transitions:
        INACTIVE -> PRESSED -> HELD -> RELEASED -> INACTIVE
    """
    INACTIVE = 0  # Key is up
    PRESSED = 1   # Key was just pressed
    HELD = 2      # Key is being held
    RELEASED = 3  # Key was just released

class InputManager:
    """Singleton input manager for handling keyboard state.
    
    This class implements the singleton pattern to ensure only one input
    manager exists. It tracks the state of configured key bindings and
    provides methods to query their states.
    
    Attributes:
        _instance: Singleton instance of InputManager
        _initialized (bool): Whether the instance has been initialized
        _states (Dict[KeyBinding, InputState]): Current state of each key binding
        
    Example:
        >>> manager = InputManager()  # Gets singleton instance
        >>> manager.update()          # Updates input states
        >>> if manager.is_pressed(KeyBinding.JUMP):
        >>>     print("Jump key was pressed")
    """
    
    _instance: Optional['InputManager'] = None

    def __new__(cls) -> 'InputManager':
        """Create or return the singleton instance.
        
        Returns:
            InputManager: The singleton input manager instance
        """
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance._initialized = False
        return cls._instance

    def __init__(self) -> None:
        """Initialize the input manager if not already initialized."""
        if not self._initialized:
            self._states: Dict[KeyBinding, InputState] = {
                key: InputState.INACTIVE for key in KeyBinding
            }
            self._initialized = True

    def update(self) -> None:
        """
        Update the input states for each frame.
        This method checks the current state of all key bindings and updates their
        states accordingly. It should be called once per frame to ensure that the
        input states are kept up to date.
        Raises:
            pygame.error: If there is an error updating the input states.
        """
        try:
            keys = pygame.key.get_pressed()
            for binding in KeyBinding:
                current = self._states[binding]
                pressed = keys[binding.value]
                self._states[binding] = self._get_new_state(current, pressed)
        except pygame.error as e:
            print(f"Error updating input states: {e}")

    def _get_new_state(self, current: InputState, is_pressed: bool) -> InputState:
        """
        Determine the new input state based on the current state and whether the input is pressed.
        Args:
            current (InputState): The current state of the input.
            is_pressed (bool): A boolean indicating whether the input is currently pressed.
        Returns:
            InputState: The new state of the input.
        """
        if is_pressed:
            return InputState.PRESSED if current == InputState.INACTIVE else InputState.HELD
        return InputState.RELEASED if current == InputState.HELD else InputState.INACTIVE

    def is_pressed(self, binding: KeyBinding) -> bool:
        """
        Check if the specified key binding was just pressed in the current frame.
        Args:
            binding (KeyBinding): The key binding to check.
        Returns:
            bool: True if the key was just pressed this frame, False otherwise.
        """
        return self._states[binding] == InputState.PRESSED

    def is_held(self, binding: KeyBinding) -> bool:
        """
        Check if a key binding is being held down.
        Args:
            binding (KeyBinding): The key binding to check.
        Returns:
            bool: True if the key binding is being held down, False otherwise.
        """
        return self._states[binding] == InputState.HELD

    def is_released(self, binding: KeyBinding) -> bool:
        """
        Check if the specified key binding was just released in the current frame.

        Args:
            binding (KeyBinding): The key binding to check.

        Returns:
            bool: True if the key was just released this frame, False otherwise.
        """
        return self._states[binding] == InputState.RELEASED

    def get_axis(self, negative: KeyBinding, positive: KeyBinding) -> int:
        """
        Get the axis value based on the state of two opposing keys.

        Args:
            negative (KeyBinding): The key binding for the negative direction.
            positive (KeyBinding): The key binding for the positive direction.

        Returns:
            int: Returns -1 if the negative key is held or pressed and the positive key is not.
                 Returns 1 if the positive key is held or pressed and the negative key is not.
                 Returns 0 if neither or both keys are held or pressed.
        """
        neg = self.is_held(negative) or self.is_pressed(negative)
        pos = self.is_held(positive) or self.is_pressed(positive)
        return -1 if neg and not pos else 1 if pos and not neg else 0
