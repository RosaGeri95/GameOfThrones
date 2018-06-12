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
    /// ViewModel for the detailed character page
    /// </summary>
    public class CharacterDetailsPageViewModel : ViewModelBase
    {
        #region membervariables

        //Holds the character whose details are currently shown on the UI
        private Character _currentCharacter;
        public Character CurrentCharacter
        {
            get { return _currentCharacter; }
            set { Set(ref _currentCharacter , value); }
        }

        public ObservableCollection<string> BookNames { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> PovBookNames { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Allegiances { get; set; } = new ObservableCollection<string>();

        private string _fatherName;

        public string FatherName
        {
            get { return _fatherName; }
            set { Set(ref _fatherName, value); }
        }

        private string _motherName;

        public string MotherName
        {
            get { return _motherName; }
            set { Set(ref _motherName, value); }
        }

        private string _spouseName;

        public string SpouseName
        {
            get { return _spouseName; }
            set { Set( ref _spouseName, value); }
        }

        public DelegateCommand FatherCommand { get; }
        public DelegateCommand MotherCommand { get; }
        public DelegateCommand SpouseCommand { get; }

        private List<Book> _books;
        private List<Book> _povBooks;
        private List<House> _allegiances;

        private string _selectedBook;
        public string SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                Set(ref _selectedBook, value);
                //jumps to the detailed page of the selected book
                SelectBook();
            }
        }

        private string _selectedPovBook;
        public string SelectedPovBook
        {
            get { return _selectedPovBook; }
            set
            {
                Set(ref _selectedPovBook, value);
                //jumps to the detailed page of the given book (in which the currently shown character has a point of view chapter)
                SelectPovBook();
            }
        }


        private string _selectedHouse;
        public string SelectedHouse
        {
            get { return _selectedHouse; }
            set
            {
                Set(ref _selectedHouse, value);
                //jumps to the selected house's detailed page
                SelectHouse();
            }
        }
        #endregion

        private void SelectBook()
        {
            foreach (var b in _books)
            {
                if (b.Name == SelectedBook)
                {
                    NavigationService.Navigate(typeof(BookDetailsPage), b.Url);
                }
            }
        }

        private void SelectPovBook()
        {
            foreach (var b in _povBooks)
            {
                if (b.Name == SelectedPovBook)
                {
                    NavigationService.Navigate(typeof(BookDetailsPage), b.Url);
                }
            }
        }
        private void SelectHouse()
        {
            foreach (var h in _allegiances)
            {
                if (h.Name == SelectedHouse)
                {
                    NavigationService.Navigate(typeof(HouseDetailsPage), h.Url);
                }
            }
        }


        public CharacterDetailsPageViewModel()
        {
            //I did this so the View can show "-" if there's no father/mother/spouse found
            FatherName = "";
            MotherName = "";
            SpouseName = "";

            _books = new List<Book>();
            _povBooks = new List<Book>();
            _allegiances = new List<House>();


            FatherCommand = new DelegateCommand(GoToFatherPage);
            MotherCommand = new DelegateCommand(GoToMotherPage);
            SpouseCommand = new DelegateCommand(GoToSpousePage);
        }

        private async void GoToFatherPage()
        {
            if (CurrentCharacter.Father != "")
            {
                NavigationService.Navigate(typeof(CharacterDetailsPage), CurrentCharacter.Father);
            }
            else
            {
                await new MessageDialog("No father specified").ShowAsync();
            }
        }

        private async void GoToMotherPage()
        {
            if (CurrentCharacter.Mother != "")
            {
                NavigationService.Navigate(typeof(CharacterDetailsPage), CurrentCharacter.Mother);
            }
            else
            {
                await new MessageDialog("No mother specified").ShowAsync();
            }
        }

        private async void GoToSpousePage()
        {
            if (CurrentCharacter.Spouse != "")
            {
                NavigationService.Navigate(typeof(CharacterDetailsPage), CurrentCharacter.Spouse);
            }
            else
            {
                await new MessageDialog("No spouse specified").ShowAsync();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            //gets the requested character
            string url = (string)parameter;
            var client = new GoTService();
            CurrentCharacter = await client.GetCharacterByUrlAsync(url);

            //First we need to check if the father data exists, if yes, the app makes a new call to the API to get the father's name
            if (CurrentCharacter.Father != "")
            {
                Character father = await client.GetCharacterByUrlAsync(CurrentCharacter.Father);
                FatherName = father.Name;
            }

            if (CurrentCharacter.Mother != "")
            {
                Character mother = await client.GetCharacterByUrlAsync(CurrentCharacter.Mother);
                MotherName = mother.Name;
            }

            if (CurrentCharacter.Spouse != "")
            {
                Character spouse = await client.GetCharacterByUrlAsync(CurrentCharacter.Spouse);
                SpouseName = spouse.Name;
            }

            foreach (var house in CurrentCharacter.Allegiances)
            {
                House h = await client.GetHouseByUrlAsync(house);
                //types are needed for navigation
                _allegiances.Add(h);
                //name is shown on the UI
                Allegiances.Add(h.Name);
            }

            foreach (var bookName in CurrentCharacter.Books)
            {
                Book b = await client.GetBookByUrlAsync(bookName);
                _books.Add(b);
                BookNames.Add(b.Name);
            }

            foreach (var povBookName in CurrentCharacter.PovBooks)
            {
                Book b = await client.GetBookByUrlAsync(povBookName);
                _povBooks.Add(b);
                PovBookNames.Add(b.Name);
            }

            await base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
