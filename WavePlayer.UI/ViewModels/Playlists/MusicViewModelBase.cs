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
    public abstract class MusicViewModelBase : PageViewModel
    {
        private RelayCommand<Audio> _playCommand;

        protected MusicViewModelBase(IPlayer player, IVkDataProvider dataProvider, INavigationService navigationService, IDialogService dialogService)
            : base(navigationService, dialogService)
        {
            Player = player;
            DataProvider = dataProvider;

            DispatcherHelper.InvokeOnUI(() =>
            {
                Audios = new CustomObservableCollection<Audio>();
            });

            LoadAudiosCommand = new RelayCommand(() => Async(() => LoadCollection(DataProvider, Audios, AudiosSource)), () => CanLoadCollection(AudiosSource));
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

        protected void SetupAudios(ICollection<Audio> audiosCollection)
        {
            AudiosSource = audiosCollection;

            Audios.Reset(AudiosSource);

            RaisePropertyChanged("TracksCount");
        }

        protected void ResetAudios()
        {
            if (AudiosSource == null && Audios.Count == 0)
            {
                return;
            }

            AudiosSource = null;

            Audios.Reset(Enumerable.Empty<Audio>());

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
