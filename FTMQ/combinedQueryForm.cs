using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FTMQ
{
    public partial class combinedQueryForm : Form
    {
        private bdDataView instBdDataView;
        private List<string> listQuerysNames;

        public combinedQueryForm(bdDataView instBdDataView)
        {
            this.instBdDataView = instBdDataView; //----------
            listQuerysNames = new List<string>();
            InitializeComponent();
        }
        
        private void combinedQueryForm_Load(object sender, EventArgs e){}

        private void execButton_Click(object sender, EventArgs e)
        {
            foreach (String it in listQuerysCheckBox.CheckedItems)
            {
                listQuerysNames.Add(it);
            }
            if(listQuerysNames.Count == 0)
            {
                MessageBox.Show("Выберите параметр");
                return;
            }
            instBdDataView.additionalParametrChecking(listQuerysNames);
            listQuerysCheckBox.Items.Clear();
            this.Hide();
            instBdDataView.Enabled = true;
            instBdDataView.Show();            
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            listQuerysCheckBox.Items.Clear();
            this.Hide();
            instBdDataView.Show();
            instBdDataView.Enabled = true;
        }

        private void listQuerysCheckBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckedListBox box = (CheckedListBox)sender;
            Console.WriteLine("Line = " + box.CheckedItems);
        }

        internal void choiceForm(string name)
        {
            addingParameters(name);
            instBdDataView.Enabled = false;
            this.Show();
        }

        private void addingParameters(string srt)
        {
            if (instBdDataView.whichBase)
            {
                switch (srt)
                {
                    case "Участники":
                        {
                            listQuerysCheckBox.Items.Add("Класс/Категория");
                            listQuerysCheckBox.Items.Add("Серия/Тип доку");
                            listQuerysCheckBox.Items.Add("Дейст.рез/Зарег на жизнь");
                            listQuerysCheckBox.Items.Add("Наим/Параметр");
                            listQuerysCheckBox.Items.Add("Тип экз/Параметр");
                            break;
                        }
                }
               
            }
            else
            {
                switch (srt)
                {
                    case "Работники":
                        {
                            listQuerysCheckBox.Items.Add("Проверка на телефон");
                            listQuerysCheckBox.Items.Add("Проверка на почту");
                            listQuerysCheckBox.Items.Add("Проверка на код работника");
                            listQuerysCheckBox.Items.Add("Проверка на код станционарный телефон");
                            listQuerysCheckBox.Items.Add("Проверка на специализацию");
                            listQuerysCheckBox.Items.Add("Проверка на прикрепление к ппэ");
                            listQuerysCheckBox.Items.Add("Проверка на категорию");
                            break;
                        }
                }
            }
        }
    }
}
