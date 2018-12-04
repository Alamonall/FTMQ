using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FTMQ
{
    public partial class combinedQueryForm : Form
    {
        private bdDataView instBdDataView;
        List<Action> listQuerysNames;

        public combinedQueryForm(bdDataView instBdDataView)
        {
            this.instBdDataView = instBdDataView; //----------            
            InitializeComponent();
        }
        
        private void combinedQueryForm_Load(object sender, EventArgs e){}

        private void execButton_Click(object sender, EventArgs e)
        {
            listQuerysNames = new List<Action>();
            foreach (KeyValuePair<string,Action> it in listQuerysCheckBox.CheckedItems)
            {
                listQuerysNames.Add(it.Value);
            }          
            if(listQuerysNames.Count == 0)
            {
                MessageBox.Show("Выберите параметр");
                return;
            }
            instBdDataView.additionalParameterChecking(listQuerysNames);
            listQuerysCheckBox.DataSource = null;
            this.Hide();
            instBdDataView.Show();            
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            listQuerysCheckBox.DataSource = null; 
            this.Hide();
            instBdDataView.Show();
        }

        private void listQuerysCheckBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckedListBox box = (CheckedListBox)sender;
            Console.WriteLine("Line = " + box.CheckedItems);
        }

        public void addingParameters(Category cat)
        {
            this.Show();
            BindingSource bs = new BindingSource
            {
                DataSource = cat.Methods
            };
            listQuerysCheckBox.DataSource = bs;
            listQuerysCheckBox.DisplayMember = "Key";
        }


        private void selectAllButton_Click(object sender, EventArgs e)
        {
            bool temp = true;
            if(listQuerysCheckBox.CheckedItems.Count != 0)
            {
                temp = false;
            }
            for(int i = 0; i < listQuerysCheckBox.Items.Count; i++)
            {
                listQuerysCheckBox.SetItemChecked(i, temp);
            }
        }
    }
}
