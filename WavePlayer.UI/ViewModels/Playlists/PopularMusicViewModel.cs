using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class PopularMusicViewModel : PlaylistViewModel
    {
        private Genre _currentGenre;
        private bool _useFilter;

        private RelayCommand<Genre> _setupAudiosCommand;
        private RelayCommand _setupGenresCommand;

        public PopularMusicViewModel(IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, navigationService, dialogService)
        {
        }

        public override string Title
        {
            get { return Resources.Popular; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string GenresCount
        {
            get { return Resources.Genres; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string OnlyForeignArtists
        {
            get { return Resources.OnlyForeignArtists; }
        }

        public bool UseFilter
        {
            get
            {
                return _useFilter;
            }

            set
            {
                SetField(ref _useFilter, value);
            }
        }

        public Genre CurrentGenre
        {
            get
            {
                return _currentGenre;
            }

            set
            {
                SetField(ref _currentGenre, value);
            }
        }

        public IEnumerable<Genre> Genres
        {
            get { return DataProvider.GetGenres(); }
        }

        public override ICommand SetupAudiosCommand
        {
            get
            {
                if (_setupAudiosCommand == null)
                {
                    _setupAudiosCommand = new RelayCommand<Genre>((genre) => SetupAudiosAsync(genre), (genre) => !IsLoading);
                }

                return _setupAudiosCommand;
            }
        }

        public ICommand SetupGenresCommand
        {
            get
            {
                if (_setupGenresCommand == null)
                {
                    _setupGenresCommand = new RelayCommand(() =>
                    {
                        if (AudiosSource != null)
                        {
                            SetupAudios(AudiosSource);
                        }
                    });
                }

                return _setupGenresCommand;
            }
        }

        private Task SetupAudiosAsync(Genre genre)
        {
            return Task.Factory.StartNew(() => SafeExecute(() => SetupAudios(genre), () => SetupAudiosAsync(genre)));
        }

        private void SetupAudios(Genre genre)
        {
            if (genre == null)
            {
                return;
            }

            var audiosCollection = DataProvider.GetPopularAudios(genre, _useFilter);

            SetupAudios(audiosCollection);
        }
    }
}
