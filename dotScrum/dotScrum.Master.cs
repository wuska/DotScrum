using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

using dotScrum.Xml;
using System.IO;

namespace dotScrum
{
    public partial class DotScrumMasterPage : System.Web.UI.MasterPage
    {
        private static XmlHandler xmlProject;
        public static string oldValueAttribute = "oldValue";
        private const string scrumBoardMenuValue = "scrumBoard ";

        public static XmlHandler XmlProject
        {
            get { return DotScrumMasterPage.xmlProject; }
            set { DotScrumMasterPage.xmlProject = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true;

            if (!Page.IsPostBack)
            {
                mainMenu.Orientation = Orientation.Horizontal;
                mainMenu.BackColor = Color.White;
                mainMenu.BorderStyle = BorderStyle.Solid;
                mainMenu.BorderWidth = 1;
                mainMenu.BorderColor = Color.SlateGray;

                mainMenu.DynamicMenuItemStyle.BackColor = Color.White;
                mainMenu.DynamicMenuItemStyle.BorderStyle = BorderStyle.Solid;
                mainMenu.DynamicMenuItemStyle.BorderWidth = 1;
                mainMenu.DynamicMenuItemStyle.BorderColor = Color.LightSlateGray;

                prepareMenusLayout();
            }
        }

        public void displayMessage(string message)
        {
            System.Web.HttpContext.Current.Response.Write("<script LANGUAGE='JavaScript' >alert('" + message + "')</script>");
        }

        public void prepareMenusLayout()
        {
            setSaveAsProjectMenuItemEnabled(true);
            initializeSprintsMenu();
        }

        private void setSaveAsProjectMenuItemEnabled(bool enabled)
        {
            MenuItem saveProjectAsMenuItem = getSaveAsMenuItem();
            if (saveProjectAsMenuItem.Equals(null))
            {
                displayMessage("saveProjectMenuItem is null");
                return;
            }

            saveProjectAsMenuItem.Enabled = enabled;
        }

        private void initializeSprintsMenu()
        {
            foreach (MenuItem menuItem in mainMenu.Items)
            {
                if (!menuItem.Value.Equals("sprints"))
                {
                    continue;
                }

                int lastSprintNumber;
                XmlResult result = DotScrumMasterPage.XmlProject.getLastSprintNumber(out lastSprintNumber);
                if (result != XmlResult.OK)
                {
                    return;
                }

                const string sprintWord = "Sprint ";
                const string scrumBoardWords = "Scrum board";
                for (int sprintNo = 0; sprintNo <= lastSprintNumber; sprintNo++)
                {
                    string sprintItemValue = sprintWord + sprintNo;
                    bool itemExists = false;
                    foreach (MenuItem tmpMenuItem in menuItem.ChildItems)
                    {
                        if (tmpMenuItem.Value.Equals(sprintItemValue))
                        {
                            itemExists = true;
                        }
                    }

                    if (itemExists)
                    {
                        continue;
                    }

                    MenuItem sprintItem = new MenuItem(sprintWord + sprintNo, sprintItemValue);

                    MenuItem scrumBoardItem = new MenuItem(scrumBoardWords, scrumBoardMenuValue + sprintNo);
                    sprintItem.ChildItems.Add(scrumBoardItem);

                    menuItem.ChildItems.Add(sprintItem);
                }

                return;
            }
        }

        private MenuItem getFileMenuItem()
        {
            foreach (MenuItem menuItem in mainMenu.Items)
            {
                if (menuItem.Value.Equals("file"))
                {
                    return menuItem;
                }
            }

            displayMessage("Could not find FileMenuItem");
            return null;
        }

        private MenuItem getSaveAsMenuItem()
        {
            MenuItem fileMenuItem = getFileMenuItem();
            if (fileMenuItem.Equals(null))
            {
                displayMessage("FileMenuItem is null");
                return null;
            }

            string saveProjectAsValue = "saveProjectAs";
            MenuItem saveMenuAsItem = findMenuItemInOtherMenuItem(ref fileMenuItem, ref saveProjectAsValue);

            if (saveMenuAsItem.Equals(null))
            {
                displayMessage("Could not find saveProjectAs in " + fileMenuItem.Value);
            }

            return saveMenuAsItem;
        }

        private MenuItem findMenuItemInOtherMenuItem(ref MenuItem menu, ref string wantedItemValue)
        {
            if (menu.Equals(null))
            {
                displayMessage("menu is null");
                return null;
            }

            foreach (MenuItem menuItem in menu.ChildItems)
            {
                if (menuItem.Value.Equals(wantedItemValue))
                {
                    return menuItem;
                }
            }

            displayMessage("Could not find " + wantedItemValue + " in menu: " + menu.Value);
            return null;
        }

        protected void mainMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            switch (e.Item.Value)
            {
                case "mainPage":
                case "newProject":
                    //redirection is handled in this html pagefile
                    return;
                case "loadProject":
                    loadProjectHandler();
                    return;
                case "saveProjectAs":
                    saveProjectAsHandler();
                    return;
            }

            if (e.Item.Value.Contains(scrumBoardMenuValue))
            {
                handleGoToSprintClicked(e.Item.Value);
            }
        }

        private void handleGoToSprintClicked(string sprintMenuItemValue)
        {
            string[] tmpStr = sprintMenuItemValue.Split(' ');
            if (tmpStr.Length < 2)
            {
                displayMessage("The selected sprint text is not valid");
                return;
            }

            int sprintNumber;
            if (int.TryParse(tmpStr[1], out sprintNumber) == false)
            {
                displayMessage("The selected sprint text has no sprint number in required place");
                return;
            }

            XmlResult result = DotScrumMasterPage.XmlProject.selectSprintScrumBoard(sprintNumber);
            if (result != XmlResult.OK)
            {
                displayMessage("Could not select scrum board: " + XmlHandler.translateErrorCode(ref result));
                return;
            }

            Response.Redirect("/Sites/loadScrumBoard.aspx");
        }

        protected void loadProjectHandler()
        {
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "OpenD", "openDialog();", true);
        }

        protected void saveProjectAsHandler()
        {
            saveXmlFile();
        }

        private void saveXmlFile()
        {
            string[] pathElements = xmlProject.getXmlFilePath().Split('\\');
            if (pathElements.Length == 0)
            {
                return;
            }

            string fileName = pathElements.Last<string>();

            Response.Clear();
            Response.ContentType = "text/xml";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.TransmitFile(xmlProject.getXmlFilePath());
            Response.End();
        }

        public void enableMenuItems()
        {
            prepareMenusLayout();
        }
    }
}