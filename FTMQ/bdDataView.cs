using System;
using System.Collections.Generic;
using SD = System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
//using Microsoft.Office.Interop.Excel;

namespace FTMQ
{
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
        private string queryForParticipants = @"select
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

        Dictionary<String, String> listOfOgeCompleteQuerys = new Dictionary<String, String>();
        Dictionary<String, String> listOfEgeCompleteQuerys = new Dictionary<String, String>();

        List<Category> listOfOgeCombineQuerys = new List<Category>();
        List<Category> listOfEgeCombineQuerys = new List<Category>();

        public SD.DataTable table;
        public List<Action> activeCategory;
        public string nameActiveCategory;

        [STAThread]
        static void Main()
        {
            Application.Run(new bdDataView());
        }

        public bdDataView() {
            InitializeComponent();
            setCompleteQuerys();
            setCombineQuerys();
            showCurrentQuerys();
        }


    #region MainMethods 
        /*сложная реализацяи*/
        private void dataGridView_Sorted(object sender, EventArgs e)
        {
            if(activeCategory!= null && activeCategory.Count!=0)
                foreach (Action act in activeCategory)
                    act.Invoke();
        }
        
        //обработка окна редактирования запроса
        private void editingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editQueryForm == null)
                editQueryForm = new editQueryForm(this);
            editQueryForm.showingTheQuery();
        }        

        private void combinedQueryMenu_Click(object sender, EventArgs e)
        {
            if (combinedQueryForm == null)
                combinedQueryForm = new combinedQueryForm(this);
            this.Enabled = false;
            Category cat;
            if (whichBase)
            {
                cat = listOfEgeCombineQuerys.Find(x => x.Name.Equals((sender as ToolStripMenuItem).Text));
            }
            else
            {
                cat = listOfOgeCombineQuerys.Find(x => x.Name.Equals((sender as ToolStripMenuItem).Text));
            }
            combinedQueryForm.addingParameters(cat);
            nameActiveCategory = cat.Name;
        }

        /*
        * Функция для получения файлов запросов и сохранение их в списки для выполнения через выпадающее меню
        */
        private void setCompleteQuerys()
        {
            try
            {
                foreach (string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "SqlCommands\\Ege\\", "*.sql"))
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding(1251)))
                    {
                        String line = sr.ReadToEnd();
                        listOfEgeCompleteQuerys.Add(Path.GetFileNameWithoutExtension(file), line);                     
                    }
                }
                foreach (string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "SqlCommands\\Gia\\", "*.sql"))
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding(1251)))
                    {
                        String line = sr.ReadToEnd();
                        listOfEgeCompleteQuerys.Add(Path.GetFileNameWithoutExtension(file), line);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Исключение говорит: " + ex.Message);
            } 
        }

        private void setCombineQuerys()
        {
            Dictionary<string, Action> temp = new Dictionary<string, Action>
            {
                { "Проверка на категорию", checkCategoryClass },
                { "Проверка на тип документа", checkPassport },
                { "Проверка на активный результат", checkActiveResult },
                { "Проверка на УКП", checkParticipantsUKP },
                { "Проверка на ОВЗ", checkParticipantsOVZ }
            };
            Category Participants = new Category("Участники", true, temp, queryForParticipants);
            listOfEgeCombineQuerys.Add(Participants);

            temp = new Dictionary<string, Action>
            {
                { "Проверка на телефон", checkMobilePhone },
                { "Проверка на почту", checkEmail },
                { "Проверка на код работника", checkWorkerCode },
                { "Проверка на код станционарный телефон", checkStaticPhone },
                { "Проверка на специализацию", checkParticipantSpecialization },
                { "Проверка на прикрепление к ппэ", checkParticipantPP },
                { "Проверка на категорию", checkParticipantCategory }
            };
            Category Workers = new Category("Работники", false, temp, "");/*Заменить запрос*/
            listOfOgeCombineQuerys.Add(Workers);
        }
        /*
         * обновление списка готовых и параметризированных запросов срабатывает при смене бд
         */
        private void showCurrentQuerys()
        {

            /************Параметризированные запросы*******************/
            /*что быстрее:
            Создать временный dict присвоить ему новый и пересчитать одним циколом или два dict пересчитать двумя foreach*/
            if (combinedQueryMenu.DropDownItems.Count > 0)
                combinedQueryMenu.DropDownItems.Clear();
            if (whichBase)
                foreach (Category cat in listOfEgeCombineQuerys) 
                    this.combinedQueryMenu.DropDownItems.AddRange(new ToolStripItem[] {
                        new ToolStripMenuItem(cat.Name, null, new EventHandler(combinedQueryMenu_Click)) });
            else
                foreach (Category cat in listOfOgeCombineQuerys)
                    this.combinedQueryMenu.DropDownItems.AddRange(new ToolStripItem[] {
                        new ToolStripMenuItem(cat.Name, null, new EventHandler(combinedQueryMenu_Click)) });
            
            /* Файловые запросы*/
            completeQueryMenu.DropDownItems.Clear();
            foreach (KeyValuePair<string, string> entry in whichBase ? listOfEgeCompleteQuerys : listOfEgeCompleteQuerys)
            {
                ToolStripMenuItem anotherOne = new ToolStripMenuItem(entry.Key);
                anotherOne.Click += new EventHandler(this.execSqlCommandEvent);
                completeQueryMenu.DropDownItems.Add(anotherOne);
            }
        }
         /*
         * выполняет выбранный запрос
         * первый метод для ОГЭ, второй для ЕГЭ
         * */
        private void execSqlCommandEvent(object sender, EventArgs args)
        {
            activeCategory = null;
            if (whichBase)
                loadingDataInDataGridView(listOfEgeCompleteQuerys[((ToolStripMenuItem)sender).Text]);
            else
                loadingDataInDataGridView(listOfEgeCompleteQuerys[((ToolStripMenuItem)sender).Text]);
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

        /*-------------------------------------------------*/
        /*экспорт в Excel текущей таблицы
         * ------другой класс------
         */
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
                    for (int i = 1; i < table.Columns.Count + 1; i++)
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
        /*          
        * подгружаем данные из бд
        * написать обработчик обрыва соединения------------------------------------------------------------ 
        */
        public void loadingDataInDataGridView(string queryText)
        {
            memorizedQuery = queryText;
            try
            {
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
        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e) => previewBox.Text = "" + dataGridView[e.ColumnIndex, e.RowIndex].Value;
        #endregion

        #region Parameters
        /*
         выполнение выбранных проверок
         сделано на основе имени параметра, что не есть хорошо и не делайте это дома
         */
        public void additionalParametrChecking(List<Action> list)
        {
            loadingDataInDataGridView(listOfEgeCombineQuerys.Find(x => x.Name.Equals(nameActiveCategory)).Query);
            table.Columns.Add("Ошибки"); //добавляем столбец с описанием ошибок
            activeCategory = list;
            foreach (Action act in list)
                act.Invoke();
            clearingUnrelevantData();
        }

        private void checkParticipantCategory()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                switch (row.Cells["Тип экзамена"].Value)
                {
                   /* case 1:
                        if (row.Cells["Спец рассадка"].Value.Equals(1) && !row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7))
                        {
                            Console.WriteLine("checkingOVZPart: ЕГЭ. Параметр ОВЗ не указан");
                            if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || 
                                !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Категория;\w*", RegexOptions.IgnoreCase))
                                row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                            row.Cells["Спец рассадка"].Style.BackColor = Color.Yellow;
                        }*/
                }
            }
        }

        private void checkParticipantPP()
        {
            throw new NotImplementedException();
        }

        private void checkParticipantSpecialization()
        {
            throw new NotImplementedException();
        }

        private void checkStaticPhone()
        {
            throw new NotImplementedException();
        }

        private void checkWorkerCode()
        {
            throw new NotImplementedException();
        }

        private void checkEmail()
        {
            throw new NotImplementedException();
        }

        private void checkMobilePhone()
        {
            throw new NotImplementedException();
        }

        private void clearingUnrelevantData()
        {
            // return;
            Console.WriteLine("Записей на входе: " + dataGridView.Rows.Count);
            try
            {
                Guid temp = new Guid();
                foreach (SD.DataRow row in table.Rows)
                {
                    if (row["Ошибки"].GetType() == typeof(DBNull))
                    {
                        row.Delete();
                    }
                    else if (row["Айди участника"].Equals(temp))
                    {
                        row.Delete();
                    }
                    else
                        temp = (Guid)row["Айди участника"];
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Какого-то поля нет в таблице: + " + e);
            }
            Console.WriteLine("Записей на выходе: " + dataGridView.Rows.Count);
        }

        private void checkParticipantsOVZ()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                switch (row.Cells["Тип экзамена"].Value)
                {
                    case 1:
                        if (row.Cells["Спец рассадка"].Value.Equals(1) && !row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7))
                        {
                            Console.WriteLine("checkingOVZPart: ЕГЭ. Параметр ОВЗ не указан");
                            if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Категория;\w*", RegexOptions.IgnoreCase))
                                row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                            row.Cells["Спец рассадка"].Style.BackColor = Color.Yellow;
                        }
                        else if (row.Cells["Спец рассадка"].Value.Equals(0) && row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7))
                        {
                            Console.WriteLine("checkingOVZPart: ЕГЭ. Спец рассадка неверна");
                            if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Категория;\w*", RegexOptions.IgnoreCase))
                                row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                            row.Cells["Спец рассадка"].Style.BackColor = Color.Yellow;
                        }
                        break;
                    case 2:
                        if (!row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7) && !row.Cells["Параметр УКП"].Value.Equals(6))
                        {
                            Console.WriteLine("checkingOVZPart: ГВЭ");
                            if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Категория;\w*", RegexOptions.IgnoreCase))
                                row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                        }
                        break;
                    case 3:
                        if (!row.Cells["Параметр ОВЗ/УКП"].Value.Equals(7) && !row.Cells["Параметр УКП"].Value.Equals(6))
                        {
                            if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Категория;\w*", RegexOptions.IgnoreCase))
                                row.Cells["Ошибки"].Value += "Категория; ";
                            row.Cells["Тип экзамена"].Style.BackColor = Color.Yellow;
                        }
                        break;
                }
            }
        }

        private void checkParticipantsUKP()
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
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*УКП;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "УКП; ";
                    row.Cells["Параметр УКП"].Style.BackColor = Color.Cyan;
                    row.Cells["Наименование школы"].Style.BackColor = Color.Cyan;
                }
            }
        }

        private void checkActiveResult()
        {
            try
            {
                Console.WriteLine("На активный результат");

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells["Код предмета"].Equals(1) || row.Cells["Код предмета"].Equals(2) || row.Cells["Код предмета"].Equals(22))
                        switch (row.Cells["Допуск"].Value)
                        {
                            case 1: //допуск на русский
                                if (!row.Cells["Код предмета"].Value.Equals(2) || !row.Cells["Код предмета"].Value.Equals(22)) //не записан на мат
                                {
                                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Экзамен;\w*", RegexOptions.IgnoreCase))
                                        row.Cells["Ошибки"].Value += "Экзамен; ";
                                    row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                    row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
                                }
                                break;
                            case 2: //допуск на мат
                                if (!row.Cells["Код предмета"].Value.Equals(1)) //не записан на рус
                                {
                                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Экзамен;\w*", RegexOptions.IgnoreCase))
                                        row.Cells["Ошибки"].Value += "Экзамен; ";
                                    row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                    row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
                                }
                                break;
                            case 3:
                                if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Экзамен;\w*", RegexOptions.IgnoreCase))
                                    row.Cells["Ошибки"].Value += "Экзамен; ";
                                row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
                                break;
                            case 0:
                                if (!(row.Cells["Код предмета"].Value.Equals(2) || row.Cells["Код предмета"].Value.Equals(22)) && !row.Cells["Код предмета"].Value.Equals(1))
                                {
                                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Экзамен;\w*", RegexOptions.IgnoreCase))
                                        row.Cells["Ошибки"].Value += "Экзамен; ";
                                    row.Cells["Допуск"].Style.BackColor = Color.Yellow;
                                    row.Cells["Код предмета"].Style.BackColor = Color.Yellow;
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

        private void checkPassport()
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    switch (row.Cells["Тип документа"].Value)
                    {
                        case 1:
                            if (!Regex.IsMatch((string)row.Cells["Серия паспорта"].Value, @"^\d{4}$", RegexOptions.IgnoreCase))
                            {
                                if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Паспорт;\w*", RegexOptions.IgnoreCase))
                                    row.Cells["Ошибки"].Value += "Паспорт; ";
                                row.Cells["Тип документа"].Style.BackColor = Color.Red;
                                row.Cells["Серия паспорта"].Style.BackColor = Color.Red;
                            }
                            break;
                        default:
                            if (Regex.IsMatch((string)row.Cells["Серия паспорта"].Value, @"^\d{4}$", RegexOptions.IgnoreCase))
                            {
                                if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Паспорт;\w*", RegexOptions.IgnoreCase))
                                    row.Cells["Ошибки"].Value += "Паспорт; ";
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

        private void checkCategoryClass()
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
                                if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Класс;\w*", RegexOptions.IgnoreCase))
                                    row.Cells["Ошибки"].Value += "Класс; ";
                                row.Cells["Класс"].Style.BackColor = Color.Cyan;
                                row.Cells["Код категории"].Style.BackColor = Color.Cyan;
                            }
                            break;
                        case 4:
                            if (!row.Cells["Класс"].Value.Equals("0"))
                            {
                                if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Класс;\w*", RegexOptions.IgnoreCase))
                                    row.Cells["Ошибки"].Value += "Класс; ";
                                row.Cells["Класс"].Style.BackColor = Color.Cyan;
                                row.Cells["Код категории"].Style.BackColor = Color.Cyan;
                            }
                            break;
                        case 3:
                            if (!row.Cells["Класс"].Value.Equals("0"))
                            {
                                if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Класс;\w*", RegexOptions.IgnoreCase))
                                    row.Cells["Ошибки"].Value += "Класс; ";
                                row.Cells["Класс"].Style.BackColor = Color.Cyan;
                                row.Cells["Код категории"].Style.BackColor = Color.Cyan;
                            }
                            break;
                        case 8:
                            if (Regex.IsMatch((string)row.Cells["Класс"].Value, @"^0\S{isCyrillic}*", RegexOptions.IgnoreCase))
                            {
                                if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Класс;\w*", RegexOptions.IgnoreCase))
                                    row.Cells["Ошибки"].Value += "Класс; ";
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
        #endregion

    #region Getters and Setters
        //возвращение префикса для выбранной базы 
        public string getPrefixFofBd() => whichBase ? prefixForEge : prefixForOge;
        public string MemorizedQuery { get => memorizedQuery; set => memorizedQuery = value; }
        public List<Category> ListOfEgeCombineQuerys { get => listOfEgeCombineQuerys; set => listOfEgeCombineQuerys = value; }
        public MenuStrip getMenuStrip() => menuStrip1;
        
    #endregion
    }
}

public class Category
{
    private string name;
    private bool bD;
    private string query;
    private Dictionary<string, Action> methods;

    public string Name { get => name; set => name = value; }
    public bool BD { get => bD; set => bD = value; }
    public string Query { get => query; set => query = value; }
    public Dictionary<string, Action> Methods { get => methods; set => methods = value; }

    public Category(string name, bool bd, Dictionary<string,Action> methods, string query)
    {
        this.Name = name;
        this.BD = bd;
        this.Query = query;
        this.Methods = methods;    
    }

    public Action getAction(string actionName)
    {
        foreach (KeyValuePair<string, Action> a in this.methods)
            if (a.Key == actionName) return a.Value;
        return null;
    }
}
