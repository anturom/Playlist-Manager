using System.Xml;
using System.Xml.Linq;

namespace Playlist_Manager
{
    public partial class Form1 : Form
    {
        XDocument lib;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();
                    
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        lib = XDocument.Load(fileStream);
                        textBox1.Text = filePath;
                    }
                }
            }
        }
    }
}