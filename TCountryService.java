/*
 * Copyright 2013 Primeton.
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
import org.gocom.components.nui.demo.newdataset.TCountry;


import commonj.sdo.DataObject;

/**
 * TODO 实现Demo中涉及到"国家"数据的增删改查功能
 *
 * @author huangct (mailto:huangct@primeton.com)
 */
public class TCountryService extends DASDaoSupport implements ITCountryService{
	public void addTCountry(TCountry tCountry){
		getDASTemplate().insertEntity(tCountry);
	}

	public void deleteTCountry(TCountry[] tCountrys ){
		for(DataObject tCountry:tCountrys){
			getDASTemplate().deleteEntityCascade(tCountry);
		}
	}


	public void getTCountry(TCountry tCountry){
		getDASTemplate().expandEntity(tCountry);
	}

    /**
     * 功能描述：根据查询条件和分页条件进行国家信息查询。
	 * @ param criteriaType 查询条件
	 * @ param pageCond     分页条件
	 * @ return Emp 符合条件的国家一览信息
     */	
	public TCountry[] queryTCountrys(CriteriaType criteria,PageCond page,String sortField,String sortOrder){
		criteria.set_entity(TCountry.QNAME);
		if (sortField != null && sortOrder != null) {
			criteria.set("_orderby[1]/_property", sortField);
			criteria.set("_orderby[1]/_sort", sortOrder);
		}
		TCountry[] countrys = null;
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(criteria);
		countrys = getDASTemplate().queryEntitiesByCriteriaEntityWithPage(
				TCountry.class, dasCriteria, page);
		return countrys;
	}


    public void updateTCountry(TCountry tCountry){
	    getDASTemplate().updateEntity(tCountry);
    }

	public int countTCountrys(CriteriaType criteria) {
		criteria.set_entity(TCountry.QNAME);
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(criteria);
		return getDASTemplate().count(dasCriteria);
	}
    
}


