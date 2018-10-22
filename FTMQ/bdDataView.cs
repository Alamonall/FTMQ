using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace FTMQ
{
    //Controller
    public partial class bdDataView : Form 
    {
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();
        private BindingSource bindingSource1 = new BindingSource();

        private editQueryForm editQueryForm;
        private combinedQueryForm combinedQueryForm;
        private string connectionString = @"Data Source=172.16.22.1;
                                            Integrated Security=SSPI;
                                            Trusted_Connection=yes;
                                            Connection Timeout = 60";
        private string tempSqlCommand = "pew pew pew"; //запрос для тех, кто хочет его посмотреть
        private static string queryForEge = "use erbd_ege_reg_18_38 Select p.Surname + ' ' + p.Name + ' ' + p.SecondName as [ФИО]," +
                        "pc.CategoryName as [Категория участника], p.ParticipantCategoryFK as [Код Категории] ," +
                        "p.pClass as [Класс] from rbd_Participants as p " +
                        "inner join dbo.rbdc_ParticipantCategories as pc on p.ParticipantCategoryFK = pc.CategoryID";

        private static string queryForGia = " use erbd_gia_reg_18_38  Select p.Surname + ' ' + p.Name + ' ' + p.SecondName as [ФИО]," +
                       "pc.CategoryName as [Категория участника], p.ParticipantCategoryFK as [Код Категории] ," +
                       "p.pClass as [Класс] from rbd_Participants as p " +
                       "inner join rbdc_ParticipantCategories as pc on p.ParticipantCategoryFK = pc.CategoryID";

        private bool whichBase = true; // true при егэ, false при гиа
                                       //private string queryText = "";

        Dictionary<String, String> sqlCommandsListForGia = new Dictionary<String, String>();
        Dictionary<String, String> sqlCommandsListForEge = new Dictionary<String, String>();

        public bdDataView() {     
            InitializeComponent();
            gettingSqlCommands();
        }

        private void BdDataView_Load(object sender, EventArgs arg) {}

        //флажок на то, какая бд активна в данный момент егэ или гиа
        public bool baseSelection
        {
            get {
                 return whichBase;
            }
            set
            {
                whichBase = value;
                loadingDataInDataGridView(getQuery(),false);
            }
        }

        public Dictionary<string, string> SqlCommandsListForGia { get => sqlCommandsListForGia; set => sqlCommandsListForGia = value; }
        public Dictionary<string, string> SqlCommandsListForEge { get => sqlCommandsListForEge; set => sqlCommandsListForEge = value; }

        //вовзращает активный запрос // на активную бд на основе фложка whichBase
        public string getQuery()
        {
            //MessageBox.Show("getQuery = " + tempSqlCommand);
            //if (whichBase)
            //    return queryForEge;
            //else
            //    return queryForGia;
            return tempSqlCommand;
        }

        /*          
         *подгружаем данные из бд
         * написать обработчик обрывы соединения------------------------------------------------------------ 
         */
        public void loadingDataInDataGridView(string queryText, bool unRules)
        {
            tempSqlCommand = queryText;
            try
            {
                if (dataGridView1.DataSource != null)
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();
                }

                dataGridView1.DataSource = bindingSource1;
                dataAdapter = new SqlDataAdapter(tempSqlCommand, connectionString);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                DataTable table = new DataTable();
                dataAdapter.Fill(table);

                //при редактированном запросе некоторые правила не смогут сработать, 
                //чтобы избежать ошибок программы было добавлены правило unRules, означающее пользовательсикй запрос
                if (!unRules)
                {
                    bindingSource1.DataSource = ChekingCategoryClass(table);
                    this.dataGridView1.Columns["ФИО"].Frozen = true; // замораживаем столбец с фио пользователя
                    this.dataGridView1.Columns["Код Категории"].Visible = false;
                    this.dataGridView1.Columns["Ошибки"].Frozen = true; // замораживаем столбец с описанием ошибки
                }
                else
                    bindingSource1.DataSource = table;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка \\/( * * )\\/: " + ex.Message);
            }
        }
        /// 
        /// Учитывая, что все ошибки будут уникальны и их нельзя будет систематизировать,
        /// данный метод предназначен для проверки на правильность введения поля класса на основе категории ученика
        /// 
        public DataTable ChekingCategoryClass(DataTable dt)
        {
            Debug.WriteLine("rowCount = " + dt.Rows.Count);
            //if(dt.Columns["Ошибки"].)
                dt.Columns.Add("Ошибки"); //добавляем столбец с описанием ошибок
            
            foreach (DataRow row in dt.Rows)
            {                
                switch (row["Код Категории"])
                {
                    case 1:
                        if (Regex.IsMatch((string)row["Класс"], @"^0\S*", RegexOptions.IgnoreCase))
                            row["Ошибки"] = "Не правильно заполнение поле \"Класс\"";
                        else
                            row.Delete();
                        break;
                    case 4:
                        if (!row["Класс"].Equals("0"))
                           row["Ошибки"] = "Не правильно заполнение поле \"Класс\"";
                        else
                            row.Delete();
                        break;
                    case 3:
                        if (!row["Класс"].Equals("0"))
                           row["Ошибки"] = "Не правильно заполнение поле \"Класс\"";
                        else
                            row.Delete();
                        break;
                    case 8:
                        if (Regex.IsMatch((string)row["Класс"], @"^0\S{isCyrillic}*", RegexOptions.IgnoreCase))
                            row["Ошибки"] = "Не правильно заполнение поле \"Класс\"";
                        else
                            row.Delete();
                        break;
                }
            }
            return dt;
        }

        public void ExportToExcel()
        {

        }

        private void BdDataView_Closing(object sender, CancelEventArgs e)
        {
            Application.Exit();
        }

        //обработка окна редактирования запроса
        private void editQueryToolStripMenuItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editQueryForm == null)
                editQueryForm = new editQueryForm(this);
            editQueryForm.showingTheQuery();
        }

        //получение экземпляра выпадающего меню для егэ и гиа
        public ToolStripMenuItem getToolStripMenuItem (string str)
        {
            if (str == "ЕГЭ")
                return egeToolStripMenuItem;
            if (str == "ГИА")
                return giaToolStripMenuItem;
            return null;
        }

        public MenuStrip getMenuStrip() => menuStrip1;

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e) => previewWindow.Text = "" + dataGridView1[e.ColumnIndex, e.RowIndex].Value;

        [STAThread]
        static void Main()
        {                   
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new bdDataView());           
        }
        
        private void giaQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(combinedQueryForm == null)
                combinedQueryForm = new combinedQueryForm(this);
            combinedQueryForm.Bd = false;
            combinedQueryForm.Show();
        }

        private void egeQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (combinedQueryForm == null)
                combinedQueryForm = new combinedQueryForm(this);
            combinedQueryForm.Bd = true;
            combinedQueryForm.Show();
        }

        /*
         * Функция для получения файлов запросов и сохранение их в списки для выполнения через выпадающее меню
         * */
        private void gettingSqlCommands()
        {
            try
            {
                foreach (string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "SqlCommands\\Ege\\", "*.sql"))
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding(1251)))
                    {
                        String line = sr.ReadToEnd();
                        sqlCommandsListForEge.Add(Path.GetFileNameWithoutExtension(file), line);
                        ToolStripMenuItem anotherOne = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file));
                        anotherOne.Click += new EventHandler(this.execSqlCommandForEgeEvent);
                        getToolStripMenuItem("ЕГЭ").DropDownItems.Add(anotherOne);
                    }
                }

                foreach (string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "SqlCommands\\Gia\\", "*.sql"))
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding(1251)))
                    {
                        String line = sr.ReadToEnd();
                        sqlCommandsListForGia.Add(Path.GetFileNameWithoutExtension(file), line);
                        ToolStripMenuItem anotherOne = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file));
                        anotherOne.Click += new EventHandler(execSqlCommandForGiaEvent);
                        getToolStripMenuItem("ГИА").DropDownItems.Add(anotherOne);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Исключение говорит: " + ex.Message);
            } 
        }

        public void executeCombinedQuery(CheckedListBox listQuerysCheckBox, bool bd)
        {
            
            try
            {
                DataTable AllInOne = new DataTable();
               
                if (bd)
                {
                    for (int i = 0; i < listQuerysCheckBox.Items.Count - 1; i++)
                    {
                        if (listQuerysCheckBox.GetItemChecked(i))
                        {
                            dataGridView1.DataSource = bindingSource1;
                            dataAdapter = new SqlDataAdapter("use erbd_ege_reg_18_38 " + sqlCommandsListForEge[(string)listQuerysCheckBox.Items[i]], connectionString);
                            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                            DataTable table = new DataTable();
                            dataAdapter.Fill(table);
                            AllInOne.Merge(table);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listQuerysCheckBox.Items.Count - 1; i++)
                    {
                        if (listQuerysCheckBox.GetItemChecked(i))
                        {
                            dataGridView1.DataSource = bindingSource1;
                            dataAdapter = new SqlDataAdapter("use erbd_gia_reg_18_38 " + sqlCommandsListForGia[(string)listQuerysCheckBox.Items[i]], connectionString);
                            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                            DataTable table = new DataTable();
                            dataAdapter.Fill(table);
                            AllInOne.Merge(table,true);
                            bindingSource1.DataSource = new BindingSource(AllInOne, null);
                        }
                    }
                }
                //MessageBox.Show(Message(AllInOne));
                if (dataGridView1.DataSource != null)
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();
                }
                bindingSource1.DataSource = new BindingSource(AllInOne,null);
            } catch (SqlException e)
            {
                MessageBox.Show("Запрос косячный: " + e);
            }
        }


        /*
         * запрос надо оптимизировать и убрать фигню с "use erbd_ege_reg_18_38" + "
         * */
        private void execSqlCommandForGiaEvent(object sender, EventArgs args)
        {
            ToolStripMenuItem temp = (ToolStripMenuItem) sender;
            loadingDataInDataGridView("use erbd_gia_reg_18_38 " + sqlCommandsListForGia[temp.Text], true);
        }

        private void execSqlCommandForEgeEvent(object sender, EventArgs args)
        {
            ToolStripMenuItem temp = (ToolStripMenuItem) sender;
            loadingDataInDataGridView("use erbd_ege_reg_18_38 " + sqlCommandsListForEge[temp.Text], true);
        }
    }   
}
