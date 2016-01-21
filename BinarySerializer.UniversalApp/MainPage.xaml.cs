using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BinarySerializer.UniversalApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var beer = new Beer
            {
                Alcohol = 6,

                Brand = "Brand",
                Sort = new List<SortContainer>
                {
                    new SortContainer{Name = "some sort of beer"}
                },
                Brewery = "Brasserie Grain d'Orge"
            };

            DoBS(beer, 100000);
        }

        private void DoBS<T>(T obj, int iterations)
        {
            var ser = new BinarySerialization.BinarySerializer();

            var stopwatch = new Stopwatch();
            using (var ms = new MemoryStream())
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    ser.Serialize(ms, obj);
                }
                stopwatch.Stop();
                SerTextBlock.Text = stopwatch.Elapsed.ToString();
                //Console.WriteLine("BS SER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }

            var dataStream = new MemoryStream();
            ser.Serialize(dataStream, obj);
            byte[] data = dataStream.ToArray();

            using (var ms = new MemoryStream(data))
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    ser.Deserialize<T>(ms);
                    ms.Position = 0;
                }
                stopwatch.Stop();
                DeserTextBlock.Text = stopwatch.Elapsed.ToString();
                //Console.WriteLine("BS DESER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }
    }
}
