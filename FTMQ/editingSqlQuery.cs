using System;
using System.Drawing;
using System.Windows.Forms;

namespace FTMQ
{
    public partial class editQueryForm : Form
    {
        public bdDataView instBdDataView;

        public editQueryForm(bdDataView instance)
        {
            InitializeComponent();
            instBdDataView = instance;
        }

        private void editQueryForm_Load(object sender, EventArgs e){
            queryRichTextBox.Font = new Font("Arial", 12);
            queryRichTextBox.WordWrap = true;
            showingTheQuery();
        }

        /*
         * Показываем последний выполненный запрос 
         * */
        public void showingTheQuery()
        {
            this.Show();
            queryRichTextBox.Text = instBdDataView.MemorizedQuery;
            this.TopMost = true;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            instBdDataView.Show();
        }

        private void bApply_Click(object sender, EventArgs e)
        {
            if (queryRichTextBox.Text == "")
                MessageBox.Show("Запрос пустой");
            else
            {
                instBdDataView.loadingDataInDataGridView(queryRichTextBox.Text);
                instBdDataView.activeCategory = null;
                this.Hide();
            }
        }
    }
}
