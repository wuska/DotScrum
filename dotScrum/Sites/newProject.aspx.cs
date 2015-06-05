using dotScrum.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dotScrum.Sites
{
    public partial class NewProject : System.Web.UI.Page
    {
        private static string newProjectName = "New project name";
        private static string toDoColumnHeader = "To do";
        private static string ongoingColumnHeader = "Ongoing";
        private static string doneColumnHeader = "Done";

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.enableMenuItems();

            if (!Page.IsPostBack)
            {
                XmlResult result = DotScrumMasterPage.XmlProject.resetProjectFile(newProjectName);
                if (result != XmlResult.OK)
                {
                    Master.displayMessage("Could not reset project file: " + XmlHandler.translateErrorCode(ref result));
                    return;
                }

                result = DotScrumMasterPage.XmlProject.addSprint();
                if (result != XmlResult.OK)
                {
                    Master.displayMessage("Could not add sprint: " + XmlHandler.translateErrorCode(ref result));
                    return;
                }

                result = DotScrumMasterPage.XmlProject.selectSprintScrumBoard(XmlHandler.LAST_SPRINT);
                if (result != XmlResult.OK)
                {
                    Master.displayMessage("Could not use sprint scrum board: " + XmlHandler.translateErrorCode(ref result));
                    return;
                }

                result = addDefaultColumns();
                if (result != XmlResult.OK)
                {
                    Master.displayMessage("Could not add default columns: " + XmlHandler.translateErrorCode(ref result));
                    return;
                }

                result = DotScrumMasterPage.XmlProject.joinScrumBoardToProject();
                if (result != XmlResult.OK)
                {
                    Master.displayMessage("Could not join scrum board with project: " + XmlHandler.translateErrorCode(ref result));
                    return;
                }

                result = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
                if (result != XmlResult.OK)
                {
                    Master.displayMessage("Could not save current project to file: " + XmlHandler.translateErrorCode(ref result));
                    return;
                }
            }

            Response.Redirect("../default.aspx");
        }

        private XmlResult addDefaultColumns()
        {
            XmlResult result = addColumn(toDoColumnHeader);
            if (result != XmlResult.OK && result != XmlResult.ITEM_ALREADY_EXISTS)
            {
                return result;
            }

            result = addColumn(ongoingColumnHeader);
            if (result != XmlResult.OK && result != XmlResult.ITEM_ALREADY_EXISTS)
            {
                return result;
            }

            result = addColumn(doneColumnHeader);
            if (result != XmlResult.OK && result != XmlResult.ITEM_ALREADY_EXISTS)
            {
                return result;
            }

            return XmlResult.OK;
        }

        private XmlResult addColumn(string columnName)
        {
            XmlResult result = DotScrumMasterPage.XmlProject.addColumn(columnName);
            if (result != XmlResult.OK)
            {
                Master.displayMessage("Could not add column '" + columnName + "': " + XmlHandler.translateErrorCode(ref result));
            }

            return result;
        }

        protected void projectNameTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox newProjectNameTextBox = (TextBox)sender;
            XmlResult res = DotScrumMasterPage.XmlProject.changeProjectName(newProjectNameTextBox.Text);
            if (res != XmlResult.OK)
            {
                Master.displayMessage("Could not change project name: " + XmlHandler.translateErrorCode(ref res));
                Response.Redirect("default.aspx");
            }

            res = DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            if (res != XmlResult.OK)
            {
                Master.displayMessage("Could not save project to file: " + XmlHandler.translateErrorCode(ref res));
                Response.Redirect("default.aspx");
            }
            Response.Redirect("../default.aspx");
        }

        protected void projectMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            switch (e.Item.Value)
            {
                case "addUserStory":
                    addNewUserStory();
                    break;
            }
        }

        private void addNewUserStory()
        {
            DotScrumMasterPage.XmlProject.addUserStory("New user story name", "New description", "0");
            DotScrumMasterPage.XmlProject.saveCurrentProjectToXmlFile();
            Response.Redirect("../default.aspx");
        }
    }
}