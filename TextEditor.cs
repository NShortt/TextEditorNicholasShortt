/*
 * Author: Nicholas Shorrtt
 * Last Modified: March 22, 2021
 * Description: 
 *  Simple text editor program.  Several basic options and a menu strip where included
 *  Options like cut, copy, and paste for editing text.  The user can save and open text 
 *  files as well as create new ones.
 *  
 */  


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TextEditorNicholasShortt
{
    public partial class formTextEditor : Form
    {
        // Varaible
        string filePath = string.Empty;

        public formTextEditor()
        {
            InitializeComponent();
        }

        #region "Event"

        /// <summary>
        /// Copies selected text, if any, to clipboard
        /// </summary>
        private void CopyClick(object sender, EventArgs e)
        {
            // Checks if there is any text selected
            if (textboxInput.SelectedText.Length != 0)
            {
                // Copy selected text to clipboard
                Clipboard.SetText(textboxInput.SelectedText);
            }
        }

        /// <summary>
        /// Copies selected text, if any, to clipboard and removes it from textbox
        /// </summary>
        private void CutClick(object sender, EventArgs e)
        {
            // Checks if there is any text selected
            if (textboxInput.SelectedText.Length != 0)
            {
                // Copy selected text to clipboard
                Clipboard.SetText(textboxInput.SelectedText);

                // Delete selected text
                textboxInput.SelectedText = "";
            }
        }

        /// <summary>
        /// Appends clipboard text, if any, to where cursor  is currently in textbox
        /// </summary>
        private void PasteClick(object sender, EventArgs e)
        {
            // Check if clipboard has text saved
            if (Clipboard.ContainsText())
            {
                // Check for higlighted text
                if (textboxInput.SelectedText.Length != 0)
                {
                    // Delete selected text
                    textboxInput.SelectedText = "";
                }

                // Appends text in clipboard to the current position of the cursor
                textboxInput.Text = textboxInput.Text.Insert(textboxInput.SelectionStart, Clipboard.GetText());                
            }
        }

        /// <summary>
        /// Calls the save function if there is a file path, if not call the SaveAsClick event
        /// </summary>
        private void SaveClick(object sender, EventArgs e)
        {
            // if there is a file path
            if (!(filePath == string.Empty))
            {
                // Call Save function
                SaveFile();
            }
            else
            {
                // Call save as event
                SaveAsClick(sender, e);
            }
        }

        /// <summary>
        /// Open a save dialoge box to create file path, and if accepted update title and call Save function
        /// </summary>
        private void SaveAsClick(object sender, EventArgs e)
        {
            // Create a new save dialoge
            SaveFileDialog saveFile = new SaveFileDialog();

            // Set the file filters
            saveFile.Filter = "Text files (*.txt)|*.txt|All files(*.*)|*.*";

            // When OK is pressed
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                // Set file path and update title
                filePath = saveFile.FileName;
                UpdateTitle();

                // Call save function
                SaveFile();
            }
        }

        /// <summary>
        /// Opens a dialoge box to select which file to open, then fills the textbox with files contents
        /// </summary>
        private void OpenClick(object sender, EventArgs e)
        {
            if (ConfirmClose())
            { 
                // Create a new open dialoge
                OpenFileDialog openFile = new OpenFileDialog();

                // Set the file filters
                openFile.Filter = "Text files (*.txt)|*.txt|All files(*.*)|*.*";

                // When OK is pressed
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFile.FileName;

                    // Update title
                    UpdateTitle();

                    // Create the file and read streams
                    FileStream fileToAccess = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(fileToAccess);

                    // Fill the textbox with the files contents
                    textboxInput.Text = reader.ReadToEnd();
                    

                    // Close the read stream
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Close the program
        /// </summary>
        private void ExitClick(object sender, EventArgs e)
        {
            if (ConfirmClose())
            {
                Close();
            }
        }

        /// <summary>
        /// Clear the textbox and current filepath. Then update title.
        /// </summary>
        private void NewClick(object sender, EventArgs e)
        {
            if (ConfirmClose())
            {
                // Clear textbox and filepath
                textboxInput.Text = "";
                filePath = string.Empty;

                // Update the title
                UpdateTitle();
            }

        }

        /// <summary>
        /// Display a message box with information on the program
        /// </summary>
        private void AboutClick(object sender, EventArgs e)
        {
            MessageBox.Show("NETD-2202\n" + "\nLab 5\n" + "\nNicholas Shortt", "About");
        }
        #endregion

        #region "Functions"

        /// <summary>
        /// Updates the forms title to include file path, if any.
        /// </summary>
        public void UpdateTitle()
        {
            // Set the standard title
            this.Text = "Text Editor";

            // If the is a filepath
            if (filePath != string.Empty)
            {
                // Add the file path to the title
                this.Text += " - " + filePath;
            }
        }

        /// <summary>
        /// Save the text in the texbox to a file.
        /// </summary>
        public void SaveFile()
        {
            // Create the file and write streams
            FileStream fileToAccess = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileToAccess);

            // Append the list entry to the file
            writer.Write(textboxInput.Text);

            // Close the write stream
            writer.Close();
        }

        public bool ConfirmClose()
        {
            // Create a variable for continuing action
            bool confirm = true;

            // Check for a file path
            if (filePath == string.Empty)
            {
                // If no path check if there is any text
                if (textboxInput.Text.Length != 0)
                {
                    // Ask user if they wish to save and save response
                    DialogResult result = MessageBox.Show("Do you wish to save.", "Save?", MessageBoxButtons.YesNoCancel);

                    // Check response
                    if (result == DialogResult.Yes)
                    {
                        // Run save event if yes
                        SaveAsClick(this, null);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // Change confirm to false if cancel
                        confirm = false;
                    }                    
                }
            }
            else
            {
                // If there is a file path

                // Create the file and read streams
                FileStream fileToAccess = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fileToAccess);

                // Check if the current text is different to the saved text
                if (textboxInput.Text != reader.ReadToEnd())
                {
                    // Ask user if they wish to save and save response
                    DialogResult result = MessageBox.Show("Do you wish to save.", "Save?", MessageBoxButtons.YesNoCancel);

                    // Check response
                    if (result == DialogResult.Yes)
                    {
                        // Close stream here as issues will occure if not done before save function creates one
                        reader.Close();
                        // Call save function
                        SaveFile();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // Change confirm to false if cancel
                        confirm = false;
                    }
                }

                // Close the reader stream
                reader.Close();
            }

            return confirm;
        }

        #endregion


    }
}
