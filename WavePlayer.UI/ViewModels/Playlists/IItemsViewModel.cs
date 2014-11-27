using System.Windows.Input;
using WavePlayer.Common;
using WavePlayer.UI.Collections;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public interface IItemsViewModel<TModel> where TModel : ModelBase
    {
        string ItemsCount { get; }

        CustomObservableCollection<TModel> Items { get; }

        ICommand SetupItemsCommand { get; }

        ICommand SelectItemCommand { get; }

        ICommand LoadItemsCommand { get; }
    }
}
