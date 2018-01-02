using Pets.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pets
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ListPets();
        }

        private async void ListPets()
        {
            List<Pet> pets = await Pet.ListPets();

            petsListView.ItemsSource = pets;
        }

        private void petsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Pet pet = e.SelectedItem as Pet;

            if (pet == null) return;

            Navigation.PushAsync(new PetDetailPage(pet));

        }
    }
}
