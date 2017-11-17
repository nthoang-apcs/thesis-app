using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThesisApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnSwitch.IsEnabled = false;
            btnRun.IsEnabled = false;
            ToPlaying(false);
        }

        private void ToPlaying(bool v)
        {
            btnPlay.IsEnabled = v;
            btnMoveBackward.IsEnabled = v;
            btnMoveForward.IsEnabled = v;
        }

        bool mode1 = true;
        private void SwitchMode()
        {
            mode1 = !mode1;
            if (mode1)
            {
                btnPlay.IsEnabled = true;
                btnMoveBackward.IsEnabled = true;
                btnMoveForward.IsEnabled = true;
            }
            else
            {
                btnPlay.IsEnabled = false;
                btnMoveBackward.IsEnabled = false;
                btnMoveForward.IsEnabled = false;
            }
        }

        private void Mediaplayer_OpenMedia(object sender, EventArgs e)
        {

        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            ToPlaying(true);
            mediaEL.Play();
            
        }
        
        private void btnMoveBackward_Click(object sender, RoutedEventArgs e)
        {
            mediaEL.Position -= TimeSpan.FromSeconds(1);
        }

        private void btnMoveForward_Click(object sender, RoutedEventArgs e)
        {
            mediaEL.Position += TimeSpan.FromSeconds(1);
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //Prepare filecapture.txt
            using (var fileStream = new StreamWriter("filecapture.txt"))
            {
                var curDir = Directory.GetCurrentDirectory();
                for (int i = 0; i <= numCapture; i++)
                {
                    fileStream.Write(curDir + "\\capture" + i.ToString() + ".png\n");
                }
            }

            // For the example
            //const string ex1 = "C:\\";
            //const string ex2 = "C:\\Dir";

            // Use ProcessStartInfo class
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;
            
            // nthoang: openCV exe app open here
            //startInfo.FileName = "dcm2jpg.exe";
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            // nthoang: Pass in arguments
            //startInfo.Arguments = "-f j -o \"" + ex1 + "\" -z 1.0 -s y " + ex2;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                //using (Process exeProcess = Process.Start(startInfo))
                //{
                //    exeProcess.WaitForExit();
                //}
            }
            catch
            {
                // Log error.
            }
        }


        private int numCapture = 0; 

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = 
                new RenderTargetBitmap(
                    mediaEL.NaturalVideoWidth, mediaEL.NaturalVideoHeight, 
                    96, 96, PixelFormats.Pbgra32);

            rtb.Render(mediaEL);
            Image img = new Image
            {
                Source = BitmapFrame.Create(rtb)
            };
            using (var fileStream = new FileStream("capture" + numCapture.ToString() + ".png", FileMode.Create))
            {
                numCapture++;
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img.Source));
                encoder.Save(fileStream);
                btnRun.IsEnabled = true;
                btnSwitch.IsDefault = true;
            }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files (*.wmv, *.mp3, *.mp4)|*.wmv; *.mp3; *.mp4";
            if (ofd.ShowDialog() == true)
            {
                mediaEL.Source = new Uri(ofd.FileName);
                btnPlay.IsEnabled = true;
            } 

        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            //nthoang: Save image?
        }
        
        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            //nthoang: Save image as?
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            //nthoang: Write more about the application here
        }
    }
}
