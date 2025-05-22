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
using System.Diagnostics;

namespace File_Explorer
{
    public partial class Form1 : Form
    {
        private String currentPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MODIFY DATAGRIDIVEW
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //REFRESH DATAGRIDVIEW
            driveInfo();
        }

        private void refreshDataGridView(){
            try
            {
                String[] filePaths = Directory.GetFiles(currentPath);
                String[] directoryPaths = Directory.GetDirectories(currentPath);

                dataGridView1.Rows.Clear();

                for (int i = 0; i < directoryPaths.Length; i++)
                {
                    dataGridView1.Rows.Add(Properties.Resources.folderImage, Path.GetFileName(directoryPaths[i]), Directory.GetLastWriteTime(directoryPaths[i]).ToString("dd.MM.yyyy  HH:mm"), "Directory", getSize(directoryPaths[i]));
                }

                for (int i = 0; i < filePaths.Length; i++)
                {
                    long length = new FileInfo(filePaths[i]).Length;
                    dataGridView1.Rows.Add(Properties.Resources.fileImage, Path.GetFileName(filePaths[i]), File.GetLastWriteTime(filePaths[i]).ToString("dd.MM.yyyy  HH:mm"), Path.GetExtension(filePaths[i]) + " File", getSize(filePaths[i]));
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception ex)
            {

            }

            backButton.Enabled = true;
            //dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (!checkIfRoot())
            {
                currentPath = Directory.GetParent(currentPath).FullName;
                refreshDataGridView();
            }
            else {
                currentPath = "";
                driveInfo();
            }
        }

        private bool checkIfRoot() {
            DirectoryInfo d = new DirectoryInfo(currentPath);
            bool root = false;
            if (d.Parent == null) {
                root = true;
            }
            return root;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            String fileName = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();

            if (!string.IsNullOrEmpty(currentPath))
            {
                Directory.Exists(currentPath + @"\" + fileName);

                if (Directory.Exists(currentPath + @"\" + fileName))
                {
                    currentPath += @"\" + fileName;
                    refreshDataGridView();
                }
            }
            else {
                currentPath = fileName;
                refreshDataGridView();
            }
        }

        private void driveInfo() {
            DriveInfo[] filePaths = DriveInfo.GetDrives();

            dataGridView1.Rows.Clear();

            for (int i = 0; i < filePaths.Length; i++)
            {
                dataGridView1.Rows.Add(Properties.Resources.hardDriveImage, filePaths[i].Name, "", "Logical Drive", (filePaths[i].TotalSize / 1073741824).ToString() + " GB");
            }

            backButton.Enabled = false;
        }

        private String getSize(String path) {
            String size = "";
            double d = 0.0;

            if (ifDirectory(path))
            {
                d = returnSize(path);
            }
            else{
                d = new FileInfo(path).Length;
            }

            //Debug.WriteLine(d);

            //int counting = 0;

            /*for (int i = 0; i < 5; i++) {
                if ((d / 1024.0) < 1)
                {
                    i = 5;
                    break;
                }
                else {
                    d /= 1024.0;
                    counting++;
                }
            }*/

            size = String.Format("{0:0}", d);

            /*if (counting == 0 && d == 0)
            {
                size = "Empty";
            }
            else {
                switch (counting)
                {
                    case 0:
                        size += " Bytes";
                        break;

                    case 1:
                        size += " KB";
                        break;

                    case 2:
                        size += " MB";
                        break;

                    case 3:
                        size += " GB";
                        break;

                    case 4:
                        size += " TB";
                        break;
                }
            }*/

            return size;
        }

        private double returnSize(String path) {
            double b = 0.0;

            try
            {
                String[] filePaths = Directory.GetFiles(path);
                String[] directoryPaths = Directory.GetDirectories(path);

                for (int i = 0; i < filePaths.Length; i++)
                {
                    b += new FileInfo(filePaths[i]).Length;
                }

                for (int i = 0; i < directoryPaths.Length; i++)
                {
                    b += returnSize(directoryPaths[i]);
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception ex)
            {

            }

            return b;
        }

        private bool ifDirectory(String path) {
            return Directory.Exists(path);
        }

    }
}
