using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OPSW11.Models;

public class SelectableOperation : INotifyPropertyChanged
{
    private bool _isSelected;

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string GroupName { get; init; } = string.Empty;
    public bool IsSafe { get; init; } = true;
    public Func<CancellationToken, Task<OperationResult>>? Execute { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
