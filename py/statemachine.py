"""State Machine Implementation Module.

This module provides a flexible state machine architecture for game objects.
It implements the State pattern with signal-based transitions and automatic
state tracking.

Example:
    >>> idle_state = IdleState()
    >>> state_machine = State_Machine(player, IdleState, True)
    >>> state_machine.update()  # Processes current state logic
"""

from signal import Signal


class State:
    """Base class for all states in the state machine.
    
    Provides the core interface and functionality for state behavior.
    States handle entry, exit, and processing logic for game objects.
    
    Attributes:
        transition (Signal): Signal emitter for state transitions
    """

    def __init__(self):
        """Initialize state and register with state machine.
        
        Creates transition signal and connects to state machine.
        Adds self to global state registry if not already present.
        """
        self.transition = Signal()
        self.transition.connect(State_Machine.__setstate__)
        if self not in State_Machine.states:
            State_Machine.states.append(self)

    def on_enter(self, actor):
        """Called when state becomes active.
        
        Args:
            actor: The game object entering this state
        """
        pass

    def on_exit(self, actor):
        """Called when state becomes inactive.
        
        Args:
            actor: The game object exiting this state
        """
        pass

    def process(self, actor):
        """Process state logic each frame.
        
        Args:
            actor: The game object to update
        """
        pass


class State_Machine:
    """Manages state transitions and processing for game objects.
    
    Handles state initialization, transitions, and updates.
    Maintains registry of all created states.
    
    Attributes:
        states (List[State]): Global registry of state instances
        actor: Game object this state machine controls
        current_state (State): Currently active state
        active (bool): Whether state machine is processing
    """
    
    states = []

    def __init__(self, actor, init_state: State.__class__, active: bool):
        """Initialize state machine with starting state.
        
        Args:
            actor: Game object to control
            init_state: Initial state class
            active: Whether to begin processing immediately
        """
        self.actor = actor
        self.current_state: State.__class__ = init_state()
        self.current_state.on_enter(self.actor)
        self.active = active

    def __setstate__(self, state: State.__class__):
        """Transition to a new state.
        
        Handles exit/entry calls and state instance creation.
        
        Args:
            state: New state class to transition to
            
        Raises:
            AssertionError: If current_state is invalid
        """
        if not self.current_state:
            raise AssertionError("current state is invalid")

        self.current_state.on_exit(self.actor)
        self.current_state = state()
        self.current_state.on_enter(self.actor)

    def __getstate__(self):
        if self.current_state: return self.current_state

    def update(self, delta: float):
        if self.active:
            self.current_state.process(self.actor, delta)
