using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using ReactiveUI;

namespace ScriptRunner.Plugins.Models;

/// <summary>
/// Represents a record item that maps to a row in a <see cref="DataTable"/>, 
/// along with its dynamic type information and display properties.
/// </summary>
public class RecordItem : ReactiveObject
{
    private bool _isDirty;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordItem"/> class.
    /// </summary>
    /// <param name="dataRow">The <see cref="DataRow"/> that this record item represents.</param>
    /// <param name="dynamicType">The dynamically generated type defining the schema for the record.</param>
    public RecordItem(DataRow dataRow, Type dynamicType)
    {
        DataRow = dataRow;

        // Construct DisplayName based on fields marked as part of display name
        var displayParts = dynamicType.GetProperties()
            .Where(prop => prop.GetCustomAttribute<FieldWithAttributes>()?.IsDisplayField == true)
            .Select(prop => dataRow[prop.Name].ToString() ?? string.Empty);

        DisplayName = string.Join(" - ", displayParts).Trim();
        if (string.IsNullOrEmpty(DisplayName)) DisplayName = "New!";

        Details = new ObservableCollection<DetailField>(
            dynamicType.GetProperties().Select(prop =>
                new DetailField
                {
                    Label = prop.Name,
                    Value = dataRow[prop.Name].ToString() ?? string.Empty
                })
        );
    }

    /// <summary>
    /// Gets the <see cref="DataRow"/> represented by this record item.
    /// </summary>
    public DataRow DataRow { get; }

    /// <summary>
    /// Gets or sets the display name for the record, which is constructed dynamically 
    /// based on specific properties defined in the dynamic type.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the detailed fields associated with the record. 
    /// These fields are constructed based on the dynamic type's properties.
    /// </summary>
    public ObservableCollection<DetailField> Details { get; set; }

    /// <summary>
    /// Gets a value indicating whether the record has unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        private set => this.RaiseAndSetIfChanged(ref _isDirty, value);
    }

    /// <summary>
    /// Marks the record as dirty, indicating that it has unsaved changes.
    /// </summary>
    public void MarkAsDirty()
    {
        IsDirty = true; // Set the entire RecordItem as dirty
    }

    /// <summary>
    /// Marks the record as clean, indicating that all changes have been saved.
    /// </summary>
    public void MarkAsClean()
    {
        // Reset all DetailFields' IsDirty status and update overall IsDirty
        IsDirty = false;
    }
}