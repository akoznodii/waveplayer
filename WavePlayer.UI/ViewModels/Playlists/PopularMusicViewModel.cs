using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.UI.Collections;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.Threading;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public class PopularMusicViewModel : PlaylistViewModel
    {
        private Genre _currentGenre;
        private bool _useFilter;

        private RelayCommand<Genre> _setupAudiosCommand;
        private ICollection<Genre> _genres;

        public PopularMusicViewModel(IPlayer player, IVkDataProvider dataProvider, IDialogService dialogService, INavigationService navigationService)
            : base(player, dataProvider, navigationService, dialogService)
        {
            DispatcherHelper.InvokeOnUI(() =>
            {
                Genres = new CustomObservableCollection<Genre>();
            });
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

        public CustomObservableCollection<Genre> Genres
        {
            get;
            private set;
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

        protected override void Reload()
        {
            base.Reload();

            var currentGenre = CurrentGenre;
            var useFilter = UseFilter;

            ResetGenres();

            _genres = DataProvider.GetGenres();

            Genres.Reset(_genres);

            if (_genres == null)
            {
                return;
            }

            UseFilter = useFilter;

            var genreId = currentGenre != null ? currentGenre.Id : 0;

            currentGenre = _genres.FirstOrDefault(g => g.Id == genreId);

            CurrentGenre = currentGenre;
        }

        private Task SetupAudiosAsync(Genre genre)
        {
            return Task.Factory.StartNew(() => SafeExecute(() => SetupAudios(genre, UseFilter), () => SetupAudiosAsync(genre)));
        }

        private void SetupAudios(Genre genre, bool useFilter)
        {
            if (genre == null)
            {
                return;
            }

            var audiosCollection = DataProvider.GetPopularAudios(genre, useFilter);

            SetupAudios(audiosCollection);

            CurrentGenre = genre;
            UseFilter = useFilter;
        }

        private void ResetGenres()
        {
            ResetAudios();

            _genres = null;

            Genres.Reset(Enumerable.Empty<Genre>());

            CurrentGenre = null;
        }
    }
}
