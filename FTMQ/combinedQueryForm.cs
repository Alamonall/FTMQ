using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FTMQ
{
    public partial class combinedQueryForm : Form
    {
        private bdDataView instBdDataView;
        private List<string> listQuerysNames;

        private List<string> listEgeQuerys;
        private List<string> listOgeQuerys;

        public combinedQueryForm(bdDataView instBdDataView)
        {
            this.instBdDataView = instBdDataView;
            listQuerysNames = new List<string>();
            listEgeQuerys = new List<string>();
            listOgeQuerys = new List<string>();
            InitializeComponent();
        }
        
        private void addingParameters()
        {
            if (instBdDataView.whichBase)
            {
                listEgeQuerys.Add("Класс/Категория");
                listQuerysCheckBox.Items.Add("Класс/Категория");
                listEgeQuerys.Add("Серия/Тип доку");
                listQuerysCheckBox.Items.Add("Серия/Тип доку");
                listEgeQuerys.Add("Дейст.рез/Зарег на жизнь");
                listQuerysCheckBox.Items.Add("Дейст.рез/Зарег на жизнь");
                listEgeQuerys.Add("Наим/Параметр");
                listQuerysCheckBox.Items.Add("Наим/Параметр");
                listEgeQuerys.Add("Тип экз/Параметр");
                listQuerysCheckBox.Items.Add("Тип экз/Параметр");
            }
            else
            {
                listOgeQuerys.Add("My Blood");
                listQuerysCheckBox.Items.Add("My Blood");
                listOgeQuerys.Add("My Blood");
                listQuerysCheckBox.Items.Add("My Blood");
                listOgeQuerys.Add("My Blood");
                listQuerysCheckBox.Items.Add("My Blood");
            }
        }

        private void combinedQueryForm_Load(object sender, EventArgs e){}

        private void execButton_Click(object sender, EventArgs e)
        {
            foreach (String it in listQuerysCheckBox.CheckedItems)
            {
                Console.WriteLine("CheckedItemName = " + it);
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
            instBdDataView.Show();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            listQuerysCheckBox.Items.Clear();
            this.Hide();
            instBdDataView.Show();
        }

        private void listQuerysCheckBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckedListBox box = (CheckedListBox)sender;
            Console.WriteLine("Line = " + box.CheckedItems);
        }

        internal void choiceForm()
        {
            addingParameters();
            this.Show();
        }
    }
}
