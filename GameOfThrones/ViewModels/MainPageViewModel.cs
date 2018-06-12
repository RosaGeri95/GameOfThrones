using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using GameOfThrones.Models;
using GameOfThrones.Services;
using GameOfThrones.Views;

namespace GameOfThrones.ViewModels
{
    /// <summary>
    /// ViewModel for the main page of the application
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        //command needed the app can navigate to the character/house/book pages
        public DelegateCommand CharacterCommand { get; }
        public DelegateCommand HouseCommand { get; }
        public DelegateCommand BookCommand { get; }

        public MainPageViewModel()
        {
            CharacterCommand = new DelegateCommand(GoToCharacterPage);
            HouseCommand = new DelegateCommand(GoToHousePage);
            BookCommand = new DelegateCommand(GoToBookPage);
        }

        private void GoToBookPage()
        {
            NavigationService.Navigate(typeof(BooksPage));
        }

        private void GoToHousePage()
        {
            NavigationService.Navigate(typeof(HousesPage));
        }

        private void GoToCharacterPage()
        {
            NavigationService.Navigate(typeof(CharactersPage));
        }
    }
}
