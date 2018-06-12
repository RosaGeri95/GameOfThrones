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
using Windows.UI.Xaml.Navigation;

namespace GameOfThrones.ViewModels
{

    /// <summary>
    /// ViewModel for the detailed book page
    /// </summary>
    public class BookDetailsPageViewModel : ViewModelBase
    {
        #region membervariables

        //Holds the book which details are currently shown on the UI
        private Book _currentBook;
        public Book CurrentBook
        {
            get { return _currentBook; }
            set { Set(ref _currentBook, value); }
        }

        
        private string _selectedCharacter;
        public string SelectedCharacter
        {
            get { return _selectedCharacter; }
            set
            {
                Set(ref _selectedCharacter, value);

                //when an item of the Characters ListView is selected, jumps immediately to the characters detailed page
                SelectCharacter();
            }
        }

        //jumps to the selected character's detailed page
        private void SelectCharacter()
        {
            foreach( var c in _characters)
            {
                if (c.Name == SelectedCharacter)
                {
                    NavigationService.Navigate(typeof(CharacterDetailsPage), c.Url);
                }
            }
        }

        //when an item of the PovCharacters ListView is selected, jumps immediately to the characters detailed page
        private string _selectedPovCharacter;
        public string SelectedPovCharacter
        {
            get { return _selectedPovCharacter; }
            set
            {
                Set(ref _selectedPovCharacter, value);

                SelectPovCharacter();
            }
        }

        //jumps to the selected character's detailed page
        private void SelectPovCharacter()
        {
            foreach (var c in _povCharacters)
            {
                if (c.Name == SelectedPovCharacter)
                {
                    NavigationService.Navigate(typeof(CharacterDetailsPage), c.Url);
                }
            }
        }

        public ObservableCollection<string> CharacterNames { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> PovCharacterNames { get; set; } = new ObservableCollection<string>();

        private List<Character> _characters;
        private List<Character> _povCharacters;

        #endregion

        public BookDetailsPageViewModel()
        {
            _characters = new List<Character>();
            _povCharacters = new List<Character>();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            //gets the book from the passed parameter
            string url = (string)parameter;
            var client = new GoTService();
            CurrentBook = await client.GetBookByUrlAsync(url);

            foreach (var povCharacter in CurrentBook.PovCharacters)
            {
                Character c = await client.GetCharacterByUrlAsync(povCharacter);
               
                //Saves the name field to a collection that is shown on the UI
                PovCharacterNames.Add(c.Name);
                //Saves the entire type, needed for navigation
                _povCharacters.Add(c);
            }

            //sets a specific limit, so the app wont get overwhelmed with too much data
            int limit = 20;
            int i = 0;
            foreach (var character in CurrentBook.Characters)
            {
                if (i >= limit)
                {
                    break;
                }

                Character c = await client.GetCharacterByUrlAsync(character);
                if (c.Name != "")
                {
                    //Saves the name field to a collection that is shown on the UI
                    CharacterNames.Add(c.Name);
                    //Saves the entire type, needed for navigation
                    _characters.Add(c);
                }
                i++;
            }

            await base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
