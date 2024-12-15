namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Provides utility methods for file and directory operations.
/// </summary>
public interface IFileHelper
{
    /// <summary>
    /// Converts a relative path to an absolute path, based on the current directory.
    /// </summary>
    /// <param name="relativePath">The relative path to be converted.</param>
    /// <returns>An absolute path based on the current directory.</returns>
    string RelativeToCurrentDirectory(string relativePath);

    /// <summary>
    /// Ensures that a directory exists. Creates it if it does not exist.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to check or create.</param>
    void EnsureDirectory(string directoryPath);

    /// <summary>
    /// Deletes a directory and all its contents recursively.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to delete.</param>
    void DeleteDirRecursively(string directoryPath);

    /// <summary>
    /// Lists all files in the specified directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to list files in.</param>
    /// <returns>An array of file paths in the directory.</returns>
    string[] DirectoryList(string directoryPath);

    /// <summary>
    /// Checks if a file exists at the specified path.
    /// </summary>
    /// <param name="filePath">The path of the file to check.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    bool FileExists(string filePath);

    /// <summary>
    /// Writes the specified content to a file, overwriting it if it already exists.
    /// </summary>
    /// <param name="filePath">The path of the file to write to.</param>
    /// <param name="content">The content to write to the file.</param>
    void WriteFile(string filePath, string content);

    /// <summary>
    /// Appends the specified content to a file. Creates the file if it does not exist.
    /// </summary>
    /// <param name="filePath">The path of the file to append to.</param>
    /// <param name="content">The content to append to the file.</param>
    void AppendToFile(string filePath, string content);

    /// <summary>
    /// Reads the contents of a file as a string.
    /// </summary>
    /// <param name="filePath">The path of the file to read.</param>
    /// <returns>The content of the file as a string.</returns>
    string ReadFile(string filePath);

    /// <summary>
    /// Deletes the specified file if it exists.
    /// </summary>
    /// <param name="filePath">The path of the file to delete.</param>
    void DeleteFile(string filePath);

    /// <summary>
    /// Copies a file to a new location. Overwrites the destination file if it already exists.
    /// </summary>
    /// <param name="sourceFilePath">The path of the source file.</param>
    /// <param name="destinationFilePath">The path of the destination file.</param>
    void CopyFile(string sourceFilePath, string destinationFilePath);

    /// <summary>
    /// Moves a file to a new location.
    /// </summary>
    /// <param name="sourceFilePath">The path of the source file.</param>
    /// <param name="destinationFilePath">The path of the destination file.</param>
    void MoveFile(string sourceFilePath, string destinationFilePath);

    /// <summary>
    /// Extracts the contents of a ZIP file to the specified destination directory.
    /// </summary>
    /// <param name="zipFilePath">The path of the ZIP file to extract.</param>
    /// <param name="destinationPath">The destination directory where the files will be extracted.</param>
    void ExtractZip(string zipFilePath, string destinationPath);

    /// <summary>
    /// Creates a ZIP file from the contents of the specified directory.
    /// </summary>
    /// <param name="sourceDirectoryPath">The path of the source directory to compress.</param>
    /// <param name="zipFilePath">The path of the output ZIP file.</param>
    void CreateZip(string sourceDirectoryPath, string zipFilePath);

    /// <summary>
    /// Validates the structure of a directory extracted from a ZIP file.
    /// </summary>
    /// <param name="tempDir">The path of the temporary directory containing the extracted files.</param>
    /// <param name="requiredDirectories">An array of directory names that must exist in the extracted structure.</param>
    /// <returns><c>true</c> if the structure is valid; otherwise, <c>false</c>.</returns>
    bool ValidateZipStructure(string tempDir, string[] requiredDirectories);

    /// <summary>
    /// Deletes a temporary directory and its contents.
    /// </summary>
    /// <param name="tempDir">The path of the temporary directory to delete.</param>
    void CleanUpTempDirectory(string tempDir);
}
