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

import com.eos.das.entity.IDASCriteria;
import com.eos.das.entity.criteria.CriteriaType;
import com.eos.foundation.PageCond;
import com.eos.spring.DASDaoSupport;
import org.gocom.components.nui.demo.newdataset.TDepartment;

import commonj.sdo.DataObject;

/**
 * 完成部门信息的增删改查功能的实现
 *
 * @author leijian (mailto:leijian@primeton.com)
 */
public class TDepartmentService extends DASDaoSupport implements ITDepartmentService{
	public void addTDepartment(TDepartment tDepartment){
		getDASTemplate().insertEntity(tDepartment);
	}

	public void deleteTDepartment(TDepartment[] tDepartments ){
		for(DataObject tDepartment:tDepartments){
			getDASTemplate().deleteEntityCascade(tDepartment);
		}
	}

	public void getTDepartment(TDepartment tDepartment){
		getDASTemplate().expandEntity(tDepartment);
	}

	public TDepartment[] queryTDepartments(CriteriaType criteria, PageCond pageCond, String sortFilder, String sortOrder) {
		criteria.set_entity(TDepartment.QNAME);
		if (sortFilder !=null && sortOrder != null) {
			criteria.set("_orderby[1]/_property", sortFilder);
			criteria.set("_orderby[1]/_sort", sortOrder);
		}
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(criteria);
		TDepartment[] results= getDASTemplate().queryEntitiesByCriteriaEntityWithPage(TDepartment.class,dasCriteria, pageCond);
		return results;
	}

	public int countTDepartments(CriteriaType criteria) {
		criteria.set_entity(TDepartment.QNAME);
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(criteria);
		return getDASTemplate().count(dasCriteria);
		
	}

	public void updateTDepartment(TDepartment tDepartment) {
		getDASTemplate().updateEntity(tDepartment);
	}

	public void updateTDepartments(TDepartment[] tDepartments) {
		getDASTemplate().updateEntityBatch(tDepartments);
	}

}



