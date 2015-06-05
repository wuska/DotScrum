<%@ Page Title="DotScrum" Language="C#" MasterPageFile="~/dotScrum.Master" AutoEventWireup="true" CodeBehind="loadScrumBoard.aspx.cs" Inherits="dotScrum.Sites.LoadScrumBoard" %>
<%@ MasterType VirtualPath="~/dotScrum.Master" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="bodyContent">
    <div class="popupBlockOuter">
        <div class="popupBlockInner">
            <asp:Panel runat="server" ID="newColumnPopupPanel" Visible="false" CssClass="popupPanelClass">
                <asp:Label runat="server" Text="Please enter new column name:" CssClass="boldedLabel" /> <br />
                <asp:TextBox runat="server" ID="newColumnNameTextBox" /> <br />
                <asp:Button runat="server" ID="newColumnNameAddButton" Text="Add column" OnClick="newColumnNameAddButton_Click" />
                <asp:Button runat="server" ID="newColumnNameCancelButton" Text="Cancel" OnClick="newColumnNameCancelButton_Click" />
            </asp:Panel>
        </div>
    </div>
    <div class="popupBlockOuter">
        <div class="popupBlockInner">
            <asp:Panel runat="server" ID="newUserStoryPopupPanel" Visible="false" CssClass="popupPanelClass">
                <asp:Label runat="server" Text="Please enter new user story data:" CssClass="boldedLabel" /> <br />
                <asp:Label runat="server" Text="Name: " />
                <asp:TextBox runat="server" ID="newUserStoryNameTextBox" /> <br />
                <asp:Label runat="server" Text="Description: " />
                <asp:TextBox runat="server" ID="newUserStoryDescriptionTextBox" /> <br />
                <asp:Label runat="server" Text="Value: " />
                <asp:TextBox runat="server" ID="newUserStoryValueTextBox" /> <br />
                <asp:Button runat="server" ID="newUserStoryAddButton" Text="Add user story" OnClick="newUserStoryAddButton_Click" />
                <asp:Button runat="server" ID="newUserStoryCancelButton" Text="Cancel" OnClick="newUserStoryCancelButton_Click" />
            </asp:Panel>
        </div>
    </div>
    <div class="popupBlockOuter">
        <div class="popupBlockInner">
            <asp:Panel runat="server" ID="newTaskPopupPanel" Visible="false" CssClass="popupPanelClass">
                <asp:Label runat="server" Text="Please fill new task data:" CssClass="boldedLabel" /> <br />
                <asp:Label runat="server" Text="Title:" />
                <asp:TextBox runat="server" ID="newTaskTitleTextBox" /> <br />
                <asp:Label runat="server" Text="Description:" />
                <asp:TextBox runat="server" ID="newTaskDescriptionTextBox" /> <br />
                <asp:Label runat="server" Text="Priority:" />
                <asp:TextBox runat="server" ID="newTaskPriorityTextBox" /> <br />
                <asp:Label runat="server" Text="Value:" />
                <asp:TextBox runat="server" ID="newTaskValueTextBox" /> <br />
                <asp:Label runat="server" Text="User story name:" />
                <asp:DropDownList runat="server" ID="newTaskUserStoryNameList">
                    <asp:ListItem Value="None" Selected="True"></asp:ListItem>
                </asp:DropDownList><br />
                <asp:Button runat="server" ID="newTaskAddButton" Text="Add task" OnClick="newTaskAddButton_Click" />
                <asp:Button runat="server" ID="newTaskCancelButton" Text="Cancel" OnClick="newTaskCancelButton_Click" />
            </asp:Panel>
        </div>
    </div>
    <table class="outerTable">
        <tr class="outerTable">
            <td class="outerTable">
                <asp:Menu ID="projectMenu" runat="server" OnMenuItemClick="projectMenu_MenuItemClick" CssClass="projectMenuBlock">
                    <Items>
                        <asp:MenuItem Text="Add task" Value="addTask"></asp:MenuItem>
                        <asp:MenuItem Text="------" Value="menuSeparator"></asp:MenuItem>
                        <asp:MenuItem Text="Add column" Value="addColumn"></asp:MenuItem>
                        <asp:MenuItem Text="Add user story" Value="addUserStory"></asp:MenuItem>
                    </Items>
                </asp:Menu>
            </td>
            <td class="outerTable">
                <script type="text/javascript" src="scripts/decision_confirmation.js"></script>
                <script type="text/javascript">
                    function allowDrop(ev) {
                        ev.preventDefault();
                    }

                    function drag(ev) {
                        ev.dataTransfer.setData("text", ev.target.id);
                    }

                    function drop(ev) {
                        ev.preventDefault();
                        var data = ev.dataTransfer.getData("text");
                        var droppedTask = document.getElementById(data);
                        ev.target.appendChild(droppedTask);

                        var button = document.getElementById("<%=DragAndDropTrigger.ClientID %>");
                        if (button == null) {
                            return;
                        }

                        var taskAndColumn = data.concat(",");
                        taskAndColumn = taskAndColumn.concat(ev.target.parentNode.id);
                        var taskElement = document.createElement("INPUT");
                        taskElement.type = "hidden";
                        taskElement.name = "taskElement";
                        taskElement.value = taskAndColumn;
                        document.forms[0].appendChild(taskElement);

                        button.click();
                    }
                </script>
                <asp:XmlDataSource runat="server" ID="xmlDataSource" XPath="ScrumBoard/Column" />
                <table class="innerTable">
                    <tr class="innerTable">
                        <asp:Repeater runat="server" ID="scrumBoardColumnRepeater" DataSourceID="xmlDataSource">
                            <ItemTemplate>
                                <td runat="server" class="innerTable" id="taskColumn" ondrop="drop(event)" ondragover="allowDrop(event)">
                                    <div class="columnTitleDiv">
                                        <asp:TextBox runat="server" ID="columnTitleTextBox" Text='<%# XPath("ColumnTitle") %>' OnDataBinding="columnTitleTextBox_DataBinding" OnTextChanged="columnTitleTextBox_TextChanged" AutoPostBack="true" style="width:87%; text-align:center; background-color:transparent; border:none"/>
                                        <asp:Button runat="server" ID="taskColumnsDeleteButton" OnClick="taskColumnsDeleteButton_Click" OnClientClick="confirmDeleteColumn()" Text="X" Font-Bold="true" style="background-color:white; border-style:solid; border-color:blue; border-width:1px; color:red; width:10%" />
                                    </div>
                                    <div class="columnContentDiv">
                                        <p>
                                            <asp:ListView runat="server" ID="columnTaskList">
                                                <ItemTemplate>
                                                    <div runat="server" id="taskBlock" class="taskBlock" draggable="true" ondragstart="drag(event)">
                                                        <asp:Panel runat="server" ID="taskPanel">
                                                            <div class="deleteTaskDiv">
                                                                <asp:Button runat="server" id="deleteTaskButton" OnClick="deleteTaskButton_Click" OnClientClick="confirmDeleteTask()" Text="X" Font-Bold="true" style="position:absolute; top:-10px; left:5px; float:right; background-color:lightyellow; border-style:solid; border-color:blue; border-width:1px; color:red" />
                                                            </div>
                                                            <div class="taskPriorityBlock">
                                                                <asp:Label runat="server" Text="Priority:" Enabled="false" CssClass="labelClass" />
                                                                <asp:TextBox runat="server" ID="taskPriorityTextBox" Text='<%#Eval("Priority") %>' AutoPostBack="true" OnTextChanged="taskPriorityTextBox_TextChanged" OnDataBinding="taskPriorityTextBox_DataBinding" CssClass="taskPriorityInput" />
                                                            </div> <br />
                                                            <asp:TextBox runat="server" id="taskTitleTextBox" Text='<%#Eval("Title") %>' AutoPostBack="true" OnTextChanged="taskTitleTextBox_TextChanged" OnDataBinding="taskTitleTextBox_DataBinding" CssClass="taskTitleInput" /> <br />
                                                            <asp:TextBox runat="server" id="taskDescriptionTextBox" Text='<%#Eval("Description") %>' AutoPostBack="true" OnTextChanged="taskDescriptionTextBox_TextChanged" OnDataBinding="taskDescriptionTextBox_DataBinding" CssClass="taskDescriptionInput" /> <br />
                                                            <div class="taskValueBlock">
                                                                <asp:Label runat="server" Text="Value:" Enabled="false" CssClass="labelClass" />
                                                                <asp:TextBox runat="server" id="taskValueTextBox" Text='<%#Eval("Value") %>' AutoPostBack="true" OnTextChanged="taskValueTextBox_TextChanged" OnDataBinding="taskValueTextBox_DataBinding" CssClass="taskValueInput" /> 
                                                            </div>
                                                            <asp:TextBox runat="server" id="taskUserStoryNameTextBox" Text='<%#Eval("UserStoryName") %>' Visible="false" />
                                                            <asp:DropDownList runat="server" id="taskUserStoryNameList" AutoPostBack="true" OnSelectedIndexChanged="taskUserStoryNameList_SelectedIndexChanged" OnInit="taskUserStoryNameList_Init" OnDataBinding="taskUserStoryNameList_DataBinding" CssClass="taskUserStoryNameList" />
                                                            <br />
                                                        </asp:Panel>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </p>
                                    </div>
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Button runat="server" ID="DragAndDropTrigger" OnClick="DragAndDropTrigger_Click" Text="" CssClass="dragAndDropTrigger" />
</asp:Content>
