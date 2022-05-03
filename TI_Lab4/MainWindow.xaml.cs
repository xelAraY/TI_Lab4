using System;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace TI_Lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string _filePath = String.Empty;

        private static El_Gamal elGamal;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignFile_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                _filePath = ofd.FileName;
                byte[] data = File.ReadAllBytes(ofd.FileName);
                try
                {
                    elGamal = new El_Gamal(int.Parse(PrimeNumQ_TextBox.Text),
                        int.Parse(PrimeNumP_TextBox.Text), int.Parse(h_TextBox.Text),
                        int.Parse(x_TextBox.Text), int.Parse(k_TextBox.Text), data);

                    //elGamal = new El_Gamal(593, 3559, 3, 17, 23, data);

                    (int hash, int r, int s) signature = elGamal.SignatureMessage();

                    if(signature.r == 0 || signature.s == 0)
                    {
                        throw new Exception("Выберите другое значение для k");
                    }

                    Hash_TextBox.Text = Convert.ToString(signature.hash, 10);

                    string? str;
                    using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        str = sw.ReadToEnd();
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append(str);
                    sb.Append($"\nr = {signature.r}, s = {signature.s};\n");
                    SignatureText_TextBox.Text = $"r = {signature.r}, s = {signature.s}";

                    string fileName = Path.GetDirectoryName(_filePath) + '\\' + "New_" +
                            Path.GetFileName(_filePath);
                    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }                 
            }
        }

        private void CheckSignature_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                _filePath = ofd.FileName;               
                byte[] data = File.ReadAllBytes(ofd.FileName);         

                //string pattern = "\\nr = [0-9]+, s = [0-9]+;\\n";

                var message = Encoding.ASCII.GetString(data);

                //var rsString = Regex.Matches(message, "\\nr = [0-9]+, s = [0-9]+;\\n").ToString();
                var mathes = Regex.Matches(message, "[0-9]+");

                if (mathes.Count > 1)
                {
                    int r = int.Parse(mathes[0].ToString());
                    int s = int.Parse(mathes[1].ToString());

                    StringBuilder sb = new StringBuilder();
                    string newMessage = Regex.Replace(message, "\\nr = [0-9]+, s = [0-9]+;\\n", string.Empty);

                    data = Encoding.ASCII.GetBytes(newMessage);

                    if (elGamal.CheckSignature(data, r, s))
                        sb.Append($"Подпись является подлинной, r = v ({r} = {elGamal.V})");
                    else
                        sb.Append($"Подпись не является подлинной, r ≠ v ({r} ≠ {elGamal.V})");

                    CheckSignatureText_TextBox.Text = sb.ToString();
                }
                else
                {
                    MessageBox.Show("Файл не подписан");
                    return;
                }                                            
            }
        }
    }
}
