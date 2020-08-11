<%@page import="com.primeton.cap.TenantManager"%>

<%
	String tenantId = TenantManager.getCurrentTenantID();
	String currentSkin = "skin1";
	Object obj = session.getAttribute("skin");
	if(obj != null){
		currentSkin = (String)obj;
	}
	String contextPath = request.getContextPath();
	String refCss = contextPath + "/runtime/skins/" + tenantId + "/" + currentSkin + "/runtime.css";
	
%>


<link rel="stylesheet" type="text/css" href="<%=refCss%>"/>

<script type="text/javascript">
	var contextPath = "<%=contextPath%>";
</script>

<script type="text/javascript"  src="<%=contextPath%>/runtime/integration/pui/resources/jquery.js" ></script>
<script type="text/javascript"  src="<%=contextPath%>/runtime/integration/pui/pui-core.js" ></script>
<script type="text/javascript"  src="<%=contextPath%>/runtime/integration/pui/pui-all.js" ></script>
<script type="text/javascript"  src="<%=contextPath%>/runtime/integration/pui/resources/jquery.js" ></script>
<script type="text/javascript"  src="<%=contextPath%>/js/portal-base.js"></script>
<script type="text/javascript"  src="<%=contextPath%>/js/portal-all.js"></script>



