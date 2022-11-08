using System.ComponentModel;
using System.Diagnostics;
using System.Xml;

namespace Playlist_Manager
{
    public partial class Form1 : Form
    {
        // The XmlDocument into which the iTunes library file will be deserialized
        XmlDocument doc;
        // The new object to store playlists my way
        Library lib = new Library();

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
                    var root = new XmlDocument();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        root.Load(fileStream);
                        doc = root;
                        textBox1.Text = filePath;
                        
                        // Execute time consuming code on another thread
                        if (backgroundWorker1.IsBusy != true)
                        {
                            // Start the asynchronous operation.
                            backgroundWorker1.RunWorkerAsync();
                        }   
                    }
                }
            }
        }

        private void extractLibrary()
        {
            XmlNodeList keys = doc.LastChild.FirstChild.ChildNodes;
         
            if(keys != null && keys.Count > 0)
            {
                extractPlaylists(keys);
            }            

        }

        private void extractPlaylists(XmlNodeList keys)
        {           
            lib.Playlists = new List<Playlist>();

            foreach (XmlNode key in keys)
            {
                if (key.Name == "key" && key.InnerText == "Playlists")
                {
                    XmlNodeList playlists = key.NextSibling.ChildNodes;
                    if (playlists != null && playlists.Count > 0)
                    {                        
                        foreach (XmlNode playlist in playlists)
                        {
                            Playlist pl = extractPlaylist(playlist);
                            if (pl.Name != null) {
                                lib.Playlists.Add(pl);
                            } 
                        }
                    }
                }
            }
            Debug.WriteLine("Found {0} playlists", lib.Playlists.Count);
        }

        private Playlist extractPlaylist(XmlNode playlist)
        {
            Playlist pl = new Playlist();
            
            if (playlist != null)
            {
                foreach (XmlNode plKey in playlist)
                {
                    if (plKey.Name == "key" && plKey.InnerText == "Name"
                        && plKey.NextSibling.InnerText != "Library"
                        && plKey.NextSibling.InnerText != "Downloaded"
                        && plKey.NextSibling.InnerText != "Music"
                        && plKey.NextSibling.InnerText != "Movies"
                        && plKey.NextSibling.InnerText != "TV Shows"
                        && plKey.NextSibling.InnerText != "Podcasts")
                    {
                        pl.Name = plKey.NextSibling.InnerText;
                        Debug.WriteLine(pl.Name);
                    }

                    // extract tracks
                    if (plKey.Name == "key" && plKey.InnerText == "Playlist Items")
                    {
                        XmlNodeList plItems = plKey.NextSibling.ChildNodes;
                        List<Track> tracks = new List<Track>();
                        if(plItems != null && plItems.Count > 0)
                        {
                            foreach(XmlNode plDict in plItems)
                            {
                                Track track = new Track();
                                int id = Int32.Parse(plDict.LastChild.InnerText);
                                track = getTrackProperties(id);
                                tracks.Add(track);                                                              
                            }
                            pl.Tracks = tracks;                            
                        }
                    }

                }
            }
            return pl;
        }

        Track getTrackProperties(int trackId)
        {           
            XmlNodeList keys = doc.LastChild.FirstChild.ChildNodes;

            foreach (XmlNode key in keys)
            {
                if (key.Name == "key" && key.InnerText == "Tracks")
                {
                    XmlNodeList trackKeys = key.NextSibling.ChildNodes;
                    if (trackKeys != null && trackKeys.Count > 0)
                    {
                        foreach (XmlNode item in trackKeys)
                        {
                            if(item.Name == "key" && Int32.Parse(item.InnerText) == trackId)
                            {
                                Track track = new Track();
                                track.Id = trackId;
                                XmlNodeList trackProperties = item.NextSibling.ChildNodes;
                                foreach(XmlNode prop in trackProperties)
                                {
                                    if(prop.Name == "key" && prop.InnerText == "Artist")
                                    {
                                        track.Artist = prop.NextSibling.InnerText;
                                    }
                                    if (prop.Name == "key" && prop.InnerText == "Name")
                                    {
                                        track.Title = prop.NextSibling.InnerText;
                                    }
                                }
                                return track;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void populateDataGridView()
        {
            dataGridView1.Rows.Clear();
            // Add the data
            foreach(Playlist pl in lib.Playlists)
            {
                int rowId = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[rowId];
                row.Cells["Column1"].Value = pl.Name;

                if(pl.Tracks == null)
                {
                    row.Cells["Column2"].Value = 0;
                } else
                {
                    row.Cells["Column2"].Value = pl.Tracks.Count;
                }                
            }
            
        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                Application.UseWaitCursor = true;
                worker.ReportProgress(0);
                // Perform time consuming operation                    
                extractLibrary();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine("Error: " + e.Error.Message);
            }
            else
            {
                Application.UseWaitCursor = false;
                toolStripStatusLabel1.Text = "Ready.";
                populateDataGridView();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Working... Please wait...";
        }
    }
}