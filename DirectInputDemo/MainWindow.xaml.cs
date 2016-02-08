using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using SharpDX.DirectInput;

namespace DirectInputDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Joystick _joyStick;
        DirectInput directInput { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            directInput = new DirectInput();
            GetDevices();
        }

        private void GetDevices()
        {
            var devices = directInput.GetDevices(DeviceType.Gamepad,
                DeviceEnumerationFlags.AllDevices);
            if (devices.Count == 0)
                devices = directInput.GetDevices(DeviceType.Joystick,
                    DeviceEnumerationFlags.AllDevices);
            if (devices.Count == 0)
            {
                eventsDisplay.Text = "No game controllers found.";
                clearButton.Visibility = Visibility.Hidden;
            }
            else
            {
                UseDevices(devices);
            }
        }

        private void UseDevices(IList<DeviceInstance> devices)
        {
            var guid = devices[0].InstanceGuid; // use first one
            _joyStick = new Joystick(directInput, guid);

            _joyStick.Properties.BufferSize = 128; // enable buffer
            _joyStick.Acquire();

            var timer = new DispatcherTimer(); // Timer(onTimer, null, 100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _joyStick.Poll();
            var data = _joyStick.GetBufferedData();
            foreach (var line in data)
            {
                eventsDisplay.AppendText($"Offset: {line.Offset} Value: {line.Value}\n"); 
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            eventsDisplay.Clear();
        }
    }
}
