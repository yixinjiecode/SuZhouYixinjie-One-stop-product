<%@page import="com.primeton.cap.TenantManager"%>
<%
	String tenantId = TenantManager.getCurrentTenantID();
	String currentSkin = "skin1";
	Object obj = session.getAttribute("skin");
	if(obj != null){
		currentSkin = (String)obj;
	}
	String contextPath = request.getContextPath();
	
	String currentPath=contextPath+"/manager/skins/"+tenantId+"/"+currentSkin;
	
	String refCss = currentPath + "/manager.css";
	String refImage = currentPath + "/images";
	
	
	String confImage=contextPath+"/skins/"+tenantId+"/config/images";
	
	String template = "template3";
	Object obj_template = session.getAttribute("template");
	if(obj_template != null){
		template = (String)obj_template;
	}
%>

<link rel="stylesheet" type="text/css" href="<%=contextPath%>/css/style-custom.css"/>
<link rel="stylesheet" type="text/css" href="<%=refCss%>"/>
<link rel="stylesheet" type="text/css" href="<%=contextPath%>/skins/<%=tenantId %>/<%=template %>/<%=currentSkin %>//portal/pui-all.css"/>

<script type="text/javascript" src="<%=contextPath%>/js/common_manager.js"></script>

<script type="text/javascript" src="<%=contextPath%>/runtime/integration/pui/resources/jquery.js"></script>
<script type="text/javascript" src="<%=contextPath%>/runtime/integration/pui/pui-core.js"></script>
<script type="text/javascript" src="<%=contextPath%>/runtime/integration/pui/pui-all.js"></script>

<script type="text/javascript" src="<%=contextPath%>/js/portal-base.js"></script>
<script type="text/javascript" src="<%=contextPath%>/js/portal-all.js"></script>

<script type="text/javascript">
	(function(){
		Pui.ns('Portal.app');
		
		Pui.apply(Portal.app,{
			currentPath:'<%=currentPath %>',
			refImage:'<%=refImage %>',
			confImage:'<%=confImage %>'
		});
	})();
</script>