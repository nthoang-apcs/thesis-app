using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ThesisApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MediaElement swapMediaPlayer = new MediaElement()
        {
            Width = 720,
            Height = 360,
            Stretch = Stretch.Fill,
            LoadedBehavior = MediaState.Manual
        };

        public MainWindow()
        {
            InitializeComponent();
            btnSwitch.IsEnabled = false;
            btnRun.IsEnabled = false;
            ToPlaying(false);
            mediaEL.Children.Add(swapMediaPlayer);
        }

        private void ToPlaying(bool v)
        {
            btnPlay.IsEnabled = v;
            btnMoveBackward.IsEnabled = v;
            btnMoveForward.IsEnabled = v;
        }

        bool mode1 = false;
        
        private CircularList<BitmapImage> RunImageList = null;

        private void SwitchMode()
        {
            mode1 = !mode1;
            if (mode1)
            {
                ToPlaying(true);
                RunImageList = null;
                mediaEL.Children.Clear();
                mediaEL.Children.Add(swapMediaPlayer);
                swapMediaPlayer.Play();
                btnCapture.IsEnabled = true;
                btnRun.IsEnabled = false;
                numCapture = 0;
            }
            else
            {
                ToPlaying(false);
                btnMoveBackward.IsEnabled = true;
                btnMoveForward.IsEnabled = true;
                btnCapture.IsEnabled = false;
                btnRun.IsEnabled = false;

                swapMediaPlayer.Stop();

                numCapture = 0;
                mediaEL.Children.Clear();

                //nthoang: filerun.txt image loading when switching mode executed here
                using (var fileStream = new StreamReader("filerun.txt"))
                {
                    var curDir = Directory.GetCurrentDirectory();
                    var imglist = new List<BitmapImage>();
                    while (true)
                    {
                        var runImgPath = fileStream.ReadLine();

                        if (runImgPath == null) break;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(runImgPath);
                        bitmap.EndInit();

                        imglist.Add(bitmap);
                    }
                    RunImageList = new CircularList<BitmapImage>(imglist.Count);
                    imglist.ForEach((runimg) =>
                    {
                        RunImageList.Value = runimg;
                        RunImageList.Next();
                    });
                }
                RunImageList.Reset();
                RunImageList.Next();
                mediaEL.Children.Add(new Image()
                {
                    Source = RunImageList.Value,
                });
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (mode1)
            {
                ToPlaying(true);
                swapMediaPlayer.Play();
            }
        }
        
        private void btnMoveBackward_Click(object sender, RoutedEventArgs e)
        {
            if (mode1)
            {
                swapMediaPlayer.Position -= TimeSpan.FromSeconds(1);
            }
            else
            {
                mediaEL.Children.Clear();
                RunImageList.Previous();
                mediaEL.Children.Add(new Image()
                {
                    Source = RunImageList.Value,
                });
            }
        }

        private void btnMoveForward_Click(object sender, RoutedEventArgs e)
        {
            if (mode1)
            {
                swapMediaPlayer.Position += TimeSpan.FromSeconds(1);
            }
            else
            {
                mediaEL.Children.Clear();
                RunImageList.Next();
                mediaEL.Children.Add(new Image()
                {
                    Source = RunImageList.Value,
                });
            }
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            SwitchMode();
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


            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;

            //startInfo.UseShellExecute = true;
            startInfo.RedirectStandardOutput = false;

            // nthoang: openCV exe app open here
            startInfo.FileName = 
                @"D:\NTH\Study\Projects\Thesis\DummyConsoleApp\DummyConsoleApp\bin\Debug\DummyConsoleApp.exe";
            // nthoang: openCV working directory must be set here
            startInfo.WorkingDirectory = 
                @"D:\NTH\Study\Projects\Thesis\DummyConsoleApp\DummyConsoleApp\bin\Debug\";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            // nthoang: Pass in arguments
            //startInfo.Arguments = " - f j -o \"" + ex1 + "\" -z 1.0 -s y " + ex2;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
                btnRun.IsEnabled = false;
                numCapture = 0;
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
                    swapMediaPlayer.NaturalVideoWidth, swapMediaPlayer.NaturalVideoHeight, 
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
                btnSwitch.IsEnabled = true;
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
                swapMediaPlayer.Source = new Uri(ofd.FileName);
                if (!mode1) {
                    SwitchMode();
                }
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
