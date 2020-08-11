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
import com.eos.spring.DASDaoSupport;
import org.gocom.components.nui.demo.newdataset.TEducational;

import commonj.sdo.DataObject;

/**
 * 完成个人学历的增删改查功能的实现
 *
 * @author leijian (mailto:leijian@primeton.com)
 */
public class TEducationalService extends DASDaoSupport implements ITEducationalService{
	public void addTEducational(TEducational tEducational){
		getDASTemplate().insertEntity(tEducational);
	}

	public void deleteTEducational(TEducational[] tEducationals ){
		for(DataObject tEducational:tEducationals){
			getDASTemplate().deleteEntityCascade(tEducational);
		}
	}


	public void getTEducational(TEducational tEducational){
		getDASTemplate().expandEntity(tEducational);
	}


	public TEducational[]  queryTEducationals(CriteriaType criteria){
		criteria.set_entity(TEducational.QNAME);
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(criteria);
		TEducational[] results = getDASTemplate().queryEntitiesByCriteriaEntity(TEducational.class,dasCriteria);
		return results;
	}


    public void updateTEducational(TEducational tEducational){
	    getDASTemplate().updateEntity(tEducational);
    }

}


