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
        #region Querys
        private string queryForParticipants = @"select
	                    distinct p.ParticipantID as [Айди]
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
        private string queryForWorkers = @"SELECT
                        /*ОСНОВНЫЕ ПОЛЯ С ИНФОРМАЦИЕЙ *НАЧАЛО* */
                         w.StationWorkerID as [айди]
                        ,g.GovernmentCode AS MO
                        ,w.StationWorkerCode AS [Код работника]
                        ,w.Surname+' '+w.Name+' '+w.SecondName AS [ФИО] 
                        ,dt.DocumentTypeName AS [Тип документа]
                        ,w.DocumentSeries AS [Серия]
                        ,w.DocumentNumber AS [Номер]
                        ,w.Phones AS [Телефоны]
                        ,w.Mails AS [Почта]
                        ,et.EduTypeName AS [Образование]
                        ,w.SWorkerCategory AS [Квалификационная категория]
                        ,w.SchoolPosition AS [Должность в ОО]
                        ,sp.SWorkerPositionname AS [Должность в ППЭ]
                        ,swos.StationCode AS [Прикреплен к ППЭ]
                        /*ОСНОВНЫЕ ПОЛЯ С ИНФОРМАЦИЕЙ *КОНЕЦ* */
						 /*ПРЕДМЕТАНЯ СПЕЦИАЛИЗАЦИЯ *НАЧАЛО* */
                        ,(CASE WHEN sws.[Рус] = 1 THEN 'Русский язык; ' ELSE '' END) + (CASE WHEN sws.[Мат] = 1 THEN 'Математика; ' ELSE '' END) 
                        +(CASE WHEN sws.[Физ] = 1 THEN 'Физика; ' ELSE '' END) + (CASE WHEN sws.[Хим] = 1 THEN 'Химия; ' ELSE '' END) 
                        +(CASE WHEN sws.[ИКТ] = 1 THEN 'ИКТ; ' ELSE '' END) + (CASE WHEN sws.[Био] = 1 THEN 'Биология; ' ELSE '' END)
                        +(CASE WHEN sws.[Ист] = 1 THEN 'История; ' ELSE '' END) + (CASE WHEN sws.[Гео] = 1 THEN 'География; ' ELSE '' END)
                        +(CASE WHEN sws.[Анг] = 1 THEN 'Английский язык; ' ELSE '' END) + (CASE WHEN sws.[Нем] = 1 THEN 'Немецкий язык; ' ELSE '' END)
                        +(CASE WHEN sws.[Фра] = 1 THEN 'Французский язык; ' ELSE '' END) + (CASE WHEN sws.[Исп] = 1 THEN 'Испанский язык; ' ELSE '' END) 
                        +(CASE WHEN sws.[Общ] = 1 THEN 'Обществознание; ' ELSE '' END) + (CASE WHEN sws.[Лит] = 1 THEN 'Литература; ' ELSE '' END)
                        +(CASE WHEN sws.[РодЯ] = 1 THEN 'Родной язык; ' ELSE '' END) + (CASE WHEN sws.[РодЛ] = 1 THEN 'Родная литература; ' ELSE '' END)
                        +(CASE WHEN sws.[Нет] = 1 THEN 'Специализация отсутствует; ' ELSE '' END) AS [Предметная специализация]
                        /*ПРЕДМЕТАНЯ СПЕЦИАЛИЗАЦИЯ *КОНЕЦ* */
                        FROM rbd_StationWorkers AS w
                        LEFT JOIN 
                         rbdc_DocumentTypes AS dt ON dt.DocumentTypeCode=w.DocumentTypeCode
                        LEFT JOIN
                         rbdc_EducationTypes AS et ON et.EduTypeCode=w.EducationTypeID
                        LEFT JOIN
                         rbdc_SWorkerPositions AS sp ON sp.SWorkerPositionCode=w.WorkerPositionID
                        LEFT JOIN
                         rbd_Governments AS g ON g.GovernmentID=w.GovernmentID

                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ СПЕЦИАЛИЗАЦИИ *НАЧАЛО* */
                        LEFT JOIN
                         (SELECT 
                          rbd_StationWorkers.StationWorkerID AS [StationWorkerID]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 1 THEN 1 ELSE NULL END) AS [Рус]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 2 THEN 1 ELSE NULL END) AS [Мат]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 3 THEN 1 ELSE NULL END) AS [Физ]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 4 THEN 1 ELSE NULL END) AS [Хим]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 5 THEN 1 ELSE NULL END) AS [Икт]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 6 THEN 1 ELSE NULL END) AS [Био]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 7 THEN 1 ELSE NULL END) AS [Ист]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 8 THEN 1 ELSE NULL END) AS [Гео]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 9 THEN 1 ELSE NULL END) AS [Анг]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 10 THEN 1 ELSE NULL END) AS [Нем]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 11 THEN 1 ELSE NULL END) AS [Фра]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 12 THEN 1 ELSE NULL END) AS [Общ]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 13 THEN 1 ELSE NULL END) AS [Исп]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 18 THEN 1 ELSE NULL END) AS [Лит]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 71 THEN 1 ELSE NULL END) AS [РодЯ]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode = 72 THEN 1 ELSE NULL END) AS [РодЛ]
                          ,MAX(CASE WHEN rbd_StationWorkersSubjects.SubjectCode is null THEN 1 ELSE NULL END) AS [Нет]
                          FROM rbd_StationWorkers 
                          LEFT JOIN
                           rbd_StationWorkersSubjects ON rbd_StationWorkersSubjects.StationWorkerID = rbd_StationWorkers.StationWorkerID 
                          WHERE rbd_StationWorkers.DeleteType = 0
                          GROUP BY rbd_StationWorkers.StationWorkerID) AS sws ON sws.StationWorkerID=w.StationWorkerID 
                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ СПЕЦИАЛИЗАЦИИ *КОНЕЦ* */

                       

                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ ПРИКРЕПЛЕНИЯ К ППЭ *НАЧАЛО* */

                        LEFT JOIN
                         (SELECT
                          DISTINCT rbd_StationWorkerOnStation.StationWorkerId
                          ,MAX(CASE WHEN rbd_StationWorkerOnStation.StationId IS NOT NULL THEN rbd_Stations.StationCode ELSE NULL END) AS StationCode
                          FROM rbd_StationWorkerOnStation
                          INNER JOIN
                           rbd_Stations ON rbd_Stations.StationId = rbd_StationWorkerOnStation.StationId
                          GROUP BY rbd_StationWorkerOnStation.StationWorkerId
                         ) AS swos ON swos.StationWorkerId = w.StationWorkerId
 

                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ ПРИКРЕПЛЕНИЯ К ППЭ *КОНЕЦ* */

                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ ДУБЛЕЙ В КОДЕ РАБОТНИКА *НАЧАЛО* */
                        LEFT JOIN
                        (Select 
                         w.StationWorkerCode
                         ,COUNT(w.StationWorkerCode) as countID
                         FROM rbd_StationWorkers AS w
                         WHERE w.DeleteType = 0
                         GROUP BY w.StationWorkerCode) AS dubcod ON dubcod.StationWorkerCode = w.StationWorkerCode
                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ ДУБЛЕЙ В КОДЕ РАБОТНИКА *КОНЕЦ* */

                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ ДУБЛЕЙ СРЕДИ РАБОТНИКОВ ПО ДОКУМЕНТАМ *НАЧАЛО* */
                        LEFT JOIN
                        (SELECT
                         w.Surname+' '+w.Name+' '+w.SecondName AS FIO 
                         ,w.DocumentSeries 
                         ,w.DocumentNumber
                         ,count(w.DocumentSeries+' '+w.DocumentNumber) AS dubDoc
                         FROM rbd_StationWorkers AS w
                         WHERE
                         w.DeleteType=0 
                         GROUP BY 
                         w.Surname+' '+w.Name+' '+w.SecondName  
                         ,w.DocumentSeries
                         ,w.DocumentNumber) as dubDoc on dubDoc.DocumentSeries = w.DocumentSeries  and dubDoc.DocumentNumber = w.DocumentNumber
                        /*ПОДЗАПРОС ДЛЯ ВЫВЕРКИ ДУБЛЕЙ СРЕДИ РАБОТНИКОВ ПО ДОКУМЕНТАМ *КОНЕЦ* */

                        WHERE
                        w.DeleteType=0 
                        --and g.GovernmentCode not in (47,777)
                        ORDER BY 
                        g.GovernmentCode
                        ,w.StationWorkerCode";
#endregion
        private string memorizedQuery = @""; //запрос для тех, кто хочет его посмотреть

        private static string prefixForEge = "use erbd_ege_reg_18_38 ";
        private static string prefixForOge = "use erbd_gia_reg_18_38 ";
        public bool whichBase = true; // true при егэ, false при огэ
                                      //private string queryText = "";

        Dictionary<String, String> listOfOgeCompleteQuerys = new Dictionary<String, String>();
        Dictionary<String, String> listOfEgeCompleteQuerys = new Dictionary<String, String>();

        List<Category> listOfOgeCombineQuerys = new List<Category>();
        List<Category> listOfEgeCombineQuerys = new List<Category>();

        public SD.DataTable table;
        public List<Action> activeMethods; //список методов проверок, выбранный пользователем 
        public string nameActiveCategory;
        private Category activeCategory;

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
            switchBD();
        }

      

    #region MainMethods 
        /*сложная реализацяи*/
            private void dataGridView_Sorted(object sender, EventArgs e)
        {
            if(activeMethods != null && activeMethods.Count !=0 )
                foreach (Action act in activeMethods)
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
            if (whichBase)
            {
                activeCategory = listOfEgeCombineQuerys.Find(x => x.Name.Equals((sender as ToolStripMenuItem).Text));
            }
            else
            {
                activeCategory = listOfOgeCombineQuerys.Find(x => x.Name.Equals((sender as ToolStripMenuItem).Text));
            }
            combinedQueryForm.addingParameters(activeCategory);
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
                { "Проверка на специализацию", checkWorkersSpecialization },
                { "Проверка на наличие типа документа", checkTypeDocument },
                { "Проверка на наличие серии документа", checkSeriesDocument },
                { "Проверка на наличие номера документа", checkNumberDocument },
                { "Проверка на телефон", checkMobilePhone },
                { "Проверка на наличие образование", checkEducationField },
                { "Проверка на наличие категории", checkCategoryField },
                { "Проверка на наличие должности в ОО", checkSchoolPosition },
                { "Проверка на наличие должности в ППЭ", checkPPPosition },
                { "Проверка на прикрепление работника к ППЭ", checkPPAttachment },
                { "Проверка на код работника", checkWorkerCode },
                { "Проверка на почту", checkEmail },
            };
            Category Workers = new Category("Работники", false, temp, queryForWorkers);
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
         * выполняет запрос из файла 
         * первый метод для ОГЭ, второй для ЕГЭ
         * */
        private void execSqlCommandEvent(object sender, EventArgs args)
        {
            //activeCategory = null; /// ????
            if (whichBase)
                executeSqlQuery(listOfEgeCompleteQuerys[((ToolStripMenuItem)sender).Text]);
            else
                executeSqlQuery(listOfEgeCompleteQuerys[((ToolStripMenuItem)sender).Text]);
        }
        
        //активируем базу ЕГЭ. Все запросы будут обращаться к ней
        private void egeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            whichBase = true;            
            switchBD();
            showCurrentQuerys();
        }
        //активируем базу ОГЭ. Все запросы будут обращаться к ней
        private void ogeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            whichBase = false;            
            switchBD();
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
        public void executeSqlQuery(string queryText)
        {
            memorizedQuery = queryText;
            try
            {
                //dataGridView.DataSource = bindingSource1;
                dataAdapter = new SqlDataAdapter(getPrefixFofBd() + queryText, connectionString);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                table = new SD.DataTable();
                dataAdapter.Fill(table);
                dataGridView.DataSource = table;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка \\/( * * )\\/: " + ex.Message);
            }
        }
        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e) => previewBox.Text = "" + dataGridView[e.ColumnIndex, e.RowIndex].Value;
        #endregion

        private void switchBD()
        {

            if (whichBase)
            {
                egeToolStripMenuItem1.Enabled = false;
                ogeToolStripMenuItem1.Enabled = true;
                bdLabel.Text = "EGE";
            }
            else
            {
                ogeToolStripMenuItem1.Enabled = false;
                egeToolStripMenuItem1.Enabled = true;
                bdLabel.Text = "OGE";
            }
        }

        #region Parameters
        /*
         выполнение выбранных проверок
         сделано на основе имени параметра, что не есть хорошо и не делайте это дома
         */

        public void additionalParameterChecking(List<Action> list)
        {
            //MessageBox.Show("whichBase = " + whichBase);
            //if (whichBase)
            //    executeSqlQuery(listOfEgeCombineQuerys.Find(x => x.Name.Equals(nameActiveCategory)).Query);
            //else
            //    executeSqlQuery(listOfOgeCombineQuerys.Find(x => x.Name.Equals(nameActiveCategory)).Query);
            executeSqlQuery(activeCategory.Query);
            table.Columns.Add("Ошибки"); //добавляем столбец с описанием ошибок
            activeMethods = list; 
            foreach (Action act in list)
                act.Invoke();
            clearingUnrelevantData();
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
                    else if (row["Айди"].Equals(temp))
                    {
                        row.Delete();
                    }
                    else
                        temp = (Guid)row["Айди"];
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Какого-то поля нет в таблице: + " + e);
            }
            Console.WriteLine("Записей на выходе: " + dataGridView.Rows.Count);
        }

        #region OgeChecking
        private void checkNumberDocument()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Номер"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указан номер документа;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не указан номер документа; ";
                    row.Cells["Номер"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkEducationField()
        {
            // (CASE WHEN et.EduTypeName IS NULL THEN 'Не указано образование; ' ELSE '' END) --Проверка на наличие образование
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Образование"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указано образование;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не указано образование; ";
                    row.Cells["Образование"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkSeriesDocument()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Серия"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указана серия документа;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не указана серия документа; ";
                    row.Cells["Серия"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkTypeDocument()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Тип документа"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указан тип документа;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не указан тип документа; ";
                    row.Cells["Тип документа"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkCategoryField()
        {
            //(CASE WHEN w.SWorkerCategory IS NULL THEN 'Не указана квалификационная категория; ' ELSE '' END) --
            //CASE WHEN w.SWorkerCategory LIKE 'Учитель'  OR w.SWorkerCategory LIKE 'учитель'
            //--  THEN 'Категория "учитель"; ' ELSE '' END) --Проверка на наличие категории

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Квалификационная категория"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указана квалификационная категория;\w*", RegexOptions.Compiled))
                        row.Cells["Ошибки"].Value += "Не указана квалификационная категория; ";
                    row.Cells["Квалификационная категория"].Style.BackColor = Color.Aqua;
                } else if(Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"учитель", RegexOptions.IgnoreCase))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не конкретиизирована категория;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не конкретиизирована категория; ";
                    row.Cells["Квалификационная категория"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkSchoolPosition()
        {
            //--+(CASE WHEN w.SchoolPosition IS NULL THEN 'Не указана должность в ОО; ' ELSE '' END) --Проверка на наличие должности в ОО
            /*(CASE WHEN w.SchoolPosition LIKE 'Учитель'  OR w.SchoolPosition LIKE 'учитель' 
            --  THEN 'Должность "учитель"; ' ELSE '' END) --Проверка на наличие должности в ОО**/
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Должность в ОО"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указана должность в ОО;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не указана должность в ОО; ";
                    row.Cells["Должность в ОО"].Style.BackColor = Color.Aqua;
                } else if(Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"учитель", RegexOptions.IgnoreCase))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не конкретиизирована категория;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не конкретиизирована категория; ";
                    row.Cells["Должность в ОО"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkPPPosition()
        {
            //--+(CASE WHEN sp.SWorkerPositionname IS NULL THEN 'Не указана должность в ППЭ; ' ELSE '' END) --Проверка на наличие должности в ППЭ 
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Должность в ППЭ"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указана должность в ППЭ;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не указана должность в ППЭ; ";
                    row.Cells["Должность в ППЭ"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkPPAttachment()
        {
            //--+(CASE WHEN swos.StationCode IS NULL THEN 'Нет прикрепления к ППЭ; ' ELSE '' END) --Проверка на прикрепление работника к ППЭ
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Прикреплен к ППЭ"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Нет прикрепления к ППЭ;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Нет прикрепления к ППЭ; ";
                    row.Cells["Прикреплен к ППЭ"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkWorkerCode()
        {
            /*,(CASE WHEN w.StationWorkerCode IS NULL THEN 'Нет кода работника; ' ELSE '' END) --Проверка на наличие кода работника*/
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Код работника"].Value.Equals(0))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) ||
                        !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Нет кода работника;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Нет кода работника; ";
                }
            }
        }
        private void checkEmail()
        {
            //[Почта]
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Почта"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Нет почты;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Нет почты; ";
                    row.Cells["Почта"].Style.BackColor = Color.Aqua;
                }
            }
        }
        private void checkMobilePhone()
        {
            /*--+(CASE WHEN w.Phones ='' THEN 'Не указан телефон; ' ELSE '' END) --Проверка на наличие телефона
                --+(CASE WHEN (w.Phones LIKE '8395%'  OR w.Phones LIKE '8(395%' OR w.Phones LIKE '8-395%' OR w.Phones LIKE '(8395%' OR 
                --             w.Phones LIKE '(395%'  OR w.Phones LIKE '((395%' OR w.Phones LIKE '395%' OR
                --             w.Phones LIKE '+7395%' OR w.Phones LIKE '+7(395%')
                --			 AND 
                --		    (sp.SWorkerPositionname='Член ГЭК' OR sp.SWorkerPositionname='Руководитель ППЭ' OR sp.SWorkerPositionname='Технический специалист ППЭ')
                --	   THEN 'Указан номер стационарного телефона; ' ELSE '' END) -- Проверка на мобильный номер у работников ППЭ*/

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Телефоны"].Value.Equals(""))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Не указан телефон;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Не указан телефон; ";
                    row.Cells["Телефоны"].Style.BackColor = Color.Aqua;
                    }
                else if((
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^8395\w", RegexOptions.Compiled) ||
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^8\(395\w", RegexOptions.Compiled) ||
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^\(8395\w", RegexOptions.Compiled) ||
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^\(395\w", RegexOptions.Compiled) ||
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^\(\(395\w", RegexOptions.Compiled) ||
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^395\w", RegexOptions.Compiled) ||
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^+7395\w", RegexOptions.Compiled) ||
                        Regex.IsMatch((string)row.Cells["Телефоны"].Value, @"^+7\(395\w", RegexOptions.Compiled) 
                        ) && (
                        row.Cells["Должность в ППЭ"].Value.Equals("Член ГЭК") ||
                        row.Cells["Должность в ППЭ"].Value.Equals("Руководитель ППЭ") ||
                        row.Cells["Должность в ППЭ"].Value.Equals("Технический специалист ППЭ") ||
                        row.Cells["Должность в ППЭ"].Value.Equals("Организатор в аудитории ППЭ") ||
                        row.Cells["Должность в ППЭ"].Value.Equals("Организатор вне аудитории ППЭ")
                   ))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\w*Указан номер стационарного телефона;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Указан номер стационарного телефона; ";
                    row.Cells["Телефоны"].Style.BackColor = Color.Aqua;
                }
              
            } 
        }
        private void checkWorkersSpecialization()
        {
            /*
             *  (w.SWorkerCategory LIKE '%Русск%'    OR w.SWorkerCategory LIKE '%русск%' OR 
			 w.SchoolPosition LIKE '%Русск%'    OR w.SchoolPosition LIKE '%русск%' OR
			 ) 
			 AND
			(sws.[Нет] = 1)*/
            foreach (DataGridViewRow row in dataGridView.Rows)
            {

                if ((
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sРусск\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sМатемат\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sФизик\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sХими\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sИнформат\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sБиолог\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sИстор\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sГеограф\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sАнгли\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sНемец\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sФранцуз\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sОбщество\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sИспанс\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Должность в ОО"].Value, @"\sЛитерат\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sЛитерат\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sМатемат\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sФизик\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sХими\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sИнформат\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sБиолог\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sИстор\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sГеограф\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sАнгли\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sНемец\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sФранцуз\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sОбщество\s", RegexOptions.IgnoreCase) ||

                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sИспанс\s", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch((string)row.Cells["Квалификационная категория"].Value, @"\sЛитерат\s", RegexOptions.IgnoreCase) 
                    )
                    && row.Cells["Должность в ОО"].Value.Equals(1))
                {
                    if (row.Cells["Ошибки"].Value.GetType().Equals(typeof(DBNull)) || !Regex.IsMatch((string)row.Cells["Ошибки"].Value, @"\wОтсутсвует специализация;\w*", RegexOptions.IgnoreCase))
                        row.Cells["Ошибки"].Value += "Отсутсвует специализация; ";
                    row.Cells["Должность в ОО"].Style.BackColor = Color.Aqua;
                }
            }
        }
        #endregion
        #region EgeChecking
        /***********ege********************/
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
