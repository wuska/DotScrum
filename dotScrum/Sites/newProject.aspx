<%@ Page Title="DotScrum" Language="C#" MasterPageFile="~/dotScrum.Master" AutoEventWireup="true" CodeBehind="newProject.aspx.cs" Inherits="dotScrum.Sites.NewProject" %>
<%@ MasterType VirtualPath="~/dotScrum.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="bodyContent" runat="server">
    <table class="outerTable">
        <tr class="outerTable">
            <td class="outerTable">
                <asp:Menu ID="projectMenu" runat="server" OnMenuItemClick="projectMenu_MenuItemClick" CssClass="projectMenuBlock">
                    <Items>
                        <asp:MenuItem Text="Add user story" Value="addUserStory"></asp:MenuItem>
                    </Items>
                </asp:Menu>
            </td>
            <td class="outerTable">
                <div class="projectTitleBlock">
                    <asp:TextBox runat="server" ID="projectNameTextBox" Font-Bold="true" Font-Size="Large" AutoPostBack="true" OnTextChanged="projectNameTextBox_TextChanged">New project name</asp:TextBox>
                </div> <br />
                <div>
                    <asp:Label runat="server" Font-Bold="true">Backlog:</asp:Label> <br />
                    <asp:ListView runat="server" ID="backlogList">
                        <ItemTemplate>
                            <div runat="server" id="backlogItemBlock" class="backlogItemBlock">
                                <asp:TextBox runat="server" ID="userStoryTitleTextBox" Text='<%#Eval("Priority") %>' AutoPostBack="true" CssClass="userStoryTitleTextBox" />
                                <asp:TextBox runat="server" ID="userStoryValue" Text='<%#Eval("Priority") %>' AutoPostBack="true" CssClass="userStoryValueTextBox" />
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
