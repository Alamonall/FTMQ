using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FTMQ
{
    public partial class combinedQueryForm : Form
    {
        bdDataView instBdDataView;
        private bool _bd; //при true - ege, false - gia

        public bool Bd { get => _bd; set => _bd = value; }

        public combinedQueryForm(bdDataView instBdDataView)
        {
            this.instBdDataView = instBdDataView;
            InitializeComponent();
        }

        private void combinedQueryForm_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> temp = null;
            if (_bd)
            {
                temp = instBdDataView.SqlCommandsListForEge;
            }
            if (!_bd)
            {
                temp = instBdDataView.SqlCommandsListForGia;
            }

            foreach (KeyValuePair<string, string> item in temp)
            {
                listQuerysCheckBox.Items.Add(item.Key);
            }
        }


        private void execButton_Click(object sender, EventArgs e)
        {
            instBdDataView.executeCombinedQuery(listQuerysCheckBox,_bd);
            this.Hide();
            instBdDataView.Show();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            instBdDataView.Show();
        }

        private void listQuerysCheckBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
