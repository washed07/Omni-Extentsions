"""Signal System Implementation Module.

Provides a simple signal/slot system for event handling and communication between objects.
Implements the Observer pattern with support for multiple connections and dynamic binding.

Example:
    >>> signal = Signal()
    >>> def handler(sender, value):
    ...     print(f"Received {value} from {sender}")
    >>> signal.connect(handler)
    >>> signal.emit(self, "hello")  # Prints: Received hello from <...>
"""

class Signal:
    """A signal that can connect to and notify multiple receivers.
    
    Implements the Observer pattern allowing objects to subscribe to notifications
    and receive callbacks when events occur. Supports multiple connections and
    dynamic connection/disconnection.

    Attributes:
        connections (list): List of connected callback functions
        
    Example:
        >>> on_value_changed = Signal()
        >>> on_value_changed.connect(update_ui)
        >>> on_value_changed.emit(self, new_value)
    """

    def __init__(self):
        """Initialize signal with empty connection list."""
        self.connections: list = []

    def connect(self, func) -> None:
        """Connect a callback function to this signal.
        
        Args:
            func: Callback function to connect
            
        Note:
            Functions are only connected once, duplicates are ignored
        """
        if func not in self.connections:
            self.connections.append(func)

    def disconnect(self, func) -> None:
        """Remove a callback function from this signal.
        
        Args:
            func: Callback function to disconnect
            
        Note:
            Silently ignores disconnecting functions that weren't connected
        """
        if func in self.connections:
            self.connections.remove(func)

    def emit(self, origin: classmethod, *args, **kwargs) -> None:
        """Notify all connected callbacks with given arguments.
        
        Args:
            origin: Object emitting the signal (typically self)
            *args: Positional arguments to pass to callbacks
            **kwargs: Keyword arguments to pass to callbacks
            
        Example:
            >>> signal.emit(self, value=42, valid=True)
        """
        for func in self.connections:
            func(origin, *args, **kwargs)
