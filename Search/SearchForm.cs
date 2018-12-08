namespace Search
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="SearchForm" />
    /// </summary>
    public partial class SearchForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchForm"/> class.
        /// </summary>
        public SearchForm()
        {
            InitializeComponent();
            exploredData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        /// The ExploredDataCellClick
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void ExploredDataCellClick(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", $"/select ,{ exploredData.SelectedCells[1].Value }");
        }

        /// <summary>
        /// The PathSearchBoxClick
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void PathSearchBoxClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog(); //обьект диалога просмотра папок
            if (!string.IsNullOrWhiteSpace(pathSearchBox.Text))
            {
                // если перевыбираем место где искать, диалог открывается с последнего места поиска
                folder.SelectedPath = pathSearchBox.Text;
            }

            if (folder.ShowDialog() == DialogResult.OK)
            {
                pathSearchBox.Text = folder.SelectedPath;
                textSearchBox.Focus();
                searchButton.Enabled = true;
            }
        }

        /// <summary>
        /// The SearchButtonClick
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void SearchButtonClick(object sender, EventArgs e)
        {
            exploredData.DataSource = null;
            string fileName = Path.GetFileNameWithoutExtension(textSearchBox.Text);
            string extension = Path.GetExtension(textSearchBox.Text);
            string pattern = !string.IsNullOrWhiteSpace(fileName) ? !string.IsNullOrWhiteSpace(extension) ? $"{fileName}*{extension}" : $"{fileName}*" : $"*{extension}";

            if (!string.IsNullOrWhiteSpace(pattern))
            {
                try
                {
                    List<string> foundFiles = Directory.GetFileSystemEntries(pathSearchBox.Text,
                           pattern, SearchOption.AllDirectories).ToList();
                    if (foundFiles.Count == 0)
                    {
                        MessageBox.Show("Not found.");
                        textSearchBox.Clear();
                        pathSearchBox.Clear();
                        pathSearchBox.Focus();
                        searchButton.Enabled = false;
                        return;
                    }

                    exploredData.DataSource = foundFiles.Select(file => new
                    {
                        FileName = Path.GetFileName(file),
                        FileDestination = file
                    }).ToList();
                    exploredData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Access exception. Ask your sys admin for help");
                }
            }
        }

        /// <summary>
        /// The TextSearchBoxKeyUp
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="KeyEventArgs"/></param>
        private void TextSearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchButton.PerformClick();
            }
        }
    }
}
