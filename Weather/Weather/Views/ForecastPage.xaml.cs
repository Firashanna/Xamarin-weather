using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Weather.Models;
using Weather.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Weather.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ForecastPage : ContentPage
    {
        OpenWeatherService service;
        GroupedForecast groupedforecast;
        string name="";
        public ForecastPage()
        {
            InitializeComponent();
            service = new OpenWeatherService();
            groupedforecast = new GroupedForecast();
        
        }

        protected override void OnAppearing()
        {
            name = this.Title;
            base.OnAppearing();
            this.Title = "Forecast of " + name;
            MainThread.BeginInvokeOnMainThread(async () => { await LoadForecast(); });

        }


        async void OnButtonClicked(object sender, EventArgs args)
        {
            groupedforecast = new GroupedForecast();
            MainThread.BeginInvokeOnMainThread(async () => { await LoadForecast(); });
        }
        private async Task LoadForecast()
        {

            Task<Forecast> t = service.GetForecastAsync(name);
            var a = await t;

            if (t.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = a;
                groupedforecast.City = a.City;

                groupedforecast.Items = a.Items.GroupBy(x => x.DateTime.Date);




            }
            DataTemplate i = new DataTemplate();

            output.ItemsSource = groupedforecast.Items;
            output.IsGroupingEnabled = true;
            output.GroupDisplayBinding = new Binding("Key");
            output.GroupHeaderTemplate = new DataTemplate(typeof(HeaderCell));
            output.ItemTemplate = new DataTemplate(typeof(ItemCell));

        }
    }


    public class HeaderCell : ViewCell
    {
        public HeaderCell()
        {

            var title = new Label
            {
                TextColor = Color.Blue,
                Font = Font.SystemFontOfSize(NamedSize.Small, FontAttributes.Bold)

            };

            title.SetBinding(Label.TextProperty, "Key", converter: new DateFormatter());
            string s = title.Text;
            View = new StackLayout
            {
                HeightRequest = 25,
                Children = { title }
            };
        }
    }

    public class ItemCell : ViewCell
    {
        public ItemCell()
        {
            var a = this.GetValue(MenuProperty);
            var Time = new Label
            {

                Font = Font.SystemFontOfSize(NamedSize.Small)

            };

            var item = new Label();
            

            Time.SetBinding(Label.TextProperty, "DateTime", converter: new TimeFormatter());
            item.SetBinding(Label.TextProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
            {
                new Binding("Description"),
                new Binding("Temperature"),
                new Binding("WindSpeed"),
            },
                Converter = new AllMultiConverter()
            });
            View = new StackLayout
            {

                Children = { Time, item }
            };
        }
    }



    public class DateFormatter : IValueConverter
    {
    
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime temp = (DateTime)value;
            return temp.ToString("dddd,  MMMM dd, yyyy");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TimeFormatter : IValueConverter
    {
     
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime temp = (DateTime)value;
            return temp.ToString("h:mm tt");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class AllMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{values[0]}, temperature: {values[1]} degC, wind: {values[2]} m/s";
         
        }
        //Detta ska inte omplementras pga inte används..
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
          
            throw new NotImplementedException();
        }
    }

}

