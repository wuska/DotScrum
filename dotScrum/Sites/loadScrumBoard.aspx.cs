using dotScrum.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace dotScrum.Sites
{
    public partial class LoadScrumBoard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.enableMenuItems();

            xmlDataSource.DataFile = DotScrumMasterPage.XmlProject.getCurrentScrumBoardFilePath();

            if (!IsPostBack)
            {
                scrumBoardColumnRepeater.DataBind();
            }
        }

        protected void newColumnNameAddButton_Click(object sender, EventArgs e)
        {
            if (!isColumnNameValid())
            {
                return;
            }

            string newColumnName = newColumnNameTextBox.Text;
            XmlResult result = DotScrumMasterPage.XmlProject.addColumn(newColumnName);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not add column: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            result = DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not merge scrum board with project: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not save current project to file: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            clearAndHideNewColumnPopupPanel();

            Response.Redirect("./loadScrumBoard.aspx");
        }

        protected void newColumnNameCancelButton_Click(object sender, EventArgs e)
        {
            clearAndHideNewColumnPopupPanel();
        }

        protected void newUserStoryAddButton_Click(object sender, EventArgs e)
        {
            if (!isNewUserStoryNameValid() ||
                !isNewUserStoryDescriptionValid() ||
                !isNewUserStoryValueValid())
            {
                return;
            }

            XmlResult result = DotScrumMasterPage.XmlProject.addUserStory(newUserStoryNameTextBox.Text,
                                                                newUserStoryDescriptionTextBox.Text,
                                                                newUserStoryValueTextBox.Text);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not add new user story: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not save current project to file: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            clearAndHideNewUserStoryPopupPanel();
            Response.Redirect("./loadScrumBoard.aspx");
        }

        protected void newUserStoryCancelButton_Click(object sender, EventArgs e)
        {
            clearAndHideNewUserStoryPopupPanel();
        }

        protected void newTaskAddButton_Click(object sender, EventArgs e)
        {
            if (!isTaskTitleValid() ||
                !isTaskDescriptionValid() ||
                !isTaskPriorityValid() ||
                !isTaskValueValid())
            {
                return;
            }

            XmlResult result = DotScrumMasterPage.XmlProject.addTask(newTaskTitleTextBox.Text,
                                                                     newTaskDescriptionTextBox.Text,
                                                                     newTaskPriorityTextBox.Text,
                                                                     newTaskValueTextBox.Text,
                                                                     newTaskUserStoryNameList.Text);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not add task: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            result = DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not merge scrum board with project: " + XmlHandler.translateErrorCode(ref result));
                return;
            }
            
            result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not save project to file: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            clearAndHideNewTaskPopupPanel();
            Response.Redirect("./loadScrumBoard.aspx");
        }

        protected void newTaskCancelButton_Click(object sender, EventArgs e)
        {
            clearAndHideNewTaskPopupPanel();
        }

        private bool isTaskValueValid()
        {
            int taskValue;
            if (int.TryParse(newTaskValueTextBox.Text, out taskValue))
            {
                return true;
            }

            Master.displayMessage("Task value is not valid");
            return false;
        }

        private bool isTaskPriorityValid()
        {
            int taskPriority;
            if (int.TryParse(newTaskPriorityTextBox.Text, out taskPriority))
            {
                return true;
            }

            Master.displayMessage("Task priority is not valid");
            return false;
        }

        private bool doesStringContainsForbiddenChars(string str)
        {
            if (newTaskDescriptionTextBox.Text.Contains(';') ||
                newTaskDescriptionTextBox.Text.Contains('<') ||
                newTaskDescriptionTextBox.Text.Contains('>'))
            {
                return true;
            }

            return false;
        }

        private bool isTaskDescriptionValid()
        {
            if (doesStringContainsForbiddenChars(newTaskDescriptionTextBox.Text))
            {
                Master.displayMessage("There are forbidden chars in the task description");
                return false;
            }

            return true;
        }

        private bool isNameValid(TextBox textBox, string name)
        {
            if (textBox.Text.Equals(""))
            {
                Master.displayMessage(name + " name must not be empty");
                return false;
            }

            if (doesStringContainsForbiddenChars(textBox.Text))
            {
                Master.displayMessage("There are forbidden chars in " + name.ToLower() + " name");
                return false;
            }

            return true;
        }

        private bool isColumnNameValid()
        {
            return isNameValid(newColumnNameTextBox, "Column");
        }

        private bool isNewUserStoryNameValid()
        {
            return isNameValid(newUserStoryNameTextBox, "User story");
        }

        private bool isNewUserStoryDescriptionValid()
        {
            if (doesStringContainsForbiddenChars(newUserStoryDescriptionTextBox.Text))
            {
                Master.displayMessage("User story description must not contain forbidden characters");
                return false;
            }

            return true;
        }

        private bool isNewUserStoryValueValid()
        {
            int userStoryValue;
            if (int.TryParse(newUserStoryValueTextBox.Text, out userStoryValue))
            {
                return true;
            }

            Master.displayMessage("Task value is not valid");
            return false;
        }

        private bool isTaskTitleValid()
        {
            return isNameValid(newTaskTitleTextBox, "Task title");
        }

        private void clearAndHideNewColumnPopupPanel()
        {
            newColumnNameTextBox.Text = "";
            newColumnPopupPanel.Visible = false;
        }

        protected void projectMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            switch (e.Item.Value)
            {
                case "addTask":
                    addTaskMenuItemClicked();
                    break;
                case "addColumn":
                    addColumnMenuItemClicked();
                    break;
                case "addUserStory":
                    addUserStoryItemClicked();
                    break;
            }
        }

        private void addUserStoryItemClicked()
        {
            newUserStoryNameTextBox.Text = "";
            newUserStoryDescriptionTextBox.Text = "";
            newUserStoryValueTextBox.Text = "";
            newUserStoryPopupPanel.Visible = true;
        }

        private void addTaskMenuItemClicked()
        {
            newTaskTitleTextBox.Text = "";
            newTaskDescriptionTextBox.Text = "";
            newTaskPriorityTextBox.Text = "";
            newTaskValueTextBox.Text = "";

            newTaskTitleTextBox.Focus();
            List<string> userStories = DotScrumMasterPage.XmlProject.getUserStoryNames();
            newTaskUserStoryNameList.DataSource = userStories;
            newTaskUserStoryNameList.DataBind();
            newTaskPopupPanel.Visible = true;
        }

        private void addColumnMenuItemClicked()
        {
            newColumnNameTextBox.Text = "";
            newColumnNameTextBox.Focus();
            newColumnPopupPanel.Visible = true;
        }

        private void clearAndHideNewUserStoryPopupPanel()
        {
            newUserStoryNameTextBox.Text = "";
            newUserStoryDescriptionTextBox.Text = "";
            newUserStoryValueTextBox.Text = "";
            newUserStoryPopupPanel.Visible = false;
        }

        private void clearAndHideNewTaskPopupPanel()
        {
            newTaskTitleTextBox.Text = "";
            newTaskDescriptionTextBox.Text = "";
            newTaskPriorityTextBox.Text = "";
            newTaskValueTextBox.Text = "";
            newTaskPopupPanel.Visible = false;
        }

        protected void columnTitleTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox columnNameTextBox = (TextBox)sender;
            string oldValue = columnNameTextBox.Attributes[DotScrumMasterPage.oldValueAttribute];
            
            XmlResult result = DotScrumMasterPage.XmlProject.changeColumnName(oldValue, columnNameTextBox.Text);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not change column name: " + XmlHandler.translateErrorCode(ref result));
                return;
            }
            
            result = DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not merge scrum board with project: " + XmlHandler.translateErrorCode(ref result));
                return;
            }
            
            result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not save project to file: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            Response.Redirect("./loadScrumBoard.aspx");
        }

        protected void columnTitleTextBox_DataBinding(object sender, EventArgs e)
        {
            TextBox columnTitleTextBox = (TextBox)sender;
            columnTitleTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, columnTitleTextBox.Text);

            fillTaskList(ref columnTitleTextBox);
        }

        private void fillTaskList(ref TextBox columnTextBox)
        {
            ListView listView = findListView(ref columnTextBox);
            if (listView == null)
            {
                Master.displayMessage("Could not find task list view control");
                return;
            }

            DataSet tasks;
            XmlResult result = DotScrumMasterPage.XmlProject.getColumnAsDataSet(columnTextBox.Text, out tasks);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not obtain tasks data set: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            listView.DataSource = tasks;
            listView.DataBind();
        }

        private ListView findListView(ref TextBox columnTextBox)
        {
            string taskListId = "columnTaskList";
            ListView taskListView = null;
            bool thisIteration = false;

            foreach (Control control in columnTextBox.Parent.Controls)
            {
                if (control.ID == null)
                {
                    continue;
                }

                if (control.Equals(columnTextBox))
                {
                    thisIteration = true;
                }

                if (control.ID.Equals(taskListId) &&
                    thisIteration.Equals(true))
                {
                    taskListView = (ListView)control;
                    break;
                }
            }

            return taskListView;
        }

        protected void deleteTaskButton_Click(object sender, EventArgs e)
        {
            string deleteDecision = Request.Form["decision"];
            if (deleteDecision == null)
            {
                Master.displayMessage("Decision is not set");
                return;
            }

            if (deleteDecision.Equals("No"))
            {
                return;
            }

            Button taskCloseButton = (Button)sender;
            const string taskPanelId = "taskPanel";
            if (!taskCloseButton.Parent.ID.Equals(taskPanelId))
            {
                Master.displayMessage("Failed to delete the task - no parent panel found. Parent id: " + taskCloseButton.Parent.ID);
                return;
            }

            String taskTitle, taskPriority, taskDescription, taskValue, columnName, userStoryName;
            Panel taskPanel = (Panel)taskCloseButton.Parent;
            getTaskParameters(out taskTitle, out taskPriority, out taskDescription, out taskValue,out userStoryName, 
                              ref taskPanel);
            getColumnNameFromTaskPanel(ref taskPanel, out columnName);
            if (taskTitle == null ||
                taskPriority == null ||
                taskDescription == null ||
                taskValue == null ||
                columnName == null)
            {
                return;
            }

            XmlResult result = DotScrumMasterPage.XmlProject.deleteTask(taskTitle, taskDescription, taskPriority, taskValue, 
                                                                        userStoryName, columnName);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Failed to delete the task: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
            DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();

            Response.Redirect("./loadScrumBoard.aspx");
        }

        private void getColumnNameFromTaskPanel(ref Panel taskPanel, out string columnName)
        {
            Control control = taskPanel.Parent;
            while (control != null)
            {
                if (control.ID != null && control.ID.Equals("taskColumn"))
                {
                    break;
                }

                control = control.Parent;
            }

            if (control != null)
            {
                control = control.FindControl("columnTitleTextBox");
                if (control != null)
                {
                    columnName = ((TextBox)control).Text;
                    return;
                }
            }

            Master.displayMessage("Could not find column of tasks in page structure");
            columnName = null;
        }

        private void getTaskParameters(out string title, out string priority, out string description, out string value, 
                                       out string userStoryName, ref Panel taskPanel)
        {
            Control taskTitle = taskPanel.FindControl("taskTitleTextBox");
            title = taskTitle == null ? null : ((TextBox)taskTitle).Text;

            Control taskPriority = taskPanel.FindControl("taskPriorityTextBox");
            priority = taskPriority == null ? null : ((TextBox)taskPriority).Text;

            Control taskDescription = taskPanel.FindControl("taskDescriptionTextBox");
            description = taskDescription == null ? null : ((TextBox)taskDescription).Text;

            Control taskValue = taskPanel.FindControl("taskValueTextBox");
            value = taskValue == null ? null : ((TextBox)taskValue).Text;

            Control taskUserStoryName = taskPanel.FindControl("taskUserStoryNameList");
            userStoryName = taskUserStoryName == null ? null : ((DropDownList)taskUserStoryName).Text;
        }

        protected void taskColumnsDeleteButton_Click(object sender, EventArgs e)
        {
            string deleteDecision = Request.Form["decision"];
            if (deleteDecision.Equals("No"))
            {
                return;
            }

            Button deleteColumnButton = (Button)sender;
            string columnName;
            getColumnNameFromDeleteColumnButton(ref deleteColumnButton, out columnName);
            if (columnName == null)
            {
                return;
            }

            XmlResult result = DotScrumMasterPage.XmlProject.deleteColumnByName(ref columnName);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not delete column: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            result = DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not merge scrum board with project: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not save project to file: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            Response.Redirect("./loadScrumBoard.aspx");
        }

        private void getColumnNameFromDeleteColumnButton(ref Button deleteColumnButton, out string columnName)
        {
            Control control = deleteColumnButton.Parent;
            while (control != null)
            {
                if (control.ID != null && control.ID.Equals("taskColumn"))
                {
                    break;
                }

                control = control.Parent;
            }

            if (control != null)
            {
                control = control.FindControl("columnTitleTextBox");
                if (control != null)
                {
                    columnName = ((TextBox)control).Text;
                    return;
                }
            }

            Master.displayMessage("Could not find column of tasks in page structure");
            columnName = null;
        }

        private void obtainColumnAndTaskIds(ref string bothIds, ref string taskId, ref string columnId)
        {
            string[] taskAndColumn = bothIds.Split(',');
            if (taskAndColumn.Length <= 1)
            {
                Master.displayMessage("Could not distinguish task and column id");
                return;
            }

            taskId = taskAndColumn[0];
            columnId = taskAndColumn[1];
        }

        private Control getRecursiveControlByClientId(Control baseControl, ref string clientId)
        {
            if (baseControl == null)
            {
                return null;
            }

            if (baseControl.ClientID.Equals(clientId))
            {
                return baseControl;
            }

            if (baseControl.Controls.Count <= 0)
            {
                return null;
            }

            Control tmpControl;
            foreach (Control control in baseControl.Controls)
            {
                tmpControl = getRecursiveControlByClientId(control, ref clientId);
                if (tmpControl == null)
                {
                    continue;
                }

                return tmpControl;
            }

            return null;
        }

        private Control getTaskBlock(int columnId, int taskId)
        {
            string idPart = "bodyContent_scrumBoardColumnRepeater_columnTaskList_" + 
                            columnId.ToString() + "_taskBlock_" + taskId.ToString();
            return getRecursiveControlByClientId(scrumBoardColumnRepeater, ref idPart);
        }

        private void obtainTaskParameters(int columnId, int taskId, ref string taskTitle, ref string taskDescription, 
                                          ref string taskPriority, ref string taskValue, ref string taskUserStoryName)
        {
            Control taskElementDiv = getTaskBlock(columnId, taskId);
            if (taskElementDiv == null)
            {
                Master.displayMessage("Could not find task element: '" + taskId + "'");
                return;
            }

            Control taskTitleTextBox = taskElementDiv.FindControl("taskTitleTextBox");
            if (taskTitleTextBox == null)
            {
                Master.displayMessage("Could not find task title");
                return;
            }
            taskTitle = ((TextBox)taskTitleTextBox).Text;

            Control taskPriorityTextBox = taskElementDiv.FindControl("taskPriorityTextBox");
            if (taskPriorityTextBox == null)
            {
                Master.displayMessage("Could not find task priority");
                return;
            }
            taskPriority = ((TextBox)taskPriorityTextBox).Text;

            Control taskDescriptionTextBox = taskElementDiv.FindControl("taskDescriptionTextBox");
            if (taskDescriptionTextBox == null)
            {
                Master.displayMessage("Could not find task description");
                return;
            }
            taskDescription = ((TextBox)taskDescriptionTextBox).Text;

            Control taskValueTextBox = taskElementDiv.FindControl("taskValueTextBox");
            if (taskValueTextBox == null)
            {
                Master.displayMessage("Could not find task value");
                return;
            }
            taskValue = ((TextBox)taskValueTextBox).Text;

            Control taskUserStoryNameList = taskElementDiv.FindControl("taskUserStoryNameList");
            if (taskUserStoryNameList == null)
            {
                Master.displayMessage("Could not find user story name in task relation");
                return;
            }
            taskUserStoryName = ((DropDownList)taskUserStoryNameList).Text;
        }

        private int obtainLastNumber(string text)
        {
            string[] tmp = text.Split('_');
            if (tmp.Length <= 0)
            {
                return XmlHandler.NOT_VALID;
            }

            int taskNumber;
            if (int.TryParse(tmp.Last(), out taskNumber) == false)
            {
                return XmlHandler.NOT_VALID;
            }

            return taskNumber;
        }

        private int obtainColumnIndex(string columnId)
        {
            return obtainLastNumber(columnId);
        }

        private int obtainTaskIndex(string taskId)
        {
            return obtainLastNumber(taskId);
        }

        private int getCurrentColumnIndex(ref string fullElementId)
        {
            string[] idParts = fullElementId.Split('_');
            int result = XmlHandler.NOT_VALID;
            List<int> numbers = new List<int>();
            foreach (string part in idParts)
            {
                if (int.TryParse(part, out result))
                {
                    numbers.Add(result);
                }
            }

            if (numbers.Count > 0)
            {
                return numbers.First();
            }

            return XmlHandler.NOT_VALID;
        }

        private void moveTask(ref string taskId, ref string columnId)
        {
            string taskTitle = "";
            string taskDescription = "";
            string taskPriority = "";
            string taskValue = "";
            string taskUserStoryName = "";
            int columnIndex = obtainColumnIndex(columnId);
            int taskIndex = obtainTaskIndex(taskId);
            if (columnIndex == XmlHandler.NOT_VALID || taskIndex == XmlHandler.NOT_VALID)
            {
                return;
            }

            int currColIndex = getCurrentColumnIndex(ref taskId);
            obtainTaskParameters(currColIndex, taskIndex, ref taskTitle, ref taskDescription, ref taskPriority, ref taskValue,
                                 ref taskUserStoryName);
            if (taskTitle.Equals("") || taskDescription.Equals("") || taskPriority.Equals("") || taskValue.Equals("") ||
                taskUserStoryName.Equals(""))
            {
                return;
            }

            XmlResult result = DotScrumMasterPage.XmlProject.moveTask(taskTitle, taskDescription, taskPriority,
                                                                      taskValue, taskUserStoryName, columnIndex);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not move task to another column: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
            DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            Response.Redirect("./loadScrumBoard.aspx");
        }

        protected void DragAndDropTrigger_Click(object sender, EventArgs e)
        {
            string taskElementId = Request.Form["taskElement"];
            if (taskElementId == null)
            {
                Master.displayMessage("Could not obtain task and column information");
                return;
            }

            string taskId = "";
            string columnId = "";
            obtainColumnAndTaskIds(ref taskElementId, ref taskId, ref columnId);
            if (taskId.Equals("") || columnId.Equals(""))
            {
                return;
            }

            moveTask(ref taskId, ref columnId);
        }

        private Control findTaskPanelInPredecessor(Control parentControl)
        {
            while (parentControl != null)
            {
                if (parentControl.ClientID == null)
                {
                    continue;
                }

                if (parentControl.ID.Equals("taskPanel"))
                {
                    return parentControl;
                }

                parentControl = parentControl.Parent;
            }

            return null;
        }

        private Control findRecursivelyByClientId(Control control, string clientId)
        {
            if (control == null || control.ClientID == null)
            {
                return null;
            }

            if (control.ClientID.Contains(clientId))
            {
                return control;
            }

            Control tmpControl;
            foreach (Control ctrl in control.Controls)
            {
                tmpControl = findRecursivelyByClientId(ctrl, clientId);
                if (tmpControl != null)
                {
                    return tmpControl;
                }
            }

            return null;
        }

        private XmlTaskParameter getChangedParameter(ref TextBox title, ref TextBox priority, ref TextBox description, 
                                                     ref TextBox value, ref DropDownList userStoryNameList, 
                                                     ref TextBox userStoryName)
        {
            XmlTaskParameter parameter = XmlTaskParameter.NONE;
            if (!title.Attributes[DotScrumMasterPage.oldValueAttribute].Equals(title.Text))
            {
                parameter = XmlTaskParameter.TITLE;
            }
            else if (!priority.Attributes[DotScrumMasterPage.oldValueAttribute].Equals(priority.Text))
            {
                parameter = XmlTaskParameter.PRIORITY;
            }
            else if (!description.Attributes[DotScrumMasterPage.oldValueAttribute].Equals(description.Text))
            {
                parameter = XmlTaskParameter.DESCRIPTION;
            }
            else if (!value.Attributes[DotScrumMasterPage.oldValueAttribute].Equals(value.Text))
            {
                parameter = XmlTaskParameter.VALUE;
            }
            else if (!userStoryName.Text.Equals(userStoryNameList.Text))
            {
                parameter = XmlTaskParameter.USER_STORY_NAME;
            }

            return parameter;
        }

        private void updateTaskParameter(ref TextBox title, ref TextBox priority, ref TextBox description, ref TextBox value,
                                         ref DropDownList userStoryNameList, ref TextBox userStoryName)
        {
            XmlTaskParameter parameter = getChangedParameter(ref title, ref priority, ref description, ref value,
                                                             ref userStoryNameList, ref userStoryName);
            switch (parameter)
            {
                case XmlTaskParameter.TITLE:
                    DotScrumMasterPage.XmlProject.changeTaskTitle(title.Attributes[DotScrumMasterPage.oldValueAttribute],
                                                                  title.Text, description.Text, priority.Text, value.Text,
                                                                  userStoryNameList.Text);
                    break;
                case XmlTaskParameter.DESCRIPTION:
                    DotScrumMasterPage.XmlProject.changeTaskDescription(description.Attributes[DotScrumMasterPage.oldValueAttribute],
                                                                        description.Text, title.Text, priority.Text, value.Text,
                                                                        userStoryNameList.Text);
                    break;
                case XmlTaskParameter.PRIORITY:
                    DotScrumMasterPage.XmlProject.changeTaskPriority(priority.Attributes[DotScrumMasterPage.oldValueAttribute],
                                                                     priority.Text, title.Text, description.Text, value.Text,
                                                                     userStoryNameList.Text);
                    break;
                case XmlTaskParameter.VALUE:
                    DotScrumMasterPage.XmlProject.changeTaskValue(value.Attributes[DotScrumMasterPage.oldValueAttribute],
                                                                  value.Text, title.Text, description.Text, priority.Text,
                                                                  userStoryNameList.Text);
                    break;
                case XmlTaskParameter.USER_STORY_NAME:
                    DotScrumMasterPage.XmlProject.changeTaskUserStoryName(userStoryName.Text, userStoryNameList.Text, 
                                                                          title.Text, description.Text, priority.Text, 
                                                                          value.Text);
                    break;
                case XmlTaskParameter.NONE:
                default:
                    return;
            }
        }

        private void updateTask(Control control)
        {
            Control taskPanel = findTaskPanelInPredecessor(control);
            if (taskPanel == null)
            {
                Master.displayMessage("Could not find task panel");
                return;
            }

            TextBox title = (TextBox)taskPanel.FindControl("taskTitleTextBox");
            TextBox priority = (TextBox)taskPanel.FindControl("taskPriorityTextBox");
            TextBox description = (TextBox)taskPanel.FindControl("taskDescriptionTextBox");
            TextBox value = (TextBox)taskPanel.FindControl("taskValueTextBox");
            DropDownList userStoryNameList = (DropDownList)taskPanel.FindControl("taskUserStoryNameList");
            TextBox userStoryTextBox = (TextBox)taskPanel.FindControl("taskUserStoryNameTextBox");
            if (title == null || priority == null || description == null || value == null || userStoryNameList == null ||
                userStoryTextBox == null)
            {
                Master.displayMessage("Could not find one of the task parameters");
                return;
            }

            updateTaskParameter(ref title, ref priority, ref description, ref value, ref userStoryNameList, ref userStoryTextBox);

            DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
            DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            Response.Redirect("./loadScrumBoard.aspx");
        }

        protected void taskPriorityTextBox_TextChanged(object sender, EventArgs e)
        {
            updateTask((TextBox)sender);
        }

        protected void taskTitleTextBox_TextChanged(object sender, EventArgs e)
        {
            updateTask((TextBox)sender);
        }

        protected void taskDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            updateTask((TextBox)sender);
        }

        protected void taskValueTextBox_TextChanged(object sender, EventArgs e)
        {
            updateTask((TextBox)sender);
        }

        protected void taskPriorityTextBox_DataBinding(object sender, EventArgs e)
        {
            TextBox priorityTextBox = (TextBox)sender;
            priorityTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, priorityTextBox.Text);
        }

        protected void taskTitleTextBox_DataBinding(object sender, EventArgs e)
        {
            TextBox titleTextBox = (TextBox)sender;
            titleTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, titleTextBox.Text);
        }

        protected void taskDescriptionTextBox_DataBinding(object sender, EventArgs e)
        {
            TextBox descriptionTextBox = (TextBox)sender;
            descriptionTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, descriptionTextBox.Text);
        }

        protected void taskValueTextBox_DataBinding(object sender, EventArgs e)
        {
            TextBox valueTextBox = (TextBox)sender;
            valueTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, valueTextBox.Text);
        }

        protected void taskUserStoryNameList_Init(object sender, EventArgs e)
        {
            DropDownList userStoryNames = (DropDownList)sender;
            userStoryNames.DataSource = DotScrumMasterPage.XmlProject.getUserStoryNames();
            userStoryNames.DataBind();
        }

        protected void taskUserStoryNameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateTask((DropDownList)sender);
        }

        protected void taskUserStoryNameList_DataBinding(object sender, EventArgs e)
        {
            DropDownList userStoryNames = (DropDownList)sender;
            TextBox userNameTextBox = (TextBox)userStoryNames.Parent.FindControl("taskUserStoryNameTextBox");
            if (userNameTextBox == null)
            {
                Master.displayMessage("Culd not find user story for task");
                return;
            }

            string userStoryName = (userNameTextBox.Text);
            foreach (ListItem item in userStoryNames.Items)
            {
                if (item.Text.Equals(userStoryName))
                {
                    item.Selected = true;
                    userStoryNames.SelectedValue = item.Value;
                    return;
                }
            }
        }
    }
}