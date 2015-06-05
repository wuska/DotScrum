<%@ Page Title="DotScrum" Language="C#" MasterPageFile="~/dotScrum.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="dotScrum.Default" %>
<%@ MasterType VirtualPath="~/dotScrum.Master" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="bodyContent">
    <table>
        <tr>
            <td>
                <asp:Menu ID="projectMenu" runat="server" OnMenuItemClick="projectMenu_MenuItemClick" CssClass="projectMenuBlock">
                    <Items>
                        <asp:MenuItem Text="Add user story" Value="addUserStory"></asp:MenuItem>
                        <asp:MenuItem Text="Add sprint" Value="addSprint"></asp:MenuItem>
                    </Items>
                </asp:Menu>
            </td>
            <td>
                <div class="projectTitleBlock">
                    <asp:TextBox runat="server" ID="projectNameTextBox" CssClass="projectNameTextBox" AutoPostBack="true" OnTextChanged="projectNameTextBox_TextChanged"></asp:TextBox>
                </div>
                <div>
                    <script type="text/javascript">
                        function allowDrop(ev) {
                            ev.preventDefault();
                        }

                        function drag(ev) {
                            ev.dataTransfer.setData("text", String(ev.target.id));
                        }

                        var cumulativeOffset = function (element) {
                            var top = 0;
                            do {
                                top += element.offsetTop || 0;
                                element = element.offsetParent;
                            } while (element);

                            return top;
                        };

                        function drop(event) {
                            event.preventDefault();

                            var button = document.getElementById("<%=DragAndDropTrigger.ClientID %>");
                            if (button == null) {
                                return;
                            }

                            var mouseY = event.clientY;
                            var target_id = "";
                            var backlogItemY = 0;
                            var backlogItemHeight = 0;
                            var parentFound = false;
                            var directionToPasteTheItem = "BEFORE"
                            target_id = target_id.concat(String(event.target.id));
                            if (target_id.search("backlogItemBlock") != -1) {
                                //dropped into backlogItemBlock
                                parentFound = true;
                                backlogItemY = cumulativeOffset(event.target);
                                backlogItemHeight = document.getElementById(target_id).clientHeight;
                            } else {
                                //find parent that contains "backlogItemBlock"
                                var parent = event.target.parentNode;//$(ev.target).parent();
                                while (parent != null) {
                                    target_id = String(parent.id);
                                    var targer_id_serarch_res = target_id.search("backlogItemBlock");
                                    if (targer_id_serarch_res == -1) {
                                        parent = parent.parentNode;
                                        continue;
                                    }

                                    backlogItemY = cumulativeOffset(parent);
                                    backlogItemHeight = document.getElementById(target_id).clientHeight;
                                    parentFound = true;

                                    if (mouseY > ((backlogItemHeight / 2) + backlogItemY)) {
                                        directionToPasteTheItem = "AFTER";
                                    }
                                    break;
                                }
                            }

                            var dataToTransfer = String(event.dataTransfer.getData("text"));
                            if (parentFound == false) {
                                //item na pierwsza pozycje
                                backlogItemY = 0;
                            }

                            dataToTransfer = dataToTransfer.concat(",");
                            if (backlogItemY == 0) {
                                dataToTransfer = dataToTransfer.concat("AS_FIRST_USER_STORY");
                            } else {
                                dataToTransfer = dataToTransfer.concat(parent.id);
                            }
                            dataToTransfer = dataToTransfer.concat(",");
                            dataToTransfer = dataToTransfer.concat(directionToPasteTheItem);

                            var usData = document.createElement("INPUT");
                            usData.type = "hidden";
                            usData.name = "dataToTransfer";
                            usData.value = dataToTransfer;
                            document.forms[0].appendChild(usData);

                            button.click();
                        }
                    </script>
                    <script type="text/javascript">
                        function backlogItemBlock_mouseover(event) {
                            for (var i = 0; i < event.currentTarget.childNodes.length; i++) {
                                var node = event.currentTarget.childNodes[i];
                                var nodeName = String(node.nodeName);
                                if (nodeName.search("TEXTAREA") == -1) {
                                    continue;
                                }

                                var childId = node.id;
                                if (String(childId).search("StoryDescription") != -1) {
                                    event.currentTarget.childNodes[i].style.width = "99%";
                                    event.currentTarget.childNodes[i].style.height = "auto";
                                    return;
                                }
                            }
                        }

                        function backlogItemBlock_mouseout(event) {
                            for (var i = 0; i < event.currentTarget.childNodes.length; i++) {
                                var node = event.currentTarget.childNodes[i];
                                var nodeName = String(node.nodeName);
                                if (nodeName.search("TEXTAREA") == -1) {
                                    continue;
                                }

                                var childId = node.id;
                                if (String(childId).search("StoryDescription") != -1) {
                                    event.currentTarget.childNodes[i].style.width = "0px";
                                    event.currentTarget.childNodes[i].style.height = "0px";
                                    return;
                                }
                            }
                        }
                    </script>
                    <asp:Label runat="server" CssClass="backlogLabel">Backlog:</asp:Label>
                    <asp:Panel runat="server" ID="backlogListPanel" CssClass="backlogListPanel">
                        <div ondrop="drop(event)" ondragover="allowDrop(event)">
                            <asp:ListView runat="server" ID="backlogList">
                                <ItemTemplate>
                                    <div runat="server" id="backlogItemBlock" class="usBacklogItemBlock" draggable="true" ondragstart="drag(event)" ondrop="drop(event)" ondragover="allowDrop(event)" onmouseover="backlogItemBlock_mouseover(event)" onmouseout="backlogItemBlock_mouseout(event)">
                                        <asp:TextBox runat="server" ID="userStoryTitleTextBox" Text='<%#Eval("Name") %>' AutoPostBack="true" OnTextChanged="userStoryTitleTextBox_TextChanged" OnDataBinding="userStoryTitleTextBox_DataBinding" CssClass="usTitleTextBox" />
                                        <asp:TextBox runat="server" ID="userStoryDescription" Text='<%#Eval("Description") %>' AutoPostBack="true" OnTextChanged="userStoryDescription_TextChanged" OnDataBinding="userStoryDescription_DataBinding" TextMode="MultiLine" Wrap="true" CssClass="usDescriptionTextBox" Width="0px" Height="0px" />
                                        <asp:Label runat="server">Value: </asp:Label>
                                        <asp:TextBox runat="server" ID="userStoryValue" Text='<%#Eval("Value") %>' AutoPostBack="true" OnTextChanged="userStoryValue_TextChanged" OnDataBinding="userStoryValue_DataBinding" CssClass="usValueTextBox" />
                                        <asp:Button runat="server" ID="deleteUserStory" Text="Delete" OnClick="deleteUserStory_Click" />
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </asp:Panel>
                    <asp:Label runat="server" CssClass="backlogLabel">Backlog of done user stories:</asp:Label> <br />
                    <asp:ListView runat="server" ID="doneBacklogList">
                        <ItemTemplate>
                            <div runat="server" id="doneBacklogItemBlock" class="usBacklogItemBlock" onmouseover="backlogItemBlock_mouseover(event)" onmouseout="backlogItemBlock_mouseout(event)">
                                <asp:TextBox runat="server" ID="doneUserStoryTitleTextBox" Text='<%#Eval("Name") %>' AutoPostBack="true" OnTextChanged="userStoryTitleTextBox_TextChanged" OnDataBinding="userStoryTitleTextBox_DataBinding" CssClass="usTitleTextBox" /> <br />
                                <asp:TextBox runat="server" ID="doneUserStoryDescription" Text='<%#Eval("Description") %>' AutoPostBack="true" OnTextChanged="userStoryDescription_TextChanged" OnDataBinding="userStoryDescription_DataBinding" TextMode="MultiLine" Wrap="true" CssClass="usDescriptionTextBox" Width="0px" Height="0px" />
                                <asp:Label runat="server">Value: </asp:Label>
                                <asp:TextBox runat="server" ID="doneUserStoryValue" Text='<%#Eval("Value") %>' AutoPostBack="true" OnTextChanged="userStoryValue_TextChanged" OnDataBinding="userStoryValue_DataBinding" CssClass="usValueTextBox" />
                                <asp:Button runat="server" ID="doneDeleteUserStory" Text="Delete" OnClick="doneDeleteUserStory_Click" />
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </td>
        </tr>
    </table>
    <asp:Button runat="server" ID="DragAndDropTrigger" OnClick="DragAndDropTrigger_Click" Text="" CssClass="dragAndDropTrigger" />
</asp:Content>
