/*
 * Copyright 2013 Primeton Technologies Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.gocom.components.nui.demo;

import org.gocom.components.nui.demo.newdataset.TEmployee;

import com.eos.das.entity.IDASCriteria;
import com.eos.das.entity.criteria.CriteriaType;
import com.eos.foundation.PageCond;
import com.eos.spring.DASDaoSupport;
import commonj.sdo.DataObject;

/**
 * 完成员工信息的增删改查功能的实现
 *
 * @author leijian (mailto:leijian@primeton.com)
 */
public class TEmployeeService extends DASDaoSupport implements ITEmployeeService{
	public void addTEmployee(TEmployee tEmployee){
		getDASTemplate().insertEntity(tEmployee);
	}

	public void deleteTEmployee(TEmployee[] tEmployees ) {
		for(DataObject tEmployee:tEmployees){
			getDASTemplate().deleteEntityCascade(tEmployee);
		}
	}
	
	public void deleteTEmployee(TEmployee tEmployee) {
		getDASTemplate().deleteEntityCascade(tEmployee);
	}
	
	public void getTEmployee(TEmployee tEmployee) {
		getDASTemplate().expandEntity(tEmployee);
	}

	public TEmployee[] queryTEmployees(CriteriaType criteria, PageCond pageCond, String sortField, String sortOrder) {
		criteria.set_entity(TEmployee.QNAME);//载入持久化实体Employee
		if (sortField != null && sortOrder != null) {
			criteria.set("_orderby[1]/_property", sortField);
			criteria.set("_orderby[1]/_sort", sortOrder);
		}
		
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(criteria);
		TEmployee[] results= getDASTemplate().queryEntitiesByCriteriaEntityWithPage(TEmployee.class,
				dasCriteria, pageCond);
		return results;
	}

    public void updateTEmployee(TEmployee tEmployee) {
	    getDASTemplate().updateEntity(tEmployee);
    }

	public int countEmployees(CriteriaType criteria) {
		criteria.set_entity(TEmployee.QNAME);
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(criteria);
		return getDASTemplate().count(dasCriteria);
	}

	public void saveEmployees(TEmployee[] employees) {
		for(TEmployee employee : employees)
		{
			String state = (String)employee.get("_state");
			if ("added".equals(state)) 
			{
				addTEmployee(employee);
			} 
			
			else if ("removed".equals(state) || "deleted".equals(state)) 
			{
				deleteTEmployee(employee);
			} 

			else if ("modified".equals(state)) //更新：_state为空，或modified
			{
				updateTEmployee(employee);
			}
		}
	}

}



