using System;
using System.Collections.Generic;
using System.ComponentModel;
using SD = System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
//using Microsoft.Office.Interop.Excel;

namespace FTMQ
{
    //Controller
    public partial class bdDataView
    {
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();
        private BindingSource bindingSource1 = new BindingSource();
        
        private editQueryForm editQueryForm;

        private combinedQueryForm combinedQueryForm;
        private string connectionString = @"Data Source=172.16.22.1;
                                            Integrated Security=SSPI;
                                            Trusted_Connection=yes;
                                            Connection Timeout = 60";
        private string mainSqlQuery = @"select
	                    distinct p.ParticipantID as [Айди участника]
	                    ,p.Surname + ' ' + p.Name + ' ' + p.SecondName as [ФИО]
	                    ,p.ParticipantCategoryFK as [Код категории]
	                    ,p.LimitPotencial as [Спец рассадка]
	                    ,p.pClass as [Класс]
	                    ,p.GiaAccept as [Допуск]
	                    ,p.DocumentSeries as [Серия паспорта]
	                    ,p.DocumentTypeCode as [Тип документа]
	                    ,s.ShortName as [Наименование школы]
	                    ,p.Gia as [Тип экзамена]
	                    ,pe.ParticipantID as [Айди участника на экзамене]
	                    ,e.SubjectCode as [Код предмета]                        
                        ,pp.Property as [Параметр ОВЗ/УКП]
                        ,pp2.Property as [Параметр УКП]
                    from rbd_Participants as p
	                    left join rbd_ParticipantProperties as pp on pp.ParticipantID = p.ParticipantID and pp.Property = 7
                        left join rbd_ParticipantProperties as pp2 on pp2.ParticipantID = p.ParticipantID and pp2.Property = 6 
	                    left join rbd_Schools as s on s.SchoolID = p.SchoolRegistration
	                    left join rbd_ParticipantsExams as pe on pe.ParticipantID = p.ParticipantID
	                    left join dat_Exams as e on e.ExamGlobalID = pe.ExamGlobalID
                    where p.DeleteType = 0 
                    order by 2";

        private string memorizedQuery = @""; //запрос для тех, кто хочет его посмотреть

        private static string prefixForEge = "use erbd_ege_reg_18_38 ";
        private static string prefixForOge = "use erbd_gia_reg_18_38 ";
        private List<String> listOfMethodsWithParaametrs;

        public bool whichBase = true; // true при егэ, false при огэ
                                      //private string queryText = "";

        Dictionary<String, String> sqlCommandsListForOge = new Dictionary<String, String>();
        Dictionary<String, String> sqlCommandsListForEge = new Dictionary<String, String>();

        public SD.DataTable table;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new bdDataView());
        }

        public bdDataView() {
            InitializeComponent();
            gettingSqlCommands();
            showCurrentQuerys();
        }

        private void BdDataView_Load(object sender, EventArgs arg) { }

        //возвращение префикса для выбранной базы 
        public string getPrefixFofBd()
        {
           if(whichBase) return prefixForEge;
           else return prefixForOge;
        }

        public Dictionary<string, string> SqlCommandsListForGia { get => sqlCommandsListForOge; set => sqlCommandsListForOge = value; }
        public Dictionary<string, string> SqlCommandsListForEge { get => sqlCommandsListForEge; set => sqlCommandsListForEge = value; }

        //вовзращает активный запрос // на активную бд на основе флажка whichBase
        public string getMemorizedQuery()
        {
            if (whichBase)
                return memorizedQuery;
            else
                return memorizedQuery;
        }

        /*          
         *подгружаем данные из бд
         * написать обработчик обрыва соединения------------------------------------------------------------ 
         */
        public void loadingDataInDataGridView(string queryText)
        {
            memorizedQuery = queryText;
            try
            {
                //if (dataGridView1.DataSource != null)
                //{
                //    dataGridView1.DataSource = null;
                //    dataGridView1.Columns.Clear();
                //    dataGridView1.Rows.Clear();
                //    dataGridView1.Refresh();
                //}
                //if (table != null)
                //    table = null;
                dataGridView.DataSource = bindingSource1;
                dataAdapter = new SqlDataAdapter(getPrefixFofBd() + queryText, connectionString);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                table = new SD.DataTable();
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка \\/( * * )\\/: " + ex.Message);
            }
        }

        /*
         выполнение выбранных проверок
         сделано на основе имени параметра, что не есть хорошо и не делайте это дом
         */
        public void additionalParametrChecking(List<string> list)
        {
            listOfMethodsWithParaametrs = list;
            loadingDataInDataGridView(mainSqlQuery);
            table.Columns.Add("Ошибки"); //добавляем столбец с описанием ошибок
            Console.WriteLine("Колво записей: " + dataGridView.Rows.Count);
            foreach (String item in listOfMethodsWithParaametrs) {
                switch (item)
                {
                    case "Класс/Категория":
                        Console.WriteLine("Класс/Категория");                        
                        checkingCategoryClass();                        
                        break;
                    case "Серия/Тип доку":
                        Console.WriteLine("Серия/Тип доку");
                        checkingPassport();                        
                        break;
                    case "Дейст.рез/Зарег на жизнь":
                        Console.WriteLine("Дейст. рез/Зарег на жизнь");
                        checkingActiveResult();
                        break;
                    case "Наим/Параметр":
                        Console.WriteLine("Наим/Параметр");
                        checkingUKPPart();                        
                        break;
                    case "Тип экз/Параметр":
                        Console.WriteLine("Тип экз/Параметр");
                        checkingOVZPart();                        
                        break;
                }
            }
            clearingUnrelevantData();
        }

        private void clearingUnrelevantData()
        {
           // return;
            Console.WriteLine("Записей на входе: " + dataGridView.Rows.Count);
            try {                
                Guid temp = new Guid();
                foreach (SD.DataRow row in table.Rows)
                {
                    if (row["Ошибки"].GetType() == typeof(DBNull))
                    {
                        row.Delete();
                    }
                    else if(row["Айди участника"].Equals(temp))
                    {
                        row.Delete();
                    }
                    else
                        temp = (Guid)row["Айди участника"];
                }
            } catch(Exception e){
                MessageBox.Show("Какого-то поля нет в таблице: + " + e);
            }
            Console.WriteLine("Записей на выходе: " + dataGridView.Rows.Count);
        }

        private void checkingOVZPart()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                switch (row.Cells["Тип экзамена"].Value)
                {
                    case 1:
                        if (row.Cells["Спец рассадка"].Value.Equals(1) && !row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7))
                        {
                            Console.WriteLine("checkingOVZPart: ЕГЭ. Параметр ОВЗ не указан");
                            row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                            row.Cells["Спец рассадка"].Style.BackColor = Color.Yellow;
                        } else if (row.Cells["Спец рассадка"].Value.Equals(0) && row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7))
                        {
                            Console.WriteLine("checkingOVZPart: ЕГЭ. Спец рассадка неверна");
                            row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                            row.Cells["Спец рассадка"].Style.BackColor = Color.Yellow;
                        }
                        break;
                    case 2:
                        if (!row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7) && !row.Cells["Параметр УКП"].Value.Equals(6))
                        {
                            Console.WriteLine("checkingOVZPart: ГВЭ");
                            row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                        }
                        break;
                    case 3:
                        if (!row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7) && !row.Cells["Параметр УКП"].Value.Equals(6))
                        {
                            Console.WriteLine("checkingOVZPart: ЕГЭ/ГВЭ");
                            row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                        }
                        break;
                }
            }
        }

        private void checkingUKPPart()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                
                //Console.WriteLine("Параметр УКП: " + row.Cells["Параметр УКП"].Value + "; Школа: " + row.Cells["Наименование школы"].Value + ";");
                if ((Regex.IsMatch((string)row.Cells["Наименование школы"].Value, @"\sУКП\s") ||
                    Regex.IsMatch((string)row.Cells["Наименование школы"].Value, @"\sГУФСИН\s") ||
                    Regex.IsMatch((string)row.Cells["Наименование школы"].Value, @"\sУФСИН\s") ||
                    Regex.IsMatch((string)row.Cells["Наименование школы"].Value, @"\sСИЗО\s") ||
                    (row.Cells["Наименование школы"].Value.Equals("МОУ Бозойская В(С)ОШ")) ||
                    (row.Cells["Наименование школы"].Value.Equals("МОУ ИРМО В(С)ОШ"))
                    ) 
                    && !row.Cells["Параметр УКП"].Value.Equals(6))
                {
                    Console.WriteLine("Тип: " + row.Cells["Наименование школы"].Value);
                    row.Cells["Ошибки"].Value += "УКП; ";
                    row.Cells["Параметр УКП"].Style.BackColor = Color.Cyan;
                    row.Cells["Наименование школы"].Style.BackColor = Color.Cyan;
                }
            }
        }

        private void checkingActiveResult()
        {
            try
            {
                Console.WriteLine("На активный результат");

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if(row.Cells["Код предмета"].Equals(1) || row.Cells["Код предмета"].Equals(2) || row.Cells["Код предмета"].Equals(22))
                        switch (row.Cells["Допуск"].Value)
                        {
                            case 1: //допуск на русский
                                if (!row.Cells["Код предмета"].Value.Equals(2) || !row.Cells["Код предмета"].Value.Equals(22)) //не записан на мат
                                {
                                    row.Cells["Ошибки"].Value += "Экзамен; ";
                                    row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                    row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
                                }
                                break;
                            case 2: //допуск на мат
                                if (!row.Cells["Код предмета"].Value.Equals(1)) //не записан на рус
                                {
                                    row.Cells["Ошибки"].Value += "Экзамен; ";
                                    row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                    row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
                                }
                                break;
                            case 3:
                                row.Cells["Ошибки"].Value += "Экзамен; ";
                                row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
                                break;
                            case 0:
                                if (!(row.Cells["Код предмета"].Value.Equals(2) || row.Cells["Код предмета"].Value.Equals(22)) && !row.Cells["Код предмета"].Value.Equals(1))
                                {
                                    row.Cells["Ошибки"].Value += "Экзамен; ";
                                    row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                    row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
                                }
                                break;
                        }
                }
            } catch(Exception e){
                MessageBox.Show("Какого-то поля нет в таблице: + " + e);
            }
        }

        private void checkingPassport()
        {
            try { 
                foreach(DataGridViewRow row in dataGridView.Rows)
                {
                    switch (row.Cells["Тип документа"].Value)
                    {
                        case 1:
                            if (!Regex.IsMatch((string)row.Cells["Серия паспорта"].Value, @"^\d{4}$", RegexOptions.IgnoreCase))
                            {
                                row.Cells["Ошибки"].Value += "Паспорт";
                                row.Cells["Тип документа"].Style.BackColor = Color.Red;
                                row.Cells["Серия паспорта"].Style.BackColor = Color.Red;
                            }
                            break;
                        default:
                            if (Regex.IsMatch((string)row.Cells["Серия паспорта"].Value, @"^\d{4}$", RegexOptions.IgnoreCase))
                            {
                                row.Cells["Ошибки"].Value += "Паспорт";
                                row.Cells["Тип документа"].Style.BackColor = Color.Red;
                                row.Cells["Серия паспорта"].Style.BackColor = Color.Red;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Какого-то поля нет в таблице: + " + e);
            }
        }

        public void checkingCategoryClass()
        {            
            try
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    switch (row.Cells["Код категории"].Value)
                    {
                        case 1:
                            if (Regex.IsMatch((string)row.Cells["Класс"].Value, @"^0\S*", RegexOptions.IgnoreCase))
                            {
                                row.Cells["Ошибки"].Value += "Класс Не правильно заполнение поле \"Класс\"";
                                row.Cells["Класс"].Style.BackColor = Color.Cyan;
                                row.Cells["Код категории"].Style.BackColor = Color.Cyan;

                            }
                            break;
                        case 4:
                            if (!row.Cells["Класс"].Value.Equals("0"))
                            {
                                row.Cells["Ошибки"].Value += "Класс Не правильно заполнение поле \"Класс\"";
                                row.Cells["Класс"].Style.BackColor = Color.Cyan;
                                row.Cells["Код категории"].Style.BackColor = Color.Cyan;
                            }

                            break;
                        case 3:
                            if (!row.Cells["Класс"].Value.Equals("0"))
                            {
                                row.Cells["Ошибки"].Value += "Класс Не правильно заполнение поле \"Класс\"";
                                row.Cells["Класс"].Style.BackColor = Color.Cyan;
                                row.Cells["Код категории"].Style.BackColor = Color.Cyan;
                            }
                            break;
                        case 8:
                            if (Regex.IsMatch((string)row.Cells["Класс"].Value, @"^0\S{isCyrillic}*", RegexOptions.IgnoreCase))
                            {
                                row.Cells["Ошибки"].Value += "Класс Не правильно заполнение поле \"Класс\"";
                                row.Cells["Класс"].Style.BackColor = Color.Cyan;
                                row.Cells["Код категории"].Style.BackColor = Color.Cyan;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка: " + e);
            }            
        }

        private void BdDataView_Closing(object sender, CancelEventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView_Sorted(object sender, EventArgs e)
        {
            foreach (String item in listOfMethodsWithParaametrs)
            {
                switch (item)
                {
                    case "Класс/Категория":
                        checkingCategoryClass();
                        Console.WriteLine("Класс/Категория");
                        break;
                    case "Серия/Тип доку":
                        checkingPassport();
                        Console.WriteLine("Серия/Тип доку");
                        break;
                    case "Дейст. рез/Зарег на жизнь":
                        checkingActiveResult();
                        Console.WriteLine("Дейст. рез/Зарег на жизнь");
                        break;
                    case "Наим/Параметр":
                        checkingUKPPart();
                        Console.WriteLine("Наим/Параметр");
                        break;
                    case "Тип экз/Параметр":
                        checkingOVZPart();
                        Console.WriteLine("Тип экз/Параметр");
                        break;
                }
            }
        }
        
        //обработка окна редактирования запроса
        private void editingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editQueryForm == null)
                editQueryForm = new editQueryForm(this);
            editQueryForm.showingTheQuery();
        }

        public MenuStrip getMenuStrip() => menuStrip1;

        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e) => previewBox.Text = "" + dataGridView[e.ColumnIndex, e.RowIndex].Value;

        private void combinedQueryMenu_Click(object sender, EventArgs e)
        {
            if (combinedQueryForm == null)
                combinedQueryForm = new combinedQueryForm(this);
            combinedQueryForm.choiceForm();
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
                    }
                }
                foreach (string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "SqlCommands\\Gia\\", "*.sql"))
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding(1251)))
                    {
                        String line = sr.ReadToEnd();
                        sqlCommandsListForOge.Add(Path.GetFileNameWithoutExtension(file), line);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Исключение говорит: " + ex.Message);
            } 
        }

        /*
         * обновление списка готовых запросов срабатывает при смене бд
         */
        private void showCurrentQuerys()
        {
            //что быстрее. 
            //Создать временный dict присвоить ему новый и пересчитать одним циколом или два dict пересчитать двумя foreach
            Dictionary<string, string> temp;

            if (whichBase)
                temp = sqlCommandsListForEge;
            else
                temp = sqlCommandsListForOge;
            completeQueryMenu.DropDownItems.Clear();
            foreach (KeyValuePair<string, string> entry in temp)
            {
                ToolStripMenuItem anotherOne = new ToolStripMenuItem(entry.Key);
                anotherOne.Click += new EventHandler(this.execSqlCommandEvent);
                completeQueryMenu.DropDownItems.Add(anotherOne);
            }
        }

        /* Неработающая хрень, которая хрень кому и зачем сдалась и вообще я бы её удалил,
         * если бы не было жалко время, которое я на неё потратил
         */
        public void executeCombinedQuery()
        {

            //try
            //{
            //    SD.DataTable AllInOne = new SD.DataTable();
               
            //    if (bd)
            //    {
            //        for (int i = 0; i < listQuerysCheckBox.Items.Count - 1; i++)
            //        {
            //            if (listQuerysCheckBox.GetItemChecked(i))
            //            {
            //                dataGridView1.DataSource = bindingSource1;
            //                dataAdapter = new SqlDataAdapter("use erbd_ege_reg_18_38 " + sqlCommandsListForEge[(string)listQuerysCheckBox.Items[i]], connectionString);
            //                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            //                SD.DataTable table = new SD.DataTable();
            //                dataAdapter.Fill(table);
            //                AllInOne.Merge(table);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < listQuerysCheckBox.Items.Count - 1; i++)
            //        {
            //            if (listQuerysCheckBox.GetItemChecked(i))
            //            {
            //                dataGridView1.DataSource = bindingSource1;
            //                dataAdapter = new SqlDataAdapter("use erbd_gia_reg_18_38 " + sqlCommandsListForOge[(string)listQuerysCheckBox.Items[i]], connectionString);
            //                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            //                SD.DataTable table = new SD.DataTable();
            //                dataAdapter.Fill(table);
            //                AllInOne.Merge(table,true);
            //                bindingSource1.DataSource = new BindingSource(AllInOne, null);
            //            }
            //        }
            //    }
            //    //MessageBox.Show(Message(AllInOne));
            //    if (dataGridView1.DataSource != null)
            //    {
            //        dataGridView1.DataSource = null;
            //        dataGridView1.Columns.Clear();
            //        dataGridView1.Rows.Clear();
            //        dataGridView1.Refresh();
            //    }
            //    bindingSource1.DataSource = new BindingSource(AllInOne,null);
            //} catch (SqlException e)
            //{
            //    MessageBox.Show("Запрос косячный: " + e);
            //}
        }

        /*
         * запрос надо оптимизировать и убрать фигню с "use erbd_ege_reg_18_38" + "
         * выполняет выбранный запрос
         *  первый метод для ОГЭ, второй для ЕГЭ
         * */
        private void execSqlCommandEvent(object sender, EventArgs args)
        {
            ToolStripMenuItem temp = (ToolStripMenuItem)sender;
            if (whichBase)
                loadingDataInDataGridView(sqlCommandsListForEge[temp.Text]);
            else
                loadingDataInDataGridView(sqlCommandsListForOge[temp.Text]);
        }

        //экспорт в Excel текущей таблицы
        private void exportToExcelTSMI_Click(object sender, EventArgs e)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            
            excelApp.Workbooks.Add();
            Microsoft.Office.Interop.Excel._Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.ActiveSheet;
            if (table == null)
                MessageBox.Show("Кажется тебе нечего выгружать");
            else
            {
                int rowCount = 1; 
                foreach (SD.DataRow row in table.Rows)
                {
                    rowCount += 1;
                    for (int i = 1; i < table.Columns.Count+1; i++)
                    {
                        if (rowCount == 2)
                        {
                            ws.Cells[1, i] = table.Columns[i - 1].ColumnName;
                        }
                        ws.Cells[rowCount, i] = row[i - 1].ToString();
                    }

                }
                ws.Columns.AutoFit();
                ws.Rows.AutoFit();
                excelApp.Visible = true;
            }
        }

        //активируем базу ЕГЭ. Все запросы будут обращаться к ней
        private void egeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            whichBase = true;
            showCurrentQuerys();
        }
        //активируем базу ОГЭ. Все запросы будут обращаться к ней
        private void ogeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            whichBase = false;
            showCurrentQuerys();
        }
    }   
}
