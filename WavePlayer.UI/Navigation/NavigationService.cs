using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WavePlayer.UI.ViewModels;

namespace WavePlayer.UI.Navigation
{
    public class NavigationService : INavigationService
    {
        private const int HistoryCapacity = 10;
        private readonly List<NavigationRule> _navigationRules;
        private readonly LinkedList<NavigationHistory> _navigationHistory;
        
        public NavigationService()
        {
            _navigationRules = new List<NavigationRule>();
            _navigationHistory = new LinkedList<NavigationHistory>();
        }

        private NavigationRule CurrentNavigation { get; set; }

        private object CurrentNavigationParamter { get; set; }

        public void Navigate<TViewModel>() where TViewModel : ViewModelBase
        {
            Navigate<TViewModel>(null);
        }

        public void Navigate<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            Navigate<TViewModel>(null, parameter);
        }

        public void Navigate<TViewModel>(TViewModel viewModel, object parameter = null) where TViewModel : ViewModelBase
        {
            var type = viewModel != null ? viewModel.GetType() : typeof(TViewModel);

            Navigate(type.FullName, parameter, false);
        }

        public void NavigateBack()
        {
            var item = Pop();

            if (item == null)
            {
                return;
            }

            Navigate(item.TargetType, item.TargetParameter, true);
        }

        public bool CanNavigateBack()
        {
           return _navigationHistory.Count > 0;
        }

        public void SetupNavigationRule(HostViewModel hostViewModel, ViewModelBase viewModel, bool isJournaled = false)
        {
            _navigationRules.Add(new NavigationRule()
            {
                TargetType = viewModel.GetType().FullName,
                HostViewModel = hostViewModel,
                ChildViewModel = viewModel,
                IsJournaled = isJournaled
            });
        }

        private void Push(NavigationHistory item)
        {
            while (_navigationHistory.Count >= HistoryCapacity)
            {
                _navigationHistory.RemoveLast();
            }

            _navigationHistory.AddFirst(item);
        }

        private NavigationHistory Pop()
        {
            var node = _navigationHistory.First;

            if (node == null)
            {
                return null;
            }

            _navigationHistory.RemoveFirst();

            return node.Value;
        }

        private void Navigate(string targetType, object parameter, bool backNavigation)
        {
            var rule = _navigationRules.FirstOrDefault(r => r.TargetType.Equals(targetType, StringComparison.OrdinalIgnoreCase));

            if (rule == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Navigation rule for type {0} not found", targetType));
            }

            var target = targetType;

            while (!string.IsNullOrEmpty(target))
            {
                target = Navigate(target);
            }

            var previousRule = CurrentNavigation;
            var previousParameter = CurrentNavigationParamter;

            CurrentNavigation = rule;
            CurrentNavigationParamter = parameter;

            var navigatable = rule.ChildViewModel as INavigatable;

            if (navigatable != null)
            {
                navigatable.OnNavigated(parameter);
            }

            if (backNavigation || 
                previousRule == null || 
                !previousRule.IsJournaled)
            {
                return;
            }

            var item = new NavigationHistory()
            {
                TargetParameter = previousParameter,
                TargetType = previousRule.TargetType
            };

            Push(item);
        }

        private string Navigate(string targetType)
        {
            var rule = _navigationRules.FirstOrDefault(r => r.TargetType.Equals(targetType, StringComparison.OrdinalIgnoreCase));

            if (rule == null ||
                rule.HostViewModel == null)
            {
                return null;
            }
            
            var host = rule.HostViewModel;
            var child = rule.ChildViewModel;

            host.CurrentView = child;

            return host.GetType().FullName;
        }
    }
}
