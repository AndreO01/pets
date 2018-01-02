using Pets.Model;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pets
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PetDetailPage : ContentPage
    {
        private Pet _pet;

        public PetDetailPage()
        {
            InitializeComponent();
        }

        public PetDetailPage(Pet pet)
        {
            InitializeComponent();
        
            detailsStackLayout.BindingContext = pet;

            _pet = pet;
        }
        
        private async void EditPetImageButton_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Take A Picture", "Cancel", "", "Camera", "Library");
            Debug.WriteLine("Action: " + action);

            switch (action)
            {
                case "Camera":
                    TakeAPhoto();
                    break;

                case "Library":
                    PickPhoto();
                    break;
            }

        }

        private async void TakeAPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions {
                SaveToAlbum = true
            });

            if (file == null) return;

            string path = file.Path;

            _pet.Image = path;

            petImage.Source = ImageSource.FromStream(() => {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

        private async void PickPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Sorry", "Pick a photo is not supported.", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null) return;

            string path = file.Path;

            _pet.Image = path;

            petImage.Source = ImageSource.FromStream(() => {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            _pet.Name = nameEntry.Text;
            _pet.Breed = breedEntry.Text;
            _pet.Description = descriptionEntry.Text;

            if (double.TryParse(weightEntry.Text, out double weight))
                _pet.Weight = weight;

            if (Pet.Update(_pet))
                DisplayAlert("Success", "Pet successfully updated.", "Ok");
            else
                DisplayAlert("Error", "Pet failed to be updated.", "Ok");

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var pet = detailsStackLayout.BindingContext as Pet;
            Navigation.PushAsync(new MapPage(pet));
        }
    }
}