using dotScrum.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dotScrum
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true;

            if (IsPostBack)
            {
                return;
            }

            initializeXmlHandlerIfNeeded();
            fillProjectName();
            fillUserStories();
        }

        private void initializeXmlHandlerIfNeeded()
        {
            if (DotScrumMasterPage.XmlProject == null)
            {
                string xmlProjectFilePath = HttpContext.Current.Server.MapPath("~/Xml/currentProject.xml");
                DotScrumMasterPage.XmlProject = new XmlHandler(ref xmlProjectFilePath);
            }
        }

        private void fillProjectName()
        {
            string projectName = "";
            XmlResult res = DotScrumMasterPage.XmlProject.getProjectName(ref projectName);
            if (res != XmlResult.OK)
            {
                Master.displayMessage("Could not obtain project name: " + XmlHandler.translateErrorCode(ref res));
            }
            projectNameTextBox.Text = projectName;
        }

        private void fillNotDoneUserStories(ref DataSet notDoneUserStories)
        {
            backlogList.DataSource = notDoneUserStories;
            backlogList.DataBind();
        }

        private void fillDoneUserStories(ref DataSet doneUserStories)
        {
            doneBacklogList.DataSource = doneUserStories;
            doneBacklogList.DataBind();
        }

        private void fillUserStories()
        {
            DataSet doneUserStories;
            DataSet notDoneUserStories;
            XmlResult result = DotScrumMasterPage.XmlProject.getDoneAndNotDoneUserStoriesAsDataSets(out doneUserStories,
                                                                                                    out notDoneUserStories);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not obtain user stories data set: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            fillNotDoneUserStories(ref notDoneUserStories);
            fillDoneUserStories(ref doneUserStories);
        }

        protected void projectNameTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox newProjectNameTextBox = (TextBox)sender;
            XmlResult res = DotScrumMasterPage.XmlProject.changeProjectName(newProjectNameTextBox.Text);
            saveFileIfNeededAndRedirect(ref res, "Could not change project name");
        }

        protected void projectMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            switch (e.Item.Value)
            {
                case "addUserStory":
                    addNewUserStory();
                    break;
                case "addSprint":
                    addNewSprint();
                    break;
            }
        }

        private void addNewUserStory()
        {
            XmlResult result = DotScrumMasterPage.XmlProject.addUserStory("New user story name", "New description", "0");
            saveFileIfNeededAndRedirect(ref result, "Could not add new user story");
        }

        private void addNewSprint()
        {
            XmlResult result = DotScrumMasterPage.XmlProject.addSprint();
            saveFileIfNeededAndRedirect(ref result, "Could not add new sprint");
        }

        private string[] getUserStoryParameters(Control parentControl)
        {
            const string backlogItemBlockName = "backlogItemBlock";
            if (parentControl == null || !parentControl.ID.Equals(backlogItemBlockName))
            {
                return null;
            }

            string[] userStoryParameters = new string[3];
            const string userStoryTitleBlockName = "userStoryTitleTextBox";
            const string userStoryDescriptionBlockName = "userStoryDescription";
            const string userStoryValueBlockName = "userStoryValue";
            foreach (Control userStoryChildControl in parentControl.Controls)
            {
                if (userStoryChildControl.ID == null)
                {
                    continue;
                }

                if (userStoryChildControl.ID.Equals(userStoryTitleBlockName))
                {
                    userStoryParameters[0] = ((TextBox)userStoryChildControl).Text;
                } else if (userStoryChildControl.ID.Equals(userStoryDescriptionBlockName)) {
                    userStoryParameters[1] = ((TextBox)userStoryChildControl).Text;
                } else if (userStoryChildControl.ID.Equals(userStoryValueBlockName)) {
                    userStoryParameters[2] = ((TextBox)userStoryChildControl).Text;
                }
            }

            return userStoryParameters;
        }

        private void saveFileIfNeededAndRedirect(ref XmlResult result, string message)
        {
            if (result != XmlResult.OK)
            {
                Master.displayMessage(message + ": " + XmlHandler.translateErrorCode(ref result));

                result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            }
            else
            {
                result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
                if (result != XmlResult.OK)
                {
                    Master.displayMessage("Could not save project to file: " + XmlHandler.translateErrorCode(ref result));
                }
            }

            Response.Redirect("./default.aspx");
        }

        protected void userStoryTitleTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox userStoryTitleTextBox = (TextBox)sender;
            string[] userStoryParameters = getUserStoryParameters(userStoryTitleTextBox.Parent);
            XmlResult res = DotScrumMasterPage.XmlProject.changeUserStoryName(
                userStoryTitleTextBox.Attributes[DotScrumMasterPage.oldValueAttribute], 
                userStoryTitleTextBox.Text,
                userStoryParameters);
            saveFileIfNeededAndRedirect(ref res, "Could not change user story title");
        }

        protected void userStoryDescription_TextChanged(object sender, EventArgs e)
        {
            TextBox userStoryDescriptionTextBox = (TextBox)sender;
            string[] userStoryParameters = getUserStoryParameters(userStoryDescriptionTextBox.Parent);
            XmlResult res = DotScrumMasterPage.XmlProject.changeUserStoryDescription(
                userStoryDescriptionTextBox.Attributes[DotScrumMasterPage.oldValueAttribute],
                userStoryDescriptionTextBox.Text,
                userStoryParameters);
            saveFileIfNeededAndRedirect(ref res, "Could not change user story description");
        }

        protected void userStoryValue_TextChanged(object sender, EventArgs e)
        {
            TextBox userStoryValueTextBox = (TextBox)sender;
            string[] userStoryParameters = getUserStoryParameters(userStoryValueTextBox.Parent);
            XmlResult res = DotScrumMasterPage.XmlProject.changeUserStoryValue(
                userStoryValueTextBox.Attributes[DotScrumMasterPage.oldValueAttribute],
                userStoryValueTextBox.Text,
                userStoryParameters);
            saveFileIfNeededAndRedirect(ref res, "Could not change user story value");
        }

        protected void userStoryTitleTextBox_DataBinding(object sender, EventArgs e)
        {
            TextBox userStoryTitleTextBox = (TextBox)sender;
            userStoryTitleTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, userStoryTitleTextBox.Text);
        }

        protected void userStoryDescription_DataBinding(object sender, EventArgs e)
        {
            TextBox userStoryDescriptionTextBox = (TextBox)sender;
            userStoryDescriptionTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, userStoryDescriptionTextBox.Text);
        }

        protected void userStoryValue_DataBinding(object sender, EventArgs e)
        {
            TextBox userStoryValueTextBox = (TextBox)sender;
            userStoryValueTextBox.Attributes.Add(DotScrumMasterPage.oldValueAttribute, userStoryValueTextBox.Text);
        }

        private void deleteUserStory(Button button)
        {
            string[] userStoryParameters = getUserStoryParameters(button.Parent);
            XmlResult result = DotScrumMasterPage.XmlProject.deleteUserStory(userStoryParameters[0],
                                                                             userStoryParameters[1],
                                                                             userStoryParameters[2]);
            saveFileIfNeededAndRedirect(ref result, "Could not delete user story");
        }

        protected void deleteUserStory_Click(object sender, EventArgs e)
        {
            deleteUserStory((Button)sender);
        }

        protected void sprintMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList sprintMenuDropDownList = (DropDownList)sender;
            
        }

        protected void DragAndDropTrigger_Click(object sender, EventArgs e)
        {
            string data = Request.Form["dataToTransfer"];
            if (data == null)
            {
                Master.displayMessage("Could not obtain task and column information");
                return;
            }

            string userStoryNamePointer = null;
            string userStoryNameToMove = null;
            string directionToMove = null;
            obtainMovingUserStoryParameters(ref data, ref userStoryNamePointer, ref userStoryNameToMove, ref directionToMove);
            if (userStoryNamePointer == null || userStoryNameToMove == null || directionToMove == null)
            {
                return;
            }

            moveTask(ref userStoryNameToMove, ref userStoryNamePointer, ref directionToMove);
        }

        private void moveTask(ref string userStoryNameToMove, ref string userStoryNamePointer, ref string directionToMove)
        {
            XmlResult result = DotScrumMasterPage.XmlProject.moveUserStory(ref userStoryNameToMove, 
                                                                           ref userStoryNamePointer, 
                                                                           ref directionToMove);
            saveFileIfNeededAndRedirect(ref result, "Could not move user story");
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

        private string obtainUserStoryNameByUserStoryBlockId(ref string usBlockId)
        {
            Control blockControl = getRecursiveControlByClientId(backlogListPanel, ref usBlockId);
            if (blockControl == null)
            {
                return null;
            }

            foreach (Control usControl in blockControl.Controls)
            {
                if (usControl.ID != null && usControl.ID.Equals("userStoryTitleTextBox"))
                {
                    TextBox usTitleTextBox = (TextBox)usControl;
                    return usTitleTextBox.Text;
                }
            }

            return null;
        }

        private void obtainMovingUserStoryParameters(ref string combinedParameters,
                                                     ref string userStoryNamePointer, 
                                                     ref string userStoryNameToMove, 
                                                     ref string directionToMove)
        {
            string[] parameters = combinedParameters.Split(',');
            if (parameters.Length != 3)
            {
                return;
            }

            userStoryNameToMove = obtainUserStoryNameByUserStoryBlockId(ref parameters[0]);
            userStoryNamePointer = obtainUserStoryNameByUserStoryBlockId(ref parameters[1]);
            directionToMove = parameters[2];
        }

        protected void doneDeleteUserStory_Click(object sender, EventArgs e)
        {
            deleteUserStory((Button)sender);
        }

        protected void BacklogItemMouseOver_Click(object sender, EventArgs e)
        {
            string backlogItemBlockId = Request.Form["mouseOverData"];
            if (backlogItemBlockId == null)
            {
                Master.displayMessage("Could not obtain task and column information");
                return;
            }

            Control backlogItemBlock = getRecursiveControlByClientId(backlogListPanel, ref backlogItemBlockId);
            if (backlogItemBlock == null)
            {
                return;
            }

            Control usDescription = backlogItemBlock.FindControl("userStoryDescription");
            if (usDescription == null)
            {
                return;
            }

            //usDescription.Visible = true;
        }

        protected void BacklogItemMouseOut_Click(object sender, EventArgs e)
        {
            string backlogItemBlockId = Request.Form["mouseOutData"];
            if (backlogItemBlockId == null)
            {
                Master.displayMessage("Could not obtain task and column information");
                return;
            }

            Control backlogItemBlock = getRecursiveControlByClientId(backlogListPanel, ref backlogItemBlockId);
            if (backlogItemBlock == null)
            {
                return;
            }

            Control usDescription = backlogItemBlock.FindControl("userStoryDescription");
            if (usDescription == null)
            {
                return;
            }

            usDescription.Visible = false;
        }
    }
}