using GameOfThrones.Models;
using GameOfThrones.Services;
using GameOfThrones.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace GameOfThrones.ViewModels
{
    /// <summary>
    /// ViewModel for the detailed house page
    /// </summary>
    public class HouseDetailsPageViewModel : ViewModelBase
    {
        #region membervariables

        //Holds the requested house's details that are currently shown on the UI
        private House _currentHouse;
        public House CurrentHouse
        {
            get { return _currentHouse; }
            set { Set(ref _currentHouse, value); }
        }

        public ObservableCollection<string> CadetBranches { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> SwornMembers { get; set; } = new ObservableCollection<string>();


        //Need to save the name of the current lord, heir and founder separately, because originally it's their Url that's stored in the house class
        private string _currentLord;
        public string CurrentLord
        {
            get { return _currentLord; }
            set { Set(ref _currentLord, value); }
        }

        private string _heir;
        public string Heir
        {
            get { return _heir; }
            set { Set(ref _heir, value); }
        }

        private string _founder;
        public string Founder
        {
            get { return _founder; }
            set { Set(ref _founder, value); }
        }

        //Command needed so the app can jump to the requested character's detailed page
        public DelegateCommand CurrentLordCommand { get; }
        public DelegateCommand HeirCommand { get; }
        public DelegateCommand FounderCommand { get; }

        //stores the members that are sworn to the house
        private List<Character> _members;
        private string _selectMember;
        public string SelectedMember
        {
            get { return _selectMember; }
            set
            {
                Set(ref _selectMember, value);
                //jumps to the selected member's detailed page
                SelectMember();
            }
        }

        private List<House> _branches;
        private string _selectedBranch;
        public string SelectedBranch
        {
            get { return _selectedBranch; }
            set
            {
                Set(ref _selectedBranch, value);
                //jumps to the detailed page of the house branch
                SelectBranch();
            }
        }
        #endregion

        private void SelectMember()
        {
            foreach (var c in _members)
            {
                if (c.Name == SelectedMember)
                {
                    NavigationService.Navigate(typeof(CharacterDetailsPage), c.Url);
                }
            }
        }

        private void SelectBranch()
        {
            foreach (var h in _branches)
            {
                if (h.Name == SelectedBranch)
                {
                    NavigationService.Navigate(typeof(HouseDetailsPage), h.Url);
                }
            }
        }

        public HouseDetailsPageViewModel()
        {
            //Needed so the UI can show "-" if there's no founder, heir or current lord found
            Founder = "";
            Heir = "";
            CurrentLord = "";

            _members = new List<Character>();
            _branches = new List<House>();

            CurrentLordCommand = new DelegateCommand(GoToCurrentLordPage);
            HeirCommand = new DelegateCommand(GoToHeirPage);
            FounderCommand = new DelegateCommand(GoToFounderPage);
        }

        private async void GoToCurrentLordPage()
        {
            if (CurrentHouse.CurrentLord != "")
            {
                NavigationService.Navigate(typeof(CharacterDetailsPage), CurrentHouse.CurrentLord);
            }
            else
            {
                await new MessageDialog("There's no current lord specified in the API").ShowAsync();
            }
        }
        private async void GoToHeirPage()
        {
            if (CurrentHouse.Heir != "")
            {
                NavigationService.Navigate(typeof(CharacterDetailsPage), CurrentHouse.Heir);
            }
            else
            {
                await new MessageDialog("There's no heir specified in the API").ShowAsync();
            }
        }
        private async void GoToFounderPage()
        {
            if (CurrentHouse.Founder != "")
            {
                NavigationService.Navigate(typeof(CharacterDetailsPage), CurrentHouse.Founder);
            }
            else
            {
                await new MessageDialog("There's no founder specified in the API").ShowAsync();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            string url = (string)parameter;
            var client = new GoTService();
            CurrentHouse = await client.GetHouseByUrlAsync(url);

            //Only request the name of the current lord if their Uri was given by the web API
            if (CurrentHouse.CurrentLord != "")
            {
                Character lord = await client.GetCharacterByUrlAsync(CurrentHouse.CurrentLord);
                CurrentLord = lord.Name;
            }

            //Only request the name of the heir if their Uri was given by the web API

            if (CurrentHouse.Heir != "")
            {
                Character heir = await client.GetCharacterByUrlAsync(CurrentHouse.Heir);
                Heir = heir.Name;
            }

            //Only request the name of the founder if their Uri was given by the web API
            if (CurrentHouse.Founder != "")
            {
                Character founder = await client.GetCharacterByUrlAsync(CurrentHouse.Founder);
                Founder = founder.Name;
            }

            foreach (var member in CurrentHouse.SwornMembers)
            {
                Character c = await client.GetCharacterByUrlAsync(member);
                //saves the character object, needed for navigation
                _members.Add(c);
                //saves the character's name separately, needed so it can be shown on the UI
                SwornMembers.Add(c.Name);
            }

            foreach (var house in CurrentHouse.CadetBranches)
            {
                House h = await client.GetHouseByUrlAsync(house);
                _branches.Add(h);
                CadetBranches.Add(h.Name);
            }

            await base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
