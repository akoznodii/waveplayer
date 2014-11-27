using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class SearchViewModel : MusicViewModelBase, INavigatable
    {
        private string _query;
        private RelayCommand<string> _setupAudiosCommand;

        public SearchViewModel(IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, navigationService, dialogService)
        {
        }

        public override string Title
        {
            get { return Resources.Search; }
        }

        public string Query
        {
            get
            {
                return _query;
            }

            set
            {
                SetField(ref _query, value);
            }
        }

        public override ICommand SetupAudiosCommand
        {
            get
            {
                if (_setupAudiosCommand == null)
                {
                    _setupAudiosCommand = new RelayCommand<string>(q => SetupAudiosAsync(q), (q) => !IsLoading,
                        (o) =>
                        {
                            var textBox = o as TextBox;

                            return textBox != null ? textBox.Text : null;
                        });
                }

                return _setupAudiosCommand;
            }
        }

        public void OnNavigated(object parameter)
        {
            var query = parameter as string;

            if (!string.IsNullOrEmpty(query) && !IsLoading)
            {
                SetupAudiosAsync(query);
            }
        }

        private Task SetupAudiosAsync(string query)
        {
            return Async(() => SafeExecute(() => SetupAudios(query), () => SetupAudiosAsync(query)));
        }

        private void SetupAudios(string query)
        {
            if (!string.Equals(Query, query, StringComparison.OrdinalIgnoreCase))
            {
                Query = query;
            }

            Audios.Clear();

            if (!string.IsNullOrEmpty(query))
            {
                var searchCollection = DataProvider.GetSearchAudios(query);

                SetupAudios(searchCollection);
            }
        }
    }
}
