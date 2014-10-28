using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Audios;
using WavePlayer.Media;
using WavePlayer.Providers;
using WavePlayer.Requests;
using WavePlayer.UI.Collections;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;
using WavePlayer.UI.Properties;
using WavePlayer.UI.Threading;

namespace WavePlayer.UI.ViewModels.Playlists
{
    public abstract class PlaylistViewModel : PageViewModel
    {
        private RelayCommand<Audio> _playCommand;

        protected PlaylistViewModel(IPlayer player, IVkDataProvider dataProvider, INavigationService navigationService, IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            Player = player;
            DataProvider = dataProvider;

            DispatcherHelper.InvokeOnUI(() =>
            {
                Audios = new CustomObservableCollection<Audio>();
            });

            LoadAudiosCommand = new RelayCommand(() => ExecuteAsync(() => LoadCollection(Audios, AudiosSource)), () => CanLoadCollection(AudiosSource));
        }

        public string TracksCount
        {
            get
            {
                var remoteCollection = AudiosSource as RemoteCollection<Audio>;

                if ((remoteCollection != null && remoteCollection.Loaded && remoteCollection.TotalCount == 0) ||
                    (remoteCollection == null && AudiosSource != null && AudiosSource.Count == 0))
                {
                    return Resources.ThereAreNoTracksInThisAlbum;
                }

                return Resources.TracksCount;
            }
        }

        public CustomObservableCollection<Audio> Audios { get; private set; }

        public abstract ICommand SetupAudiosCommand { get; }

        public ICommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                {
                    _playCommand = new RelayCommand<Audio>(PlayTrack);
                }

                return _playCommand;
            }
        }

        public ICommand LoadAudiosCommand
        {
            get;
            private set;
        }

        protected IVkDataProvider DataProvider { get; private set; }

        protected IPlayer Player { get; private set; }

        protected ICollection<Audio> AudiosSource { get; private set; }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();
            RaisePropertyChanged("TracksCount");
        }

        protected bool CanLoadCollection<T>(ICollection<T> collection)
        {
            var remoteCollection = collection as RemoteCollection<T>;

            return !IsLoading &&
                   remoteCollection != null &&
                   remoteCollection.TotalCount > remoteCollection.Count;
        }

        protected void LoadCollection<T>(CustomObservableCollection<T> viewCollection, ICollection<T> sourceCollection)
        {
            SafeExecute(() =>
            {
                if (sourceCollection == null) { return; }

                var count = sourceCollection.Count;

                DataProvider.LoadCollection(sourceCollection);

                viewCollection.AddRange(sourceCollection.Skip(count));
            });
        }

        protected static Task ExecuteAsync(Action action)
        {
            return Task.Factory.StartNew(action);
        }

        protected void SetupAudios(ICollection<Audio> audiosCollection)
        {
            AudiosSource = audiosCollection;

            Audios.Reset(AudiosSource);

            RaisePropertyChanged("TracksCount");
        }

        private void PlayTrack(Track track)
        {
            if (track == null ||
                AudiosSource == null ||
                !AudiosSource.Contains(track))
            {
                return;
            }

            SafeExecute(() =>
            {
                var currentTrack = Player.Track;

                if (track == currentTrack)
                {
                    if (Player.Engine.PlaybackState == PlaybackState.Paused)
                    {
                        Player.Engine.Play();
                    }
                    else
                    {
                        Player.Engine.Pause();
                    }
                }
                else
                {
                    var tracks = Player.Tracklist as Tracklist<Audio>;

                    if (tracks == null ||
                        !tracks.Source.Equals(AudiosSource))
                    {
                        tracks = new Tracklist<Audio>((IList<Audio>)AudiosSource);
                    }

                    Player.PlayTracks(tracks, track);
                }
            },
            longRunning: false);
        }
    }
}
