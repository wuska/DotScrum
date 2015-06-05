using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace dotScrum.Xml
{
    public enum XmlResult
    {
        OK,
        NOT_OK,
        ITEM_IS_NOT_VALID,
        ITEM_NOT_FOUND,
        ITEM_ALREADY_EXISTS
    }

    public enum XmlTaskParameter
    {
        NONE,
        TITLE,
        DESCRIPTION,
        PRIORITY,
        VALUE,
        USER_STORY_NAME
    }

    public class XmlHandler
    {
        private XmlDocument xmlDoc = new XmlDocument();
        private XmlDocument xmlScrumBoard = new XmlDocument();
        private string xmlPath;

        private XmlNode projectNode;
        private XmlNode backlogNode;
        private XmlNode scrumBoardNode;
        private int selectedSprintNumber;
        private List<string> userStoryNameList = new List<string>();

        private const string currentScrumBoardFilePath = "~/Xml/currentScrumBoard.xml";
        private const string projectNodeName = "Project";
        private const string projectNameAttributeName = "name";
        private const string backlogNodeName = "Backlog";
        private const string userStoryNodeName = "UserStory";
        private const string userStoryNameTag = "Name";
        private const string userStoryDescriptionTag = "Description";
        private const string userStoryValueTag = "Value";
        private const string sprintNodeName = "Sprint";
        private const string sprintNumberAttributeName = "number";
        private const string scrumBoardNodeName = "ScrumBoard";
        private const string columnNodeName = "Column";
        private const string columnTitleTag = "ColumnTitle";
        private const string taskNodeName = "Task";
        private const string taskTitleTag = "Title";
        private const string taskDescTag = "Description";
        private const string taskPrioTag = "Priority";
        private const string taskValueTag = "Value";
        private const string taskUserStoryNameTag = "UserStoryName";

        public static int NOT_VALID = -1;
        public static int LAST_SPRINT = -2;

        public XmlHandler(ref string xmlPath)
        {
            this.xmlPath = xmlPath;
            xmlDoc.Load(this.xmlPath);
            projectNode = xmlDoc.SelectSingleNode("/" + projectNodeName);
            initializeBacklogNode();
            initializeUserStoryList();
            initializeScrumBoardNode();
            selectSprintScrumBoard(LAST_SPRINT);
        }

        private void initializeBacklogNode()
        {
            foreach (XmlNode node in projectNode.ChildNodes)
            {
                if (!node.Name.Equals(backlogNodeName))
                {
                    continue;
                }

                backlogNode = node;
            }
        }

        private void initializeUserStoryList()
        {
            foreach (XmlNode userStoryNode in backlogNode.ChildNodes)
            {
                if (!userStoryNode.Name.Equals(userStoryNodeName))
                {
                    continue;
                }

                foreach (XmlNode userStoryTag in userStoryNode.ChildNodes)
                {
                    if (!userStoryTag.Name.Equals(userStoryNameTag))
                    {
                        continue;
                    }

                    userStoryNameList.Add(userStoryTag.InnerText);
                }
            }
        }

        private void initializeScrumBoardNode()
        {
            scrumBoardNode = xmlScrumBoard.CreateElement(scrumBoardNodeName);
        }

        public static string translateErrorCode(ref XmlResult code)
        {
            switch (code)
            {
                case XmlResult.OK:
                    return "ok";
                case XmlResult.NOT_OK:
                    return "not ok";
                case XmlResult.ITEM_NOT_FOUND:
                    return "item not found";
                case XmlResult.ITEM_IS_NOT_VALID:
                    return "item is not valid";
                case XmlResult.ITEM_ALREADY_EXISTS:
                    return "item already exists";
            }

            return "not valid error code";
        }

        public string getXmlFilePath()
        {
            return this.xmlPath;
        }

        public XmlResult addColumn(string columnName)
        {
            XmlNode columnNode = createColumnNode(ref columnName);
            if (getColumnNames().Contains(columnName))
            {
                return XmlResult.ITEM_ALREADY_EXISTS;
            }

            scrumBoardNode.AppendChild(columnNode);
            return XmlResult.OK;
        }

        public XmlResult addTask(string taskTitle, string taskDesc, string taskPrio, string taskValue, string userStoryName)
        {
            string columnName;
            XmlResult result = getFirstColumnName(out columnName);
            if (!result.Equals(XmlResult.OK)) 
            {
                return result;
            }

            XmlNode destColumnNode;
            result = getSrumBoardColumnByName(ref columnName, out destColumnNode);
            if (result != XmlResult.OK)
            {
                return result;
            }

            XmlNode taskNode = createTaskNode(ref taskTitle, ref taskDesc, ref taskPrio, ref taskValue, ref userStoryName);

            destColumnNode.AppendChild(taskNode);
            return result;
        }

        private XmlResult getFirstColumnName(out string columnName)
        {
            foreach (XmlNode columnNode in scrumBoardNode.ChildNodes)
            {
                if (!columnNode.Name.Equals(columnNodeName))
                {
                    continue;
                }

                foreach (XmlNode columnChild in columnNode.ChildNodes)
                {
                    if (!columnChild.Name.Equals(columnTitleTag))
                    {
                        continue;
                    }

                    columnName = columnChild.InnerText;
                    return XmlResult.OK;
                }
            }

            columnName = "";
            return XmlResult.OK;
        }

        private XmlNode createColumnNode(ref string columnName)
        {
            XmlNode columnNode = xmlScrumBoard.CreateElement(columnNodeName);

            XmlNode columnNameNode = xmlScrumBoard.CreateElement(columnTitleTag);
            columnNameNode.InnerText = columnName;
            columnNode.AppendChild(columnNameNode);

            return columnNode;
        }

        private XmlNode createColumnNodeInProjectNode(string columnName)
        {
            XmlNode columnNode = xmlDoc.CreateElement(columnNodeName);

            XmlNode columnNameNode = xmlDoc.CreateElement(columnTitleTag);
            columnNameNode.InnerText = columnName;
            columnNode.AppendChild(columnNameNode);

            return columnNode;
        }

        private XmlNode createTaskNode(ref string title, ref string description, ref string priority, ref string value,
                                       ref string userStoryName)
        {
            XmlNode taskNode = xmlScrumBoard.CreateElement(taskNodeName);

            XmlNode taskTitleNode = xmlScrumBoard.CreateElement(taskTitleTag);
            taskTitleNode.InnerText = title;
            taskNode.AppendChild(taskTitleNode);

            XmlNode taskDescNode = xmlScrumBoard.CreateElement(taskDescTag);
            taskDescNode.InnerText = description;
            taskNode.AppendChild(taskDescNode);

            XmlNode taskPrioNode = xmlScrumBoard.CreateElement(taskPrioTag);
            taskPrioNode.InnerText = priority;
            taskNode.AppendChild(taskPrioNode);

            XmlNode taskValueNode = xmlScrumBoard.CreateElement(taskValueTag);
            taskValueNode.InnerText = value;
            taskNode.AppendChild(taskValueNode);

            XmlNode taskUserStoryName = xmlScrumBoard.CreateElement(taskUserStoryNameTag);
            taskUserStoryName.InnerText = userStoryName;
            taskNode.AppendChild(taskUserStoryName);

            return taskNode;
        }

        private XmlResult getSrumBoardColumnByName(ref string columnName, out XmlNode column)
        {
            foreach (XmlNode xmlColumn in scrumBoardNode.ChildNodes)
            {
                if (xmlColumn.Name.Equals(columnNodeName))
                {
                    if (xmlColumn.FirstChild != null &&
                        xmlColumn.FirstChild.Name.Equals(columnTitleTag) &&
                        xmlColumn.FirstChild.InnerText.Equals(columnName))
                    {
                        column = xmlColumn;
                        return XmlResult.OK;
                    }
                }
            }

            column = null;
            return XmlResult.ITEM_NOT_FOUND;
        }

        public XmlResult deleteColumnByName(ref string columnName)
        {
            foreach (XmlNode columnNode in scrumBoardNode.ChildNodes)
            {
                if (!columnNode.Name.Equals(columnNodeName))
                {
                    continue;
                }

                foreach (XmlNode columnTitleNode in columnNode.ChildNodes)
                {
                    if (!columnTitleNode.Name.Equals(columnTitleTag))
                    {
                        continue;
                    }

                    if (columnTitleNode.InnerText.Equals(columnName))
                    {
                        columnNode.ParentNode.RemoveChild(columnNode);
                        return XmlResult.OK;
                    }
                }
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        public XmlResult deleteTask(string taskTitle, string taskDescription, string taskPriority, 
                                    string taskValue, string taskUserStoryName, string columnName)
        {
            XmlNode columnNode;
            XmlResult result = getSrumBoardColumnByName(ref columnName, out columnNode);
            if (result != XmlResult.OK)
            {
                return result;
            }

            return deleteTask(ref taskTitle, ref taskDescription, ref taskPriority, 
                              ref taskValue, ref taskUserStoryName, ref columnNode);
        }

        private XmlResult deleteTask(ref string taskTitle, ref string taskDescription, ref string taskPriority,
                                     ref string taskValue, ref string userStoryName, ref XmlNode columnNode)
        {
            XmlNode wantedTask = createTaskNode(ref taskTitle, ref taskDescription, ref taskPriority, ref taskValue, 
                                                ref userStoryName);
            foreach (XmlNode columnChild in columnNode.ChildNodes)
            {
                if (!columnChild.Name.Equals(taskNodeName))
                {
                    continue;
                }

                if (columnChild.InnerXml.Equals(wantedTask.InnerXml))
                {
                    columnChild.ParentNode.RemoveChild(columnChild);
                    return XmlResult.OK;
                }
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        public XmlResult saveCurrentProjectToXmlFile()
        {
            return saveCurrentProjectToXmlFile(this.xmlPath);
        }

        public XmlResult saveCurrentProjectToXmlFile(string xmlFilePath)
        {
            this.xmlPath = xmlFilePath;
            xmlDoc.Save(this.xmlPath);

            return XmlResult.OK;
        }

        public XmlResult resetProjectFile(string projectName)
        {
            xmlDoc.RemoveAll();

            addXmlDeclaration();
            XmlResult result = addRootNode(projectName);
            if (result != XmlResult.OK)
            {
                return result;
            }

            result = addBacklogNode();
            if (result != XmlResult.OK)
            {
                return XmlResult.ITEM_IS_NOT_VALID;
            }
            
            return saveCurrentProjectToXmlFile();
        }

        private void addXmlDeclaration()
        {
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = xmlDoc.DocumentElement;
            xmlDoc.InsertBefore(xmlDeclaration, root);
        }

        private XmlResult addRootNode(string projectName)
        {
            XmlNode rootNode = xmlDoc.CreateElement(projectNodeName);

            XmlAttribute projectNameAttr = xmlDoc.CreateAttribute(projectNameAttributeName);
            projectNameAttr.InnerText = projectName;
            rootNode.Attributes.Append(projectNameAttr);
            
            xmlDoc.AppendChild(rootNode);
            projectNode = rootNode;

            return XmlResult.OK;
        }

        private XmlResult addBacklogNode()
        {
            if (projectNode.InnerXml.Contains(backlogNodeName))
            {
                return XmlResult.ITEM_ALREADY_EXISTS;
            }

            XmlNode newBacklogNode = xmlDoc.CreateElement(backlogNodeName);
            projectNode.AppendChild(newBacklogNode);
            backlogNode = newBacklogNode;

            return XmlResult.OK;
        }

        private XmlNode createSprintNode(int sprintNumber)
        {
            XmlNode sprintNode = xmlDoc.CreateElement(sprintNodeName);
            XmlAttribute sprintNumberAttribute = xmlDoc.CreateAttribute(sprintNumberAttributeName);
            sprintNode.Attributes.Append(sprintNumberAttribute);
            sprintNumberAttribute.InnerText = sprintNumber.ToString();

            return sprintNode;
        }

        public XmlResult getLastSprintNumber(out int sprintNumber)
        {
            XmlNode lastSprint = projectNode.LastChild;
            if (lastSprint == null || !lastSprint.Name.Equals(sprintNodeName))
            {
                sprintNumber = NOT_VALID;
                return XmlResult.ITEM_NOT_FOUND;
            }
            
            if (lastSprint.Attributes[sprintNumberAttributeName] == null)
            {
                sprintNumber = NOT_VALID;
                return XmlResult.ITEM_IS_NOT_VALID;
            }

            sprintNumber = NOT_VALID;
            if (int.TryParse(lastSprint.Attributes[sprintNumberAttributeName].InnerText, out sprintNumber))
            {
                return XmlResult.OK;
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        public XmlResult addSprint()
        {
            int sprintNumber;
            XmlResult result = getLastSprintNumber(out sprintNumber);

            switch (result)
            {
                case XmlResult.ITEM_IS_NOT_VALID:
                    return result;
                case XmlResult.ITEM_NOT_FOUND:
                    sprintNumber = 0;
                    break;
                case XmlResult.OK:
                    sprintNumber++;
                    break;
            }

            XmlNode newSprintNode = createSprintNode(sprintNumber);
            XmlNode scmBoardNode = createProjectScrumBoardNode();

            List<string> columnNames = getColumnNames();
            foreach (string columnName in columnNames)
            {
                XmlNode columnNode = createColumnNodeInProjectNode(columnName);
                scmBoardNode.AppendChild(columnNode);
            }

            newSprintNode.AppendChild(scmBoardNode);
            projectNode.AppendChild(newSprintNode);

            return XmlResult.OK;
        }

        private XmlNode createProjectScrumBoardNode()
        {
            return xmlDoc.CreateElement(scrumBoardNodeName);
        }

        private XmlResult getSprintNode(int sprintNumber, out XmlNode sprintNode)
        {
            if (sprintNumber == LAST_SPRINT)
            {
                if (projectNode.LastChild == null)
                {
                    sprintNode = null;
                    return XmlResult.ITEM_NOT_FOUND;
                }

                if (!projectNode.LastChild.Name.Equals(sprintNodeName))
                {
                    sprintNode = null;
                    return XmlResult.ITEM_IS_NOT_VALID;
                }

                sprintNode = projectNode.LastChild;
                return XmlResult.OK;
            }

            foreach (XmlNode node in projectNode.ChildNodes)
            {
                if (node.Name.Equals(sprintNodeName))
                {
                    if (node.Attributes[sprintNumberAttributeName] == null)
                    {
                        continue;
                    }

                    int currentSprintNumber = NOT_VALID;
                    if (int.TryParse(node.Attributes[sprintNumberAttributeName].InnerText, out currentSprintNumber) == false)
                    {
                        sprintNode = null;
                        return XmlResult.ITEM_IS_NOT_VALID;
                    }

                    if (currentSprintNumber == sprintNumber)
                    {
                        sprintNode = node;
                        return XmlResult.OK;
                    }
                }
            }

            sprintNode = null;
            return XmlResult.ITEM_NOT_FOUND;
        }

        private XmlResult getProjectScrumBoard(int sprintNumber, out XmlNode scrumBoard)
        {
            XmlNode sprintNode;
            XmlResult result = getSprintNode(sprintNumber, out sprintNode);
            if (result != XmlResult.OK)
            {
                scrumBoard = null;
                return result;
            }

            foreach (XmlNode node in sprintNode.ChildNodes)
            {
                if (!node.Name.Equals(scrumBoardNodeName))
                {
                    continue;
                }

                scrumBoard = node;
                return XmlResult.OK;
            }

            scrumBoard = null;
            return XmlResult.ITEM_NOT_FOUND;
        }

        public XmlResult addProjectScrumBoard(int sprintNumber)
        {
            XmlNode sprintNode;
            XmlResult result = getSprintNode(sprintNumber, out sprintNode);
            if (result != XmlResult.OK)
            {
                return result;
            }

            XmlNode tmpScrumBoardNode;
            if (getProjectScrumBoard(sprintNumber, out tmpScrumBoardNode) == XmlResult.OK)
            {
                return XmlResult.OK;
            }

            tmpScrumBoardNode = xmlDoc.CreateElement(scrumBoardNodeName);
            sprintNode.AppendChild(tmpScrumBoardNode);

            return XmlResult.OK;
        }

        public XmlResult selectSprintScrumBoard(int sprintNumber)
        {
            XmlNode scrumBoard;
            XmlResult result = getProjectScrumBoard(sprintNumber, out scrumBoard);
            if (result == XmlResult.OK)
            {
                selectedSprintNumber = sprintNumber;

                return saveScrumBoardAsXmlFile(ref scrumBoard);
            }

            return result;
        }

        public void writeXmlToConsole()
        {
            String xmlFile = File.ReadAllText(this.xmlPath);
            Console.Write(xmlFile);
        }

        public string getCurrentScrumBoardFilePath()
        {
            return currentScrumBoardFilePath;
        }

        private XmlResult saveScrumBoardAsXmlFile(ref XmlNode scrumBoardToSave)
        {
            string xmlCurrentScrumBoardFilePath = HttpContext.Current.Server.MapPath(currentScrumBoardFilePath);
            xmlScrumBoard = createScrumBoardAsXmlFile(ref scrumBoardToSave);
            xmlScrumBoard.Save(xmlCurrentScrumBoardFilePath);
            scrumBoardNode = xmlScrumBoard.SelectSingleNode("/" + scrumBoardNodeName);

            return XmlResult.OK;
        }

        private XmlDocument createScrumBoardAsXmlFile(ref XmlNode scrumBoard)
        {
            XmlDocument docScrumBoard = new XmlDocument();

            XmlDeclaration xmlDeclaration = docScrumBoard.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = docScrumBoard.DocumentElement;
            docScrumBoard.InsertBefore(xmlDeclaration, root);

            copyScrumBoardIntoNewDocument(ref scrumBoard, ref docScrumBoard);

            return docScrumBoard;
        }

        private XmlResult copyScrumBoardIntoNewDocument(ref XmlNode scrumBoardToCopy, ref XmlDocument newDocument)
        {
            XmlNode tmpScrumBoardNode = newDocument.CreateElement(scrumBoardNodeName);
            newDocument.AppendChild(tmpScrumBoardNode);

            tmpScrumBoardNode.InnerXml = scrumBoardToCopy.InnerXml;

            return XmlResult.OK;
        }

        public XmlResult joinScrumBoardToProject()
        {
            XmlNode projScrumBoard;
            XmlResult result = getProjectScrumBoard(selectedSprintNumber, out projScrumBoard);
            if (!result.Equals(XmlResult.OK))
            {
                return result;
            }

            if (!projScrumBoard.InnerXml.Equals(scrumBoardNode.InnerXml))
            {
                return replaceProjectScrumBoardWithCurrent(ref projScrumBoard);
            }

            return XmlResult.OK;
        }

        private XmlResult replaceProjectScrumBoardWithCurrent(ref XmlNode projScrumBoard)
        {
            XmlNode sprintNode;
            XmlResult result = getSprintNode(selectedSprintNumber, out sprintNode);
            if (!result.Equals(XmlResult.OK))
            {
                return result;
            }

            projScrumBoard.InnerXml = scrumBoardNode.InnerXml;
            return XmlResult.OK;
        }

        public XmlResult changeColumnName(string oldColumnName, string newColumnName)
        {
            foreach (XmlNode columnNode in scrumBoardNode.ChildNodes)
            {
                if (!columnNode.Name.Equals(columnNodeName))
                {
                    continue;
                }

                foreach (XmlNode columnTitleNode in columnNode.ChildNodes)
                {
                    if (!columnTitleNode.Name.Equals(columnTitleTag))
                    {
                        continue;
                    }

                    if (columnTitleNode.InnerText.Equals(oldColumnName))
                    {
                        columnTitleNode.InnerText = newColumnName;
                        return XmlResult.OK;
                    }
                }
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        private XmlResult changeColumnNameInProjectNode(string oldColumnName, string newColumnName)
        {
            int lastSprintNumber;
            XmlResult result = getLastSprintNumber(out lastSprintNumber);
            if (result != XmlResult.OK)
            {
                return result;
            }

            for (int sprintNumber = 0; sprintNumber <= lastSprintNumber; sprintNumber++)
            {
                XmlNode sprintNode;
                result = getSprintNode(sprintNumber, out sprintNode);
                if (result != XmlResult.OK)
                {
                    return result;
                }

                changeColumnNameInSprintNode(ref sprintNode, oldColumnName, newColumnName);
            }

            return XmlResult.OK;
        }

        private void changeColumnNameInSprintNode(ref XmlNode sprintNode, string oldColumnName, string newColumnNode)
        {
            foreach (XmlNode columnNode in sprintNode.ChildNodes)
            {
                if (!columnNode.Name.Equals(columnNodeName))
                {
                    continue;
                }

                foreach (XmlNode columnTitle in columnNode.ChildNodes)
                {
                    if (!columnTitle.Name.Equals(columnTitleTag))
                    {
                        continue;
                    }

                    if (columnTitle.InnerText.Equals(oldColumnName))
                    {
                        columnTitle.InnerText = newColumnNode;
                    }
                }
            }
        }

        public XmlResult getColumnAsDataSet(string columnName, out DataSet dataSet)
        {
            XmlNode columnNode;
            dataSet = null;
            XmlResult result = getSrumBoardColumnByName(ref columnName, out columnNode);
            if (result != XmlResult.OK)
            {
                return result;
            }

            DataTable tasks;
            result = fillTaskDataTable(ref columnNode, out tasks);
            if (result != XmlResult.OK)
            {
                return result;
            }

            if (tasks == null)
            {
                return XmlResult.OK;
            }

            dataSet = new DataSet();
            dataSet.Tables.Add(tasks);

            return XmlResult.OK;
        }

        private XmlResult fillTaskDataTable(ref XmlNode columnNode, out DataTable tasks)
        {
            tasks = new DataTable();
            tasks.Columns.Add(taskTitleTag, typeof(string));
            tasks.Columns.Add(taskDescTag, typeof(string));
            tasks.Columns.Add(taskPrioTag, typeof(string));
            tasks.Columns.Add(taskValueTag, typeof(string));
            tasks.Columns.Add(taskUserStoryNameTag, typeof(string));

            foreach (XmlNode columnChild in columnNode.ChildNodes)
            {
                if (!columnChild.Name.Equals(taskNodeName))
                {
                    continue;
                }

                addTaskToDataTable(columnChild, ref tasks);
            }

            return XmlResult.OK;
        }

        private void addTaskToDataTable(XmlNode taskNode, ref DataTable tasks)
        {
            string taskTitle = "";
            string taskDescription = "";
            string taskPriority = "";
            string taskValue = "";
            string taskUserStoryName = "";

            foreach (XmlNode taskChild in taskNode.ChildNodes)
            {
                switch (taskChild.Name)
                {
                    case taskTitleTag:
                        taskTitle = taskChild.InnerText;
                        break;
                    case taskDescTag:
                        taskDescription = taskChild.InnerText;
                        break;
                    case taskPrioTag:
                        taskPriority = taskChild.InnerText;
                        break;
                    case taskValueTag:
                        taskValue = taskChild.InnerText;
                        break;
                    case taskUserStoryNameTag:
                        taskUserStoryName = taskChild.InnerText;
                        break;
                }
            }

            tasks.Rows.Add(taskTitle, taskDescription, taskPriority, taskValue, taskUserStoryName);
        }

        private List<string> getColumnNames()
        {
            List<string> columnNames = new List<string>();

            foreach (XmlNode columnNode in scrumBoardNode.ChildNodes)
            {
                if (columnNode.Name == null || !columnNode.Name.Equals(columnNodeName))
                {
                    continue;
                }

                foreach (XmlNode columnChild in columnNode.ChildNodes)
                {
                    if (columnChild.Name == null || !columnChild.Name.Equals(columnTitleTag))
                    {
                        continue;
                    }

                    columnNames.Add(columnChild.InnerText);
                    break;
                }
            }

            return columnNames;
        }

        public XmlResult moveTask(string title, string description, string priority, string value, string userStoryName, 
                                  int columnIndex)
        {
            if (columnIndex == NOT_VALID)
            {
                return XmlResult.ITEM_IS_NOT_VALID;
            }

            List<string> columnNames = getColumnNames();
            return moveTask(ref title, ref description, ref priority, ref value, ref userStoryName, ref columnIndex, 
                            ref columnNames);
        }

        private XmlResult moveTask(ref string title, ref string description, ref string priority, ref string value, 
                                   ref string userStoryName, ref int destColumnIndex, ref List<string> columnNames)
        {
            if (columnNames.Count < destColumnIndex)
            {
                return XmlResult.ITEM_NOT_FOUND;
            }

            string destColumnName = columnNames.ElementAt(destColumnIndex);
            XmlNode columnNode;
            XmlResult result = getSrumBoardColumnByName(ref destColumnName, out columnNode);
            if (result != XmlResult.OK)
            {
                return result;
            }

            columnNode.AppendChild(createTaskNode(ref title, ref description, ref priority, ref value, ref userStoryName));

            foreach (string columnName in columnNames)
            {
                if (columnName.Equals(destColumnName))
                {
                    continue;
                }

                result = deleteTask(title, description, priority, value, userStoryName, columnName);
                if (result == XmlResult.ITEM_NOT_FOUND)
                {
                    continue;
                }

                break;
            }

            return result;
        }

        private XmlResult changeTaskParameter(ref XmlNode oldTaskNode, ref XmlNode newTaskNode)
        {
            foreach (XmlNode column in scrumBoardNode.ChildNodes)
            {
                if (!column.InnerXml.Contains(oldTaskNode.InnerXml))
                {
                    continue;
                }

                foreach (XmlNode task in column.ChildNodes)
                {
                    if (!task.Name.Equals(taskNodeName))
                    {
                        continue;
                    }

                    task.InnerXml = newTaskNode.InnerXml;
                    return XmlResult.OK;
                }
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        public XmlResult changeTaskTitle(string oldTitle, string newTitle, string description, string priority, string value,
                                         string userStoryName)
        {
            XmlNode oldTaskNode = createTaskNode(ref oldTitle, ref description, ref priority, ref value, ref userStoryName);
            XmlNode newTaskNode = createTaskNode(ref newTitle, ref description, ref priority, ref value, ref userStoryName);
            return changeTaskParameter(ref oldTaskNode, ref newTaskNode);
        }

        public XmlResult changeTaskDescription(string oldDescriprion, string newDescription, string title, string priority, 
                                               string value, string userStoryName)
        {
            XmlNode oldTaskNode = createTaskNode(ref title, ref oldDescriprion, ref priority, ref value, ref userStoryName);
            XmlNode newTaskNode = createTaskNode(ref title, ref newDescription, ref priority, ref value, ref userStoryName);
            return changeTaskParameter(ref oldTaskNode, ref newTaskNode);
        }

        public XmlResult changeTaskPriority(string oldPriority, string newPriority, string title, string description, 
                                            string value, string userStoryName)
        {
            XmlNode oldTaskNode = createTaskNode(ref title, ref description, ref oldPriority, ref value, ref userStoryName);
            XmlNode newTaskNode = createTaskNode(ref title, ref description, ref newPriority, ref value, ref userStoryName);
            return changeTaskParameter(ref oldTaskNode, ref newTaskNode);
        }

        public XmlResult changeTaskValue(string oldValue, string newValue, string title, string description, string priority,
                                         string userStoryName)
        {
            XmlNode oldTaskNode = createTaskNode(ref title, ref description, ref priority, ref oldValue, ref userStoryName);
            XmlNode newTaskNode = createTaskNode(ref title, ref description, ref priority, ref newValue, ref userStoryName);
            return changeTaskParameter(ref oldTaskNode, ref newTaskNode);
        }

        public XmlResult changeTaskUserStoryName(string oldUserStoryName, string newUserStoryName, string title, 
                                                 string description, string priority, string value)
        {
            XmlNode oldTaskNode = createTaskNode(ref title, ref description, ref priority, ref value, ref oldUserStoryName);
            XmlNode newTaskNode = createTaskNode(ref title, ref description, ref priority, ref value, ref newUserStoryName);
            return changeTaskParameter(ref oldTaskNode, ref newTaskNode);
        }

        private XmlNode createUserStoryNode(ref string usName, ref string usDescription, ref string usValue)
        {
            XmlNode userStoryNode = xmlDoc.CreateElement(userStoryNodeName);

            XmlNode usNameNode = xmlDoc.CreateElement(userStoryNameTag);
            usNameNode.InnerText = usName;
            userStoryNode.AppendChild(usNameNode);

            XmlNode usDescriptionNode = xmlDoc.CreateElement(userStoryDescriptionTag);
            usDescriptionNode.InnerText = usDescription;
            userStoryNode.AppendChild(usDescriptionNode);

            XmlNode usValueNode = xmlDoc.CreateElement(userStoryValueTag);
            usValueNode.InnerText = usValue;
            userStoryNode.AppendChild(usValueNode);

            return userStoryNode;
        }

        public XmlResult addUserStory(string usName, string usDescription, string usValue)
        {
            XmlNode newUserStoryNode = createUserStoryNode(ref usName, ref usDescription, ref usValue);
            if (backlogNode.InnerXml.Contains(newUserStoryNode.InnerXml))
            {
                return XmlResult.ITEM_ALREADY_EXISTS;
            }

            backlogNode.AppendChild(newUserStoryNode);
            userStoryNameList.Add(usName);
            return XmlResult.OK;
        }

        public XmlResult deleteUserStory(string usName, string usDescription, string usValue)
        {
            XmlNode userStory = createUserStoryNode(ref usName, ref usDescription, ref usValue);
            foreach (XmlNode tmpUserStory in backlogNode.ChildNodes)
            {
                if (!tmpUserStory.Name.Equals(userStoryNodeName))
                {
                    continue;
                }

                if (tmpUserStory.InnerXml.Equals(userStory.InnerXml))
                {
                    tmpUserStory.ParentNode.RemoveChild(tmpUserStory);
                    userStoryNameList.Remove(usName);
                    return XmlResult.OK;
                }
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        public List<string> getUserStoryNames()
        {
            return userStoryNameList;
        }

        public XmlResult changeProjectName(string newProjectName)
        {
            XmlAttribute projectNameAttribute = projectNode.Attributes[projectNameAttributeName];
            if (projectNameAttribute == null)
            {
                return XmlResult.ITEM_IS_NOT_VALID;
            }

            projectNameAttribute.InnerText = newProjectName;
            return XmlResult.OK;
        }

        public XmlResult getProjectName(ref string projectName)
        {
            XmlAttribute projectNameAttribute = projectNode.Attributes[projectNameAttributeName];
            if (projectNameAttribute == null)
            {
                return XmlResult.ITEM_IS_NOT_VALID;
            }

            projectName = projectNameAttribute.InnerText;
            return XmlResult.OK;
        }

        private XmlResult getSprintScrumBoard(int sprintNumber, out XmlNode scrumBoardNode)
        {
            scrumBoardNode = null;
            XmlNode sprintNode;
            XmlResult result = getSprintNode(sprintNumber, out sprintNode);
            if (result != XmlResult.OK)
            {
                return result;
            }

            foreach (XmlNode sprintChild in sprintNode.ChildNodes)
            {
                if (!sprintChild.Name.Equals(scrumBoardNodeName))
                {
                    continue;
                }

                scrumBoardNode = sprintChild;
                return XmlResult.OK;
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        private XmlResult doesUserStoryHaveTaskAndTasksAsNotDoneInSprint(ref string userStoryName, 
                                                                         ref string lastColumnName,
                                                                         int sprintNumber,
                                                                         ref bool haveTask, 
                                                                         ref bool taskNotDone)
        {
            XmlNode sprintScrumBoard;
            XmlResult result = getSprintScrumBoard(sprintNumber, out sprintScrumBoard);
            if (result != XmlResult.OK)
            {
                return result;
            }

            string currentColumnName = null;
            foreach (XmlNode columnNode in sprintScrumBoard.ChildNodes)
            {
                if (!columnNode.Name.Equals(columnNodeName))
                {
                    continue;
                }

                foreach (XmlNode columnChildNode in columnNode.ChildNodes)
                {
                    if (columnChildNode.Name.Equals(columnTitleTag))
                    {
                        currentColumnName = columnChildNode.InnerText;
                    }

                    if (!columnChildNode.Name.Equals(taskNodeName))
                    {
                        continue;
                    }

                    foreach (XmlNode taskParameter in columnChildNode.ChildNodes)
                    {
                        if (!taskParameter.Name.Equals(taskUserStoryNameTag))
                        {
                            continue;
                        }

                        if (!taskParameter.InnerText.Equals(userStoryName))
                        {
                            continue;
                        }

                        haveTask = true;

                        if (currentColumnName != null && !currentColumnName.Equals(lastColumnName))
                        {
                            taskNotDone = true;
                            return XmlResult.OK;
                        }
                    }
                }
            }

            return XmlResult.OK;
        }

        private XmlResult doesUserStoryHaveTasksAndTasksAsNotDone(string userStoryName, 
                                                                  ref string lastColumnName,
                                                                  ref bool haveTask, 
                                                                  ref bool taskNotDone)
        {
            int lastSprintNumber;
            XmlResult result = getLastSprintNumber(out lastSprintNumber);
            if (result != XmlResult.OK)
            {
                return result;
            }

            for (int sprintNumber = 0; sprintNumber <= lastSprintNumber; sprintNumber++)
            {
                result = doesUserStoryHaveTaskAndTasksAsNotDoneInSprint(ref userStoryName, ref lastColumnName, sprintNumber, 
                                                                        ref haveTask, ref taskNotDone);
                if (result != XmlResult.OK)
                {
                    return result;
                }

                if (taskNotDone && haveTask)
                {
                    return XmlResult.OK;
                }
            }

            return XmlResult.OK;
        }

        private void addElementToListIfDoesNotExists(ref List<string> list, string item)
        {
            if (list.Contains(item))
            {
                return;
            }

            list.Add(item);
        }

        private XmlResult getDoneAndNotDoneUserStoriesAsLists(out List<string> doneUserStories, out List<string> notDoneUserStories)
        {
            doneUserStories = new List<string>();
            notDoneUserStories = new List<string>();
            XmlResult result;
            string lastColumnName = getColumnNames().Last();
            bool haveTask, haveNotDoneTask;
            foreach (string userStoryName in userStoryNameList)
            {
                haveTask = false;
                haveNotDoneTask = false;
                result = doesUserStoryHaveTasksAndTasksAsNotDone(userStoryName, 
                                                                 ref lastColumnName, 
                                                                 ref haveTask, 
                                                                 ref haveNotDoneTask);
                if (result != XmlResult.OK)
                {
                    return result;
                }

                if (haveNotDoneTask || !haveTask)
                {
                    addElementToListIfDoesNotExists(ref notDoneUserStories, userStoryName);
                }
            }

            foreach (string doneUserStory in userStoryNameList)
            {
                if (!notDoneUserStories.Contains(doneUserStory))
                {
                    doneUserStories.Add(doneUserStory);
                }
            }

            return XmlResult.OK;
        }

        public XmlResult getDoneAndNotDoneUserStoriesAsDataSets(out DataSet doneUserStories,
                                                                out DataSet notDoneUserStories)
        {
            doneUserStories = null;
            notDoneUserStories = null;
            
            List<string> doneUserStoriesList;
            List<string> notDoneUserStoriesList;
            XmlResult result = getDoneAndNotDoneUserStoriesAsLists(out doneUserStoriesList, out notDoneUserStoriesList);
            if (result != XmlResult.OK)
            {
                return result;
            }

            result = getFilteredUserStoriesAsDataSet(out doneUserStories, ref notDoneUserStoriesList);
            if (result != XmlResult.OK)
            {
                return result;
            }

            result = getFilteredUserStoriesAsDataSet(out notDoneUserStories, ref doneUserStoriesList);
            if (result != XmlResult.OK)
            {
                return result;
            }

            return result;
        }

        private XmlResult getFilteredUserStoriesAsDataSet(out DataSet dataSet, ref List<string> userStoryNamesToFilter)
        {
            dataSet = new DataSet();
            DataTable userStories = new DataTable();
            userStories.Columns.Add(userStoryNameTag, typeof(string));
            userStories.Columns.Add(userStoryDescriptionTag, typeof(string));
            userStories.Columns.Add(userStoryValueTag, typeof(string));

            foreach (XmlNode userStoryNode in backlogNode.ChildNodes)
            {
                if (!userStoryNode.Name.Equals(userStoryNodeName))
                {
                    continue;
                }

                addUserStoryToDataTable(userStoryNode, ref userStories, ref userStoryNamesToFilter);
            }

            dataSet.Tables.Add(userStories);
            return XmlResult.OK;
        }

        private void addUserStoryToDataTable(XmlNode userStoryNode, ref DataTable userStories, 
                                             ref List<string> userStoryNamesToFilter)
        {
            string userStoryName = "";
            string userStoryDescription = "";
            string userStoryValue = "";

            foreach (XmlNode userStoryChild in userStoryNode.ChildNodes)
            {
                switch (userStoryChild.Name)
                {
                    case userStoryNameTag:
                        userStoryName = userStoryChild.InnerText;
                        break;
                    case userStoryDescriptionTag:
                        userStoryDescription = userStoryChild.InnerText;
                        break;
                    case userStoryValueTag:
                        userStoryValue = userStoryChild.InnerText;
                        break;
                }
            }

            if (userStoryNamesToFilter.Contains(userStoryName))
            {
                return;
            }

            userStories.Rows.Add(userStoryName, userStoryDescription, userStoryValue);
        }

        private XmlResult changeUserStory(ref XmlNode oldUserStoryNode, ref string[] userStoryParameters)
        {
            XmlNode newUserStoryNode = createUserStoryNode(ref userStoryParameters[0],
                                                           ref userStoryParameters[1],
                                                           ref userStoryParameters[2]);
            foreach (XmlNode userStoryNode in backlogNode.ChildNodes)
            {
                if (!userStoryNode.InnerXml.Equals(oldUserStoryNode.InnerXml))
                {
                    continue;
                }

                userStoryNode.InnerXml = newUserStoryNode.InnerXml;
                return XmlResult.OK;
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        private void changeScrumBoardAllTaskUserStoryName(XmlNode boardNode, ref string oldValue, ref string newValue)
        {
            foreach (XmlNode columnNode in boardNode.ChildNodes)
            {
                if (!columnNode.Name.Equals(columnNodeName))
                {
                    continue;
                }

                foreach (XmlNode taskNode in columnNode.ChildNodes)
                {
                    if (!taskNode.Name.Equals(taskNodeName))
                    {
                        continue;
                    }

                    foreach (XmlNode taskParameter in taskNode.ChildNodes)
                    {
                        if (!taskParameter.Name.Equals(taskUserStoryNameTag))
                        {
                            continue;
                        }

                        if (taskParameter.InnerText.Equals(oldValue))
                        {
                            taskParameter.InnerText = newValue;
                        }
                    }
                }
            }
        }

        private void changeSprintAllTaskUserStoryName(ref XmlNode sprintNode, ref string oldValue, ref string newValue)
        {
            foreach (XmlNode childNode in sprintNode.ChildNodes)
            {
                if (!childNode.Name.Equals(scrumBoardNodeName))
                {
                    continue;
                }

                changeScrumBoardAllTaskUserStoryName(childNode, ref oldValue, ref newValue);
            }
        }

        private XmlResult changeProjectAllTaskUserStoryName(ref string oldValue, ref string newValue)
        {
            int sprintNumber;
            XmlResult result = getLastSprintNumber(out sprintNumber);
            if (result != XmlResult.OK)
            {
                return result;
            }

            XmlNode sprintNode;
            for (int sprintNo = 0; sprintNo <= sprintNumber; sprintNo++)
            {
                result = getSprintNode(sprintNo, out sprintNode);
                if (result != XmlResult.OK)
                {
                    return result;
                }

                changeSprintAllTaskUserStoryName(ref sprintNode, ref oldValue, ref newValue);
            }
            
            return XmlResult.OK;
        }

        private XmlResult changeAllTaskUserStoryName(ref string oldValue, ref string newValue)
        {
            changeScrumBoardAllTaskUserStoryName(scrumBoardNode, ref oldValue, ref newValue);
            return changeProjectAllTaskUserStoryName(ref oldValue, ref newValue);
        }

        public XmlResult changeUserStoryName(string oldValue, string newValue, string[] userStoryParameters)
        {
            if (userStoryParameters.Length != 3)
            {
                return XmlResult.NOT_OK;
            }

            XmlNode oldUserStoryNode = createUserStoryNode(ref oldValue, ref userStoryParameters[1], ref userStoryParameters[2]);
            XmlResult result = changeUserStory(ref oldUserStoryNode, ref userStoryParameters);
            if (result != XmlResult.OK)
            {
                return result;
            }

            int oldUSNameIndex = userStoryNameList.IndexOf(oldValue);
            if (oldUSNameIndex != -1)
            {
                userStoryNameList.RemoveAt(oldUSNameIndex);
                userStoryNameList.Insert(oldUSNameIndex, newValue);
            }

            return changeAllTaskUserStoryName(ref oldValue, ref newValue);
        }

        public XmlResult changeUserStoryDescription(string oldValue, string newValue, string[] userStoryParameters)
        {
            if (userStoryParameters.Length != 3)
            {
                return XmlResult.NOT_OK;
            }

            XmlNode oldUserStoryNode = createUserStoryNode(ref userStoryParameters[0], ref oldValue, ref userStoryParameters[2]);
            return changeUserStory(ref oldUserStoryNode, ref userStoryParameters);
        }

        public XmlResult changeUserStoryValue(string oldValue, string newValue, string[] userStoryParameters)
        {
            if (userStoryParameters.Length != 3)
            {
                return XmlResult.NOT_OK;
            }

            XmlNode oldUserStoryNode = createUserStoryNode(ref userStoryParameters[0], ref userStoryParameters[1], ref oldValue);
            return changeUserStory(ref oldUserStoryNode, ref userStoryParameters);
        }

        private XmlResult getUserStoryByName(ref XmlNode userStoryNode)
        {
            foreach (XmlNode usNode in backlogNode.ChildNodes)
            {
                if (!usNode.Name.Equals(userStoryNodeName))
                {
                    continue;
                }

                foreach (XmlNode usParameter in usNode.ChildNodes)
                {
                    if (!usParameter.Name.Equals(userStoryNameTag))
                    {
                        continue;
                    }

                    if (usParameter.InnerText.Equals(userStoryNode))
                    {
                        userStoryNode = usNode;
                        return XmlResult.OK;
                    }
                }
            }

            return XmlResult.ITEM_NOT_FOUND;
        }

        public XmlResult moveUserStory(ref string userStoryNameToMove, 
                                       ref string userStoryNamePointer, 
                                       ref string directionToMove)
        {
            XmlNode usToPaste = null;
            if (userStoryNameToMove.Equals("AS_FIRST_USER_STORY"))
            {
                XmlResult result = getUserStoryByName(ref usToPaste);
                if (result == XmlResult.OK)
                {
                    XmlNode tmpUsToPaste = xmlDoc.CreateElement(userStoryNodeName);
                    tmpUsToPaste.InnerXml = usToPaste.InnerXml;
                    usToPaste.ParentNode.RemoveChild(usToPaste);
                    backlogNode.InsertBefore(tmpUsToPaste, scrumBoardNode.FirstChild);
                }

                return result;
            }

            XmlNode usPointer = null;
            foreach (XmlNode usNode in backlogNode)
            {
                if (!usNode.Name.Equals(userStoryNodeName))
                {
                    continue;
                }

                foreach (XmlNode usParameter in usNode.ChildNodes)
                {
                    if (!usParameter.Name.Equals(userStoryNameTag))
                    {
                        continue;
                    }

                    if (usParameter.InnerText.Equals(userStoryNameToMove))
                    {
                        usToPaste = usNode;
                    }
                    else if (usParameter.InnerText.Equals(userStoryNamePointer))
                    {
                        usPointer = usNode;
                    }
                }

                if (usToPaste == null || usPointer == null)
                {
                    continue;
                }

                XmlNode userStoryToPaste = xmlDoc.CreateElement(userStoryNodeName);
                userStoryToPaste.InnerXml = usToPaste.InnerXml;
                usToPaste.ParentNode.RemoveChild(usToPaste);
                if (directionToMove.Equals("AFTER"))
                {
                    backlogNode.InsertAfter(userStoryToPaste, usPointer);
                }
                else
                {
                    backlogNode.InsertBefore(userStoryToPaste, usPointer);
                }
                return XmlResult.OK;
            }

            return XmlResult.ITEM_NOT_FOUND;
        }
    }
}
