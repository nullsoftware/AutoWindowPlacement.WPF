# AutoWindowPlacement.WPF

A lightweight library that automatically saves and restores WPF window positions and states between application sessions.

[![NuGet](https://img.shields.io/nuget/v/AutoWindowPlacement.WPF.svg)](https://www.nuget.org/packages/AutoWindowPlacement.WPF/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

## Features

- **Automatic window placement persistence** - Save and restore window position, size, and state
- **Registry-based storage** - Built-in storage using Windows Registry
- **Extensible storage** - Implement custom storage strategies (JSON, XML, database, etc.)
- **Simple XAML integration** - Add with a single line of code
- **Zero dependencies** - Pure WPF implementation

## Supported Frameworks

- .NET 5.0-windows, 6.0-windows, 7.0-windows, 8.0-windows

## Installation

Install via NuGet Package Manager:

```bash
Install-Package AutoWindowPlacement.WPF
```

Or via .NET CLI:

```bash
dotnet add package AutoWindowPlacement.WPF
```

## Quick Start

Add the namespace to your Window XAML:

```xml
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:awp="https://github.com/nullsoftware/AutoWindowPlacement.WPF"
        awp:WindowExtensions.PlacementStorageStrategy="{awp:RegistryStorage}"
        Title="Main Window" Height="450" Width="800">
    <!-- Your window content -->
</Window>
```

That's it! The window position and state will now be automatically saved to the Windows Registry and restored on next launch.

## Usage

### Basic Usage with Registry Storage

The simplest way to use AutoWindowPlacement is with the built-in `RegistryStorage`:

```xml
<Window xmlns:awp="https://github.com/nullsoftware/AutoWindowPlacement.WPF"
        awp:WindowExtensions.PlacementStorageStrategy="{awp:RegistryStorage}">
</Window>
```

By default, this stores window placement in:
```
HKEY_CURRENT_USER\SOFTWARE\{CompanyName}\{ProductName}\{WindowName}.Placement
```

### Custom Registry Location

Customize the registry key and naming format:

```xml
<Window awp:WindowExtensions.PlacementStorageStrategy="{awp:RegistryStorage 
            Key='SOFTWARE\\MyCompany\\MyApp\\WindowSettings',
            NameFormat='{0}_Position'}">
</Window>
```

### Custom Storage Implementation

Implement the `IWindowPlacementStorage` interface for custom storage:

```csharp
using System.Windows;
using NullSoftware.Windows;

public class JsonFileStorage : IWindowPlacementStorage
{
    private readonly string _filePath;

    public JsonFileStorage(string filePath)
    {
        _filePath = filePath;
    }

    public byte[]? LoadPlacement(Window window)
    {
        // Load from JSON file
        // Return byte array or null if not found
    }

    public void SavePlacement(Window window, byte[] serializedPlacement)
    {
        // Save to JSON file
    }
}
```

Then use it in code-behind:

```csharp
public MainWindow()
{
    InitializeComponent();
    WindowExtensions.SetPlacementStorageStrategy(this, new JsonFileStorage("settings.json"));
}
```

## API Reference

### WindowExtensions

The main class providing attached properties for window placement.

#### Attached Properties

- `PlacementStorageStrategy` - Gets or sets the storage strategy for window placement

```csharp
WindowExtensions.SetPlacementStorageStrategy(window, storageInstance);
IWindowPlacementStorage storage = WindowExtensions.GetPlacementStorageStrategy(window);
```

### IWindowPlacementStorage

Interface for implementing custom storage strategies.

```csharp
public interface IWindowPlacementStorage
{
    void SavePlacement(Window window, byte[] serializedPlacement);
    byte[]? LoadPlacement(Window window);
}
```

### RegistryStorage

Built-in implementation that stores window placement in Windows Registry.

#### Properties

- `Hive` - Registry hive (default: `RegistryHive.CurrentUser`)
- `Key` - Registry key path (default: `SOFTWARE\{Company}\{Product}`)
- `NameFormat` - Value name format (default: `{0}.Placement`)

#### Methods

- `GetSettingKey(Window)` - Override to customize the registry value name
- `ProvideDefaultHive()` - Override to change default registry hive
- `ProvideDefaultKey()` - Override to change default registry key path

### WindowPlacementManager

Low-level API for direct window placement manipulation.

```csharp
// Get window placement
var placement = WindowPlacementManager.GetPlacement(window);

// Set window placement
WindowPlacementManager.SetPlacement(window, placement);

// Serialize placement to bytes
byte[] data = WindowPlacementManager.Serialize(placement);

// Deserialize placement from bytes
var placement = WindowPlacementManager.Deserialize(data);
```

## Advanced Examples

### Per-User Storage with Custom Key

```xml
<Window awp:WindowExtensions.PlacementStorageStrategy="{awp:RegistryStorage 
            Key='SOFTWARE\\MyApp\\Settings',
            Hive='CurrentUser'}">
</Window>
```

### Conditional Storage in Code-Behind

```csharp
public MainWindow()
{
    InitializeComponent();
    
    if (Settings.Default.RememberWindowPosition)
    {
        WindowExtensions.SetPlacementStorageStrategy(this, new RegistryStorage());
    }
}
```

### Custom Storage with Inheritance

```csharp
public class CustomRegistryStorage : RegistryStorage
{
    protected override string GetSettingKey(Window window)
    {
        // Use window title instead of type name
        return string.Format(NameFormat, window.Title.Replace(" ", "_"));
    }

    protected override string ProvideDefaultKey()
    {
        return @"SOFTWARE\MyCompany\MyApp\Windows";
    }
}
```

## How It Works

1. When `PlacementStorageStrategy` is set, the library hooks into window events
2. On `SourceInitialized`, it loads the saved placement and applies it to the window
3. On `Closing`, it captures the current placement and saves it via the storage strategy
4. Window placement includes: position, size, and state (normal/maximized/minimized)

## Design-Time Support

The library automatically detects design-time mode and disables itself in the Visual Studio designer, ensuring a smooth design experience.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Credits

Developed by [Null Software](https://github.com/nullsoftware)

## Repository

[https://github.com/nullsoftware/AutoWindowPlacement.WPF](https://github.com/nullsoftware/AutoWindowPlacement.WPF)
