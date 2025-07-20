"""
ConsoleMenu Module

This module provides a flexible menu system that handles user input and navigation.
It supports three types of menu items: Option, Value, and Submenu.

Classes:
    Menu: A class to create and manage menus with options, values, and submenus.

Usage:
    Create a Menu instance with a name and a list of menu items. Menu items can be options
    that execute functions, values that configure variables, or submenus that open other menus.

Example:
    #### Create a simple menu with options
    - menu = Menu("Main Menu", [
        Menu.Option("start", start_game),
        Menu.Option("quit", quit_game)
    ])

    #### Create a menu with configurable values
    - settings = Menu("Settings", [
        Menu.Value(game, "lives", "Player Lives"),
        Menu.Value(game, "range", "Number Range")
    ])

    #### Create a menu with submenus
    - main = Menu("Game", [
        Menu.Option("play", play_game),
        settings,  # Submenu
        Menu.Option("exit", quit_game)
    ])

    #### Open the menu
    - main.open()

Notes:
    - Options are displayed as numbered choices
    - Values show current value in parentheses
    - Use callable menu items for dynamic submenus
    - Input validation is handled automatically
"""

class Menu:
    """
    A flexible menu system that handles user input and navigation.
    
    Attributes:
        headerDecor (str): Template for header decoration
        name (str): Menu name
        header (str): Formatted header
        footer (str): Formatted footer
        choices (list): List of menu options

    Usage:
        The Menu class supports three types of menu items:
        1. Option - Executes a function when selected
        2. Value - Configures a variable when selected
        3. Submenu - Opens another menu when selected

    Example:
        #### Create a simple menu with options
        - menu = Menu("Main Menu", [
            Menu.Option("start", start_game),
            Menu.Option("quit", quit_game)
        ])

        #### Create a menu with configurable values
        - settings = Menu("Settings", [
            Menu.Value(game, "lives", "Player Lives"),
            Menu.Value(game, "range", "Number Range")
        ])

        #### Create a menu with submenus
        - main = Menu("Game", [
            Menu.Option("play", play_game),
            settings,  # Submenu
            Menu.Option("exit", quit_game)
        ])

        #### Open the menu
        - main.open()

    Notes:
        - Options are displayed as numbered choices
        - Values show current value in parentheses
        - Use callable menu items for dynamic submenus
        - Input validation is handled automatically
    """
    
    # Format string for creating decorated headers
    headerDecor = "<=-=-=-=-=-={ %s }=-=-=-=-=-=>"
    
    def __init__(self, name: str, choices: list, header: str = None) -> None:
        """
        Initialize menu with name and choices.

        Args:
            name (str): Menu name
            choices (list): List of menu options
            header (str, optional): Custom header text
        """
        # Capitalize menu name
        self.name = name.title()
        # Create header with custom text or menu name
        if header is None:
            self.header = self.headerDecor % self.name
        else:
            self.header = self.headerDecor % header.title()
        # Create footer line matching header length
        self.footer = "=" * len(self.header)
        self.choices = choices
    
    def open(self) -> None:
        """
        Display menu and handle user input.
        Processes user selection and navigates to appropriate option.
        """
        print("") # Visual spacing
        
        # Display menu header
        print(self.header)
        # Display numbered menu options
        for i, option in enumerate(self.choices):
            # Handle callable menu items (like dynamic submenus)
            if callable(option):
                option = option()
            print(f"[{i + 1}] {option.name}", end = "")
            # Show current value for configurable options
            if option.__class__ is self.Value:
                print(f" ({getattr(option.parent, option.atr)})", end = "\n")
            else:
                print("", end = "\n")
        print(self.footer)
        
        # Menu selection loop
        choosing = True
        while choosing:
            try:
                # Get user input (subtract 1 for zero-based indexing)
                choice = int(input(">>> ")) - 1
            except ValueError:
                print("Please enter the number of the option you want to choose.")
                continue
                
            # Validate input range
            if choice > len(self.choices) - 1 or choice < 0:
                print("Please enter a valid option.")
                continue
            else:
                choice = self.choices[choice]
                # Handle callable menu items
                if callable(choice):
                    choice = choice()
                
                # Process different types of menu items
                if choice.__class__ is Menu:
                    choosing = False
                    choice.open()  # Open submenu
                elif choice.__class__ is self.Value:
                    choice.select(len(self.header))  # Configure value
                    self.open()  # Reopen current menu
                elif choice.__class__ is self.Option:
                    choosing = False
                    choice.select()  # Execute option function
                    
    class Option:
        """
        Represents a menu option that executes a function when selected.
        
        Attributes:
            name (str): Option display name
            func (callable): Function to execute when selected

        Example:
            option = Menu.Option("start game", start_function)
        """
        def __init__(self, name: str, func: callable) -> None:
            self.name = name.title()
            self.func = func
            
        def select(self) -> None:
            self.func()
                    
    class Value:
        """
        Represents a configurable value in the menu system.
        
        Attributes:
            parent (object): Object containing the value
            atr (str): Attribute name to modify
            name (str): Display name for the value

        Example:
            value = Menu.Value(game, "lives", "Player Lives")\n
            value = Menu.Value(game, "range", "Number Range")\n
        """
        def __init__(self, obj: object, atr: str, name: str = None) -> None:
            self.parent = obj
            self.atr = atr
            self.name = name or atr
            
        def select(self, padding: int = 0) -> None:
            print("")
            # Display value configuration UI
            print(f"{self.name.title().center(padding, "-")}")
            print(f"enter the new value ".center(padding))
            
            choosing = True
            while choosing:
                try:
                    # Get current value type
                    atrType = getattr(self.parent, self.atr).__class__
                    print("<<<", end="")
                    # Handle list/tuple inputs differently
                    if atrType is list or atrType is tuple:
                        setattr(self.parent, self.atr, atrType(map(int or str, input().split())))
                    else:
                        setattr(self.parent, self.atr, atrType(input()))
                except TypeError:
                    print("Value must be of type %s" % atrType.__name__)
                    continue
                choosing = False