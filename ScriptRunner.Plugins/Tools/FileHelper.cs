using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
///     Implementation of the <see cref="IFileHelper" /> interface for handling file and directory operations.
/// </summary>
public class FileHelper : IFileHelper
{
    /// <summary>
    ///     Constructs an absolute path based on a relative path, rooted at the current directory.
    /// </summary>
    /// <param name="relativePath">The relative path to a file or directory.</param>
    /// <returns>An absolute path combining the current directory and the provided relative path.</returns>
    public string RelativeToCurrentDirectory(string relativePath)
    {
        return Path.GetFullPath(relativePath, Directory.GetCurrentDirectory());
    }

    /// <summary>
    ///     Ensures the directory exists, creating it if necessary.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to ensure exists.</param>
    public void EnsureDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error ensuring directory exists: {directoryPath}", ex);
        }
    }

    /// <summary>
    ///     Deletes a directory and all its contents recursively.
    /// </summary>
    /// <param name="directoryPath">The directory to delete.</param>
    public void DeleteDirRecursively(string directoryPath)
    {
        try
        {
            if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error deleting directory: {directoryPath}", ex);
        }
    }

    /// <summary>
    ///     Lists all files in a directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to list files in.</param>
    /// <returns>An array of file paths in the directory.</returns>
    public string[] DirectoryList(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            return Directory.GetFiles(directoryPath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error listing files in directory: {directoryPath}", ex);
        }
    }

    /// <summary>
    ///     Checks if a file exists.
    /// </summary>
    /// <param name="filePath">The file path to check.</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    public bool FileExists(string filePath)
    {
        try
        {
            return File.Exists(filePath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error checking if file exists: {filePath}", ex);
        }
    }

    /// <summary>
    ///     Writes text to a file, overwriting if it already exists.
    /// </summary>
    /// <param name="filePath">The file to write to.</param>
    /// <param name="content">The content to write.</param>
    public void WriteFile(string filePath, string content)
    {
        try
        {
            File.WriteAllText(filePath, content);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error writing to file: {filePath}", ex);
        }
    }

    /// <summary>
    ///     Appends the specified content to a file. Creates the file if it does not exist.
    /// </summary>
    /// <param name="filePath">The path of the file to append to.</param>
    /// <param name="content">The content to append to the file.</param>
    public void AppendToFile(string filePath, string content)
    {
        try
        {
            File.AppendAllText(filePath, content);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error appending to file: {filePath}", ex);
        }
    }

    /// <summary>
    ///     Reads the contents of a file into a string.
    /// </summary>
    /// <param name="filePath">The path of the file to read.</param>
    /// <returns>The contents of the file as a string.</returns>
    public string ReadFile(string filePath)
    {
        try
        {
            if (!FileExists(filePath)) throw new FileNotFoundException("File not found.", filePath);

            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error reading file: {filePath}", ex);
        }
    }

    /// <summary>
    ///     Deletes a file if it exists.
    /// </summary>
    /// <param name="filePath">The path of the file to delete.</param>
    public void DeleteFile(string filePath)
    {
        try
        {
            if (FileExists(filePath)) File.Delete(filePath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error deleting file: {filePath}", ex);
        }
    }

    /// <summary>
    ///     Copies a file to a new location. Overwrites the file at the destination if it exists.
    /// </summary>
    /// <param name="sourceFilePath">The source file to copy.</param>
    /// <param name="destinationFilePath">The destination file path.</param>
    public void CopyFile(string sourceFilePath, string destinationFilePath)
    {
        try
        {
            File.Copy(sourceFilePath, destinationFilePath, true);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error copying file from {sourceFilePath} to {destinationFilePath}", ex);
        }
    }

    /// <summary>
    ///     Moves a file to a new location.
    /// </summary>
    /// <param name="sourceFilePath">The source file to move.</param>
    /// <param name="destinationFilePath">The destination file path.</param>
    public void MoveFile(string sourceFilePath, string destinationFilePath)
    {
        try
        {
            File.Move(sourceFilePath, destinationFilePath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error moving file from {sourceFilePath} to {destinationFilePath}", ex);
        }
    }

    /// <summary>
    ///     Extracts the contents of a zip file to the specified destination directory.
    /// </summary>
    /// <param name="zipFilePath">The path of the zip file to extract.</param>
    /// <param name="destinationPath">The directory where the zip contents should be extracted.</param>
    /// <exception cref="ArgumentException">Thrown if the zip file path or destination path is invalid.</exception>
    public void ExtractZip(string zipFilePath, string destinationPath)
    {
        if (string.IsNullOrWhiteSpace(zipFilePath) || !File.Exists(zipFilePath))
            throw new ArgumentException("Invalid zip file path.", nameof(zipFilePath));

        if (string.IsNullOrWhiteSpace(destinationPath))
            throw new ArgumentException("Invalid destination path.", nameof(destinationPath));

        Directory.CreateDirectory(destinationPath);

        ZipFile.ExtractToDirectory(zipFilePath, destinationPath, overwriteFiles: true);
    }

    /// <summary>
    ///     Creates a zip file from the contents of a specified directory.
    /// </summary>
    /// <param name="sourceDirectoryPath">The directory to compress into a zip file.</param>
    /// <param name="zipFilePath">The path where the zip file should be created.</param>
    /// <exception cref="ArgumentException">Thrown if the source directory or zip file path is invalid.</exception>
    public void CreateZip(string sourceDirectoryPath, string zipFilePath)
    {
        if (string.IsNullOrWhiteSpace(sourceDirectoryPath) || !Directory.Exists(sourceDirectoryPath))
            throw new ArgumentException("Invalid source directory path.", nameof(sourceDirectoryPath));

        if (string.IsNullOrWhiteSpace(zipFilePath))
            throw new ArgumentException("Invalid zip file path.", nameof(zipFilePath));

        ZipFile.CreateFromDirectory(sourceDirectoryPath, zipFilePath, CompressionLevel.Optimal, includeBaseDirectory: false);
    }

    /// <summary>
    ///     Validates that a temporary directory contains the required subdirectories.
    /// </summary>
    /// <param name="tempDir">The temporary directory to validate.</param>
    /// <param name="requiredDirectories">An array of directory names that must be present in the temporary directory.</param>
    /// <returns>True if all required directories are present; otherwise, false.</returns>
    public bool ValidateZipStructure(string tempDir, string[] requiredDirectories)
    {
        return Directory.Exists(tempDir) && requiredDirectories.All(dir => Directory.Exists(Path.Combine(tempDir, dir)));
    }

    /// <summary>
    /// Deletes a temporary directory and all its contents.
    /// </summary>
    /// <param name="tempDir">The path of the temporary directory to delete.</param>
    /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
    /// <exception cref="IOException">Thrown if the directory cannot be deleted.</exception>
    public void CleanUpTempDirectory(string tempDir)
    {
        if (!Directory.Exists(tempDir))
        {
            throw new DirectoryNotFoundException($"The directory '{tempDir}' does not exist.");
        }

        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to clean up temporary directory '{tempDir}': {ex.Message}", ex);
        }
    }
}