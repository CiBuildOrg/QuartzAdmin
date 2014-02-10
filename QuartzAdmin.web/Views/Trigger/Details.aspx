<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<QuartzAdmin.web.Models.TriggerFireTimesModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        <p>
            Name:
            <%= Html.Encode(Model.Trigger.Key.Name) %>
        </p>
        <p>
            Group:
            <%= Html.Encode(Model.Trigger.Key.Group) %>
        </p>
        <p>
            JobName:
            <%= Html.Encode(Model.Trigger.JobKey.Name) %>
        </p>
        <p>
            JobGroup:
            <%= Html.Encode(Model.Trigger.JobKey.Group) %>
        </p>
        <p>
            FullName:
            <%= Html.Encode(Model.Trigger.GetType().FullName) %>
        </p>
        <p>
            FullJobName:
            <%= Html.Encode("Model.Trigger.FullJobName") %>
        </p>
        <p>
            Description:
            <%= Html.Encode(Model.Trigger.Description) %>
        </p>
        <p>
            CalendarName:
            <%= !string.IsNullOrWhiteSpace(Model.Trigger.CalendarName) ? Html.ActionLink(Model.Trigger.CalendarName, "Details", "Calendar", new { instanceName=Url.Encode(Model.Instance.InstanceName), itemName = Url.Encode(Model.Trigger.CalendarName) }, null) : MvcHtmlString.Create("None")%>
        </p>
        <p>
            Concurrent Execution Disallowed:
            <%= Html.Encode("Model.Trigger.ConcurrentExecutionDisallowed") %>
        </p>
        <p>
            FinalFireTimeUtc:
            <%= Html.Encode(String.Format("{0:g}", Model.Trigger.FinalFireTimeUtc)) %>
        </p>
        <p>
            MisfireInstruction:
            <%= Html.Encode(Model.Trigger.MisfireInstruction) %>
        </p>
        <p>
            FireInstanceId:
            <%= Html.Encode("Model.Trigger.FireInstanceId") %>
        </p>
        <p>
            EndTimeUtc:
            <%= Html.Encode(String.Format("{0:g}", Model.Trigger.EndTimeUtc)) %>
        </p>
        <p>
            StartTimeUtc:
            <%= Html.Encode(String.Format("{0:g}", Model.Trigger.StartTimeUtc)) %>
        </p>
        <p>
            HasMillisecondPrecision:
            <%= Html.Encode(Model.Trigger.HasMillisecondPrecision) %>
        </p>
        <p>
            Priority:
            <%= Html.Encode(Model.Trigger.Priority) %>
        </p>
        <p>
            HasAdditionalProperties:
            <%= Html.Encode(Model.Trigger.JobDataMap.Count > 0) %>
        </p>
        <p>
            CRON:
            <%if (Model.Trigger is Quartz.ICronTrigger)
              {
				  Quartz.ICronTrigger cronTrig = (Quartz.ICronTrigger)Model.Trigger;
                  %>
                  Expression:  <%=cronTrig.CronExpressionString%> <br />
                  Translated:  <%=cronTrig.GetExpressionSummary() %> <br />
                  <%
              }
                 %>
        </p>
    </fieldset>
    <p>
        <%=Html.ActionLink("Back to List", "Connect", "Instance", new { id = Model.Instance.InstanceName }, null)%>
        <a href="javascript:void(0)" id="showFireTimes" name="showFireTimes">Show Fire Times</a>
    </p>
    
<div id="panelFireTimes">
	<div class="hd">Fire Times for triggger</div>
	<div class="bd">
	    <%Html.RenderPartial("FireTimesPartial", Model); %>
	</div>
	<div class="ft"></div>
</div>    


<script type="text/javascript">
    YAHOO.namespace("bdr");
    YAHOO.util.Event.addListener(window, "load", function() {
    YAHOO.bdr.panel1 = new YAHOO.widget.Panel("panelFireTimes", { width: "320px", visible: false, constraintoviewport: true, fixedcenter: true });
        YAHOO.bdr.panel1.render();
        YAHOO.util.Event.addListener("showFireTimes", "click", YAHOO.bdr.panel1.show, YAHOO.bdr.panel1, true);
    });
</script>
</asp:Content>

