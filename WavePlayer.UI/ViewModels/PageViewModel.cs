using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WavePlayer.Providers;
using WavePlayer.Requests;
using WavePlayer.UI.Collections;
using WavePlayer.UI.Commands;
using WavePlayer.UI.Dialogs;
using WavePlayer.UI.Navigation;

namespace WavePlayer.UI.ViewModels
{
    public abstract class PageViewModel : ViewModelBase
    {
        private RelayCommand _navigateBackCommand;
        private RelayCommand _reloadCommand;
        private bool _isLoading;
        private int _taskCount;

        protected PageViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            NavigationService = navigationService;

            DialogService = dialogService;
        }

        public virtual string Title { get { return String.Empty; } }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetField(ref _isLoading, value); }
        }

        protected INavigationService NavigationService { get; private set; }

        protected IDialogService DialogService { get; private set; }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();

            RaisePropertyChanged("Title");
        }

        public ICommand NavigateBackCommand
        {
            get
            {
                if (_navigateBackCommand == null)
                {
                    _navigateBackCommand = new RelayCommand(() => NavigationService.NavigateBack(), () => NavigationService.CanNavigateBack());
                }

                return _navigateBackCommand;
            }
        }

        public ICommand ReloadCommand
        {
            get
            {
                if (_reloadCommand == null)
                {
                    _reloadCommand = new RelayCommand(ExecuteReloadAsync, () => !IsLoading);
                }

                return _reloadCommand;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Show error message to the end-user")]
        protected void SafeExecute(Action action, Action retryAction = null, bool longRunning = true)
        {
            if (longRunning)
            {
                IncrementTaskCount();
            }

            Exception exception = null;

            try
            {
                action();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                if (longRunning)
                {
                    DecrementTaskCount();
                }
            }

            if (exception != null)
            {
                OnError(exception, retryAction);
            }
        }

        protected virtual void OnError(Exception exception, Action retryAction)
        {
            if (retryAction == null)
            {
                DialogService.NotifyError(exception);
            }
            else
            {
                DialogService.NotifyError(exception, retryAction);
            }
        }

        protected virtual void Reload()
        {
        }

        protected static Task Async(Action action)
        {
            return Task.Factory.StartNew(action);
        }

        protected bool CanLoadCollection<T>(ICollection<T> collection)
        {
            var remoteCollection = collection as RemoteCollection<T>;

            return !IsLoading &&
                   remoteCollection != null &&
                   remoteCollection.TotalCount > remoteCollection.Count;
        }

        protected void LoadCollection<T>(IVkDataProvider dataProvider, CustomObservableCollection<T> viewCollection, ICollection<T> sourceCollection)
        {
            SafeExecute(() =>
            {
                if (sourceCollection == null) { return; }

                var count = sourceCollection.Count;

                dataProvider.LoadCollection(sourceCollection);

                viewCollection.AddRange(sourceCollection.Skip(count));
            });
        }

        private void ExecuteReloadAsync()
        {
            Async(ExecuteReload);
        }

        private void ExecuteReload()
        {
            SafeExecute(Reload, ExecuteReloadAsync);
        }

        private void IncrementTaskCount()
        {
            Interlocked.Increment(ref _taskCount);

            if (_taskCount > 0)
            {
                IsLoading = true;
            }
        }

        private void DecrementTaskCount()
        {
            Interlocked.Decrement(ref _taskCount);

            if (_taskCount == 0)
            {
                IsLoading = false;
            }
        }
    }
}
