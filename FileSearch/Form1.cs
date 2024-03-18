using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearchApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Заполняем комбинированный список доступными дисками
            foreach (var drive in DriveInfo.GetDrives())
            {
                comboBoxDrive.Items.Add(drive.Name);
            }
        }

        private async void buttonSearch_Click(object sender, EventArgs e)
        {
            // Получаем выбранный диск
            string selectedDrive = comboBoxDrive.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedDrive))
            {
                MessageBox.Show("Выберите диск для поиска.");
                return;
            }

            // Получаем введенную пользователем маску для поиска
            string searchPattern = textBoxMask.Text;
            if (string.IsNullOrEmpty(searchPattern))
            {
                MessageBox.Show("Введите маску для поиска.");
                return;
            }

            // Очищаем список просмотра перед началом нового поиска
            listViewResults.Items.Clear();

            // Выполняем поиск асинхронно
            await SearchAsync(selectedDrive, searchPattern);
        }

        private async Task SearchAsync(string drive, string searchPattern)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Получаем список файлов и папок в указанном диске
                    DirectoryInfo directory = new DirectoryInfo(drive);
                    FileSystemInfo[] items = directory.GetFileSystemInfos(searchPattern, SearchOption.AllDirectories);

                    // Добавляем найденные элементы в список просмотра
                    foreach (var item in items)
                    {
                        ListViewItem listItem = new ListViewItem(item.Name);
                        listItem.SubItems.Add(item.FullName);
                        listItem.SubItems.Add(item.LastWriteTime.ToString());
                        listItem.SubItems.Add((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? "Папка" : "Файл");

                        // Добавляем элемент в список просмотра (выполняем это действие в UI-потоке)
                        listViewResults.Invoke(new Action(() => listViewResults.Items.Add(listItem)));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при поиске файлов и папок: " + ex.Message);
                }
            });
        }
    }
}
