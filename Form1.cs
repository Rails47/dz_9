using System.Net.Sockets;
using System.Net;
using System.Text;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private UdpClient receivingClient;
        private UdpClient sendingClient;
        private const int port = 12345;
        private string username;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            username = Prompt.ShowDialog("Enter your username:", "Username");
            receivingClient = new UdpClient(port);
            sendingClient = new UdpClient();
            Thread receivingThread = new Thread(new ThreadStart(ReceiveMessages));
            receivingThread.Start();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = $"{username}: {txtMessage.Text}";
            byte[] data = Encoding.ASCII.GetBytes(message);
            sendingClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, port));
            txtMessage.Clear();
        }

        private void ReceiveMessages()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
            while (true)
            {
                byte[] data = receivingClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                AppendMessage(message);
            }
        }

        private void AppendMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendMessage), message);
                return;
            }
            txtChat.AppendText(message + Environment.NewLine);
        }
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 300;
            prompt.Height = 150;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };
            Button confirmation = new Button() { Text = "Ok", Left = 100, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.ShowDialog();
            return textBox.Text;
        }
    }
}
