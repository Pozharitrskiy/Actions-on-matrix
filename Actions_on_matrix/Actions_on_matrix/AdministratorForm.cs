﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Actions_on_matrix
{


    public partial class AdministratorForm : Form
    {
        List<System.Windows.Forms.TextBox> matrix1 = new List<System.Windows.Forms.TextBox>();
        List<System.Windows.Forms.TextBox> matrix2 = new List<System.Windows.Forms.TextBox>();
        private string _currentUser;
        private DataTable _dataTable;


        public AdministratorForm(string userLogin)
        {
            InitializeComponent();
            for (int i = 7; i > 1; i--)
            {
                matrixSizeLeft.Items.Add(i);
                matrixSizeRight.Items.Add(i);
            }

            if (userLogin == "admin")
                LoadAdminData(userLogin);
            else LoadUserAdminData(userLogin);
            LoadUsersData();
        }


        public AdministratorForm()
        {
            InitializeComponent();
            dataGridView1.DataSource = null;


        }

        private void LoadUsersData()
        {
            int index = 1;
            foreach (string filepath in Directory.GetFiles("Users", "*.txt"))
            {
                if (Path.GetFileNameWithoutExtension(filepath).Contains("Info"))
                {
                    continue;
                }
                else if (Path.GetFileNameWithoutExtension(filepath).Contains("Log"))
                {
                    continue;
                }

                string login, password, name, surname, admin;
                using (StreamReader sr = new StreamReader(filepath))
                {
                    login = sr.ReadLine();
                    password = sr.ReadLine();
                    name = sr.ReadLine();
                    surname = sr.ReadLine();
                    admin = "";
                    if (sr.ReadLine() == "True")
                        admin = "*";
                }

                if (!File.Exists("Users\\" + Path.GetFileNameWithoutExtension(filepath) + "Info.txt"))
                {
                    UpdateUserInfo("0", "0", Path.GetFileNameWithoutExtension(filepath));
                }

                string edit, warning;
                using (StreamReader sr = new StreamReader("Users\\" + Path.GetFileNameWithoutExtension(filepath) + "Info.txt"))
                {
                    edit = sr.ReadLine();
                    warning = sr.ReadLine();
                }

                dataGridView1.Rows.Add(index + admin, login, password, name, surname, warning, edit);
                index++;
            }

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                comboBox.Items.Add(column.HeaderText);
            }
            comboBox.SelectedIndex = 0;

            _dataTable = new DataTable();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                _dataTable.Columns.Add(column.HeaderText);
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataRow dataRow = _dataTable.NewRow();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dataRow[cell.ColumnIndex] = cell.Value;
                }
                _dataTable.Rows.Add(dataRow);
            }
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = _dataTable;
            label26.Text = "Количество записей: " + dataGridView1.Rows.Count;
        }

        private void LoadUserAdminData(string userLogin)
        {
            if (!File.Exists($"Users\\{userLogin}Info.txt"))
            {
                UpdateUserInfo("0", "0", userLogin);
            }

            using (StreamReader sr = new StreamReader($"Users\\{userLogin}Info.txt"))
            {

                profileEditCount.Text = sr.ReadLine();
                warningsCount.Text = sr.ReadLine();
            }

            using (StreamReader sr = new StreamReader($"Users\\{userLogin}.txt"))
            {
                userLoginBox.Text = sr.ReadLine();
                userPasswordBox.Text = sr.ReadLine();
                userNameBox.Text = sr.ReadLine();
                userSurnameBox.Text = sr.ReadLine();
            }


            label12.Text = userLogin;
            label17.Text = userLogin;
            _currentUser = userLogin;
        }

        private void LoadAdminData(string userLogin)
        {
            if (!File.Exists($"Users\\{userLogin}Info.txt"))
            {
                using (StreamWriter sw = new StreamWriter($"Users\\{userLogin}Info.txt", true))
                    sw.Write("0\n0\n0\n");
            }

            using (StreamReader sr = new StreamReader($"Users\\{userLogin}Info.txt"))
            {

                profileEditCount.Text = sr.ReadLine();
                warningsCount.Text = sr.ReadLine();
            }

            label2.Text = userLogin;
            label26.Text = userLogin;
            label17.Text = userLogin;
            _currentUser = userLogin;
        }



        private void UpdateLogInfo(string messageToLog, string user)
        {
            DateTime time;
            time = DateTime.Now;
            using (StreamWriter sw = new StreamWriter($"Users\\{user}Log.txt", true))
            {
                sw.Write($"{messageToLog} ; {time.ToShortTimeString()}\n");
            }
        }
        private void UpdateLogInfo(string message, string dataToLog, string user)
        {
            DateTime time;
            time = DateTime.Now;
            using (StreamWriter sw = new StreamWriter($"Users\\{user}.txt", true))
            {
                sw.WriteLine($"{message} - {dataToLog}: {time.ToShortTimeString()}\n");
            }
        }


        private void UpdateUserInfo(string edits, string warnings, string user)
        {
            using (StreamWriter sw = new StreamWriter($"Users\\{user}Info.txt"))
            {
                sw.Write($"\n{edits}\n{warnings}\n");
            }
            if (_currentUser == "admin")
                LoadAdminData(_currentUser);
            else
            {
                LoadUserAdminData(_currentUser);
                LoadUsersData();
            }
        }

        private void addNewAdmin_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow.Cells[0].Value.ToString().Contains("*"))
            {
                MessageBox.Show("Данный пользователь уже является администратором.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                warningsCount.Text = (Int32.Parse(warningsCount.Text) + 1).ToString();
                UpdateUserInfo(profileEditCount.Text, warningsCount.Text, _currentUser);

                return;
            }
            dataGridView1.CurrentRow.Cells[0].Value = dataGridView1.CurrentRow.Cells[0].Value.ToString() + '*';
            UpdateUserData(dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[4].Value.ToString());
            MessageBox.Show("Данный пользователь назначен администратором.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateLogInfo("Пользователь " + dataGridView1.CurrentRow.Cells[1].Value.ToString() + " был назначен администратором ", _currentUser);
        }

        private void UpdateUserData(string login, string password, string name, string surname)
        {
            using (StreamWriter sw = new StreamWriter($"Users\\{login}.txt"))
            {
                sw.Write($"{login}\n{password}\n{name}\n{surname}\n{true}\n");
            }
        }

        private void DeleteSelectedUser()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                deleteButton.Enabled = true;
                // получаем имя пользователя из выбранной строки dataGridView
                string username = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                string messageToLog = $"Был удален пользователь под следующим именем";
                UpdateLogInfo(messageToLog + " " + username, _currentUser);
                // вызываем функцию для удаления пользователя
                DeleteUser(username);
            }
        }

        private void DeleteUser(string username)
        {
            string userFilePath = $"Users\\{username}.txt";
            string userInfoFilePath = $"Users\\{username}Info.txt";

            if (File.Exists(userFilePath))
            {
                File.Delete(userFilePath);
            }

            if (File.Exists(userInfoFilePath))
            {
                File.Delete(userInfoFilePath);
            }

            // перезагрузка данных пользователей, чтобы обновить таблицу
            dataGridView1.Rows.Clear();
            LoadUsersData();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteSelectedUser();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
            {
                _dataTable.DefaultView.RowFilter = null;
                return;
            }

            try
            {
                _dataTable.DefaultView.RowFilter = string.Format("{0} LIKE '%{1}%'", comboBox.SelectedItem, textBox7.Text);
            }
            catch
            {
                _dataTable.DefaultView.RowFilter = string.Format("[{0}] = '{1}'", comboBox.SelectedItem, textBox7.Text);
            }
        }

        private void editButton_Click_1(object sender, EventArgs e)
        {
            if (_currentUser == "admin")
                MessageBox.Show("Главный администратор не может редактировать свои данные", "Ошибка редактирования", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            string messageToLog = "AdminEditLogs";
            new EditProfileForm(userLoginBox.Text, userPasswordBox.Text, userNameBox.Text, userSurnameBox.Text, true).ShowDialog();
            profileEditCount.Text = (Int32.Parse(warningsCount.Text) + 1).ToString();
            UpdateUserInfo(profileEditCount.Text, warningsCount.Text, _currentUser);
            LoadUserAdminData(userLoginBox.Text);
            UpdateLogInfo("Имя", userNameBox.Text, messageToLog);
            UpdateLogInfo("Фамилия", userSurnameBox.Text, messageToLog);
            UpdateLogInfo("Логин", userLoginBox.Text, messageToLog);
            UpdateLogInfo("Пароль", userPasswordBox.Text, messageToLog);
        }


        private void matrixSizeLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            matrix1.Clear();
            int size = (int)matrixSizeLeft.SelectedItem;

            matrixOne.Controls.Clear();
            matrixOne.ColumnCount = size;
            matrixOne.RowCount = size;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
                    matrix1.Add(textBox);
                    textBox.Width = 30;
                    textBox.Height = 20;
                    textBox.KeyPress += TextBox_KeyPress;
                    matrixOne.Controls.Add(textBox, i, j);
                }
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)
                           && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void matrixSizeRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            matrix2.Clear();
            int size = (int)matrixSizeRight.SelectedItem;

            matrixTwo.Controls.Clear();
            matrixTwo.ColumnCount = size;
            matrixTwo.RowCount = size;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    System.Windows.Forms.TextBox textBox1 = new System.Windows.Forms.TextBox();
                    matrix2.Add(textBox1);
                    textBox1.Width = 30;
                    textBox1.Height = 20;
                    textBox1.KeyPress += TextBox_KeyPress;
                    matrixTwo.Controls.Add(textBox1, i, j);
                }
            }
        }

        private void actionButton_Click(object sender, EventArgs e)
        {


            if ((int)matrixSizeLeft.SelectedItem != (int)matrixSizeRight.SelectedItem)
            {
                MessageBox.Show("Мартицы должны быть одинакового размера");
            }
            else
            {
                int size = (int)matrixSizeRight.SelectedItem;
                double[,] result = new double[size, size];
                switch (actionBox.SelectedItem.ToString())
                {
                    case "+":
                        for (int i = 0; i < matrix1.Count; i++)
                        {
                            double a = Double.Parse(matrix1[i].Text);
                            double b = Double.Parse(matrix2[i].Text);

                            result[i / size, i % size] = a + b;
                        }
                        break;

                    case "-":
                        for (int i = 0; i < matrix1.Count; i++)
                        {
                            double a = Double.Parse(matrix1[i].Text);
                            double b = Double.Parse(matrix2[i].Text);

                            result[i / size, i % size] = a - b;
                        }
                        break;

                    case "*":
                        for (int i = 0; i < matrix1.Count; i++)
                        {
                            double a = Double.Parse(matrix1[i].Text);
                            double b = Double.Parse(matrix2[i].Text);

                            result[i / size, i % size] = a * b;
                        }
                        break;

                }
                string message = "Результат:";

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        message += result[i, j].ToString().PadLeft(8);
                    }
                    message += "\n";
                }

                MessageBox.Show(message);

            }

        }


    }
}
