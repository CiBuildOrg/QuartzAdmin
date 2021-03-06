<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<QuartzAdmin.web.Models.TriggerFireTimesModel>" %>
    <ul>
    <%
		DateTimeOffset? tempDate = DateTimeOffset.UtcNow;

		DateTimeOffset? lastFireTime = Model.Trigger.GetPreviousFireTimeUtc();
        if (!lastFireTime.HasValue)
        {
            lastFireTime = DateTime.Now.ToUniversalTime();
        }
        tempDate = lastFireTime;    
        
        for (int i = 0; i < 30; i++)
        {
            DateTimeOffset? printDate = null;
            tempDate = Model.Trigger.GetFireTimeAfter(tempDate);
            printDate = tempDate;
                if (Model.Calendar != null && tempDate.HasValue)
                {
                    if (!Model.Calendar.IsTimeIncluded(tempDate.Value))
                    {
                        printDate = null;
                    }
                    
                }
                if (printDate.HasValue)
                {

            %>
            <li><%=printDate.Value.ToLocalTime()%></li>
            <%
				}
        }        

         %>
</ul>


