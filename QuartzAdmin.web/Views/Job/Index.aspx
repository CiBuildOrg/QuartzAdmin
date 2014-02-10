<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Quartz.IJobDetail>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Current Status</h3>
    <%
        ViewDataDictionary vdd = new ViewDataDictionary();
        vdd.Add("instanceName", ViewData["instanceName"]);
        Html.RenderPartial("~/Views/Group/CurrentStatus.ascx", vdd);
     %>


    <h2>Index</h2>

    <table>
        <tr>
            <th></th>
            <th>
                Name
            </th>
            <th>
                Group
            </th>
            <th>
                FullName
            </th>
            <th>
                Description
            </th>
            <th>
                RequestsRecovery
            </th>
            <th>
                Volatile
            </th>
            <th>
                Durable
            </th>
            <th>
                Stateful
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.ActionLink("Edit", "Edit", new { instanceName = ViewData["instanceName"], groupName = item.Key.Group, itemName = item.Key.Name })%> |
                <%= Html.ActionLink("Details", "Details", new { instanceName=ViewData["instanceName"], groupName=item.Key.Group, itemName=item.Key.Name })%>
            </td>
            <td>
                <%= Html.Encode(item.Key.Name) %>
            </td>
            <td>
                <%= Html.Encode(item.Key.Group) %>
            </td>
            <td>
                <%= Html.Encode(item.JobType.FullName) %>
            </td>
            <td>
                <%= Html.Encode(item.Description) %>
            </td>
            <td>
                <%= Html.Encode(item.RequestsRecovery) %>
            </td>
            <td>
                <%= Html.Encode(item.PersistJobDataAfterExecution) %>
            </td>
            <td>
                <%= Html.Encode(item.Durable) %>
            </td>
            <td>
                <%= Html.Encode(item.ConcurrentExecutionDisallowed) %>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

