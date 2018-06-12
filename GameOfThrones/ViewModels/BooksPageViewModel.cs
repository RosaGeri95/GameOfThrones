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
    /// ViewModel for the view that shows the list of books
    /// </summary>
    public class BooksPageViewModel : ViewModelBase
    {
        private Book _selectedBook;

        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                Set(ref _selectedBook, value);
                //jumps immediately to the selected book's detailed page
                Select();
            }
        }

        public ObservableCollection<Book> Books { get; set; }

        public BooksPageViewModel()
        {
            Books = new ObservableCollection<Book>();
        }

        private void Select()
        {
            NavigationService.Navigate(typeof(BookDetailsPage), SelectedBook.Url);
        }

        //On navigation, fills up the collection with books
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var service = new GoTService();
            var books = await service.GetBooksAsync();

            foreach (var b in books)
            {
                Books.Add(b);
            }
            await base.OnNavigatedToAsync(parameter, mode, state);
        }
    }
}
