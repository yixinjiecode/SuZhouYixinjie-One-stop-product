<%@page pageEncoding="UTF-8"%>
<%@page import="com.primeton.cap.AppUserManager"%>
<%@page import="com.primeton.cap.TenantManager"%>
<%@page import="com.primeton.cap.auth.manager.AuthRuntimeManager"%>
<%
	String tenantId = TenantManager.getCurrentTenantID();
	String currentSkin = "skin1";
	Object obj = session.getAttribute("skin");
	if(obj != null){
		currentSkin = (String)obj;
	}

	String contextPath=request.getContextPath();
	
	String template = "template1";
	Object obj_template = session.getAttribute("template");
	if(obj_template != null){
		template = (String)obj_template;
	}
	
	
	boolean hasSysThemeState = false;
	String[] sysThemeStates = AuthRuntimeManager.getInstance().getCurrentPartyResAuthState("portal_sysTheme_manager", "function");
	if(sysThemeStates!=null){
		for(int i=0;i<sysThemeStates.length;i++){
			String state = sysThemeStates[i];
			if("1".equals(state)){
				hasSysThemeState = true;
				break;
			}
		}
	}
%>
<script type="text/javascript">
	var currentSkin='<%=currentSkin %>';
	var tenantId="<%=tenantId %>";
	var template="<%=template %>";
	var contextPath="<%=contextPath %>";
	var hasSysThemeState=<%=hasSysThemeState %>;
</script>
