using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Provides utility methods for managing dialogs in Avalonia applications.
/// </summary>
public static class DialogHelper
{
    /// <summary>
    /// Shows a dialog and ensures the application lifetime and main window are valid.
    /// </summary>
    /// <typeparam name="T">The type of the dialog's result.</typeparam>
    /// <param name="callback">
    /// A callback to execute the dialog's show method.
    /// The callback receives the main application window as a parameter.
    /// </param>
    /// <returns>
    /// A <see cref="Task{T}"/> representing the asynchronous operation.
    /// Returns the dialog result if shown,
    /// or the default value of <typeparamref name="T"/> if the application lifetime or main window is invalid.
    /// </returns>
    public static async Task<T?> ShowDialogAsync<T>(Func<Window, Task<T?>> callback)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: not null
            } desktopLifetime)
        {
            return await callback(desktopLifetime.MainWindow);
        }

        return default;
    }
}