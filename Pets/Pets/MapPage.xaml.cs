using Pets.Model;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Pets
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private Pet _pet;

        public MapPage()
        {
            InitializeComponent();
        }

        public MapPage(Pet pet)
        {
            InitializeComponent();

            _pet = pet;

            Title = $"{pet.Name.ToUpper()} LOCATION";
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(TimeSpan.Zero, 100);

            var position = await locator.GetPositionAsync();

            var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            locationsMap.MoveToRegion(span);

            DisplayInMap(_pet);
        }

        private void DisplayInMap(Pet pet)
        {
            try
            {
                var position = new Position(pet.Latitude, pet.Longitude);

                var pin = new Xamarin.Forms.Maps.Pin() {
                    Type = Xamarin.Forms.Maps.PinType.SavedPin,
                    Position = position,
                    Label = pet.Name == null ? "teste" : pet.Name
                };

                locationsMap.Pins.Add(pin);

            } catch (NullReferenceException nre)
            {

            } catch (Exception ex)
            {

                throw;
            }
        }

        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            var locator = CrossGeolocator.Current;
            locator.PositionChanged -= Locator_PositionChanged;

            await locator.StopListeningAsync();
        }

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var center = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            locationsMap.MoveToRegion(span);
        }
    }
}