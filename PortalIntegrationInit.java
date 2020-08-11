package org.gocom.portal.integration.init;

import javax.servlet.FilterChain;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import org.gocom.components.ecm.common.sso.GenerateTicket;

import com.eos.access.http.OnlineUserManager;
import com.eos.data.datacontext.DataContextManager;
import com.eos.data.datacontext.IMUODataContext;
import com.eos.data.datacontext.IUserObject;
import com.primeton.cap.ISystemConstants;
import com.primeton.cap.party.Party;
import com.primeton.cap.party.manager.PartyManagerServiceLoader;
import com.primeton.cap.party.manager.PartyRuntimeManager;
import com.primeton.cap.portal.core.spi.auth.IAuthConstants;
import com.primeton.cap.portal.runtime.integration.PortalLoginInit;
import com.primeton.ext.common.muo.MUODataContextHelper;

import edu.yale.its.tp.cas.client.IContextInit;

public class PortalIntegrationInit implements IContextInit {

	public String getTranslatorUser(String userId) {
		Party userParty = PartyRuntimeManager.getInstance().getPartyByPartyID(userId, IAuthConstants.USER_PARTY_TYPE_ID);
		if (userParty == null) {
			return null;
		} else {
			return userId;
		}
	}

	public void initContext(ServletRequest request, ServletResponse response, FilterChain fc, String userId) {
		HttpServletRequest servletRequest = (HttpServletRequest) request;
		HttpSession session = servletRequest.getSession();
		if (userId == null) {
			throw new RuntimeException("Illegal user");
		}
		String tenantId = String.valueOf(getTenantID(servletRequest));
		IUserObject userObject = PartyManagerServiceLoader.getCurrentPartyUserInitService().initUserObject(userId, tenantId);

		PortalLoginInit.init(userId, session, tenantId);

		DataContextManager.current().setMapContextFactory(new com.primeton.ext.access.http.HttpMapContextFactory(request, response));
		OnlineUserManager.login(userObject);
		session.setAttribute(IUserObject.KEY_IN_CONTEXT, userObject);
		IMUODataContext muo = MUODataContextHelper.create(session);
		DataContextManager.current().setMUODataContext(muo);

		// 初始化ECM Ticket到session中
		userObject.getAttributes().put("ticket", GenerateTicket.generate(request, response));
	}

	private String getTenantID(HttpServletRequest servletRequest) {
		// TODO 根据二级url拿到租户信息
		return ISystemConstants.DEFAULT_TENANT_ID;
	}
}