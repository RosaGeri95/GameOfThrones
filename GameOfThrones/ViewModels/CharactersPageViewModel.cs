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
    /// ViewModel for the view that shows the list of characters
    /// </summary>
    public class CharactersPageViewModel : ViewModelBase
    {
        #region membervariables
        //stores the page we are currently viewing
        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
            set {
                if (value < 1)
                {
                    Set(ref _pageCount, 43);
                }
                //the max page number of the API for the characters is 43, so we shouldn't go higher than that
                else if (value > 43)
                {
                    Set(ref _pageCount, 1);
                }
                else
                {
                    Set(ref _pageCount, value);
                }
            }
        }

        private Character _selectedCharacter;
        public Character SelectedCharacter
        {
            get { return _selectedCharacter; }
            set
            {
                Set(ref _selectedCharacter, value);
                //jumps to the selected character's detailed page
                Select();
            }
        }

        //commands needed for paging
        public DelegateCommand PageRightCommand { get; }
        public DelegateCommand PageLeftCommand { get; }

        //command for searching characters by name
        public DelegateCommand SearchCommand { get; }
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { Set(ref _searchText, value); }
        }

        
        public ObservableCollection<Character> Characters { get; set; }
        #endregion

        public CharactersPageViewModel()
        {
            Characters = new ObservableCollection<Character>();

            //needed initialization, to make the search() function work
            SearchText = "";

            PageCount = 1;

            PageRightCommand = new DelegateCommand(PageRight);
            PageLeftCommand = new DelegateCommand(PageLeft);
            SearchCommand = new DelegateCommand(Search);
        }

        private async void Search()
        {
            if (SearchText == "")
            {
                await new MessageDialog("Please give a name!").ShowAsync();
                return;
            }

            List<Character> chars = new List<Character>();
            var service = new GoTService();
            chars = await service.GetCharactersByNameAsync(SearchText);

            if(chars.Count == 0)
            {
                await new MessageDialog("No character found with this name!").ShowAsync();
                return;
            }

            //If there are more characters returned by the API, jumps to the last character's detailed page
            await NavigationService.NavigateAsync(typeof(CharacterDetailsPage), chars.LastOrDefault().Url);
        }

        private void Select()
        {
           NavigationService.Navigate(typeof(CharacterDetailsPage), SelectedCharacter.Url);
        }

        private async void PageLeft()
        {
            PageCount--;
            await GetCharacters();
        }

        private async void PageRight()
        {
            PageCount++;
            await GetCharacters();
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await GetCharacters();

            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        //Gets a list of characters from the requested pagenumber
        private async Task GetCharacters()
        {
            var service = new GoTService();
            var characters = await service.GetCharactersAsync(PageCount);

            Characters.Clear();
            foreach (Character c in characters)
            {
                Characters.Add(c);
            }
        }
    }
}
