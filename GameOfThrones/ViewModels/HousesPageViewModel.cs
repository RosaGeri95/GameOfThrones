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
    /// ViewModel for the view that shows the list of houses
    /// </summary>
    public class HousesPageViewModel : ViewModelBase
    {
        #region membervariables
        //stores the page we are currently viewing
        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                if (value < 1)
                {
                    Set(ref _pageCount, 1);
                }
                //the maximum page number of the API for the houses is 9, so we shouldn't go higher than that
                else if (value > 9)
                {
                    Set(ref _pageCount, 9);
                }
                else
                {
                    Set(ref _pageCount, value);
                }
            }
        }

        private House _selectedHouse;
        public House SelectedHouse
        {
            get { return _selectedHouse; }
            set
            {
                Set(ref _selectedHouse, value);
                //jumps to the selected house's page of details
                Select();
            }
        }

        //commands needed for navigating between pages
        public DelegateCommand PageRightCommand { get; }
        public DelegateCommand PageLeftCommand { get; }


        public ObservableCollection<House> Houses { get; set; }
        #endregion

        public HousesPageViewModel()
        {
            Houses = new ObservableCollection<House>();
            PageCount = 1;
            PageRightCommand = new DelegateCommand(PageRight);
            PageLeftCommand = new DelegateCommand(PageLeft);
        }

        private void Select()
        {
            NavigationService.Navigate(typeof(HouseDetailsPage), SelectedHouse.Url);
        }

        private async void PageLeft()
        {
            PageCount--;
            await GetHouses();
        }

        private async void PageRight()
        {
            PageCount++;
            await GetHouses();
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await GetHouses();

            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        private async Task GetHouses()
        {
            var service = new GoTService();
            var houses = await service.GetHousesAsync(PageCount);

            Houses.Clear();
            foreach (var h in houses)
            {
                Houses.Add(h);
            }
        }

    }
}